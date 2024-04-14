using Godot;
using System;

public partial class ControlButton : Button
{
	#region VARIABLES
	//IF THERE ARE MULTIPLE CONTROLS IN AN ACTION, SET actionIndex VALUE IN EDITOR
	[Export]
	private int actionIndex = 0;
	private string action;
	#endregion <--->

	#region FUNCTIONS
	private void SetControlButtonText()
    {
        Text = InputMap.ActionGetEvents(action)[actionIndex].AsText().Replace(" (Physical)", "");
    }
	#endregion <--->

	#region SIGNALS
	private void ToggleControlButton(bool ButtonPressed)
    {
        SetProcessUnhandledInput(ButtonPressed);
        if(ButtonPressed)
        {
            Text = "Press a key...";
            ReleaseFocus();
        }
        else
        {
            SetControlButtonText();
            GrabFocus();
			Owner.EmitSignal(MainMenu.SignalName.OptionChanged);
        }
    }

	public void OnClick()
	{
		Global.Instance.PlaySound(Global.sfx_button);
	}
	#endregion <--->

	#region GODOT FUNCTIONS
	public override void _Ready()
	{
		SetProcessUnhandledInput(false);
		action = Text;
		CallDeferred("SetControlButtonText");
		Toggled += ToggleControlButton;
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsPressed())
		{
			//SAVE ALL CURRENT INPUTS
			InputEvent[] savedInputEvents = new InputEvent[InputMap.ActionGetEvents(action).Count];
			for(int i = 0; i < InputMap.ActionGetEvents(action).Count; i++)
			{
				if(i != actionIndex)
					savedInputEvents[i] = InputMap.ActionGetEvents(action)[i];
			}
			//ADD NEW EVENT TO APPROPRIATE INDEX
			savedInputEvents[actionIndex] = @event;
			//DELETE ALL EVENTS
			InputMap.ActionEraseEvents(action);
			//REBUILD EVENTS WITH NEW ADDITION
			for(int i = 0; i < savedInputEvents.Length; i++)
				InputMap.ActionAddEvent(action, savedInputEvents[i]);
			ButtonPressed = false;
		}
    }
    #endregion <--->
}
