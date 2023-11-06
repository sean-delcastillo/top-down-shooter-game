using Godot;
using System;
using System.Security.Cryptography;

public partial class ui : CanvasLayer
{
	[Signal]
	public delegate void SaveAndQuitEventHandler();
	[Signal]
	public delegate void QuitEventHandler();

	public PlayerCharacterController Player { set; get; }
	public PauseMenu PauseMenu { set; get; }

	private Label _currentAmmo;
	private Label _maxAmmo;
	private Label _currentHealth;
	private Label _maxHealth;
	private Resource _mousePointer = ResourceLoader.Load("res://assets/CrosshairCenter.png");
	private CrosshairController _crosshair;
	private TextureRect _crosshairSpread;
	private Vector2 _mousePosition;
	private Vector2 _mouseVelocity;
	private double _weaponStability;
	private ObjectiveManager _objectiveManager;
	private Label _mcguffinsCounter;
	private Label _mcguffinsMax;
	private Panel _extractNotif;

	public override void _Ready()
	{
		_currentAmmo = GetNode<Label>("Control/PlayerInfo/AmmoCounter/AmmoInMagazine");
		_maxAmmo = GetNode<Label>("Control/PlayerInfo/AmmoCounter/MaxAmmoInMagazine");
		_currentHealth = GetNode<Label>("Control/PlayerInfo/HealthCounter/CurrentHealth");
		_maxHealth = GetNode<Label>("Control/PlayerInfo/HealthCounter/MaximumHealth");
		_crosshair = GetNode<CrosshairController>("Crosshair");
		PauseMenu = GetNode<PauseMenu>("Pause");
		_objectiveManager = GetNode<ObjectiveManager>("../ObjectiveManager");

		_mcguffinsCounter = GetNode<Label>("ObjectiveTracker/McguffinsCounter/VBoxContainer/Collected/Current");
		_mcguffinsMax = GetNode<Label>("ObjectiveTracker/McguffinsCounter/VBoxContainer/Collected/Total");
		_extractNotif = GetNode<Panel>("ObjectiveTracker/ExtractNotification");

		//Input.SetCustomMouseCursor(_mousePointer, hotspot: new Vector2(13, 23));
		Input.MouseMode = Input.MouseModeEnum.Hidden;

		_objectiveManager.McguffinCollected += OnMcguffinCollected;
		_objectiveManager.AllMcguffinsCollected += OnAllMcguffinsCollected;

		PauseMenu.SaveAndQuit += OnSaveAndQuit;
		PauseMenu.Quit += OnQuit;
	}

	private void OnSaveAndQuit()
	{
		EmitSignal(SignalName.SaveAndQuit);
	}

	private void OnQuit()
	{
		EmitSignal(SignalName.Quit);
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
		_crosshair.UpdateCrosshairPosition(_mousePosition);
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

	public void Init()
	{
		Player.HealthChange += OnPlayerHealthChange;
		Player.Weapon.AmmoChange += OnWeaponAmmoChange;
		Player.Weapon.WeaponStability += OnWeaponStability;
	}

	private void OnPlayerHealthChange(int health, int oldHealth, int maxHealth)
	{
		_currentHealth.Text = health.ToString();
		_maxHealth.Text = maxHealth.ToString();
	}

	private void OnWeaponAmmoChange(int ammo, int oldAmmo, int maxAmmo)
	{
		_currentAmmo.Text = ammo.ToString();
		_maxAmmo.Text = maxAmmo.ToString();
	}

	private void OnWeaponStability(double stability)
	{
		_weaponStability = stability;
	}
}
