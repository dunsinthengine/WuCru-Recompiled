using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class LevelChunk
{
	private int id;

	private static int idGen = 1;

	protected List<LevelNode> nodeList = new List<LevelNode>();

	protected LevelNode[,] nodes;

	protected int maxRows;

	protected int maxCols;

	public int Id
	{
		get
		{
			return id;
		}
	}

	public ReadOnlyCollection<LevelNode> NodeList
	{
		get
		{
			return nodeList.AsReadOnly();
		}
	}

	public abstract LevelNodeType NodeType { get; }

	public abstract LevelChunkType ChunkType { get; }

	public LevelChunk(int maxRows, int maxCols)
	{
		id = idGen++;
		this.maxRows = maxRows;
		this.maxCols = maxCols;
		nodes = new LevelNode[maxRows, maxCols];
		for (int i = 0; i < maxRows; i++)
		{
			for (int j = 0; j < maxCols; j++)
			{
				nodes[i, j] = new LevelNode(0, i, j);
				nodes[i, j].Type = LevelNodeType.None;
			}
		}
	}

	public abstract void SetupGameplay(LevelEngine engine);

	public virtual void SetupAI(LevelEngine engine)
	{
	}

	public abstract void OnSerializeSpecialized(ref BytePacker bp);

	public abstract void OnDeserializeSpecialized(ref BytePacker bp);

	public void CalculateBounds(ref int minR, ref int maxR, ref int minC, ref int maxC)
	{
		minR = int.MaxValue;
		minC = int.MaxValue;
		maxR = int.MinValue;
		maxC = int.MinValue;
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].Type != LevelNodeType.None)
				{
					if (i < minR)
					{
						minR = i;
					}
					if (j < minC)
					{
						minC = j;
					}
					if (i > maxR)
					{
						maxR = i;
					}
					if (j > maxC)
					{
						maxC = j;
					}
				}
			}
		}
	}

	public void AddNodes(LevelChunkDB chunkDB, List<LevelNode> nodes)
	{
		foreach (LevelNode node in nodes)
		{
			AddNode(chunkDB, node);
		}
	}

	public void AddNode(LevelChunkDB chunkDB, LevelNode node)
	{
		if (node.ChunkId > 0)
		{
			LevelChunk chunk = chunkDB.GetChunk(node.ChunkId);
			if (chunk != null)
			{
				RemoveNode(chunkDB, node);
			}
			else
			{
				Debug.LogWarning("Chunk is null!");
			}
		}
		nodeList.Add(node);
		node.SetChunkId(id);
		nodes[node.Row, node.Col] = node;
	}

	public void RemoveNode(LevelChunkDB chunkDB, LevelNode node)
	{
		nodeList.Remove(node);
		node.ResetChunkId();
		nodes[node.Row, node.Col].Type = LevelNodeType.None;
		if (nodeList.Count == 0)
		{
			chunkDB.UnregisterChunk(this);
		}
	}

	public int RegisterAIAgent(LevelEngine engine, LevelNode node, AIAgentType type, AIPathMode pathMode)
	{
		node.AddObjectType(LevelNodeObjectType.GruntEnemy, engine.ChunkDB, string.Empty);
		return engine.ChunkDB.RegisterAIAgent(type, node, engine.Nodes, pathMode);
	}

	public LevelNode GetNodeFarthestFrom(LevelNode point, PathFinding pathFinding)
	{
		int num = 0;
		LevelNode result = null;
		foreach (LevelNode node in nodeList)
		{
			PathFinding.Node[] pathIgnoreDoors = pathFinding.GetPathIgnoreDoors(node, point, null);
			if (pathIgnoreDoors != null && pathIgnoreDoors.Length > num)
			{
				num = pathIgnoreDoors.Length;
				result = node;
			}
		}
		return result;
	}

	public LevelNode GetNodeClosestTo(LevelNode point, PathFinding pathFinding)
	{
		int num = int.MaxValue;
		LevelNode result = null;
		foreach (LevelNode node in nodeList)
		{
			PathFinding.Node[] pathIgnoreDoors = pathFinding.GetPathIgnoreDoors(node, point, null);
			if (pathIgnoreDoors != null && pathIgnoreDoors.Length < num)
			{
				num = pathIgnoreDoors.Length;
				result = node;
			}
		}
		return result;
	}

	protected List<LevelNode> GetNeighbours(LevelPoint point, bool excludeTypeNone)
	{
		return LevelHelper.GetNeighbours(point, nodes, excludeTypeNone);
	}

	protected LevelNode GetNeighbourAt(LevelPoint point, LevelDirection direction, bool excludeTypeNode)
	{
		return LevelHelper.GetNeighbourAt(point, direction, nodes, excludeTypeNode);
	}

	protected LevelPoint ClampToBounds(LevelPoint point)
	{
		return LevelHelper.ClampToBounds(point, nodes);
	}

	public List<LevelNode> GetPerimeterNodes()
	{
		List<LevelNode> list = new List<LevelNode>();
		foreach (LevelNode node in nodeList)
		{
			List<LevelNode> neighbours = GetNeighbours(node.Location, true);
			if (neighbours.Count < 4)
			{
				list.Add(node);
			}
		}
		return list;
	}

	public void Serialize(ref BytePacker bp)
	{
		bp.Write(nodeList.Count);
		for (int i = 0; i < nodeList.Count; i++)
		{
			bp.Write(nodeList[i].Row);
			bp.Write(nodeList[i].Col);
		}
		OnSerializeSpecialized(ref bp);
	}

	public void Deserialize(ref BytePacker bp, ref Level level)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = bp.ReadInt32();
			int num3 = bp.ReadInt32();
			AddNode(level.ChunkDB, level.Nodes[num2, num3]);
		}
		OnDeserializeSpecialized(ref bp);
	}
}
