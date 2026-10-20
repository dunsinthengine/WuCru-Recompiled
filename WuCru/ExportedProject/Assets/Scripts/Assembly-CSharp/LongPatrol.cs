using System.Collections.Generic;
using UnityEngine;

public class LongPatrol : GameplayPattern
{
	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.LongPatrol;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNodes, Level level, bool isEndLevel)
	{
		int num = 0;
		if ((rect.width >= 4 || rect.height >= 4) && (level.Nodes[level.StartPoint.row, level.StartPoint.col].Elevation != rect.nodes[0].Elevation || rect.min.col > 6) && (!isEndLevel || rect.max.col < level.Nodes.GetLength(1) - 10))
		{
			num += CalcIsChunkTypeScore(level, rect, LevelChunkType.EllipseRoom, 100f);
			num += rect.width * rect.height;
			num += CalcRectObjectDensityScore(level, rect, 50f);
			LevelNode[] patrolPathWaypoints = GetPatrolPathWaypoints(rect, level, 4, 8, 6);
			num = ((patrolPathWaypoints != null) ? (num + patrolPathWaypoints.Length) : 0);
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		LevelNode[] patrolPathWaypoints = GetPatrolPathWaypoints(rect, level, 4, 8, 6);
		if (patrolPathWaypoints != null && patrolPathWaypoints.Length >= 2)
		{
			AIAgentType randomEnemyType = GetRandomEnemyType();
			LevelNodeObjectType levelNodeTypeForEnemy = GetLevelNodeTypeForEnemy(randomEnemyType);
			patrolPathWaypoints[0].AddObjectType(levelNodeTypeForEnemy, level.ChunkDB, string.Empty);
			int agentId = level.ChunkDB.RegisterAIAgent(randomEnemyType, patrolPathWaypoints[0], level.Nodes, AIPathMode.PingPong);
			int num = 0;
			LevelNode[] array = patrolPathWaypoints;
			foreach (LevelNode node in array)
			{
				float restTime = 0f;
				if (num == 0 || num == patrolPathWaypoints.Length - 1)
				{
					restTime = Random.Range(0.8f, 2f);
				}
				level.ChunkDB.RegisterAIPathWaypoint(agentId, new AIWaypoint(node, restTime), level.Nodes);
				num++;
			}
			return 12;
		}
		Debug.LogWarning(string.Concat(PatternType, ": path is null or a single point"));
		return 0;
	}
}
