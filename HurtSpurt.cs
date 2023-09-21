using Godot;
using System;

public partial class HurtSpurt : Node3D
{
	private GpuParticles3D _particle;

	public override void _Ready()
	{
		_particle = GetNode<GpuParticles3D>("HurtSpurtParticles");
		GetTree().CreateTimer(_particle.Lifetime).Timeout += QueueFree;
	}
}
