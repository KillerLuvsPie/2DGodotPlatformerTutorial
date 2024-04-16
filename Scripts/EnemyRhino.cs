using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyRhino : Area2D
{
	#region VARIABLES
	//EXPORTS
	[Export] private AnimatedSprite2D animatedSprite2D;

	//PROPERTIES
	[Export] private float speed = 700;
	[Export] private float accelerationMultiplier = 1;
	private Vector2 startPos;
	private int direction = -1;
	private float acceleration = 0;
	public bool active = false;
	private bool stopAction = false;
	public List<Node2D> objectsInArea = new List<Node2D>();
	#endregion <--->

	#region FUNCTIONS

	#endregion <--->

	#region COROUTINES
	private async void Move()
	{
		stopAction = false;
		Global.Instance.PlayPositionalSound(Global.sfx_rhinoCharge, GlobalPosition, 150);
		animatedSprite2D.Animation = "charge";
		while(active && !stopAction)
		{
			if(GetTree().Paused == false)
			{
				acceleration = Mathf.Clamp(acceleration + (accelerationMultiplier * (float)GetPhysicsProcessDeltaTime()), 0, 1);
				Translate(new Vector2(direction * acceleration * speed * (float)GetPhysicsProcessDeltaTime(), 0));
			}
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}
	}
	#endregion

	#region SIGNALS
	public void OnFrameChange()
	{
		switch(animatedSprite2D.Animation)
		{
			case "charge":
				switch(animatedSprite2D.Frame)
				{
					case 2:
						Global.Instance.PlayPositionalSound(Global.sfx_rhinoSteps[0], GlobalPosition);
					break;
					case 5:
						Global.Instance.PlayPositionalSound(Global.sfx_rhinoSteps[1], GlobalPosition);
					break;
				}
			break;
		}
	}

	public void OnAnimationFinished()
	{
		switch(animatedSprite2D.Animation)
		{
			case "hit":
			direction = -direction;
			animatedSprite2D.FlipH = !animatedSprite2D.FlipH;
			animatedSprite2D.Animation = "idle";
			animatedSprite2D.Play();
			break;
		}
	}

	public void OnTriggerAreaEnter(Node2D body)
	{
		if(body.IsInGroup(Global.playerGroup) || body.IsInGroup(Global.bossGroup))
		{
			objectsInArea.Add(body);
			if(objectsInArea.Count == 0)
				GD.Print("No objects in rhino area");
			else
				for(int i = 0; i < objectsInArea.Count; i++)
					GD.Print(i + ":" + objectsInArea[i].Name);
		}
	}

	public void OnTriggerAreaExit(Node2D body)
	{
		if(body.IsInGroup(Global.playerGroup) || body.IsInGroup(Global.bossGroup))
		{
			objectsInArea.Remove(body);
			if(objectsInArea.Count == 0)
				GD.Print("No objects in rhino area");
			else
				for(int i = 0; i < objectsInArea.Count; i++)
					GD.Print(i + ":" + objectsInArea[i].Name);
		}
	}

	public void OnObjectAreaEnter(Node2D body)
	{
		if(body.Name == Global.playerGroup)
		{
			Player.Instance.Die();
		}
		else if(body.IsInGroup(Global.bossGroup))
		{
			body.GetNode<EnemyFatBird>(".").SetAndRunAction(EnemyFatBird.FatBirdActions.Hit, direction);
		}
		else if(body.Name == "TileMap")
		{
			active = false;
			animatedSprite2D.Animation = "hit";
			acceleration = 0;
			Global.Instance.PlayPositionalSound(Global.sfx_thud, GlobalPosition);
			GetTree().GetNodesInGroup(Global.switchGroup)[0].GetNode<Switch>(".").RotateGates(false);
		}
	}

	private void ResetRhino()
	{
		stopAction = true;
		active = false;
		GlobalPosition = startPos;
		direction = -1;
		animatedSprite2D.Animation = "idle";
		animatedSprite2D.FlipH = false;
		acceleration = 0;
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		GameManager.Instance.Respawn += ResetRhino;
		startPos = GlobalPosition;
	}

	public override void _Process(double delta)
	{
		if(objectsInArea.Count > 0 && active == true && animatedSprite2D.Animation == "idle")
			Move();
	}
	#endregion <--->
}
