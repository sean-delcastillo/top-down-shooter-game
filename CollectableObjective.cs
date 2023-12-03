using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[Tool]
public partial class CollectableObjective : StaticBody3D
{
	[Signal]
	public delegate void CollectibleCollectedEventHandler(string type);

	[Export]
	public bool IsGuaranteedObjective;
	[Export]
	public ObjectiveType Type
	{
		set
		{
			_type = value;
			SetMesh();
		}
		get { return _type; }
	}

	private ObjectiveType _type;
	private Area3D _collectionArea;
	private CollisionShape3D _collision;
	public enum ObjectiveType
	{
		GoldBall,
		RedBall,
		GreenBall
	}

	public override void _Ready()
	{
		_collectionArea = GetNode<Area3D>("CollectionArea");
		_collision = GetNode<CollisionShape3D>("CollisionShape3D");
		_collectionArea.BodyEntered += OnCollectionAreaBodyEntered;

		_type = Type;
		SetMesh();
	}

	public void Activate()
	{
		// Visible = true;
		// _collectionArea.Monitoring = true;
		SetDeferred(Node3D.PropertyName.Visible, true);
		_collectionArea.SetDeferred(Area3D.PropertyName.Monitoring, true);
	}

	public void Deactivate()
	{
		//	Visible = false;
		// _collectionArea.Monitoring = false;
		SetDeferred(Node3D.PropertyName.Visible, false);
		_collectionArea.SetDeferred(Area3D.PropertyName.Monitoring, false);
	}

	private void OnCollectionAreaBodyEntered(Node3D body)
	{
		if (body is PlayerCharacter player)
		{
			Deactivate();
			player.CollectedObjective(_type);
			EmitSignal(SignalName.CollectibleCollected, Enum.GetName(_type));
		}
	}

	private void SetMesh()
	{
		MeshInstance3D mesh = GetNode<MeshInstance3D>("%Mesh");

		mesh.Mesh.ResourceLocalToScene = true;

		StandardMaterial3D typeMesh = _type switch
		{
			ObjectiveType.GoldBall => GD.Load<StandardMaterial3D>("res://gold_objective.tres"),
			ObjectiveType.RedBall => GD.Load<StandardMaterial3D>("res://red_objective.tres"),
			ObjectiveType.GreenBall => GD.Load<StandardMaterial3D>("res://green_objective.tres"),
			_ => GD.Load<StandardMaterial3D>("res://gold_objective.tres"),
		};
		//mesh.SetSurfaceOverrideMaterial(0, typeMesh);
		mesh.Mesh.SurfaceSetMaterial(0, typeMesh);
		//mesh.Mesh.SurfaceSetMaterial(0, GD.Load<StandardMaterial3D>("res://red_objective.tres"));
	}
}
