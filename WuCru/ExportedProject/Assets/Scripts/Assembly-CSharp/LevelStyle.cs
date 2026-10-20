using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStyle : ScriptableObject
{
	[Serializable]
	public class SeededPrefabs
	{
		public int seed;

		public GameObject[] prefabList;

		public string[] resourceLoadedPrefabs;

		public bool hideBaseMesh;
	}

	[Serializable]
	public class EnemyTypeDistribution
	{
		public int percentage;

		public MinifigType type;
	}

	public enum e_LevelGroundType
	{
		None = 0,
		Abys = 1,
		Ground = 2,
		Water = 3,
		Shore = 4,
		TileTerrain = 5
	}

	public string styleName;

	public Sprite previewImage;

	public Color ambientLight = new Color(0.38f, 0.38f, 0.38f);

	public float uvMapSize = 2f;

	public float floorVariance = 0.03f;

	public float sideVariance = 0.04f;

	public string AmbienceAudioEventName;

	public string MusicAudioEventName;

	public string AudioHierarchyPrefix = "Audio_Music_Forest_";

	public string ExplorationMusicComponentName;

	public string AlertedMusicComponentName;

	public string CombatMusicComponentName;

	public List<Color> floorColors = new List<Color>();

	public Color sideColor = Color.white;

	public Color houseFloorColor = Color.white;

	public Color groundColor = new Color(0.51f, 0.36f, 0.17f);

	public Gradient heightGradient;

	public List<SeededPrefabs> seededPrefabs = new List<SeededPrefabs>();

	public List<GameObject> environmentSettings = new List<GameObject>();

	public GameObject hidingSpotPrefab;

	public GameObject woodChestPrefab;

	public GameObject metalChestPrefab;

	public GameObject goldChestPrefab;

	public GameObject lightPrefab;

	public GameObject lightPrefabDay;

	public GameObject distractionPrefab;

	public List<GameObject> treePrefabs = new List<GameObject>();

	public GameObject rectangleRoomHidingSpotPrefab;

	public GameObject trapPrefab;

	public GameObject keyPrefab;

	public GameObject doorTriggerPrefab;

	public GameObject campfirePrefab;

	public GameObject ladder1Prefab;

	public GameObject ladder2Prefab;

	public GameObject ladder3Prefab;

	public GameObject ladder4Prefab;

	public GameObject wallPrefab;

	public GameObject wallWithHidingSpotPrefab;

	public GameObject enemyDoorPrefab;

	public GameObject keyDoorPrefab;

	public GameObject triggeredDoorPrefab;

	public GameObject altWallPrefab;

	public GameObject altKeyDoorPrefab;

	public GameObject crystalShardPrefab;

	public GameObject endLevelObjectPrefab;

	public List<GameObject> decorationInserts = new List<GameObject>();

	public List<GameObject> decorationInsertsFree = new List<GameObject>();

	public List<GameObject> decorationForeground = new List<GameObject>();

	public List<GameObject> backDecorationInsertsFree = new List<GameObject>();

	public List<GameObject> backDecorationInsertsHuge = new List<GameObject>();

	public List<GameObject> decorationAddOns = new List<GameObject>();

	public List<GameObject> decorationAddOnsWallColor = new List<GameObject>();

	public List<GameObject> decorationAddOnsColored = new List<GameObject>();

	public List<GameObject> decorationHangouts = new List<GameObject>();

	public Texture2D pathEndpointTexture;

	public int minElevation = 1;

	public int maxElevation = 5;

	public int minElevationHills = 5;

	public int maxElevationHills = 10;

	public int minElevationPlateauRows = 3;

	public int maxElevationPlateauRows = 6;

	public int minElevationPlateuaCols = 4;

	public int maxElevationPlateuaCols = 8;

	public int numDecoInsertHugeItems = 20;

	public int minTrees = 2;

	public int maxTrees = 6;

	public int treeClusterSize = 3;

	public float treePlacementTopFactor = 1f;

	public float floorHeightNoiseFactor = 0.25f;

	public float propScaleVariactionFactor;

	public float decoFreeOffset = 0.9f;

	public float decoMediumOffset = 0.1f;

	public float decoRestrictedOffset = 0.05f;

	public float decoOutsideScale = 1.25f;

	public float decoBackFreeScaleVariation = 0.8f;

	public float decoBackMediumScaleVariation = 0.1f;

	public float decoBackRestrictedScaleVariation = 0.05f;

	public float decoOtherFreeScaleVariation = 0.25f;

	public float decoOtherMediumScaleVariation = 0.1f;

	public float decoOtherRestrictedScaleVariation = 0.05f;

	public float decoBackFreeProbability = 0.7f;

	public float decoOtherFreeProbability = 0.5f;

	public float decoBackProbability = 0.2f;

	public float decoOtherProbability = 0.3f;

	public float decoAddOnProbability = 0.3f;

	public float decoAddOnDistToPathFactor = 0.25f;

	public float decoAddOnScaleVariation = 0.2f;

	public float decoHangoutProbabilityFront = 0.5f;

	public float decoHangoutProbabilitySide = 0.33f;

	public float decoHangoutProbabilityBack = 0.2f;

	public float decoHugeMinDistToLevelMesh = 7f;

	public float decoHugeMaxDistToLevelmesh = 20f;

	public float decoForegroundProbability = 0.1f;

	public float decoForegroundMinDistToLevelMesh = 6f;

	public float decoForegroundMaxDistToLevelmesh = 8f;

	public AnimationCurve interactableFlash;

	public AnimationCurve interactableLoop;

	public EnemyTypeDistribution[] enemyTypeDistributions;

	public e_LevelGroundType LevelGroundType = e_LevelGroundType.Abys;

	public Texture2D WaterTexture;

	public Texture2D WaveTexture;

	[Range(0f, 2f)]
	public float WaveSpeed = 0.1f;

	[Range(0f, 2f)]
	public float WaveHeight = 0.5f;

	public Color ShorelineCol = Color.white;

	[Range(0.01f, 3f)]
	public float ShorelineWidth = 1f;

	public List<GameObject> groundTiles = new List<GameObject>();

	public int totalMinifigTypePercentage
	{
		get
		{
			int num = 0;
			for (int i = 0; i < enemyTypeDistributions.Length; i++)
			{
				num += enemyTypeDistributions[i].percentage;
			}
			return num;
		}
	}

	public MinifigType MinifigTypeForRandomValue(int value)
	{
		int num = 0;
		for (int i = 0; i < enemyTypeDistributions.Length; i++)
		{
			num += enemyTypeDistributions[i].percentage;
			if (value <= num)
			{
				return enemyTypeDistributions[i].type;
			}
		}
		Debug.LogError("Unable to find randomized enemy type for value " + value);
		return MinifigType.None;
	}
}
