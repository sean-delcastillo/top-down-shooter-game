using Godot;
using Godot.Collections;

static class Persistence
{
	/// <summary>
	/// Persistent classes have serializable data that should be saved onto the filesystem on save and can be loaded from the filesystem.
	/// </summary>
	public interface IPersistent
	{
		public Dictionary<string, Variant> Save();

		public void Load();
	}

	public static void SaveToSaveFile(Dictionary<string, Variant> nodeData)
	{
		FileAccess saveFile = GetSaveFile(FileAccess.ModeFlags.Write);
		saveFile.StoreLine(Json.Stringify(nodeData));
		saveFile.Dispose();
	}

	public static void SaveToConfigFile(Dictionary<string, Variant> data)
	{
		FileAccess configFile = GetConfigFile(FileAccess.ModeFlags.Write);
		configFile.StoreLine(Json.Stringify(data));
		configFile.Dispose();
	}

	public static bool SaveFileExists()
	{
		return FileAccess.FileExists("user://savegame.save");
	}

	public static bool ConfigFileExists()
	{
		return FileAccess.FileExists("user://config.config");
	}

	public static FileAccess GetSaveFile(FileAccess.ModeFlags flag)
	{
		return FileAccess.Open("user://savegame.save", flag);
	}

	public static FileAccess GetConfigFile(FileAccess.ModeFlags flag)
	{
		return FileAccess.Open("user://config.config", flag);
	}

	public static Dictionary<string, Variant> LoadSaveFile()
	{
		var saveGame = GetSaveFile(FileAccess.ModeFlags.Read);

		Dictionary<string, Variant> saveData = new();

		while (saveGame.GetPosition() < saveGame.GetLength())
		{
			var jsonString = saveGame.GetLine();

			var json = new Json();
			var parseResult = json.Parse(jsonString);
			if (parseResult != Error.Ok)
			{
				continue;
			}

			var lineData = new Dictionary<string, Variant>((Dictionary)json.Data);

			foreach (System.Collections.Generic.KeyValuePair<string, Variant> value in lineData)
			{
				saveData.Add(value.Key, value.Value);
			}
		}
		saveGame.Dispose();
		return saveData;
	}

	public static Dictionary<string, Variant> LoadConfigFile()
	{
		var configFile = GetConfigFile(FileAccess.ModeFlags.Read);

		Dictionary<string, Variant> configData = new();

		while (configFile.GetPosition() < configFile.GetLength())
		{
			var jsonString = configFile.GetLine();

			var json = new Json();
			var parseResult = json.Parse(jsonString);
			if (parseResult != Error.Ok)
			{
				continue;
			}

			var lineData = new Dictionary<string, Variant>((Dictionary)json.Data);

			foreach (System.Collections.Generic.KeyValuePair<string, Variant> value in lineData)
			{
				configData.Add(value.Key, value.Value);
			}
		}
		configFile.Dispose();
		return configData;
	}
}
