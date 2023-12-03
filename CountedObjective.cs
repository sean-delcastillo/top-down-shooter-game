using System.Security.Principal;
using Godot;

public partial class CountedObjective : PanelContainer
{
	public string ObjectiveName
	{
		set { _name.Text = value; }
		get { return _name.Text; }
	}
	public int MaxCount
	{
		set { _maxCount.Text = value.ToString(); }
		get
		{
			if (int.TryParse(_maxCount.Text, out int maxValue))
			{
				return maxValue;
			}
			else { return -1; }
		}
	}
	public int CurrentCount
	{
		set { _currentCount.Text = value.ToString(); }
		get
		{
			if (int.TryParse(_currentCount.Text, out int currentValue))
			{
				return currentValue;
			}
			else { return -1; }
		}
	}

	private Label _name;
	private Label _currentCount;
	private Label _maxCount;

	public override void _Ready()
	{
		_name = GetNode<Label>("%ObjectiveName");
		_currentCount = GetNode<Label>("%CurrentObjectiveCount");
		_maxCount = GetNode<Label>("%MaxObjectiveCount");
	}

	public async void UpdateLabel(string objectiveName, int currentCount, int maxCount)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		ObjectiveName = objectiveName;
		CurrentCount = currentCount;
		MaxCount = maxCount;
	}

	public async void UpdateLabel(int currentCount, int maxCount)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		CurrentCount = currentCount;
		MaxCount = maxCount;
	}

	public void SetCompleted()
	{
		Modulate = new Color(0, 1, 0, 0.25f);
	}

	public void SetUncompleted()
	{
		Modulate = new Color(1, 1, 1, 1);
	}
}
