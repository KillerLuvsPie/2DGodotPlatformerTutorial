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
	private int direction = -1;
	private float acceleration = 0;
	public bool active = false;
	public List<Node2D> objectsInArea = new List<Node2D>();
	#endregion <--->

	#region FUNCTIONS

	#endregion <--->

	#region COROUTINES
	private async void Move()
	{
		animatedSprite2D.Animation = "charge";
		while(active)
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
			if(active == true && animatedSprite2D.Animation == "idle")
				Move();
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
			//INSERT FETCHING THE BOSS DAMAGE FUNCTION HERE
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
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
			
	}

	public override void _Process(double delta)
	{

	}
	#endregion <--->
}
