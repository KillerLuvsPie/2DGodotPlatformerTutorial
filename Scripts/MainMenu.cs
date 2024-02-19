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
    
    #endregion VARIABLES

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

        //CONTROLS
        foreach (Button button in GetTree().GetNodesInGroup("ControlButtons"))
        {
            button.Text = InputMap.ActionGetEvents(button.Text)[0].AsText().Replace(" (Physical)", "");
        }
    }
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

    public void OptionsControlsButtonClick()
    {
        optionsScreen.Visible = false;
        controlsScreen.Visible = true;
    }

    private void CenterWindow()
    {
        Vector2I screenCenter = DisplayServer.ScreenGetPosition() + DisplayServer.ScreenGetSize() / 2;
        Vector2I windowSize = GetWindow().GetSizeWithDecorations();
        GetWindow().SetImePosition(screenCenter - windowSize /2);
    }

    public void WindowModeSelected(int id)
    {
        switch (id)
        {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                CenterWindow();
                break;
            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 2:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                CenterWindow();
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
        CenterWindow();
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
    public void InitializeControlButtons(string action)
    {

    }

    public void ControlsBackButtonClick()
    {
        controlsScreen.Visible = false;
        optionsScreen.Visible = true;
    }
    #endregion SIGNALS

    #region GODOT FUNCTIONS
    public override void _Ready()
    {
        InitializeOptions();
    }
    #endregion GODOT FUNCTIONS
}