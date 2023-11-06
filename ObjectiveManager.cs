using Godot;
using System;
using System.Collections.Generic;

public partial class ObjectiveManager : Node
{
    [Signal]
    public delegate void AllMcguffinsCollectedEventHandler();
    [Signal]
    public delegate void ExtractionEnteredEventHandler();
    [Signal]
    public delegate void McguffinCollectedEventHandler(int currentMcguffinsCollectedCount, int maxMcguffins);

    private List<CollectableObjective> _mcguffins;
    private List<CollectableObjective> _activatedMcguffins;
    private List<ExtractionPoint> _extracts;
    private int _mcguffinsCollected;
    private RandomNumberGenerator _rng;
    private List<int> _validObjectiveIndices;

    public override void _Ready()
    {
        _rng = new();
        _rng.Randomize();

        _validObjectiveIndices = new();
        _activatedMcguffins = new();

        _mcguffins = new();
        _extracts = new();

        foreach (Node child in GetChildren())
        {
            if (child is CollectableObjective collectable)
            {
                _mcguffins.Add(collectable);
                collectable.Deactivate();
                collectable.McguffinCollected += OnCollectableMcguffinCollected;
            }
            else if (child is ExtractionPoint extract)
            {
                _extracts.Add(extract);
                extract.ExtractionEntered += OnExtractExtractionEntered;
            }
        }

        ChooseValidObjectives();

        GD.Print("VALID OBJECTIVES " + _activatedMcguffins.Count + " / " + _mcguffins.Count);
    }

    public ExtractionPoint ActivateRandomExtraction()
    {
        var randomIdx = _rng.RandiRange(0, _extracts.Count);
        _extracts[randomIdx].ActivateExtraction();
        return _extracts[randomIdx];
    }

    private void OnCollectableMcguffinCollected()
    {
        _mcguffinsCollected++;
        GD.Print("MCGUFFIN COLLECTED. " + _mcguffinsCollected + " / " + _activatedMcguffins.Count);
        EmitSignal(SignalName.McguffinCollected, _mcguffinsCollected, _activatedMcguffins.Count);
        if (_mcguffinsCollected >= _activatedMcguffins.Count)
        {
            EmitSignal(SignalName.AllMcguffinsCollected);
        }
    }

    private void OnExtractExtractionEntered()
    {
        EmitSignal(SignalName.ExtractionEntered);
    }

    private void ChooseValidObjectives()
    {
        for (int i = 0; i <= 3; i++)
        {
            ChooseUniqueIdentifiers();
        }

        foreach (int idx in _validObjectiveIndices)
        {
            _mcguffins[idx].Activate();
            _activatedMcguffins.Add(_mcguffins[idx]);
        }
    }

    public void ChooseUniqueIdentifiers()
    {
        if (_validObjectiveIndices.Count >= 3)
        {
            return;
        }
        else
        {
            var randomIdx = _rng.RandiRange(0, _mcguffins.Count - 1);
            if (_validObjectiveIndices.Contains(randomIdx))
            {
                ChooseUniqueIdentifiers();
            }
            else
            {
                _validObjectiveIndices.Add(randomIdx);
            }
        }
    }
}
