using Godot;
using System;
using System.Dynamic;

public partial class EnemyCharacterController : CharacterBody3D
{
	[Export]
	public CharacterInformation CharacterInformation { set; get; }
	[Export]
	public bool IsComputerControlled { set; get; } = false;
	[Export]
	public CharacterWeapon Weapon { set; get; }

	private NavigationAgent3D _navigation;
	private VisionAwareness _visionAwareness;

	public override void _Ready()
	{
		_navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_visionAwareness = GetNode<VisionAwareness>("VisionAwareness");
	}

	public override void _Process(double delta)
	{
		if (CharacterInformation.Health <= 0)
		{
			Die();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsComputerControlled)
		{
			FaceVisualContacts();
			FireControl();
		}

		MoveAndSlide();
	}

	public void TakeDamage(double damage)
	{
		CharacterInformation.Health -= damage;
	}

	public void DamageAtLocation(Vector3 at, Vector3 normal)
	{
		var hurtParticles = CharacterInformation.HurtAnimation.Instantiate() as HurtSpurt;
		hurtParticles.LookAtFromPosition(at, normal);
		GetParent().AddChild(hurtParticles);
	}

	private void Die()
	{
		var deathAnimation = CharacterInformation.DeathAnimation.Instantiate() as Node3D;
		deathAnimation.GlobalPosition = GlobalPosition;
		GetParent().AddChild(deathAnimation);
		QueueFree();
	}

	private void FaceVisualContacts()
	{
		if (_visionAwareness.BodiesInVisualContact.Count > 0)
		{
			LookAt(_visionAwareness.BodiesInVisualContact[0].GlobalPosition);
		}
	}

	private void FireControl()
	{
		if (_visionAwareness.Facing != null)
		{
			Weapon.PrimaryAction();
		}
	}
}
