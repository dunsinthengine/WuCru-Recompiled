using System.Collections.Generic;
using UnityEngine;

public class EllipseRoomChunk : LevelChunk
{
	private LevelPoint center;

	private LevelPoint extent;

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
			return LevelChunkType.EllipseRoom;
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

	public EllipseRoomChunk(int maxRows, int maxCols)
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
		List<LevelNode> perimeterNodes = GetPerimeterNodes();
		int num = 0;
		foreach (LevelNode item in perimeterNodes)
		{
			if (num % 3 == 0 && Random.value < 0.9f && item.ObjectTypes.Count == 0 && !item.IsAIPatrolNode && !LevelHelper.NodeHasLadderAnyDirection(item, nodes))
			{
				item.AddObjectType(LevelNodeObjectType.Bush, engine.ChunkDB, string.Empty);
			}
			num++;
		}
	}

	public override void SetupAI(LevelEngine engine)
	{
	}
}
