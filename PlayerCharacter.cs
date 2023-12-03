using System.Collections.Generic;
using Godot;
using Godot.Collections;

/// <summary>
/// The main player-controllable character.
/// </summary>
public partial class PlayerCharacter : CharacterBody3D
{
	[Signal]
	public delegate void HealthChangeEventHandler(int NewHealth, int OldHealth, int MaxHealth);
	[Signal]
	public delegate void PlayerDiedEventHandler(Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> collectedObjectiveCounts);
	[Signal]
	public delegate void PlayerExtractedEventHandler(Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> collectedObjectiveCounts);


	[Export]
	public CharacterInformation CharacterInformation { set; get; }
	[Export]
	public CharacterWeapon Weapon { set; get; }

	public Camera3D CurrentCamera { set; get; }

	private Vector3 _mousePositionInWorld;
	private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
	private Vector3 _gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
	private Godot.Collections.Dictionary<CollectableObjective.ObjectiveType, int> _collectedObjectives;

	public override void _Ready()
	{
		_collectedObjectives = new()
		{
			{CollectableObjective.ObjectiveType.GoldBall, 0},
			{CollectableObjective.ObjectiveType.RedBall, 0},
			{CollectableObjective.ObjectiveType.GreenBall, 0}
		};
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("LeftMouseButton"))
		{
			Weapon.PrimaryAction();
		}
		if (Input.IsActionJustPressed("Reload"))
		{
			Weapon.Reload();
		}

		CheckIfDead();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateVelocityVectors();
		LookAtCursor();

		MoveAndSlide();
	}

	public void CollectedObjective(CollectableObjective.ObjectiveType type)
	{
		_collectedObjectives[type]++;
	}

	/// <summary>
	/// Called by damaging effects.
	/// </summary>
	/// <param name="damage">The amount of health to deduct from the player</param>
	/// <param name="_">The position where the damage had hit the player. Unused in this implementation</param>
	public void TakeDamage(double damage, Vector3 _)
	{
		EmitSignal(SignalName.HealthChange, CharacterInformation.Health - damage, CharacterInformation.Health, CharacterInformation.MaxHealth);
		CharacterInformation.Health -= damage;
	}

	/// <summary>
	/// Rotates the player's position to face towards where the cursor is in relation to the player's position.
	/// </summary>
	private void LookAtCursor()
	{
		// Create an upwards facing plane at the player's position
		var intersectionPlane = new Plane(new Vector3(0, 1, 0), Position.Y);

		// Create a long raycast from viewport mouse position to world
		var rayLength = 1000;
		var viewportMousePosition = GetViewport().GetMousePosition();
		var from = CurrentCamera.ProjectRayOrigin(viewportMousePosition);
		var to = from + CurrentCamera.ProjectRayNormal(viewportMousePosition) * rayLength;

		// Check if the raycast collides with the upwards facing plane at player's position
		var cursorPositionOnPlane = intersectionPlane.IntersectsRay(from, to);
		if (cursorPositionOnPlane is Vector3 position)
		{
			LookAt(position, Vector3.Up);
		}
	}

	/// <summary>
	/// Takes in user input to move the character.
	/// </summary>
	private void UpdateVelocityVectors()
	{
		var inputDirection = Input.GetVector("StrafeLeft", "StrafeRight", "Forward", "Backward");
		var movementDirection = new Vector3(inputDirection.X, 0, inputDirection.Y).Normalized();

		if (movementDirection != Vector3.Zero)
		{
			var newVelocity = new Vector3(movementDirection.X, 0, movementDirection.Z);
			Velocity = newVelocity * CharacterInformation.MovementSpeed;
		}
		else
		{
			Velocity = Velocity.Lerp(Vector3.Zero, 0.5f);
		}

		// Fall down if not standing on the ground
		if (!IsOnFloor())
		{
			var velocityWithGravity = Velocity;
			velocityWithGravity.Y = -_gravity;

			Velocity += velocityWithGravity;
		}
	}

	/// <summary>
	/// If the player's health reaches 0 then the player will die.
	/// </summary>
	private void CheckIfDead()
	{
		if (CharacterInformation.Health <= 0)
		{
			Die();
		}
	}

	/// <summary>
	/// Plays a death animation at the location of death and frees the player node
	/// </summary>
	private void Die()
	{
		EmitSignal(SignalName.PlayerDied, _collectedObjectives);
		var deathAnimation = CharacterInformation.DeathAnimation.Instantiate() as Node3D;
		deathAnimation.GlobalPosition = GlobalPosition;
		GetParent().AddChild(deathAnimation);
		QueueFree();
	}

	public void ExtractionEntered()
	{
		EmitSignal(SignalName.PlayerExtracted, _collectedObjectives);
		QueueFree();
	}
}
