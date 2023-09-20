using Godot;
using System;

public partial class EnemyCharacterController : CharacterBody3D
{
	private double _health = 200;

	private PackedScene _deathSpriteAnimation = GD.Load<PackedScene>("res://death_animation_sprite.tscn");
	private PackedScene _hurtSpurtParticles = GD.Load<PackedScene>("res://hurt_spurt.tscn");

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

	public void DamageAtLocation(Vector3 at, Vector3 normal)
	{
		var hurtParticles = _hurtSpurtParticles.Instantiate() as GpuParticles3D;
		hurtParticles.Position = at;
		hurtParticles.LookAt(-normal);
		GetParent().AddChild(hurtParticles);
	}

	private void Die()
	{
		var deathAnimation = _deathSpriteAnimation.Instantiate() as Node3D;
		deathAnimation.GlobalPosition = GlobalPosition;
		GetParent().AddChild(deathAnimation);
		QueueFree();
	}
}
