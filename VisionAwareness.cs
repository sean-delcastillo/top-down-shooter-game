using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

public partial class VisionAwareness : Node3D
{
	[Export]
	public Area3D AwarenessArea { set; get; }
	[Export]
	public RayCast3D VisionCast { set; get; }
	[Export]
	public RayCast3D FrontCast { set; get; }
	[Export]
	public CharacterBody3D Self { set; get; }
	[Export]
	public double FieldOfView { set; get; } = 120;
	/*
	[Export]
	public double CloseAwarenessDistance { set; get; } = 1;
	*/
	public List<Node3D> BodiesInAwarenessRange { set; get; } = new();
	public List<Node3D> BodiesInVisualContact { set; get; } = new();

	public Node3D Facing { set; get; } = null;

	private List<string> _groups;

	public override void _Ready()
	{
		AwarenessArea.BodyEntered += OnAwarenessAreaBodyEntered;
		AwarenessArea.BodyExited += OnAwarenessAreaBodyExited;

		List<string> nonInternalGroups = new();
		foreach (string group in GetGroups())
		{
			if (!group.StartsWith("_"))
			{
				nonInternalGroups.Add(group);
			}
		}
		_groups = nonInternalGroups;
	}

	public override void _Process(double delta)
	{
		CheckLineOfSightToAwarenessRangeObjects();
		CheckLineOfSightToVisualContactObjects();
		UpdateFacing();

		Transform = Transform.Orthonormalized();
	}

	private void CheckLineOfSightToAwarenessRangeObjects()
	{
		for (int i = 0; i < BodiesInAwarenessRange.Count; i++)
		{
			var bodyPosition = BodiesInAwarenessRange[i].GlobalPosition;
			var bodyLocalPosition = bodyPosition * GlobalTransform;
			VisionCast.TargetPosition = bodyLocalPosition;
			VisionCast.ForceRaycastUpdate();
			if (!IsInFov(bodyLocalPosition)) { return; }
			if (VisionCast.GetCollider() as Node3D == BodiesInAwarenessRange[i])
			{
				BodiesInVisualContact.Add(BodiesInAwarenessRange[i]);
			}
		}
	}

	/*
		private bool IsInCloseAwarenessRange(Vector3 position)
		{
			return GlobalPosition.DistanceTo(position) <= CloseAwarenessDistance;
		}
	*/

	private void CheckLineOfSightToVisualContactObjects()
	{
		for (int i = 0; i < BodiesInVisualContact.Count; i++)
		{
			var bodyPosition = BodiesInVisualContact[i].GlobalPosition;
			var bodyLocalPosition = bodyPosition * GlobalTransform;
			VisionCast.TargetPosition = bodyLocalPosition;
			VisionCast.ForceRaycastUpdate();
			if (VisionCast.GetCollider() as Node3D != BodiesInVisualContact[i])
			{
				BodiesInVisualContact.Remove(BodiesInVisualContact[i]);
			}
		}
	}

	private bool IsInFov(Vector3 position)
	{
		var vectorToContact = position.Normalized();
		var forwardVector = Vector3.Forward.Normalized();
		var dot = forwardVector.Dot(vectorToContact);
		var cosTheta = Math.Cos(FieldOfView / 2f * (Math.PI / 180));

		var contactDebug = GetNode<RayCast3D>("ContactDebug");
		var forwardDebug = GetNode<RayCast3D>("ForwardDebug");

		contactDebug.TargetPosition = vectorToContact;
		forwardDebug.TargetPosition = forwardVector;

		return dot > cosTheta;
	}


	private void UpdateFacing()
	{
		FrontCast.ForceRaycastUpdate();
		if (FrontCast.GetCollider() is CharacterBody3D character)
		{
			Facing = character;
		}
		else
		{
			Facing = null;
		}
	}

	private void OnAwarenessAreaBodyEntered(Node3D body)
	{
		if (body is CharacterBody3D && body != Self && !IsInSameGroups(body))
		{
			BodiesInAwarenessRange.Add(body);
		}
	}

	private void OnAwarenessAreaBodyExited(Node3D body)
	{
		BodiesInAwarenessRange.Remove(body);
	}

	private bool IsInSameGroups(Node3D body)
	{
		foreach (string group in _groups)
		{
			if (body.IsInGroup(group))
			{
				return true;
			}
		}
		return false;
	}
}
