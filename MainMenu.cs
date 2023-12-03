using System.Collections.Generic;
using Godot;

public partial class MainMenu : Node2D
{
	private HSlider _volumeSlider;
	private Button _continue;
	private Button _newGame;
	private Button _quitToDesktop;
	private AudioStreamPlayer _music;
	private SaveFilePanel _saveFilePanel;
	private Button _howToPlay;
	private PanelContainer _manual;

	public override void _Ready()
	{
		_volumeSlider = GetNode<HSlider>("%VolumeSlider");
		_continue = GetNode<Button>("%Continue");
		_newGame = GetNode<Button>("%NewGame");
		_quitToDesktop = GetNode<Button>("%QuitToDesktop");
		_music = GetNode<AudioStreamPlayer>("%MusicPlayer");
		_saveFilePanel = GetNode<SaveFilePanel>("%SaveFilePanel");
		_howToPlay = GetNode<Button>("%HowToPlay");
		_manual = GetNode<PanelContainer>("%ManualPanel");

		_volumeSlider.ValueChanged += OnVolumeSliderValueChanged;
		_continue.Pressed += OnContinueButtonPressed;
		_newGame.Pressed += OnNewGameButtonPressed;
		_quitToDesktop.Pressed += OnQuitToDesktopButtonPressed;
		_saveFilePanel.SaveFileDeleted += OnSaveFilePanelSaveFileDeleted;
		_howToPlay.Pressed += OnHowToPlayButtonPressed;

		if (!Persistence.ConfigFileExists())
		{
			PlayerPrefs playerPrefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
			Persistence.SaveToConfigFile(playerPrefs.Save());
		}
		else
		{
			PlayerPrefs playerPrefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
			var configData = Persistence.LoadConfigFile();
			playerPrefs.VolumeValue = (float)configData.GetValueOrDefault("VolumeValue");
			_volumeSlider.Value = Godot.Mathf.DbToLinear(playerPrefs.VolumeValue);
		}

		if (!Persistence.SaveFileExists())
		{
			_continue.Disabled = true;
			PlayerProgression playerProgression = GetNode<PlayerProgression>("/root/PlayerProgression");
			playerProgression.Reset();
		}
		else
		{
			PlayerProgression playerProgression = GetNode<PlayerProgression>("/root/PlayerProgression");
			playerProgression.Load();
			/*
			var saveData = Persistence.LoadSaveFile();
			PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");
			progression.Money = (int)saveData.GetValueOrDefault("Money");
			progression.MostMoneyInOneRun = (int)saveData.GetValueOrDefault("MostMoneyInOneRun");
			*/
		}
	}

	private void OnVolumeSliderValueChanged(double value)
	{
		_music.VolumeDb = (float)Mathf.LinearToDb(value);

		PlayerPrefs playerPrefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
		playerPrefs.VolumeValue = _music.VolumeDb;
	}

	private void OnContinueButtonPressed()
	{
		if (!Persistence.SaveFileExists())
		{
			_continue.Disabled = true;
		}
		else
		{
			PlayerPrefs playerPrefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
			Persistence.SaveToConfigFile(playerPrefs.Save());
			GetTree().ChangeSceneToFile("res://TestingGrounds.tscn");
		}
	}

	private void OnNewGameButtonPressed()
	{
		PlayerProgression progression = GetNode<PlayerProgression>("/root/PlayerProgression");
		progression.Money = 0;
		progression.MostMoneyInOneRun = 0;
		Persistence.SaveToSaveFile(progression.Save());

		PlayerPrefs playerPrefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
		Persistence.SaveToConfigFile(playerPrefs.Save());

		GetTree().ChangeSceneToFile("res://TestingGrounds.tscn");
	}

	private void OnQuitToDesktopButtonPressed()
	{
		PlayerPrefs playerPrefs = GetNode<PlayerPrefs>("/root/PlayerPrefs");
		Persistence.SaveToConfigFile(playerPrefs.Save());

		GetTree().Quit();
	}

	private void OnSaveFilePanelSaveFileDeleted()
	{
		if (!Persistence.SaveFileExists())
		{
			_continue.Disabled = true;
		}
	}

	private void OnHowToPlayButtonPressed()
	{
		_manual.Visible = true;
	}
}
