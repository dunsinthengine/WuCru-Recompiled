using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
	public class Node
	{
		public int row;

		public int col;

		public LevelNode levelNode;

		public Node[] neighbourNodes;

		public Vector3 Position
		{
			get
			{
				return new Vector3(col, levelNode.Elevation, row);
			}
		}

		public override string ToString()
		{
			return string.Format("[Node: Position={0}]", Position);
		}
	}

	public struct IntVector2
	{
		public int x;

		public int y;

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static IntVector2 operator -(IntVector2 a, IntVector2 b)
		{
			return new IntVector2(a.x - b.x, a.y - b.y);
		}

		public static IntVector2 operator +(IntVector2 a, IntVector2 b)
		{
			return new IntVector2(a.x + b.x, a.y + b.y);
		}
	}

	private Dictionary<LevelNode, Node> nodeByLevelNode;

	private Node[,] nodes;

	private Node startNode;

	public Node[,] Nodes
	{
		get
		{
			return nodes;
		}
	}

	public Node StartNode
	{
		get
		{
			return startNode;
		}
	}

	public PathFinding(LevelNode[,] levelNodes)
	{
		nodes = new Node[levelNodes.GetLength(0), levelNodes.GetLength(1)];
		nodeByLevelNode = new Dictionary<LevelNode, Node>();
		for (int i = 0; i < levelNodes.GetLength(0); i++)
		{
			for (int j = 0; j < levelNodes.GetLength(1); j++)
			{
				LevelNode levelNode = levelNodes[i, j];
				Node node = new Node
				{
					row = i,
					col = j,
					levelNode = levelNode
				};
				nodes[i, j] = node;
				nodeByLevelNode.Add(node.levelNode, node);
				if (levelNode.ContainsObjectType(LevelNodeObjectType.StartFlag))
				{
					startNode = node;
				}
			}
		}
		for (int k = 0; k < nodes.GetLength(0); k++)
		{
			for (int l = 0; l < nodes.GetLength(1); l++)
			{
				Node node2 = nodes[k, l];
				List<Node> list = new List<Node>();
				if (k > 0)
				{
					Node item = nodes[k - 1, l];
					list.Add(item);
				}
				if (k < levelNodes.GetLength(0) - 1)
				{
					Node item2 = nodes[k + 1, l];
					list.Add(item2);
				}
				if (l > 0)
				{
					Node item3 = nodes[k, l - 1];
					list.Add(item3);
				}
				if (l < levelNodes.GetLength(1) - 1)
				{
					Node item4 = nodes[k, l + 1];
					list.Add(item4);
				}
				node2.neighbourNodes = list.ToArray();
			}
		}
	}

	public bool DoesNeighbourHaveAccess(LevelNode node, LevelNode neighbour, bool ignoreAllDoors = true)
	{
		return DoesNeighbourHaveAccess(node, neighbour, ignoreAllDoors, ignoreAllDoors, ignoreAllDoors);
	}

	public bool DoesNeighbourHaveAccess(LevelNode node, LevelNode neighbour, bool ignoreKeyDoors, bool ignoreTriggeredDoors, bool ignoreElementalDoors)
	{
		if (node.Type == LevelNodeType.None || neighbour.Type == LevelNodeType.None)
		{
			return false;
		}
		if (neighbour.ContainsObjectType(LevelNodeObjectType.LampPost))
		{
			return false;
		}
		if (neighbour.ContainsObjectType(LevelNodeObjectType.Tree))
		{
			return false;
		}
		int num = neighbour.Elevation - node.Elevation;
		int num2 = neighbour.Row - node.Row;
		int num3 = neighbour.Col - node.Col;
		if (Mathf.Abs(num2) > 1 || Mathf.Abs(num3) > 1)
		{
			throw new Exception("Nodes are not neighbours!");
		}
		if (!ignoreKeyDoors && ((num3 == -1 && (node.HasKeyDoor(LevelDirection.West) || neighbour.HasKeyDoor(LevelDirection.East))) || (num3 == 1 && (node.HasKeyDoor(LevelDirection.East) || neighbour.HasKeyDoor(LevelDirection.West))) || (num2 == -1 && (node.HasKeyDoor(LevelDirection.South) || neighbour.HasKeyDoor(LevelDirection.North))) || (num2 == 1 && (node.HasKeyDoor(LevelDirection.North) || neighbour.HasKeyDoor(LevelDirection.South)))))
		{
			return false;
		}
		if (!ignoreTriggeredDoors && ((num3 == -1 && (node.HasTriggeredDoor(LevelDirection.West) || neighbour.HasTriggeredDoor(LevelDirection.East))) || (num3 == 1 && (node.HasTriggeredDoor(LevelDirection.East) || neighbour.HasTriggeredDoor(LevelDirection.West))) || (num2 == -1 && (node.HasTriggeredDoor(LevelDirection.South) || neighbour.HasTriggeredDoor(LevelDirection.North))) || (num2 == 1 && (node.HasTriggeredDoor(LevelDirection.North) || neighbour.HasTriggeredDoor(LevelDirection.South)))))
		{
			return false;
		}
		if (!ignoreElementalDoors && ((num3 == -1 && (node.HasElementalDoor(LevelDirection.West) || neighbour.HasElementalDoor(LevelDirection.East))) || (num3 == 1 && (node.HasElementalDoor(LevelDirection.East) || neighbour.HasElementalDoor(LevelDirection.West))) || (num2 == -1 && (node.HasElementalDoor(LevelDirection.South) || neighbour.HasElementalDoor(LevelDirection.North))) || (num2 == 1 && (node.HasElementalDoor(LevelDirection.North) || neighbour.HasElementalDoor(LevelDirection.South)))))
		{
			return false;
		}
		switch (num)
		{
		case 0:
			if ((num3 == -1 && !node.HasWallAnyType(LevelDirection.West) && !neighbour.HasWallAnyType(LevelDirection.East)) || (num3 == 1 && !node.HasWallAnyType(LevelDirection.East) && !neighbour.HasWallAnyType(LevelDirection.West)) || (num2 == -1 && !node.HasWallAnyType(LevelDirection.South) && !neighbour.HasWallAnyType(LevelDirection.North)) || (num2 == 1 && !node.HasWallAnyType(LevelDirection.North) && !neighbour.HasWallAnyType(LevelDirection.South)))
			{
				return true;
			}
			break;
		case 1:
			if ((num3 == 1 && node.EastEdgeValue == LevelNodeEdgeValue.Ladder) || (num3 == -1 && node.WestEdgeValue == LevelNodeEdgeValue.Ladder) || (num2 == 1 && node.NorthEdgeValue == LevelNodeEdgeValue.Ladder) || (num2 == -1 && node.SouthEdgeValue == LevelNodeEdgeValue.Ladder))
			{
				return true;
			}
			break;
		case -1:
			if ((num3 == 1 && neighbour.WestEdgeValue == LevelNodeEdgeValue.Ladder) || (num3 == -1 && neighbour.EastEdgeValue == LevelNodeEdgeValue.Ladder) || (num2 == 1 && neighbour.SouthEdgeValue == LevelNodeEdgeValue.Ladder) || (num2 == -1 && neighbour.NorthEdgeValue == LevelNodeEdgeValue.Ladder))
			{
				return true;
			}
			break;
		case 2:
			if ((num3 == 1 && node.EastEdgeValue == LevelNodeEdgeValue.DoubleLadder) || (num3 == -1 && node.WestEdgeValue == LevelNodeEdgeValue.DoubleLadder) || (num2 == 1 && node.NorthEdgeValue == LevelNodeEdgeValue.DoubleLadder) || (num2 == -1 && node.SouthEdgeValue == LevelNodeEdgeValue.DoubleLadder))
			{
				return true;
			}
			break;
		case -2:
			if ((num3 == 1 && neighbour.WestEdgeValue == LevelNodeEdgeValue.DoubleLadder) || (num3 == -1 && neighbour.EastEdgeValue == LevelNodeEdgeValue.DoubleLadder) || (num2 == 1 && neighbour.SouthEdgeValue == LevelNodeEdgeValue.DoubleLadder) || (num2 == -1 && neighbour.NorthEdgeValue == LevelNodeEdgeValue.DoubleLadder))
			{
				return true;
			}
			break;
		case 3:
			if ((num3 == 1 && node.EastEdgeValue == LevelNodeEdgeValue.DownDrain) || (num3 == -1 && node.WestEdgeValue == LevelNodeEdgeValue.DownDrain) || (num2 == 1 && node.NorthEdgeValue == LevelNodeEdgeValue.DownDrain) || (num2 == -1 && node.SouthEdgeValue == LevelNodeEdgeValue.DownDrain))
			{
				return true;
			}
			break;
		case -3:
			if ((num3 == 1 && neighbour.WestEdgeValue == LevelNodeEdgeValue.DownDrain) || (num3 == -1 && neighbour.EastEdgeValue == LevelNodeEdgeValue.DownDrain) || (num2 == 1 && neighbour.SouthEdgeValue == LevelNodeEdgeValue.DownDrain) || (num2 == -1 && neighbour.NorthEdgeValue == LevelNodeEdgeValue.DownDrain))
			{
				return true;
			}
			break;
		case 4:
			if ((num3 == 1 && node.EastEdgeValue == LevelNodeEdgeValue.Tree) || (num3 == -1 && node.WestEdgeValue == LevelNodeEdgeValue.Tree) || (num2 == 1 && node.NorthEdgeValue == LevelNodeEdgeValue.Tree) || (num2 == -1 && node.SouthEdgeValue == LevelNodeEdgeValue.Tree))
			{
				return true;
			}
			break;
		case -4:
			if ((num3 == 1 && neighbour.WestEdgeValue == LevelNodeEdgeValue.Tree) || (num3 == -1 && neighbour.EastEdgeValue == LevelNodeEdgeValue.Tree) || (num2 == 1 && neighbour.SouthEdgeValue == LevelNodeEdgeValue.Tree) || (num2 == -1 && neighbour.NorthEdgeValue == LevelNodeEdgeValue.Tree))
			{
				return true;
			}
			break;
		}
		return false;
	}

	public Node[] GetPathIgnoreDoors(LevelNode start, LevelNode goal, Character character)
	{
		if (character == null || character.isPlayer)
		{
			return GetPath(start, goal, (Node node, Node neighbour) => DoesNeighbourHaveAccess(node.levelNode, neighbour.levelNode));
		}
		return GetPath(start, goal, (Node node, Node neighbour) => DoesNeighbourHaveAccess(node.levelNode, neighbour.levelNode, false));
	}

	public Node[] GetPathIncludeDoors(LevelNode start, LevelNode goal, Character character)
	{
		return GetPath(start, goal, (Node node, Node neighbour) => DoesNeighbourHaveAccess(node.levelNode, neighbour.levelNode, false));
	}

	public Node[] GetPathIgnoreDoors(LevelNode start, LevelNode goal, bool ignoreKeyDoors, bool ignoreTriggeredDoors, bool ignoreElementalDoors)
	{
		return GetPath(start, goal, (Node node, Node neighbour) => DoesNeighbourHaveAccess(node.levelNode, neighbour.levelNode, ignoreKeyDoors, ignoreTriggeredDoors, ignoreElementalDoors));
	}

	public Node[] GetPathWithKey(LevelNode start, LevelNode goal)
	{
		return GetPath(start, goal, (Node node, Node neighbour) => DoesNeighbourHaveAccess(node.levelNode, neighbour.levelNode, true, false, false));
	}

	private bool SmoothPathEvaluation(LevelNode currentNode, LevelNode testNode)
	{
		if (currentNode.Elevation == testNode.Elevation)
		{
			if (currentNode.Row > testNode.Row)
			{
				if (testNode.HasWallAnyType(LevelDirection.North))
				{
					return false;
				}
				if (testNode.HasWallAnyType(LevelDirection.South))
				{
					return false;
				}
				if (currentNode.HasWallAnyType(LevelDirection.South))
				{
					return false;
				}
			}
			if (currentNode.Row < testNode.Row)
			{
				if (testNode.HasWallAnyType(LevelDirection.South))
				{
					return false;
				}
				if (testNode.HasWallAnyType(LevelDirection.North))
				{
					return false;
				}
				if (currentNode.HasWallAnyType(LevelDirection.North))
				{
					return false;
				}
			}
			if (currentNode.Col < testNode.Col)
			{
				if (testNode.HasWallAnyType(LevelDirection.East))
				{
					return false;
				}
				if (testNode.HasWallAnyType(LevelDirection.West))
				{
					return false;
				}
				if (currentNode.HasWallAnyType(LevelDirection.East))
				{
					return false;
				}
			}
			if (currentNode.Col > testNode.Col)
			{
				if (testNode.HasWallAnyType(LevelDirection.West))
				{
					return false;
				}
				if (testNode.HasWallAnyType(LevelDirection.East))
				{
					return false;
				}
				if (currentNode.HasWallAnyType(LevelDirection.West))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public Node[] GetSmoothPath(Node[] originalNodes)
	{
		if (originalNodes == null || originalNodes.Length < 2)
		{
			return null;
		}
		List<Node> list = new List<Node>();
		list.Add(originalNodes[0]);
		int num = 0;
		for (int i = 1; i < originalNodes.Length; i++)
		{
			Node node = originalNodes[num];
			Node to = originalNodes[i];
			List<Node> list2 = IntersectingNodes(node, to);
			foreach (Node item in list2)
			{
				if (SmoothPathEvaluation(node.levelNode, item.levelNode))
				{
					continue;
				}
				bool flag = false;
				if (i < originalNodes.Length - 1)
				{
					int num2 = i + 1;
					int num3 = 0;
					bool flag2 = true;
					do
					{
						Node to2 = originalNodes[num2];
						List<Node> list3 = IntersectingNodes(node, to2);
						foreach (Node item2 in list3)
						{
							if (!SmoothPathEvaluation(node.levelNode, item2.levelNode))
							{
								flag2 = false;
								break;
							}
						}
						num3++;
						num2++;
					}
					while (!flag2 && num2 < originalNodes.Length);
					if (flag2)
					{
						flag = true;
						i += num3 - 1;
					}
				}
				if (!flag)
				{
					list.Add(originalNodes[i - 1]);
					list.Add(originalNodes[i]);
					num = i;
				}
				break;
			}
		}
		list.Add(originalNodes[originalNodes.Length - 1]);
		int num4 = 1;
		while (num4 < list.Count)
		{
			if (list[num4] == list[num4 - 1])
			{
				list.RemoveAt(num4);
			}
			else
			{
				num4++;
			}
		}
		return list.ToArray();
	}

	private List<Node> IntersectingNodes(Node from, Node to)
	{
		List<Node> list = new List<Node>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (LevelHelper.LineRectangleIntersection2d(new Vector2(from.col, from.row), new Vector2(to.col, to.row), (float)j - 0.49f, (float)i - 0.49f, (float)j + 0.49f, (float)i + 0.49f))
				{
					list.Add(nodes[i, j]);
				}
			}
		}
		return list;
	}

	public List<LevelNode> GetPathLevelNodes(LevelNode start, LevelNode goal, Func<Node, Node, bool> isReachablePredicate)
	{
		Node[] path = GetPath(start, goal, isReachablePredicate);
		if (path == null)
		{
			return null;
		}
		List<LevelNode> list = new List<LevelNode>();
		Node[] array = path;
		foreach (Node node in array)
		{
			list.Add(node.levelNode);
		}
		return list;
	}

	public Node[] GetPath(LevelNode start, LevelNode goal, Func<Node, Node, bool> isReachablePredicate)
	{
		if (goal == null)
		{
			return null;
		}
		HashSet<Node> hashSet = new HashSet<Node>();
		List<Node> list = new List<Node>();
		Node node = nodeByLevelNode[start];
		Node node2 = nodeByLevelNode[goal];
		Dictionary<Node, float> dictionary = new Dictionary<Node, float>();
		Update(dictionary, node, 0f);
		list.Add(node);
		Dictionary<Node, float> dictionary2 = new Dictionary<Node, float>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				dictionary2.Add(nodes[i, j], float.PositiveInfinity);
			}
		}
		Update(dictionary2, node, ManhattanDistance(node, node2));
		Dictionary<Node, Node> dictionary3 = new Dictionary<Node, Node>();
		while (list.Count > 0)
		{
			Node node3 = list[0];
			if (node3 == node2)
			{
				List<Node> list2 = new List<Node>();
				Node node4 = node2;
				list2.Add(node4);
				while (dictionary3.ContainsKey(node4))
				{
					node4 = dictionary3[node4];
					list2.Insert(0, node4);
				}
				return list2.ToArray();
			}
			list.RemoveAt(0);
			hashSet.Add(node3);
			Node[] neighbourNodes = node3.neighbourNodes;
			foreach (Node node5 in neighbourNodes)
			{
				if (!isReachablePredicate(node3, node5) || hashSet.Contains(node5))
				{
					continue;
				}
				float num = dictionary[node3] + Distance(node3, node5);
				if (list.Contains(node5) && !(num < dictionary[node5]))
				{
					continue;
				}
				Update(dictionary3, node5, node3);
				Update(dictionary, node5, num);
				float num2 = num + ManhattanDistance(node5, node2);
				Update(dictionary2, node5, num2);
				if (list.Contains(node5))
				{
					list.Remove(node5);
				}
				int num3 = -1;
				for (int l = 0; l < list.Count; l++)
				{
					if (dictionary2[list[l]] > num2)
					{
						num3 = l;
						break;
					}
				}
				if (num3 == -1)
				{
					list.Add(node5);
				}
				else
				{
					list.Insert(num3, node5);
				}
			}
		}
		return null;
	}

	private void Update<T, V>(Dictionary<T, V> dict, T t, V v)
	{
		if (dict.ContainsKey(t))
		{
			dict.Remove(t);
		}
		dict.Add(t, v);
	}

	private float ManhattanDistance(Node a, Node b)
	{
		return Mathf.Abs(a.row - b.row) + Mathf.Abs(a.col - b.col);
	}

	public float GetPathLength(Node[] path)
	{
		float num = 0f;
		for (int i = 0; i < path.Length - 1; i++)
		{
			num += Distance(path[i], path[i + 1]);
		}
		return num;
	}

	private float Distance(Node a, Node b)
	{
		return new Vector2(a.row - b.row, a.col - b.col).magnitude + (float)Mathf.Abs(a.levelNode.Elevation - b.levelNode.Elevation);
	}

	public Node GetNodeAtPosition(Vector3 position)
	{
		if (position.z < 0f || position.z >= (float)nodes.GetLength(0))
		{
			return null;
		}
		if (position.x < 0f || position.x >= (float)nodes.GetLength(1))
		{
			return null;
		}
		return nodes[(int)Mathf.Floor(position.z), (int)Mathf.Floor(position.x)];
	}
}
