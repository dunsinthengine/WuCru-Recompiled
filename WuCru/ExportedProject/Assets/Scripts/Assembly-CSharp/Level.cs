using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level
{
	private int seed;

	private LevelNode[,] nodes;

	private LevelChunkDB chunkDB;

	private List<AIAgent> agents = new List<AIAgent>();

	private List<LightSource> lightSources = new List<LightSource>();

	private LevelStyle style;

	private LevelThemeCollection themeCollection;

	private LevelTheme theme;

	private LevelPlacementHelper placementHelper;

	private ElementalPower primaryPower;

	private ElementalPower secondaryPower;

	private int numFilledTiles;

	private LevelPoint startPoint;

	private LevelPoint endPoint;

	private LevelChallengeBase challenge;

	public int Seed
	{
		get
		{
			return seed;
		}
	}

	public LevelNode[,] Nodes
	{
		get
		{
			return nodes;
		}
	}

	public LevelChunkDB ChunkDB
	{
		get
		{
			return chunkDB;
		}
	}

	public AIAgent[] Agents
	{
		get
		{
			return agents.ToArray();
		}
	}

	public LightSource[] LightSources
	{
		get
		{
			return lightSources.ToArray();
		}
	}

	public LevelStyle LevelStyle
	{
		get
		{
			return style;
		}
		set
		{
			style = value;
		}
	}

	public LevelTheme LevelTheme
	{
		get
		{
			return theme;
		}
		set
		{
			theme = value;
		}
	}

	public LevelThemeCollection LevelThemeCollection
	{
		get
		{
			return themeCollection;
		}
		set
		{
			themeCollection = value;
		}
	}

	public LevelPlacementHelper PlacementHelper
	{
		get
		{
			return placementHelper;
		}
	}

	public ElementalPower PrimaryPower
	{
		get
		{
			return primaryPower;
		}
		set
		{
			primaryPower = value;
		}
	}

	public ElementalPower SecondaryPower
	{
		get
		{
			return secondaryPower;
		}
		set
		{
			secondaryPower = value;
		}
	}

	public LevelPoint StartPoint
	{
		get
		{
			return startPoint;
		}
		set
		{
			startPoint = value;
		}
	}

	public LevelPoint EndPoint
	{
		get
		{
			return endPoint;
		}
		set
		{
			endPoint = value;
		}
	}

	public int NumFilledTiles
	{
		get
		{
			return numFilledTiles;
		}
	}

	public LevelChallengeBase Challenge
	{
		get
		{
			return challenge;
		}
		set
		{
			challenge = value;
		}
	}

	public Level(int seed, LevelNode[,] nodes, LevelChunkDB chunkDB, ElementalPower primaryPower, ElementalPower secondaryPower)
	{
		this.seed = seed;
		this.nodes = nodes;
		this.chunkDB = chunkDB;
		this.primaryPower = primaryPower;
		this.secondaryPower = secondaryPower;
		placementHelper = new LevelPlacementHelper(nodes, chunkDB);
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				LevelNode levelNode = nodes[i, j];
				LevelNodeType type = levelNode.Type;
				int elevation = levelNode.Elevation;
				if (type != LevelNodeType.None && elevation > 0)
				{
					numFilledTiles++;
				}
			}
		}
		Debug.Log("Level seed: " + seed.ToString() + " - created with " + nodes.GetLength(0) * nodes.GetLength(1) + " tiles and " + chunkDB.Chunks.Count + " chunks");
	}

	public void RegisterAgent(AIAgent agent, LevelNode node)
	{
		agent.AgentId = node.GetCustomHashCode(nodes);
		agents.Add(agent);
	}

	public LevelNode[] GetNodesWithObject(LevelNodeObjectType type)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				LevelNode levelNode = nodes[i, j];
				for (int k = 0; k < levelNode.ObjectTypes.Count; k++)
				{
					if (levelNode.ObjectTypes[k].objectType == type)
					{
						list.Add(levelNode);
					}
				}
			}
		}
		return list.Distinct().ToArray();
	}

	public void ReplaceAgents(int count, LevelNodeObjectType type)
	{
		PathFinding pathFinding = new PathFinding(nodes);
		LevelNode start = nodes[startPoint.row, startPoint.col];
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				LevelNode levelNode = nodes[i, j];
				for (int k = 0; k < levelNode.ObjectTypes.Count; k++)
				{
					LevelNodeObjectType objectType = levelNode.ObjectTypes[k].objectType;
					if ((objectType == LevelNodeObjectType.ScoutEnemy || objectType == LevelNodeObjectType.GruntEnemy || objectType == LevelNodeObjectType.WarriorEnemy) && pathFinding.GetPathIgnoreDoors(start, levelNode, true, true, false) != null)
					{
						list.Add(levelNode);
					}
				}
			}
		}
		Random.seed = Seed;
		if (list.Count > count)
		{
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = Random.Range(0, num);
				LevelNode value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}
		list = list.OrderBy((LevelNode n) => n.ObjectTypes[0].objectTag).ToList();
		for (int num2 = 0; num2 < Mathf.Min(count, list.Count); num2++)
		{
			list[num2].ClearObjectTypesExcept(chunkDB, LevelNodeObjectType.Key);
			list[num2].AddObjectType(type, chunkDB, string.Empty);
		}
	}

	public void RegisterLightSource(LightSource ls)
	{
		lightSources.Add(ls);
	}

	public void ResetAI()
	{
		foreach (AIAgent agent in agents)
		{
			if (!agent.Character.IsDead())
			{
				agent.Perception.ResetLastKnownPlayerLocation();
				TriggerHash.ResetTriggers(agent.Character.Animator);
				agent.Reset();
				agent.Character.Animator.SetTrigger(TriggerHash.reset);
				agent.Character.Visible(true);
			}
		}
		AIBeacon.Instance.Reset();
	}

	public AIAgent GetAIAgent(int agentID)
	{
		foreach (AIAgent agent in agents)
		{
			if (agent.AgentId == agentID)
			{
				return agent;
			}
		}
		return null;
	}

	public LevelPlateauRect GetForceSettingRectForPattern(GameplayPatternType patternType)
	{
		if (theme.forceSettings == null)
		{
			return null;
		}
		int num = theme.forceSettings.Length;
		for (int i = 0; i < num; i++)
		{
			if (theme.forceSettings[i].levelSeed != Seed)
			{
				continue;
			}
			int num2 = theme.forceSettings[i].settings.Length;
			for (int j = 0; j < num2; j++)
			{
				if (theme.forceSettings[i].settings[j].patternType == patternType)
				{
					int num3 = 0;
					int num4 = 0;
					num3 = theme.forceSettings[i].settings[j].forcePlateau;
					num4 = theme.forceSettings[i].settings[j].forceRect;
					return ChunkDB.LevelPlateaus[num3].rects[num4];
				}
			}
			return null;
		}
		return null;
	}

	private void OnDestroy()
	{
		Debug.Log("Level OnDestroy");
	}
}
