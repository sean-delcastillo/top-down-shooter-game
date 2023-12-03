using System.Security;
using Godot;
using Godot.Collections;

public partial class EnemyState : Node
{
    public EnemyStateManager StateManager;

    public override void _Ready()
    {
        //await ToSignal(Owner, Node.SignalName.Ready);
    }

    public virtual void Enter(Dictionary TransitionData = null)
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Update(double Delta)
    {
    }

    public virtual void PhysicsUpdate(double Delta)
    {
    }

    public virtual void HostileDetected(Node3D hostile)
    {

    }

    public virtual void HostileLost(Node3D hostile)
    {

    }
}
