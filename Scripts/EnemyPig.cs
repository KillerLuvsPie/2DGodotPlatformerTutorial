using Godot;
using System;

public partial class EnemyPig : Node2D
{
	#region VARIABLES
	//CUSTOM SIGNAL
	[Signal] public delegate void GameStartedEventHandler();

	//EXPORTS
	[Export] private Marker2D leftBound;
	[Export] private Marker2D rightBound;
	[Export] private AnimatedSprite2D animatedSprite2D;

	//PROPERTIES
	[Export] private float speed = 150;
	[Export] private int direction = -1;
	#endregion <--->

	#region FUNCTIONS
	private void InvertDirection()
	{
		if(direction == 1)
			direction = -1;
		else
			direction = 1;
	}
	private void MoveCharacter(double delta)
	{
		if(Position <= leftBound.Position && direction < 0)
			direction = 1;
		else if(Position >= rightBound.Position && direction > 0)
			direction = -1;
			
		Translate(new Vector2(speed * direction * (float)delta, 0));
	}
	#endregion <--->

	#region SIGNALS
	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup("Player"))
		{
			Player.Instance.Die();
		}
		if(body.Name == "TileMap")
		{
			InvertDirection();
		}
		
	}

	public void OnGameStart()
	{
		animatedSprite2D.Play("walk");
	}
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _PhysicsProcess(double delta)
	{
		if(GameManager.Instance.started)
			MoveCharacter(delta);
		animatedSprite2D.FlipH = direction == 1;
	}
	#endregion <--->
}
