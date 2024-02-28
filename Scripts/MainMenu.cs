using Godot;
using System;
using System.Linq;

public partial class MainMenu : CanvasLayer
{
    #region VARIABLES
    //SCREENS
    [Export] private Control mainMenuScreen;
    [Export] private Control levelSelectScreen;
    [Export] private Control optionsScreen;
    [Export] private Control controlsScreen;
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

    //CONTROLS REMAPPING
    private string[] controlButtonActions;
    #endregion <--->

    #region FUNCTIONS
    private void InitializeOptions()
    {
        //VIDEO
        windowModeOptions.Select(1);
        for(int i = 0; i < Global.Instance.Resolutions.Count; i++)
        {
            resolutionOptions.AddItem(Global.Instance.Resolutions.ElementAt(i).Key);
            if(DisplayServer.ScreenGetSize() == Global.Instance.Resolutions.ElementAt(i).Value)
                resolutionOptions.Select(i);
        }

        //SOUND
        musicSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music")));
        musicSliderLabel.Text = (musicSlider.Value * 100).ToString("0") + "%";
        sfxSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("SFX")));
        sfxSliderLabel.Text = (sfxSlider.Value * 100).ToString("0") + "%";
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
        if(Global.levelTimeRecords[i] == null)
            timeLabel.Text = "Time: --:--";
        else
        {
            int minutes = Mathf.FloorToInt((float)Global.levelTimeRecords[i] / 60);
            int seconds = Mathf.FloorToInt((float)Global.levelTimeRecords[i] % 60);
            timeLabel.Text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }   
        if(Global.levelDeathRecords[i] == null)
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
    public void OptionsBackButtonClick()
    {
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
    }

    public void ResolutionSelected(int id)
    {
        DisplayServer.WindowSetSize(Global.Instance.Resolutions.ElementAt(id).Value);
    }

    public void MusicSliderValueChange(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(value));
        musicSliderLabel.Text = (musicSlider.Value * 100).ToString("0") + "%";
    }

    public void SFXSliderValueChange(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb(value));
        sfxSliderLabel.Text = (sfxSlider.Value * 100).ToString("0") + "%";
    }

    //CONTROLS MENU
    public void ControlsBackButtonClick()
    {
        controlsScreen.Visible = false;
        optionsScreen.Visible = true;
    }
    #endregion <--->

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        GetTree().Paused = false;
        InitializeOptions();
        InitializeButtonLevels();
        fadeAnimator.Play(Global.str_fadeIn);
    }
    #endregion <--->
}