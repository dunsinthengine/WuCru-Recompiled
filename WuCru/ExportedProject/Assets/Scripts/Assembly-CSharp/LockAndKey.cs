using System.Collections.Generic;
using UnityEngine;

public class LockAndKey : GameplayPattern
{
	private int keyGameplayPointCost = 15;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.LockAndKey;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		LevelPoint levelPoint = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.x));
		PathFinding pathFinding = new PathFinding(level.Nodes);
		int num = 0;
		bool flag = false;
		foreach (LevelPlateauRect houseRect in houseRects)
		{
			if (houseRect.Intersect(rect))
			{
				flag = true;
				break;
			}
		}
		if (!flag && rect.chokePoint != null && rect.chokePoint.dir == LevelDirection.North && rect.min.col > level.Nodes.GetLength(1) / 2 + 3 && rect.max.col < level.Nodes.GetLength(1) - 3 && pathFinding.GetPathIgnoreDoors(level.Nodes[levelPoint.row, levelPoint.col], level.Nodes[rect.row, rect.col], null) != null && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			bool flag2 = false;
			for (int i = rect.chokePoint.startPoint.row; i < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; i++)
			{
				if (level.Nodes[i, rect.chokePoint.startPoint.col].EastEdgeValue == LevelNodeEdgeValue.Ladder || level.Nodes[i, rect.chokePoint.startPoint.col].EastEdgeValue == LevelNodeEdgeValue.DoubleLadder || level.Nodes[i, rect.chokePoint.startPoint.col].EastEdgeValue == LevelNodeEdgeValue.DownDrain || level.Nodes[i, rect.chokePoint.startPoint.col].EastEdgeValue == LevelNodeEdgeValue.Tree || Door.IsDoorEdgeValue(level.Nodes[i, rect.chokePoint.startPoint.col].EastEdgeValue) || level.Nodes[i, rect.chokePoint.startPoint.col].IsAIPatrolNode)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				bool flag3 = true;
				LevelNode levelNode = null;
				List<LevelNode> list = new List<LevelNode>();
				int col = rect.chokePoint.startPoint.col;
				for (int j = rect.chokePoint.startPoint.row; j < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; j++)
				{
					if (level.PlacementHelper.EvaluateNodeEdge(level.Nodes[j, col], LevelNodeEdgeValue.AltWall, LevelDirection.East))
					{
						level.Nodes[j, col].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.AltWall);
						list.Add(level.Nodes[j, col]);
						if (levelNode == null)
						{
							LevelNode neighbour = level.Nodes[j, col].GetNeighbour(level.Nodes, LevelDirection.East);
							if (neighbour != null && neighbour.Elevation == level.Nodes[j, col].Elevation)
							{
								levelNode = neighbour;
							}
						}
						continue;
					}
					flag3 = false;
					break;
				}
				if (flag3)
				{
					if (levelNode == null)
					{
						flag3 = false;
					}
					else
					{
						LevelPoint levelPoint2 = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.x));
						if (pathFinding.GetPathIgnoreDoors(levelNode, level.Nodes[levelPoint2.row, levelPoint2.col], null) == null)
						{
							flag3 = false;
						}
					}
				}
				foreach (LevelNode item in list)
				{
					item.EastEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (flag3)
				{
					num += CalcUsageScore(rect, usedRects);
					Debug.Log("Considering lockkey Chokepoint: " + rect.chokePoint.startPoint.col + " / " + rect.chokePoint.numNodes);
					num += Mathf.Clamp(7 - rect.chokePoint.numNodes, 0, 7) * 20;
					num += CalcChokepointElevationScore(level, rect, 150f);
					num += CalcChokepointObjectDensityScore(level, rect, 30f);
					num += CalcChokepointNotChunkTypeScore(level, rect, LevelChunkType.RectangleRoom, 100f);
				}
			}
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		LevelPlateauRect forceSettingRectForPattern = level.GetForceSettingRectForPattern(GameplayPatternType.LockAndKey);
		if (forceSettingRectForPattern != null)
		{
			rect = forceSettingRectForPattern;
		}
		if (rect.chokePoint == null)
		{
			Debug.LogError("Cannot setup on rect, because rect has no choke-point!");
			return 0;
		}
		Debug.Log("Lock And Key Chokepoint on: " + rect.chokePoint.startPoint.col);
		PathFinding pathFinding = new PathFinding(level.Nodes);
		LevelPoint levelPoint = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.x));
		LevelPoint levelPoint2 = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.x));
		int col = rect.chokePoint.startPoint.col;
		for (int i = rect.chokePoint.startPoint.row; i < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; i++)
		{
			bool flag = true;
			bool flag2 = true;
			if (level.Nodes[i, col].ObjectTypes.Count > 0)
			{
				foreach (LevelNodeObjectTypeAndTag objectType in level.Nodes[i, col].ObjectTypes)
				{
					if (objectType.objectType == LevelNodeObjectType.EndFlag || objectType.objectType == LevelNodeObjectType.Key || objectType.objectType == LevelNodeObjectType.DoorTrigger)
					{
						flag = false;
					}
				}
			}
			if (flag2)
			{
				if (flag)
				{
					level.Nodes[i, col].ClearObjectTypes(level.ChunkDB);
				}
				level.Nodes[i, col].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.AltWall);
			}
		}
		List<LevelPoint> list = new List<LevelPoint>();
		for (int j = rect.chokePoint.startPoint.row; j < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; j++)
		{
			PathFinding.Node[] pathIgnoreDoors = pathFinding.GetPathIgnoreDoors(level.Nodes[levelPoint.row, levelPoint.col], level.Nodes[j, col], null);
			if (pathIgnoreDoors != null)
			{
				PathFinding.Node[] pathIgnoreDoors2 = pathFinding.GetPathIgnoreDoors(level.Nodes[j, col + 1], level.Nodes[levelPoint2.row, levelPoint2.col], null);
				if (pathIgnoreDoors2 != null && (level.Nodes[j, col + 1].Elevation == level.Nodes[j, col].Elevation || level.Nodes[j, col].HasLadderAnySize(LevelDirection.East) || level.Nodes[j, col + 1].HasLadderAnySize(LevelDirection.West)))
				{
					list.Add(new LevelPoint(j, col));
				}
			}
		}
		if (list.Count > 0)
		{
			int num = rect.chokePoint.startPoint.row + rect.chokePoint.numNodes / 2;
			int num2 = int.MaxValue;
			LevelPoint doorPoint = default(LevelPoint);
			foreach (LevelPoint item in list)
			{
				if (Mathf.Abs(num - item.row) < num2)
				{
					num2 = Mathf.Abs(num - item.row);
					doorPoint.row = item.row;
					doorPoint.col = col;
				}
			}
			int row = doorPoint.row;
			level.Nodes[row, col].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.AltKeyDoor);
			LevelNode levelNode = level.Nodes[row, col + 1];
			if (levelNode != null)
			{
				levelNode.SetNodeEdgeValue(LevelDirection.West, LevelNodeEdgeValue.Empty);
			}
			if (PlaceKey(level, rect, doorPoint))
			{
				return rect.chokePoint.numNodes + keyGameplayPointCost;
			}
			return rect.chokePoint.numNodes;
		}
		Debug.LogWarning("Could not find door/key location. Not the best rect! Should fix in CalcRectScore (and delete wall before bailing out if it ever occur again)");
		return rect.chokePoint.numNodes;
	}

	private bool PlaceKey(Level level, LevelPlateauRect rect, LevelPoint doorPoint)
	{
		PathFinding pathFinding = new PathFinding(level.Nodes);
		LevelPoint levelPoint = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.x));
		LevelPoint levelPoint2 = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.x));
		List<LevelNode> list = CollectPickups(level);
		int num = 0;
		LevelPoint levelPoint3 = default(LevelPoint);
		if (rect.chokePoint.dir == LevelDirection.North)
		{
			for (int i = 0; i < level.Nodes.GetLength(0); i++)
			{
				for (int j = 0; j < doorPoint.col - 1; j++)
				{
					if (!level.PlacementHelper.EvaluateNode(level.Nodes[i, j], LevelNodeObjectType.Key))
					{
						continue;
					}
					PathFinding.Node[] pathIncludeDoors = pathFinding.GetPathIncludeDoors(level.Nodes[levelPoint.row, levelPoint.col], level.Nodes[i, j], null);
					PathFinding.Node[] pathIgnoreDoors = pathFinding.GetPathIgnoreDoors(level.Nodes[doorPoint.row, doorPoint.col], level.Nodes[i, j], null);
					if (pathIncludeDoors == null || pathIgnoreDoors == null)
					{
						continue;
					}
					int num2 = pathIncludeDoors.Length + pathIgnoreDoors.Length;
					if (level.Nodes[i, j].Type == LevelNodeType.SideQuest)
					{
						num2 += 10;
					}
					if (list.Count > 0)
					{
						int num3 = 0;
						foreach (LevelNode item in list)
						{
							num3 += Mathf.RoundToInt(LevelHelper.Distance(item, level.Nodes[i, j]));
						}
						num2 += num3 / list.Count;
					}
					if (num2 > num)
					{
						num = num2;
						levelPoint3.row = i;
						levelPoint3.col = j;
					}
				}
			}
		}
		if (num > 0)
		{
			level.Nodes[levelPoint3.row, levelPoint3.col].AddObjectType(LevelNodeObjectType.Key, level.ChunkDB, string.Empty);
			return true;
		}
		Debug.LogError("Could not find location for Key");
		return false;
	}
}
