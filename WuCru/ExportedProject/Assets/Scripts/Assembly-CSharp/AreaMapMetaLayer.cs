using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaMapMetaLayer : ScriptableObject
{
	[Serializable]
	public class ColArray
	{
		[SerializeField]
		public AreaMapNode[] colArray;
	}

	[Serializable]
	public class EmitterArray
	{
		public int emitterRow;

		public int emitterCol;

		public float emitterLocalOffsetX;

		public float emitterLocalOffsetY;

		public GameObject emitter;
	}

	[SerializeField]
	public string areaName;

	[SerializeField]
	public string displayName;

	[SerializeField]
	public string mapImagePath;

	[SerializeField]
	public string addonImagePath;

	[SerializeField]
	public bool showAddon;

	[SerializeField]
	public int addonStartRow;

	[SerializeField]
	public int addonStartCol;

	[SerializeField]
	public int addonEndRow;

	[SerializeField]
	public int addonEndCol;

	[SerializeField]
	public int addonUnlockChapter;

	[SerializeField]
	public int addonUnlockSeason;

	[SerializeField]
	public Sprite previewImage;

	[SerializeField]
	public Vector2 worldMapButtonPosition;

	[SerializeField]
	public ColArray[] rowArray;

	[SerializeField]
	public int baseSeed = 1000;

	[SerializeField]
	public int season = 1;

	[SerializeField]
	public int startRow;

	[SerializeField]
	public int startCol;

	[SerializeField]
	public EmitterArray[] emitterArray;

	public int currentRow;

	public int currentCol;

	public bool unlockAllLevelsCheat;

	private List<int> newlyUnlockedLevels = new List<int>();

	public List<int> NewlyUnlockedLevels
	{
		get
		{
			return newlyUnlockedLevels;
		}
	}

	public void GenerateDefaultData(int rows, int cols, int defaultStyleIndex, int defaultThemeCollectionIndex)
	{
		rowArray = new ColArray[rows];
		for (int i = 0; i < rows; i++)
		{
			rowArray[i] = new ColArray();
			rowArray[i].colArray = new AreaMapNode[cols];
			for (int j = 0; j < cols; j++)
			{
				rowArray[i].colArray[j] = new AreaMapNode();
				rowArray[i].colArray[j].row = i;
				rowArray[i].colArray[j].col = j;
				rowArray[i].colArray[j].styleIndex = defaultStyleIndex;
				rowArray[i].colArray[j].themeCollectionIndex = defaultThemeCollectionIndex;
				rowArray[i].colArray[j].primaryPower = GetDefaultPrimaryPowerFromPosition(i, j);
				rowArray[i].colArray[j].secondaryPower = GetDefaultSecondaryPowerFromPosition(i, j);
			}
		}
		rowArray[rows / 2].colArray[cols / 2].isUnlocked = true;
	}

	public void GenerateDefaultPowerData(int rows, int cols)
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				rowArray[i].colArray[j].primaryPower = GetDefaultPrimaryPowerFromPosition(i, j);
				rowArray[i].colArray[j].secondaryPower = GetDefaultSecondaryPowerFromPosition(i, j);
			}
		}
	}

	public AreaMapNode GetNodeAt(int row, int col)
	{
		return rowArray[row].colArray[col];
	}

	public int GetCustomHashCode(int row, int col)
	{
		return row + rowArray.Length * col;
	}

	public int GetSeedFromPosition(int row, int col)
	{
		return baseSeed + row + rowArray.Length * col;
	}

	public ElementalPower GetPrimaryPowerForPosition(int row, int col)
	{
		return rowArray[row].colArray[col].primaryPower;
	}

	public ElementalPower GetSecondaryPowerForPosition(int row, int col)
	{
		return rowArray[row].colArray[col].secondaryPower;
	}

	public Sprite GetPreviewImageForPosition(int row, int col)
	{
		return GameController.Instance.styleDB._styles[rowArray[row].colArray[col].styleIndex].previewImage;
	}

	private ElementalPower GetDefaultPrimaryPowerFromPosition(int row, int col)
	{
		UnityEngine.Random.seed = GetSeedFromPosition(row, col);
		float value = UnityEngine.Random.value;
		if (value <= 0.16f)
		{
			return ElementalPower.Earth;
		}
		if (value < 0.33f)
		{
			return ElementalPower.Fire;
		}
		if (value <= 0.49f)
		{
			return ElementalPower.Ice;
		}
		if (value < 0.66f)
		{
			return ElementalPower.Electricity;
		}
		if (value <= 0.83f)
		{
			return ElementalPower.Water;
		}
		return ElementalPower.Energy;
	}

	private ElementalPower GetDefaultSecondaryPowerFromPosition(int row, int col)
	{
		UnityEngine.Random.seed = GetSeedFromPosition(row, col) + 100;
		float value = UnityEngine.Random.value;
		if (value <= 0.16f)
		{
			return ElementalPower.Earth;
		}
		if (value < 0.33f)
		{
			return ElementalPower.Fire;
		}
		if (value <= 0.49f)
		{
			return ElementalPower.Ice;
		}
		if (value < 0.66f)
		{
			return ElementalPower.Electricity;
		}
		if (value <= 0.83f)
		{
			return ElementalPower.Water;
		}
		return ElementalPower.Energy;
	}

	public bool IsPlayed(int seed)
	{
		if (GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(seed))
		{
			LevelUnlockData levelUnlockData = GameController.Instance.PlayerProfile.LevelUnlocked[seed];
			return levelUnlockData.starCompletion;
		}
		return false;
	}

	public int GetNumStars(int seed)
	{
		if (!IsPlayed(seed))
		{
			return 0;
		}
		int num = 0;
		if (GameController.Instance.PlayerProfile.LevelUnlocked[seed].starCompletion)
		{
			num++;
		}
		if (GameController.Instance.PlayerProfile.LevelUnlocked[seed].starObjects)
		{
			num++;
		}
		if (GameController.Instance.PlayerProfile.LevelUnlocked[seed].starChallenges)
		{
			num++;
		}
		return num;
	}

	public bool IsUnlocked(int seed)
	{
		return GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(seed);
	}

	public bool IsBlocked(int r, int c)
	{
		return rowArray[r].colArray[c].isBlocked;
	}

	public bool IsLockedUntilChapter(int r, int c)
	{
		AreaMapNode areaMapNode = rowArray[r].colArray[c];
		if (areaMapNode.lockUntilChapter > GameController.Instance.PlayerProfile.GetChapterCursor(season))
		{
			return true;
		}
		return false;
	}

	public bool IsDiscoverable(int r, int c)
	{
		if (!IsLockedUntilChapter(r, c))
		{
			return false;
		}
		AreaMapNode areaMapNode = rowArray[r].colArray[c];
		int seedFromPosition = GetSeedFromPosition(r, c);
		ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (objectiveLine.objectiveType == ObjectiveType.Discover && objectiveLine.seed == seedFromPosition && !objectiveLine.Completed && objectiveLine.season == season && !GameController.Instance.PlayerProfile.IsLevelDiscovered(seedFromPosition))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsLocked(int r, int c)
	{
		if (!IsLockedUntilChapter(r, c))
		{
			return false;
		}
		AreaMapNode areaMapNode = rowArray[r].colArray[c];
		int seedFromPosition = GetSeedFromPosition(r, c);
		ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (GameController.Instance.PlayerProfile.IsLevelDiscovered(seedFromPosition))
			{
				return true;
			}
		}
		return false;
	}

	public int NumLevelsPlayed()
	{
		int num = 0;
		for (int i = 0; i < rowArray.Length; i++)
		{
			for (int j = 0; j < rowArray[0].colArray.Length; j++)
			{
				if (IsPlayed(GetSeedFromPosition(i, j)))
				{
					num++;
				}
			}
		}
		return num;
	}

	public int NumPlayableLevels()
	{
		int num = 0;
		for (int i = 0; i < rowArray.Length; i++)
		{
			for (int j = 0; j < rowArray[0].colArray.Length; j++)
			{
				if (!IsBlocked(i, j))
				{
					num++;
				}
			}
		}
		return num;
	}

	public bool IsFirstTimeInArea()
	{
		bool flag = false;
		for (int i = 0; i < rowArray.Length; i++)
		{
			for (int j = 0; j < rowArray[0].colArray.Length; j++)
			{
				int seedFromPosition = GetSeedFromPosition(i, j);
				if (GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(seedFromPosition))
				{
					flag = true;
					break;
				}
			}
		}
		return !flag;
	}

	public void UnlockAfterPlay(LevelUnlockData unlockData)
	{
		if (GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(unlockData.seed))
		{
			LevelUnlockData levelUnlockData = GameController.Instance.PlayerProfile.LevelUnlocked[GameController.Instance.CurrentLevel.Seed];
			levelUnlockData.starCompletion = unlockData.starCompletion;
			levelUnlockData.starObjects = unlockData.starObjects;
			levelUnlockData.starChallenges = unlockData.starChallenges;
		}
		else
		{
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(unlockData.seed, unlockData);
			Debug.Log("<color=yellow>Level Unlock" + unlockData.seed + "</color>");
			newlyUnlockedLevels.Add(unlockData.seed);
		}
		int num = GameController.Instance.areaMapMetaLayer.currentRow;
		int num2 = GameController.Instance.areaMapMetaLayer.currentCol;
		if (num < rowArray.Length - 1 && !IsBlocked(num + 1, num2) && !IsPlayed(GetSeedFromPosition(num + 1, num2)) && !IsUnlocked(GetSeedFromPosition(num + 1, num2)))
		{
			LevelUnlockData levelUnlockData2 = new LevelUnlockData();
			levelUnlockData2.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
			levelUnlockData2.seed = GetSeedFromPosition(num + 1, num2);
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData2.seed, levelUnlockData2);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData2.seed + "</color>");
			newlyUnlockedLevels.Add(levelUnlockData2.seed);
		}
		if (num > 0 && !IsBlocked(num - 1, num2) && !IsPlayed(GetSeedFromPosition(num - 1, num2)) && !IsUnlocked(GetSeedFromPosition(num - 1, num2)))
		{
			LevelUnlockData levelUnlockData3 = new LevelUnlockData();
			levelUnlockData3.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
			levelUnlockData3.seed = GetSeedFromPosition(num - 1, num2);
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData3.seed, levelUnlockData3);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData3.seed + "</color>");
			newlyUnlockedLevels.Add(levelUnlockData3.seed);
		}
		if (num2 < rowArray[0].colArray.Length - 1 && !IsBlocked(num, num2 + 1) && !IsPlayed(GetSeedFromPosition(num, num2 + 1)) && !IsUnlocked(GetSeedFromPosition(num, num2 + 1)))
		{
			LevelUnlockData levelUnlockData4 = new LevelUnlockData();
			levelUnlockData4.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
			levelUnlockData4.seed = GetSeedFromPosition(num, num2 + 1);
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData4.seed, levelUnlockData4);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData4.seed + "</color>");
			newlyUnlockedLevels.Add(levelUnlockData4.seed);
		}
		if (num2 > 0 && !IsBlocked(num, num2 - 1) && !IsPlayed(GetSeedFromPosition(num, num2 - 1)) && !IsUnlocked(GetSeedFromPosition(num, num2 - 1)))
		{
			LevelUnlockData levelUnlockData5 = new LevelUnlockData();
			levelUnlockData5.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
			levelUnlockData5.seed = GetSeedFromPosition(num, num2 - 1);
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData5.seed, levelUnlockData5);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData5.seed + "</color>");
			newlyUnlockedLevels.Add(levelUnlockData5.seed);
		}
		GameController.Instance.PlayerProfile.Save();
	}
}
