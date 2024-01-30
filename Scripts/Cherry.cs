using Godot;
using System;

public partial class Cherry : Area2D
{
	#region VARIABLES
	private bool active = true;
	private AnimatedSprite2D animatedSprite2D;
	#endregion VARIABLES

	#region SIGNALS
	public void OnAnimatedSprite2dAnimationFinished()
	{
		QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup("Player") && active == true)
		{
			active = false;
			GameManager.Instance.DecreaseFruitCounter();
			animatedSprite2D.Animation = "collected";
		}	
	}
	#endregion SIGNALS

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("Animation");
	}

	public override void _Process(double delta)
	{
	}
	#endregion GODOT FUNCTIONS
}