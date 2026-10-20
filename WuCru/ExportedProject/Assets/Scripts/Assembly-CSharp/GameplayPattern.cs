using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameplayPattern
{
	private class AIPathSegment
	{
		public LevelPoint from;

		public LevelPoint to;

		public bool IsNull
		{
			get
			{
				return from.row == 0 && from.col == 0 && to.row == 0 && to.col == 0;
			}
		}

		public int Length
		{
			get
			{
				if (from.row == to.row)
				{
					return Mathf.Max(from.col, to.col) - Math.Min(from.col, to.col);
				}
				return Mathf.Max(from.row, to.row) - Math.Min(from.row, to.row);
			}
		}

		public AIPathSegment()
		{
		}

		public AIPathSegment(LevelPoint from, LevelPoint to)
		{
			this.from = from;
			this.to = to;
		}

		public bool DoesCross(AIPathSegment other)
		{
			if (from.row == to.row && other.from.row == other.to.row && from.col == other.from.col)
			{
				return true;
			}
			if (from.row == to.row && other.from.row == other.to.row && from.col != other.from.col)
			{
				return false;
			}
			if (from.col == to.col && other.from.col == other.to.col && from.row == other.from.row)
			{
				return true;
			}
			if (from.col == to.col && other.from.col == other.to.col && from.row != other.from.row)
			{
				return false;
			}
			if (from.col == to.col && Mathf.Min(other.from.col, other.to.col) <= from.col && Mathf.Max(other.from.col, other.to.col) >= from.col && Mathf.Min(from.row, to.row) <= other.from.row && Mathf.Max(from.row, to.row) >= other.from.row)
			{
				return true;
			}
			if (from.row == to.row && Mathf.Min(other.from.row, other.to.row) <= from.row && Mathf.Max(other.from.row, other.to.row) >= from.row && Mathf.Min(from.col, to.col) <= other.from.col && Mathf.Max(from.col, to.col) >= other.from.col)
			{
				return true;
			}
			return false;
		}

		public static AIPathSegment GetFirstSegment(AIPathSegment segment, Level level, int maxLength)
		{
			AIPathSegment aIPathSegment = new AIPathSegment();
			int elevation = level.Nodes[segment.from.row, segment.from.col].Elevation;
			bool flag = false;
			bool flag2 = false;
			if (segment.from.row == segment.to.row)
			{
				if (segment.from.col > segment.to.col)
				{
					flag2 = true;
				}
				for (int i = Mathf.Min(segment.from.col, segment.to.col); i <= Mathf.Max(segment.from.col, segment.to.col); i++)
				{
					if (!level.Nodes[segment.from.row, i].IsAIPatrolNode && !level.Nodes[segment.from.row, i].IsNeighbourToAIPatrolNode(level.Nodes) && level.Nodes[segment.from.row, i].ObjectTypes.Count == 0 && !flag && !level.Nodes[segment.from.row, i].HasWallAnyType(LevelDirection.East) && !level.Nodes[segment.from.row, i].HasWallAnyType(LevelDirection.West) && level.Nodes[segment.from.row, i].Elevation == elevation)
					{
						flag = true;
						if (!flag2)
						{
							aIPathSegment.from.row = segment.from.row;
							aIPathSegment.from.col = i;
							aIPathSegment.to.row = segment.from.row;
							aIPathSegment.to.col = i;
						}
						else
						{
							aIPathSegment.to.row = segment.to.row;
							aIPathSegment.to.col = i;
							aIPathSegment.from.row = segment.to.row;
							aIPathSegment.from.col = i;
						}
					}
					if (flag)
					{
						if (level.Nodes[segment.from.row, i].IsAIPatrolNode || level.Nodes[segment.from.row, i].IsNeighbourToAIPatrolNode(level.Nodes) || level.Nodes[segment.from.row, i].ObjectTypes.Count != 0 || level.Nodes[segment.from.row, i].HasWallAnyType(LevelDirection.East) || level.Nodes[segment.from.row, i].HasWallAnyType(LevelDirection.West) || ((!flag2) ? aIPathSegment.to.col : aIPathSegment.from.col) + maxLength > i || level.Nodes[segment.from.row, i].Elevation != elevation)
						{
							break;
						}
						if (!flag2)
						{
							aIPathSegment.to.col = i;
						}
						else
						{
							aIPathSegment.from.col = i;
						}
					}
				}
			}
			else
			{
				if (segment.from.row > segment.to.row)
				{
					flag2 = true;
				}
				for (int j = Mathf.Min(segment.from.row, segment.to.row); j <= Mathf.Max(segment.from.row, segment.to.row); j++)
				{
					if (!flag && !level.Nodes[j, segment.from.col].IsAIPatrolNode && !level.Nodes[j, segment.from.col].IsNeighbourToAIPatrolNode(level.Nodes) && level.Nodes[j, segment.from.col].ObjectTypes.Count == 0 && !level.Nodes[j, segment.from.col].HasWallAnyType(LevelDirection.North) && !level.Nodes[j, segment.from.col].HasWallAnyType(LevelDirection.South) && level.Nodes[j, segment.from.col].Elevation == elevation)
					{
						flag = true;
						if (!flag2)
						{
							aIPathSegment.from.row = j;
							aIPathSegment.from.col = segment.from.col;
							aIPathSegment.to.col = segment.from.col;
							aIPathSegment.to.row = j;
						}
						else
						{
							aIPathSegment.to.row = j;
							aIPathSegment.to.col = segment.to.col;
							aIPathSegment.from.col = segment.to.col;
							aIPathSegment.from.row = j;
						}
					}
					if (flag)
					{
						if (level.Nodes[j, segment.from.col].IsAIPatrolNode || level.Nodes[j, segment.from.col].IsNeighbourToAIPatrolNode(level.Nodes) || level.Nodes[j, segment.from.col].ObjectTypes.Count != 0 || level.Nodes[j, segment.from.col].HasWallAnyType(LevelDirection.North) || level.Nodes[j, segment.from.col].HasWallAnyType(LevelDirection.South) || level.Nodes[j, segment.from.col].Elevation != elevation)
						{
							break;
						}
						if (!flag2)
						{
							aIPathSegment.to.row = j;
						}
						else
						{
							aIPathSegment.from.row = j;
						}
					}
				}
			}
			return aIPathSegment;
		}

		public static AIPathSegment GetNextSegment(AIPathSegment segment, Level level, List<AIPathSegment> segmentList, int maxSegmentLength)
		{
			AIPathSegment aIPathSegment = new AIPathSegment();
			aIPathSegment.from = segment.to;
			aIPathSegment.to = segment.to;
			if (segment.from.row == segment.to.row)
			{
				int num = segment.to.row;
				for (int i = segment.to.row; i < segment.to.row + maxSegmentLength && LevelHelper.PointInsideBounds(new LevelPoint(i, segment.to.col), level.Nodes) && level.Nodes[i, segment.to.col].Elevation >= 1 && level.Nodes[i, segment.to.col].Elevation == level.Nodes[segment.to.row, segment.to.col].Elevation && !level.Nodes[i, segment.to.col].IsAIPatrolNode && !level.Nodes[i, segment.to.col].IsNeighbourToAIPatrolNode(level.Nodes) && level.Nodes[i, segment.to.col].ObjectTypes.Count == 0 && !level.Nodes[i, segment.to.col].HasWallAnyType(LevelDirection.South) && !level.Nodes[i, segment.to.col].HasWallAnyType(LevelDirection.North); i++)
				{
					bool flag = false;
					if (i > segment.to.row)
					{
						AIPathSegment other = new AIPathSegment(new LevelPoint(segment.to.row, segment.to.col), new LevelPoint(i, segment.to.col));
						for (int j = 0; j < segmentList.Count - 1; j++)
						{
							if (segmentList[j].DoesCross(other))
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						num = i;
						continue;
					}
					break;
				}
				int num2 = segment.to.row;
				int num3 = segment.to.row;
				while (num3 > segment.to.row - maxSegmentLength && LevelHelper.PointInsideBounds(new LevelPoint(num3, segment.to.col), level.Nodes) && level.Nodes[num3, segment.to.col].Elevation >= 1 && level.Nodes[num3, segment.to.col].Elevation == level.Nodes[segment.to.row, segment.to.col].Elevation && !level.Nodes[num3, segment.to.col].IsAIPatrolNode && !level.Nodes[num3, segment.to.col].IsNeighbourToAIPatrolNode(level.Nodes) && level.Nodes[num3, segment.to.col].ObjectTypes.Count == 0 && !level.Nodes[num3, segment.to.col].HasWallAnyType(LevelDirection.North) && !level.Nodes[num3, segment.to.col].HasWallAnyType(LevelDirection.South))
				{
					bool flag2 = false;
					if (num3 < segment.to.row)
					{
						AIPathSegment other2 = new AIPathSegment(new LevelPoint(segment.to.row, segment.to.col), new LevelPoint(num3, segment.to.col));
						for (int k = 0; k < segmentList.Count - 1; k++)
						{
							if (segmentList[k].DoesCross(other2))
							{
								flag2 = true;
							}
						}
					}
					if (!flag2)
					{
						num2 = num3;
						num3--;
						continue;
					}
					break;
				}
				if (num - segment.to.row >= segment.to.row - num2)
				{
					aIPathSegment.to.row = num;
				}
				else
				{
					aIPathSegment.to.row = num2;
				}
			}
			else
			{
				int num4 = segment.to.col;
				for (int l = segment.to.col; l < segment.to.col + maxSegmentLength && LevelHelper.PointInsideBounds(new LevelPoint(segment.to.row, l), level.Nodes) && level.Nodes[segment.to.row, l].Elevation >= 1 && level.Nodes[segment.to.row, l].Elevation == level.Nodes[segment.to.row, segment.to.col].Elevation && !level.Nodes[segment.to.row, l].IsAIPatrolNode && !level.Nodes[segment.to.row, l].IsNeighbourToAIPatrolNode(level.Nodes) && level.Nodes[segment.to.row, l].ObjectTypes.Count == 0 && !level.Nodes[segment.to.row, l].HasWallAnyType(LevelDirection.West) && !level.Nodes[segment.to.row, l].HasWallAnyType(LevelDirection.East); l++)
				{
					bool flag3 = false;
					if (l > segment.to.col)
					{
						AIPathSegment other3 = new AIPathSegment(new LevelPoint(segment.to.row, segment.to.col), new LevelPoint(segment.to.row, l));
						for (int m = 0; m < segmentList.Count - 1; m++)
						{
							if (segmentList[m].DoesCross(other3))
							{
								flag3 = true;
							}
						}
					}
					if (!flag3)
					{
						num4 = l;
						continue;
					}
					break;
				}
				int num5 = segment.to.col;
				int num6 = segment.to.col;
				while (num6 > segment.to.col - maxSegmentLength && LevelHelper.PointInsideBounds(new LevelPoint(segment.to.row, num6), level.Nodes) && level.Nodes[segment.to.row, num6].Elevation >= 1 && level.Nodes[segment.to.row, num6].Elevation == level.Nodes[segment.to.row, segment.to.col].Elevation && !level.Nodes[segment.to.row, num6].IsAIPatrolNode && !level.Nodes[segment.to.row, num6].IsNeighbourToAIPatrolNode(level.Nodes) && level.Nodes[segment.to.row, num6].ObjectTypes.Count == 0 && !level.Nodes[segment.to.row, num6].HasWallAnyType(LevelDirection.East) && !level.Nodes[segment.to.row, num6].HasWallAnyType(LevelDirection.West))
				{
					bool flag4 = false;
					if (num6 < segment.to.col)
					{
						AIPathSegment other4 = new AIPathSegment(new LevelPoint(segment.to.row, segment.to.col), new LevelPoint(segment.to.row, num6));
						for (int n = 0; n < segmentList.Count - 1; n++)
						{
							if (segmentList[n].DoesCross(other4))
							{
								flag4 = true;
							}
						}
					}
					if (!flag4)
					{
						num5 = num6;
						num6--;
						continue;
					}
					break;
				}
				if (num4 - segment.to.col >= segment.to.col - num5)
				{
					aIPathSegment.to.col = num4;
				}
				else
				{
					aIPathSegment.to.col = num5;
				}
			}
			return aIPathSegment;
		}
	}

	public abstract GameplayPatternType PatternType { get; }

	public abstract int Setup(LevelPlateauRect rect, Level level);

	public abstract int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNodes, Level level, bool isEndLevel);

	public int CalcScoreFromValueMatch(int matchValue, int value)
	{
		float num = (float)matchValue - Mathf.Clamp(Mathf.Abs((float)matchValue - (float)value), 0f, matchValue);
		return Mathf.RoundToInt(num / (float)value * 100f);
	}

	protected int CalcUsageScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects)
	{
		int num = 0;
		float num2 = 0f;
		int num3 = 0;
		List<LevelPlateau> list = new List<LevelPlateau>();
		foreach (LevelPlateauRect usedRect in usedRects)
		{
			if (!list.Contains(usedRect.plateau))
			{
				list.Add(usedRect.plateau);
			}
			num2 += rect.GetOverlappingFactor(usedRect);
			if (rect.plateau != usedRect.plateau)
			{
				num3++;
			}
		}
		num += Mathf.RoundToInt((1f - num2 / (float)usedRects.Count) * 100f);
		return num + Mathf.RoundToInt((float)num3 / (float)list.Count * 50f);
	}

	protected int CalcChokepointElevationScore(Level level, LevelPlateauRect rect, float scale = 50f)
	{
		int num = 0;
		for (int i = 0; i < rect.chokePoint.numNodes; i++)
		{
			if (rect.chokePoint.dir == LevelDirection.North && level.Nodes[rect.chokePoint.startPoint.row + i, rect.chokePoint.startPoint.col].Elevation == level.Nodes[rect.chokePoint.startPoint.row + i, rect.chokePoint.startPoint.col + 1].Elevation)
			{
				num++;
			}
			if (rect.chokePoint.dir == LevelDirection.East && level.Nodes[rect.chokePoint.startPoint.row, rect.chokePoint.startPoint.col + i].Elevation == level.Nodes[rect.chokePoint.startPoint.row + 1, rect.chokePoint.startPoint.col + i].Elevation)
			{
				num++;
			}
		}
		return Mathf.RoundToInt((float)num / (float)rect.chokePoint.numNodes * scale);
	}

	protected int CalcChokepointObjectDensityScore(Level level, LevelPlateauRect rect, float scale = 20f)
	{
		if (rect.chokePoint == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < rect.chokePoint.numNodes; i++)
		{
			if (rect.chokePoint.dir == LevelDirection.North && level.Nodes[rect.chokePoint.startPoint.row + i, rect.chokePoint.startPoint.col].ObjectTypes.Count == 0)
			{
				num++;
			}
			if (rect.chokePoint.dir == LevelDirection.East && level.Nodes[rect.chokePoint.startPoint.row, rect.chokePoint.startPoint.col + i].ObjectTypes.Count == 0)
			{
				num++;
			}
		}
		return Mathf.RoundToInt((float)num / (float)rect.chokePoint.numNodes * scale);
	}

	protected int CalcRectObjectDensityScore(Level level, LevelPlateauRect rect, float scale = 20f)
	{
		int num = 0;
		for (int i = 0; i < rect.nodes.Count; i++)
		{
			if (rect.nodes[i].ObjectTypes.Count == 0)
			{
				num++;
			}
		}
		return Mathf.RoundToInt((float)num / (float)rect.nodes.Count * scale);
	}

	protected int CalcChokepointNotChunkTypeScore(Level level, LevelPlateauRect rect, LevelChunkType chunkType, float scale)
	{
		int num = 0;
		for (int i = 0; i < rect.chokePoint.numNodes; i++)
		{
			if (rect.chokePoint.dir == LevelDirection.North && level.ChunkDB.GetChunk(level.Nodes[rect.chokePoint.startPoint.row + i, rect.chokePoint.startPoint.col].ChunkId).ChunkType != LevelChunkType.RectangleRoom)
			{
				num++;
			}
			if (rect.chokePoint.dir == LevelDirection.East && level.ChunkDB.GetChunk(level.Nodes[rect.chokePoint.startPoint.row, rect.chokePoint.startPoint.col + 1].ChunkId).ChunkType != LevelChunkType.RectangleRoom)
			{
				num++;
			}
		}
		return Mathf.RoundToInt((float)num / (float)rect.chokePoint.numNodes * scale);
	}

	protected int CalcIsChunkTypeScore(Level level, LevelPlateauRect rect, LevelChunkType chunkType, float scale)
	{
		int num = 0;
		foreach (LevelNode node in rect.nodes)
		{
			if (level.ChunkDB.GetChunk(node.ChunkId).ChunkType == chunkType)
			{
				num++;
			}
		}
		return Mathf.RoundToInt((float)num / (float)rect.nodes.Count * scale);
	}

	protected int CalcNotChunkTypeScore(Level level, LevelPlateauRect rect, LevelChunkType chunkType, float scale)
	{
		int num = 0;
		foreach (LevelNode node in rect.nodes)
		{
			if (level.ChunkDB.GetChunk(node.ChunkId).ChunkType != chunkType)
			{
				num++;
			}
		}
		return Mathf.RoundToInt((float)num / (float)rect.nodes.Count * scale);
	}

	public int CalcRectStartOfLevelScore(LevelPlateauRect rect, Level level, float scale)
	{
		return Mathf.RoundToInt(((float)level.Nodes.GetLength(1) - (float)rect.min.col / (float)level.Nodes.GetLength(1)) * scale);
	}

	public LevelPlateauRect FindBestRect(Level level, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNodes, bool isEndLevel)
	{
		int num = 0;
		LevelPlateauRect result = null;
		foreach (LevelPlateau levelPlateau in level.ChunkDB.LevelPlateaus)
		{
			foreach (LevelPlateauRect rect in levelPlateau.rects)
			{
				if (rect == null)
				{
					Debug.LogError("Rect is null!");
				}
				else if (!rect.isUsed && !usedRects.Contains(rect))
				{
					int num2 = CalcRectScore(rect, usedRects, houseRects, houseNodes, level, isEndLevel);
					if (num2 > num)
					{
						num = num2;
						result = rect;
					}
				}
			}
		}
		return result;
	}

	protected List<LevelNode> CollectPickups(Level level)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < level.Nodes.GetLength(0); i++)
		{
			for (int j = 0; j < level.Nodes.GetLength(1); j++)
			{
				if (level.Nodes[i, j].ObjectTypes.Count <= 0)
				{
					continue;
				}
				foreach (LevelNodeObjectTypeAndTag objectType in level.Nodes[i, j].ObjectTypes)
				{
					if (objectType.objectType == LevelNodeObjectType.GoldChest || objectType.objectType == LevelNodeObjectType.Key || objectType.objectType == LevelNodeObjectType.MetalChest || objectType.objectType == LevelNodeObjectType.WoodChest || objectType.objectType == LevelNodeObjectType.CrystalShard)
					{
						list.Add(level.Nodes[i, j]);
					}
				}
			}
		}
		return list;
	}

	protected AIAgentType GetRandomEnemyType()
	{
		AIAgentType result = AIAgentType.ScoutEnemy;
		if (GameController.Instance.PlayerProfile.Level < 2)
		{
			return AIAgentType.ScoutEnemy;
		}
		if (GameController.Instance.PlayerProfile.Level >= 2 && GameController.Instance.PlayerProfile.Level <= 3)
		{
			float value = UnityEngine.Random.value;
			if (!((double)value < 0.5))
			{
				result = AIAgentType.GruntEnemy;
			}
		}
		else
		{
			float value2 = UnityEngine.Random.value;
			if (!((double)value2 < 0.33))
			{
				result = (((double)value2 < 0.66) ? AIAgentType.GruntEnemy : AIAgentType.WarriorEnemy);
			}
		}
		return result;
	}

	protected LevelNodeObjectType GetLevelNodeTypeForEnemy(AIAgentType agentType)
	{
		switch (agentType)
		{
		case AIAgentType.ScoutEnemy:
			return LevelNodeObjectType.ScoutEnemy;
		case AIAgentType.GruntEnemy:
			return LevelNodeObjectType.GruntEnemy;
		case AIAgentType.WarriorEnemy:
			return LevelNodeObjectType.WarriorEnemy;
		default:
			return LevelNodeObjectType.None;
		}
	}

	protected LevelNode[] GetPatrolPathWaypoints(LevelPlateauRect rect, Level level, int maxSegments, int maxFirstSegmentLength, int maxNextSegmentLength)
	{
		int num = 0;
		AIPathSegment aIPathSegment = new AIPathSegment();
		int num2 = 0;
		foreach (LevelDirection value in Enum.GetValues(typeof(LevelDirection)))
		{
			AIPathSegment segment = new AIPathSegment();
			List<AIPathSegment> list = new List<AIPathSegment>();
			if (value == LevelDirection.North)
			{
				segment = new AIPathSegment(new LevelPoint(rect.min.row, rect.min.col), new LevelPoint(rect.max.row, rect.min.col));
			}
			if (value == LevelDirection.South)
			{
				segment = new AIPathSegment(new LevelPoint(rect.max.row, rect.max.col), new LevelPoint(rect.min.row, rect.max.col));
			}
			if (value == LevelDirection.East)
			{
				segment = new AIPathSegment(new LevelPoint(rect.max.row, rect.min.col), new LevelPoint(rect.max.row, rect.max.col));
			}
			if (value == LevelDirection.West)
			{
				segment = new AIPathSegment(new LevelPoint(rect.min.row, rect.min.col), new LevelPoint(rect.min.row, rect.max.col));
			}
			AIPathSegment firstSegment = AIPathSegment.GetFirstSegment(segment, level, maxFirstSegmentLength);
			int num3 = firstSegment.Length;
			int num4 = 1;
			if (firstSegment.Length > 0 && maxSegments > 1)
			{
				list.Add(firstSegment);
				AIPathSegment aIPathSegment2 = firstSegment;
				for (int i = 1; i < maxSegments; i++)
				{
					aIPathSegment2 = AIPathSegment.GetNextSegment(aIPathSegment2, level, list, maxNextSegmentLength);
					if (aIPathSegment2.Length > 0)
					{
						list.Add(aIPathSegment2);
						num3 += aIPathSegment2.Length;
						num4++;
						continue;
					}
					break;
				}
			}
			if (num3 > num)
			{
				num = num3;
				aIPathSegment = firstSegment;
				num2 = num4;
			}
		}
		if (!aIPathSegment.IsNull && aIPathSegment.Length > 0)
		{
			List<AIPathSegment> list2 = new List<AIPathSegment>();
			LevelNode[] array = new LevelNode[num2 + 1];
			array[0] = level.Nodes[aIPathSegment.from.row, aIPathSegment.from.col];
			array[1] = level.Nodes[aIPathSegment.to.row, aIPathSegment.to.col];
			list2.Add(aIPathSegment);
			AIPathSegment aIPathSegment3 = aIPathSegment;
			for (int j = 2; j <= num2; j++)
			{
				aIPathSegment3 = AIPathSegment.GetNextSegment(aIPathSegment3, level, list2, maxNextSegmentLength);
				array[j] = level.Nodes[aIPathSegment3.to.row, aIPathSegment3.to.col];
			}
			int elevation = array[0].Elevation;
			LevelNode[] array2 = array;
			foreach (LevelNode levelNode in array2)
			{
				if (levelNode.Elevation != elevation)
				{
					Debug.LogWarning("********** AIPatrol Path waypoints have DIFFERENT ELEVATION **********");
					Debug.DrawLine(LevelHelper.NodeToVector3(array[0]), LevelHelper.NodeToVector3(levelNode), Color.yellow, 10f);
					Debug.DrawLine(LevelHelper.NodeToVector3(array[0]), LevelHelper.NodeToVector3(array[0]) + Vector3.up, Color.green, 10f);
				}
			}
			return array;
		}
		return null;
	}
}
