using System.Collections.Generic;
using UnityEngine;

public class MiniStronghold : GameplayPattern
{
	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.MiniStronghold;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		int num = 0;
		bool flag = false;
		bool flag2 = true;
		bool flag3 = false;
		foreach (LevelPlateauRect houseRect in houseRects)
		{
			if (houseRect.Intersect(rect))
			{
				flag = true;
				break;
			}
		}
		if (!flag && rect.width >= 5 && rect.height >= 5 && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			LevelPoint levelPoint = new LevelPoint(rect.min.row + rect.height / 2, rect.min.col + rect.width / 2);
			bool flag4 = true;
			for (int i = levelPoint.row - 1; i <= levelPoint.row + 1; i++)
			{
				for (int j = levelPoint.col - 1; j <= levelPoint.col + 1; j++)
				{
					if (level.Nodes[i, j].HasWallAnyTypeAnyDirection())
					{
						flag4 = false;
						break;
					}
				}
			}
			if (flag4)
			{
				for (int k = levelPoint.row - 1; k <= levelPoint.row + 1; k++)
				{
					if (!level.PlacementHelper.EvaluateNodeEdge(level.Nodes[k, levelPoint.col + 1], LevelNodeEdgeValue.AltWall, LevelDirection.East))
					{
						flag4 = false;
						break;
					}
					if (!level.PlacementHelper.EvaluateNodeEdge(level.Nodes[k, levelPoint.col - 1], LevelNodeEdgeValue.AltWall, LevelDirection.West))
					{
						flag4 = false;
						break;
					}
				}
				for (int l = levelPoint.col - 1; l <= levelPoint.col + 1; l++)
				{
					if (!level.PlacementHelper.EvaluateNodeEdge(level.Nodes[levelPoint.row - 1, l], LevelNodeEdgeValue.AltWall, LevelDirection.South))
					{
						flag4 = false;
						break;
					}
					if (!level.PlacementHelper.EvaluateNodeEdge(level.Nodes[levelPoint.row + 1, l], LevelNodeEdgeValue.AltWall, LevelDirection.North))
					{
						flag4 = false;
						break;
					}
				}
				for (int m = levelPoint.row - 1; m <= levelPoint.row + 1; m++)
				{
					for (int n = levelPoint.col - 1; n <= levelPoint.col + 1; n++)
					{
						LevelNode levelNode = level.Nodes[m, n];
						LevelChunk chunk = level.ChunkDB.GetChunk(levelNode.ChunkId);
						if (chunk.ChunkType == LevelChunkType.EllipseRoom || chunk.ChunkType == LevelChunkType.RectangleRoom || chunk.ChunkType == LevelChunkType.EndLevelArea)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					LevelNode levelNode2 = level.Nodes[levelPoint.row, levelPoint.col];
					if (levelNode2.ObjectTypes.Count > 0 && (levelNode2.ContainsObjectType(LevelNodeObjectType.CampFire) || levelNode2.ContainsObjectType(LevelNodeObjectType.DoorTrigger) || levelNode2.ContainsObjectType(LevelNodeObjectType.EndFlag) || levelNode2.ContainsObjectType(LevelNodeObjectType.GruntEnemy) || levelNode2.ContainsObjectType(LevelNodeObjectType.GoldChest) || levelNode2.ContainsObjectType(LevelNodeObjectType.Key) || levelNode2.ContainsObjectType(LevelNodeObjectType.MetalChest) || levelNode2.ContainsObjectType(LevelNodeObjectType.WoodChest)))
					{
						flag2 = false;
					}
				}
			}
			if (flag4 && !flag3 && flag2)
			{
				num += CalcUsageScore(rect, usedRects);
				num += CalcRectObjectDensityScore(level, rect, 100f);
				if (rect.min.col > level.Nodes.GetLength(1) / 3)
				{
					num += 50;
				}
				if (rect.min.col > level.Nodes.GetLength(1) / 3 * 2)
				{
					num += 50;
				}
			}
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		LevelPoint center = new LevelPoint(rect.min.row + rect.height / 2, rect.min.col + rect.width / 2);
		level.Nodes[center.row, center.col].AddObjectType(LevelNodeObjectType.GoldChest, level.ChunkDB, "guarded");
		for (int i = center.row - 1; i <= center.row + 1; i++)
		{
			level.Nodes[i, center.col + 1].EastEdgeValue = LevelNodeEdgeValue.AltWall;
			level.Nodes[i, center.col - 1].WestEdgeValue = LevelNodeEdgeValue.AltWall;
		}
		for (int j = center.col - 1; j <= center.col + 1; j++)
		{
			level.Nodes[center.row - 1, j].SouthEdgeValue = LevelNodeEdgeValue.AltWall;
			level.Nodes[center.row + 1, j].NorthEdgeValue = LevelNodeEdgeValue.AltWall;
		}
		level.ChunkDB.interestZones.Add(new InterestZoneData(center, new LevelPoint(2, 2)));
		if (Random.value < 0.5f)
		{
			level.Nodes[center.row + 1, center.col].NorthEdgeValue = LevelNodeEdgeValue.Empty;
		}
		else
		{
			level.Nodes[center.row - 1, center.col].SouthEdgeValue = LevelNodeEdgeValue.Empty;
		}
		if (Random.value < 0.5f)
		{
			level.Nodes[center.row, center.col + 1].EastEdgeValue = LevelNodeEdgeValue.Empty;
		}
		else
		{
			level.Nodes[center.row, center.col - 1].WestEdgeValue = LevelNodeEdgeValue.Empty;
		}
		AIAgentType type = AIAgentType.WarriorEnemy;
		LevelNodeObjectType obj = LevelNodeObjectType.WarriorEnemy;
		if (GameController.Instance.PlayerProfile.Level < 2)
		{
			type = AIAgentType.ScoutEnemy;
			obj = LevelNodeObjectType.ScoutEnemy;
		}
		level.Nodes[center.row - 2, center.col - 2].AddObjectType(obj, level.ChunkDB, string.Empty);
		int agentId = level.ChunkDB.RegisterAIAgent(type, level.Nodes[center.row - 2, center.col - 2], level.Nodes, AIPathMode.PingPong);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row - 2, center.col - 2], 2f, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row - 2, center.col + 2], 2f, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row + 2, center.col + 2], 2f, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row + 2, center.col - 2], 2f, level.Nodes);
		level.Nodes[center.row + 2, center.col + 2].AddObjectType(obj, level.ChunkDB, string.Empty);
		agentId = level.ChunkDB.RegisterAIAgent(type, level.Nodes[center.row + 2, center.col + 2], level.Nodes, AIPathMode.PingPong);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row + 2, center.col + 2], 2f, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row + 2, center.col - 2], 2f, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row - 2, center.col - 2], 2f, level.Nodes);
		level.ChunkDB.RegisterAIPathWaypoint(agentId, level.Nodes[center.row - 2, center.col + 2], 2f, level.Nodes);
		return 30;
	}
}
