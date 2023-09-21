using Godot;
using Godot.Collections;
using System;

public partial class VisionAwareness : Node3D
{
	[Export]
	public Area3D AwarenessArea { set; get; }
	[Export]
	public RayCast3D VisionCast { set; get; }

	private System.Collections.ArrayList _bodiesInRange;

	public override void _Ready()
	{
		AwarenessArea.BodyEntered += OnAwarenessAreaBodyEntered;
	}

	public override void _Process(double delta)
	{
		if (_bodiesInRange.Count > 0)
		{
			for (int i = 0; i < _bodiesInRange.Count; i++)
			{
				var body = _bodiesInRange[i] as Node3D;
				VisionCast.TargetPosition = Transform * body.GlobalPosition;
			}
		}
	}

	private void OnAwarenessAreaBodyEntered(Node3D body)
	{
		_bodiesInRange.Add(body);
	}
}
