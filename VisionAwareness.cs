using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class VisionAwareness : Node3D
{
	[Export]
	public Area3D AwarenessArea { set; get; }
	[Export]
	public RayCast3D VisionCast { set; get; }
	[Export]
	public CharacterBody3D Self { set; get; }

	private List<Node3D> _bodiesInAwarenessRange = new();
	private List<Node3D> _bodiesInVisualContact = new();

	public override void _Ready()
	{
		AwarenessArea.BodyEntered += OnAwarenessAreaBodyEntered;
		AwarenessArea.BodyExited += OnAwarenessAreaBodyExited;
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < _bodiesInAwarenessRange.Count; i++)
		{
			VisionCast.TargetPosition = _bodiesInAwarenessRange[i].GlobalPosition * GlobalTransform;
			if (VisionCast.GetCollider() as Node3D == _bodiesInAwarenessRange[i])
			{
				_bodiesInVisualContact.Add(_bodiesInAwarenessRange[i]);
			}
		}

		for (int i = 0; i < _bodiesInVisualContact.Count; i++)
		{
			VisionCast.TargetPosition = _bodiesInVisualContact[i].GlobalPosition * GlobalTransform;
			if (VisionCast.GetCollider() as Node3D != _bodiesInVisualContact[i])
			{
				_bodiesInVisualContact.Remove(_bodiesInVisualContact[i]);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_bodiesInVisualContact.Count > 0)
		{
			GetParent<Node3D>().LookAt(_bodiesInVisualContact[0].GlobalPosition);
		}
	}

	private void OnAwarenessAreaBodyEntered(Node3D body)
	{
		if (body is CharacterBody3D && body != Self)
		{
			_bodiesInAwarenessRange.Add(body);
		}
	}

	private void OnAwarenessAreaBodyExited(Node3D body)
	{
		_bodiesInAwarenessRange.Remove(body);
	}
}
