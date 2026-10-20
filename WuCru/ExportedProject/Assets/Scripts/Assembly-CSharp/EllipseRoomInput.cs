public class EllipseRoomInput : LevelChunkInput
{
	public LevelPoint extent;

	public int elevationVariance;

	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.EllipseRoom;
		}
	}
}
