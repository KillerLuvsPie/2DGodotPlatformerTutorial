using Godot;
using System;

public partial class Switch : StaticBody2D
{
	#region VARIABLES
	[Export] public AnimatedSprite2D animatedSprite2D;
	[Export] public Node2D westGate;
	[Export] public Node2D eastGate;
	#endregion <--->

	#region FUNCTIONS
	private void ToggleSwitchState()
	{
		for(int i = 0; i < GetTree().GetNodesInGroup(Global.switchGroup).Count; i++)
		{
			Switch node = GetTree().GetNodesInGroup(Global.switchGroup)[i].GetNode<Switch>(".");
			if(node.animatedSprite2D.Animation == "off")
				node.animatedSprite2D.Animation = "on";
			else
				node.animatedSprite2D.Animation = "off";
		}
	}
	private void ActivateSwitches()
	{
		ToggleSwitchState();
		RotateGates(true);
	}
	#endregion <--->

	#region COROUTINES
	public async void RotateGates(bool open)
	{
		float gateSpeed = 1;
		if(open)
		{
			while(westGate.RotationDegrees > 0)
			{
				westGate.Rotate(-gateSpeed * (float)GetPhysicsProcessDeltaTime());
				eastGate.Rotate(gateSpeed * (float)GetPhysicsProcessDeltaTime());
				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			}
			for(int i = 0; i < GetTree().GetNodesInGroup(Global.enemyGroup).Count; i++)
			{
				Node2D node = (Node2D)GetTree().GetNodesInGroup(Global.enemyGroup)[i];
				if(node.Owner.Name.ToString().Contains("rhino"))
					node.GetNode<EnemyRhino>(".").active = true;
			}
		}
		else
		{
			while(westGate.RotationDegrees < 90)
			{
				westGate.Rotate(gateSpeed * (float)GetPhysicsProcessDeltaTime());
				eastGate.Rotate(-gateSpeed * (float)GetPhysicsProcessDeltaTime());
				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			}
			ToggleSwitchState();
		}
	}
	#endregion

	#region SIGNALS
	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup(Global.bossGroup) && animatedSprite2D.Animation == "off")
			ActivateSwitches();
	}
	#endregion <--->

	#region GODOT FUNCTIONS

	public override void _Process(double delta)
	{
		GD.Print(westGate.Rotation);
	}
	#endregion <--->
}
