using Godot;
using System;

public partial class MovingPlatform : Node2D
{
	#region VARIABLES
	[Export] private Path2D path;
	[Export] private PathFollow2D pathPoint;
	[Export] private Line2D line;
	[Export] private CompressedTexture2D pointTexture;
	[Export] private Vector2 pointTextureScale = new Vector2(1,1);
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
	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup(Global.playerGroup))
			Player.Instance.Die();
	}
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
			sprite.Scale = pointTextureScale;
			path.AddChild(sprite);
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		pathPoint.ProgressRatio += speed * (float)delta;
		ChangeDirection();
		GD.Print(Name + ": " + pathPoint.ProgressRatio + " |||| Speed: " + speed + " * Delta: " + delta + " = " + (speed * delta));
	}
	#endregion <--->
}
