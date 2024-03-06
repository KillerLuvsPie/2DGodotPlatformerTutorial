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
	#endregion <--->

	#region FUNCTIONS
	private async void Shoot()
	{
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
						Global.Instance.PlaySound(Global.sfx_trunkShoot, 25);
					break;
				}
			break;
		}
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		Shoot();
	}

	public override void _PhysicsProcess(double delta)
	{
		animatedSprite2D.FlipH = direction == 1;
		shotSpawnPoint.Position = Global.GetAbsVector2(shotSpawnPoint.Position) * direction;
	}
	#endregion <--->
}
