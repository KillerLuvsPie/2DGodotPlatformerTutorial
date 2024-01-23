using Godot;
using System;
using System.Text.RegularExpressions;

public partial class GameManager : Node
{
	public static GameManager instance;
	private int fruitCounter;
	private int lives = 3;

	public void SetFruitCounter()
	{
		fruitCounter = GetTree().GetNodesInGroup("Fruit").Count;
		GD.Print(fruitCounter);
	}

	public void DecreaseFruitCounter()
	{
		fruitCounter--;
		GD.Print(fruitCounter);
		//CHANGE UI HERE
		CheckForWin();
	}

	public void CheckForWin()
	{
		if(fruitCounter == 0)
			//SHOW YOU WIN LABEL
			GD.Print("You Win!!!");
	}

	public void CheckForGameOver()
	{
		if(lives == 0)
			GD.Print("Game Over");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(instance != this && instance != null)
			QueueFree();
		else
			instance = this;
		SetFruitCounter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
