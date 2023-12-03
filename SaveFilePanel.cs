using Godot;
using System.IO;

public partial class SaveFilePanel : PanelContainer
{
	[Signal]
	public delegate void SaveFileDeletedEventHandler();

	private Label _mainLabel;
	private MarginContainer _currentMoneyContainer;
	private MarginContainer _mostMoneyContainer;
	private Label _currentMoney;
	private Label _mostMoney;
	private Label _currentHealth;
	private Label _currentSpeed;
	private Button _upgradeHealth;
	private Button _upgradeSpeed;
	private Button _delete;
	private PanelContainer _upgrades;
	private AnimationPlayer _healthNotEnoughMoney;
	private AnimationPlayer _speedNotEnoughMoney;
	private PanelContainer _everythingContainer;

	public override void _Ready()
	{
		_mainLabel = GetNode<Label>("%MainLabel");
		_currentMoneyContainer = GetNode<MarginContainer>("%CurrentMoneyContainer");
		_mostMoneyContainer = GetNode<MarginContainer>("%MostMoneyContainer");
		_currentMoney = GetNode<Label>("%CurrentMoney");
		_mostMoney = GetNode<Label>("%MostMoney");
		_delete = GetNode<Button>("%Delete");
		_currentHealth = GetNode<Label>("%CurrentHealth");
		_currentSpeed = GetNode<Label>("%CurrentSpeed");
		_upgradeHealth = GetNode<Button>("%UpgradeHealth");
		_upgradeSpeed = GetNode<Button>("%UpgradeSpeed");
		_upgrades = GetNode<PanelContainer>("%Upgrades");
		_healthNotEnoughMoney = GetNode<AnimationPlayer>("%UpgradeHealthNotEnoughMoney");
		_speedNotEnoughMoney = GetNode<AnimationPlayer>("%UpgradeSpeedNotEnoughMoney");
		_everythingContainer = GetNode<PanelContainer>("%EverythingContainer");

		_delete.Pressed += OnDeletePressed;
		_upgradeHealth.Pressed += OnUpgradeHealthPressed;
		_upgradeSpeed.Pressed += OnUpgradeSpeedPressed;

		LoadInfo();
	}

	private void LoadInfo()
	{
		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");

		if (Persistence.SaveFileExists())
		{
			_mainLabel.Text = "Current Save File";
			_everythingContainer.Visible = true;
			/*
			_currentMoneyContainer.Visible = true;
			_mostMoneyContainer.Visible = true;
			_delete.Visible = true;
			_upgrades.Visible = true;
			*/

			_currentMoney.Text = progression.Money.ToString();
			_mostMoney.Text = progression.MostMoneyInOneRun.ToString();

			_currentHealth.Text = progression.CurrentHealth.ToString();
			_currentSpeed.Text = progression.CurrentSpeed.ToString();
		}
		else
		{
			_mainLabel.Text = "Save File Not Found";
			_everythingContainer.Visible = false;
			/*
			_currentMoneyContainer.Visible = false;
			_mostMoneyContainer.Visible = false;
			_delete.Visible = false;
			_upgrades.Visible = false;
			*/
		}
	}

	private void OnDeletePressed()
	{
		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");
		var dir = DirAccess.Open("user://");
		dir.Remove("savegame.save");
		progression.Reset();
		EmitSignal(SignalName.SaveFileDeleted);
		LoadInfo();
	}

	private void OnUpgradeHealthPressed()
	{
		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");

		int HealthUpgradePrice = 500;

		if (progression.Money >= HealthUpgradePrice)
		{
			if (progression.CurrentHealth + 50 < 500)
			{
				progression.Money -= HealthUpgradePrice;
				progression.CurrentHealth += 50;

				if (progression.CurrentSpeed > 3)
				{
					progression.CurrentSpeed -= 0.5f;
				}
			}
			else
			{
				progression.CurrentHealth = 500;
			}
			LoadInfo();
		}
		else
		{
			_healthNotEnoughMoney.Play("NotEnoughMoney");
		}
	}

	private void OnUpgradeSpeedPressed()
	{
		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");

		int SpeedUpgradePrice = 500;

		if (progression.Money >= SpeedUpgradePrice)
		{
			if (progression.CurrentSpeed < 8)
			{
				progression.Money -= SpeedUpgradePrice;
				progression.CurrentSpeed += 0.5f;
			}
			LoadInfo();
		}
		else
		{
			_speedNotEnoughMoney.Play("NotEnoughMoney");
		}
	}
}