using Godot;
using System;

public partial class Hud : Control
{
	[Export] private Label _levelLabel;
	[Export] private Label _movesLabel;
	[Export] private Label _bestLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnNewGame += OnNewGame;
		SignalManager.Instance.OnMoveMade += OnMoveMade;
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnNewGame -= OnNewGame;
		SignalManager.Instance.OnMoveMade -= OnMoveMade;
	}

	private void OnNewGame(string level)
	{
		_levelLabel.Text = level;
		_bestLabel.Text = ScoreSync.GetLevelBestScore(level).ToString();
		OnMoveMade(0);
		Show();
	}

	private void OnMoveMade(int moves)
	{
		_movesLabel.Text = $"{moves}".PadLeft(2, '0');
	}
}
