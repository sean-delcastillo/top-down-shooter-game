using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class PlayerPrefs : Node, Persistence.IPersistent
{
    public float VolumeValue { set; get; } = 1;

    public Godot.Collections.Dictionary<string, Variant> Save()
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            {"VolumeValue", VolumeValue}
        };
    }

    public void Load()
    {
        var configData = Persistence.LoadConfigFile();
        VolumeValue = (float)configData.GetValueOrDefault("VolumeValue");
    }
}
