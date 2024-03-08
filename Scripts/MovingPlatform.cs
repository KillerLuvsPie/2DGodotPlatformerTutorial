using Godot;
using System;

public partial class MovingPlatform : Node2D
{
	#region VARIABLES
	[Export] private PathFollow2D pathPos;
	[Export] private float speed = 0.25f;
	#endregion <--->

	#region FUNCTIONS
	private void ChangeDirection()
	{
		if(pathPos.ProgressRatio == 1 || pathPos.ProgressRatio == 0)
		{
			speed = -speed;
		}
	}
	#endregion <--->

	#region SIGNALS
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _PhysicsProcess(double delta)
	{
		pathPos.ProgressRatio += speed * (float)delta;
		ChangeDirection();
		GD.Print("Speed: " + speed + " | Progress: " + pathPos.ProgressRatio);
	}
	#endregion <--->
}
