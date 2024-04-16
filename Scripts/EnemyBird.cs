using Godot;
using System;

public partial class EnemyBird : Area2D
{
	#region VARIABLES
	//EXPORTS
	[Export] private Marker2D leftTopBound;
	[Export] private Marker2D rightBottomBound;
	[Export] private AnimatedSprite2D animatedSprite2D;

	//PROPERTIES
	[Export] private float speed = 100f;
	[Export] private int direction = -1;
	private float acceleration = 0;
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
		if(Position <= leftTopBound.Position && direction < 0)
			direction = 1;
		else if(Position >= rightBottomBound.Position && direction > 0)
			direction = -1;
		
		acceleration = Mathf.Clamp(acceleration + (float)delta * direction, -1, 1);
		Translate((rightBottomBound.Position - leftTopBound.Position).Normalized() * speed * acceleration * (float)delta);
	}
	#endregion <--->

	#region SIGNALS
	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup("Player"))
		{
			Player.Instance.Die();
		}
	}
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _PhysicsProcess(double delta)
	{
		if(GameManager.Instance.started)
			MoveCharacter(delta);
		animatedSprite2D.FlipH = GlobalPosition.X < Player.Instance.GlobalPosition.X;
	}
	#endregion <--->
}
