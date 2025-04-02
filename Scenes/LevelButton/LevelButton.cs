using Godot;
using System;

public partial class LevelButton : NinePatchRect
{
	[Export] private TextureRect _checkMark;
	[Export] private Label _levelLabel;
	
	public int LevelNumber { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelLabel.Text = LevelNumber.ToString();
		GuiInput += OnGuiInput;
		if (ScoreSync.HasLevelScore(LevelNumber.ToString()))
		{
			_checkMark.Show();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnGuiInput(InputEvent e)
	{
		if (e.IsActionPressed("select"))
		{
			SignalManager.EmitOnLevelSelected(LevelNumber.ToString());
		}
	}
}
