using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Level : Node2D
{
	[Export] private TileMapLayer _floorTiles;
	[Export] private TileMapLayer _wallTiles;
	[Export] private TileMapLayer _targetTiles;
	[Export] private TileMapLayer _boxTiles;
	
	[Export] private Node2D _tilesHolder;
	[Export] private AnimatedSprite2D _player;
	[Export] private Camera2D _camera2D;

	private const int SourceId = 0;

	private Dictionary<TileLayerNames, TileMapLayer> _layerMap;
	private int _tileSize = 32;
	private Vector2I _playerTile;
	private int _totalMoves = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetUpLayerMap();
		CallDeferred(MethodName.SetupLevel);
		_tileSize = _floorTiles.TileSet.TileSize.X;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("exit"))
		{
			GameManager.LoadMainScene();
		}

		if (Input.IsActionJustPressed("reload"))
		{
			SetupLevel();
		}

		var moveDirection = GetInputDirection();
		if (moveDirection != Vector2.Zero)
		{
			PlayerMove(moveDirection);
		}
	}

	private Vector2I GetInputDirection()
	{
		if(Input.IsActionJustPressed("right")) return Vector2I.Right;
		if (Input.IsActionJustPressed("left")) return Vector2I.Left;
		if(Input.IsActionJustPressed("up")) return Vector2I.Up;
		if(Input.IsActionJustPressed("down")) return Vector2I.Down;
		return Vector2I.Zero;
	}

	private bool CellIsWall(Vector2I cell)
	{
		return _wallTiles.GetUsedCells().Contains(cell);
	}

	private bool CellIsBox(Vector2I cell)
	{
		return _boxTiles.GetUsedCells().Contains(cell);
	}

	private bool CellIsEmpty(Vector2I cell)
	{
		return !CellIsWall(cell) && !CellIsBox(cell);
	}

	private bool BoxCanMove(Vector2I boxTile, Vector2I direction)
	{
		return CellIsEmpty(boxTile + direction);
	}

	private void MoveBox(Vector2I boxTile, Vector2I direction)
	{
		Vector2I newTile = boxTile + direction;
		
		_boxTiles.EraseCell(boxTile);

		var tln = TileLayerNames.Box;

		if (_targetTiles.GetUsedCells().Contains(newTile))
		{
			tln = TileLayerNames.TargetBox;
		}
		_boxTiles.SetCell(newTile, SourceId, GetAtlasCoordForLayerName(tln));
	}

	private void CheckGameState()
	{
		foreach (var t in _targetTiles.GetUsedCells())
		{
			if (!CellIsBox(t)) return;
		}
		
		GD.Print($"Completed {_totalMoves} moves");
		ScoreSync.LevelCompleted(GameManager.SelectedLevel, _totalMoves);
	}

	private void PlayerMove(Vector2I moveDirection)
	{
		Vector2I newTile = _playerTile + moveDirection;

		if (CellIsWall(newTile)) return;
		if (CellIsBox(newTile) && !BoxCanMove(newTile, moveDirection)) return;

		if (moveDirection == Vector2.Left)
		{
			_player.FlipH = true;
		} else if (moveDirection == Vector2.Right)
		{
			_player.FlipH = false;
		}

		if (CellIsBox(newTile))
		{
			MoveBox(newTile, moveDirection);
		}

		_totalMoves++;
		SignalManager.EmitOnMoveMade(_totalMoves);
		PlacePlayerOnTile(newTile);
		CheckGameState();
	}

	private void PlacePlayerOnTile(Vector2I tileCoord)
	{
		Vector2 newPosition = _tilesHolder.Position + tileCoord * _tileSize;
		_player.Position = newPosition;
		_playerTile = tileCoord;
	}

	private void SetUpLayerMap()
	{
		_layerMap = new Dictionary<TileLayerNames, TileMapLayer>()
		{
			{ TileLayerNames.Floor, _floorTiles },
			{ TileLayerNames.Wall, _wallTiles },
			{ TileLayerNames.Target, _targetTiles },
			{ TileLayerNames.Box, _boxTiles },
			{ TileLayerNames.TargetBox, _boxTiles }
		};
	}

	private Vector2I GetAtlasCoordForLayerName(TileLayerNames layerName)
	{
		switch (layerName)
		{
			case TileLayerNames.TargetBox:
				return new Vector2I(0, 0);
			case TileLayerNames.Box:
				return new Vector2I(1, 0);
			case TileLayerNames.Wall:
				return new Vector2I(2, 0);
			case TileLayerNames.Floor:
				return new Vector2I(GD.RandRange(3, 9), 0);
			case TileLayerNames.Target:
				return new Vector2I(9, 0);
			default:
				return Vector2I.Zero;
		}
	}

	private void ClearTiles()
	{
		foreach (var layer in _layerMap.Values)
		{
			layer.Clear();
		}
	}

	private void AddTileToLayer(TileLayerNames layerName, Vector2I tileCoord)
	{
		TileMapLayer tileMapLayer = _layerMap[layerName];
		tileMapLayer.SetCell(tileCoord, SourceId, GetAtlasCoordForLayerName(layerName));
	}

	private void SetupLayer(TileLayerNames layerName, LevelLayout levelLayout)
	{
		var tiles = levelLayout.GetTilesForLayer(layerName);
		foreach (var tileCoord in tiles)
		{
			AddTileToLayer(layerName, tileCoord.ToVector2I());
		}
	}

	private void SetupLevel()
	{
		string ln = GameManager.SelectedLevel;
		_totalMoves = 0;
		LevelLayout levelLayout = GameData.GetLevelLayout(ln);
		
		ClearTiles();

		foreach (var layerName in _layerMap.Keys)
		{
			SetupLayer(layerName, levelLayout);
		}

		PlacePlayerOnTile(levelLayout.PlayerStart.ToVector2I());
		MoveCamera();
		SignalManager.EmitOnNewGame(ln);
	}

	private async void MoveCamera()
	{
		var usedRect = _floorTiles.GetUsedRect();
		Vector2 center = (usedRect.Position + usedRect.Size / 2) * _tileSize;
		_camera2D.Position = center;
		await Task.Delay(50);
		Reveal();
	}

	private void Reveal()
	{
		_tilesHolder.Show();
		_player.Show();
	}
}
