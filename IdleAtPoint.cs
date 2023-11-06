using Godot;
using Godot.Collections;

public partial class IdleAtPoint : EnemyState
{
    private int _routeIndex;
    private int _pointIndex;
    public override void Enter(Dictionary RoutePoint = null)
    {
        _routeIndex = (int)RoutePoint["Route"];
        _pointIndex = (int)RoutePoint["Point"];

        GetTree().CreateTimer(3).Timeout += ProceedToNextpoint;
    }

    public override void Update(double Delta)
    {
        if (StateManager.Character._visionAwareness.BodiesInVisualContact.Count > 1)
        {
            var seenEnemy = StateManager.Character._visionAwareness.BodiesInVisualContact[0];
            GD.Print("I see you, " + seenEnemy);
            Dictionary dict = new() {
                { "SeenEnemy", seenEnemy }
            };

            StateManager.TransitionTo("EngagingEnemy", dict);
        }
    }

    public void ProceedToNextpoint()
    {
        StateManager.Character.NavManager.GetNextPointOnRoute();
        StateManager.TransitionTo("NavigatingToPoint");
    }
}
