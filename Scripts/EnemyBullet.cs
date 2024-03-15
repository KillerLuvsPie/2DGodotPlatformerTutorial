using Godot;
using System;

public partial class EnemyBullet : Area2D
{
	#region VARIABLES
	[Export] private AnimatedSprite2D animatedSprite2D;
	private float speed = 300;
	private int direction = -1;
	private float lifetime = 0;
	private bool active = true;
	#endregion <--->

	#region FUNCTIONS
	public void SetProperties(int d, float s = 300)
	{
		direction = d;
		speed = s;
	}

	private async void OnTimerDestroy()
	{
		await ToSignal(GetTree().CreateTimer(lifetime, false), Timer.SignalName.Timeout);
		active = false;
		animatedSprite2D.Animation = "die";
	}
	#endregion <--->

	#region SIGNALS
	public void OnBodyEntered(Node2D body)
	{
		if(body.Name == Global.playerGroup && active)
		{
			active = false;
			animatedSprite2D.Animation = "die";
			Player.Instance.Die();
		}
		
		if(!body.IsInGroup(Global.enemyGroup) && !body.IsInGroup(Global.platformGroup) && active)
		{
			active = false;
			animatedSprite2D.Animation = "die";
		}	
	}

	public void AnimationFinished()
	{
		QueueFree();
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		if(lifetime > 0)
			OnTimerDestroy();
	}

	public override void _PhysicsProcess(double delta)
	{
		animatedSprite2D.FlipH = direction == 1;
		if(active)
			Translate(new Vector2(speed * direction * (float)delta, 0));
	}
	#endregion <--->
}
