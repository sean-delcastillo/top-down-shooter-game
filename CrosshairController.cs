using Godot;
using System;

public partial class CrosshairController : Control
{
	readonly private Vector2 positionOffset = new(-13, -23);
	private Vector2 _position;
	private TextureRect _spread;
	private TextureRect _center;

	public override void _Ready()
	{
		_spread = GetNode<TextureRect>("Spread");
		_center = GetNode<TextureRect>("Center");
	}

	public void UpdateSpreadScale(float weaponStability)
	{
		float scaleDueToInstability = (float)(1.5 - weaponStability);
		if (scaleDueToInstability >= 100) { scaleDueToInstability = 0; }
		else { scaleDueToInstability = Math.Clamp(scaleDueToInstability, 0, 2); }

		_spread.Scale = new Vector2(scaleDueToInstability, scaleDueToInstability);
	}

	public void UpdateCrosshairPosition(Vector2 mousePosition)
	{
		Position = mousePosition + positionOffset;
	}
}
