using System;
using UnityEngine;

public static class LevelManager
{
	private const string playerPrefsClearedLevelprefix = "has_cleared_";
	private const string playerPrefsNextLevelName = "nextLevelName";

	public static readonly string MenuLevelName = "Menu";
	public static readonly string LevelsLevelName = "Levels";

	public static readonly string[] OrderedLevelNames = new[]
	{
		"Level 1",
		"Level 2",
		"Level 3",
		"Level 4",
		"Level 5",
		"Level 6",
		"Level 7",
		"Level 8",
		"Level 9",
		"Level 10",
		"Level 11"
	};

	public static string NextLevelName()
	{
		string next = PlayerPrefs.GetString(playerPrefsNextLevelName);
		return !String.IsNullOrEmpty(next) ? next : OrderedLevelNames[0];
	}
	public static string NextLevelName(string currentLevelName)
	{
		int index = Array.IndexOf(OrderedLevelNames, currentLevelName);

		if (index == -1 || index + 1 == OrderedLevelNames.Length)
		{
			return null;
		}
		else
		{
			return OrderedLevelNames[index + 1];
		}
	}

	public static void SetClearedLevel(string levelName)
	{
		if (!HasClearedLevel(levelName))
		{
			PlayerPrefs.SetInt(playerPrefsClearedLevelprefix + levelName, 1);
			PlayerPrefs.SetString(playerPrefsNextLevelName, NextLevelName(levelName));
		}
	}
	public static bool HasClearedLevel(string levelName)
	{
		return PlayerPrefs.GetInt(playerPrefsClearedLevelprefix + levelName, 0) == 1;
	}
}