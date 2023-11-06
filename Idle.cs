using Godot;
using Godot.Collections;
using System;

public partial class Idle : EnemyState
{
	[Export]
	public int InitialRoute { set; get; } = 0;
	[Export]
	public int InitialPoint { set; get; } = 0;

	public override void Enter(Dictionary _ = null)
	{
		Dictionary dict = new()
		{
			{ "Route", InitialPoint },
			{ "Point", InitialPoint }
		};
		StateManager.TransitionTo("NavigatingToPoint", dict);
	}
}
