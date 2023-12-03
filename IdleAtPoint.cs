using Godot;
using Godot.Collections;

public partial class IdleAtPoint : EnemyState
{
    private int _routeIndex;
    private int _pointIndex;

    private Timer _timer;

    public override void _Ready()
    {
        base._Ready();

        _timer = new()
        {
            WaitTime = 3,
            OneShot = true
        };
        AddChild(_timer);
    }

    public override void Enter(Dictionary RoutePoint = null)
    {
        _routeIndex = (int)RoutePoint["Route"];
        _pointIndex = (int)RoutePoint["Point"];

        WaitAtPoint();
    }

    public override void Update(double Delta)
    {
        /* Replaced by StateManager's HostileDetected method
        if (StateManager.Character._visionAwareness.BodiesInVisualContact.Count > 1)
        {
            var seenEnemy = StateManager.Character._visionAwareness.BodiesInVisualContact[0];
            GD.Print("I see you, " + seenEnemy);
            Dictionary dict = new() {
                { "SeenEnemy", seenEnemy }
            };

            StateManager.TransitionTo("EngagingEnemy", dict);
        }
        */
    }

    public async void WaitAtPoint()
    {
        _timer.Start();
        await ToSignal(_timer, Timer.SignalName.Timeout);
        ProceedToNextPoint();
    }

    public void ProceedToNextPoint()
    {
        StateManager.Character.NavManager.GetNextPointOnRoute();
        StateManager.TransitionTo("NavigatingToPoint");
    }

    public override void HostileDetected(Node3D hostile)
    {
        Dictionary dict = new()
        {
            {"SeenEnemy", hostile}
        };
        _timer.Stop();
        StateManager.TransitionTo("EngagingEnemy", dict);
    }
}
