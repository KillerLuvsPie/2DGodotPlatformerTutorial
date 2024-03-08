using Godot;
using System;

public partial class SFXPlayer : AudioStreamPlayer
{
	public void OnFinished()
	{
		if(Stream == Global.sfx_fall)
			GameManager.Instance.GameOver();
		QueueFree();
	}
}
