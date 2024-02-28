using Godot;
using System;

public partial class SFXPlayer : AudioStreamPlayer
{
	public void OnFinished()
	{
		QueueFree();
	}
}
