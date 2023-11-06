using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class EnemyStateManager : Node
{
	[Signal]
	public delegate void StateTransisionedEventHandler();

	[Export]
	public EnemyState InitialState { set; get; }

	public EnemyState CurrentState { set; get; }
	public EnemyCharacterController Character;

	private int _lastRouteIndex;
	private int _lastPointIndex;

	public override void _Ready()
	{
		//await ToSignal(Owner, Node.SignalName.Ready);

		CurrentState = InitialState;

		foreach (Node child in GetChildren())
		{
			if (child is EnemyState state)
			{
				state.StateManager = this;
			}
		}
		CurrentState.Enter();
	}

	public override void _Process(double delta)
	{
		CurrentState.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentState.PhysicsUpdate(delta);
	}

	public void TransitionTo(string StateName, Dictionary TransitionData = null)
	{
		//GD.Print("Transitioning State to " + StateName + " from " + CurrentState.Name);
		CurrentState.Exit();
		CurrentState = GetNode<EnemyState>(StateName);
		CurrentState.Enter(TransitionData);
		EmitSignal(SignalName.StateTransisioned, StateName);
	}

	public void SetCurrentRoute(int InitialRoute, int InitialPoint)
	{
		Dictionary dict = new()
		{
			{ "Route", InitialRoute },
			{ "Point", InitialPoint }
		};
		TransitionTo("NavigatingToPoint", dict);
	}

	public void SaveNavigationState(int routeIndex, int pointIndex)
	{
		_lastRouteIndex = routeIndex;
		_lastPointIndex = pointIndex;
	}
}
