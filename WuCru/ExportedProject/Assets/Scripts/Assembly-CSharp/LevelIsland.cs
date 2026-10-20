using UnityEngine;

public class LevelIsland
{
	public LevelNode[] islandNodes;

	public int pathDistToStart;

	public int pathDistToEnd;

	public LevelNodeType originalType;

	public bool isConnected;

	public bool ConnectToLevel(LevelEngine engine)
	{
		isConnected = true;
		int num = 0;
		LevelNode levelNode = null;
		LevelNode[] array = islandNodes;
		foreach (LevelNode levelNode2 in array)
		{
			int num2 = CalcNodeScore(levelNode2, engine.Nodes);
			if (num2 > num)
			{
				num = num2;
				levelNode = levelNode2;
			}
		}
		if (levelNode != null)
		{
			LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.North, engine.Nodes, true);
			LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.South, engine.Nodes, true);
			LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.East, engine.Nodes, true);
			LevelNode neighbourAt4 = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.West, engine.Nodes, true);
			if (neighbourAt != null && neighbourAt.Elevation > levelNode.Elevation)
			{
				int num3 = Mathf.Abs(levelNode.Elevation - neighbourAt.Elevation);
				if (num3 > 0 && num3 <= 4 && levelNode.NorthEdgeValue == LevelNodeEdgeValue.Empty)
				{
					PlaceLadder(levelNode, LevelDirection.North, num3);
					return true;
				}
			}
			if (neighbourAt2 != null && neighbourAt2.Elevation < levelNode.Elevation)
			{
				int num4 = Mathf.Abs(levelNode.Elevation - neighbourAt2.Elevation);
				if (num4 > 0 && num4 <= 4 && neighbourAt2.NorthEdgeValue == LevelNodeEdgeValue.Empty)
				{
					PlaceLadder(neighbourAt2, LevelDirection.North, num4);
					return true;
				}
			}
			if (neighbourAt4 != null)
			{
				int num5 = Mathf.Abs(levelNode.Elevation - neighbourAt4.Elevation);
				if (num5 > 0 && num5 <= 4)
				{
					if (levelNode.Elevation > neighbourAt4.Elevation && neighbourAt4.EastEdgeValue == LevelNodeEdgeValue.Empty)
					{
						PlaceLadder(neighbourAt4, LevelDirection.East, num5);
						return true;
					}
					if (levelNode.Elevation < neighbourAt4.Elevation && levelNode.WestEdgeValue == LevelNodeEdgeValue.Empty)
					{
						PlaceLadder(levelNode, LevelDirection.West, num5);
						return true;
					}
				}
			}
			if (neighbourAt3 != null)
			{
				int num6 = Mathf.Abs(levelNode.Elevation - neighbourAt3.Elevation);
				if (num6 > 0 && num6 <= 4)
				{
					if (levelNode.Elevation > neighbourAt3.Elevation && neighbourAt3.WestEdgeValue == LevelNodeEdgeValue.Empty)
					{
						PlaceLadder(neighbourAt3, LevelDirection.West, num6);
						return true;
					}
					if (levelNode.Elevation < neighbourAt3.Elevation && levelNode.EastEdgeValue == LevelNodeEdgeValue.Empty)
					{
						PlaceLadder(levelNode, LevelDirection.East, num6);
						return true;
					}
				}
			}
		}
		isConnected = false;
		return false;
	}

	private void PlaceLadder(LevelNode node, LevelDirection dir, int eDif)
	{
		switch (eDif)
		{
		case 1:
			node.SetNodeEdgeValue(dir, LevelNodeEdgeValue.Ladder);
			isConnected = true;
			break;
		case 2:
			node.SetNodeEdgeValue(dir, LevelNodeEdgeValue.DoubleLadder);
			isConnected = true;
			break;
		case 3:
			node.SetNodeEdgeValue(dir, LevelNodeEdgeValue.DownDrain);
			isConnected = true;
			break;
		case 4:
			node.SetNodeEdgeValue(dir, LevelNodeEdgeValue.Tree);
			isConnected = true;
			break;
		default:
			Debug.LogError("Cannot connect island, as elevation dif is too high or too low");
			break;
		}
	}

	private int CalcNodeScore(LevelNode node, LevelNode[,] levelNodes)
	{
		int num = 0;
		LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), LevelDirection.North, levelNodes, true);
		LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), LevelDirection.South, levelNodes, true);
		LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), LevelDirection.East, levelNodes, true);
		LevelNode neighbourAt4 = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), LevelDirection.West, levelNodes, true);
		if (neighbourAt != null && neighbourAt.Elevation > node.Elevation && node.NorthEdgeValue == LevelNodeEdgeValue.Empty)
		{
			if (!IsNodeInIsland(neighbourAt))
			{
				num += 3;
			}
			int num2 = Mathf.Abs(node.Elevation - neighbourAt.Elevation);
			if (num2 > 0)
			{
				num += Mathf.Clamp(4 - num2, 0, 3);
			}
		}
		else if (neighbourAt2 != null && neighbourAt2.Elevation < node.Elevation && neighbourAt2.NorthEdgeValue == LevelNodeEdgeValue.Empty)
		{
			if (!IsNodeInIsland(neighbourAt2))
			{
				num += 3;
			}
			int num3 = Mathf.Abs(node.Elevation - neighbourAt2.Elevation);
			if (num3 > 0)
			{
				num += Mathf.Clamp(4 - num3, 0, 3);
			}
		}
		else if (neighbourAt3 != null)
		{
			if ((neighbourAt3.Elevation < node.Elevation && neighbourAt3.WestEdgeValue == LevelNodeEdgeValue.Empty) || (neighbourAt3.Elevation > node.Elevation && node.EastEdgeValue == LevelNodeEdgeValue.Empty))
			{
				if (!IsNodeInIsland(neighbourAt3))
				{
					num += 2;
				}
				int num4 = Mathf.Abs(node.Elevation - neighbourAt3.Elevation);
				if (num4 > 0)
				{
					num += Mathf.Clamp(4 - num4, 0, 3);
				}
			}
		}
		else if (neighbourAt4 != null && ((neighbourAt4.Elevation < node.Elevation && neighbourAt4.EastEdgeValue == LevelNodeEdgeValue.Empty) || (neighbourAt4.Elevation > node.Elevation && node.WestEdgeValue == LevelNodeEdgeValue.Empty)))
		{
			if (!IsNodeInIsland(neighbourAt4))
			{
				num += 2;
			}
			int num5 = Mathf.Abs(node.Elevation - neighbourAt4.Elevation);
			if (num5 > 0)
			{
				num += Mathf.Clamp(4 - num5, 0, 3);
			}
		}
		return num;
	}

	private bool IsNodeInIsland(LevelNode node)
	{
		LevelNode[] array = islandNodes;
		foreach (LevelNode levelNode in array)
		{
			if (node == levelNode)
			{
				return true;
			}
		}
		return false;
	}
}
