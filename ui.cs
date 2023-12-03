using Godot;
using System;

public partial class ui : CanvasLayer
{
	[Signal]
	public delegate void AbandonRunEventHandler();
	[Signal]
	public delegate void QuitToDesktopEventHandler();

	[Export]
	public ObjectiveManager ObjectiveManager { set; get; }

	public PlayerCharacter Player { set; get; }
	public PauseMenu PauseMenu { set; get; }
	public ObjectiveTracker ObjectiveTracker;

	private PlayerInfoTracker _playerInfo;
	private Resource _mousePointer = ResourceLoader.Load("res://assets/CrosshairCenter.png");
	private CrosshairController _crosshair;
	private TextureRect _crosshairSpread;
	private Vector2 _mousePosition;
	private Vector2 _mouseVelocity;
	private double _weaponStability;
	private Label _mcguffinsCounter;
	private Label _mcguffinsMax;
	private Panel _extractNotif;
	//private PlayerDeathPanel _playerDeath;

	public override void _Ready()
	{
		_playerInfo = GetNode<PlayerInfoTracker>("%CharacterInfoTracker");
		_crosshair = GetNode<CrosshairController>("%Crosshair");
		PauseMenu = GetNode<PauseMenu>("%PauseMenu");
		ObjectiveTracker = GetNode<ObjectiveTracker>("%ObjectiveTracker");
		//_playerDeath = GetNode<PlayerDeathPanel>("%PlayerDeathPanel");

		_crosshair.Player = Player;

		Input.MouseMode = Input.MouseModeEnum.Hidden;

		ObjectiveManager.CollectableCollected += ObjectiveTracker.OnObjectiveManagerCollectibleCollected;
		ObjectiveManager.AllCollectableCollected += ObjectiveTracker.OnObjectiveManagerAllCollectableCollected;
		ObjectiveManager.ExtractionsActivated += ObjectiveTracker.OnObjectiveManagerExtractionsActivated;

		PauseMenu.AbandonRun += OnPauseMenuAbandonRun;
		PauseMenu.QuitToDesktop += OnPauseMenuQuitToDesktopToDesktop;
	}

	public void SetPlayerInfoHealth(int currentHealth, int maxHealth)
	{
		_playerInfo.SetHealth(currentHealth, maxHealth);
	}

	public void SetPlayerInfoAmmo(int currentAmmo, int maxAmmo)
	{
		_playerInfo.SetAmmo(currentAmmo, maxAmmo);
	}

	private void OnPauseMenuAbandonRun()
	{
		EmitSignal(SignalName.AbandonRun);
	}

	private void OnPauseMenuQuitToDesktopToDesktop()
	{
		EmitSignal(SignalName.QuitToDesktop);
	}

	private void OnMcguffinCollected(int currentCount, int maxCount)
	{
		_mcguffinsCounter.Text = currentCount.ToString();
		_mcguffinsMax.Text = maxCount.ToString();
	}

	private void OnAllMcguffinsCollected()
	{
		_extractNotif.Visible = true;
	}

	public override void _Process(double delta)
	{
		UpdateCrosshair();
	}

	private void UpdateCrosshair()
	{
		_crosshair.UpdateSpreadScale((float)_weaponStability);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			_mousePosition = eventMouseMotion.Position;
			_mouseVelocity = eventMouseMotion.Velocity;
		}
	}

	public void PlayerDied(int moneyLost)
	{
		PackedScene playerDeath = GD.Load<PackedScene>("res://player_death_panel.tscn");
		PlayerDeathPanel deathPanel = playerDeath.Instantiate<PlayerDeathPanel>();

		AddChild(deathPanel);

		Input.MouseMode = Input.MouseModeEnum.Visible;

		deathPanel.Visible = true;
		deathPanel.MoneyLost = moneyLost;

		_playerInfo.Visible = false;
		_crosshair.Visible = false;
		ObjectiveTracker.Visible = false;
	}

	public void PlayerWon(int moneyWon)
	{
		PackedScene playerWin = GD.Load<PackedScene>("res://player_win_panel.tscn");
		PlayerDeathPanel winPanel = playerWin.Instantiate<PlayerDeathPanel>();

		AddChild(winPanel);

		Input.MouseMode = Input.MouseModeEnum.Visible;

		winPanel.Visible = true;
		winPanel.MoneyLost = moneyWon;

		_playerInfo.Visible = false;
		_crosshair.Visible = false;
		ObjectiveTracker.Visible = false;
	}

	public void OnPlayerHealthChange(int health, int _, int maxHealth)
	{
		_playerInfo.SetHealth(health, maxHealth);
	}

	public void OnWeaponAmmoChange(int ammo, int _, int maxAmmo)
	{
		_playerInfo.SetAmmo(ammo, maxAmmo);
	}

	private void OnWeaponStability(double stability)
	{
		_weaponStability = stability;
	}
}
