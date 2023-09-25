using Godot;
using System;

public partial class PlayerCharacterController : CharacterBody3D
{
	[Export]
	public CharacterInformation CharacterInformation { set; get; }
	[Export]
	public CharacterWeapon Weapon { set; get; }

	private Vector3 _mousePositionInWorld;
	private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
	private Vector3 _gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("LeftMouseButton"))
		{
			Weapon.PrimaryAction();
		}

		CheckIfDead();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateVelocityVectors();
		UpdateMouseLook();

		MoveAndSlide();
	}

	public void TakeDamage(double damage)
	{
		CharacterInformation.Health -= damage;
	}

	public void UpdateMousePositionInWorld(Vector3 position)
	{
		position.Y = Position.Y;
		_mousePositionInWorld = position;
	}

	private void UpdateMouseLook()
	{
		LookAt(_mousePositionInWorld);
	}

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

		if (!IsOnFloor())
		{
			var velocityWithGravity = Velocity;
			velocityWithGravity.Y = -_gravity;

			Velocity += velocityWithGravity;
		}
	}

	private void CheckIfDead()
	{
		if (CharacterInformation.Health <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		var deathAnimation = CharacterInformation.DeathAnimation.Instantiate() as Node3D;
		deathAnimation.GlobalPosition = GlobalPosition;
		GetParent().AddChild(deathAnimation);
		QueueFree();
	}
}
