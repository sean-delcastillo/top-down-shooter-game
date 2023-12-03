using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A system to give a node vision and awarenes.
/// </summary>
public partial class VisionAwareness : Node3D
{
	[Signal]
	public delegate void HostileContactDetectedEventHandler(CharacterBody3D hostile);
	[Signal]
	public delegate void HostileContactLostEventHandler(CharacterBody3D hostile);

	[Export]
	public double FieldOfView { set; get; } = 120;

	public Area3D AwarenessArea;
	public RayCast3D VisionCast;
	public RayCast3D ForwardCast;
	public CharacterBody3D SelfBody;
	/*
	[Export]
	public double CloseAwarenessDistance { set; get; } = 1;
	*/
	public List<CharacterBody3D> BodiesInAwarenessRange { set; get; } = new();
	public List<CharacterBody3D> BodiesInVisualContact { set; get; } = new();
	/*
	public List<Node3D> HostilesInAwarenessRange { set; get; } = new();
	public List<Node3D> HostilesInVisualContact { set; get; } = new();
	*/

	public Node3D Facing { set; get; } = null;

	private List<string> _groups;
	private Timer _updateTimer;

	public override void _Ready()
	{
		AwarenessArea = GetNode<Area3D>("%AwarenessArea");
		VisionCast = GetNode<RayCast3D>("%VisionCast");
		ForwardCast = GetNode<RayCast3D>("%ForwardCast");
		SelfBody = GetParent<CharacterBody3D>();
		_updateTimer = GetNode<Timer>("%UpdateTimer");

		_updateTimer.Timeout += CheckLineOfSight;
		//AwarenessArea.BodyEntered += OnAwarenessAreaBodyEntered;
		//AwarenessArea.BodyExited += OnAwarenessAreaBodyExited;

		// Getting groups that aren't automatically added by Godot
		List<string> nonInternalGroups = new();
		foreach (string group in GetParent().GetGroups())
		{
			// Internal group names begin with an underscore
			if (!group.StartsWith("_"))
			{
				nonInternalGroups.Add(group);
			}
		}
		_groups = nonInternalGroups;
	}

	public override void _Process(double delta)
	{
		//CheckLineOfSightToAwarenessRangeObjects();
		//CheckLineOfSightToVisualContactObjects();
		UpdateFacing();

		Transform = Transform.Orthonormalized();

	}

	public bool CanSeeBody(CharacterBody3D body)
	{
		if (AwarenessArea.HasOverlappingBodies())
		{
			var bodyPosition = body.GlobalPosition;
			VisionCast.LookAt(bodyPosition, Vector3.Up);
			VisionCast.ForceRaycastUpdate();
		}

		if (IsInFov(ToLocal(body.GlobalPosition)) && VisionCast.IsColliding())
		{
			return VisionCast.GetCollider() == body;
		}

		return false;
	}

	public bool IsFacing(CharacterBody3D body)
	{
		VisionCast.LookAt(body.GlobalPosition, Vector3.Up);
		VisionCast.ForceRaycastUpdate();

		var forward = Vector3.Forward;
		var dot = forward.Dot(VisionCast.TargetPosition);
		var cosTheta = Math.Cos(0);

		if (VisionCast.IsColliding() && dot >= cosTheta)
		{
			var collider = VisionCast.GetCollider();

			if (collider == body)
			{
				return true;
			}
		}

		return false;
	}

	public void CheckLineOfSight()
	{
		if (AwarenessArea.HasOverlappingBodies())
		{
			var bodiesInRange = AwarenessArea.GetOverlappingBodies();
			foreach (Node3D body in bodiesInRange)
			{
				if (body is CharacterBody3D && IsInFov(ToLocal(body.GlobalPosition)) && !IsInSameGroups(body) && body != SelfBody)
				{
					var bodyPosition = body.GlobalPosition;
					VisionCast.LookAt(bodyPosition, Vector3.Up);
					VisionCast.ForceRaycastUpdate();
				}

				if (VisionCast.IsColliding())
				{
					var collider = VisionCast.GetCollider();

					if (collider is CharacterBody3D && !IsInSameGroups(body) && collider == body)
					{
						EmitSignal(SignalName.HostileContactDetected, (CharacterBody3D)body);
					}
				}
			}
		}
	}

	/*
	private void CheckLineOfSightToAwarenessRangeObjects()
	{
		for (int i = 0; i < BodiesInAwarenessRange.Count; i++)
		{
			// Get position of the body to check and turn it to local position
			var bodyPosition = BodiesInAwarenessRange[i].GlobalPosition;
			var bodyLocalPosition = bodyPosition * GlobalTransform;

			// Aim the visioncast towards the body's position
			VisionCast.TargetPosition = bodyLocalPosition;
			VisionCast.ForceRaycastUpdate();

			if (!IsInFov(bodyLocalPosition)) { return; }

			// Enemy can see the body
			//if (VisionCast.GetCollider() as Node3D == BodiesInAwarenessRange[i])
			if (VisionCast.GetCollider() is CharacterBody3D body && body == BodiesInAwarenessRange[i])
			{
				BodiesInVisualContact.Add(body);
				if (!IsInSameGroups(body))
				{
					EmitSignal(SignalName.HostileContactDetected, body);
					//HostilesInVisualContact.Add(body);
				}
			}
		}
	}
	*/

	/*
		private bool IsInCloseAwarenessRange(Vector3 position)
		{
			return GlobalPosition.DistanceTo(position) <= CloseAwarenessDistance;
		}
	*/

	/*
	private void CheckLineOfSightToVisualContactObjects()
	{
		for (int i = 0; i < BodiesInVisualContact.Count; i++)
		{
			var bodyPosition = BodiesInVisualContact[i].GlobalPosition;
			var bodyLocalPosition = bodyPosition * GlobalTransform;
			VisionCast.TargetPosition = bodyLocalPosition;
			VisionCast.ForceRaycastUpdate();
			//if (VisionCast.GetCollider() as Node3D != BodiesInVisualContact[i])
			//if (VisionCast.GetCollider() is CharacterBody3D body && body != BodiesInVisualContact[i])
			var collider = VisionCast.GetCollider();
			if (collider is not CharacterBody3D || collider is CharacterBody3D body && body != BodiesInVisualContact[i])
			{
				BodiesInVisualContact.Remove(BodiesInVisualContact[i]);
				if (!IsInSameGroups(BodiesInVisualContact[i]))
				{
					EmitSignal(SignalName.HostileContactLost, BodiesInVisualContact[i]);
				}
			}
		}
	}
	*/

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
		ForwardCast.ForceRaycastUpdate();
		if (ForwardCast.GetCollider() is CharacterBody3D character)
		{
			Facing = character;
		}
		else
		{
			Facing = null;
		}
	}

	/*
	private void OnAwarenessAreaBodyEntered(Node3D body)
	{
		if (body is CharacterBody3D characterBody && body != SelfBody && !IsInSameGroups(body))
		{
			BodiesInAwarenessRange.Add(characterBody);
		}
	}

	private void OnAwarenessAreaBodyExited(Node3D body)
	{
		if (body is CharacterBody3D characterBody && BodiesInAwarenessRange.Contains(characterBody))
		{
			BodiesInAwarenessRange.Remove(characterBody);

			if (BodiesInVisualContact.Contains(characterBody))
			{
				BodiesInVisualContact.Remove(characterBody);
			}
		}
	}
	*/

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
