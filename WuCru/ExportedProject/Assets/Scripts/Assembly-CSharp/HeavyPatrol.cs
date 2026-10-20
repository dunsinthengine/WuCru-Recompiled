using System.Collections.Generic;
using UnityEngine;

public class HeavyPatrol : GameplayPattern
{
	private int aiAgentGameplayPointCost = 5;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.HeavyPatrol;
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
		if (rect == null)
		{
			Debug.LogError("Rect is null - can't setup gameplay");
			return 0;
		}
		AIWaypoint waypoint = new AIWaypoint(level.Nodes[rect.max.row, rect.max.col], 0f);
		AIWaypoint waypoint2 = new AIWaypoint(level.Nodes[rect.min.row, rect.max.col], 0f);
		AIWaypoint waypoint3 = new AIWaypoint(level.Nodes[rect.min.row, rect.min.col], 0f);
		AIWaypoint waypoint4 = new AIWaypoint(level.Nodes[rect.max.row, rect.min.col], 0f);
		AIAgentType type = AIAgentType.GruntEnemy;
		LevelNodeObjectType obj = LevelNodeObjectType.GruntEnemy;
		if (GameController.Instance.PlayerProfile.Level < 2)
		{
			type = AIAgentType.ScoutEnemy;
			obj = LevelNodeObjectType.ScoutEnemy;
		}
		level.Nodes[rect.max.row, rect.min.col].AddObjectType(obj, level.ChunkDB, "heavyPatrol");
		int agentId = level.ChunkDB.RegisterAIAgent(type, level.Nodes[rect.max.row, rect.min.col], level.Nodes, AIPathMode.Cycle);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint2, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint3, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint4, level.Nodes);
		level.Nodes[rect.max.row, rect.min.col + 1].AddObjectType(obj, level.ChunkDB, "heavyPatrol");
		int agentId2 = level.ChunkDB.RegisterAIAgent(type, level.Nodes[rect.max.row, rect.min.col + 1], level.Nodes, AIPathMode.Cycle);
		level.ChunkDB.RegisterAIPathWaypoint(agentId2, waypoint, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId2, waypoint2, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId2, waypoint3, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId2, waypoint4, level.Nodes);
		level.Nodes[rect.max.row, rect.min.col + 2].AddObjectType(obj, level.ChunkDB, "heavyPatrol");
		int agentId3 = level.ChunkDB.RegisterAIAgent(type, level.Nodes[rect.max.row, rect.min.col + 2], level.Nodes, AIPathMode.Cycle);
		level.ChunkDB.RegisterAIPathWaypoint(agentId3, waypoint, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId3, waypoint2, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId3, waypoint3, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId3, waypoint4, level.Nodes);
		return 3 * aiAgentGameplayPointCost + 3 * rect.width;
	}
}
