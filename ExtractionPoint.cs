using Godot;
using System;

public partial class ExtractionPoint : StaticBody3D
{
	[Signal]
	public delegate void ExtractionEnteredEventHandler();

	private Area3D _enterArea;

	public override void _Ready()
	{
		_enterArea = GetNode<Area3D>("Area3D");
		_enterArea.BodyEntered += OnEnterAreaBodyEntered;
	}

	public void ActivateExtraction()
	{
		Visible = true;
		_enterArea.Monitoring = true;
	}

	private void OnEnterAreaBodyEntered(Node3D body)
	{
		if (body is PlayerCharacterController)
		{
			EmitSignal(SignalName.ExtractionEntered);
		}
	}
}
