using Godot;
using System;

public partial class MovingPlatform : Node2D
{
	#region VARIABLES
	[Export] private Path2D path;
	[Export] private PathFollow2D pathPoint;
	[Export] private Line2D line;
	[Export] private CompressedTexture2D pointTexture;
	[Export] private float speed = 0.25f;
	#endregion <--->

	#region FUNCTIONS
	private void ChangeDirection()
	{
		if(pathPoint.ProgressRatio == 1 || pathPoint.ProgressRatio == 0)
		{
			speed = -speed;
		}
	}
    #endregion <--->

    #region SIGNALS
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        for(int i = 0; i < path.Curve.PointCount; i++)
		{
			Vector2 point = path.Curve.GetPointPosition(i);
			line.AddPoint(point);
			Sprite2D sprite = new Sprite2D();
			sprite.Texture = pointTexture;
			sprite.Position = point;
			path.AddChild(sprite);
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		pathPoint.ProgressRatio += speed * (float)delta;
		ChangeDirection();
	}
	#endregion <--->
}
