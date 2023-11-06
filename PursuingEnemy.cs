using Godot;
using Godot.Collections;
using System;

public partial class PursuingEnemy : EnemyState
{
    private Vector3 _lastKnownLocation;

    public override void Enter(Dictionary LastKnownLocation = null)
    {
        _lastKnownLocation = (Vector3)LastKnownLocation["LastKnownLocation"];

        StateManager.Character.NavAgent.TargetPosition = _lastKnownLocation;

        StateManager.Character.NavAgent.TargetReached += AtLastKnownLocation;
    }

    public override void Update(double Delta)
    {
        if (StateManager.Character._visionAwareness.BodiesInAwarenessRange.Count > 1)
        {
            var seenEntity = StateManager.Character._visionAwareness.BodiesInAwarenessRange[0];
            if (seenEntity.IsInGroup("Enemies"))
            {
                return;
            }
            var seenEnemy = seenEntity;
            Dictionary dict = new() {
                { "SeenEnemy", seenEnemy }
            };
            StateManager.TransitionTo("EngagingEnemy", dict);
        }
    }

    private void AtLastKnownLocation()
    {
        GetTree().CreateTimer(5).Timeout += ReturnToPatrol;
    }

    private void ReturnToPatrol()
    {
        StateManager.TransitionTo("NavigatingToPoint");
    }
}
