using Godot;

public partial class NavigationPoint : Marker3D
{
    [ExportGroup("Agent Behavior at Point")]
    [Export]
    public float LoiterTime;
    [Export]
    public float LoiterVarianceTime;
    [Export]
    public bool WillWanderAtPoint;
    [Export]
    public int WanderRange;

    [ExportGroup("Route Information")]
    [Export]
    public bool isPartOfRoute;
    [Export]
    public int RouteId;
    [Export]
    public int RouteOrder;
}
