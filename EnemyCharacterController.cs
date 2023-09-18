using Godot;
using System;

public partial class EnemyCharacterController : CharacterBody3D
{
	[Signal]
	public delegate void EnemyDeathOccuredEventHandler(Vector3 position, PackedScene sceneToSpawn);

	private double _health = 200;

	private PackedScene _deathSpriteAnimation = GD.Load<PackedScene>("res://death_animation_sprite.tscn");

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (_health <= 0)
		{
			Die();
		}
	}

	public void TakeDamage(Double damage)
	{
		_health -= damage;
	}

	private void Die()
	{
		EmitSignal(SignalName.EnemyDeathOccured, Position, _deathSpriteAnimation);

		QueueFree();
	}
}
