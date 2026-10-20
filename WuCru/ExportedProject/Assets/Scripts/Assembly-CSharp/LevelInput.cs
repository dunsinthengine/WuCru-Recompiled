using System.Collections.Generic;

public class LevelInput
{
	public int seed;

	public int rows;

	public int cols;

	public LevelPoint startPoint;

	public LevelPoint endPoint;

	public int minElevation;

	public int maxElevation;

	public int minElevationHills;

	public int maxElevationHills;

	public int minElevationPlateauRows;

	public int maxElevationPlateauRows;

	public int minElevationPlateuaCols;

	public int maxElevationPlateuaCols;

	public List<LevelChunkInput> chunks = new List<LevelChunkInput>();
}
