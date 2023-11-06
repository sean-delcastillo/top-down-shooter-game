using Godot;
using System;
using System.Text.RegularExpressions;

public partial class MainMenu : Node2D
{
	private Button _NewGame;
	private Button _Continue;
	private Button _Settings;
	private Button _Quit;
	private HSlider _VolumeSlider;
	private AudioStreamPlayer _Audio;

	private enum ButtonPressed
	{
		New,
		Continue,
		Quit
	}

	public override void _Ready()
	{
		_NewGame = GetNode<Button>("MarginContainer/VBoxContainer/NewGame");
		_Continue = GetNode<Button>("MarginContainer/VBoxContainer/Continue");
		_Quit = GetNode<Button>("MarginContainer/VBoxContainer/Quit");
		_VolumeSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/VBoxContainer/HSlider");

		_NewGame.Pressed += () => { ButtonHandler(ButtonPressed.New); };
		_Continue.Pressed += () => { ButtonHandler(ButtonPressed.Continue); };
		_Quit.Pressed += () => { ButtonHandler(ButtonPressed.Quit); };
		_VolumeSlider.ValueChanged += OnVolumeValueChanged;
		_Audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer2D");

		if (!ResourceLoader.Exists("SAVEGAMESCENE.tscn"))
		{
			_Continue.Disabled = true;
		}
	}


	private void ButtonHandler(ButtonPressed button)
	{
		switch (button)
		{
			case ButtonPressed.New:
				GetTree().ChangeSceneToFile("Maze1.tscn");
				break;
			case ButtonPressed.Continue:
				GetTree().ChangeSceneToFile("SAVEGAMESCENE.tscn");
				break;
			case ButtonPressed.Quit:
				GetTree().Quit();
				break;
		}
	}

	private void OnVolumeValueChanged(double value)
	{
		_Audio.VolumeDb = (float)Mathf.LinearToDb(value);

		player_prefs playerPrefs = GetNode<player_prefs>("/root/PlayerPrefs");
		playerPrefs.VolumeValue = _Audio.VolumeDb;

	}
}
