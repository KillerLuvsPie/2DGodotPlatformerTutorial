using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
	//SINGLETON
	public static Global Instance;

	#region GROUP NAMES
	public const string controlButtonGroup = "ControlButtons";
	public const string levelButtonGroup = "LevelButtons";
	public const string fruitGroup = "Fruit";
	public const string enemyGroup = "Enemy";
	#endregion <--->

	#region ANIMATION NAMES
	public const string str_fadeIn = "FadeIn";
	public const string str_fadeOut = "FadeOut";
	#endregion <--->

	#region OPTION VARIABLES
	public readonly Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>()
	{
		{"800 x 600", new Vector2I(800, 600)},
		{"1280 x 720", new Vector2I(1280, 720)},
		{"1920 x 1080", new Vector2I(1920, 1080)} 
	};
	#endregion <--->

	#region LEVEL VARIABLES AND FUNCTIONS
	public static int currentLevelIndex = 0;
	public static int UnlockedLevelIndex = 1;
	public static Dictionary<int, double?> levelTimeRecords = new Dictionary<int, double?>();
	public static Dictionary<int, int?> levelDeathRecords = new Dictionary<int, int?>();

	public static string GetScenePath()
	{
		if(currentLevelIndex == 0)
			return "res://Scenes/MainMenu.scn";
		return "res://Scenes/Level"+ currentLevelIndex +".scn";
	}

	private static void InitializeLevelLists()
	{
		int levelTotal = 5;
		for(int i = 1; i <= levelTotal; i++)
		{
			levelTimeRecords.Add(i, null);
			levelDeathRecords.Add(i, null);
		}
		GD.Print("Lists initialized successfully");
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		if(Instance != this && Instance != null && IsInstanceValid(Instance))
			QueueFree();
		else
			Instance = this;
		InitializeLevelLists();
	}
	#endregion <--->
}
