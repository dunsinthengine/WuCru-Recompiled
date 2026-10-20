using System.Collections.Generic;
using UnityEngine;

public class FindTheHoloLog : GameplayPattern
{
	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.FindTheHoloLog;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		int num = 0;
		if (rect.min.col > level.Nodes.GetLength(1) - 11)
		{
			num += CalcUsageScore(rect, usedRects);
			num += CalcIsChunkTypeScore(level, rect, LevelChunkType.EndLevelArea, 200f);
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		Debug.Log(string.Concat(PatternType, " Setting up on rect"));
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
				goto IL_005b;
			}
			continue;
			IL_005b:
			levelNode = levelNode2;
			break;
		}
		LevelNode levelNode3 = level.Nodes[levelNode.Row, levelNode.Col - 3];
		levelNode3.AddObjectType(LevelNodeObjectType.HoloLog, level.ChunkDB, string.Empty);
		Debug.Log("Apparently added the holo log");
		return 0;
	}
}
