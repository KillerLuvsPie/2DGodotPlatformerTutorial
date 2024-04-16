using Godot;
using System;

public partial class Switch : StaticBody2D
{
	#region VARIABLES
	[Export] public AnimatedSprite2D animatedSprite2D;
	[Export] public AnimatableBody2D westGate;
	[Export] public AnimatableBody2D eastGate;
	private bool stopAction = false;
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
			Global.Instance.PlaySound(Global.sfx_switch, 80);
		}
	}
	private void ActivateSwitches()
	{
		ToggleSwitchState();
		RotateGates(true);
	}

	private void SetGatesBossLayerState(bool state)
	{
		westGate.SetCollisionLayerValue(2, state);
		westGate.SetCollisionMaskValue(2, state);
		eastGate.SetCollisionLayerValue(2, state);
		eastGate.SetCollisionMaskValue(2, state);
	}
	#endregion <--->

	#region COROUTINES
	public async void RotateGates(bool open)
	{
		float gateSpeed = 1;
		stopAction = false;
		if(open)
		{
			//WAIT FOR GATE TO FINISH OPENING
			while(westGate.RotationDegrees > 0 && stopAction == false)
			{
				westGate.Rotate(-gateSpeed * (float)GetPhysicsProcessDeltaTime());
				eastGate.Rotate(gateSpeed * (float)GetPhysicsProcessDeltaTime());
				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			}
			//ACTIVATE RHINO
			if(stopAction == false)
			{
				for(int i = 0; i < GetTree().GetNodesInGroup(Global.enemyGroup).Count; i++)
				{
					Node2D node = (Node2D)GetTree().GetNodesInGroup(Global.enemyGroup)[i];
					if(node.Owner.Name.ToString().Contains("rhino"))
						node.GetNode<EnemyRhino>(".").active = true;
				}
			}
			
		}
		else
		{
			SetGatesBossLayerState(false);
			while(westGate.RotationDegrees < 90 && stopAction == false)
			{
				westGate.Rotate(gateSpeed * (float)GetPhysicsProcessDeltaTime());
				eastGate.Rotate(-gateSpeed * (float)GetPhysicsProcessDeltaTime());
				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			}
			SetGatesBossLayerState(true);
			if(stopAction == false)
				ToggleSwitchState();
		}
	}

	private void AssignToSignal()
	{
		GameManager.Instance.Respawn += ResetGates;
	}
	#endregion

	#region SIGNALS
	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup(Global.bossGroup) && animatedSprite2D.Animation == "off")
			ActivateSwitches();
	}

	private void ResetGates()
	{
		stopAction = true;
		eastGate.GlobalRotationDegrees = 90;
		westGate.GlobalRotationDegrees = 90;
		animatedSprite2D.Animation = "off";
	}
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        CallDeferred("AssignToSignal");
    }

    public override void _Process(double delta)
	{
	
	}
	#endregion <--->
}
