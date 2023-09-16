using Godot;
using System;
using System.Dynamic;

public partial class CameraController : Node3D
{
    // TODO Capture and reemit raycast collision from RaycastPicker node
    [Export]
    public Node3D CameraTarget { get; set; }

    [Export]
    public int XOffset { get; set; }
    [Export]
    public int YOffset { get; set; } = 20;
    [Export]
    public int ZOffset { get; set; } = 5;

    [Export]
    public float CameraSmooth
    {
        get { return 1 / CameraFollowSlerpWeight; }
        set
        {
            if (CameraSmooth != 0) { CameraFollowSlerpWeight = 1 / CameraSmooth; }
            else { CameraFollowSlerpWeight = DEFAULT_SLERP_WEIGHT; }
        }
    }

    private const float DEFAULT_SLERP_WEIGHT = 1 / 5;

    private Camera3D camera;
    private float CameraFollowSlerpWeight;


    [Signal]
    public delegate void RayPickerCollisionEventHandler(Vector3 collision);

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");

        RaycastPicker RaycastPicker = GetNode<RaycastPicker>("RaycastPicker");
        RaycastPicker.Set("Camera", camera);
        RaycastPicker.RayPickerCollision += OnRaycastPickerRayPickerCollision; // I dont understand why this isnt working
    }

    private void OnRaycastPickerRayPickerCollision(Vector3 position)
    {
        GD.Print(position);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CameraTarget != null)
        {
            Position = GetCameraPosition();
            LookAt(CameraTarget.Position);
        }
    }

    private Vector3 GetCameraPosition()
    {
        var DesiredPosition = new Vector3
        (
            CameraTarget.Position.X + XOffset,
            CameraTarget.Position.Y + YOffset,
            CameraTarget.Position.Z + ZOffset
        );
        return Position.Slerp(DesiredPosition, CameraFollowSlerpWeight);
    }
}
