using System.Collections.Generic;
using UnityEngine;

public class ChokePoint
{
	public LevelPoint startPoint;

	public LevelDirection dir;

	public int numNodes;

	public LevelPlateauRect rect;

	public LevelNode[] GetNodes(LevelNode[,] nodes)
	{
		List<LevelNode> list = new List<LevelNode>();
		if (dir == LevelDirection.North)
		{
			int col = startPoint.col;
			for (int i = startPoint.row; i < startPoint.row + numNodes; i++)
			{
				list.Add(nodes[i, col]);
			}
		}
		else if (dir == LevelDirection.East)
		{
			int row = startPoint.row;
			for (int j = startPoint.col; j < startPoint.col + numNodes; j++)
			{
				list.Add(nodes[row, j]);
			}
		}
		else
		{
			Debug.LogError("Only North and East supported");
		}
		return list.ToArray();
	}
}
