using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelExtension : ScriptableObject
{
	public string extensionName;

	public bool showExtensionIconOnMap;

	public GameObject extensionMapIconPrefab;

	public bool replaceEnemies;

	public LevelNodeObjectType replaceEnemiesNodeType;

	public AIAgentType replaceEnemiesType;

	public int replaceCountMin;

	public int replaceCountMax;

	public bool enemyShouldDrop;

	public GameObject enemyDropItem;

	public GameObject replaceLampPrefab;

	public string confirmPlaySoundName;

	public List<GameObject> replaceDecorationForeground = new List<GameObject>();

	public Sprite previewImage;

	public string AmbienceAudioEventName;

	public string MusicAudioEventName;

	public string AudioHierarchyPrefix;

	public string ExplorationMusicComponentName;

	public string AlertedMusicComponentName;

	public string CombatMusicComponentName;

	public GameObject lightPrefab;

	public GameObject lightPrefabDay;

	public List<GameObject> environmentSettings = new List<GameObject>();

	public List<Color> floorColors = new List<Color>();

	public Color groundColor;

	public List<GameObject> decorationForeground = new List<GameObject>();

	public void OverwriteStyleValuesOn(LevelStyle style)
	{
		if (previewImage != null)
		{
			style.previewImage = previewImage;
		}
		if (AmbienceAudioEventName != null)
		{
			style.AmbienceAudioEventName = AmbienceAudioEventName;
		}
		if (MusicAudioEventName != null)
		{
			style.MusicAudioEventName = MusicAudioEventName;
		}
		if (AudioHierarchyPrefix != null)
		{
			style.AudioHierarchyPrefix = AudioHierarchyPrefix;
		}
		if (ExplorationMusicComponentName != null)
		{
			style.ExplorationMusicComponentName = ExplorationMusicComponentName;
		}
		if (AlertedMusicComponentName != null)
		{
			style.AlertedMusicComponentName = AlertedMusicComponentName;
		}
		if (CombatMusicComponentName != null)
		{
			style.CombatMusicComponentName = CombatMusicComponentName;
		}
		if (lightPrefab != null)
		{
			style.lightPrefab = lightPrefab;
		}
		if (lightPrefabDay != null)
		{
			style.lightPrefabDay = lightPrefabDay;
		}
		if (environmentSettings != null)
		{
			style.environmentSettings = environmentSettings;
		}
		if (floorColors != null)
		{
			style.floorColors = floorColors;
		}
		style.groundColor = groundColor;
		if (decorationForeground != null)
		{
			style.decorationForeground = decorationForeground;
		}
	}

	public void DoLevelAlternations(Level level)
	{
		if (replaceEnemies)
		{
			UnityEngine.Random.seed = level.Seed;
			level.ReplaceAgents(UnityEngine.Random.Range(replaceCountMin, replaceCountMax + 1), replaceEnemiesNodeType);
		}
	}

	public GameObject GetEnemyDropItem(AIAgentType agentType)
	{
		if (enemyShouldDrop && agentType == replaceEnemiesType)
		{
			return enemyDropItem;
		}
		return null;
	}
}
