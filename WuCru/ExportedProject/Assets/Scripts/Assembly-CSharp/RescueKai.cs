using System.Collections.Generic;
using UnityEngine;

public class RescueKai : GameplayPattern
{
	private LevelNode endFlagNode;

	public override GameplayPatternType PatternType
	{
		get
		{
			return GameplayPatternType.RescueKai;
		}
	}

	public override int CalcRectScore(LevelPlateauRect rect, List<LevelPlateauRect> usedRects, List<LevelPlateauRect> houseRects, List<LevelNode> houseNode, Level level, bool isEndLevel)
	{
		int num = 0;
		if (rect.min.col >= 26)
		{
			num += CalcIsChunkTypeScore(level, rect, LevelChunkType.EndLevelArea, 50f);
			num += CalcRectObjectDensityScore(level, rect, 50f);
			foreach (LevelNode node in rect.nodes)
			{
				if (node.ContainsObjectType(LevelNodeObjectType.EndFlag))
				{
					num += 500;
					endFlagNode = node;
					break;
				}
			}
		}
		return num;
	}

	public override int Setup(LevelPlateauRect rect, Level level)
	{
		if (endFlagNode != null)
		{
			Debug.Log("EndFlagNode is found");
			LevelNode levelNode = level.Nodes[endFlagNode.Row - 1, endFlagNode.Col - 3];
			if (levelNode.ObjectTypes.Count > 0)
			{
				Debug.LogError(string.Concat("LevelNode found for Kai contains another object of type: ", levelNode.Objects[0].GetType(), ". This object will now be deleted!"));
				levelNode.ClearObjectTypes(level.ChunkDB);
			}
			levelNode.AddObjectType(LevelNodeObjectType.KaiFriend, level.ChunkDB, string.Empty);
			level.ChunkDB.RegisterAIAgent(AIAgentType.KaiFriend, levelNode, level.Nodes, AIPathMode.PingPong);
			Debug.Log("Kai added to levelnode " + levelNode);
		}
		else
		{
			Debug.Log("EndFlagNode not found, calculating placement");
			LevelPoint levelPoint = rect.max - rect.min;
			int num = rect.min.row + levelPoint.row / 2;
			int num2 = rect.min.col + levelPoint.col / 2;
			LevelNode levelNode2 = level.Nodes[num, num2];
			while (levelNode2.ObjectTypes.Count > 0 && num2 >= rect.min.col)
			{
				num2--;
				levelNode2 = level.Nodes[num, num2];
			}
			if (levelNode2.ObjectTypes.Count > 0)
			{
				Debug.LogError(string.Concat("LevelNode found for Kai contains another object of type: ", levelNode2.Objects[0].GetType(), ". This object will now be deleted!"));
				levelNode2.ClearObjectTypes(level.ChunkDB);
			}
			levelNode2.AddObjectType(LevelNodeObjectType.KaiFriend, level.ChunkDB, string.Empty);
			level.ChunkDB.RegisterAIAgent(AIAgentType.KaiFriend, levelNode2, level.Nodes, AIPathMode.PingPong);
			Debug.Log("Kai added to levelnode " + levelNode2);
		}
		return 0;
	}
}
