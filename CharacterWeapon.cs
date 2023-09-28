using Godot;
using System;

public partial class CharacterWeapon : Node3D
{
	[Export]
	public RayCast3D GunRay { set; get; }

	[Export]
	public float Damage { set; get; }

	[Export]
	public int FireRate { set; get; }

	[Export]
	public PackedScene RangeTrail { set; get; }

	private bool _canFire = true;
	private double _secondsPerRound;
	private Area3D _collisionArea;

	private Timer _fireRateTimer;

	public override void _Ready()
	{
		_fireRateTimer = GetNode<Timer>("FireRateTimer");
		_collisionArea = GetNode<Area3D>("GunMesh/CrowdingCollisionArea");

		CalculateFireRateTimer();
	}

	public override void _Process(double delta)
	{
		if (_collisionArea.HasOverlappingBodies())
		{
			_canFire = false;
		}
		else
		{
			_canFire = true;
		}
	}

	public void PrimaryAction()
	{
		if (!_fireRateTimer.IsStopped() || !_canFire)
		{
			return;
		}
		else
		{
			if (GunRay.IsColliding())
			{
				BulletTrail(GunRay.GetCollisionPoint());
				var target = GunRay.GetCollider() as Node3D;
				if (target is CharacterBody3D characterTarget)
				{
					characterTarget.Call("TakeDamage", Damage);
					characterTarget.Call("DamageAtLocation", GunRay.GetCollisionPoint(), GunRay.GetCollisionNormal());
				}
			}
			else
			{
				BulletTrail(GlobalTransform * GunRay.TargetPosition);
			}
			_fireRateTimer.Start(_secondsPerRound);
		}
	}

	private void CalculateFireRateTimer()
	{
		_secondsPerRound = Math.ReciprocalEstimate(FireRate / 60f);
	}

	private void BulletTrail(Vector3 to)
	{
		var bulletTrail = RangeTrail.Instantiate<BulletTrail>();
		bulletTrail.Init(GunRay.GlobalPosition, to);
		GetTree().Root.AddChild(bulletTrail);
	}
}
