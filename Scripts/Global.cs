using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
	//SINGLETON
	public static Global Instance;
	#region OPTION VARIABLES
	public readonly Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>()
	{
		{"800 x 600", new Vector2I(800, 600)},
		{"1280 x 720", new Vector2I(1280, 720)},
		{"1920 x 1080", new Vector2I(1920, 1080)} 
	};
	#endregion

	#region LEVEL VARIABLES AND FUNCTIONS
	public int currentLevelIndex = 0;
	public int UnlockedLevelIndex = 1;
	
	public string GetScenePath()
	{
		if(currentLevelIndex == 0)
			return "res://Scenes/MainMenu.scn";
		return "res://Scenes/Level"+ currentLevelIndex +".scn";
	}
	#endregion

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		if(Instance != this && Instance != null)
			QueueFree();
		else
			Instance = this;
	}
	#endregion
}
