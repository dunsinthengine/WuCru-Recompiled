using System.Collections.Generic;
using UnityEngine;

public class HousePattern : GameplayPattern
{
	private int maxHouseRows = 7;

	private int maxHouseCols = 7;

	private int altMax = 5;

	private bool pathFromStart;

	private bool pathToEnd;

	private PathFinding pf;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.HousePattern;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		int num = 0;
		if (!level.LevelTheme.allowHousePatterns)
		{
			return num;
		}
		bool flag = false;
		foreach (LevelPlateauRect houseRect in houseRects)
		{
			if (houseRect.Intersect(rect))
			{
				flag = true;
				break;
			}
		}
		pf = new PathFinding(level.Nodes);
		pathFromStart = pf.GetPathIgnoreDoors(level.Nodes[level.StartPoint.row, level.StartPoint.col], level.Nodes[rect.min.row, rect.min.col], null) != null;
		pathToEnd = pf.GetPathIgnoreDoors(level.Nodes[level.EndPoint.row, level.EndPoint.col], level.Nodes[rect.min.row, rect.min.col], null) != null;
		if (rect.width >= 5 && rect.height >= 5 && !flag && rect.min.col > 5 && rect.max.col < level.Nodes.GetLength(1) - 5 && pathFromStart && pathToEnd && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			num += rect.width + rect.height;
			if (level.LevelTheme.rectRoomPlacement == LevelRoomPlacement.OnMainPath)
			{
				CalcIsChunkTypeScore(level, rect, LevelChunkType.MainPath, 20f);
			}
			else if (level.LevelTheme.rectRoomPlacement == LevelRoomPlacement.OffMainPath)
			{
				CalcNotChunkTypeScore(level, rect, LevelChunkType.MainPath, 20f);
			}
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		int num = rect.height - 2;
		int num2 = rect.width - 2;
		if (num % 2 == 0)
		{
			num++;
		}
		if (num2 % 2 == 0)
		{
			num2++;
		}
		if (num > maxHouseRows)
		{
			num = maxHouseRows;
		}
		if (num2 > maxHouseCols)
		{
			num2 = maxHouseCols;
		}
		if (num == maxHouseRows && num2 == maxHouseCols)
		{
			if (Random.value < 0.5f)
			{
				num = altMax;
			}
			else
			{
				num2 = altMax;
			}
		}
		AIAgentType type = AIAgentType.GruntEnemy;
		LevelNodeObjectType obj = LevelNodeObjectType.GruntEnemy;
		if (GameController.Instance.PlayerProfile.Level < 2)
		{
			type = AIAgentType.ScoutEnemy;
			obj = LevelNodeObjectType.ScoutEnemy;
		}
		LevelPoint center = new LevelPoint(rect.min.row + (rect.max.row - rect.min.row) / 2, rect.min.col + (rect.max.col - rect.min.col) / 2);
		int num3 = center.col + num2 / 2;
		int num4 = center.col - num2 / 2;
		for (int i = center.row - num / 2; i <= center.row + num / 2; i++)
		{
			if (!level.Nodes[i, num3].HasLadderAnySize(LevelDirection.East))
			{
				LevelNode neighbour = level.Nodes[i, num3].GetNeighbour(level.Nodes, LevelDirection.East);
				if (neighbour == null || !neighbour.HasLadderAnySize(LevelDirection.West))
				{
					level.Nodes[i, num3].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.Wall);
				}
			}
			if (!level.Nodes[i, num4].HasLadderAnySize(LevelDirection.West))
			{
				LevelNode neighbour2 = level.Nodes[i, num4].GetNeighbour(level.Nodes, LevelDirection.West);
				if (neighbour2 == null || !neighbour2.HasLadderAnySize(LevelDirection.East))
				{
					level.Nodes[i, num4].SetNodeEdgeValue(LevelDirection.West, LevelNodeEdgeValue.Wall);
				}
			}
		}
		int num5 = center.row + num / 2;
		int num6 = center.row - num / 2;
		for (int j = center.col - num2 / 2; j <= center.col + num2 / 2; j++)
		{
			if (!level.Nodes[num5, j].HasLadderAnySize(LevelDirection.North))
			{
				LevelNode neighbour3 = level.Nodes[num5, j].GetNeighbour(level.Nodes, LevelDirection.North);
				if (neighbour3 == null || !neighbour3.HasLadderAnySize(LevelDirection.South))
				{
					level.Nodes[num5, j].SetNodeEdgeValue(LevelDirection.North, LevelNodeEdgeValue.Wall);
				}
			}
			if (!level.Nodes[num6, j].HasLadderAnySize(LevelDirection.South))
			{
				LevelNode neighbour4 = level.Nodes[num6, j].GetNeighbour(level.Nodes, LevelDirection.South);
				if (neighbour4 == null || !neighbour4.HasLadderAnySize(LevelDirection.North))
				{
					level.Nodes[num6, j].SetNodeEdgeValue(LevelDirection.South, LevelNodeEdgeValue.Wall);
				}
			}
		}
		MakeDoorOpenings(level, center, num, num2);
		pathFromStart = pf.GetPathIgnoreDoors(level.Nodes[level.StartPoint.row, level.StartPoint.col], level.Nodes[rect.min.row, rect.min.col], null) != null;
		pathToEnd = pf.GetPathIgnoreDoors(level.Nodes[level.EndPoint.row, level.EndPoint.col], level.Nodes[rect.min.row, rect.min.col], null) != null;
		if (pathFromStart && pathToEnd)
		{
			bool flag = false;
			bool flag2 = false;
			if (num >= 5)
			{
				flag = true;
			}
			if (num2 >= 5)
			{
				flag2 = true;
			}
			if (flag && !flag2)
			{
				int num7 = Random.Range(-1, 2);
				int num8 = Random.Range(-1, 2);
				int num9 = center.row + num7;
				LevelDirection value = ((num7 > 0) ? LevelDirection.South : LevelDirection.North);
				for (int k = center.col - num2 / 2; k <= center.col + num2 / 2; k++)
				{
					level.Nodes[num9, k].SetNodeEdgeValue(value, LevelNodeEdgeValue.Wall);
				}
				level.Nodes[num9, center.col + num8].SetNodeEdgeValue(value, LevelNodeEdgeValue.Empty);
				LevelNode node = level.Nodes[center.row - num / 2, center.col - num2 / 2];
				LevelNode node2 = level.Nodes[center.row - num / 2, center.col + num8];
				LevelNode node3 = level.Nodes[center.row + num / 2, center.col + num8];
				LevelNode node4 = level.Nodes[center.row + num / 2, center.col + num2 / 2];
				int agentId = level.ChunkDB.RegisterAIAgent(type, node, level.Nodes, AIPathMode.PingPong);
				level.ChunkDB.RegisterAIPathWaypoint(agentId, node, Random.Range(0.5f, 1.5f), level.Nodes);
				level.ChunkDB.RegisterAIPathWaypoint(agentId, node2, 0f, level.Nodes);
				level.ChunkDB.RegisterAIPathWaypoint(agentId, node3, 0f, level.Nodes);
				level.ChunkDB.RegisterAIPathWaypoint(agentId, node4, Random.Range(0.5f, 1.5f), level.Nodes);
			}
			else if (flag2 && !flag)
			{
				int num10 = Random.Range(-1, 2);
				int num11 = Random.Range(-1, 2);
				int num12 = center.col + num10;
				LevelDirection value2 = ((num10 <= 0) ? LevelDirection.East : LevelDirection.West);
				for (int l = center.row - num / 2; l <= center.row + num / 2; l++)
				{
					level.Nodes[l, num12].SetNodeEdgeValue(value2, LevelNodeEdgeValue.Wall);
				}
				level.Nodes[center.row + num11, num12].SetNodeEdgeValue(value2, LevelNodeEdgeValue.Empty);
				LevelNode levelNode = level.Nodes[center.row - num / 2, center.col - num2 / 2];
				LevelNode node5 = level.Nodes[center.row + num11, center.col - num2 / 2];
				LevelNode node6 = level.Nodes[center.row + num11, center.col + num2 / 2];
				LevelNode node7 = level.Nodes[center.row + num / 2, center.col + num2 / 2];
				levelNode.AddObjectType(obj, level.ChunkDB, string.Empty);
				int agentId2 = level.ChunkDB.RegisterAIAgent(type, levelNode, level.Nodes, AIPathMode.PingPong);
				level.ChunkDB.RegisterAIPathWaypoint(agentId2, levelNode, Random.Range(0.5f, 1.5f), level.Nodes);
				level.ChunkDB.RegisterAIPathWaypoint(agentId2, node5, 0f, level.Nodes);
				level.ChunkDB.RegisterAIPathWaypoint(agentId2, node6, 0f, level.Nodes);
				level.ChunkDB.RegisterAIPathWaypoint(agentId2, node7, Random.Range(0.5f, 1.5f), level.Nodes);
			}
			else if (flag && flag2)
			{
				int num13 = Random.Range(-1, 2);
				int num14 = Random.Range(-1, 2);
				int num15 = center.col + num13;
				LevelDirection value3 = ((num13 <= 0) ? LevelDirection.East : LevelDirection.West);
				for (int m = center.row - num / 2; m <= center.row + num / 2; m++)
				{
					level.Nodes[m, num15].SetNodeEdgeValue(value3, LevelNodeEdgeValue.Wall);
				}
				level.Nodes[center.row + num14, num15].SetNodeEdgeValue(value3, LevelNodeEdgeValue.Empty);
				int num16 = Random.Range(-1, 2);
				int num17 = num15 - 1;
				int num18 = num15 + 1;
				int num19 = center.row + num16;
				LevelDirection value4 = ((num16 > 0) ? LevelDirection.South : LevelDirection.North);
				for (int n = center.col - num2 / 2; n <= center.col + num2 / 2; n++)
				{
					level.Nodes[num19, n].SetNodeEdgeValue(value4, LevelNodeEdgeValue.Wall);
				}
				level.Nodes[num19, num17].SetNodeEdgeValue(value4, LevelNodeEdgeValue.Empty);
				level.Nodes[num19, num18].SetNodeEdgeValue(value4, LevelNodeEdgeValue.Empty);
				List<LevelNode> list = new List<LevelNode>();
				list.Add(level.Nodes[center.row - num / 2, center.col - num2 / 2]);
				if (num19 < center.row + num14)
				{
					list.Add(level.Nodes[center.row + num14, center.col - num2 / 2]);
					list.Add(level.Nodes[center.row + num14, num18]);
					list.Add(level.Nodes[center.row + num / 2, num18]);
					list.Add(level.Nodes[center.row + num / 2, center.col + num2 / 2]);
				}
				else
				{
					list.Add(level.Nodes[center.row - num / 2, num17]);
					list.Add(level.Nodes[center.row - num / 2, num17]);
					list.Add(level.Nodes[center.row + num14, num17]);
					list.Add(level.Nodes[center.row + num14, num18]);
					list.Add(level.Nodes[center.row + num / 2, num18]);
					list.Add(level.Nodes[center.row + num / 2, center.col + num2 / 2]);
				}
				list[0].AddObjectType(obj, level.ChunkDB, string.Empty);
				int agentId3 = level.ChunkDB.RegisterAIAgent(type, list[0], level.Nodes, AIPathMode.PingPong);
				int num20 = 0;
				foreach (LevelNode item in list)
				{
					if (num20 == 0 || num20 == list.Count - 1)
					{
						level.ChunkDB.RegisterAIPathWaypoint(agentId3, item, Random.Range(0.5f, 1.5f), level.Nodes);
					}
					else
					{
						level.ChunkDB.RegisterAIPathWaypoint(agentId3, item, 0f, level.Nodes);
					}
					num20++;
				}
			}
			level.ChunkDB.interestZones.Add(new InterestZoneData(center, new LevelPoint(num - 1, num2 - 1)));
			HouseChunk houseChunk = (HouseChunk)level.ChunkDB.RegisterNewChunk(level.Nodes.GetLength(0), level.Nodes.GetLength(1), LevelChunkType.House);
			for (int num21 = center.row - num / 2; num21 <= center.row + num / 2; num21++)
			{
				for (int num22 = center.col - num2 / 2; num22 <= center.col + num2 / 2; num22++)
				{
					level.ChunkDB.ResetNode(level.Nodes[num21, num22]);
					houseChunk.AddNode(level.ChunkDB, level.Nodes[num21, num22]);
					level.Nodes[num21, num22].Type = LevelNodeType.House;
				}
			}
		}
		else
		{
			Debug.LogWarning("House is blocking path...destroying house again");
			num3 = center.col + num2 / 2;
			num4 = center.col - num2 / 2;
			for (int num23 = center.row - num / 2; num23 <= center.row + num / 2; num23++)
			{
				if (!level.Nodes[num23, num3].HasLadderAnySize(LevelDirection.East))
				{
					LevelNode neighbour5 = level.Nodes[num23, num3].GetNeighbour(level.Nodes, LevelDirection.East);
					if (neighbour5 == null || !neighbour5.HasLadderAnySize(LevelDirection.West))
					{
						level.Nodes[num23, num3].SetNodeEdgeValue(LevelDirection.East, LevelNodeEdgeValue.Empty);
					}
				}
				if (!level.Nodes[num23, num3].HasLadderAnySize(LevelDirection.West))
				{
					LevelNode neighbour6 = level.Nodes[num23, num3].GetNeighbour(level.Nodes, LevelDirection.West);
					if (neighbour6 == null || !neighbour6.HasLadderAnySize(LevelDirection.East))
					{
						level.Nodes[num23, num4].SetNodeEdgeValue(LevelDirection.West, LevelNodeEdgeValue.Empty);
					}
				}
			}
			num5 = center.row + num / 2;
			num6 = center.row - num / 2;
			for (int num24 = center.col - num2 / 2; num24 <= center.col + num2 / 2; num24++)
			{
				if (!level.Nodes[num5, num24].HasLadderAnySize(LevelDirection.North))
				{
					LevelNode neighbour7 = level.Nodes[num5, num24].GetNeighbour(level.Nodes, LevelDirection.North);
					if (neighbour7 == null || !neighbour7.HasLadderAnySize(LevelDirection.South))
					{
						level.Nodes[num5, num24].SetNodeEdgeValue(LevelDirection.North, LevelNodeEdgeValue.Empty);
					}
				}
				if (!level.Nodes[num6, num24].HasLadderAnySize(LevelDirection.South))
				{
					LevelNode neighbour8 = level.Nodes[num6, num24].GetNeighbour(level.Nodes, LevelDirection.South);
					if (neighbour8 == null || !neighbour8.HasLadderAnySize(LevelDirection.North))
					{
						level.Nodes[num6, num24].SetNodeEdgeValue(LevelDirection.South, LevelNodeEdgeValue.Empty);
					}
				}
			}
		}
		return 0;
	}

	private void MakeDoorOpenings(Level level, LevelPoint center, int houseRows, int houseCols)
	{
		int num = WallOpeningScore(level, center, houseRows, houseCols, LevelDirection.North);
		int num2 = WallOpeningScore(level, center, houseRows, houseCols, LevelDirection.South);
		int num3 = WallOpeningScore(level, center, houseRows, houseCols, LevelDirection.East);
		int num4 = WallOpeningScore(level, center, houseRows, houseCols, LevelDirection.West);
		if (num > num2)
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.North);
		}
		else if (num2 > num)
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.South);
		}
		else if (Random.value < 0.5f)
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.North);
		}
		else
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.South);
		}
		if (num3 > num4)
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.East);
		}
		else if (num4 > num3)
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.West);
		}
		else if (Random.value < 0.5f)
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.East);
		}
		else
		{
			MakeWallOpening(level, center, houseRows, houseCols, LevelDirection.West);
		}
	}

	private int WallOpeningScore(Level level, LevelPoint center, int houseRows, int houseCols, LevelDirection dir)
	{
		int elevation = level.Nodes[center.row, center.col].Elevation;
		int num = 0;
		int num2 = 0;
		if (dir == LevelDirection.East || dir == LevelDirection.West)
		{
			int num3 = ((dir != LevelDirection.East) ? (center.col - houseCols / 2 - 1) : (center.col + houseCols / 2 + 1));
			for (int i = center.row - houseRows / 2; i <= center.row + houseRows / 2; i++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(i, num3), level.Nodes) && level.Nodes[i, num3].Elevation == elevation)
				{
					num2++;
				}
				num++;
			}
		}
		if (dir == LevelDirection.North || dir == LevelDirection.South)
		{
			int num4 = ((dir != LevelDirection.North) ? (center.row - houseRows / 2 - 1) : (center.row + houseRows / 2 + 1));
			for (int j = center.col - houseCols / 2; j <= center.col + houseCols / 2; j++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num4, j), level.Nodes) && level.Nodes[num4, j].Elevation == elevation)
				{
					num2++;
				}
				num++;
			}
		}
		return Mathf.RoundToInt((float)(num2 / num) * 100f);
	}

	private void MakeWallOpening(Level level, LevelPoint center, int houseRows, int houseCols, LevelDirection dir)
	{
		int elevation = level.Nodes[center.row, center.col].Elevation;
		bool flag = false;
		int num;
		switch (dir)
		{
		case LevelDirection.East:
		case LevelDirection.West:
		{
			int num2 = ((dir != LevelDirection.East) ? (center.col - houseCols / 2) : (center.col + houseCols / 2));
			for (int i = center.row; i <= center.row + houseRows / 2 - 1; i++)
			{
				if (level.Nodes[i, num2].Elevation == elevation)
				{
					level.Nodes[i, num2].SetNodeEdgeValue(dir, LevelNodeEdgeValue.Empty);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return;
			}
			for (int num3 = center.row; num3 >= center.row - houseRows / 2 + 1; num3--)
			{
				if (level.Nodes[num3, num2].Elevation == elevation)
				{
					level.Nodes[num3, num2].SetNodeEdgeValue(dir, LevelNodeEdgeValue.Empty);
					flag = true;
					break;
				}
			}
			return;
		}
		case LevelDirection.North:
			num = center.row + houseRows / 2;
			break;
		default:
			num = center.row - houseRows / 2;
			break;
		}
		for (int j = center.col; j <= center.col + houseCols / 2 - 1; j++)
		{
			if (level.Nodes[num, j].Elevation == elevation)
			{
				level.Nodes[num, j].SetNodeEdgeValue(dir, LevelNodeEdgeValue.Empty);
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		for (int num4 = center.col; num4 >= center.col - houseCols / 2 + 1; num4--)
		{
			if (level.Nodes[num, num4].Elevation == elevation)
			{
				level.Nodes[num, num4].SetNodeEdgeValue(dir, LevelNodeEdgeValue.Empty);
				flag = true;
				break;
			}
		}
	}
}
