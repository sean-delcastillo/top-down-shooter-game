using Godot;

public partial class PlayerDeathPanel : CenterContainer
{
	public int MoneyLost
	{
		set { _moneyLost.Text = value.ToString(); }
		get { return _moneyLost.Text.ToInt(); }
	}

	private Label _moneyLost;
	private Button _return;
	private AnimationPlayer _animation;

	public override void _Ready()
	{
		_moneyLost = GetNode<Label>("%MoneyLost");
		_return = GetNode<Button>("%Return");
		_animation = GetNode<AnimationPlayer>("%LabelFlash");

		_return.Pressed += OnReturnButtonPressed;

		if (Name == "PlayerWinPanel")
		{
			_animation.Play("YouWonTextFlash");
		}
		else
		{
			_animation.Play("YouDiedTextFlash");
		}
	}

	private void OnReturnButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://main_menu.tscn");
	}
}
