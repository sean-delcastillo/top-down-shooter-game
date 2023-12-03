using Godot;
using System;

public partial class CrosshairController : Control
{
	public PlayerCharacter Player { set; get; }

	readonly private Vector2 positionOffset = new(-10, -100);
	private TextureRect _spread;
	private TextureRect _center;

	public override void _Ready()
	{
		_spread = GetNode<TextureRect>("Spread");
		_center = GetNode<TextureRect>("Center");
	}

	public override void _Process(double delta)
	{
		Position = GetViewport().GetMousePosition() + positionOffset;
	}

	public void UpdateSpreadScale(float weaponStability)
	{
		float scaleDueToInstability = (float)(1.5 - weaponStability);
		if (scaleDueToInstability >= 100) { scaleDueToInstability = 0; }
		else { scaleDueToInstability = Math.Clamp(scaleDueToInstability, 0, 2); }

		_spread.Scale = new Vector2(scaleDueToInstability, scaleDueToInstability);
	}
}
