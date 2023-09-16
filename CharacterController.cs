using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
	// TODO Remove logic for raycast picking since it has been moved to CameraRig scene
	// TODO Connect to CameraRig's raycast collision signal
	private NavigationAgent3D agent;
	private Camera3D camera;

	private Vector3 from = default;
	private Vector3 to = default;

	private float RayLength = 100f;

	public override void _Ready()
	{
		agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		camera = GetNode<Camera3D>("../CameraRig/Camera3D");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("LeftMouseButton"))
		{
			agent.TargetPosition = EmitPickingRay();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsActionPressed("LeftMouseButton"))
		{
			from = camera.ProjectRayOrigin(eventMouseButton.Position);
			to = from + camera.ProjectRayNormal(eventMouseButton.Position) * RayLength;

		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = GetNewVelocityFromAgent();

		MoveAndSlide();
	}

	private Vector3 EmitPickingRay()
	{
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = GetWorld3D().DirectSpaceState.IntersectRay(query);

		return (Vector3)result["position"];
	}

	private Vector3 GetNewVelocityFromAgent()
	{
		var currentLocation = GlobalTransform.Origin;
		var nextLocation = agent.GetNextPathPosition();
		return (nextLocation - currentLocation).Normalized() * 10;
	}
}
