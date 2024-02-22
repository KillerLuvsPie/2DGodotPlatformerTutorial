using Godot;
using System;
using System.Linq;

public partial class MainMenu : CanvasLayer
{
    #region VARIABLES
    //SCREENS
    [Export]
    private Control mainMenuScreen;
    [Export]
    private Control levelSelectScreen;
    [Export]
    private Control optionsScreen;
    [Export]
    private Control controlsScreen;

    //LEVEL STATS PANEL
    [Export]
    private HBoxContainer statsContainer;
    [Export]
    private Label timeLabel;
    [Export]
    private Label deathLabel;

    //OPTION ELEMENTS
    [Export]
    private OptionButton windowModeOptions;
    [Export]
    private OptionButton resolutionOptions;
    [Export]
    private HSlider musicSlider;
    [Export]
    private Label musicSliderLabel;
    [Export]
    private HSlider sfxSlider;
    [Export]
    private Label sfxSliderLabel;

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
            if(i < Global.Instance.UnlockedLevelIndex)
            {
                button.Disabled = false;
                button.MouseFilter = Control.MouseFilterEnum.Stop;
            }
            button.MouseEntered += () => OnHoverDisplayLevelStats(i);
            button.MouseExited += HoverExitHideLevelStats;
        }
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
    //LEVEL SELECT MENU
    public void LevelSelectBackButtonClick()
    {
        levelSelectScreen.Visible = false;
        mainMenuScreen.Visible = true;
    }

    public void LevelButtonClick(int num)
    {
        Global.Instance.currentLevelIndex = num;
        GetTree().ChangeSceneToFile(Global.Instance.GetScenePath());
    }

    public void OnHoverDisplayLevelStats(int i)
    {
        /*int seconds;
        int minutes;*/
        if(Global.Instance.levelTimeRecords[i] == null)
            timeLabel.Text = "Time: --:--";
        else
            timeLabel.Text = Global.Instance.levelTimeRecords[i].ToString();
        if(Global.Instance.levelDeathRecords[i] == null)
            deathLabel.Text = "Deaths: ---";
        else
            deathLabel.Text = Global.Instance.levelDeathRecords[i].ToString();
        statsContainer.Show();
    }

    public void HoverExitHideLevelStats()
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
        InitializeOptions();
        InitializeButtonLevels();
    }
    #endregion <--->
}