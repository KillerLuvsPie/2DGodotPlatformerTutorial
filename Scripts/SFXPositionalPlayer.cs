using Godot;
using System;

public partial class SFXPositionalPlayer : AudioStreamPlayer2D
{
	public void OnFinished()
	{
		if(Stream == Global.sfx_fall)
			GameManager.Instance.GameOver();
		QueueFree();
	}
}
