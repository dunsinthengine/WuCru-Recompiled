using System.Collections.Generic;

public class LevelPlateauRect
{
	public int row;

	public int col;

	public int width;

	public int height;

	public LevelPoint min;

	public LevelPoint max;

	public List<LevelNode> nodes = new List<LevelNode>();

	public List<int> chunkIDs = new List<int>();

	public LevelPlateau plateau;

	public bool isUsed;

	private int maxCols;

	public ChokePoint chokePoint;

	public LevelPlateauRect(int row, int col, int width, int height, LevelNode[,] levelNodes, LevelPlateau plateau)
	{
		this.row = row;
		this.col = col;
		this.width = width;
		this.height = height;
		this.plateau = plateau;
		min = new LevelPoint(row, col);
		max = new LevelPoint(row + height - 1, col + width - 1);
		for (int i = row; i < row + height; i++)
		{
			for (int j = col; j < col + width; j++)
			{
				nodes.Add(levelNodes[i, j]);
				if (!chunkIDs.Contains(levelNodes[i, j].ChunkId))
				{
					chunkIDs.Add(levelNodes[i, j].ChunkId);
				}
			}
		}
		maxCols = levelNodes.GetLength(1);
	}

	public float GetOverlappingFactor(LevelPlateauRect rect)
	{
		int num = 0;
		foreach (LevelNode node in nodes)
		{
			if (rect.min.row <= node.Row && rect.max.row >= node.Row && rect.min.col <= node.Col && rect.max.col >= node.Col)
			{
				num++;
			}
		}
		return (float)num / (float)nodes.Count;
	}

	public bool Intersect(LevelPlateauRect rect)
	{
		if (rect.min.row < max.row && rect.max.row > min.row && rect.min.col < max.col && rect.max.col > min.col)
		{
			return true;
		}
		return false;
	}

	public float GetLevelNodeTypeFactor(LevelNodeType type)
	{
		int num = 0;
		foreach (LevelNode node in nodes)
		{
			if (node.Type == type)
			{
				num++;
			}
		}
		return (float)num / (float)nodes.Count;
	}

	public bool IsPointInRect(LevelPoint point)
	{
		if (point.row >= min.row && point.row <= max.row && point.col >= min.col && point.col <= max.col)
		{
			return true;
		}
		return false;
	}

	public bool IsChokePointInRect(ChokePoint chokePoint)
	{
		if (IsPointInRect(chokePoint.startPoint))
		{
			for (int i = 0; i < chokePoint.numNodes; i++)
			{
				LevelPoint point = new LevelPoint(chokePoint.startPoint.row, chokePoint.startPoint.col);
				if (chokePoint.dir == LevelDirection.North)
				{
					point.row = chokePoint.startPoint.row + i;
				}
				if (chokePoint.dir == LevelDirection.East)
				{
					point.col = chokePoint.startPoint.col + i;
				}
				if (!IsPointInRect(point))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
