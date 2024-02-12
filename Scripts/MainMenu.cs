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
    private HSlider sfxSlider;
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

    #endregion SIGNALS

    #region GODOT FUNCTIONS
    #endregion GODOT FUNCTIONS
}