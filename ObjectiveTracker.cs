using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class ObjectiveTracker : MarginContainer
{
	public System.Collections.Generic.Dictionary<CollectableObjective.ObjectiveType, (int, int)> ObjectiveCounts { set; get; }
	public System.Collections.Generic.Dictionary<string, bool> ExtractionStates { set; get; }

	private VBoxContainer _mainContainer;
	private readonly System.Collections.Generic.Dictionary<CollectableObjective.ObjectiveType, string> DisplayNames = new()
	{
		{CollectableObjective.ObjectiveType.GoldBall, "Rare Ball"},
		{CollectableObjective.ObjectiveType.RedBall, "Uncommon Ball"},
		{CollectableObjective.ObjectiveType.GreenBall, "Common Ball"}
	};

	private PackedScene _countedObjective;
	private PackedScene _boolObjective;
	private System.Collections.Generic.Dictionary<string, Node> _objectiveTrack;

	public override async void _Ready()
	{
		_mainContainer = GetNode<VBoxContainer>("%MainContainer");
		var exampleCounted = GetNode<CountedObjective>("%ExampleCountedObjective");
		var exampleBool = GetNode<BoolObjective>("%ExampleBoolObjective");

		exampleCounted.QueueFree();
		exampleBool.QueueFree();

		_countedObjective = GD.Load<PackedScene>("res://counted_objective.tscn");
		_boolObjective = GD.Load<PackedScene>("res://bool_objective.tscn");

		_objectiveTrack = new();

		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		foreach (KeyValuePair<CollectableObjective.ObjectiveType, (int, int)> counts in ObjectiveCounts)
		{
			AddCountedObjective(counts.Key, counts.Value.Item1, counts.Value.Item2);
		}

		foreach (KeyValuePair<string, bool> extractState in ExtractionStates)
		{
			AddBoolObjective(extractState.Key, extractState.Value);
		}
	}

	public void AddBoolObjective(string name, bool state)
	{
		if (state is true)
		{
			var objectiveLabel = _boolObjective.Instantiate<BoolObjective>();
			_mainContainer.AddChild(objectiveLabel);
			_mainContainer.MoveChild(objectiveLabel, 0);
			_objectiveTrack.Add(name, objectiveLabel);
			objectiveLabel.UpdateLabel("\"" + name.Capitalize() + "\"" + " Exit", "Available");
		}
	}

	public void AddCountedObjective(CollectableObjective.ObjectiveType objectiveName, int currentObjectiveCount, int maxObjectiveCount)
	{
		if (maxObjectiveCount > 0)
		{
			string objectiveLabelName;

			// If display name enum contains a display name, use it
			if (DisplayNames.TryGetValue(objectiveName, out string displayName))
			{
				objectiveLabelName = displayName;
			}
			// Else use the default string in the ObjectiveType definition
			else
			{
				objectiveLabelName = Enum.GetName<CollectableObjective.ObjectiveType>(objectiveName).Capitalize();
			}

			var objectiveLabel = _countedObjective.Instantiate<CountedObjective>();
			_mainContainer.AddChild(objectiveLabel);
			_mainContainer.MoveChild(objectiveLabel, 0);
			_objectiveTrack.Add(Enum.GetName(objectiveName), objectiveLabel);
			objectiveLabel.UpdateLabel(objectiveLabelName, currentObjectiveCount, maxObjectiveCount);
		}
	}

	public void OnObjectiveManagerCollectibleCollected(string type, int currentCollectableCollectedCount, int maxCollectable)
	{
		if (_objectiveTrack.TryGetValue(type, out Node collectable))
		{
			var coll = collectable as CountedObjective;
			coll.UpdateLabel(currentCollectableCollectedCount, maxCollectable);
		}
	}

	public void OnObjectiveManagerAllCollectableCollected(string type)
	{
		if (_objectiveTrack.TryGetValue(type, out Node collectable))
		{
			var coll = collectable as CountedObjective;
			_mainContainer.MoveChild(coll, -1);
			coll.SetCompleted();
		}
	}

	public void OnObjectiveManagerExtractionsActivated(Array<string> extractionName)
	{
		foreach (string name in extractionName)
		{
			AddBoolObjective(name, true);
		}
	}
}
