using Godot;
using System;

public partial class CharacterInformation : Resource
{
    [Export]
    public double Health { set; get; }
    [Export]
    public int MovementSpeed { set; get; }
    [Export]
    public PackedScene DeathAnimation { set; get; }
    [Export]
    public PackedScene HurtAnimation { set; get; }

    public CharacterInformation() : this(0, 0, null, null) { }

    public CharacterInformation(double health, int movementSpeed, PackedScene deathAnimation, PackedScene hurtAnimation)
    {
        Health = health;
        MovementSpeed = movementSpeed;
        DeathAnimation = deathAnimation;
        HurtAnimation = hurtAnimation;
    }
}
