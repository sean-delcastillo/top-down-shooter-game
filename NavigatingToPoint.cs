using Godot;
using Godot.Collections;

public partial class NavigatingToPoint : EnemyState
{
	private int _routeIndex;
	private int _pointIndex;

	public override void Enter(Dictionary RoutePoint = null)
	{
		if (RoutePoint != null)
		{
			_routeIndex = (int)RoutePoint["Route"];
			_pointIndex = (int)RoutePoint["Point"];
			StateManager.Character.NavManager.SetCurrentRoute(_routeIndex, _pointIndex);
		}
		else
		{
			var point = StateManager.Character.NavManager.GetCurrentPoint();
			_routeIndex = point.RouteId;
			_pointIndex = point.RouteOrder;
		}
		Vector3 targetPosition = StateManager.Character.GetClosestPointOnMap(StateManager.Character.NavManager.GetCurrentPointGlobalPosition());

		//GD.Print("Going to Point " + _routeIndex + " " + _pointIndex + " @ " + targetPosition);

		StateManager.Character.NavAgent.TargetPosition = targetPosition;
	}

	public override void Update(double Delta)
	{
		if (StateManager.Character._visionAwareness.BodiesInVisualContact.Count > 1)
		{
			var seenEntity = StateManager.Character._visionAwareness.BodiesInVisualContact[0];
			if (seenEntity.IsInGroup("Enemies"))
			{
				return;
			}
			var seenEnemy = seenEntity;
			//GD.Print("I see you, " + seenEnemy);
			Dictionary dict = new() {
				{ "SeenEnemy", seenEnemy }
			};
			StateManager.TransitionTo("EngagingEnemy", dict);
		}

		if (StateManager.Character.NavAgent.IsNavigationFinished())
		{
			Dictionary dict = new() {
				{ "Route", _routeIndex},
				{ "Point", _pointIndex}
			};

			//GD.Print("Reached Point " + _routeIndex + " " + _pointIndex);

			StateManager.TransitionTo("IdleAtPoint", dict);
		}
	}
}
