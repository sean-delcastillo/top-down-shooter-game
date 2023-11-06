using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
public partial class NavigationPointManager : Node
{
	public int CurrentRouteIndex { set; get; } = 0;
	public int CurrentPointIndex { set; get; } = 0;

	private System.Collections.Generic.List<NavigationPoint> _navigationPoints;
	private System.Collections.Hashtable _navigationRoutes;

	public override void _Ready()
	{
		_navigationPoints = new();
		_navigationRoutes = new();

		var children = GetChildren();
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] is NavigationPoint navPoint)
			{
				_navigationPoints.Add(navPoint);
			}
		}

		for (int i = 0; i < _navigationPoints.Count; i++)
		{
			var point = _navigationPoints[i];
			if (point.isPartOfRoute)
			{
				var id = point.RouteId;
				if (!_navigationRoutes.ContainsKey(id))
				{
					var list = new List<NavigationPoint>
					{
						point
					};
					_navigationRoutes.Add(id, list);
				}
				else
				{
					var list = (List<NavigationPoint>)_navigationRoutes[id];
					list.Add(point);
					list.Sort(ComparePointsByRouteOrder);
					_navigationRoutes.Remove(id);
					_navigationRoutes.Add(id, list);
				}
			}
		}
	}

	/// <summary>
	/// Sets manager's current routing state.
	/// </summary>
	/// <param name="patrolId">Patrol identified by integer ID key</param>
	/// <param name="patrolPoint">Point within a patrol identified by order integer</param>
	public void SetCurrentRoute(int routeId, int routePoint = 0)
	{
		if (_navigationRoutes.ContainsKey(routeId))
		{
			CurrentRouteIndex = routeId;
			CurrentPointIndex = routePoint;
		}
	}

	/// <summary>
	/// Advances the current path position state, handling wrapping around to the first position on overflow.
	/// </summary>
	/// <returns>The next NavigationPoint on the path</returns>
	public NavigationPoint GetNextPointOnRoute()
	{
		// List from GetCurrentRoute() is sorted by RouteOrder
		if (CurrentPointIndex < GetCurrentRoute()[^1].RouteOrder)
		{
			CurrentPointIndex++;
		}
		else
		{
			CurrentPointIndex = 0;
		}
		return GetCurrentPoint();
	}

	public Vector3 GetNextPointOnRouteGlobalPosition()
	{
		return GetNextPointOnRoute().GlobalPosition;
	}

	public NavigationPoint GetCurrentPoint()
	{
		return ((List<NavigationPoint>)_navigationRoutes[CurrentRouteIndex])[CurrentPointIndex];
	}

	public Vector3 GetCurrentPointGlobalPosition()
	{
		return GetCurrentPoint().GlobalPosition;
	}

	public List<NavigationPoint> GetCurrentRoute()
	{
		return (List<NavigationPoint>)_navigationRoutes[CurrentRouteIndex];
	}

	public List<int> GetPossibleRouteIds()
	{
		List<int> list = new();
		foreach (int key in _navigationRoutes.Keys)
		{
			list.Add(key);
		}
		list.Sort();
		return list;
	}

	private static int ComparePointsByRouteOrder(NavigationPoint x, NavigationPoint y)
	{
		if (x == null)
		{
			if (y == null)
			{
				return 0;
			}
			else
			{
				return -1;
			}
		}
		else
		{
			if (y == null)
			{
				return 1;
			}
			else
			{
				int retVal = x.RouteOrder.CompareTo(y.RouteOrder);

				if (retVal != 0)
				{
					return retVal;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
