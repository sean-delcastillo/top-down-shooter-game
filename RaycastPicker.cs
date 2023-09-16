using Godot;
using System;
using System.Dynamic;

public partial class RaycastPicker : Node3D
{
    [Signal]
    public delegate void RayPickerCollisionEventHandler(Vector3 position);

    public Camera3D Camera { set; get; }

    private Vector3 from = default;
    private Vector3 to = default;

    private const float RAY_LENGTH = 100f;


    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("LeftMouseButton"))
        {
            EmitSignal(SignalName.RayPickerCollision, EmitPickingRay());
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsActionPressed("LeftMouseButton"))
        {
            from = Camera.ProjectRayOrigin(eventMouseButton.Position);
            to = from + Camera.ProjectRayNormal(eventMouseButton.Position) * RAY_LENGTH;

        }
    }

    private Vector3 EmitPickingRay()
    {
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = GetWorld3D().DirectSpaceState.IntersectRay(query);

        return (Vector3)result["position"];
    }
}
