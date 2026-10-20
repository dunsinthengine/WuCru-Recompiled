using System.Collections.Generic;
using UnityEngine;

public class LevelChunkDB
{
	private Dictionary<int, LevelChunk> levelChunks = new Dictionary<int, LevelChunk>();

	private Dictionary<int, AIAgentData> aiAgents = new Dictionary<int, AIAgentData>();

	private Dictionary<int, List<AIWaypoint>> aiAgentPathData = new Dictionary<int, List<AIWaypoint>>();

	private List<LevelPlateau> levelPlateaus = new List<LevelPlateau>();

	private List<LevelIsland> islands;

	public Dictionary<LevelNodeObjectType, int> objectTypeCounter = new Dictionary<LevelNodeObjectType, int>();

	public Dictionary<LevelNodeObjectType, int> pickedObjectTypeCounter = new Dictionary<LevelNodeObjectType, int>();

	public List<InterestZoneData> interestZones = new List<InterestZoneData>();

	public Dictionary<int, LevelChunk>.ValueCollection Chunks
	{
		get
		{
			return levelChunks.Values;
		}
	}

	public List<LevelPlateau> LevelPlateaus
	{
		get
		{
			return levelPlateaus;
		}
	}

	public List<LevelIsland> LevelIslands
	{
		get
		{
			return islands;
		}
		set
		{
			islands = value;
		}
	}

	public Dictionary<int, AIAgentData> AIAgents
	{
		get
		{
			return aiAgents;
		}
	}

	public LevelChunk RegisterNewChunk(int maxRows, int maxCols, LevelChunkType chunkType)
	{
		LevelChunk levelChunk = Factory(chunkType, maxRows, maxCols);
		levelChunks.Add(levelChunk.Id, levelChunk);
		return levelChunk;
	}

	public void UnregisterChunk(LevelChunk chunk)
	{
		levelChunks.Remove(chunk.Id);
	}

	public int RegisterAIAgent(AIAgentType type, LevelNode node, LevelNode[,] nodes, AIPathMode pathMode)
	{
		int customHashCode = node.GetCustomHashCode(nodes);
		AIAgentData value = new AIAgentData(type, pathMode);
		if (!aiAgents.ContainsKey(customHashCode))
		{
			aiAgents.Add(customHashCode, value);
			aiAgentPathData[customHashCode] = new List<AIWaypoint>();
		}
		else
		{
			Debug.LogWarning("AIAgent ID already in dictinary. Is an enemy already placed at same node?");
		}
		return customHashCode;
	}

	public void RegisterAIAgent(AIAgentType type, int id, AIPathMode pathMode)
	{
		AIAgentData value = new AIAgentData(type, pathMode);
		aiAgents.Add(id, value);
		aiAgentPathData[id] = new List<AIWaypoint>();
	}

	public bool RegisterAIPathWaypoint(int agentId, AIWaypoint waypoint, LevelNode[,] nodes)
	{
		if (aiAgents.ContainsKey(agentId))
		{
			aiAgentPathData[agentId].Add(waypoint);
			for (int i = 0; i < aiAgentPathData[agentId].Count - 1; i++)
			{
				LevelNode levelNode = aiAgentPathData[agentId][i].levelNode;
				LevelNode levelNode2 = aiAgentPathData[agentId][i + 1].levelNode;
				if (levelNode.Row != levelNode2.Row)
				{
					for (int j = Mathf.Min(levelNode.Row, levelNode2.Row) - 1; j <= Mathf.Max(levelNode.Row, levelNode2.Row) + 1; j++)
					{
						if (LevelHelper.PointInsideBounds(new LevelPoint(j, levelNode.Col), nodes))
						{
							nodes[j, levelNode.Col].IsAIPatrolNode = true;
						}
					}
					continue;
				}
				for (int k = Mathf.Min(levelNode.Col, levelNode2.Col) - 1; k <= Mathf.Max(levelNode.Col, levelNode2.Col) + 1; k++)
				{
					if (LevelHelper.PointInsideBounds(new LevelPoint(levelNode.Row, k), nodes))
					{
						nodes[levelNode.Row, k].IsAIPatrolNode = true;
					}
				}
			}
			return true;
		}
		Debug.LogError("AddPathWaypoint - agentId not registered");
		return false;
	}

	public bool RegisterAIPathWaypoint(int agentId, LevelNode node, float restTime, LevelNode[,] nodes)
	{
		AIWaypoint waypoint = new AIWaypoint(node, restTime);
		return RegisterAIPathWaypoint(agentId, waypoint, nodes);
	}

	public List<AIWaypoint> GetAIAgentPath(int agentId)
	{
		if (aiAgentPathData.ContainsKey(agentId))
		{
			return aiAgentPathData[agentId];
		}
		Debug.LogError("GetAIAgentPath - don't have agentId '" + agentId + "' registered");
		return null;
	}

	private static LevelChunk Factory(LevelChunkType chunkType, int maxRows, int maxCols)
	{
		switch (chunkType)
		{
		case LevelChunkType.MainPath:
			return new MainPathChunk(maxRows, maxCols);
		case LevelChunkType.SideQuest:
			return new SideQuestChunk(maxRows, maxCols);
		case LevelChunkType.RectangleRoom:
			return new RectangleRoomChunk(maxRows, maxCols);
		case LevelChunkType.EllipseRoom:
			return new EllipseRoomChunk(maxRows, maxCols);
		case LevelChunkType.EndLevelArea:
			return new EndLevelAreaChunk(maxRows, maxCols);
		case LevelChunkType.House:
			return new HouseChunk(maxRows, maxCols);
		default:
			return null;
		}
	}

	public void Clear()
	{
		levelChunks.Clear();
	}

	public LevelChunk GetChunk(int id)
	{
		if (levelChunks.ContainsKey(id))
		{
			return levelChunks[id];
		}
		return null;
	}

	public T GetChunk<T>()
	{
		foreach (LevelChunk value in levelChunks.Values)
		{
			if (value.GetType() == typeof(T))
			{
				return (T)(object)value;
			}
		}
		return default(T);
	}

	public List<T> GetChunks<T>()
	{
		List<T> list = new List<T>();
		foreach (LevelChunk value in levelChunks.Values)
		{
			if (value.GetType() == typeof(T))
			{
				list.Add((T)(object)value);
			}
		}
		return list;
	}

	public void ResetNode(LevelNode node)
	{
		if (node.ChunkId > 0)
		{
			LevelChunk chunk = GetChunk(node.ChunkId);
			if (chunk != null)
			{
				chunk.RemoveNode(this, node);
			}
			else
			{
				Debug.LogWarning("Chunk is null!");
			}
		}
		else
		{
			node.Type = LevelNodeType.None;
			node.Elevation = 0;
		}
	}

	public void ResetNodes(LevelNode[] nodes)
	{
		if (nodes == null)
		{
			Debug.LogWarning("Node array is null!");
			return;
		}
		foreach (LevelNode levelNode in nodes)
		{
			if (levelNode != null)
			{
				ResetNode(levelNode);
			}
			else
			{
				Debug.LogWarning("Node is null!");
			}
		}
	}

	public void SetupGameplay(LevelEngine engine)
	{
		foreach (LevelChunk value in levelChunks.Values)
		{
			value.SetupGameplay(engine);
		}
	}

	public void SetupAI(LevelEngine engine)
	{
		foreach (LevelChunk value in levelChunks.Values)
		{
			value.SetupAI(engine);
		}
	}

	public int GetNumObjectsOfType(LevelNodeObjectType type)
	{
		if (objectTypeCounter.ContainsKey(type))
		{
			return objectTypeCounter[type];
		}
		return 0;
	}

	public void PickupLoot(LevelNodeObjectType type)
	{
		if (pickedObjectTypeCounter.ContainsKey(type))
		{
			pickedObjectTypeCounter[type]++;
		}
		else
		{
			pickedObjectTypeCounter.Add(type, 1);
		}
	}

	public int GetNumObjectOfTypePicked(LevelNodeObjectType type)
	{
		if (pickedObjectTypeCounter.ContainsKey(type))
		{
			return pickedObjectTypeCounter[type];
		}
		return 0;
	}
}
