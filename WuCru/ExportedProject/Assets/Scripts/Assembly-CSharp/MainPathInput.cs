public class MainPathInput : LevelChunkInput
{
	public int minWidth;

	public int maxWidth;

	public float segmentLength;

	public float varianceFactor;

	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.MainPath;
		}
	}
}
