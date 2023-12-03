using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class ObjectiveManager : Node
{
    [Signal]
    public delegate void AllCollectableCollectedEventHandler(string type);
    [Signal]
    public delegate void ExtractionEnteredEventHandler();
    [Signal]
    public delegate void CollectableCollectedEventHandler(string type, int currentCollectableCollectedCount, int maxCollectable);
    [Signal]
    public delegate void ExtractionsActivatedEventHandler(Array<string> names);


    [ExportGroup("Collectable Objectives")]
    [Export]
    public int ActivatedCollectableObjectivesCount = 3;

    [ExportGroup("Extraction Objectives")]
    [Export]
    public int ActivatableExtractionsCount = 3;
    [Export]
    public int CollectablesCollectedToTriggerExtractions = 3;

    public System.Collections.Generic.Dictionary<CollectableObjective.ObjectiveType, (int, int)> ObjectiveCounts;
    public System.Collections.Generic.Dictionary<string, bool> ExtractionStates;

    private List<CollectableObjective> _activatedCollectables;
    private List<CollectableObjective> _possibleCollectable;
    private List<ExtractionPoint> _extracts;
    private int _possibleCollectableCollected;
    private RandomNumberGenerator _rng;
    private bool extractsOpened = false;

    public override void _Ready()
    {
        _rng = new();
        _rng.Randomize();

        _possibleCollectable = new();
        _activatedCollectables = new();
        _extracts = new();
        ExtractionStates = new();

        ObjectiveCounts = new();

        // Initialize (currentCount = 0, maxCount = 0) for each collectable objective types
        foreach (string name in Enum.GetNames(typeof(CollectableObjective.ObjectiveType)))
        {
            ObjectiveCounts.Add(Enum.Parse<CollectableObjective.ObjectiveType>(name), (0, 0));
        }

        // Count all objectives and store references
        foreach (Node child in GetChildren())
        {
            if (child is CollectableObjective collectable)
            {
                _possibleCollectable.Add(collectable);
                collectable.Deactivate();
                collectable.CollectibleCollected += OnCollectibleCollected;
            }
            else if (child is ExtractionPoint extract)
            {
                _extracts.Add(extract);
                extract.Deactivate();
                ExtractionStates.Add(extract.ExtractionName, false);
                extract.ExtractionEntered += OnExtractExtractionEntered;
            }
        }

        if (_possibleCollectable.Count > 0)
        {
            var objectives = _possibleCollectable;

            // Activate Guaranteed Collectable Objectives
            foreach (CollectableObjective objective in objectives)
            {
                if (objective.IsGuaranteedObjective == true)
                {
                    objective.Activate();
                    objectives.Remove(objective);
                }
            }

            // Activate Rest of Random Objectives
            for (int i = 0; i < ActivatedCollectableObjectivesCount; i++)
            {
                if (objectives.Count >= ActivatedCollectableObjectivesCount - i)
                {
                    var index = _rng.RandiRange(0, objectives.Count - 1);
                    var randObjective = objectives[index];
                    randObjective.Activate();
                    _activatedCollectables.Add(randObjective);
                    objectives.Remove(randObjective);
                }
            }

            // Set max count for objectives that are activated
            foreach (CollectableObjective objective in _activatedCollectables)
            {
                if (ObjectiveCounts.TryGetValue(objective.Type, out (int, int) value))
                {
                    value.Item2++;
                    ObjectiveCounts[objective.Type] = value;
                }
            }
        }
    }

    public void ActivateRandomExtraction()
    {
        var extracts = _extracts;

        for (int i = 0; i < ActivatableExtractionsCount; i++)
        {
            if (extracts.Count >= ActivatableExtractionsCount - i)
            {
                var randomIdx = _rng.RandiRange(0, _extracts.Count - 1);
                var randomExtract = extracts[randomIdx];
                extracts.Remove(randomExtract);
                randomExtract.Activate();
                List<string> extractNameList = new()
                {
                    randomExtract.ExtractionName
                };
                EmitSignal(SignalName.ExtractionsActivated, extractNameList.ToArray());
                extractsOpened = true;
            }
        }
    }

    private void OnCollectibleCollected(string type)
    {
        // ObjectiveCounts's value is a tuple (currentCount, maxCount) for the corresponding objective type
        var objectiveKey = Enum.Parse<CollectableObjective.ObjectiveType>(type);
        var objectiveCounts = ObjectiveCounts[objectiveKey];

        objectiveCounts.Item1++;
        ObjectiveCounts[objectiveKey] = objectiveCounts;

        EmitSignal(SignalName.CollectableCollected, type, objectiveCounts.Item1, objectiveCounts.Item2);
        //GD.Print(type.Capitalize() + " Collected :: " + objectiveCounts.Item1 + " / " + objectiveCounts.Item2);

        if (objectiveCounts.Item1 >= objectiveCounts.Item2)
        {
            //GD.Print("All " + type.Capitalize() + " Objectives Collected!");
            EmitSignal(SignalName.AllCollectableCollected, type);
        }

        //GD.Print("Total Collected for Extract " + TotalCollectedObjectivesCount() + " / " + CollectablesCollectedToTriggerExtractions);
        if (!extractsOpened && TotalCollectedObjectivesCount() >= CollectablesCollectedToTriggerExtractions)
        {
            ActivateRandomExtraction();
        }
    }

    private void OnExtractExtractionEntered()
    {
        EmitSignal(SignalName.ExtractionEntered);
    }

    private int TotalCollectedObjectivesCount()
    {
        int sum = 0;
        foreach ((int, int) counts in ObjectiveCounts.Values)
        {
            sum += counts.Item1;
        }
        return sum;
    }
}
