using System.Collections.Generic;
using UnityEngine;

public class Minefield : GameplayPattern
{
	private int mineGameplayPointCost = 3;

	private int distractiopnGameplayPointCost = 5;

	private int maxMines = 10;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.Minefield;
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
		if (!flag && rect.width >= 3 && rect.height >= 3 && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			num += CalcUsageScore(rect, usedRects);
			num += CalcScoreFromValueMatch(8, rect.width);
			num += CalcScoreFromValueMatch(5, rect.height);
			if (rect.max.col < level.Nodes.GetLength(1) / 3 * 2)
			{
				num += 50;
			}
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		int num = 0;
		List<LevelNode> list = new List<LevelNode>();
		foreach (LevelNode node in rect.nodes)
		{
			if (level.PlacementHelper.EvaluateNode(node, LevelNodeObjectType.BearTrap))
			{
				list.Add(node);
			}
		}
		float num2 = Mathf.Min((float)maxMines / (float)list.Count, 0.4f);
		foreach (LevelNode item in list)
		{
			if (Random.value <= num2)
			{
				item.AddObjectType(LevelNodeObjectType.BearTrap, level.ChunkDB, string.Empty);
				num++;
			}
			if (num >= maxMines)
			{
				break;
			}
		}
		LevelNode levelNode = null;
		int num3 = 100;
		do
		{
			levelNode = rect.nodes[Random.Range(0, rect.nodes.Count)];
			num3--;
		}
		while (!level.PlacementHelper.EvaluateNode(levelNode, LevelNodeObjectType.BellTower) && num3 > 0);
		if (levelNode != null && num3 > 0)
		{
			levelNode.AddObjectType(LevelNodeObjectType.BellTower, level.ChunkDB, string.Empty);
		}
		return num * mineGameplayPointCost + ((levelNode != null && num3 > 0) ? distractiopnGameplayPointCost : 0);
	}
}
