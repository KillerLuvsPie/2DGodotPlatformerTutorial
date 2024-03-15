using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
	//SINGLETON
	public static Global Instance;

	#region RESOURCES
	//SFX
	//UI SFX
	public static readonly AudioStream sfx_button = GD.Load<AudioStream>("res://Assets/Sound/SFX/General Sounds/Buttons/sfx_sounds_button11.wav");
	public static readonly AudioStream sfx_pause = GD.Load<AudioStream>("res://Assets/Sound/SFX/General Sounds/Pause Sounds/sfx_sounds_pause1_in.wav");
	public static readonly AudioStream sfx_unpause = GD.Load<AudioStream>("res://Assets/Sound/SFX/General Sounds/Pause Sounds/sfx_sounds_pause1_out.wav");
	public static readonly AudioStream sfx_win = GD.Load<AudioStream>("res://Assets/Sound/SFX/General Sounds/Positive Sounds/sfx_sounds_powerup18.wav");

	//OBJECT SFX
	public static readonly AudioStream sfx_fruitGet = GD.Load<AudioStream>("res://Assets/Sound/SFX/General Sounds/Interactions/sfx_sounds_interaction1.wav");

	//PLAYER SFX
	public static readonly AudioStream sfx_jump = GD.Load<AudioStream>("res://Assets/Sound/SFX/Movement/Jumping and Landing/sfx_movement_jump7.wav");
	public static readonly AudioStream sfx_landing = GD.Load<AudioStream>("res://Assets/Sound/SFX/Movement/Jumping and Landing/sfx_movement_jump9_landing.wav");
	public static readonly AudioStream sfx_fall = GD.Load<AudioStream>("res://Assets/Sound/SFX/Movement/Falling Sounds/sfx_sounds_falling1.wav");
	public static readonly AudioStream[] sfx_deaths =
	{
		GD.Load<AudioStream>("res://Assets/Sound/SFX/General Sounds/Negative Sounds/sfx_sounds_damage1.wav"),
		GD.Load<AudioStream>("res://Assets/Sound/SFX/Explosions/Short/sfx_exp_short_soft7.wav")
	};

	//ENEMY SFX
	public static readonly AudioStream sfx_trunkShoot = GD.Load<AudioStream>("res://Assets/Sound/SFX/Weapons/Single Shot Sounds/sfx_weapon_singleshot12.wav");
	
	#endregion <--->

	#region INSTANTIABLE OBJECTS
	public static readonly PackedScene audioPlayerPrefab = GD.Load<PackedScene>("res://Prefabs/sfx_player.res");
	public static readonly PackedScene enemyBullet = GD.Load<PackedScene>("res://Prefabs/enemy_bullet.res");
	#endregion <--->

	#region GROUP NAMES
	public const string controlButtonGroup = "ControlButtons";
	public const string levelButtonGroup = "LevelButtons";
	public const string fruitGroup = "Fruit";
	public const string enemyGroup = "Enemy";
	public const string playerGroup = "Player";
	public const string platformGroup = "Platform";
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

	public static float musicVolumePercent = 80;
	public static float sfxVolumePercent = 100;
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

	#region SOUND FUNCTIONS
	public void PlaySound(AudioStream audioStream, float volumePercentage = 100)
	{
		AudioStreamPlayer audioPlayer = audioPlayerPrefab.Instantiate<AudioStreamPlayer>();
		GetTree().CurrentScene.AddChild(audioPlayer);
		audioPlayer.Stream = audioStream;
		audioPlayer.VolumeDb = Mathf.LinearToDb(volumePercentage / 100);
		audioPlayer.Play();
	}
	#endregion <--->

	#region MATH FUNCTIONS
	public static Vector2 GetAbsVector2(Vector2 vec)
	{
		return new Vector2(Mathf.Abs(vec.X), Mathf.Abs(vec.Y));
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