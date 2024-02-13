using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    #region VARIABLES
    [Export]
    private Control mainMenuScreen;
    [Export]
    private Control levelSelectScreen;
    [Export]
    private Control optionsScreen;
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
    #endregion VARIABLES

    #region FUNCTIONS
    #endregion FUNCTIONS

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
    //OPTIONS MENU
    public void OptionsBackButtonClick()
    {
        optionsScreen.Visible = false;
        mainMenuScreen.Visible = true;
    }

    public void MusicSliderValueChange(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(value));
    }

    public void SFXSliderValueChange(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb(value));
    }
    #endregion SIGNALS

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        musicSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music")));
        musicSliderLabel.Text = (musicSlider.Value * 100).ToString("0");
        sfxSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("SFX")));
        sfxSliderLabel.Text = (sfxSlider.Value * 100).ToString("0");
    }
    #endregion GODOT FUNCTIONS
}