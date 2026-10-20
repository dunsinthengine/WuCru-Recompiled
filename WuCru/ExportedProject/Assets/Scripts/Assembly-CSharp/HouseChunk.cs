public class HouseChunk : LevelChunk
{
	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.House;
		}
	}

	public override LevelNodeType NodeType
	{
		get
		{
			return LevelNodeType.House;
		}
	}

	public HouseChunk(int maxRows, int maxCols)
		: base(maxRows, maxCols)
	{
	}

	public override void OnSerializeSpecialized(ref BytePacker bp)
	{
	}

	public override void OnDeserializeSpecialized(ref BytePacker bp)
	{
	}

	public override void SetupGameplay(LevelEngine engine)
	{
	}

	public override void SetupAI(LevelEngine engine)
	{
	}
}
