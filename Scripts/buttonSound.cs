using Godot;
using System;

public partial class ButtonSound : Button
{
	public void OnClick()
	{
		Global.Instance.PlaySound(Global.sfx_button);
	}
}
