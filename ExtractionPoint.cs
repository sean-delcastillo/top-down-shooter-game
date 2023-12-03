using Godot;
using System;

public partial class ExtractionPoint : StaticBody3D
{
	[Signal]
	public delegate void ExtractionEnteredEventHandler();

	[Export]
	public string ExtractionName;

	private Area3D _enterArea;

	public override void _Ready()
	{
		_enterArea = GetNode<Area3D>("Area3D");
		_enterArea.BodyEntered += OnEnterAreaBodyEntered;

		ExtractionName ??= Name;
	}

	public void Activate()
	{
		// Visible = true;
		// _enterArea.Monitoring = true;

		SetDeferred(Node3D.PropertyName.Visible, true);
		_enterArea.SetDeferred(Area3D.PropertyName.Monitoring, true);
	}

	public void Deactivate()
	{
		SetDeferred(Node3D.PropertyName.Visible, false);
		_enterArea.SetDeferred(Area3D.PropertyName.Monitoring, false);
	}

	private void OnEnterAreaBodyEntered(Node3D body)
	{
		if (body is PlayerCharacter player)
		{
			player.ExtractionEntered();
			EmitSignal(SignalName.ExtractionEntered);
		}
	}
}
