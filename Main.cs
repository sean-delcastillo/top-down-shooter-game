using Godot;
using System;

public partial class Main : Node3D
{
	public CameraController _cameraRig;
	public KeyboardCharacterController _player;

	public override void _Ready()
	{
		_cameraRig = GetNode<CameraController>("CameraRig");
		_player = GetNode<KeyboardCharacterController>("PlayerCharacter");

		_cameraRig.MousePositionInWorldChanged += SendNewMousePositionToPlayer;
	}

	private void SendNewMousePositionToPlayer(Vector3 position)
	{
		_player.Call("UpdateMousePositionInWorld", position);
	}
}
