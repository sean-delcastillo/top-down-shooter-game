using Godot;
using Godot.Collections;

public partial class EngagingEnemy : EnemyState
{
    public CharacterBody3D CurrentlyEngagedHostile;
    private Vector3 _lastKnownLocation;
    private Timer _timer;

    public override void _Ready()
    {
        base._Ready();

        _timer = new()
        {
            WaitTime = 0.35,
            OneShot = true
        };
        AddChild(_timer);
    }

    public override void Enter(Dictionary SeenEnemy = null)
    {
        StateManager.NavAgent.TargetPosition = StateManager.Character.GlobalPosition;
        CurrentlyEngagedHostile = (CharacterBody3D)SeenEnemy["SeenEnemy"];
        _lastKnownLocation = CurrentlyEngagedHostile.GlobalPosition;
    }

    public override void Exit()
    {
        _timer.Stop();
    }

    public override void PhysicsUpdate(double Delta)
    {
        if (StateManager.VisionAwareness.IsFacing(CurrentlyEngagedHostile))
        {
            HandleShooting();
        }

        StateManager.Character.LookAt(_lastKnownLocation, Vector3.Up);

        if (StateManager.VisionAwareness.CanSeeBody(CurrentlyEngagedHostile))
        {
            _lastKnownLocation = CurrentlyEngagedHostile.GlobalPosition;
        }
        else
        {
            Dictionary dict = new()
            {
                {"LastKnownLocation", _lastKnownLocation}
            };

            StateManager.TransitionTo("PursuingEnemy", dict);
        }
    }

    /*
    public override void HostileLost(Node3D hostile)
    {
        if (hostile == CurrentlyEngagedHostile)
        {
            Dictionary dict = new()
            {
                {"LastKnownLocation", hostile.GlobalPosition}
            };

            StateManager.TransitionTo("PursuingEnemy", dict);
        }
    }
    */

    private void HandleShooting()
    {
        StateManager.Character.Weapon?.PrimaryAction();
    }
}
