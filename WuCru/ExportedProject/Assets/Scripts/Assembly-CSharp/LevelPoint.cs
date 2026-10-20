public struct LevelPoint
{
	public int col;

	public int row;

	public LevelPoint(int row, int col)
	{
		this.row = row;
		this.col = col;
	}

	public LevelPoint(LevelPoint point)
	{
		row = point.row;
		col = point.col;
	}

	public LevelNode LevelNodeAtPoint(LevelNode[,] nodes)
	{
		return nodes[row, col];
	}

	public static LevelPoint operator -(LevelPoint a, LevelPoint b)
	{
		return new LevelPoint(a.row - b.row, a.col - b.col);
	}

	public static LevelPoint operator +(LevelPoint a, LevelPoint b)
	{
		return new LevelPoint(a.row + b.row, a.col + b.col);
	}

	public static bool operator ==(LevelPoint a, LevelPoint b)
	{
		return a.row == b.row && a.col == b.col;
	}

	public static bool operator !=(LevelPoint a, LevelPoint b)
	{
		return a.row != b.row || a.col != b.col;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is LevelPoint))
		{
			return false;
		}
		LevelPoint levelPoint = (LevelPoint)obj;
		if (levelPoint.row == row && levelPoint.col == col)
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return row + col * 1000;
	}

	public override string ToString()
	{
		return "row: " + row + ", col: " + col;
	}
}
