using Godot;
using System;

public partial class World : Node3D
{
	private CameraController _cameraRig;
	private PlayerCharacterController _player;
	private EnemyCharacterController _enemy;

	private Button _returnButton;

	public override void _Ready()
	{
		_cameraRig = GetNode<CameraController>("CameraRig");
		_player = GetNode<PlayerCharacterController>("PlayerCharacter");
		_returnButton = GetNode<Button>("UI/Control/MarginContainer/ReturnButton");

		_cameraRig.MousePositionInWorldChanged += SendNewMousePositionToPlayer;
		_returnButton.Pressed += OnReturnButtonPressed;
	}

	private void SendNewMousePositionToPlayer(Vector3 position)
	{
		_player.Call("UpdateMousePositionInWorld", position);
	}

	private void OnReturnButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://main_menu.tscn");
	}
}
