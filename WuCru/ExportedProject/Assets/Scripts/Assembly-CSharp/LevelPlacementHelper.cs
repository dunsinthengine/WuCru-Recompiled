using System.Collections.Generic;
using UnityEngine;

public class LevelPlacementHelper
{
	private LevelNode[,] nodes;

	private LevelNode startNode;

	private LevelNode endNode;

	private PathFinding pf;

	private LevelChunkDB chunkDB;

	public LevelPlacementHelper(LevelNode[,] nodes, LevelChunkDB chunkDB)
	{
		this.nodes = nodes;
		this.chunkDB = chunkDB;
		pf = new PathFinding(nodes);
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].ContainsObjectType(LevelNodeObjectType.StartFlag))
				{
					startNode = nodes[i, j];
				}
				if (nodes[i, j].ContainsObjectType(LevelNodeObjectType.EndFlag))
				{
					endNode = nodes[i, j];
				}
				if (startNode != null && endNode != null)
				{
					break;
				}
			}
			if (startNode != null && endNode != null)
			{
				break;
			}
		}
	}

	public LevelNode GetClosestFreeNode(LevelNode node, LevelNodeObjectType type, int range)
	{
		if (EvaluateNode(node, type))
		{
			return node;
		}
		LevelNode result = null;
		int num = int.MaxValue;
		int num2 = Mathf.Max(0, node.Row - range);
		int num3 = Mathf.Min(nodes.GetLength(0) - 1, node.Row + range);
		int num4 = Mathf.Max(0, node.Col - range);
		int num5 = Mathf.Min(nodes.GetLength(1) - 1, node.Col + range);
		for (int i = node.Row; i <= num3; i++)
		{
			if (EvaluateNode(nodes[i, node.Col], type))
			{
				int num6 = i - node.Row;
				if (num6 < num)
				{
					num = num6;
					result = nodes[i, node.Col];
				}
			}
		}
		for (int num7 = node.Row; num7 >= num2; num7--)
		{
			if (EvaluateNode(nodes[num7, node.Col], type))
			{
				int num8 = node.Row - num7;
				if (num8 < num || (num8 == num && Random.value <= 0.25f))
				{
					num = num8;
					result = nodes[num7, node.Col];
				}
			}
		}
		for (int j = node.Col; j <= num5; j++)
		{
			if (EvaluateNode(nodes[node.Row, j], type))
			{
				int num9 = j - node.Col;
				if (num9 < num || (num9 == num && Random.value <= 0.25f))
				{
					num = num9;
					result = nodes[node.Row, j];
				}
			}
		}
		for (int num10 = node.Col; num10 >= num4; num10--)
		{
			if (EvaluateNode(nodes[node.Row, num10], type))
			{
				int num11 = node.Col - num10;
				if (num11 < num || (num11 == num && Random.value <= 0.25f))
				{
					num = num11;
					result = nodes[node.Row, num10];
				}
			}
		}
		return result;
	}

	public bool EvaluateNode(LevelNode node, LevelNodeObjectType type)
	{
		if (node.ObjectTypes.Count > 0)
		{
			return false;
		}
		if (node.Type == LevelNodeType.None)
		{
			return false;
		}
		switch (type)
		{
		case LevelNodeObjectType.BearTrap:
		{
			if (node.Type == LevelNodeType.House)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			LevelNode neighbour2 = node.GetNeighbour(nodes, LevelDirection.South);
			if (neighbour2 != null && neighbour2.ObjectTypes.Count > 0 && neighbour2.ContainsObjectType(LevelNodeObjectType.BellTower))
			{
				return false;
			}
			if (IsNodeOnMainPath(node))
			{
				return false;
			}
			break;
		}
		case LevelNodeObjectType.BellTower:
		{
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (node.Type == LevelNodeType.House)
			{
				return false;
			}
			if (IsNeighbourToHouse(node))
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (!HasNeighboursWithSameElevation(node))
			{
				return false;
			}
			LevelNode neighbour3 = node.GetNeighbour(nodes, LevelDirection.North);
			if (neighbour3 != null && neighbour3.ObjectTypes.Count > 0)
			{
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.BearTrap))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.CampFire))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.DoorTrigger))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.EndFlag))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.GoldChest))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.Key))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.MetalChest))
				{
					return false;
				}
				if (neighbour3.ContainsObjectType(LevelNodeObjectType.WoodChest))
				{
					return false;
				}
				if (neighbour3.HasLadderAnySizeAnyDirection())
				{
					return false;
				}
			}
			LevelNode neighbour4 = node.GetNeighbour(nodes, LevelDirection.South);
			if (neighbour4 != null && neighbour4.ObjectTypes.Count > 0)
			{
				if (neighbour4.ContainsObjectType(LevelNodeObjectType.Tree))
				{
					return false;
				}
				if (neighbour4.ContainsObjectType(LevelNodeObjectType.BellTower))
				{
					return false;
				}
			}
			if (IsNodeOnMainPath(node))
			{
				return false;
			}
			if (IsNodeOnSideQuestPath(node))
			{
				return false;
			}
			break;
		}
		case LevelNodeObjectType.Bush:
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (node.Type == LevelNodeType.House)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeOnMainPath(node))
			{
				return false;
			}
			if (IsNodeOnSideQuestPath(node))
			{
				return false;
			}
			break;
		case LevelNodeObjectType.CampFire:
			if (node.Type == LevelNodeType.House)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			break;
		case LevelNodeObjectType.DoorTrigger:
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			if (!PathFromStartExists(node, false))
			{
				return false;
			}
			if (!IsSouthNeighbourEmpty(node))
			{
				return false;
			}
			break;
		case LevelNodeObjectType.WoodChest:
		case LevelNodeObjectType.MetalChest:
		case LevelNodeObjectType.GoldChest:
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			if (!PathFromStartExists(node, true))
			{
				return false;
			}
			if (node.Type == LevelNodeType.EndBossArea)
			{
				return false;
			}
			break;
		case LevelNodeObjectType.CrystalShard:
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			if (!PathFromStartExists(node, true))
			{
				return false;
			}
			if (!IsNorthNeighbourEmpty(node))
			{
				return false;
			}
			if (node.Type == LevelNodeType.EndBossArea)
			{
				return false;
			}
			break;
		case LevelNodeObjectType.Key:
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			if (!PathFromStartExists(node, false))
			{
				return false;
			}
			if (!IsSouthNeighbourEmpty(node))
			{
				return false;
			}
			break;
		case LevelNodeObjectType.LampPost:
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (node.Type == LevelNodeType.House)
			{
				return false;
			}
			if (IsNeighbourToHouse(node))
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeOnMainPath(node))
			{
				return false;
			}
			if (IsNodeOnChokePoint(node))
			{
				return false;
			}
			if (!DoesNodeHaveFreePathAround(node) && !IsNodeAtBoundry(node))
			{
				return false;
			}
			if (!IsNorthNeighbourEmpty(node))
			{
				return false;
			}
			if (IsNodeEndLevelArea(node))
			{
				return false;
			}
			if (DoesBlockKeyOrLever(node))
			{
				return false;
			}
			break;
		case LevelNodeObjectType.Studs:
			if (IsNodeBehindWall(node))
			{
				return false;
			}
			if (IsNodeBehindTile(node))
			{
				return false;
			}
			if (!PathFromStartExists(node, true))
			{
				return false;
			}
			break;
		case LevelNodeObjectType.Tree:
		{
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			if (node.Type == LevelNodeType.House)
			{
				return false;
			}
			if (IsNeighbourToHouse(node))
			{
				return false;
			}
			if (DoesNodeTouchLadder(node))
			{
				return false;
			}
			if (IsNodeOnChokePoint(node))
			{
				return false;
			}
			LevelNode neighbour = node.GetNeighbour(nodes, LevelDirection.North);
			if (neighbour != null && neighbour.ObjectTypes.Count > 0)
			{
				if (neighbour.ContainsObjectType(LevelNodeObjectType.BellTower))
				{
					return false;
				}
				if (neighbour.ContainsObjectType(LevelNodeObjectType.CampFire))
				{
					return false;
				}
				if (neighbour.ContainsObjectType(LevelNodeObjectType.DoorTrigger))
				{
					return false;
				}
				if (neighbour.ContainsObjectType(LevelNodeObjectType.EndFlag))
				{
					return false;
				}
				if (neighbour.ContainsObjectType(LevelNodeObjectType.Tree))
				{
					return false;
				}
				if (neighbour.HasLadderAnySizeAnyDirection())
				{
					return false;
				}
			}
			if (IsNodeOnMainPath(node))
			{
				return false;
			}
			if (IsNodeOnSideQuestPath(node))
			{
				return false;
			}
			if (!DoesNodeHaveFreePathAround(node) && !IsNodeAtBoundry(node))
			{
				return false;
			}
			if (DoesBlockKeyOrLever(node))
			{
				return false;
			}
			break;
		}
		default:
			Debug.LogError("Unhandled NodeObjectType in EvaluateNode");
			break;
		case LevelNodeObjectType.GruntEnemy:
			break;
		}
		return true;
	}

	public bool DoesBlockKeyOrLever(LevelNode node)
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbour = node.GetNeighbour(nodes, levelDirection);
			if (neighbour == null || neighbour.Elevation != node.Elevation)
			{
				continue;
			}
			if (neighbour.ContainsObjectType(LevelNodeObjectType.Key))
			{
				int elevation = node.Elevation;
				node.Elevation = 100;
				bool flag = true;
				if (pf.GetPathIncludeDoors(nodes[(int)chunkDB.GetChunk<MainPathChunk>().Path.from.y, (int)chunkDB.GetChunk<MainPathChunk>().Path.from.x], neighbour, null) == null)
				{
					flag = false;
				}
				node.Elevation = elevation;
				if (!flag)
				{
					return true;
				}
			}
			if (neighbour.ContainsObjectType(LevelNodeObjectType.DoorTrigger))
			{
				int elevation2 = node.Elevation;
				node.Elevation = 100;
				bool flag2 = true;
				if (pf.GetPathIgnoreDoors(nodes[(int)chunkDB.GetChunk<MainPathChunk>().Path.from.y, (int)chunkDB.GetChunk<MainPathChunk>().Path.from.x], neighbour, null) == null)
				{
					flag2 = false;
				}
				node.Elevation = elevation2;
				if (!flag2)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsNeighbourToHouse(LevelNode node)
	{
		foreach (LevelNode neighbour in LevelHelper.GetNeighbours(new LevelPoint(node.Row, node.Col), nodes, true))
		{
			if (neighbour.Type == LevelNodeType.House)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSouthNeighbourEmpty(LevelNode node)
	{
		if (LevelHelper.PointInsideBounds(new LevelPoint(node.Row - 1, node.Col), nodes))
		{
			LevelNode levelNode = nodes[node.Row - 1, node.Col];
			if (levelNode != null && levelNode.ObjectTypes.Count == 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsNorthNeighbourEmpty(LevelNode node)
	{
		if (LevelHelper.PointInsideBounds(new LevelPoint(node.Row + 1, node.Col), nodes))
		{
			LevelNode levelNode = nodes[node.Row + 1, node.Col];
			if (levelNode != null && levelNode.ObjectTypes.Count == 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsNodeOnChokePoint(LevelNode node)
	{
		if (node.ObjectTypes.Count > 0)
		{
			return true;
		}
		node.AddObjectType(LevelNodeObjectType.LampPost, chunkDB, string.Empty);
		bool flag = PathFromStartToEndExists(true);
		node.ClearObjectTypes(chunkDB);
		return !flag;
	}

	public bool DoesNodeHaveFreePathAround(LevelNode node)
	{
		LevelNode levelNode = null;
		LevelNode levelNode2 = null;
		levelNode = node.GetNeighbour(nodes, LevelDirection.West);
		if (levelNode == null || levelNode.Elevation != node.Elevation)
		{
			if (LevelHelper.PointInsideBounds(new LevelPoint(node.Row - 1, node.Col - 1), nodes))
			{
				levelNode = nodes[node.Row - 1, node.Col - 1];
			}
			if ((levelNode == null || levelNode.Elevation != node.Elevation) && LevelHelper.PointInsideBounds(new LevelPoint(node.Row + 1, node.Col - 1), nodes))
			{
				levelNode = nodes[node.Row + 1, node.Col - 1];
			}
		}
		if (levelNode == null || levelNode.Elevation != node.Elevation)
		{
			return false;
		}
		levelNode2 = node.GetNeighbour(nodes, LevelDirection.East);
		if (levelNode2 == null || levelNode2.Elevation != node.Elevation)
		{
			if (LevelHelper.PointInsideBounds(new LevelPoint(node.Row - 1, node.Col + 1), nodes))
			{
				levelNode2 = nodes[node.Row - 1, node.Col + 1];
			}
			if ((levelNode2 == null || levelNode2.Elevation != node.Elevation) && LevelHelper.PointInsideBounds(new LevelPoint(node.Row + 1, node.Col + 1), nodes))
			{
				levelNode2 = nodes[node.Row + 1, node.Col + 1];
			}
		}
		if (levelNode2 == null || levelNode2.Elevation != node.Elevation)
		{
			return false;
		}
		int elevation = node.Elevation;
		node.Elevation = 100;
		PathFinding.Node[] pathIgnoreDoors = pf.GetPathIgnoreDoors(levelNode, levelNode2, null);
		node.Elevation = elevation;
		if (pathIgnoreDoors == null)
		{
			return false;
		}
		if (pathIgnoreDoors.Length <= 9)
		{
			return true;
		}
		return false;
	}

	public bool IsNodeAtBoundry(LevelNode node)
	{
		LevelNode[] array = LevelHelper.GetNeighbours(new LevelPoint(node.Row, node.Col), nodes, true).ToArray();
		int num = 0;
		LevelNode[] array2 = array;
		foreach (LevelNode levelNode in array2)
		{
			if (levelNode == null || levelNode.Elevation != node.Elevation)
			{
				num++;
			}
		}
		return num >= 3;
	}

	public bool DoesNodeTouchLadder(LevelNode node)
	{
		if (node.HasLadderAnySizeAnyDirection())
		{
			return true;
		}
		if (node.HasNeighbourLadderAnySize(nodes))
		{
			return true;
		}
		return false;
	}

	public bool IsNodeBehindTile(LevelNode node)
	{
		LevelNode neighbour = node.GetNeighbour(nodes, LevelDirection.South);
		if (neighbour != null && neighbour.Elevation > node.Elevation)
		{
			return true;
		}
		if (node.Row >= 2)
		{
			LevelNode levelNode = nodes[node.Row - 2, node.Col];
			if (levelNode != null && levelNode.Elevation > node.Elevation + 1)
			{
				return true;
			}
			if (levelNode != null && levelNode.Elevation > node.Elevation && levelNode.ObjectTypes.Count > 0)
			{
				return true;
			}
			if (node.Col > 0)
			{
				LevelNode levelNode2 = nodes[node.Row - 2, node.Col - 1];
				if (levelNode2 != null && levelNode2.Elevation > node.Elevation + 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsNodeBehindWall(LevelNode node)
	{
		if (node.HasWallAnyType(LevelDirection.South))
		{
			return true;
		}
		LevelNode neighbour = node.GetNeighbour(nodes, LevelDirection.South);
		if (neighbour != null && neighbour.HasWallAnyType(LevelDirection.North))
		{
			return true;
		}
		return false;
	}

	public bool PathFromStartExists(LevelNode node, bool withKey)
	{
		PathFinding.Node[] array = null;
		array = ((!withKey) ? pf.GetPathIncludeDoors(startNode, node, null) : pf.GetPathIgnoreDoors(startNode, node, null));
		return array != null;
	}

	public bool PathToEndExists(LevelNode node, bool withKey)
	{
		PathFinding.Node[] array = null;
		array = ((!withKey) ? pf.GetPathIncludeDoors(node, endNode, null) : pf.GetPathIgnoreDoors(node, endNode, null));
		return array != null;
	}

	public bool PathFromStartToEndExists(bool withKey)
	{
		PathFinding.Node[] array = null;
		array = ((!withKey) ? pf.GetPathIncludeDoors(startNode, endNode, null) : pf.GetPathIgnoreDoors(startNode, endNode, null));
		return array != null;
	}

	public bool HasNeighboursWithSameElevation(LevelNode node)
	{
		LevelNode[] array = LevelHelper.GetNeighbours(new LevelPoint(node.Row, node.Col), nodes, true).ToArray();
		if (array.Length < 4)
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			if (array[i].Elevation != node.Elevation)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsNodeOnMainPath(LevelNode node)
	{
		MainPathChunk chunk = chunkDB.GetChunk<MainPathChunk>();
		float num = chunk.Path.ClosestDistanceToPoint(new LevelPoint(node.Row, node.Col));
		if (num < 1f)
		{
			return true;
		}
		return false;
	}

	public bool IsNodeOnSideQuestPath(LevelNode node)
	{
		SideQuestChunk[] array = chunkDB.GetChunks<SideQuestChunk>().ToArray();
		SideQuestChunk[] array2 = array;
		foreach (SideQuestChunk sideQuestChunk in array2)
		{
			if (sideQuestChunk.Path.ClosestDistanceToPoint(new LevelPoint(node.Row, node.Col)) < 1f)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsNodeEndLevelArea(LevelNode node)
	{
		if (node.Type == LevelNodeType.EndBossArea)
		{
			return true;
		}
		return false;
	}

	public bool EvaluateNodeEdge(LevelNode node, LevelNodeEdgeValue type, LevelDirection edge)
	{
		LevelNode neighbour = node.GetNeighbour(nodes, edge);
		switch (type)
		{
		case LevelNodeEdgeValue.KeyDoor:
		case LevelNodeEdgeValue.TriggeredDoor:
		case LevelNodeEdgeValue.AltKeyDoor:
		case LevelNodeEdgeValue.ElementalDoor_earth:
		case LevelNodeEdgeValue.ElementalDoor_electricity:
		case LevelNodeEdgeValue.ElementalDoor_energy:
		case LevelNodeEdgeValue.ElementalDoor_fire:
		case LevelNodeEdgeValue.ElementalDoor_ice:
		case LevelNodeEdgeValue.ElementalDoor_water:
			if (neighbour == null)
			{
				return false;
			}
			if (neighbour.Elevation != node.Elevation)
			{
				return false;
			}
			if (node.HasLadderAnySize(edge) || neighbour.HasLadderAnySize(LevelHelper.OppositeDirection(edge)))
			{
				return false;
			}
			break;
		case LevelNodeEdgeValue.Wall:
		case LevelNodeEdgeValue.WallWithHidingSpot:
		case LevelNodeEdgeValue.AltWall:
			if (node.HasLadderAnySize(edge) || neighbour.HasLadderAnySize(LevelHelper.OppositeDirection(edge)))
			{
				return false;
			}
			if (node.IsAIPatrolNode)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public LevelNode GetNodeFarthestFrom(LevelNode point, bool ignoreOccupied)
	{
		int num = 0;
		LevelNode result = null;
		LevelNode[,] array = nodes;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = array[i, j];
				PathFinding.Node[] pathIgnoreDoors = pf.GetPathIgnoreDoors(levelNode, point, null);
				if (pathIgnoreDoors != null && pathIgnoreDoors.Length > num && (ignoreOccupied || levelNode.ObjectTypes.Count == 0))
				{
					num = pathIgnoreDoors.Length;
					result = levelNode;
				}
			}
		}
		return result;
	}

	public LevelNode GetNodeFarthestFrom(LevelNode point, bool ignoreOccupied, LevelNodeType type)
	{
		int num = 0;
		LevelNode result = null;
		LevelNode[,] array = nodes;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = array[i, j];
				if (levelNode.Type == type)
				{
					PathFinding.Node[] pathIgnoreDoors = pf.GetPathIgnoreDoors(levelNode, point, null);
					if (pathIgnoreDoors != null && pathIgnoreDoors.Length > num && (ignoreOccupied || levelNode.ObjectTypes.Count == 0))
					{
						num = pathIgnoreDoors.Length;
						result = levelNode;
					}
				}
			}
		}
		return result;
	}

	public LevelNode GetBestLootNode(List<LevelNode> lootList, LevelNodeObjectType lootType)
	{
		LevelPoint startPoint = new LevelPoint(Mathf.RoundToInt(chunkDB.GetChunk<MainPathChunk>().Path.from.y), Mathf.RoundToInt(chunkDB.GetChunk<MainPathChunk>().Path.from.x));
		LevelPoint levelPoint = new LevelPoint(Mathf.RoundToInt(chunkDB.GetChunk<MainPathChunk>().Path.to.y), Mathf.RoundToInt(chunkDB.GetChunk<MainPathChunk>().Path.to.x));
		LevelQuads levelQuads = new LevelQuads(nodes, pf, startPoint);
		List<LevelQuad> list = levelQuads.AddLootInfo(lootList);
		Vector3 a = new Vector3(chunkDB.GetChunk<MainPathChunk>().Path.from.x, 0f, chunkDB.GetChunk<MainPathChunk>().Path.from.y);
		Vector3 a2 = new Vector3(chunkDB.GetChunk<MainPathChunk>().Path.to.x, 0f, chunkDB.GetChunk<MainPathChunk>().Path.to.y);
		float num = 0f;
		LevelQuad levelQuad = null;
		LevelQuad[,] quads = levelQuads.quads;
		int length = quads.GetLength(0);
		int length2 = quads.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelQuad levelQuad2 = quads[i, j];
				if (levelQuad2.numLoot != 0 || levelQuad2.nodes.Count <= 0 || !levelQuad2.pathFromStartExists || !levelQuad2.CanHoldLoot(this, lootType))
				{
					continue;
				}
				float num2 = Vector3.Distance(a, levelQuad2.center);
				float num3 = Vector3.Distance(a2, levelQuad2.center);
				if (!(num3 > 3f))
				{
					continue;
				}
				float num4 = num2;
				foreach (LevelQuad item in list)
				{
					float num5 = Vector3.Distance(levelQuad2.center, item.center);
					if (num5 < num4)
					{
						num4 = num5;
					}
				}
				if (num4 > num)
				{
					num = num4;
					levelQuad = levelQuad2;
				}
			}
		}
		if (levelQuad == null)
		{
			Debug.LogWarning("Cannot place loot, as no suited LevelQuad was found");
			return null;
		}
		int num6 = 0;
		LevelNode result = null;
		foreach (LevelNode node in levelQuad.nodes)
		{
			if (!EvaluateNode(node, lootType))
			{
				continue;
			}
			PathFinding.Node[] pathIgnoreDoors = pf.GetPathIgnoreDoors(nodes[startPoint.row, startPoint.col], node, null);
			PathFinding.Node[] pathIgnoreDoors2 = pf.GetPathIgnoreDoors(nodes[levelPoint.row, levelPoint.col], node, null);
			if (pathIgnoreDoors == null || pathIgnoreDoors2 == null)
			{
				continue;
			}
			int num7 = Mathf.Min(pathIgnoreDoors.Length, pathIgnoreDoors2.Length);
			foreach (LevelNode loot in lootList)
			{
				PathFinding.Node[] pathIgnoreDoors3 = pf.GetPathIgnoreDoors(node, loot, null);
				if (pathIgnoreDoors3 != null && pathIgnoreDoors3.Length < num7)
				{
					num7 = pathIgnoreDoors3.Length;
				}
			}
			if (num7 > num6)
			{
				num6 = num7;
				result = node;
			}
		}
		return result;
	}
}
