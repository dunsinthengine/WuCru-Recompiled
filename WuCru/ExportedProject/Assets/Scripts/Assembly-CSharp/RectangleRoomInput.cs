public class RectangleRoomInput : LevelChunkInput
{
	public LevelPoint extent;

	public int elevationVariance;

	public override LevelChunkType ChunkType
	{
		get
		{
			return LevelChunkType.RectangleRoom;
		}
	}
}
