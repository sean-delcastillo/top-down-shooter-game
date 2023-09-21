using Godot;
using System;
using System.Dynamic;

public partial class EnemyCharacterController : CharacterBody3D
{
	[Export]
	public bool IsComputerControlled { set; get; } = false;
	[Export]
	public double Health { set; get; } = 200;
	[Export]
	public CharacterWeapon Weapon { set; get; }

	private PackedScene _deathSpriteAnimation = GD.Load<PackedScene>("res://death_animation_sprite.tscn");
	private PackedScene _hurtSpurtParticles = GD.Load<PackedScene>("res://hurt_spurt.tscn");
	private NavigationAgent3D _navigation;

	public override void _Ready()
	{
		_navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
	}

	public override void _Process(double delta)
	{
		if (Health <= 0)
		{
			Die();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsComputerControlled)
		{

		}

		MoveAndSlide();
	}

	public void TakeDamage(Double damage)
	{
		Health -= damage;
	}

	public void DamageAtLocation(Vector3 at, Vector3 normal)
	{
		var hurtParticles = _hurtSpurtParticles.Instantiate() as HurtSpurt;
		hurtParticles.LookAtFromPosition(at, normal);
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
