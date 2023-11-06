using Godot;
using System;

public partial class CollectableObjective : StaticBody3D
{
	[Signal]
	public delegate void McguffinCollectedEventHandler();

	private Area3D _collectionArea;
	private CollisionShape3D _collision;

	public override void _Ready()
	{
		_collectionArea = GetNode<Area3D>("CollectionArea");
		_collision = GetNode<CollisionShape3D>("CollisionShape3D");
		_collectionArea.BodyEntered += OnCollectionAreaBodyEntered;
	}

	public void Activate()
	{
		Visible = true;
		//_collision.Disabled = false;
		_collectionArea.Monitoring = true;
	}

	public void Deactivate()
	{
		Visible = false;
		//_collision.Disabled = true;
		_collectionArea.Monitoring = false;
	}

	private void OnCollectionAreaBodyEntered(Node3D body)
	{
		if (body is PlayerCharacterController)
		{
			Deactivate();
			EmitSignal(SignalName.McguffinCollected);
			QueueFree();
		}
	}
}
