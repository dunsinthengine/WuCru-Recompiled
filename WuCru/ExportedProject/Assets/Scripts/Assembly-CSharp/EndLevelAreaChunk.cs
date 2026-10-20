public class EndLevelAreaChunk : LevelChunk
{
	private LevelPoint center;

	private LevelPoint extent;

	public override LevelNodeType NodeType
	{
		get
		{
			return LevelNodeType.EndBossArea;
		}
	}

	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.EndLevelArea;
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

	public EndLevelAreaChunk(int maxRows, int maxCols)
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

	public override void SetupGameplay(LevelEngine engine)
	{
	}
}
