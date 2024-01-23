using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
	public const float Speed = 400.0f;
	public const float JumpVelocity = -1000.0f;
	private AnimatedSprite2D animSprite;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		float direction = Input.GetAxis("left", "right");
		if (direction != 0)
		{
			velocity.X = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, 40);
		}

		if(Input.IsActionJustPressed("fall") && IsOnFloor())
		{
			Position += new Vector2(0,1);
		}

		Velocity = velocity;
		MoveAndSlide();

		//ANIMATION CONTROL
		//FLIP SPRITE
		if(velocity.X < 0)
			animSprite.FlipH = true;
		else if(velocity.X > 0)
			animSprite.FlipH = false;

		//RUN/IDLE
		if(Mathf.Abs(velocity.X) > 1)
			animSprite.Animation = "run";
		else
			animSprite.Animation = "idle";

		if(velocity.Y < 0)
			animSprite.Animation = "jump";
		else if(velocity.Y > 0)
			animSprite.Animation = "fall";
	}

	public override void _Ready()
	{
		animSprite = GetNode<AnimatedSprite2D>("Sprite");
	}
}
