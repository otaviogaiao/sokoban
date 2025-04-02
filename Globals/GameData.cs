using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class GameData : Node
{
	private const string LevelDataPath = "res://Data/level_data.json";

	private Dictionary<string, LevelLayout> _levelLayouts = new();
	
	public static int LevelCount
	{
		get { return Instance._levelLayouts.Count; }
	}

	public static GameData Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		LoadlevelData();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LoadlevelData()
	{
		using var file = FileAccess.Open(LevelDataPath, FileAccess.ModeFlags.Read);
		if (file != null)
		{
			var jsonData = file.GetAsText();
			if (!string.IsNullOrEmpty(jsonData))
			{
				_levelLayouts = JsonConvert.DeserializeObject<Dictionary<string, LevelLayout>>(jsonData);
			}
		}
	}

	public static LevelLayout GetLevelLayout(string levelName)
	{
		if (Instance._levelLayouts.ContainsKey(levelName))
		{
			return Instance._levelLayouts[levelName];
		}

		return null;
	}
}
