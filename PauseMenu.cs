using Godot;
using System;

public partial class PauseMenu : Control
{
	[Signal]
	public delegate void AbandonRunEventHandler();
	[Signal]
	public delegate void QuitToDesktopEventHandler();

	private Button _continue;
	private Button _quit;
	private Button _abandon;

	public override void _Ready()
	{
		_continue = GetNode<Button>("%Continue");
		_abandon = GetNode<Button>("%Abandon");
		_quit = GetNode<Button>("%Quit");

		_continue.Pressed += OnContinuePressed;
		_abandon.Pressed += AbandonRunPressed;
		_quit.Pressed += OnQuitToDesktopPressed;

	}

	private void OnContinuePressed()
	{
		Hide();
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		GetTree().Paused = false;
	}

	private void AbandonRunPressed()
	{
		EmitSignal(SignalName.AbandonRun);
	}

	private void OnQuitToDesktopPressed()
	{
		EmitSignal(SignalName.QuitToDesktop);
	}
}
