using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaMapDB : ScriptableObject
{
	[SerializeField]
	public DictionaryStringAreaMapMetaLayer areaMaps = new DictionaryStringAreaMapMetaLayer();

	public string[] GetFirstLevelOfAllSeasons()
	{
		List<string> list = new List<string>();
		int num = 1;
		foreach (AreaMapMetaLayer value in areaMaps.values)
		{
			if (value.season == num)
			{
				list.Add(value.areaName);
				num++;
			}
		}
		return list.ToArray();
	}

	public bool IsMapAvalible(string mapID)
	{
		if (!areaMaps.ContainsKey(mapID))
		{
			return false;
		}
		AreaMapMetaLayer areaMapMetaLayer = areaMaps[mapID];
		int seedFromPosition = areaMapMetaLayer.GetSeedFromPosition(areaMapMetaLayer.startRow, areaMapMetaLayer.startCol);
		Debug.Log("Checking Seed: " + seedFromPosition);
		return areaMapMetaLayer.IsUnlocked(seedFromPosition);
	}

	public bool HasMapBeenPlayed(string mapID)
	{
		if (!areaMaps.ContainsKey(mapID))
		{
			return false;
		}
		AreaMapMetaLayer areaMapMetaLayer = areaMaps[mapID];
		int seedFromPosition = areaMapMetaLayer.GetSeedFromPosition(areaMapMetaLayer.startRow, areaMapMetaLayer.startCol);
		return areaMapMetaLayer.IsPlayed(seedFromPosition);
	}
}
