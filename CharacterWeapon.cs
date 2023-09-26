using Godot;
using System;

public partial class CharacterWeapon : Node3D
{
	[Export]
	public RayCast3D GunRay { set; get; }

	[Export]
	public float Damage { set; get; }

	[Export]
	public PackedScene RangeTrail { set; get; }

	public void PrimaryAction()
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
	}

	private void BulletTrail(Vector3 to)
	{
		var bulletTrail = RangeTrail.Instantiate<BulletTrail>();
		bulletTrail.Init(GunRay.GlobalPosition, to);
		GetTree().Root.AddChild(bulletTrail);
	}
}
