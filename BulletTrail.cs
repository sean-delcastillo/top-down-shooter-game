using Godot;
using System;

public partial class BulletTrail : MeshInstance3D
{
	private double TimeToLive = 0.1;
	public override void _Ready()
	{
		GetTree().CreateTimer(TimeToLive).Timeout += QueueFree;
	}

	public override void _Process(double delta)
	{
	}

	public void Init(Vector3 from, Vector3 to)
	{
		var drawMesh = new ImmediateMesh();
		Mesh = drawMesh;
		drawMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, MaterialOverride);
		drawMesh.SurfaceAddVertex(from);
		drawMesh.SurfaceAddVertex(to);
		drawMesh.SurfaceEnd();
	}
}
