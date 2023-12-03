using Godot;
using System;
/// <summary>
/// For weapons that a character can have and activate.
/// </summary>
public partial class CharacterWeapon : Node3D
{
	[Signal]
	public delegate void AmmoChangeEventHandler(int CurrentAmmo, int OldAmmo, int MaxAmmo);
	[Signal]
	public delegate void WeaponStabilityEventHandler(double Stability);

	[Export]
	public float Damage { set; get; }
	[Export]
	public int FireRate { set; get; }
	[Export]
	public int MagazineSize { set; get; }
	[Export]
	public PackedScene RangeTrail { set; get; }

	private MeshInstance3D _aimLaser;
	private RayCast3D _gunRay;
	private Area3D _collisionArea;
	private Timer _fireFrequency;
	private Timer _recoilRecovery;
	private double _stability = 1;
	private FastNoiseLite _noise = new()
	{
		NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex,
		Frequency = 0.5f
	};
	private RandomNumberGenerator _rand = new();
	private int _bulletsInMagazine;
	private bool _canFire = true;

	public override void _Ready()
	{
		_collisionArea = GetNode<Area3D>("%Crowding");
		_aimLaser = GetNode<MeshInstance3D>("%AimLaser");
		_gunRay = GetNode<RayCast3D>("%GunRay");

		_fireFrequency = new()
		{
			OneShot = true,
			WaitTime = CalculateSecondsPerRound(FireRate)
		};
		AddChild(_fireFrequency);

		_recoilRecovery = new()
		{
			OneShot = true,
			WaitTime = 0.05
		};
		AddChild(_recoilRecovery);

		_bulletsInMagazine = MagazineSize;

		_rand.Randomize();
	}

	public override void _Process(double delta)
	{
		CheckForCrowding();
		CheckMagazine();

		if (_stability <= 0.9 && _recoilRecovery.IsStopped())
		{
			_stability += 0.1;
			Math.Clamp(_stability, 0, 1);
		}

		UpdateAimLaser();

		EmitSignal(SignalName.WeaponStability, _stability);
	}

	/// <summary>
	/// Emits initial weapon information signal for UI.
	/// </summary>
	public void RegisterWithUi()
	{
		EmitSignal(SignalName.AmmoChange, MagazineSize, MagazineSize, MagazineSize);
	}

	/// <summary>
	/// Checks if any condition stops the weapon from firing, else shoots a raycast and checks for collision. 
	/// If colliding draws a trail to collision point and calls collider's damage methods, else draws a 
	/// trail straight forward.
	/// </summary>
	public void PrimaryAction()
	{
		if (!_fireFrequency.IsStopped() || !_canFire) { return; }
		else
		{
			var forward = _gunRay.TargetPosition;
			var recoilRay = CalculateDeflectedRay(_gunRay.TargetPosition);
			_gunRay.TargetPosition = recoilRay;
			_gunRay.ForceRaycastUpdate();

			if (_gunRay.IsColliding())
			{
				DrawBulletTrail(_gunRay.GetCollisionPoint());
				var target = _gunRay.GetCollider() as Node3D;
				target.Call("TakeDamage", Damage, GlobalPosition);
				target.Call("DamageAtLocation", _gunRay.GetCollisionPoint(), _gunRay.GetCollisionNormal());
			}
			else
			{
				DrawBulletTrail(GlobalTransform * _gunRay.TargetPosition);
			}
			_gunRay.TargetPosition = forward;
			if (_stability >= 0.2)
			{
				_stability -= 0.2;
			}
			EmitSignal(SignalName.AmmoChange, _bulletsInMagazine - 1, _bulletsInMagazine, MagazineSize);
			_bulletsInMagazine -= 1;
			_fireFrequency.Start();
			_recoilRecovery.Start();
		}
	}

	/// <summary>
	/// Resets number of bullets in magazine to maximum number of bullets and emits UI signal.
	/// </summary>
	public void Reload()
	{
		_bulletsInMagazine = MagazineSize;
		_canFire = true;
		EmitSignal(SignalName.AmmoChange, MagazineSize, _bulletsInMagazine, MagazineSize);
	}

	/// <summary>
	/// Samples stability and calculates random deflection based on it.
	/// </summary>
	/// <param name="straight">The direction straight ahead from the weapon</param>
	/// <returns>A vector3 indicating the deflected ray</returns>
	public Vector3 CalculateDeflectedRay(Vector3 straight)
	{
		var maxVerticalDeflectionAngle = 20;
		var maxHorizontalDeflectionAngle = 40;

		var stability = _stability;
		var instability = (float)(1 - stability);

		var verticalDeflectionAngle = (float)Math.Pow(instability, 2) * maxVerticalDeflectionAngle;
		var horizontalDeflectionAngle = _noise.GetNoise1D(instability) * _rand.Randfn(deviation: instability * maxHorizontalDeflectionAngle);

		Vector3 verticalDeflection = straight.Rotated(Vector3.Right, verticalDeflectionAngle * (float)(Math.PI / 180));
		Vector3 finalDeflection = verticalDeflection.Rotated(Vector3.Up, horizontalDeflectionAngle * (float)(Math.PI / 180));

		return finalDeflection;
	}


	/// <summary>
	/// Updates the aim laser mesh based on the aim ray's collisions.
	/// </summary>
	private void UpdateAimLaser()
	{
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
	}

	/// <summary>
	/// Checks if the weapon's crowding area is overlapping with something. If so sets condition for being 
	/// able to fire to false, else true.
	/// </summary>
	private void CheckForCrowding()
	{
		if (_collisionArea.HasOverlappingBodies()) { _canFire = false; }
		else { _canFire = true; }
	}

	/// <summary>
	/// Makes the weapon unfirable if there are no bullets in the magazine.
	/// </summary>
	private void CheckMagazine()
	{
		if (_bulletsInMagazine <= 0)
		{
			_canFire = false;
		}
	}

	/// <summary>
	/// Calculates the duration of time in seconds between shots.
	/// </summary>
	/// <param name="fireRate">The rate of fire of this weapon in rounds per second.</param>
	/// <returns>The duration of time in seconds between shots.</returns>
	private double CalculateSecondsPerRound(int fireRate)
	{
		return Math.ReciprocalEstimate(fireRate / 60f);
	}

	/// <summary>
	/// Draws a bullet trail mesh from the muzzle of the weapon to a position.
	/// </summary>
	/// <param name="to">The end position to draw the bullet trail.</param>
	private void DrawBulletTrail(Vector3 to)
	{
		var bulletTrail = RangeTrail.Instantiate<BulletTrail>();
		bulletTrail.Init(_gunRay.GlobalPosition, to);
		GetTree().Root.AddChild(bulletTrail);
	}
}
