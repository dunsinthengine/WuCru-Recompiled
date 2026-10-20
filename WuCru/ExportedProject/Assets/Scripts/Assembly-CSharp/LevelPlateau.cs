using System.Collections.Generic;

public class LevelPlateau
{
	public List<LevelNode> nodes = new List<LevelNode>();

	public List<Segment> segments = new List<Segment>();

	public List<EndPoint> endPoints = new List<EndPoint>();

	public List<LevelPlateauRect> rects = new List<LevelPlateauRect>();

	public LevelPoint min;

	public LevelPoint max;

	public bool isUsed;

	public int Elevation
	{
		get
		{
			return (nodes.Count != 0) ? nodes[0].Elevation : (-1);
		}
	}

	public LevelPlateauRect GetBiggestRect()
	{
		int num = 0;
		LevelPlateauRect result = null;
		foreach (LevelPlateauRect rect in rects)
		{
			if (rect.width * rect.height > num)
			{
				num = rect.width * rect.height;
				result = rect;
			}
		}
		return result;
	}

	public int SizeOfBiggestRect()
	{
		int num = 0;
		foreach (LevelPlateauRect rect in rects)
		{
			if (rect.width * rect.height > num)
			{
				num = rect.width * rect.height;
			}
		}
		return num;
	}

	public List<LevelNode> GetPerimeterNodes(Level level)
	{
		List<LevelNode> list = new List<LevelNode>();
		LevelNode[,] array = new LevelNode[level.Nodes.GetLength(0), level.Nodes.GetLength(1)];
		for (int i = 0; i < level.Nodes.GetLength(0); i++)
		{
			for (int j = 0; j < level.Nodes.GetLength(1); j++)
			{
				array[i, j] = new LevelNode(0, i, j);
			}
		}
		foreach (LevelNode node in nodes)
		{
			array[node.Row, node.Col] = node;
		}
		foreach (LevelNode node2 in nodes)
		{
			List<LevelNode> neighbours = LevelHelper.GetNeighbours(node2.Location, array, true);
			if (neighbours.Count < 4)
			{
				list.Add(node2);
			}
		}
		return list;
	}
}
