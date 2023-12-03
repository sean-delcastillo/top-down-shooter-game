using System.Collections.Generic;
using Godot;

/// <summary>
/// The entry point for gameplay levels for TopDownGame. Should be set as the script for a gameplay level's root node. Initializes context for the rest of nodes.
/// </summary>
public partial class GameplayMain : Node3D
{
	private CameraController _cameraRig;
	private PlayerCharacter _player;
	private ui _UI;
	private ObjectiveManager _objectiveManager;
	private AudioStreamPlayer _music;

	private readonly Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> objectiveValues = new()
	{
		{CollectableObjective.ObjectiveType.GoldBall, 100},
		{CollectableObjective.ObjectiveType.RedBall, 25},
		{CollectableObjective.ObjectiveType.GreenBall, 10}
	};

	public override void _Ready()
	{
		_cameraRig = GetNode<CameraController>("%Camera");
		_player = GetNode<PlayerCharacter>("%Player");
		_UI = GetNode<ui>("%UI");
		_objectiveManager = GetNode<ObjectiveManager>("%ObjectiveManager");
		_music = GetNode<AudioStreamPlayer>("%MusicPlayer");
		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");

		_player.CharacterInformation.MovementSpeed = progression.CurrentSpeed;
		_player.CharacterInformation.Health = progression.CurrentHealth;
		_player.CharacterInformation.MaxHealth = progression.CurrentHealth;

		// Inject Player's camera dependency
		_player.CurrentCamera = _cameraRig.Camera;
		_player.PlayerDied += OnPlayerPlayerDied;

		// Initialize the UI with Player information
		_UI.SetPlayerInfoHealth((int)_player.CharacterInformation.Health, (int)_player.CharacterInformation.Health);
		_UI.SetPlayerInfoAmmo(_player.Weapon.MagazineSize, _player.Weapon.MagazineSize);

		_UI.Player = _player;

		// Connect UI to Player's health and ammo change signals
		_player.HealthChange += _UI.OnPlayerHealthChange;
		_player.Weapon.AmmoChange += _UI.OnWeaponAmmoChange;

		_player.PlayerExtracted += OnPlayerExtracted;

		_UI.ObjectiveTracker.ObjectiveCounts = _objectiveManager.ObjectiveCounts;
		_UI.ObjectiveTracker.ExtractionStates = _objectiveManager.ExtractionStates;

		_UI.PauseMenu.AbandonRun += OnPauseMenuAbandonRun;
		_UI.PauseMenu.QuitToDesktop += OnPauseMenuQuitToDesktop;

		PlayerPrefs prefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
		_music.VolumeDb = prefs.VolumeValue;

	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Pause"))
		{
			if (GetTree().Paused == false)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
				GetTree().Paused = true;
				_UI.PauseMenu.Show();
			}
		}
	}

	private void OnPlayerPlayerDied(Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> collectedObjectiveCounts)
	{
		_cameraRig.CameraTarget = null;

		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");

		Persistence.SaveToSaveFile(progression.Save());

		_UI.PlayerDied(CalculateMoneyFromLoot(collectedObjectiveCounts));
	}

	private int CalculateMoneyFromLoot(Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> objectiveCounts)
	{
		int value = 0;

		foreach (KeyValuePair<CollectableObjective.ObjectiveType, int> objcount in objectiveCounts)
		{
			switch (objcount.Key)
			{
				case CollectableObjective.ObjectiveType.GoldBall:
					value += objectiveValues.GetValueOrDefault(CollectableObjective.ObjectiveType.GoldBall) * objcount.Value;
					break;
				case CollectableObjective.ObjectiveType.RedBall:
					value += objectiveValues.GetValueOrDefault(CollectableObjective.ObjectiveType.RedBall) * objcount.Value;
					break;
				case CollectableObjective.ObjectiveType.GreenBall:
					value += objectiveValues.GetValueOrDefault(CollectableObjective.ObjectiveType.GreenBall) * objcount.Value;
					break;
				default:
					break;
			}
		}

		return value;
	}

	private void OnPlayerExtracted(Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> objectiveCounts)
	{
		int moneyFromRun = CalculateMoneyFromLoot(objectiveCounts);

		_UI.PlayerWon(moneyFromRun);

		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");
		progression.Money += moneyFromRun;

		if (moneyFromRun > progression.MostMoneyInOneRun)
		{
			progression.MostMoneyInOneRun = moneyFromRun;
		}

		Persistence.SaveToSaveFile(progression.Save());
	}

	private void OnPauseMenuAbandonRun()
	{
		QuitToMainMenu();
	}

	private void OnPauseMenuQuitToDesktop()
	{
		QuitToDesktop();
	}

	private void QuitToMainMenu()
	{
		GetTree().ChangeSceneToFile("res://main_menu.tscn");
	}

	private void QuitToDesktop()
	{
		GetTree().Quit();
	}
}
