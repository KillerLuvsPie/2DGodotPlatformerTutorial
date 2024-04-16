using Godot;
using System;

public partial class EnemyRockhead : Area2D
{
	#region VARIABLES
	//EXPORTS
	[Export] private AnimatedSprite2D animatedSprite2D;

	//PROPERTIES
	[Export] private bool horizontal = false;
	[Export] private float speed = 700;
	[Export] private float accelerationMultiplier = 1;
	private Vector2 direction = Vector2.Zero;
	private float acceleration = 0;
	private bool active = false;
	
	#endregion <--->

	#region FUNCTIONS
	#endregion <--->

	#region COROUTINES
	private async void Blink()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		float minRange = 2;
		float maxRange = 5;
		while(true)
		{
			float waitTime = rng.RandfRange(minRange, maxRange);
			if(animatedSprite2D.Animation == "idle")
			{
				animatedSprite2D.Animation = "blink";
			}
			await ToSignal(GetTree().CreateTimer(waitTime, false), Timer.SignalName.Timeout);
		}
	}

	private async void Move()
	{
		float offset = 10;
		if(active == true)
		{
			//SET DIRECTION TO MOVE
			if(horizontal)
			{
				if(GlobalPosition.X > Player.Instance.GlobalPosition.X + offset)
					direction.X = -1;
				else if(GlobalPosition.X < Player.Instance.GlobalPosition.X - offset)
					direction.X = 1;
			}
			else
			{
				if(GlobalPosition.Y > Player.Instance.GlobalPosition.Y + offset)
					direction.Y = -1;
				else if(GlobalPosition.Y < Player.Instance.GlobalPosition.Y - offset)
					direction.Y = 1;
			}
			//MOVE UNTIL ACTIVE IS FALSE
			if(direction != Vector2.Zero)
			{
				while(active)
				{
					if(GetTree().Paused == false)
					{
						acceleration = Mathf.Clamp(acceleration + (accelerationMultiplier * (float)GetPhysicsProcessDeltaTime()), 0, 1);
						Translate(direction * acceleration * speed * (float)GetPhysicsProcessDeltaTime());
					}
					await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
				}
			}
			else
				active = false;
		}
	}
	#endregion <--->

	#region SIGNALS
	public void OnAnimationFinished()
	{
		switch(animatedSprite2D.Animation)
		{
			case "blink":
			case "rhit":
			case "lhit":
			case "bhit":
			case "thit":
				animatedSprite2D.Animation = "idle";
				animatedSprite2D.Play();
			break;
		}
	}

	public void OnTriggerAreaEnter(Node2D body)
	{
		if(body.IsInGroup(Global.playerGroup) && active == false && (animatedSprite2D.Animation == "blink" || animatedSprite2D.Animation == "idle"))
		{
			active = true;
			Move();
		}
	}

	public void OnObjectAreaEnter(Node2D body)
	{
		if(body.IsInGroup(Global.playerGroup))
		{
			Player.Instance.Die();
		}
		else if(body.Name == "TileMap")
		{
			active = false;
			if(horizontal)
			{
				if(direction.X > 0)
					animatedSprite2D.Animation = "rhit";
				else if(direction.X < 0)
					animatedSprite2D.Animation = "lhit";
			}
			else
			{
				if(direction.Y > 0)
					animatedSprite2D.Animation = "bhit";
				else if(direction.Y < 0)
					animatedSprite2D.Animation = "thit";
			}
			acceleration = 0;
			direction = Vector2.Zero;
			Global.Instance.PlayPositionalSound(Global.sfx_thud, GlobalPosition);
		}
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		Blink();
	}
    #endregion <--->
}
