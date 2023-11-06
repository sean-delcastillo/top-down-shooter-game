using Godot;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

public partial class World : Node3D
{
	[Export]
	private CameraController _cameraRig;
	[Export]
	private PlayerCharacterController _player;
	private Button _returnButton;
	private ui _ui;
	private bool _isPaused;
	private ObjectiveManager _objectiveManager;
	private AudioStreamPlayer _audio;

	public override void _Ready()
	{
		_ui = GetNode<ui>("UI");
		_objectiveManager = GetNode<ObjectiveManager>("ObjectiveManager");
		_audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		_cameraRig.MousePositionInWorldChanged += SendNewMousePositionToPlayer;
		_objectiveManager.AllMcguffinsCollected += OnObjectiveManagerAllMcguffinsCollected;
		_objectiveManager.ExtractionEntered += OnExtractExtractionEntered;
		InitUI();

		_ui.SaveAndQuit += SaveAndQuit;
		_ui.Quit += Quit;

		player_prefs playerPrefs = GetNode<player_prefs>("/root/PlayerPrefs");
		_audio.VolumeDb = playerPrefs.VolumeValue;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Pause"))
		{
			if (GetTree().Paused == false)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
				GetTree().Paused = true;
				_ui.PauseMenu.Show();
			}
		}
	}

	private void InitUI()
	{
		_ui.Set(ui.PropertyName.Player, _player);
		_ui.Call(ui.MethodName.Init);
		_player.Call(PlayerCharacterController.MethodName.RegisterWithUi);
	}

	private void SendNewMousePositionToPlayer(Vector3 position)
	{
		_player.Call("UpdateMousePositionInWorld", position);
	}

	private void OnObjectiveManagerAllMcguffinsCollected()
	{
		GD.Print("ALL MCGUFFINS COLLECTED. EXTRACTION UNLOCKED.");
		ExtractionPoint extract = _objectiveManager.ActivateRandomExtraction();
	}

	private void OnExtractExtractionEntered()
	{
		GD.Print("EXTRACTION ENTERED!");
		GetTree().ChangeSceneToFile("main_menu.tscn");
	}

	public void Save()
	{
		var packedScene = new PackedScene();
		var sceneNode = GetTree().CurrentScene;
		packedScene.Pack(sceneNode);
		ResourceSaver.Save(packedScene, "res://SAVEGAMESCENE.tscn");
	}

	public void Quit()
	{
		GetTree().Quit();
	}

	public void SaveAndQuit()
	{
		Save();
		Hide();
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("main_menu.tscn");
	}
}
