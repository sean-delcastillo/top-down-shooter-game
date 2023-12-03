using Godot;
using System;

public partial class ManualPanel : PanelContainer
{
	private Button _close;

	public override void _Ready()
	{
		_close = GetNode<Button>("%Close");

		_close.Pressed += OnCloseButtonPressed;
	}

	public void OnCloseButtonPressed()
	{
		Visible = false;
	}
}
