using Godot;

public partial class PlayerInfoTracker : MarginContainer
{
	private Label _currentAmmo;
	private Label _maxAmmo;
	private Label _currentHealth;
	private Label _maxHealth;

	public override void _Ready()
	{
		_currentAmmo = GetNode<Label>("%CurrentAmmo");
		_maxAmmo = GetNode<Label>("%MaxAmmo");
		_currentHealth = GetNode<Label>("%CurrentHealth");
		_maxHealth = GetNode<Label>("%MaxHealth");
	}

	public void SetAmmo(int currentAmmo, int maxAmmo)
	{
		SetCurrentAmmo(currentAmmo);
		SetMaxAmmo(maxAmmo);
	}

	public void SetHealth(int currentHealth, int maxHealth)
	{
		SetCurrentHealth(currentHealth);
		SetMaxHealth(maxHealth);
	}

	public void SetCurrentAmmo(int currentAmmo)
	{
		SetLabelTextFromInt(_currentAmmo, currentAmmo);
	}

	public void SetMaxAmmo(int maxAmmo)
	{
		SetLabelTextFromInt(_maxAmmo, maxAmmo);
	}

	public void SetCurrentHealth(int currentHealth)
	{
		SetLabelTextFromInt(_currentHealth, currentHealth);
	}

	public void SetMaxHealth(int maxHealth)
	{
		SetLabelTextFromInt(_maxHealth, maxHealth);
	}

	private static void SetLabelTextFromInt(Label label, int integer)
	{
		label.Text = integer.ToString();
	}
}
