using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayEngine
{
	private Level level;

	private List<GameplayPattern> patterns = new List<GameplayPattern>();

	private List<GameplayPattern> aiPatterns = new List<GameplayPattern>();

	private List<LevelPlateauRect> usedRects = new List<LevelPlateauRect>();

	private List<GameplayPattern> usedPatterns = new List<GameplayPattern>();

	private List<GameplayPattern> setupPatterns = new List<GameplayPattern>();

	private List<ChokePoint> chokePoints = new List<ChokePoint>();

	private List<LevelNode> houseNodes = new List<LevelNode>();

	private List<LevelPlateauRect> houseRects = new List<LevelPlateauRect>();

	private bool isEndLevel;

	private bool hasObjectivePattern;

	private int gameplayPoints;

	private int targetGameplayPoints;

	public List<LevelPlateauRect> HouseRects
	{
		get
		{
			return houseRects;
		}
	}

	public List<LevelNode> HouseNodes
	{
		get
		{
			return houseNodes;
		}
	}

	public bool HasObjectivePattern
	{
		get
		{
			return hasObjectivePattern;
		}
	}

	public GameplayEngine(ref Level level)
	{
		this.level = level;
		if (level.LevelTheme.index == level.LevelThemeCollection.themes.Count - 1)
		{
			isEndLevel = true;
		}
		patterns.Add(new Minefield());
		patterns.Add(new HeavyPatrol());
		patterns.Add(new LockAndKey());
		patterns.Add(new GuardedChokepoint());
		patterns.Add(new DistractionWithBushes());
		patterns.Add(new MiniStronghold());
		patterns.Add(new TimedGate());
		if (level.Seed >= 4000)
		{
			Debug.Log("Add Pattern Elemental Boss");
			patterns.Add(new ElementalBoss());
		}
		aiPatterns.Add(new SimplePatrol());
		aiPatterns.Add(new MediumPatrol());
		aiPatterns.Add(new LongPatrol());
		aiPatterns.Add(new DoublePatrol());
		DetectChokePoints();
	}

	public bool HasSetupPattern(Type patternType)
	{
		foreach (GameplayPattern setupPattern in setupPatterns)
		{
			if (setupPattern.GetType() == patternType)
			{
				return true;
			}
		}
		return false;
	}

	public void SetupGameplay(int numGameplayPoints)
	{
		targetGameplayPoints = numGameplayPoints;
		int seed = GameController.Instance.Seed;
		GameObject areaMapPointer = GameController.Instance.LevelExtensionController.GetAreaMapPointer(seed);
		bool levelExtensionComplete = GameController.Instance.levelExtensionController.GetLevelExtensionComplete("DayOfTheDeparted");
		Debug.Log("<color=yellow>DOTD completed? " + levelExtensionComplete + "</color>");
		if (areaMapPointer != null && !levelExtensionComplete)
		{
			GameplayPattern pattern = new RescueLloyd();
			SetupPattern(pattern);
			hasObjectivePattern = true;
		}
		else if (GameController.Instance.ObjectiveController.CurrentObjective != null)
		{
			ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
			foreach (ObjectiveLine objectiveLine in lines)
			{
				if ((objectiveLine.seed == level.Seed || objectiveLine.seed == -1) && (objectiveLine.levelSequence == level.LevelTheme.index || objectiveLine.levelSequence == -1) && objectiveLine.patternID != ObjectivePatternID.None)
				{
					GameplayPattern gameplayPattern = null;
					switch (objectiveLine.patternID)
					{
					case ObjectivePatternID.RescueCole:
						gameplayPattern = new RescueCole();
						break;
					case ObjectivePatternID.RescueZane:
						gameplayPattern = new RescueZane();
						break;
					case ObjectivePatternID.RescueKai:
						gameplayPattern = new RescueKai();
						break;
					case ObjectivePatternID.RescueJay:
						gameplayPattern = new RescueJay();
						break;
					case ObjectivePatternID.RescueNyah:
						gameplayPattern = new RescueNyah();
						break;
					case ObjectivePatternID.RescueLloyd:
						gameplayPattern = new RescueLloyd();
						break;
					case ObjectivePatternID.Oroborous:
						gameplayPattern = new Oroborous();
						break;
					case ObjectivePatternID.ConstrictaiBoss:
						gameplayPattern = new ConstrictaiBoss();
						break;
					case ObjectivePatternID.FangpyreBoss:
						gameplayPattern = new FangpyreBoss();
						break;
					case ObjectivePatternID.HypnobraiBoss:
						gameplayPattern = new HypnobraiBoss();
						break;
					case ObjectivePatternID.VenomariBoss:
						gameplayPattern = new VenomariBoss();
						break;
					case ObjectivePatternID.FindTheHoloLog:
						gameplayPattern = new FindTheHoloLog();
						break;
					case ObjectivePatternID.FindTheBiteBike:
						gameplayPattern = new FindTheBiteBike();
						break;
					case ObjectivePatternID.SamuraiXCave:
						gameplayPattern = new SamuraiXCave();
						break;
					case ObjectivePatternID.MeetMasterWu:
						gameplayPattern = new MeetMasterWu();
						break;
					}
					if (gameplayPattern != null)
					{
						Debug.Log(string.Concat("***** Setting up ObjectivePattern: ", gameplayPattern.PatternType, " *****"));
						SetupPattern(gameplayPattern);
						hasObjectivePattern = true;
						continue;
					}
					Debug.LogError(string.Concat("***** Could not setup ObjectivePattern '", objectiveLine.patternID, "' on seed: ", objectiveLine.seed, " *****"));
				}
			}
		}
		int num = 50;
		int num2 = 0;
		while (gameplayPoints < targetGameplayPoints)
		{
			GameplayPattern gameplayPattern2 = SelectPattern();
			if (gameplayPattern2 != null)
			{
				SetupPattern(gameplayPattern2);
				num2++;
				if (num2 >= num)
				{
					break;
				}
				continue;
			}
			Debug.LogWarning("No more unused patterns to setup - can't reach target");
			break;
		}
		int num3 = 50;
		while (level.ChunkDB.AIAgents.Count < 8 && num3 > 0)
		{
			GameplayPattern pattern2 = aiPatterns[UnityEngine.Random.Range(0, aiPatterns.Count)];
			SetupPattern(pattern2);
			num3--;
		}
		if (num3 <= 0)
		{
			Debug.LogWarning("Could not setup enough agents to reach target");
		}
	}

	public void BuildHouses(LevelEngine engine, Level level)
	{
		int num = UnityEngine.Random.Range(level.LevelTheme.minRectangleRooms, level.LevelTheme.maxRectangleRooms + 1);
		for (int i = 0; i < num; i++)
		{
			HousePattern housePattern = new HousePattern();
			LevelPlateauRect levelPlateauRect = housePattern.FindBestRect(level, usedRects, houseRects, houseNodes, isEndLevel);
			if (levelPlateauRect == null)
			{
				Debug.LogWarning("Cannot setup House, as no best rect was returned. Bailing out");
				break;
			}
			housePattern.Setup(levelPlateauRect, level);
			usedRects.Add(levelPlateauRect);
			houseRects.Add(levelPlateauRect);
			foreach (LevelNode node in levelPlateauRect.nodes)
			{
				houseNodes.Add(node);
			}
		}
	}

	public void PlaceElementalDoors(LevelEngine engine, Level level, float probablity)
	{
		bool checkForStronghold = false;
		int count = usedPatterns.Count;
		for (int i = 0; i < count; i++)
		{
			if (usedPatterns[i].PatternType == GameplayPatternType.MiniStronghold)
			{
				checkForStronghold = true;
				break;
			}
		}
		if (level.PrimaryPower == ElementalPower.None)
		{
			return;
		}
		List<LevelNode> list = new List<LevelNode>();
		ChokePoint bestChokePointForElementalDoor = GetBestChokePointForElementalDoor(level, checkForStronghold);
		if (bestChokePointForElementalDoor != null)
		{
			int num = bestChokePointForElementalDoor.numNodes / 2;
			LevelNode levelNode = null;
			level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + num].NorthEdgeValue = GetDoorByPrimaryElementalPower(level);
			list.Add(level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + num]);
			for (int j = 0; j < bestChokePointForElementalDoor.numNodes; j++)
			{
				if (j != num)
				{
					levelNode = LevelHelper.GetNeighbourAt(new LevelPoint(bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + j), LevelDirection.North, level.Nodes, true);
					if (level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + j].NorthEdgeValue == LevelNodeEdgeValue.Empty && (levelNode == null || levelNode.SouthEdgeValue == LevelNodeEdgeValue.Empty))
					{
						level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + j].NorthEdgeValue = LevelNodeEdgeValue.AltWall;
						list.Add(level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + j]);
					}
				}
			}
			PathFinding pathFinding = new PathFinding(level.Nodes);
			LevelPathGenerator path = level.ChunkDB.GetChunk<MainPathChunk>().Path;
			LevelNode start = LevelHelper.Vector3ToNode(new Vector3(path.from.x, 0f, path.from.y), level.Nodes);
			levelNode = null;
			LevelDirection levelDirection = LevelDirection.North;
			LevelChunk chunk = level.ChunkDB.GetChunk(level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col].ChunkId);
			if (chunk.ChunkType == LevelChunkType.SideQuest)
			{
				SideQuestChunk sideQuestChunk = chunk as SideQuestChunk;
				if (sideQuestChunk.Path.to.y < 10f)
				{
					levelDirection = LevelDirection.South;
				}
			}
			for (int k = 0; k < bestChokePointForElementalDoor.numNodes; k++)
			{
				levelNode = ((levelDirection != LevelDirection.North) ? level.Nodes[bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + k] : LevelHelper.GetNeighbourAt(new LevelPoint(bestChokePointForElementalDoor.startPoint.row, bestChokePointForElementalDoor.startPoint.col + k), levelDirection, level.Nodes, true));
				if (levelNode != null)
				{
					break;
				}
			}
			if (levelNode == null)
			{
				Debug.LogError("Neighbour was null. Something is rotten...");
				UndoWall(list);
			}
			else if (pathFinding.GetPathIncludeDoors(start, levelNode, null) != null)
			{
				Debug.LogError("Area not locked in by ElementalDoor. Destroying...");
				UndoWall(list);
			}
			else if (HasKeyOrLever(level, levelNode, pathFinding))
			{
				Debug.LogError("Key or DoorTrigger locked in - destroying wall...");
				UndoWall(list);
			}
			else if (!HasChestOrCrystal(level, levelNode, pathFinding))
			{
				Debug.LogWarning("No chest or crystal behind elemental wall/door - destroying wall...");
				UndoWall(list);
			}
		}
		else
		{
			Debug.LogWarning("No best ChokePoint returned, could not place ElementalDoor");
		}
	}

	private void UndoWall(List<LevelNode> buildList)
	{
		foreach (LevelNode build in buildList)
		{
			build.NorthEdgeValue = LevelNodeEdgeValue.Empty;
		}
	}

	private bool HasChestOrCrystal(Level level, LevelNode startPoint, PathFinding pf)
	{
		List<LevelNode> visitedNodes = new List<LevelNode>();
		int count = 0;
		NeighboursHasChestOrCrystal(level, startPoint, visitedNodes, pf, ref count);
		return count != 0;
	}

	private bool NeighboursHasChestOrCrystal(Level level, LevelNode node, List<LevelNode> visitedNodes, PathFinding pf, ref int count)
	{
		visitedNodes.Add(node);
		if (node.ContainsObjectType(LevelNodeObjectType.GoldChest) || node.ContainsObjectType(LevelNodeObjectType.MetalChest) || node.ContainsObjectType(LevelNodeObjectType.WoodChest) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_earth) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_electricity) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_energy) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_fire) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_ice) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_water))
		{
			count++;
		}
		if (count > 0)
		{
			return true;
		}
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), levelDirection, level.Nodes, true);
			if (neighbourAt != null && !visitedNodes.Contains(neighbourAt) && pf.GetPathIncludeDoors(node, neighbourAt, null) != null)
			{
				NeighboursHasChestOrCrystal(level, neighbourAt, visitedNodes, pf, ref count);
			}
		}
		if (count > 0)
		{
			return true;
		}
		return false;
	}

	private bool HasKeyOrLever(Level level, LevelNode startPoint, PathFinding pf)
	{
		List<LevelNode> visitedNodes = new List<LevelNode>();
		int count = 0;
		NeighboursHasKeyOrLever(level, startPoint, visitedNodes, pf, ref count);
		return count != 0;
	}

	private bool NeighboursHasKeyOrLever(Level level, LevelNode node, List<LevelNode> visitedNodes, PathFinding pf, ref int count)
	{
		visitedNodes.Add(node);
		if (node.ContainsObjectType(LevelNodeObjectType.Key) || node.ContainsObjectType(LevelNodeObjectType.DoorTrigger))
		{
			count++;
		}
		if (count > 0)
		{
			return true;
		}
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), levelDirection, level.Nodes, true);
			if (neighbourAt != null && !visitedNodes.Contains(neighbourAt) && pf.GetPathIncludeDoors(node, neighbourAt, null) != null)
			{
				NeighboursHasKeyOrLever(level, neighbourAt, visitedNodes, pf, ref count);
			}
		}
		if (count > 0)
		{
			return true;
		}
		return false;
	}

	private LevelNodeEdgeValue GetDoorByPrimaryElementalPower(Level level)
	{
		switch (level.PrimaryPower)
		{
		case ElementalPower.Earth:
			return LevelNodeEdgeValue.ElementalDoor_earth;
		case ElementalPower.Electricity:
			return LevelNodeEdgeValue.ElementalDoor_electricity;
		case ElementalPower.Energy:
			return LevelNodeEdgeValue.ElementalDoor_energy;
		case ElementalPower.Fire:
			return LevelNodeEdgeValue.ElementalDoor_fire;
		case ElementalPower.Ice:
			return LevelNodeEdgeValue.ElementalDoor_ice;
		case ElementalPower.Water:
			return LevelNodeEdgeValue.ElementalDoor_water;
		default:
			return LevelNodeEdgeValue.Empty;
		}
	}

	private ChokePoint GetBestChokePointForElementalDoor(Level level, bool checkForStronghold)
	{
		ChokePoint result = null;
		int num = 0;
		foreach (ChokePoint chokePoint in chokePoints)
		{
			bool flag = false;
			if (checkForStronghold)
			{
				LevelNode[] nodes = chokePoint.GetNodes(level.Nodes);
				int numNodes = chokePoint.numNodes;
				for (int i = 0; i < numNodes; i++)
				{
					if (nodes[i].ContainsObjectType(LevelNodeObjectType.GoldChest) || nodes[i].HasWallAnyTypeAnyDirection())
					{
						flag = true;
					}
				}
			}
			if (flag || chokePoint.dir != LevelDirection.East || chokePoint.numNodes > 7 || level.Nodes[chokePoint.startPoint.row, chokePoint.startPoint.col].Type != LevelNodeType.SideQuest)
			{
				continue;
			}
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int j = 0; j < chokePoint.numNodes; j++)
			{
				if (level.Nodes[chokePoint.startPoint.row, chokePoint.startPoint.col + j].Type != LevelNodeType.SideQuest)
				{
					num3++;
				}
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(chokePoint.startPoint.row, chokePoint.startPoint.col + j), LevelDirection.North, level.Nodes, true);
				if (neighbourAt == null || neighbourAt.Elevation != level.Nodes[chokePoint.startPoint.row, chokePoint.startPoint.col + j].Elevation)
				{
					num4++;
				}
			}
			SideQuestChunk sideQuestChunk = level.ChunkDB.GetChunk(level.Nodes[chokePoint.startPoint.row, chokePoint.startPoint.col].ChunkId) as SideQuestChunk;
			bool flag2 = false;
			foreach (LevelNode node in sideQuestChunk.NodeList)
			{
				if (node.ContainsObjectType(LevelNodeObjectType.Key) || node.ContainsObjectType(LevelNodeObjectType.DoorTrigger))
				{
					flag2 = true;
					break;
				}
			}
			if (num4 != 0)
			{
				continue;
			}
			bool flag3 = false;
			LevelNode[] nodes2 = chokePoint.GetNodes(level.Nodes);
			foreach (LevelNode levelNode in nodes2)
			{
				if (levelNode.Type == LevelNodeType.House)
				{
					flag3 = true;
					break;
				}
			}
			if (flag3)
			{
				continue;
			}
			bool flag4 = false;
			LevelNode[] nodes3 = chokePoint.GetNodes(level.Nodes);
			foreach (LevelNode levelNode2 in nodes3)
			{
				if (levelNode2.NorthEdgeValue != LevelNodeEdgeValue.Empty)
				{
					flag4 = true;
					break;
				}
				LevelNode neighbour = levelNode2.GetNeighbour(level.Nodes, LevelDirection.North);
				if (neighbour != null && neighbour.SouthEdgeValue != LevelNodeEdgeValue.Empty)
				{
					flag4 = true;
					break;
				}
				LevelNode neighbour2 = levelNode2.GetNeighbour(level.Nodes, LevelDirection.South);
				if (neighbour != null && neighbour2 != null && ((neighbour.EastEdgeValue != LevelNodeEdgeValue.Empty && neighbour2.EastEdgeValue != LevelNodeEdgeValue.Empty) || (neighbour.WestEdgeValue != LevelNodeEdgeValue.Empty && neighbour2.WestEdgeValue != LevelNodeEdgeValue.Empty)))
				{
					flag4 = true;
					break;
				}
			}
			if (!flag4)
			{
				num2 += 10;
				if (num3 == 0)
				{
					num2 += 100;
				}
				if (!flag2)
				{
					num2 += 200;
				}
				if (chokePoint.numNodes == 1)
				{
					num2 += 100;
				}
				if (chokePoint.numNodes == 3)
				{
					num2 += 50;
				}
				if (chokePoint.numNodes == 5)
				{
					num2 += 25;
				}
				if (chokePoint.numNodes == 7)
				{
					num2 += 10;
				}
				if (num2 > num)
				{
					num = num2;
					result = chokePoint;
				}
			}
		}
		return result;
	}

	public void PlaceLoot(LevelEngine engine, Level level, int numChests, int numCrystalShards)
	{
		LevelPlacementHelper levelPlacementHelper = new LevelPlacementHelper(engine.Nodes, engine.ChunkDB);
		int num = 0;
		int num2 = 0;
		List<LevelNode> list = new List<LevelNode>();
		LevelNode[,] nodes = engine.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = nodes[i, j];
				if (levelNode.ContainsObjectType(LevelNodeObjectType.WoodChest) || levelNode.ContainsObjectType(LevelNodeObjectType.MetalChest) || levelNode.ContainsObjectType(LevelNodeObjectType.GoldChest))
				{
					num++;
					list.Add(levelNode);
				}
				if (levelNode.ContainsObjectType(LevelNodeObjectType.CrystalShard))
				{
					num2++;
					list.Add(levelNode);
				}
			}
		}
		numChests -= num;
		numCrystalShards -= num2;
		for (int k = 0; k < numChests; k++)
		{
			float value = UnityEngine.Random.value;
			LevelNodeObjectType levelNodeObjectType = ((value < 0.18f) ? LevelNodeObjectType.GoldChest : ((!(value < 0.5f)) ? LevelNodeObjectType.WoodChest : LevelNodeObjectType.MetalChest));
			LevelNode bestLootNode = levelPlacementHelper.GetBestLootNode(list, levelNodeObjectType);
			if (bestLootNode != null)
			{
				bestLootNode.AddObjectType(levelNodeObjectType, level.ChunkDB, string.Empty);
				list.Add(bestLootNode);
				continue;
			}
			Debug.LogWarning("Can't place chest, as no best node was returned!");
			break;
		}
		for (int l = 0; l < numCrystalShards; l++)
		{
			LevelNode bestLootNode2 = levelPlacementHelper.GetBestLootNode(list, LevelNodeObjectType.CrystalShard);
			if (bestLootNode2 != null)
			{
				if (UnityEngine.Random.value < 0.66f)
				{
					bestLootNode2.AddObjectType(LevelNodeObjectType.CrystalShard, level.ChunkDB, string.Empty);
				}
				else
				{
					LevelNodeObjectType elementalPowerType = CrystalShard.GetElementalPowerType(level);
					if (UnityEngine.Random.value < 0.66f)
					{
						bestLootNode2.AddObjectType(CrystalShard.GetElementalPowerType(level), engine.ChunkDB, string.Empty);
					}
				}
				list.Add(bestLootNode2);
				continue;
			}
			Debug.LogWarning("Can't place CrystalShard, as no best node was returned");
			break;
		}
	}

	public void PlaceStuds(LevelEngine engine, int numStuds)
	{
		int num = 0;
		int num2 = numStuds / 3;
		foreach (LevelIsland item in engine.ChunkDB.LevelIslands.OrderByDescending((LevelIsland o) => o.islandNodes.Length))
		{
			if (item.islandNodes.Length >= 3 && num < num2)
			{
				if (item.isConnected || item.ConnectToLevel(engine))
				{
					LevelNode[] islandNodes = item.islandNodes;
					foreach (LevelNode levelNode in islandNodes)
					{
						if (levelNode.ObjectTypes.Count == 0 && num < num2)
						{
							levelNode.AddObjectType(LevelNodeObjectType.Studs, engine.ChunkDB, string.Empty);
							num++;
						}
					}
				}
				else
				{
					Debug.LogWarning("Unable to connect island...");
				}
			}
			else if (!item.isConnected && !item.ConnectToLevel(engine))
			{
				Debug.LogWarning("Unable to connect island...");
			}
		}
		while (num < num2)
		{
			LevelPlateauRect bestRectForStuds = GetBestRectForStuds(engine);
			if (bestRectForStuds == null)
			{
				break;
			}
			foreach (LevelNode node in bestRectForStuds.nodes)
			{
				if (node.ObjectTypes.Count == 0 && num < num2)
				{
					node.AddObjectType(LevelNodeObjectType.Studs, level.ChunkDB, string.Empty);
					num++;
				}
			}
			usedRects.Add(bestRectForStuds);
		}
		int num4 = numStuds - num;
		SideQuestChunk[] array = engine.ChunkDB.GetChunks<SideQuestChunk>().ToArray();
		int numStuds2 = num4 / array.Length;
		int num5 = 0;
		SideQuestChunk[] array2 = array;
		foreach (SideQuestChunk sideQuestChunk in array2)
		{
			num += PlaceStudsAtPath(numStuds2, array[num5].Path);
			num5++;
			if (num5 < array.Length - 1)
			{
				num4 = numStuds - num;
				numStuds2 = num4 / (array.Length - num5);
			}
		}
		if (num < numStuds)
		{
			int num7 = numStuds - num;
			if (num7 < 3)
			{
				num7 = 3;
			}
			MainPathChunk chunk = engine.ChunkDB.GetChunk<MainPathChunk>();
			num += PlaceStudsAtPath(num7, chunk.Path);
		}
	}

	public void PlaceLamps(LevelEngine engine)
	{
		LevelPlacementHelper levelPlacementHelper = new LevelPlacementHelper(engine.Nodes, engine.ChunkDB);
		LevelPathGenerator path = engine.ChunkDB.GetChunk<MainPathChunk>().Path;
		PathFinding.Node[] pathIgnoreDoors = engine.PathFinding.GetPathIgnoreDoors(engine.Nodes[(int)path.from.y, (int)path.from.x], engine.Nodes[(int)path.to.y, (int)path.to.x], null);
		if (pathIgnoreDoors == null)
		{
			Debug.LogError("Could not find path from start to end. Cannot setup lamps");
			return;
		}
		int num = 5;
		int num2 = 0;
		int num3 = 0;
		PathFinding.Node[] array = pathIgnoreDoors;
		foreach (PathFinding.Node node in array)
		{
			num2++;
			if (num2 % num == 0)
			{
				LevelNode levelNode = node.levelNode;
				if (!levelPlacementHelper.EvaluateNode(levelNode, LevelNodeObjectType.LampPost))
				{
					levelNode = levelPlacementHelper.GetClosestFreeNode(levelNode, LevelNodeObjectType.LampPost, 1);
				}
				if (levelNode != null)
				{
					levelNode.AddObjectType(LevelNodeObjectType.LampPost, engine.ChunkDB, string.Empty);
					num3++;
				}
			}
		}
	}

	private LevelPlateauRect GetBestRectForStuds(LevelEngine engine)
	{
		int num = 0;
		LevelPlateauRect result = null;
		foreach (LevelPlateau levelPlateau in engine.ChunkDB.LevelPlateaus)
		{
			int num2 = 0;
			foreach (LevelPlateauRect rect in levelPlateau.rects)
			{
				if (!usedRects.Contains(rect) && rect.nodes.Count >= 3 && rect.min.col > 8)
				{
					num2 += CalcRectEmptinessScore(rect);
					num2 += rect.min.col * 3;
					if (levelPlateau.nodes.Count < 15)
					{
						num2 += 50;
					}
					if (num2 > num)
					{
						num = num2;
						result = rect;
					}
				}
			}
		}
		return result;
	}

	private int CalcRectEmptinessScore(LevelPlateauRect rect)
	{
		int num = 0;
		foreach (LevelNode node in rect.nodes)
		{
			if (node.ObjectTypes.Count > 0)
			{
				num++;
			}
		}
		return Mathf.RoundToInt(((float)rect.nodes.Count - (float)num) / (float)rect.nodes.Count * 100f);
	}

	private int PlaceStudsAtPath(int numStuds, LevelPathGenerator path)
	{
		int num = 0;
		LevelNode[] array = path.GetIntersectingNodes(level.Nodes).ToArray();
		int num2 = array.Length / 2 - numStuds / 2;
		int num3 = num2 + numStuds;
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num3 > array.Length - 1)
		{
			num3 = array.Length - 1;
		}
		for (int i = num2; i <= num3; i++)
		{
			if (array[i].ObjectTypes.Count == 0 && array[i].Elevation > 0)
			{
				array[i].AddObjectType(LevelNodeObjectType.Studs, level.ChunkDB, string.Empty);
				num++;
			}
		}
		return num;
	}

	private void SetupPattern(GameplayPattern pattern)
	{
		LevelPlateauRect levelPlateauRect = pattern.FindBestRect(level, usedRects, houseRects, houseNodes, isEndLevel);
		if (levelPlateauRect == null)
		{
			Debug.LogWarning(string.Concat("Cannot setup pattern ", pattern.PatternType, ", as no best rect was returned"));
			usedPatterns.Add(pattern);
			return;
		}
		int num = pattern.Setup(levelPlateauRect, level);
		usedPatterns.Add(pattern);
		if (num > 0)
		{
			usedRects.Add(levelPlateauRect);
			gameplayPoints += num;
			setupPatterns.Add(pattern);
		}
	}

	private GameplayPattern SelectPattern()
	{
		List<GameplayPattern> list = new List<GameplayPattern>();
		bool flag = false;
		foreach (GameplayPattern pattern in patterns)
		{
			if (!usedPatterns.Contains(pattern))
			{
				list.Add(pattern);
				if (pattern.PatternType == GameplayPatternType.ElementalBoss)
				{
					flag = true;
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		if (level.Seed >= 4000 && flag)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i].PatternType == GameplayPatternType.ElementalBoss)
				{
					return list[i];
				}
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private void DetectChokePoints()
	{
		PathFinding pathFinding = new PathFinding(level.Nodes);
		LevelPathGenerator path = level.ChunkDB.GetChunk<MainPathChunk>().Path;
		List<LevelNode> pathNodes = path.GetPathNodes(level.Nodes, pathFinding);
		int num = 0;
		foreach (LevelNode item in pathNodes)
		{
			if (item.Col <= num || item.Col >= level.Nodes.GetLength(1) - 2)
			{
				continue;
			}
			num = item.Col;
			if (1 == 0)
			{
				continue;
			}
			int elevation = item.Elevation;
			int row = item.Row;
			LevelNode neighbour = item.GetNeighbour(level.Nodes, LevelDirection.South);
			while (neighbour != null && neighbour.Type != LevelNodeType.None && neighbour.Elevation == elevation)
			{
				row = neighbour.Row;
				neighbour = neighbour.GetNeighbour(level.Nodes, LevelDirection.South);
			}
			int row2 = item.Row;
			LevelNode neighbour2 = item.GetNeighbour(level.Nodes, LevelDirection.North);
			while (neighbour2 != null && neighbour2.Type != LevelNodeType.None && neighbour2.Elevation == elevation)
			{
				row2 = neighbour2.Row;
				neighbour2 = neighbour2.GetNeighbour(level.Nodes, LevelDirection.North);
			}
			int[] array = new int[row2 - row + 1];
			for (int i = row; i <= row2; i++)
			{
				array[i - row] = level.Nodes[i, num].Elevation;
				level.Nodes[i, num].Elevation = 100;
			}
			LevelNode neighbour3 = item.GetNeighbour(level.Nodes, LevelDirection.West);
			LevelNode neighbour4 = item.GetNeighbour(level.Nodes, LevelDirection.East);
			PathFinding.Node[] pathIgnoreDoors = pathFinding.GetPathIgnoreDoors(neighbour3, neighbour4, null);
			if (pathIgnoreDoors == null)
			{
				ChokePoint chokePoint = new ChokePoint();
				chokePoint.startPoint = new LevelPoint(row, num);
				chokePoint.numNodes = row2 - row + 1;
				chokePoint.dir = LevelDirection.North;
				LevelPlateauRect levelPlateauRect = null;
				foreach (LevelPlateau levelPlateau in level.ChunkDB.LevelPlateaus)
				{
					foreach (LevelPlateauRect rect in levelPlateau.rects)
					{
						if (rect.IsChokePointInRect(chokePoint))
						{
							if (levelPlateauRect == null)
							{
								levelPlateauRect = rect;
							}
							else if (rect.nodes.Count < levelPlateauRect.nodes.Count)
							{
								levelPlateauRect = rect;
							}
						}
					}
				}
				if (levelPlateauRect != null)
				{
					chokePoint.rect = levelPlateauRect;
					levelPlateauRect.chokePoint = chokePoint;
					chokePoints.Add(chokePoint);
				}
				else
				{
					Debug.LogWarning("Choke-point detected, but it doesn't fit into any rect...skipping choke-point!");
				}
			}
			for (int j = row; j <= row2; j++)
			{
				level.Nodes[j, num].Elevation = array[j - row];
			}
		}
		foreach (SideQuestChunk chunk in level.ChunkDB.GetChunks<SideQuestChunk>())
		{
			LevelPathGenerator path2 = chunk.Path;
			List<LevelNode> pathNodes2 = path2.GetPathNodes(level.Nodes, pathFinding);
			int num2 = 0;
			List<int> list = new List<int>();
			foreach (LevelNode item2 in pathNodes2)
			{
				if (list.Contains(item2.Row) || item2.Row >= level.Nodes.GetLength(0) - 2)
				{
					continue;
				}
				LevelNode neighbour5 = item2.GetNeighbour(level.Nodes, LevelDirection.South);
				LevelNode neighbour6 = item2.GetNeighbour(level.Nodes, LevelDirection.North);
				if (neighbour5 == null || neighbour6 == null)
				{
					continue;
				}
				list.Add(item2.Row);
				num2 = item2.Row;
				int elevation2 = item2.Elevation;
				int col = item2.Col;
				LevelNode neighbour7 = item2.GetNeighbour(level.Nodes, LevelDirection.West);
				while (neighbour7 != null && neighbour7.Type != LevelNodeType.None && neighbour7.Elevation == elevation2)
				{
					col = neighbour7.Col;
					neighbour7 = neighbour7.GetNeighbour(level.Nodes, LevelDirection.West);
				}
				int col2 = item2.Col;
				LevelNode neighbour8 = item2.GetNeighbour(level.Nodes, LevelDirection.East);
				while (neighbour8 != null && neighbour8.Type != LevelNodeType.None && neighbour8.Elevation == elevation2)
				{
					col2 = neighbour8.Col;
					neighbour8 = neighbour8.GetNeighbour(level.Nodes, LevelDirection.East);
				}
				int[] array2 = new int[col2 - col + 1];
				for (int k = col; k <= col2; k++)
				{
					array2[k - col] = level.Nodes[num2, k].Elevation;
					level.Nodes[num2, k].Elevation = 100;
				}
				PathFinding.Node[] pathIgnoreDoors2 = pathFinding.GetPathIgnoreDoors(neighbour5, neighbour6, null);
				if (pathIgnoreDoors2 == null)
				{
					ChokePoint chokePoint2 = new ChokePoint();
					chokePoint2.startPoint = new LevelPoint(num2, col);
					chokePoint2.numNodes = col2 - col + 1;
					chokePoint2.dir = LevelDirection.East;
					LevelPlateauRect levelPlateauRect2 = null;
					foreach (LevelPlateau levelPlateau2 in level.ChunkDB.LevelPlateaus)
					{
						foreach (LevelPlateauRect rect2 in levelPlateau2.rects)
						{
							if (rect2.IsChokePointInRect(chokePoint2))
							{
								if (levelPlateauRect2 == null)
								{
									levelPlateauRect2 = rect2;
								}
								else if (rect2.nodes.Count < levelPlateauRect2.nodes.Count)
								{
									levelPlateauRect2 = rect2;
								}
							}
						}
					}
					if (levelPlateauRect2 != null)
					{
						chokePoint2.rect = levelPlateauRect2;
						levelPlateauRect2.chokePoint = chokePoint2;
						chokePoints.Add(chokePoint2);
					}
					else
					{
						Debug.LogWarning("Choke-point detected, but it doesn't fit into any rect...skipping choke-point!");
					}
				}
				for (int l = col; l <= col2; l++)
				{
					level.Nodes[num2, l].Elevation = array2[l - col];
				}
			}
		}
	}
}
