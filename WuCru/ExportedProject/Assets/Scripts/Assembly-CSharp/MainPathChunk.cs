using UnityEngine;

public class MainPathChunk : LevelChunk
{
	private LevelPathGenerator path;

	public override LevelNodeType NodeType
	{
		get
		{
			return LevelNodeType.MainPath;
		}
	}

	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.MainPath;
		}
	}

	public LevelPathGenerator Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
		}
	}

	public MainPathChunk(int maxRows, int maxCols)
		: base(maxRows, maxCols)
	{
	}

	public override void OnSerializeSpecialized(ref BytePacker bp)
	{
		bp.Write(Path.Segments.Count);
		foreach (LevelPathSegment segment in Path.Segments)
		{
			bp.Write(segment.from.x);
			bp.Write(segment.from.y);
			bp.Write(segment.to.x);
			bp.Write(segment.to.y);
		}
	}

	public override void OnDeserializeSpecialized(ref BytePacker bp)
	{
		Path = new LevelPathGenerator();
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			LevelPathSegment item = new LevelPathSegment(new Vector2(bp.ReadSingle(), bp.ReadSingle()), new Vector2(bp.ReadSingle(), bp.ReadSingle()));
			Path.Segments.Add(item);
		}
	}

	public override void SetupGameplay(LevelEngine engine)
	{
	}
}
