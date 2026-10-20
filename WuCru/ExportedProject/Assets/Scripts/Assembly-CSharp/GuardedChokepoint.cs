using System.Collections.Generic;
using UnityEngine;

public class GuardedChokepoint : GameplayPattern
{
	private int aiAgentGameplayPointCost = 5;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.GuardedChokepoint;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		LevelPoint startPoint = level.StartPoint;
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
		if (!flag && rect.chokePoint != null && rect.min.col > level.Nodes.GetLength(1) / 4 && rect.max.col < level.Nodes.GetLength(1) - 6 && rect.chokePoint.numNodes >= 3 && pathFinding.GetPathIgnoreDoors(level.Nodes[startPoint.row, startPoint.col], level.Nodes[rect.min.row, rect.min.col], null) != null && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			bool flag2 = false;
			if (rect.chokePoint.dir == LevelDirection.North)
			{
				for (int i = rect.chokePoint.startPoint.row; i < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; i++)
				{
					if (level.Nodes[i, rect.chokePoint.startPoint.col].HasLadderAnySizeAnyDirection() || level.Nodes[i, rect.chokePoint.startPoint.col].HasWallAnyTypeAnyDirection() || Door.IsDoorEdgeValue(level.Nodes[i, rect.chokePoint.startPoint.col].EastEdgeValue) || level.Nodes[i, rect.chokePoint.startPoint.col].IsAIPatrolNode)
					{
						flag2 = true;
					}
				}
			}
			else if (rect.chokePoint.dir == LevelDirection.East)
			{
				for (int j = rect.chokePoint.startPoint.col; j < rect.chokePoint.startPoint.col + rect.chokePoint.numNodes; j++)
				{
					if (level.Nodes[rect.chokePoint.startPoint.row, j].HasLadderAnySizeAnyDirection() || level.Nodes[rect.chokePoint.startPoint.row, j].HasWallAnyTypeAnyDirection() || Door.IsDoorEdgeValue(level.Nodes[rect.chokePoint.startPoint.row, j].EastEdgeValue) || level.Nodes[rect.chokePoint.startPoint.row, j].IsAIPatrolNode)
					{
						flag2 = true;
					}
				}
			}
			if (!flag2)
			{
				num += CalcUsageScore(rect, usedRects);
				Debug.Log("Considering guarded Chokepoint: " + rect.chokePoint.startPoint.col + " / " + rect.chokePoint.numNodes);
				num += Mathf.Clamp(rect.plateau.nodes.Count, 0, 40) / 40 * 100;
				num += CalcChokepointElevationScore(level, rect, 120f);
				num += CalcChokepointObjectDensityScore(level, rect, 25f);
				if (rect.chokePoint.dir == LevelDirection.North)
				{
					bool flag3 = true;
					int num2 = rect.min.col + 1;
					int elevation = level.Nodes[rect.chokePoint.startPoint.row, rect.chokePoint.startPoint.col].Elevation;
					int num3 = 0;
					while (flag3)
					{
						int num4 = 0;
						for (int k = rect.min.row; k < rect.min.row + rect.chokePoint.numNodes; k++)
						{
							if (LevelHelper.PointInsideBounds(new LevelPoint(k, num2), level.Nodes) && level.Nodes[k, num2].Elevation == elevation)
							{
								num4++;
							}
						}
						if (num4 < rect.chokePoint.numNodes)
						{
							flag3 = false;
							continue;
						}
						num2++;
						num3++;
					}
					num += num3 * 100;
				}
				if (rect.chokePoint.dir != LevelDirection.East)
				{
				}
			}
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		LevelPlateauRect forceSettingRectForPattern = level.GetForceSettingRectForPattern(GameplayPatternType.GuardedChokepoint);
		if (forceSettingRectForPattern != null)
		{
			rect = forceSettingRectForPattern;
		}
		Debug.Log("Guarded Chokepoint on: " + rect.chokePoint.startPoint.col);
		PathFinding pathFinding = new PathFinding(level.Nodes);
		LevelPoint levelPoint = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.from.x));
		LevelPoint levelPoint2 = new LevelPoint(Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.y), Mathf.RoundToInt(level.ChunkDB.GetChunk<MainPathChunk>().Path.to.x));
		if (rect.chokePoint.dir == LevelDirection.North)
		{
			int col = rect.chokePoint.startPoint.col;
			for (int i = rect.chokePoint.startPoint.row; i < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; i++)
			{
				bool flag = true;
				if (level.Nodes[i, col].ObjectTypes.Count > 0)
				{
					foreach (LevelNodeObjectTypeAndTag objectType in level.Nodes[i, col].ObjectTypes)
					{
						if (objectType.objectType == LevelNodeObjectType.EndFlag || objectType.objectType == LevelNodeObjectType.GruntEnemy || objectType.objectType == LevelNodeObjectType.Key || objectType.objectType == LevelNodeObjectType.DoorTrigger)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					level.Nodes[i, col].ClearObjectTypes(level.ChunkDB);
					level.Nodes[i, col].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.AltWall);
				}
			}
			List<LevelPoint> list = new List<LevelPoint>();
			for (int j = rect.chokePoint.startPoint.row; j < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; j++)
			{
				PathFinding.Node[] pathIgnoreDoors = pathFinding.GetPathIgnoreDoors(level.Nodes[levelPoint.row, levelPoint.col], level.Nodes[j, col], null);
				if (pathIgnoreDoors != null && LevelHelper.PointInsideBounds(new LevelPoint(j, col + 1), level.Nodes))
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
				LevelPoint levelPoint3 = default(LevelPoint);
				foreach (LevelPoint item in list)
				{
					if (Mathf.Abs(num - item.row) < num2)
					{
						num2 = Mathf.Abs(num - item.row);
						levelPoint3.row = item.row;
						levelPoint3.col = col;
					}
				}
				int row = levelPoint3.row;
				level.Nodes[row, col].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.Empty);
				if (level.Nodes[row, col + 1].ObjectTypes.Count == 0)
				{
					level.Nodes[row, col + 1].AddObjectType(LevelNodeObjectType.ScoutEnemy, level.ChunkDB, string.Empty);
					int agentId = level.ChunkDB.RegisterAIAgent(AIAgentType.ScoutEnemy, level.Nodes[row, col + 1], level.Nodes, AIPathMode.PingPong);
					level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[row, col + 1], 2f, level.Nodes);
				}
				return rect.chokePoint.numNodes + aiAgentGameplayPointCost;
			}
			col = rect.chokePoint.startPoint.col;
			for (int k = rect.chokePoint.startPoint.row; k < rect.chokePoint.startPoint.row + rect.chokePoint.numNodes; k++)
			{
				level.Nodes[k, col].EastEdgeValue = LevelNodeEdgeValue.Empty;
			}
			return rect.chokePoint.numNodes;
		}
		if (rect.chokePoint.dir == LevelDirection.East)
		{
			int row2 = rect.chokePoint.startPoint.row;
			for (int l = rect.chokePoint.startPoint.col; l < rect.chokePoint.startPoint.col + rect.chokePoint.numNodes; l++)
			{
				level.Nodes[row2, l].ClearObjectTypes(level.ChunkDB);
				level.Nodes[row2, l].SetNodeEdgeValue(LevelDirection.North, LevelNodeEdgeValue.AltWall);
			}
			List<LevelPoint> list2 = new List<LevelPoint>();
			for (int m = rect.chokePoint.startPoint.col; m < rect.chokePoint.startPoint.col + rect.chokePoint.numNodes; m++)
			{
				PathFinding.Node[] pathIgnoreDoors3 = pathFinding.GetPathIgnoreDoors(level.Nodes[levelPoint.row, levelPoint.col], level.Nodes[row2, m], null);
				if (pathIgnoreDoors3 != null && LevelHelper.PointInsideBounds(new LevelPoint(row2 + 1, m), level.Nodes))
				{
					PathFinding.Node[] pathIgnoreDoors4 = pathFinding.GetPathIgnoreDoors(level.Nodes[row2 + 1, m], level.Nodes[levelPoint2.row, levelPoint2.col], null);
					if (pathIgnoreDoors4 != null && (level.Nodes[row2 + 1, m].Elevation == level.Nodes[row2, m].Elevation || level.Nodes[row2, m].HasLadderAnySize(LevelDirection.North) || level.Nodes[row2 + 1, m].HasLadderAnySize(LevelDirection.South)))
					{
						list2.Add(new LevelPoint(row2, m));
					}
				}
			}
			if (list2.Count > 0)
			{
				int num3 = rect.chokePoint.startPoint.col + rect.chokePoint.numNodes / 2;
				int num4 = int.MaxValue;
				LevelPoint levelPoint4 = default(LevelPoint);
				foreach (LevelPoint item2 in list2)
				{
					if (Mathf.Abs(num3 - item2.col) < num4)
					{
						num4 = Mathf.Abs(num3 - item2.col);
						levelPoint4.row = row2;
						levelPoint4.col = item2.col;
					}
				}
				int col2 = levelPoint4.col;
				level.Nodes[row2, col2].SetNodeEdgeValue(LevelDirection.North, LevelNodeEdgeValue.Empty);
				level.Nodes[row2, col2].AddObjectType(LevelNodeObjectType.ScoutEnemy, level.ChunkDB, string.Empty);
				int agentId2 = level.ChunkDB.RegisterAIAgent(AIAgentType.ScoutEnemy, level.Nodes[row2, col2], level.Nodes, AIPathMode.PingPong);
				level.ChunkDB.RegisterAIPathWaypoint(agentId2, level.Nodes[row2, col2], 2f, level.Nodes);
				return rect.chokePoint.numNodes + aiAgentGameplayPointCost;
			}
			int col3 = rect.chokePoint.startPoint.col;
			for (int n = rect.chokePoint.startPoint.col; n < rect.chokePoint.startPoint.col + rect.chokePoint.numNodes; n++)
			{
				level.Nodes[row2, n].SetNodeEdgeValue(LevelDirection.North, LevelNodeEdgeValue.Empty);
			}
			return 0;
		}
		return 0;
	}
}
