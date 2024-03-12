using Godot;
using System;

public partial class PlatformCarrousel : Sprite2D
{
	#region VARIABLES
	[Export] private float speed = 1;
	[Export] private int direction = 1;
	[Export] private Node2D platformsParent;
	#endregion <--->

	#region FUNCTIONS
	#endregion <--->

	#region SIGNALS
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _PhysicsProcess(double delta)
	{
		Rotate(speed * direction * (float)delta);
		foreach(AnimatableBody2D platform in platformsParent.GetChildren())
		{
			platform.Rotation = -Rotation;
		}
	}
	#endregion <--->
}
