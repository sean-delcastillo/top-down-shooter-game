using Godot;
using Godot.Collections;
using System;

public partial class Idle : EnemyState
{
	[Export]
	public int InitialRoute { set; get; } = 0;
	[Export]
	public int InitialPoint { set; get; } = 0;

	public override async void Enter(Dictionary _ = null)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		transition();
	}

	private void transition()
	{
		Dictionary dict = new()
		{
			{ "Route", InitialPoint },
			{ "Point", InitialPoint }
		};
		StateManager.TransitionTo("NavigatingToPoint", dict);
	}
}
