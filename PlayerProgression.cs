using System.Collections.Generic;
using Godot;

public partial class PlayerProgression : Node, Persistence.IPersistent
{
	public int Money { set; get; } = 0;
	public int MostMoneyInOneRun { set; get; } = 0;
	public int CurrentHealth { set; get; } = 300;
	public float CurrentSpeed { set; get; } = 5;

	public Godot.Collections.Dictionary<string, Variant> Save()
	{
		return new Godot.Collections.Dictionary<string, Variant>
		{
			{"Money", Money},
			{"MostMoneyInOneRun", MostMoneyInOneRun},
			{"CurrentHealth", CurrentHealth},
			{"CurrentSpeed", CurrentSpeed}
		};
	}

	public void Load()
	{
		var saveData = Persistence.LoadSaveFile();
		Money = (int)saveData.GetValueOrDefault("Money");
		MostMoneyInOneRun = (int)saveData.GetValueOrDefault("MostMoneyInOneRun");
		CurrentHealth = (int)saveData.GetValueOrDefault("CurrentHealth");
		CurrentSpeed = (float)saveData.GetValueOrDefault("CurrentSpeed");
	}

	public void Reset()
	{
		Money = 0;
		MostMoneyInOneRun = 0;
		CurrentHealth = 300;
		CurrentSpeed = 5;
	}
}
