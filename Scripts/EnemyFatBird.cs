using Godot;
using System;

public partial class EnemyFatBird : CharacterBody2D
{
	#region VARIABLES
	//EXPORTS
	[Export] private AnimatedSprite2D animatedSprite2D;
	[Export] public int health = 5;
	
	public enum FatBirdActions {Idle, FollowPlayer, Smash, Bounce, Stun, Hit, Die}
	//PROPERTIES
	private Vector2 speeds = new Vector2(300, 500);
	private Vector2 directions = Vector2.Zero;
	private Vector2 playerPosOffset = new Vector2(0, -410);
	private Vector2 startPos;
	private FatBirdActions currentAction = FatBirdActions.Idle;
	private bool stopCurrentAction = false;
	private AudioStreamPlayer2D fallSoundPlayer;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    #endregion <--->

    #region FUNCTIONS
	public void SetAndRunAction(FatBirdActions newAction, int rhinoDirection = 0)
	{
		currentAction = newAction;
		ProcessAction(rhinoDirection);
	}

	private void ProcessAction(int rhinoDirection)
	{
		GD.Print("CURRENT ACTION: " + currentAction);
		switch(currentAction)
		{
			case FatBirdActions.FollowPlayer:
				FollowPlayer();
			break;

			case FatBirdActions.Smash:
				Smash();
			break;

			case FatBirdActions.Bounce:
				Bounce();
			break;

			case FatBirdActions.Stun:
				Stun();
			break;

			case FatBirdActions.Hit:
				Hit(rhinoDirection);
			break;

			case FatBirdActions.Die:
				Die();
			break;
		}
	}

	private void Move(Vector2 target, bool snapYCoordinates = true)
	{
		float clampValue = 1;
		directions.X = Mathf.Clamp(directions.X + target.X * (float)GetProcessDeltaTime(), -clampValue, clampValue);
		if(snapYCoordinates)
			directions.Y = target.Y;
		else
			directions.Y = Mathf.Clamp(directions.Y + target.Y * (float)GetProcessDeltaTime(), -clampValue, clampValue);
		Velocity = ScaledSpeed() * directions;
		MoveAndSlide();
	}

	private Vector2 ScaledSpeed()
	{
		int healthOffset = 5;
		int xIncrement = 100;
		int yIncrement = 35;
		int multiplier;
		if(currentAction == FatBirdActions.Smash)
			multiplier = 2;
		else
			multiplier = 1;
		return new Vector2(speeds.X + (xIncrement * (healthOffset - health)), speeds.Y * multiplier + (yIncrement * (healthOffset - health)));
	}

	private void SetBossHealth(int hp)
	{
		health = hp;
		GameManager.Instance.UpdateBossHPCounter(health);
	}

	private void ResetMovement()
	{
		Velocity = Vector2.Zero;
		directions = Vector2.Zero;
	}

	private void PlayFallSound(float volumePercentage = 100)
	{
		fallSoundPlayer = Global.positionalAudioPlayerPrefab.Instantiate<AudioStreamPlayer2D>();
		GetTree().CurrentScene.AddChild(fallSoundPlayer);
		fallSoundPlayer.Position = GlobalPosition;
		fallSoundPlayer.Stream = Global.sfx_bossFall;
		fallSoundPlayer.VolumeDb = Mathf.LinearToDb(volumePercentage / 100);
		fallSoundPlayer.Play();
	}

	private void StopFallSound()
	{
		fallSoundPlayer.Stop();
		fallSoundPlayer.QueueFree();
	}
    #endregion <--->

	#region COROUTINES
	private async void FollowPlayer()
	{
		double timer = health * 2f;
		stopCurrentAction = false;
		animatedSprite2D.Animation = "fly";
		while(timer > 0 && stopCurrentAction == false)
		{
			if(GetTree().Paused == false)
			{
				timer -= GetProcessDeltaTime();
				Move((Player.Instance.GlobalPosition + playerPosOffset - GlobalPosition).Normalized());
			}
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}
		if(stopCurrentAction == false)
			SetAndRunAction(FatBirdActions.Smash);
	}

	private async void Smash()
	{
		Vector2 initialPos = GlobalPosition;
		stopCurrentAction = false;
		ResetMovement();
		PlayFallSound();
		animatedSprite2D.Animation = "fall";
		do
		{
			if(GetTree().Paused == false)
				Move((Vector2.Down * 99999).Normalized(), false);
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		} while(Velocity.Y > 0 && stopCurrentAction == false);
		StopFallSound();
		if(stopCurrentAction == false)
		{
			if(Mathf.Abs(initialPos.Y - GlobalPosition.Y) >= 550)
				SetAndRunAction(FatBirdActions.Stun);
			else
				SetAndRunAction(FatBirdActions.Bounce);
		}
	}

	private async void Bounce()
	{
		double timer = 0.33f;
		stopCurrentAction = false;
		ResetMovement();
		Global.Instance.PlayPositionalSound(Global.sfx_bossBounce, GlobalPosition);
		animatedSprite2D.Animation = "ground";
		animatedSprite2D.Play();
		while(timer > 0 && stopCurrentAction == false)
		{
			if(GetTree().Paused == false)
			{
				timer -= GetProcessDeltaTime();
				Move((Vector2.Up * 99999).Normalized(), false);
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		if(stopCurrentAction == false)
			SetAndRunAction(FatBirdActions.FollowPlayer);
	}

	private async void Stun()
	{
		double timer = 2.5;
		stopCurrentAction = false;
		Global.Instance.PlayPositionalSound(Global.sfx_bossThud, GlobalPosition);
		animatedSprite2D.Animation = "stun";
		animatedSprite2D.Play();
		while(timer > 0 && stopCurrentAction == false)
		{
			if(GetTree().Paused == false)
				timer -= GetProcessDeltaTime();
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		if(stopCurrentAction == false)
			SetAndRunAction(FatBirdActions.Bounce);
	}

	public async void Hit(int rhinoDirection)
	{
		stopCurrentAction = true;
		SetBossHealth(--health);
		GD.Print(ScaledSpeed());
		if(health == 0)
		{
			SetAndRunAction(FatBirdActions.Die);
		}
		else
		{
			double timer = 0.3f;
			Global.Instance.PlayPositionalSound(Global.sfx_bossHit, GlobalPosition);
			animatedSprite2D.Animation = "hit";
			animatedSprite2D.Play();
			while(timer > 0)
			{
				if(GetTree().Paused == false)
				{
					timer -= GetProcessDeltaTime();
					Move(new Vector2(speeds.X * rhinoDirection * (float)GetProcessDeltaTime(), -speeds.Y * (float)GetProcessDeltaTime()), false);
				}
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			stopCurrentAction = false;
		}
	}

	public async void Die()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		stopCurrentAction = false;
		double timer = 2;
		double explosionTimer = 0.25;
		animatedSprite2D.Animation = "die";
		animatedSprite2D.Play();
		while(timer > 0 && stopCurrentAction == false)
		{
			if(GetTree().Paused == false)
			{
				timer -= GetProcessDeltaTime();
				explosionTimer -= GetProcessDeltaTime();
				if(explosionTimer <= 0)
				{
					RandomNumberGenerator random = new RandomNumberGenerator();
					Vector2 randomPos = new Vector2(random.RandfRange(-10f, 10f), random.RandfRange(-10f, 10f));
					Global.Instance.InstantiateExplosion(GlobalPosition + randomPos);
					explosionTimer = 0.25;
				}
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		if(stopCurrentAction == false)
		{
			Global.Instance.InstantiateExplosion(GlobalPosition, 5);
			Visible = false;
			await ToSignal(GetTree().CreateTimer(1, false), Timer.SignalName.Timeout);
			GameManager.Instance.CheckForWin();
		}
	}
	#endregion <--->

    #region SIGNALS
	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup(Global.playerGroup) && health > 0)
			Player.Instance.Die();
	}

	public void OnFrameChange()
	{
		switch(animatedSprite2D.Animation)
		{
			case "fly":
				switch(animatedSprite2D.Frame)
				{
					case 4:
						Global.Instance.PlayPositionalSound(Global.sfx_bossWingFlap, GlobalPosition);
					break;
				}
			break;
		}
	}

	public void OnAnimationFinished()
	{
		switch(animatedSprite2D.Animation)
		{
			case "ground":
				animatedSprite2D.Animation = "fly";
				animatedSprite2D.Play();
			break;
			case "hit":
				animatedSprite2D.Animation = "fly";
				animatedSprite2D.Play();
				SetAndRunAction(FatBirdActions.FollowPlayer);
			break;
		}
	}
	private async void ResetBoss()
	{
		SetBossHealth(5);
		Visible = true;
		GlobalPosition = startPos;
		stopCurrentAction = true;
		ResetMovement();
		animatedSprite2D.Play();
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		SetAndRunAction(FatBirdActions.FollowPlayer);
	}
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        GameManager.Instance.GameStart += () => SetAndRunAction(FatBirdActions.FollowPlayer);
		GameManager.Instance.Respawn += ResetBoss;
		startPos = GlobalPosition;
		GameManager.Instance.UpdateBossHPCounter(health);
    }

    public override void _PhysicsProcess(double delta)
	{
		
	}
	#endregion <--->
}
