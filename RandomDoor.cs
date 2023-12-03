using Godot;
using System;

public partial class RandomDoor : StaticBody3D
{
	public override void _Ready()
	{
		var collision = GetNode<CollisionShape3D>("%Collision");

		var rand = new Random();
		var randChoice = rand.NextSingle();

		if (randChoice < 0.5)
		{
			Visible = false;
			collision.Disabled = true;
		}
	}
}
