using Godot;
using System;

public partial class buttonSound : Button
{
	public void OnClick()
	{
		Global.Instance.PlaySound(Global.sfx_button);
	}
}
