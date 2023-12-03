using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class EnemyStateManager : Node
{
	[Signal]
	public delegate void StateTransisionedEventHandler();

	[Export]
	public EnemyState InitialState { set; get; }

	public EnemyState CurrentState { set; get; }
	public EnemyCharacterController Character;
	public NavigationPointManager NavManager;
	public NavigationAgent3D NavAgent;
	public VisionAwareness VisionAwareness;

	private int _lastRouteIndex;
	private int _lastPointIndex;
	private Vector3 _lastKnownHostileLocation;
	private Vector3 _lastKnownAwarenessLocation;
	private List<string> _possibleStates;

	public override void _Ready()
	{
		//await ToSignal(Owner, Node.SignalName.Ready);
		_possibleStates = new();

		CurrentState = InitialState;

		foreach (Node child in GetChildren())
		{
			if (child is EnemyState state)
			{
				_possibleStates.Add(state.Name);
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
		if (_possibleStates.Contains(StateName))
		{
			var prevState = CurrentState;
			CurrentState.Exit();
			CurrentState = GetNode<EnemyState>(StateName);
			CurrentState.Enter(TransitionData);
			//GD.Print("Changing State From " + prevState.Name + " to " + CurrentState.Name);
			EmitSignal(SignalName.StateTransisioned, StateName);
		}
	}

	public void SetCurrentRoute(int InitialRoute, int InitialPoint)
	{
		Dictionary dict = new()
		{
			{"Route", InitialRoute},
			{"Point", InitialPoint}
		};
		TransitionTo("NavigatingToPoint", dict);
	}

	public void SaveNavigationState(int routeIndex, int pointIndex)
	{
		_lastRouteIndex = routeIndex;
		_lastPointIndex = pointIndex;
	}

	public void HostileDetected(Node3D hostile)
	{
		if (CurrentState is not EngagingEnemy)
		{
			CurrentState.HostileDetected(hostile);
		}
	}

	public void HostileLost(Node3D hostile)
	{
		if (CurrentState is EngagingEnemy currentState)
		{
			currentState.HostileLost(hostile);
		}
	}
}
