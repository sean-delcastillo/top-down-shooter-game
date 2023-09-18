using Godot;
using System;

public partial class CameraController : Node3D
{
    [Signal]
    public delegate void MousePositionInWorldChangedEventHandler(Vector3 collision);

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
        get { return 1 / _cameraFollowSlerpWeight; }
        set
        {
            if (CameraSmooth != 0) { _cameraFollowSlerpWeight = 1 / CameraSmooth; }
        }
    }

    private const float RAYCAST_LENGTH = 100f;
    private const float DEFAULT_SLERP_WEIGHT = 4f / 5f;

    private Camera3D _camera;
    private float _cameraFollowSlerpWeight = DEFAULT_SLERP_WEIGHT;
    private Vector3 _mousePositionInWorld;
    private bool _isValidMousePosition = false;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CameraTarget != null)
        {
            UpdateCameraPosition();
            LookAt(CameraTarget.Position);
        }

        UpdateMousePositionInWorld();
        EmitSignal(SignalName.MousePositionInWorldChanged, _mousePositionInWorld);
    }

    private void UpdateMousePositionInWorld()
    {
        var from = _camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        var to = from + _camera.ProjectRayNormal(GetViewport().GetMousePosition()) * RAYCAST_LENGTH;

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        if (result.ContainsKey("position"))
        {
            _mousePositionInWorld = (Vector3)result["position"];
        }
    }


    private void UpdateCameraPosition()
    {
        var DesiredPosition = new Vector3
        (
            CameraTarget.Position.X + XOffset,
            CameraTarget.Position.Y + YOffset,
            CameraTarget.Position.Z + ZOffset
        );

        Position = DesiredPosition;
    }
}
