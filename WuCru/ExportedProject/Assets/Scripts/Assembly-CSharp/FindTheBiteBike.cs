using System.Collections.Generic;

public class FindTheBiteBike : GameplayPattern
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
		LevelNode levelNode3 = level.Nodes[levelNode.Row - 3, levelNode.Col - 2];
		levelNode3.AddObjectType(LevelNodeObjectType.BiteBike, level.ChunkDB, string.Empty);
		return 0;
	}
}
