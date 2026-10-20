using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss : GameplayPattern
{
	private int aiAgentGameplayPointCost = 5;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.ElementalBoss;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
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
		if (!flag && rect.width >= 6 && rect.height >= 3 && rect.min.col > 8 && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			num += CalcUsageScore(rect, usedRects);
			num += CalcScoreFromValueMatch(10, rect.width);
			num += CalcScoreFromValueMatch(6, rect.height);
			num += Mathf.RoundToInt(rect.GetLevelNodeTypeFactor(LevelNodeType.MainPath) * 10f);
			num += Mathf.RoundToInt(rect.GetLevelNodeTypeFactor(LevelNodeType.SideQuest) * 10f);
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		Debug.Log(string.Concat(PatternType, " Setting up on rect"));
		if (rect == null)
		{
			Debug.LogError("Rect is null - can't setup gameplay");
			return 0;
		}
		int row = rect.min.row;
		int row2 = rect.max.row;
		int col = rect.min.col;
		int col2 = rect.max.col;
		row = (rect.max.row + rect.min.row) / 2;
		if (row + 2 <= rect.max.row)
		{
			row2 = row + 2;
		}
		else if (row + 1 <= rect.max.row)
		{
			row2 = row + 1;
		}
		else
		{
			row2 = row;
			row--;
		}
		col = (rect.max.col + rect.min.col) / 2;
		if (col + 2 <= rect.max.col)
		{
			col2 = col + 2;
		}
		else if (col + 1 <= rect.max.col)
		{
			col2 = col + 1;
		}
		else
		{
			col2 = col;
			col--;
		}
		AIWaypoint waypoint = new AIWaypoint(level.Nodes[row2, col2], 0f);
		AIWaypoint waypoint2 = new AIWaypoint(level.Nodes[row, col2], 0f);
		AIWaypoint waypoint3 = new AIWaypoint(level.Nodes[row, col], 0f);
		AIWaypoint waypoint4 = new AIWaypoint(level.Nodes[row2, col], 0f);
		AIAgentType type = AIAgentType.ElementalBoss;
		LevelNodeObjectType obj = LevelNodeObjectType.ElementalBossEnemy;
		level.Nodes[row2, col2].AddObjectType(obj, level.ChunkDB, string.Empty);
		int agentId = level.ChunkDB.RegisterAIAgent(type, level.Nodes[row2, col2], level.Nodes, AIPathMode.Cycle);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint2, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint3, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint4, level.Nodes);
		return aiAgentGameplayPointCost + rect.width;
	}
}
