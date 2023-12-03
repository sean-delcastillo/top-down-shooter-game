using Godot;
using System;

public partial class CameraController : Node3D
{
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

    public Camera3D Camera;

    private const float RAYCAST_LENGTH = 100f;
    private const float DEFAULT_SLERP_WEIGHT = 4f / 5f;
    private float _cameraFollowSlerpWeight = DEFAULT_SLERP_WEIGHT;
    private Vector3 _mousePositionInWorld;
    private bool _isValidMousePosition = false;

    public override void _Ready()
    {
        Camera = GetNode<Camera3D>("Camera3D");

        // Calculate Rotation based on offset values to have CameraTarget in middle of frame
        var angle = Math.Atan(YOffset / ZOffset);
        Rotate(Vector3.Left, (float)angle + (float)(5 * Math.PI / 180));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CameraTarget != null)
        {
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        var zeroPosition = new Vector3
        (
            CameraTarget.Position.X + XOffset,
            CameraTarget.Position.Y + YOffset,
            CameraTarget.Position.Z + ZOffset
        );

        var mousePositionOnViewport = GetViewport().GetMousePosition();
        var viewportSize = GetViewport().GetVisibleRect().Size;

        var mouseDisplacementRatioFromMiddleOfViewport = (mousePositionOnViewport / viewportSize) - new Vector2(0.5f, 0.5f);
        var mouseDisplacementFromMiddleOfViewport = mouseDisplacementRatioFromMiddleOfViewport.Length();

        var mouseDisplacement = new Vector3(mouseDisplacementRatioFromMiddleOfViewport.X, 0, mouseDisplacementRatioFromMiddleOfViewport.Y);

        Position = zeroPosition + mouseDisplacement * ((float)Math.Pow(mouseDisplacementFromMiddleOfViewport + 1, 3));
    }
}
