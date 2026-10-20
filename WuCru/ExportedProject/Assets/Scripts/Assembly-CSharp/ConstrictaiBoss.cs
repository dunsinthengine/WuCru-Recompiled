using System.Collections.Generic;

public class ConstrictaiBoss : GameplayPattern
{
	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.ConstrictaiBoss;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		return 1;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		LevelNode levelNode = null;
		LevelNode[,] nodes = level.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			LevelNode levelNode2;
			while (num < length2)
			{
				levelNode2 = nodes[i, num];
				if (!levelNode2.ContainsObjectType(LevelNodeObjectType.EndFlag))
				{
					num++;
					continue;
				}
				goto IL_0041;
			}
			continue;
			IL_0041:
			levelNode = levelNode2;
			break;
		}
		if (levelNode != null)
		{
			AIWaypoint waypoint = new AIWaypoint(level.Nodes[levelNode.Row + 3, levelNode.Col - 3], 0f);
			AIWaypoint waypoint2 = new AIWaypoint(level.Nodes[levelNode.Row + 3, levelNode.Col + 3], 0f);
			AIWaypoint waypoint3 = new AIWaypoint(level.Nodes[levelNode.Row - 3, levelNode.Col + 3], 0f);
			AIWaypoint waypoint4 = new AIWaypoint(level.Nodes[levelNode.Row - 3, levelNode.Col - 3], 0f);
			level.Nodes[levelNode.Row + 3, levelNode.Col - 3].AddObjectType(LevelNodeObjectType.ConstrictaiBossEnemy, level.ChunkDB, string.Empty);
			int agentId = level.ChunkDB.RegisterAIAgent(AIAgentType.ConstrictaiBossEnemy, level.Nodes[levelNode.Row + 3, levelNode.Col - 3], level.Nodes, AIPathMode.Cycle);
			level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint, level.Nodes);
			level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint2, level.Nodes);
			level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint3, level.Nodes);
			level.ChunkDB.RegisterAIPathWaypoint(agentId, waypoint4, level.Nodes);
		}
		return 20;
	}
}
