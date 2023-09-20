using Godot;
using System;

public partial class DeathAnimationSprite : Marker3D
{
    private AnimatedSprite3D _deathAnimationSprite;
    private const int FRAME_COUNT = 18;
    private const int FPS = 18;

    public override void _Ready()
    {
        _deathAnimationSprite = GetNode<AnimatedSprite3D>("DeathAnimationSprite");
        var duration = (float)FRAME_COUNT / FPS;
        _deathAnimationSprite.Play();

        GetTree().CreateTimer(duration).Timeout += QueueFree;
    }
}
