using Godot;
using System;

public partial class PauseInputHandler : Node
{
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause") && GameManager.Instance.pausable)
			GameManager.Instance.TogglePause();
	}
}
