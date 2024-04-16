using Godot;
using System;

public partial class EnemyTree : AnimatableBody2D
{
	#region VARIABLES
	//CUSTOM SIGNAL
	[Signal] public delegate void GameStartedEventHandler();

	//EXPORTS
	[Export] private Marker2D leftBound;
	[Export] private Marker2D rightBound;
	[Export] private Marker2D shotSpawnPoint;
	[Export] private AnimatedSprite2D animatedSprite2D;
	[Export] bool moveable = false;

	//PROPERTIES
	[Export] private float speed = 75;
	[Export] private int direction = -1;
	[Export] private float shotCooldown = 1.5f;
	[Export] private float startupWait = 0;
	private bool test = false;
	#endregion <--->

	#region FUNCTIONS
	private async void Shoot()
	{
		if(startupWait > 0)
			await ToSignal(GetTree().CreateTimer(startupWait, false), Timer.SignalName.Timeout);
		bool shoot = true;
		while(shoot)
		{
			await ToSignal(GetTree().CreateTimer(shotCooldown, false), Timer.SignalName.Timeout);
			animatedSprite2D.Play("shoot");
		}
	}
	#endregion <--->

	#region SIGNALS
	public void AnimationFinished()
	{
		if(animatedSprite2D.Animation == "shoot")
			animatedSprite2D.Play("idle");
	}

	public void OnFrameChange()
	{
		switch(animatedSprite2D.Animation)
		{
			case "shoot":
				switch(animatedSprite2D.Frame)
				{
					case 7:
						Area2D bullet = Global.enemyBullet.Instantiate<Area2D>();
						bullet.Position = shotSpawnPoint.GlobalPosition;
						bullet.GetNode<EnemyBullet>(".").SetProperties(direction);
						GetTree().CurrentScene.AddChild(bullet);
						Global.Instance.PlayPositionalSound(Global.sfx_treeShoot, shotSpawnPoint.GlobalPosition, 75);
					break;
				}
			break;
		}
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		animatedSprite2D.FlipH = direction == 1;
		shotSpawnPoint.Position = new Vector2(shotSpawnPoint.Position.X * direction, shotSpawnPoint.Position.Y);
		Shoot();
	}

	public override void _PhysicsProcess(double delta)
	{
		
	}
	#endregion <--->
}
