using System.Collections.Generic;
using UnityEngine;

public class LevelPathGenerator
{
	public Vector2 from;

	public Vector2 to;

	private List<LevelPathSegment> segments = new List<LevelPathSegment>();

	public List<LevelPathSegment> Segments
	{
		get
		{
			return segments;
		}
	}

	public List<LevelNode> Generate(LevelNode[,] nodes, Vector2 from, Vector2 to, float segmentLength, float variationFactor, int minWidth, int maxWidth)
	{
		this.from = from;
		this.to = to;
		GenerateSegments(from, to, segmentLength, variationFactor, nodes.GetLength(0), nodes.GetLength(1));
		return GetIntersectingNodes(nodes, minWidth, maxWidth);
	}

	public void GenerateSegments(Vector2 from, Vector2 to, float segmentLength, float variationFactor, int maxRows, int maxCols)
	{
		segments.Clear();
		LevelPathSegment levelPathSegment = new LevelPathSegment(from, to);
		levelPathSegment.SubDivide(segmentLength, ref segments, variationFactor, maxCols - 1, maxRows - 1);
	}

	private List<LevelNode> GetIntersectingNodes(LevelNode[,] nodes, int minWidth, int maxWidth)
	{
		List<LevelNode> list = new List<LevelNode>();
		foreach (LevelPathSegment segment in Segments)
		{
			List<LevelNode> list2 = segment.IntersectingNodes(nodes, minWidth, maxWidth);
			foreach (LevelNode item in list2)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public List<LevelNode> GetIntersectingNodesAtCol(LevelNode[,] nodes, int col)
	{
		List<LevelNode> list = new List<LevelNode>();
		foreach (LevelPathSegment segment in Segments)
		{
			List<LevelNode> list2 = segment.IntersectingNodes(nodes, col);
			foreach (LevelNode item in list2)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public float ClosestDistanceToPoint(LevelPoint point)
	{
		float num = float.MaxValue;
		foreach (LevelPathSegment segment in segments)
		{
			float num2 = segment.ClosestDistanceToPoint(point);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public float ClosestDistanceToPoint(Vector2 point)
	{
		float num = float.MaxValue;
		foreach (LevelPathSegment segment in segments)
		{
			float num2 = segment.ClosestDistanceToPoint(point);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public List<LevelNode> GetPathNodes(LevelNode[,] nodes, PathFinding pf)
	{
		List<LevelNode> list = new List<LevelNode>();
		List<LevelNode> list2 = new List<LevelNode>();
		foreach (LevelPathSegment segment in segments)
		{
			float num = float.MaxValue;
			LevelNode levelNode = null;
			for (int i = 0; i < nodes.GetLength(0); i++)
			{
				for (int j = 0; j < nodes.GetLength(1); j++)
				{
					float num2 = segment.ClosestDistanceToPoint(new LevelPoint(i, j));
					if (num2 < num)
					{
						num = num2;
						levelNode = nodes[i, j];
					}
				}
			}
			if (levelNode != null && !list2.Contains(levelNode))
			{
				list2.Add(levelNode);
			}
		}
		LevelNode item = nodes[(int)to.y, (int)to.x];
		if (!list2.Contains(item))
		{
			list2.Add(item);
		}
		for (int k = 0; k < list2.Count - 1; k++)
		{
			list.Add(list2[k]);
			if (!IsNeighbours(list2[k], list2[k + 1]))
			{
				PathFinding.Node[] path = pf.GetPath(list2[k], list2[k + 1], (PathFinding.Node n1, PathFinding.Node n2) => true);
				for (int num3 = 0; num3 < path.Length; num3++)
				{
					list.Add(path[num3].levelNode);
				}
			}
		}
		list.Add(list2[list2.Count - 1]);
		return list;
	}

	private bool IsNeighbours(LevelNode n1, LevelNode n2)
	{
		if (n1.Row == n2.Row && Mathf.Abs(n1.Col - n2.Col) == 1)
		{
			return true;
		}
		if (n1.Col == n2.Col && Mathf.Abs(n1.Row - n2.Row) == 1)
		{
			return true;
		}
		return false;
	}

	public List<LevelNode> GetIntersectingNodes(LevelNode[,] nodes)
	{
		List<LevelNode> list = new List<LevelNode>();
		foreach (LevelPathSegment segment in segments)
		{
			foreach (LevelNode item in segment.IntersectingNodes(nodes))
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}
}
