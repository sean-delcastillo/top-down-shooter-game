using Godot;
using System;

public partial class World : Node3D
{
	private CameraController _cameraRig;
	private PlayerCharacterController _player;
	private EnemyCharacterController _enemy;

	public override void _Ready()
	{
		_cameraRig = GetNode<CameraController>("CameraRig");
		_player = GetNode<PlayerCharacterController>("PlayerCharacter");

		_cameraRig.MousePositionInWorldChanged += SendNewMousePositionToPlayer;
	}

	private void SendNewMousePositionToPlayer(Vector3 position)
	{
		_player.Call("UpdateMousePositionInWorld", position);
	}
}
