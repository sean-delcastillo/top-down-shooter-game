using System;
using System.Collections.Generic;
using Godot;

public interface IPrimaryAction
{
	public abstract void PrimaryAction();
}

public interface ISecondaryAction
{
	public abstract void SecondaryAction();
}

[Tool]
public abstract partial class RangedWeapon : Node
{
	[Signal]
	public delegate void WeaponFiredEventHandler();
	[Signal]
	public delegate void WeaponReloadedEventHandler();

	[Export]
	public float Damage { set; get; }
	[Export]
	public int FireRate { set; get; }
	[Export]
	public int RecoilPerShot { set; get; }
	[Export]
	public int MagazineSize { set; get; }
	[Export]
	public float MaximumRange { set; get; }

	public double CharacterStabilityRecoveryModifier = 0;
	public double CharacterStabilityModifier = 0;

	private RayCast3D _gunRay;
	private Area3D _crowdingArea;
	private MeshInstance3D _aimLaser;
	private Timer _fireFrequency;
	private Timer _recoilRecovery;
	private bool _canFire = false;
	private int _currentMagazineCount = 0;
	private double _stability = 1;


	public override string[] _GetConfigurationWarnings()
	{
		List<string> warnings = new();

		if (_gunRay is null)
		{
			warnings.Add("RangedWeapon needs a GunRay RayCast3D node");
		}
		if (_crowdingArea is null)
		{
			warnings.Add("RangedWeapon needs a Crowding Area3D node");
		}
		if (_aimLaser is null)
		{
			warnings.Add("RangedWeapon needs an AimLaser MeshInstance3D node.");
		}

		return warnings.ToArray();
	}

	public override void _Ready()
	{
		_gunRay = GetNode<RayCast3D>("%GunRay");
		_crowdingArea = GetNode<Area3D>("%Crowding");
		_aimLaser = GetNode<MeshInstance3D>("%AimLaser");
		UpdateConfigurationWarnings();

		_fireFrequency = new()
		{
			OneShot = true,
			WaitTime = Math.ReciprocalEstimate(FireRate / 60f)
		};
		AddChild(_fireFrequency);

		_recoilRecovery = new()
		{
			OneShot = true,
			WaitTime = 0.05
		};
		AddChild(_recoilRecovery);

		_canFire = true;
		_currentMagazineCount = MagazineSize;

		_crowdingArea.BodyEntered += OnCrowdingAreaBodyEntered;
	}

	public override void _Process(double delta)
	{
		// Recover Stability over time
		if (_stability <= 0.9 && _recoilRecovery.IsStopped())
		{
			_stability += 0.1 + CharacterStabilityRecoveryModifier;
			Math.Clamp(_stability, 0, 1);
		}

		// Update Aim Laser Mesh
		_gunRay.ForceRaycastUpdate();
		if (_gunRay.IsColliding())
		{
			Vector3 collision = _gunRay.ToLocal(_gunRay.GetCollisionPoint());
			float laserLength = Math.Clamp(collision.Z, -3, 3);
			_aimLaser.Mesh.Set("height", laserLength);
			_aimLaser.Position = new Vector3(_aimLaser.Position.X, _aimLaser.Position.Y, laserLength / 2);
		}

		if (!_canFire)
		{
			_aimLaser.Visible = false;
		}
		else
		{
			_aimLaser.Visible = true;
		}

		// Check if Weapon is currently crowded
		if (!_crowdingArea.HasOverlappingBodies()) { _canFire = true; }

		// Check if Magazine is empty
		if (_currentMagazineCount <= 0)
		{
			_canFire = false;
		}
	}

	private void OnCrowdingAreaBodyEntered(Node3D _)
	{
		_canFire = false;
	}
}
