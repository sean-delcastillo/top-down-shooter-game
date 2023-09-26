using Godot;
using System;

public partial class MainMenu : Node2D
{
	private Button _sceneOneButton;
	private Button _sceneTwoButton;

	private enum Level
	{
		One,
		Two
	}

	public override void _Ready()
	{
		_sceneOneButton = GetNode<Button>("MarginContainer/VBoxContainer/SceneOne");
		_sceneTwoButton = GetNode<Button>("MarginContainer/VBoxContainer/SceneTwo");

		_sceneOneButton.Pressed += () => HandleButtonPress(Level.One);
		_sceneTwoButton.Pressed += () => HandleButtonPress(Level.Two);
	}

	private void HandleButtonPress(Level level)
	{
		switch (level)
		{
			case Level.One:
				GetTree().ChangeSceneToFile("res://move_and_shoot.tscn");
				break;
			case Level.Two:
				GetTree().ChangeSceneToFile("res://vision_test.tscn");
				break;
			default:
				break;
		}
	}
}
