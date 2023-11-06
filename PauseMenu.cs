using Godot;
using System;

public partial class PauseMenu : Control
{
	[Signal]
	public delegate void SaveAndQuitEventHandler();
	[Signal]
	public delegate void QuitEventHandler();

	private Button _continue;
	private Button _quit;
	private Button _saveAndQuit;

	public override void _Ready()
	{
		_continue = GetNode<Button>("Panel/SplitContainer/Continue");
		_saveAndQuit = GetNode<Button>("Panel/SplitContainer/SaveAndQuit");
		_quit = GetNode<Button>("Panel/SplitContainer/Quit");

		_continue.Pressed += OnContinuePressed;
		_saveAndQuit.Pressed += OnSaveAndQuitPressed;
		_quit.Pressed += OnQuitPressed;

	}

	private void OnContinuePressed()
	{
		Hide();
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		GetTree().Paused = false;
	}

	private void OnSaveAndQuitPressed()
	{
		EmitSignal(SignalName.SaveAndQuit);
	}

	private void OnQuitPressed()
	{
		EmitSignal(SignalName.Quit);
	}
}
