using System.Collections.Generic;

public class SamuraiXCave : GameplayPattern
{
	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.SamuraiXCave;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		int num = 0;
		if (rect.width >= 3 && rect.height >= 3 && rect.min.col > level.Nodes.GetLength(1) - 11)
		{
			num += CalcUsageScore(rect, usedRects);
			num += CalcIsChunkTypeScore(level, rect, LevelChunkType.EndLevelArea, 200f);
		}
		return num;
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
		LevelNode levelNode3 = level.Nodes[levelNode.Row, levelNode.Col - 4];
		levelNode3.AddObjectType(LevelNodeObjectType.CaveEntrance, level.ChunkDB, string.Empty);
		return 0;
	}
}
