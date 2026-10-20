using System.Collections.Generic;
using UnityEngine;

public class DistractionWithBushes : GameplayPattern
{
	private int bushGameplayPointCost = 3;

	private int distractionGameplayPointCost = 5;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.DistractionWithBushes;
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
		if (!flag && rect.min.col > 5 && rect.width >= 3 && rect.height >= 3 && level.Nodes[rect.row + rect.height / 2, rect.col + rect.width / 2].ObjectTypes.Count == 0 && (!isEndLevel || rect.col + rect.width < level.Nodes.GetLength(1) - 11))
		{
			num += CalcUsageScore(rect, usedRects);
			num += Mathf.RoundToInt(rect.GetLevelNodeTypeFactor(LevelNodeType.MainPath) * 10f);
			num += Mathf.RoundToInt(rect.GetLevelNodeTypeFactor(LevelNodeType.SideQuest) * 10f);
			num += CalcRectObjectDensityScore(level, rect, 25f);
			num += CalcRectStartOfLevelScore(rect, level, 20f);
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		int num = rect.row + rect.height / 2;
		int num2 = rect.col + rect.width / 2;
		if (!level.PlacementHelper.EvaluateNode(level.Nodes[num, num2], LevelNodeObjectType.BellTower))
		{
			LevelNode closestFreeNode = level.PlacementHelper.GetClosestFreeNode(level.Nodes[num, num2], LevelNodeObjectType.BellTower, 1);
			if (closestFreeNode == null)
			{
				Debug.LogWarning("Cannot place distraction-gong...will not setup pattern");
				return 0;
			}
			num = closestFreeNode.Row;
			num2 = closestFreeNode.Col;
		}
		level.Nodes[num, num2].AddObjectType(LevelNodeObjectType.BellTower, level.ChunkDB, string.Empty);
		int num3 = 0;
		for (int i = 0; i < 4; i++)
		{
			LevelNode levelNode = null;
			int num4 = 100;
			do
			{
				levelNode = rect.plateau.nodes[Random.Range(0, rect.plateau.nodes.Count)];
				num4--;
			}
			while (!level.PlacementHelper.EvaluateNode(levelNode, LevelNodeObjectType.Bush) && LevelHelper.Distance(level.Nodes[num, num2], levelNode) > 4f && num4 > 0);
			if (levelNode != null && level.PlacementHelper.EvaluateNode(levelNode, LevelNodeObjectType.Bush))
			{
				levelNode.AddObjectType(LevelNodeObjectType.Bush, level.ChunkDB, string.Empty);
				num3++;
			}
		}
		return distractionGameplayPointCost + num3 * bushGameplayPointCost;
	}
}
