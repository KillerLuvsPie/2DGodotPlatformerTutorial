using Godot;
using System;

public partial class Global : Node
{
	//SINGLETON
	public static Global Instance;

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
