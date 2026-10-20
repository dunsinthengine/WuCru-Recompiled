using System;
using System.Collections.Generic;
using UnityEngine;

public static class LevelSerializer
{
	public static byte[] SerializeLevel(Level level)
	{
		BytePacker bp = new BytePacker();
		bp.Write(level.Seed);
		bp.Write(level.Nodes.GetLength(0));
		bp.Write(level.Nodes.GetLength(1));
		bp.Write(level.PrimaryPower.ToString());
		bp.Write(level.SecondaryPower.ToString());
		for (int i = 0; i < level.Nodes.GetLength(0); i++)
		{
			for (int j = 0; j < level.Nodes.GetLength(1); j++)
			{
				LevelNode levelNode = level.Nodes[i, j];
				bp.Write((int)levelNode.Type);
				bp.Write(levelNode.Elevation);
				for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
				{
					bp.Write((int)levelNode.GetNodeEdgeValue(levelDirection));
				}
				bp.Write(levelNode.ObjectTypes.Count);
				for (int k = 0; k < levelNode.ObjectTypes.Count; k++)
				{
					bp.Write((int)levelNode.ObjectTypes[k].objectType);
				}
			}
		}
		bp.Write(level.ChunkDB.Chunks.Count);
		foreach (LevelChunk chunk in level.ChunkDB.Chunks)
		{
			bp.Write((int)chunk.ChunkType);
			chunk.Serialize(ref bp);
		}
		bp.Write(level.ChunkDB.AIAgents.Count);
		foreach (int key in level.ChunkDB.AIAgents.Keys)
		{
			bp.Write((int)level.ChunkDB.AIAgents[key].agentType);
			bp.Write((int)level.ChunkDB.AIAgents[key].pathMode);
			bp.Write(key);
		}
		for (int l = 0; l < level.ChunkDB.AIAgents.Count; l++)
		{
			List<AIWaypoint> aIAgentPath = level.ChunkDB.GetAIAgentPath(l);
			bp.Write(aIAgentPath.Count);
			foreach (AIWaypoint item in aIAgentPath)
			{
				bp.Write(item.levelNode.Row);
				bp.Write(item.levelNode.Col);
				bp.Write(item.restTime);
			}
		}
		return bp.ToArray();
	}

	public static Level DeserializeLevel(byte[] bytes)
	{
		LevelChunkDB levelChunkDB = new LevelChunkDB();
		BytePacker bp = new BytePacker(bytes);
		int seed = bp.ReadInt32();
		int num = bp.ReadInt32();
		int num2 = bp.ReadInt32();
		ElementalPower primaryPower = (ElementalPower)Enum.Parse(typeof(ElementalPower), bp.ReadString());
		ElementalPower secondaryPower = (ElementalPower)Enum.Parse(typeof(ElementalPower), bp.ReadString());
		LevelNode[,] array = new LevelNode[num, num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				array[i, j] = new LevelNode(0, i, j);
				array[i, j].Type = (LevelNodeType)bp.ReadInt32();
				array[i, j].Elevation = bp.ReadInt32();
				for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
				{
					array[i, j].SetNodeEdgeValue(levelDirection, (LevelNodeEdgeValue)bp.ReadInt32());
				}
				int num3 = bp.ReadInt32();
				for (int k = 0; k < num3; k++)
				{
					array[i, j].AddObjectType((LevelNodeObjectType)bp.ReadInt32(), levelChunkDB, string.Empty);
				}
			}
		}
		Debug.Log("Deserialized " + array.GetLength(0) * array.GetLength(1) + " tiles");
		Level level = new Level(seed, array, levelChunkDB, primaryPower, secondaryPower);
		int num4 = bp.ReadInt32();
		for (int l = 0; l < num4; l++)
		{
			LevelChunk levelChunk = levelChunkDB.RegisterNewChunk(num, num2, (LevelChunkType)bp.ReadInt32());
			levelChunk.Deserialize(ref bp, ref level);
		}
		Debug.Log("Deserialized " + num4 + " chunks");
		int num5 = bp.ReadInt32();
		for (int m = 0; m < num5; m++)
		{
			levelChunkDB.RegisterAIAgent((AIAgentType)bp.ReadInt32(), bp.ReadInt32(), (AIPathMode)bp.ReadInt32());
		}
		for (int n = 0; n < num5; n++)
		{
			int num6 = bp.ReadInt32();
			for (int num7 = 0; num7 < num6; num7++)
			{
				levelChunkDB.RegisterAIPathWaypoint(n, array[bp.ReadInt32(), bp.ReadInt32()], bp.ReadSingle(), array);
			}
		}
		return level;
	}
}
