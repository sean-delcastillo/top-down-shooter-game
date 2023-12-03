using Godot;
using System;

public partial class BoolObjective : PanelContainer
{
	public string ObjectiveName
	{
		set { _name.Text = value; }
		get { return _name.Text; }
	}

	private Label _name;
	private Label _boolLabel;

	public override void _Ready()
	{
		_name = GetNode<Label>("%ObjectiveName");
		_boolLabel = GetNode<Label>("%BoolLabel");
	}

	public async void UpdateLabel(string name, string label)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		_name.Text = name;
		_boolLabel.Text = label;
	}
}
