using System.Collections.Generic;
using UnityEngine;

public class LevelPathSegment
{
	public Vector2 from;

	public Vector2 to;

	public float Magnitude
	{
		get
		{
			return (to - from).magnitude;
		}
	}

	public LevelPathSegment()
	{
	}

	public LevelPathSegment(Vector2 from, Vector2 to)
	{
		this.from = from;
		this.to = to;
	}

	private Vector2 GetNormal()
	{
		float y = to.x - from.x;
		float num = to.y - from.y;
		return new Vector2(0f - num, y).normalized;
	}

	public void SubDivide(float maxMagnitude, ref List<LevelPathSegment> segments, float variationFactor, float maxX, float maxY)
	{
		float num = Vector2.Distance(from, to);
		Vector2 normalized = (to - from).normalized;
		Vector2 vector = from + normalized * (num / 2f);
		float num2 = Random.Range((0f - num) / 2f * variationFactor, num / 2f * variationFactor);
		vector += GetNormal() * num2;
		if (vector.x > maxX)
		{
			vector.x = maxX;
		}
		if (vector.x < 0f)
		{
			vector.x = 0f;
		}
		if (vector.y > maxY)
		{
			vector.y = maxY;
		}
		if (vector.y < 0f)
		{
			vector.y = 0f;
		}
		LevelPathSegment levelPathSegment = new LevelPathSegment(from, vector);
		LevelPathSegment levelPathSegment2 = new LevelPathSegment(vector, to);
		if (levelPathSegment.Magnitude > maxMagnitude)
		{
			levelPathSegment.SubDivide(maxMagnitude, ref segments, variationFactor, maxX, maxY);
		}
		else
		{
			segments.Add(levelPathSegment);
		}
		if (levelPathSegment2.Magnitude > maxMagnitude)
		{
			levelPathSegment2.SubDivide(maxMagnitude, ref segments, variationFactor, maxX, maxY);
		}
		else
		{
			segments.Add(levelPathSegment2);
		}
	}

	public List<LevelNode> IntersectingNodes(LevelNode[,] nodes)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (SegmentCellIntersection((float)j - 0.5f, (float)i - 0.5f, (float)j + 0.5f, (float)i + 0.5f))
				{
					list.Add(nodes[i, j]);
				}
			}
		}
		return list;
	}

	public List<LevelNode> IntersectingNodes(LevelNode[,] nodes, int col)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			if (SegmentCellIntersection((float)col - 0.5f, (float)i - 0.5f, (float)col + 0.5f, (float)i + 0.5f))
			{
				list.Add(nodes[i, col]);
			}
		}
		return list;
	}

	public List<LevelNode> IntersectingNodes(LevelNode[,] nodes, float minWidth, float maxWidth)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (!SegmentCellIntersection((float)j - 0.5f, (float)i - 0.5f, (float)j + 0.5f, (float)i + 0.5f))
				{
					continue;
				}
				float num = Mathf.Clamp01(Mathf.PerlinNoise((float)j / (float)nodes.GetLength(1), (float)i / (float)nodes.GetLength(0)));
				float num2 = Mathf.Clamp01(Mathf.PerlinNoise((float)j / (float)nodes.GetLength(1) + 0.1f, (float)i / (float)nodes.GetLength(0) + 0.1f));
				num *= maxWidth / 2f;
				num2 *= maxWidth / 2f;
				if (num < minWidth / 2f)
				{
					num = minWidth / 2f;
				}
				if (num2 < minWidth / 2f)
				{
					num2 = minWidth / 2f;
				}
				for (int k = (int)(0f - num); k <= 0; k++)
				{
					for (int l = (int)(0f - num); l <= 0; l++)
					{
						if (k + i >= 0 && k + i < nodes.GetLength(0) && l + j >= 0 && l + j < nodes.GetLength(1))
						{
							list.Add(nodes[k + i, l + j]);
						}
					}
				}
				for (int m = 1; m <= (int)num2; m++)
				{
					for (int n = 1; n <= (int)num2; n++)
					{
						if (m + i >= 0 && m + i < nodes.GetLength(0) && n + j >= 0 && n + j < nodes.GetLength(1))
						{
							list.Add(nodes[m + i, n + j]);
						}
					}
				}
			}
		}
		return list;
	}

	private bool SegmentCellIntersection(float minX, float minY, float maxX, float maxY)
	{
		return LevelHelper.LineRectangleIntersection2d(from, to, minX, minY, maxX, maxY);
	}

	public float ClosestDistanceToPoint(LevelPoint point)
	{
		return ClosestDistanceToPoint(new Vector2(point.col, point.row));
	}

	public float ClosestDistanceToPoint(Vector2 p)
	{
		Vector2 lhs = to - from;
		Vector2 vector = p - from;
		Vector2 b;
		if (Vector2.Dot(lhs, vector) > 0f)
		{
			Vector2 rhs = p - to;
			if (Vector2.Dot(lhs, rhs) < 0f)
			{
				Vector2 normalized = lhs.normalized;
				b = from + normalized * Vector3.Dot(normalized, vector);
			}
			else
			{
				b = to;
			}
		}
		else
		{
			b = from;
		}
		return Vector2.Distance(p, b);
	}
}
