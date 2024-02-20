using Godot;
using System;

public partial class GameManager : Node
{
	#region VARIABLES
	//SINGLETON
	public static GameManager Instance;
	//EXPORTS
	[Export]
	private Label fruitLabel;
	[Export]
	private Label timeLabel;
	[Export]
	private Panel pauseMenu;
	[Export]
	private Panel winScreen;
	[Export]
	private Panel loseScreen;
	[Export]
	private Label deathTotal;
	[Export]
	private Label timeTotal;
	[Export]
	private Camera2D camera2D;
	[Export]
	private Marker2D cameraLeftBound;
	[Export]
	private Marker2D cameraBottomBound;
	[Export]
	private Marker2D cameraRightBound;
	[Export]
	private Marker2D cameraUpperBound;
	//CONTROL
	public bool pausable = true;
	public bool paused = false;
	public bool playerControl = true;
	//STATS
	private int fruitCounter;
	private int deathCounter = 0;
	public Vector2 currentCheckpoint;
	//TIME
	private double timeElapsed = 0;
	private int minutes = 0;
	private int seconds = 0;
	#endregion VAARIABLES

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

	public void DecreaseFruitCounter()
	{
		fruitCounter--;
		fruitLabel.Text = "Fruit: " + fruitCounter;
		CheckForWin();
	}

	public void TogglePause()
	{
		paused = !paused;
		pauseMenu.Visible = !pauseMenu.Visible;
		playerControl = !playerControl;
		if(paused)
			Engine.TimeScale = 0;
		else
			Engine.TimeScale = 1;
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
			playerControl = false;
			Engine.TimeScale = 0;
			deathTotal.Text = "Deaths: " + deathCounter.ToString();
			timeTotal.Text = "Time cleared: " + minutes.ToString("00") + ":" + seconds.ToString("00");
			winScreen.Visible = true;
		}
	}

	public void GameOver()
	{
		deathCounter++;
		loseScreen.Visible = true;
		Engine.TimeScale = 0;
	}
	#endregion FUNCTIONS

	#region SIGNALS
	public void OnRetryButtonPressed()
	{
		loseScreen.Visible = false;
		Engine.TimeScale = 1;
		Player.Instance.Respawn();	
	}

	public void OnNextLevelButtonPressed()
	{
		Global.Instance.currentLevelIndex++;
		GetTree().ChangeSceneToFile(Global.Instance.GetScenePath());
	}

	public void OnExitButtonPressed()
	{
		Global.Instance.currentLevelIndex = 0;
		GetTree().ChangeSceneToFile(Global.Instance.GetScenePath());
	}

	public void LateReady()
	{
		SetCurrentCheckpoint(Player.Instance.Position);
	}
	#endregion SIGNALS

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		if(Instance != this && Instance != null)
			QueueFree();
		else
			Instance = this;
		SetFruitCounter();
		SetCameraLimits();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TimerFunction(delta);
	}
	#endregion GODOT FUNCTIONS
}