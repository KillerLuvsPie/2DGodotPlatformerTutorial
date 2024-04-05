using Godot;
using System;

public partial class explosion : AnimatedSprite2D
{
	public void OnAnimationFinished()
	{
		QueueFree();
	}
}
