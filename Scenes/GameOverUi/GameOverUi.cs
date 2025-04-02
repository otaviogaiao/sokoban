using Godot;
using System;

public partial class GameOverUi : Control
{
	[Export] private Label _recordLabel;
	[Export] private Label _movesLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnLevelCompleted += OnLevelCompleted;
		SignalManager.Instance.OnNewGame += OnNewGame;
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnLevelCompleted -= OnLevelCompleted;
		SignalManager.Instance.OnNewGame -= OnNewGame;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnLevelCompleted(string levelNumber, int moves, bool isBest)
	{
		Show();
		_movesLabel.Text = $"{moves} Moves Taken";
		//record label
		if (isBest)
		{
			_recordLabel.Show();
		}
	}

	private void OnNewGame(string level)
	{
		Hide();
		_recordLabel.Hide();
	}
}
