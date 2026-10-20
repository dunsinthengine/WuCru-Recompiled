using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelExtensionController : ScriptableObject
{
	[Serializable]
	public class MapSeedList
	{
		[SerializeField]
		public string mapname;

		[SerializeField]
		public int[] seeds;

		public int seedIndex { get; set; }

		public int currentSeed
		{
			get
			{
				return seeds[seedIndex];
			}
		}
	}

	[Serializable]
	public class LevelExtensionSettings
	{
		[SerializeField]
		public string name;

		[SerializeField]
		public MapSeedList[] mapSeedLists;

		[SerializeField]
		public int lockedUntilLevelsPlayed;

		[SerializeField]
		public int total;

		[SerializeField]
		public LevelExtension levelExtension;

		public float progress
		{
			get
			{
				return Mathf.Clamp01((float)collected / (float)total);
			}
		}

		public int collected { get; set; }

		public bool isActive
		{
			get
			{
				return GameController.Instance.PlayerProfile.TotalUniqueLevelsPlayed >= lockedUntilLevelsPlayed;
			}
		}

		public bool isComplete
		{
			get
			{
				Debug.Log(name + " is Complete? " + collected + " >= " + total);
				return collected >= total;
			}
		}
	}

	[SerializeField]
	private LevelExtensionSettings[] extensionSettings;

	public void CheatCompleteExtension(string name)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (extensionSettings[i].name == name)
			{
				extensionSettings[i].collected = extensionSettings[i].total;
			}
		}
	}

	public void HandleLevelComplete(int seed)
	{
		Debug.Log("HandleLevelComplete(" + seed + ")");
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				Debug.Log("mapseedList: " + mapSeedList.currentSeed + " / " + i + " / " + j);
				if (mapSeedList.currentSeed == seed)
				{
					Debug.Log("Saving?");
					mapSeedList.seedIndex = (mapSeedList.seedIndex + 1) % mapSeedList.seeds.Length;
					SaveToPlayerProfile();
				}
			}
		}
	}

	public bool GetLevelExtensionComplete(string name)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (extensionSettings[i].name == name)
			{
				return extensionSettings[i].isComplete;
			}
		}
		return false;
	}

	public LevelExtension GetLevelExtension(int seed)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (!extensionSettings[i].isActive)
			{
				continue;
			}
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed)
				{
					return extensionSettings[i].levelExtension;
				}
			}
		}
		return null;
	}

	public LevelExtensionSettings GetLevelExtensionSettings(string name)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (extensionSettings[i].name == name)
			{
				return extensionSettings[i];
			}
		}
		return null;
	}

	public GameObject GetAreaMapPointer(int seed)
	{
		if (GameController.Instance.PlayerProfile.NinjaTeam.Length > 1)
		{
			for (int i = 0; i < extensionSettings.Length; i++)
			{
				if (!extensionSettings[i].isActive)
				{
					continue;
				}
				for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
				{
					MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
					if (mapSeedList.seeds[0] == seed && j == 0)
					{
						return (!extensionSettings[i].levelExtension.showExtensionIconOnMap) ? null : extensionSettings[i].levelExtension.extensionMapIconPrefab;
					}
				}
			}
		}
		return null;
	}

	public string GetConfirmPlaySoundName(int seed)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (!extensionSettings[i].isActive)
			{
				continue;
			}
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed)
				{
					return extensionSettings[i].levelExtension.confirmPlaySoundName;
				}
			}
		}
		return null;
	}

	public Sprite GetLevelImageOverlay(int seed)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (!extensionSettings[i].isActive)
			{
				continue;
			}
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed)
				{
					return extensionSettings[i].levelExtension.previewImage;
				}
			}
		}
		return null;
	}

	public GameObject GetReplaceLampGameObject(int seed)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (!extensionSettings[i].isActive)
			{
				continue;
			}
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed)
				{
					return extensionSettings[i].levelExtension.replaceLampPrefab;
				}
			}
		}
		return null;
	}

	public List<GameObject> GetReplaceDecorationForegrounds(int seed)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (!extensionSettings[i].isActive)
			{
				continue;
			}
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed)
				{
					return extensionSettings[i].levelExtension.replaceDecorationForeground;
				}
			}
		}
		return new List<GameObject>();
	}

	public GameObject GetEnemyDropItem(int seed, AIAgentType agent)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (!extensionSettings[i].isActive)
			{
				continue;
			}
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed)
				{
					return extensionSettings[i].levelExtension.GetEnemyDropItem(agent);
				}
			}
		}
		return null;
	}

	public string[] GetAllLevelExtensionNames()
	{
		return extensionSettings.Select((LevelExtensionSettings e) => e.name).ToArray();
	}

	public void SetSeedIndexesToZero(string extName)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			if (extensionSettings[i].name == extName)
			{
				for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
				{
					MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
					mapSeedList.seedIndex = 0;
				}
			}
		}
	}

	public void HandleEnemyDropPickedUp(int seed)
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				if (mapSeedList.currentSeed == seed && extensionSettings[i].levelExtension.enemyShouldDrop)
				{
					extensionSettings[i].collected++;
					break;
				}
			}
		}
	}

	public void SaveToPlayerProfile()
	{
		Debug.Log("SaveToPlayerProfile()");
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			LevelExtensionData levelExtensionData = new LevelExtensionData();
			levelExtensionData.progress = extensionSettings[i].collected;
			Debug.Log("Progress[" + i + "] = " + levelExtensionData.progress);
			for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[j];
				levelExtensionData.mapSeedIndex[mapSeedList.mapname] = mapSeedList.seedIndex;
			}
			GameController.Instance.PlayerProfile.SetLevelExtensionData(extensionSettings[i].name, levelExtensionData);
		}
	}

	public void LoadFromPlayerProfile()
	{
		for (int i = 0; i < extensionSettings.Length; i++)
		{
			LevelExtensionData levelExtensionData = GameController.Instance.PlayerProfile.GetLevelExtensionData(extensionSettings[i].name);
			if (levelExtensionData == null)
			{
				extensionSettings[i].collected = 0;
				for (int j = 0; j < extensionSettings[i].mapSeedLists.Length; j++)
				{
					extensionSettings[i].mapSeedLists[j].seedIndex = 0;
				}
				continue;
			}
			extensionSettings[i].collected = levelExtensionData.progress;
			for (int k = 0; k < extensionSettings[i].mapSeedLists.Length; k++)
			{
				MapSeedList mapSeedList = extensionSettings[i].mapSeedLists[k];
				mapSeedList.seedIndex = levelExtensionData.mapSeedIndex[mapSeedList.mapname];
			}
		}
	}
}
