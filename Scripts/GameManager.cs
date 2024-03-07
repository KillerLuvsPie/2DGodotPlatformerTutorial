using Godot;
using System;

public partial class GameManager : Node
{
	#region VARIABLES
	//SINGLETON
	public static GameManager Instance;

	//EXPORTS
	//COUNTDOWN TEXT
	[Export] private Texture2D[] countdownSprites = new Texture2D[4];

	//UI
	[Export] private TextureRect countdownDisplay;
	[Export] private Label fruitLabel;
	[Export] private Label timeLabel;
	[Export] private Panel pauseMenu;
	[Export] private Panel winScreen;
	[Export] private Panel loseScreen;
	[Export] private Label deathTotal;
	[Export] private Label timeTotal;
	[Export] private AnimationPlayer fadeAnimator;

	//CAMERA
	[Export] private Camera2D camera2D;
	[Export] private Marker2D cameraLeftBound;
	[Export] private Marker2D cameraBottomBound;
	[Export] private Marker2D cameraRightBound;
	[Export] private Marker2D cameraUpperBound;

	//AUDIO PLAYERS
	[Export] private AudioStreamPlayer musicPlayer;
	
	//CONTROL
	public bool pausable = false;
	public bool playerControl = false;
	public bool started = false;

	//STATS
	private int fruitCounter;
	private int deathCounter = 0;
	public Vector2 currentCheckpoint;

	//TIME
	private double timeElapsed = 0;
	private int minutes = 0;
	private int seconds = 0;
	#endregion <--->

	#region FUNCTIONS
	private void SetCameraLimits()
	{
		camera2D.LimitLeft = (int)cameraLeftBound.Position.X;
		camera2D.LimitBottom = (int)cameraBottomBound.Position.Y;
		camera2D.LimitRight = (int)cameraRightBound.Position.X;
		camera2D.LimitTop = (int)cameraUpperBound.Position.Y;
	}

	private void SetFruitCounter()
	{
		fruitCounter = GetTree().GetNodesInGroup(Global.fruitGroup).Count;
		fruitLabel.Text = "Fruit: " + fruitCounter;
	}

	public void SetCurrentCheckpoint(Vector2 pos)
	{
		currentCheckpoint = pos;
	}

	private void SetMusicVolume(float volumePercentage)
	{
		musicPlayer.VolumeDb = Mathf.LinearToDb(volumePercentage / 100);
	}

	public void DecreaseFruitCounter()
	{
		fruitCounter--;
		fruitLabel.Text = "Fruit: " + fruitCounter;
		CheckForWin();
	}

	private void MoveEnemies()
	{
		for(int i = 0; i < GetTree().GetNodesInGroup(Global.enemyGroup).Count; i++)
		{
			if(GetTree().GetNodesInGroup(Global.enemyGroup)[i].GetType() == typeof(EnemyPig))
			{
				EnemyPig enemyPig = GetTree().GetNodesInGroup(Global.enemyGroup)[i].GetNode<EnemyPig>(".");
				enemyPig.EmitSignal(EnemyPig.SignalName.GameStarted);
			}
		}
	}
	public void TogglePause()
	{
		GetTree().Paused = !GetTree().Paused;
		pauseMenu.Visible = !pauseMenu.Visible;
		if(GetTree().Paused == true)
		{
			SetMusicVolume(20);
			Global.Instance.PlaySound(Global.sfx_pause, 75);
		}
		else
		{
			SetMusicVolume(100);
			Global.Instance.PlaySound(Global.sfx_unpause);
		}	
	}

	private void TimerFunction(double delta)
	{
		timeElapsed += delta;
		minutes = Mathf.FloorToInt(timeElapsed / 60);
		seconds = Mathf.FloorToInt(timeElapsed % 60);
		timeLabel.Text = minutes.ToString("00") + ":" + seconds.ToString("00");
	}

	private void CheckForWin()
	{
		if(fruitCounter == 0)
		{
			pausable = false;
			SetMusicVolume(20);
			Global.Instance.PlaySound(Global.sfx_win);
			GetTree().Paused = true;
			deathTotal.Text = "Deaths: " + deathCounter.ToString();
			timeTotal.Text = "Time cleared: " + minutes.ToString("00") + ":" + seconds.ToString("00");
			ResultsToGlobal();
			winScreen.Visible = true;
		}
	}

	public void GameOver()
	{
		deathCounter++;
		loseScreen.Visible = true;
		SetMusicVolume(20);
		GetTree().Paused = true;
	}

	private void ResultsToGlobal()
	{
		Global.levelTimeRecords[Global.currentLevelIndex] = timeElapsed;
		Global.levelDeathRecords[Global.currentLevelIndex] = deathCounter;
		if(Global.UnlockedLevelIndex <= Global.currentLevelIndex && Global.UnlockedLevelIndex < 5)
			Global.UnlockedLevelIndex = Global.currentLevelIndex + 1;
	}

	/*public void PlaySound(AudioStream audioStream)
	{
		AudioStreamPlayer player = Global.audioPlayerPrefab.Instantiate<AudioStreamPlayer>();
		GetTree().CurrentScene.AddChild(player);
		player.Stream = audioStream;
		player.Play();
	}*/
	#endregion <--->

	#region COROUTINES
	private async void Countdown()
	{
		int timer = 3;
		AnimationPlayer countdownAnimation = countdownDisplay.GetChild<AnimationPlayer>(0);
		while(timer > 0)
		{
			countdownDisplay.Texture = countdownSprites[timer];
			countdownAnimation.Play(Global.str_fadeOut);
			timer--;
			await ToSignal(GetTree().CreateTimer(1f, false), Timer.SignalName.Timeout);
		}
		started = true;
		playerControl = true;
		pausable = true;
		countdownAnimation.Play();
		countdownDisplay.Texture = countdownSprites[timer];
		MoveEnemies();
		await ToSignal(GetTree().CreateTimer(1f, false), Timer.SignalName.Timeout);
		countdownDisplay.Hide();
	}

	private async void SceneTransition(int num)
	{
		Global.currentLevelIndex = num;
        ShowFadeEffect(true);
        FadeOutMusic();
        await ToSignal(fadeAnimator, AnimationPlayer.SignalName.AnimationFinished);
        GetTree().ChangeSceneToFile(Global.GetScenePath());
	}

	private async void FadeOutMusic()
    {
        while(Mathf.DbToLinear(musicPlayer.VolumeDb) > 0)
        {
            musicPlayer.VolumeDb -= 1;
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }
	#endregion <--->

	#region SIGNALS
	//BUTTONS
	public void OnRetryButtonPressed()
	{
		loseScreen.Visible = false;
		SetMusicVolume(100);
		GetTree().Paused = false;
		Player.Instance.Respawn();	
	}

	public void OnNextLevelButtonPressed()
	{
		SceneTransition(++Global.currentLevelIndex);
	}

	public void OnExitButtonPressed()
	{
		SceneTransition(0);
	}

	//ANIMATIONS
	public void FadeAnimationFinished(string animationName)
	{
		if(animationName == Global.str_fadeIn)
		{
			ShowFadeEffect(false);
			musicPlayer.Play();
			Countdown();
		}
	}

	public void ShowFadeEffect(bool show)
	{
		ColorRect parent = fadeAnimator.GetParent<ColorRect>();
		if(show)
		{
			parent.Show();
			fadeAnimator.Play(Global.str_fadeOut);
		}
		else
			parent.Hide();
	}

	//GAME DATA
	public void LateReady()
	{
		SetCurrentCheckpoint(Player.Instance.Position);
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		if(Instance != this && Instance != null && IsInstanceValid(Instance))
			QueueFree();
		else
			Instance = this;
		timeLabel.Text = "00:00";
		SetFruitCounter();
		SetCameraLimits();
		GetTree().Paused = false;
		fadeAnimator.Play(Global.str_fadeIn);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(started)
			TimerFunction(delta);
	}
	#endregion <--->
}