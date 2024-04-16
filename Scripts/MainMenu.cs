using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.IO;

public partial class MainMenu : CanvasLayer
{
    [Signal] public delegate void OptionChangedEventHandler();
    #region VARIABLES
    //SCREENS
    [Export] private Control mainMenuScreen;
    [Export] private Control levelSelectScreen;
    [Export] private Control optionsScreen;
    [Export] private Control controlsScreen;
    [Export] private Control creditsScreen;
    [Export] private AnimationPlayer fadeAnimator;

    //LEVEL STATS PANEL
    [Export] private HBoxContainer statsContainer;
    [Export] private Label timeLabel;
    [Export] private Label deathLabel;

    //OPTION ELEMENTS
    [Export] private OptionButton windowModeOptions;
    [Export] private OptionButton resolutionOptions;
    [Export] private HSlider musicSlider;
    [Export] private Label musicSliderLabel;
    [Export] private HSlider sfxSlider;
    [Export] private Label sfxSliderLabel;

    //MUSIC PLAYER
    [Export] private AudioStreamPlayer musicPlayer;
    
    //CHECK FOR OPTIONS CHANGE
    public static bool optionsChanged = false;
    #endregion <--->

    #region FUNCTIONS
    private void InitializeOptions()
    {
        //VIDEO
        windowModeOptions.Select(Global.windowOptionIndex);
        WindowModeSelected(Global.windowOptionIndex);
        for(int i = 0; i < Global.Resolutions.Count; i++)
        {
            resolutionOptions.AddItem(Global.Resolutions.ElementAt(i).Key);
            if(Global.currentResolution == resolutionOptions.GetItemText(i))
            {
                resolutionOptions.Select(i);
                ResolutionSelected(i);
            }
        }

        //SOUND
        MusicSliderValueChange(Global.musicVolumePercent / 100f);
        SFXSliderValueChange(Global.sfxVolumePercent /100f);
    }

    private void InitializeButtonLevels()
    {
        for(int i = 0; i < GetTree().GetNodesInGroup(Global.levelButtonGroup).Count; i++)
        {
            Button button = (Button)GetTree().GetNodesInGroup(Global.levelButtonGroup)[i];
            if(i < Global.UnlockedLevelIndex)
            {
                button.Disabled = false;
                button.MouseFilter = Control.MouseFilterEnum.Stop;
            }
            int index = i + 1;
            button.MouseEntered += () => OnHoverDisplayLevelStats(index);
            button.MouseExited += OnHoverExitHideLevelStats;
        }
    }

    private void ShowFadeEffect(bool show)
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

    private void SaveControlsToResource()
    {
        Dictionary<string, Array<InputEvent>> newInputsToSave = new Dictionary<string, Array<InputEvent>>();
        foreach(StringName str in InputMap.GetActions())
        {
            if(!str.ToString().Contains("ui_"))
            {
                newInputsToSave.Add(str, InputMap.ActionGetEvents(str)); 
            }
        }
        Global.savedInputsInstance.inputList = newInputsToSave;
        ResourceSaver.Save(Global.savedInputsInstance, Global.SavedInputPath);
        GD.Print(Global.savedInputsInstance.PrintInputs());
    }

    private void LoadControlsFromResource()
    {
        GD.Print(Global.savedInputsInstance.PrintInputs());
        for(int i = 0; i < InputMap.GetActions().Count; i++)
        {
            if(Global.savedInputsInstance.inputList.ContainsKey(InputMap.GetActions()[i]))
            {
                InputMap.ActionEraseEvents(InputMap.GetActions()[i]);
                foreach(InputEvent input in Global.savedInputsInstance.inputList[InputMap.GetActions()[i]])
                {
                    InputMap.ActionAddEvent(InputMap.GetActions()[i], input);
                }
            }
        }
    }
    #endregion <--->

    #region COROUTINES
    private async void FadeOutMusic()
    {
        while(Mathf.DbToLinear(musicPlayer.VolumeDb) > 0)
        {
            musicPlayer.VolumeDb -= 1;
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }

    private async void SceneTransition(int num)
    {
        Global.currentLevelIndex = num;
        ShowFadeEffect(true);
        FadeOutMusic();
        await ToSignal(fadeAnimator, AnimationPlayer.SignalName.AnimationFinished);
        GetTree().ChangeSceneToFile(Global.GetScenePath());
    }
    #endregion <--->

    #region SIGNALS
    //MAIN SCREEN
    public void StartButtonClick()
    {
        mainMenuScreen.Visible = false;
        levelSelectScreen.Visible = true;
    }

    public void OptionsButtonClick()
    {
        mainMenuScreen.Visible = false;
        optionsScreen.Visible = true;
    }

    public void CreditsButtonClick()
    {
        mainMenuScreen.Visible = false;
        creditsScreen.Visible = true;
    }

    public void QuitButtonClick()
    {
        GetTree().Quit();
    }

    public void FadeAnimationFinished(string animationName)
    {
        if(animationName == Global.str_fadeIn)
        {
            ShowFadeEffect(false);
            musicPlayer.Play();
            optionsChanged = false;
        }
    }

    //LEVEL SELECT MENU
    public void LevelSelectBackButtonClick()
    {
        levelSelectScreen.Visible = false;
        mainMenuScreen.Visible = true;
    }

    public void LevelButtonClick(int num)
    {
        SceneTransition(num);
    }

    public void OnHoverDisplayLevelStats(int i)
    {
        if(Global.levelTimeRecords[i] == -1)
            timeLabel.Text = "Time: --:--";
        else
        {
            int minutes = Mathf.FloorToInt((float)Global.levelTimeRecords[i] / 60);
            int seconds = Mathf.FloorToInt((float)Global.levelTimeRecords[i] % 60);
            timeLabel.Text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }   
        if(Global.levelDeathRecords[i] == -1)
            deathLabel.Text = "Deaths: ---";
        else
            deathLabel.Text = "Deaths: " + Global.levelDeathRecords[i].ToString();
        statsContainer.Show();
    }

    public void OnHoverExitHideLevelStats()
    {
        statsContainer.Hide();
    }
    //OPTIONS MENU
    private void OnOptionChanged()
    {
        optionsChanged = true;
    }
    public void OptionsBackButtonClick()
    {
        if(optionsChanged)
        {
            optionsChanged = false;
            SaveLoadManager.Save();
            SaveControlsToResource();
        }
        optionsScreen.Visible = false;
        mainMenuScreen.Visible = true;
    }

    public void OptionsControlsButtonClick()
    {
        optionsScreen.Visible = false;
        controlsScreen.Visible = true;
    }

    public void WindowModeSelected(int id)
    {
        switch (id)
        {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 2:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
            case 3:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
        }
        Global.windowOptionIndex = id;
        EmitSignal(SignalName.OptionChanged);
    }

    public void ResolutionSelected(int id)
    {
        DisplayServer.WindowSetSize(Global.Resolutions.ElementAt(id).Value);
        Global.currentResolution = Global.Resolutions.ElementAt(id).Key;
        EmitSignal(SignalName.OptionChanged);
    }

    public void MusicSliderValueChange(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(value));
        musicSlider.Value = value;
        musicSliderLabel.Text = (value * 100).ToString("0") + "%";
        Global.musicVolumePercent = value * 100;
        EmitSignal(SignalName.OptionChanged);
    }

    public void SFXSliderValueChange(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb(value));
        sfxSlider.Value = value;
        sfxSliderLabel.Text = (value * 100).ToString("0") + "%";
        Global.sfxVolumePercent = value * 100;
        EmitSignal(SignalName.OptionChanged);
    }

    //CONTROLS MENU
    public void ControlsBackButtonClick()
    {
        controlsScreen.Visible = false;
        optionsScreen.Visible = true;
    }

    //CREDITS MENU
    public void CreditsBackButtonClick()
    {
        creditsScreen.Visible = false;
        mainMenuScreen.Visible = true;
    }

    public void OnLinkClick(string url)
    {
        OS.ShellOpen(url);
    }
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        GetTree().Paused = false;
        InitializeOptions();
        InitializeButtonLevels();
        fadeAnimator.Play(Global.str_fadeIn);
        if(Global.CheckIfSavedInputsFileExists())
            LoadControlsFromResource();
        OptionChanged += OnOptionChanged;
    }
    #endregion <--->
}