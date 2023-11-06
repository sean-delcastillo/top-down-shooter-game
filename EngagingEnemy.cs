using Godot;
using Godot.Collections;
using System;

public partial class EngagingEnemy : EnemyState
{
    private Vector3 _lastKnownLocation;

    public override void Enter(Dictionary SeenEnemy = null)
    {
        _lastKnownLocation = ((Node3D)SeenEnemy["SeenEnemy"]).GlobalPosition;
    }

    public override void Update(double Delta)
    {
        if (StateManager.Character._visionAwareness.BodiesInVisualContact.Count < 1)
        {
            Dictionary dict = new() {
                { "LastKnownLocation", _lastKnownLocation}
            };
            StateManager.TransitionTo("PursuingEnemy", dict);
        }
    }

    public override void PhysicsUpdate(double Delta)
    {
        StateManager.Character.LookAt(_lastKnownLocation);
        if (StateManager.Character._visionAwareness.Facing != null && !StateManager.Character._visionAwareness.Facing.IsInGroup("Enemies"))
        {
            StateManager.Character.Weapon?.PrimaryAction();
        }
    }
}
