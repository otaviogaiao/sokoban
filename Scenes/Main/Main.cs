using Godot;
using System;

public partial class Main : Control
{
	[Export] private GridContainer _levelButtonsGrid;
	private PackedScene _levelButtonsScene = GD.Load<PackedScene>("res://Scenes/LevelButton/LevelButton.tscn");
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var levelCount = GameData.LevelCount;

		for (int i = 0; i < levelCount; i++)
		{
			LevelButton levelButton = _levelButtonsScene.Instantiate<LevelButton>();
			levelButton.LevelNumber = i + 1;
 			
			_levelButtonsGrid.AddChild(levelButton);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
