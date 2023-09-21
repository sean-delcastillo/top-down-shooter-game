using Godot;
using System;

public partial class PlayerCharacterController : CharacterBody3D
{
	[Export]
	public float Speed = 5f;

	[Export]
	public CharacterWeapon Weapon;

	private Vector3 _mousePositionInWorld;
	private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
	private Vector3 _gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("LeftMouseButton"))
		{
			Weapon.PrimaryAction();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateVelocityVectors();
		UpdateMouseLook();

		MoveAndSlide();
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
			Velocity = newVelocity * Speed;
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
}
