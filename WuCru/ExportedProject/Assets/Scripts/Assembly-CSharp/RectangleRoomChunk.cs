public class RectangleRoomChunk : LevelChunk
{
	private LevelPoint center;

	private LevelPoint extent;

	private LevelPoint wallMin;

	private LevelPoint wallMax;

	public override LevelNodeType NodeType
	{
		get
		{
			return LevelNodeType.Room;
		}
	}

	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.RectangleRoom;
		}
	}

	public LevelPoint Center
	{
		get
		{
			return center;
		}
		set
		{
			center = value;
		}
	}

	public LevelPoint Extent
	{
		get
		{
			return extent;
		}
		set
		{
			extent = value;
		}
	}

	public LevelPoint WallMin
	{
		get
		{
			return wallMin;
		}
	}

	public LevelPoint WallMax
	{
		get
		{
			return wallMax;
		}
	}

	public RectangleRoomChunk(int maxRows, int maxCols)
		: base(maxRows, maxCols)
	{
	}

	public override void OnSerializeSpecialized(ref BytePacker bp)
	{
		bp.Write(Center.row);
		bp.Write(Center.col);
		bp.Write(extent.row);
		bp.Write(extent.col);
	}

	public override void OnDeserializeSpecialized(ref BytePacker bp)
	{
		center.row = bp.ReadInt32();
		center.col = bp.ReadInt32();
		extent.row = bp.ReadInt32();
		extent.col = bp.ReadInt32();
	}

	public void SetWallPiece(LevelNode node, LevelDirection dir)
	{
		node.SetNodeEdgeValue(dir, LevelNodeEdgeValue.Wall);
	}

	public override void SetupGameplay(LevelEngine engine)
	{
	}

	public override void SetupAI(LevelEngine engine)
	{
	}
}
