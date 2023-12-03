using Godot;
using Godot.Collections;

public partial class EnemyCharacterController : CharacterBody3D
{
	[Export]
	public CharacterInformation CharacterInformation { set; get; }
	[Export]
	public bool IsComputerControlled { set; get; } = false;
	[Export]
	public CharacterWeapon Weapon { set; get; } = null;
	[Export]
	public EnemyStateManager StateManager { set; get; }

	[ExportGroup("Navigation Manager")]
	[Export]
	public NavigationPointManager NavManager { set; get; } = null;

	[Export]
	public int InitialRoute { set; get; } = 0;
	[Export]
	public int InitialPoint { set; get; } = 0;

	public NavigationAgent3D NavAgent;
	private VisionAwareness _visionAwareness;

	public Vector3 _newInitNavPointGlobalPosition;
	public Vector3 _currentNavPointGlobalPosition;

	public override void _Ready()
	{
		NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_visionAwareness = GetNode<VisionAwareness>("VisionAwareness");

		StateManager.Character = this;
		StateManager.NavAgent = NavAgent;
		StateManager.NavManager = NavManager;
		NavAgent.VelocityComputed += OnNavAgentVelocityComputed;

		StateManager.VisionAwareness = _visionAwareness;

		_visionAwareness.HostileContactDetected += StateManager.HostileDetected;
		_visionAwareness.HostileContactLost += StateManager.HostileLost;
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
			MoveToNavAgentTarget();
			FaceMovementDirection();
		}

		MoveAndSlide();
	}

	public Vector3 GetClosestPointOnMap(Vector3 Position)
	{
		var map = GetWorld3D().NavigationMap;
		return NavigationServer3D.MapGetClosestPoint(map, Position);
	}

	private void OnNavigationPointManagerReachedNavigationPoint(int _, int __)
	{
		NavManager.GetNextPointOnRoute();
	}

	private void MoveToNavAgentTarget()
	{
		if (!NavAgent.IsNavigationFinished())
		{
			Vector3 nextPathPosition = NavAgent.GetNextPathPosition();
			var currentPosition = GlobalPosition;
			var newVelocity = (nextPathPosition - currentPosition).Normalized() * 5;
			//Velocity = newVelocity;
			NavAgent.Velocity = newVelocity;
		}
		else
		{
			Velocity = Vector3.Zero;
			//NavManager.GetNextPointOnRoute();
		}
	}
	private void OnNavAgentVelocityComputed(Vector3 safeVelocity)
	{
		Velocity = safeVelocity;
		MoveAndSlide();
	}

	private void FaceMovementDirection()
	{
		if (Velocity != Vector3.Zero)
		{
			Vector3 lookAtDirection = Velocity;
			lookAtDirection.Y = 0;

			LookAt(Transform.Origin + lookAtDirection, Vector3.Up);
		}
	}

	public void TakeDamage(double damage, Vector3 from)
	{
		CharacterInformation.Health -= damage;

		ReactToDamage(from);
	}

	private async void ReactToDamage(Vector3 from)
	{
		Dictionary dict = new()
		{
			{ "LastKnownLocation", from }
		};

		await ToSignal(GetTree().CreateTimer(0.35), SceneTreeTimer.SignalName.Timeout);

		StateManager.TransitionTo("PursuingEnemy", dict);
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

	/*
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
				Weapon?.PrimaryAction();
			}
		}
	*/
}
