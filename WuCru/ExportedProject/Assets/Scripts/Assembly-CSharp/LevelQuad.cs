using System.Collections.Generic;
using UnityEngine;

public class LevelQuad
{
	public LevelPoint min;

	public LevelPoint max;

	public Vector3 center;

	public List<LevelNode> nodes = new List<LevelNode>();

	public int numLoot;

	public bool pathFromStartExists;

	public LevelQuad(int row, int col, int height, int width, LevelNode[,] levelNodes, PathFinding pf, LevelPoint startPoint)
	{
		min = new LevelPoint(row, col);
		max = new LevelPoint(row + height - 1, col + width - 1);
		center = new Vector3((float)col + (float)width / 2f - 0.5f, 0f, (float)row + (float)height / 2f - 0.5f);
		for (int i = min.row; i <= max.row; i++)
		{
			for (int j = min.col; j <= max.col; j++)
			{
				if (LevelHelper.PointInsideBounds(i, j, levelNodes) && levelNodes[i, j].Type != LevelNodeType.None && levelNodes[i, j].Elevation > 0)
				{
					nodes.Add(levelNodes[i, j]);
					if (!pathFromStartExists && pf.GetPathIgnoreDoors(levelNodes[startPoint.row, startPoint.col], levelNodes[i, j], null) != null)
					{
						pathFromStartExists = true;
					}
				}
			}
		}
	}

	public bool CanHoldLoot(LevelPlacementHelper placementHelper, LevelNodeObjectType type)
	{
		foreach (LevelNode node in nodes)
		{
			if (placementHelper.EvaluateNode(node, type))
			{
				return true;
			}
		}
		return false;
	}
}
