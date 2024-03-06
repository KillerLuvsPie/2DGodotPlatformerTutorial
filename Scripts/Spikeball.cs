using Godot;
using System;

public partial class Spikeball : Node2D
{
	[Export]
	private int rotationDirection = 1;
	[Export]
	private float speed = 5;
	[Export]
	private Sprite2D ballSprite;

	private void OnBallBodyEntered(Node2D body)
	{
		if(body.IsInGroup("Player"))
		{
			Player.Instance.Die();
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        Rotate(speed * rotationDirection * (float)delta);
		foreach(Sprite2D chain in GetChild<Node2D>(0).GetChildren())
			chain.Rotation = -Rotation;
		ballSprite.Rotation = -Rotation;
    }
}
