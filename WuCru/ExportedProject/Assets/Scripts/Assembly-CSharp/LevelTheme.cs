using System;
using UnityEngine;

[Serializable]
public class LevelTheme : ScriptableObject
{
	[Serializable]
	public class ForceSetting
	{
		public int levelSeed;

		public ForceSettingEntry[] settings;
	}

	[Serializable]
	public class ForceSettingEntry
	{
		public GameplayPatternType patternType;

		public int forcePlateau;

		public int forceRect;
	}

	public int index;

	public string themeName;

	public int rows = 22;

	public int cols = 40;

	public int minStartPointRow = 5;

	public int maxStartPointRow = 17;

	public int minEndPointRow = 5;

	public int maxEndPointRow = 17;

	public int elevationImpact;

	public PathType mainPathType;

	public int mainPathMinWidth = 4;

	public int mainPathMaxWidth = 10;

	public float mainPathMinSegmentLength = 1.8f;

	public float mainPathMaxSegmentLength = 2.2f;

	public float mainPathMinVarianceFactor = 0.5f;

	public float mainPathMaxVarianceFactor = 0.7f;

	public int minRectangleRooms = 1;

	public int maxRectangleRooms = 3;

	public int rectRoomMinRows = 4;

	public int rectRoomMaxRows = 10;

	public int rectRoomMinCols = 5;

	public int rectRoomMaxCols = 12;

	public LevelRoomPlacement rectRoomPlacement;

	public int rectRoomMinRowOffset = -2;

	public int rectRoomMaxRowOffset = 2;

	public int rectRoomMinColOffset = -2;

	public int rectRoomMaxColOffset = 2;

	public int rectRoomElevationVariance = 2;

	public int rectRoomMinPadding;

	public int rectRoomMaxPadding;

	public float rectRoomPaddingProbability = 0.7f;

	public int minEllipseRooms = 1;

	public int maxEllipseRooms = 3;

	public int ellipseRoomMinRows = 4;

	public int ellipseRoomMaxRows = 10;

	public int ellipseRoomMinCols = 5;

	public int ellipseRoomMaxCols = 12;

	public LevelRoomPlacement ellipseRoomPlacement;

	public int ellipseRoomMinRowOffset = -2;

	public int ellipseRoomMaxRowOffset = 2;

	public int ellipseRoomMinColOffset = -2;

	public int ellipseRoomMaxColOffset = 2;

	public int ellipseRoomElevationVariance = 2;

	public int ellipseRoomPadding;

	public PathType sideQuestPathType;

	public int minSideQuests = 1;

	public int maxSideQuests = 3;

	public int sideQuestMinWidth = 4;

	public int sideQuestMaxWidth = 10;

	public float sideQuestMinSegmentLength = 1.8f;

	public float sideQuestMaxSegmentLength = 2.2f;

	public float sideQuestMinVarianceFactor = 0.5f;

	public float sideQuestMaxVarianceFactor = 0.7f;

	public int sideQuestMinColOffset = -4;

	public int sideQuestMaxColOffset = 4;

	public int sideQuestMinEndPointColOffset = -10;

	public int sideQuestMaxEndPointColOffset = 10;

	public float treeFactor = 1f;

	public bool allowHousePatterns = true;

	public bool useSquareEndArea;

	public bool allowChokepointsAtEndOfLevel;

	public int distanceFromEndOfLevelForChokePoints = 11;

	public ForceSetting[] forceSettings;
}
