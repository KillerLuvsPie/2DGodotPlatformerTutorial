using Godot;
using System;

public partial class EnemyPig : Node2D
{
	#region VARIABLES
	//EXPORTS
	[Export]
	private Marker2D leftBound;
	[Export]
	private Marker2D rightBound;
	[Export]
	private AnimatedSprite2D animatedSprite2D;
	//PROPERTIES
	private float speed = 150;
	private int direction = -1;
	#endregion <--->

	#region FUNCTIONS
	private void MoveCharacter(double delta)
	{
		if(Position <= leftBound.Position && direction < 0)
		{
			direction = 1;
			animatedSprite2D.FlipH = true;
		}
			
		else if(Position >= rightBound.Position && direction > 0)
		{
			direction = -1;
			animatedSprite2D.FlipH = false;
		}
			
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
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveCharacter(delta);
	}
	#endregion <--->
}
