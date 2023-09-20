using Godot;
using System;

public partial class Main : Node3D
{
	private CameraController _cameraRig;
	private PlayerCharacterController _player;
	private EnemyCharacterController _enemy;

	public override void _Ready()
	{
		_cameraRig = GetNode<CameraController>("CameraRig");
		_player = GetNode<PlayerCharacterController>("PlayerCharacter");
		_enemy = GetNode<EnemyCharacterController>("EnemyCharacter");

		_cameraRig.MousePositionInWorldChanged += SendNewMousePositionToPlayer;
	}

	private void SendNewMousePositionToPlayer(Vector3 position)
	{
		_player.Call("UpdateMousePositionInWorld", position);
	}

	private void HandleEnemyDeath(Vector3 position, PackedScene deathAnimation)
	{
		var animation = deathAnimation.Instantiate() as AnimatedSprite3D;
		AddChild(animation);
		animation.Position = position;
		animation.Play();
		if (!animation.IsPlaying())
		{
			animation.QueueFree();
		}
	}
}
