using Godot;
using System;

public partial class Player : CharacterBody2D
{
	#region VARIABLES
	//SINGLETON
	public static Player Instance;

	//PROPERTIES
	[Export] private const float Speed = 400.0f;
	[Export] private const float JumpVelocity = -1000.0f;

	private static float acceleration = 30f;
	private static bool justLanded = true;
	public AnimatedSprite2D animatedSprite2D;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	#endregion <--->

	#region FUNCTIONS
	public void Die(bool fallDeath = false)
	{
		if(GameManager.Instance.playerControl)
		{
			GameManager.Instance.pausable = false;
			GameManager.Instance.playerControl = false;
			if(fallDeath)
			{
				Global.Instance.PlaySound(Global.sfx_fall);
			}
			else
			{
				animatedSprite2D.Animation = "die";
				Global.Instance.PlaySound(Global.sfx_deaths[0]);
			}
		}
	}

	public void Respawn()
	{
		Position = GameManager.Instance.currentCheckpoint;
		GameManager.Instance.pausable = true;
		GameManager.Instance.playerControl = true;
		animatedSprite2D.Play();
	}
	#endregion <--->

	#region SIGNALS
	public void OnSpriteFrameChanged()
	{
		switch (animatedSprite2D.Animation)
		{
			case "die":
				switch (animatedSprite2D.Frame)
				{
					case 7:
						Global.Instance.PlaySound(Global.sfx_deaths[1]);
					break;
				}
			break;
		}
	}

	public void OnSpriteAnimationFinished()
	{
		if(animatedSprite2D.Animation == "die")
			GameManager.Instance.GameOver();
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _PhysicsProcess(double delta)
	{
		//PLAYER CONTROLS AND ANIMATIONS
		if(GameManager.Instance.playerControl)
		{
			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				if(Input.IsActionJustReleased("jump") && velocity.Y < -100)
					velocity.Y = -100;
	
				velocity.Y += gravity * (float)delta;
				justLanded = false;
			}
				
			if(IsOnFloor() && justLanded == false)
			{
				justLanded = true;
				Global.Instance.PlaySound(Global.sfx_landing, 300f);
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("jump") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
				Global.Instance.PlaySound(Global.sfx_jump);
			}
				
			// Get the input direction and handle the movement/deceleration.
			float direction = Input.GetAxis("left", "right");
			if (direction != 0)
			{
				velocity.X = Mathf.MoveToward(Velocity.X, direction * Speed, acceleration);
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, acceleration);
			}

			if(Input.IsActionJustPressed("fall") && IsOnFloor())
			{
				Position += new Vector2(0,1);
			}

			Velocity = velocity;
			MoveAndSlide();

			//FALL DEATH
			if(Position.Y > GameManager.Instance.cameraBottomBound.Position.Y + 20)
				Die(true);

			//ANIMATION CONTROL
			//FLIP SPRITE
			if(velocity.X < 0)
				animatedSprite2D.FlipH = true;
			else if(velocity.X > 0)
				animatedSprite2D.FlipH = false;

			//RUN/IDLE
			if(Mathf.Abs(velocity.X) > 1)
				animatedSprite2D.Animation = "run";
			else
				animatedSprite2D.Animation = "idle";

			if(velocity.Y < 0)
				animatedSprite2D.Animation = "jump";
			else if(velocity.Y > 0)
				animatedSprite2D.Animation = "fall";
		}
	}

	public override void _Ready()
	{
		if(Instance != this && Instance != null && IsInstanceValid(Instance))
			QueueFree();
		else
			Instance = this;
		animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite");
	}
	#endregion <--->
}