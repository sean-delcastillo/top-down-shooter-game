using Godot;
using Godot.Collections;
using System;

public partial class PursuingEnemy : EnemyState
{
    private Vector3 _lastKnownLocation;
    private Timer _timer;

    public override void _Ready()
    {
        base._Ready();

        _timer = new Timer()
        {
            WaitTime = 3,
            OneShot = true
        };
        AddChild(_timer);
    }

    public override void Enter(Dictionary LastKnownLocation = null)
    {
        _lastKnownLocation = (Vector3)LastKnownLocation["LastKnownLocation"];

        StateManager.NavAgent.TargetPosition = _lastKnownLocation;
        StateManager.NavAgent.TargetReached += AtLastKnownLocation;
    }

    public override void Exit()
    {
        StateManager.NavAgent.TargetReached -= AtLastKnownLocation;
        _timer.Stop();
    }

    public override void Update(double Delta)
    {
        /* Replaced by StateManager's HostileDetected method
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
        */
    }

    private async void AtLastKnownLocation()
    {
        _timer.Start();
        await ToSignal(_timer, Timer.SignalName.Timeout);
        ReturnToPatrol();
    }

    private void ReturnToPatrol()
    {
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
