using System;
using System.Collections.Generic;
using UnityEngine;

public static class LevelHelper
{
	public static float Distance(LevelNode a, LevelNode b)
	{
		Vector2 a2 = new Vector2(a.Col, a.Row);
		Vector2 b2 = new Vector2(b.Col, b.Row);
		return Vector2.Distance(a2, b2);
	}

	public static float Distance(LevelPoint a, LevelPoint b)
	{
		Vector2 a2 = new Vector2(a.col, a.row);
		Vector2 b2 = new Vector2(b.col, b.row);
		return Vector2.Distance(a2, b2);
	}

	public static List<LevelNode> GetNeighbours(LevelPoint point, LevelNode[,] nodes, bool excludeTypeNone = false)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbourAt = GetNeighbourAt(point, levelDirection, nodes, excludeTypeNone);
			if (neighbourAt != null && (!excludeTypeNone || neighbourAt.Type != LevelNodeType.None))
			{
				list.Add(neighbourAt);
			}
		}
		return list;
	}

	public static List<LevelNode> GetNeighboursOfType(LevelPoint point, LevelNodeType type, LevelNode[,] nodes)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbourAt = GetNeighbourAt(point, levelDirection, nodes);
			if (neighbourAt != null && neighbourAt.Elevation > 0 && neighbourAt.Type == type)
			{
				list.Add(neighbourAt);
			}
		}
		return list;
	}

	public static LevelDirection GetLevelDirectionForVector(Vector3 vector)
	{
		if (vector.z == 0f)
		{
			return (!(vector.x < 0f)) ? LevelDirection.East : LevelDirection.West;
		}
		if (vector.x == 0f)
		{
			return (vector.z < 0f) ? LevelDirection.South : LevelDirection.North;
		}
		float num = vector.x / vector.z;
		if (num > 1f)
		{
			return (!(vector.x < 0f)) ? LevelDirection.East : LevelDirection.West;
		}
		if (num >= 0f && num < 1f)
		{
			return (vector.x < 0f) ? LevelDirection.South : LevelDirection.North;
		}
		if (num < 0f && num >= -1f)
		{
			return (!(vector.x < 0f)) ? LevelDirection.South : LevelDirection.North;
		}
		return (!(vector.x < 0f)) ? LevelDirection.East : LevelDirection.West;
	}

	public static LevelNode GetNeighbourAt(LevelPoint point, LevelDirection direction, LevelNode[,] nodes, bool excludeTypeNone = false)
	{
		if (!PointInsideBounds(point, nodes))
		{
			return null;
		}
		if (point.row == 0 && direction == LevelDirection.South)
		{
			return null;
		}
		if (point.row == nodes.GetLength(0) - 1 && direction == LevelDirection.North)
		{
			return null;
		}
		if (point.col == 0 && direction == LevelDirection.West)
		{
			return null;
		}
		if (point.col == nodes.GetLength(1) - 1 && direction == LevelDirection.East)
		{
			return null;
		}
		if (direction == LevelDirection.North && (!excludeTypeNone || nodes[point.row + 1, point.col].Type != LevelNodeType.None))
		{
			return nodes[point.row + 1, point.col];
		}
		if (direction == LevelDirection.South && (!excludeTypeNone || nodes[point.row - 1, point.col].Type != LevelNodeType.None))
		{
			return nodes[point.row - 1, point.col];
		}
		if (direction == LevelDirection.East && (!excludeTypeNone || nodes[point.row, point.col + 1].Type != LevelNodeType.None))
		{
			return nodes[point.row, point.col + 1];
		}
		if (direction == LevelDirection.West && (!excludeTypeNone || nodes[point.row, point.col - 1].Type != LevelNodeType.None))
		{
			return nodes[point.row, point.col - 1];
		}
		return null;
	}

	public static LevelDirection GetRightDirection(LevelDirection direction)
	{
		switch (direction)
		{
		case LevelDirection.North:
			return LevelDirection.East;
		case LevelDirection.West:
			return LevelDirection.North;
		case LevelDirection.South:
			return LevelDirection.West;
		default:
			return LevelDirection.South;
		}
	}

	public static LevelDirection GetLeftDirection(LevelDirection direction)
	{
		switch (direction)
		{
		case LevelDirection.North:
			return LevelDirection.West;
		case LevelDirection.West:
			return LevelDirection.South;
		case LevelDirection.South:
			return LevelDirection.East;
		default:
			return LevelDirection.North;
		}
	}

	public static bool PointInsideBounds(LevelPoint point, LevelNode[,] nodes)
	{
		return PointInsideBounds(point.row, point.col, nodes);
	}

	public static bool PointInsideBounds(int row, int col, LevelNode[,] nodes)
	{
		if (row < 0)
		{
			return false;
		}
		if (row >= nodes.GetLength(0))
		{
			return false;
		}
		if (col < 0)
		{
			return false;
		}
		if (col >= nodes.GetLength(1))
		{
			return false;
		}
		return true;
	}

	public static void ClampToBounds(ref int row, ref int col, LevelNode[,] nodes)
	{
		LevelPoint point = new LevelPoint(row, col);
		point = ClampToBounds(point, nodes);
		row = point.row;
		col = point.col;
	}

	public static LevelPoint ClampToBounds(LevelPoint point, LevelNode[,] nodes)
	{
		LevelPoint result = new LevelPoint(point);
		if (result.row < 0)
		{
			result.row = 0;
		}
		if (result.row >= nodes.GetLength(0))
		{
			result.row = nodes.GetLength(0) - 1;
		}
		if (result.col < 0)
		{
			result.col = 0;
		}
		if (result.col >= nodes.GetLength(1))
		{
			result.col = nodes.GetLength(1) - 1;
		}
		return result;
	}

	public static bool PointInsideEllipse(LevelPoint center, int rowDiameter, int colDiameter, LevelPoint point)
	{
		float num = Mathf.Cos(0f);
		float num2 = Mathf.Sin(0f);
		float num3 = (float)rowDiameter / 2f * ((float)rowDiameter / 2f);
		float num4 = (float)colDiameter / 2f * ((float)colDiameter / 2f);
		float num5 = Mathf.Pow(num * ((float)point.row - (float)center.row) + num2 * ((float)point.col - (float)center.col), 2f);
		float num6 = Mathf.Pow(num2 * ((float)point.row - (float)center.row) - num * ((float)point.col - (float)center.col), 2f);
		float num7 = num5 / num3 + num6 / num4;
		if (num7 <= 1f)
		{
			return true;
		}
		return false;
	}

	public static LevelDirection OppositeDirection(LevelDirection dir)
	{
		switch (dir)
		{
		case LevelDirection.North:
			return LevelDirection.South;
		case LevelDirection.South:
			return LevelDirection.North;
		case LevelDirection.East:
			return LevelDirection.West;
		case LevelDirection.West:
			return LevelDirection.East;
		default:
			Debug.LogError("Garbage data received in LevelHelper.OppositeDirection");
			return dir;
		}
	}

	public static int GetFirstFilledRowAtCol(LevelNode[,] nodes, int col)
	{
		if (col < 0 || col >= nodes.GetLength(1))
		{
			return -1;
		}
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			if (nodes[i, col].Elevation != 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static int GetLastFilledRowAtCol(LevelNode[,] nodes, int col)
	{
		if (col < 0 || col >= nodes.GetLength(1))
		{
			return -1;
		}
		for (int num = nodes.GetLength(0) - 1; num >= 0; num--)
		{
			if (nodes[num, col].Elevation != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public static int GetFirstFilledRowAtCol(LevelNode[,] nodes, int col, LevelNodeType exclude)
	{
		if (col < 0 || col >= nodes.GetLength(1))
		{
			return -1;
		}
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			if (nodes[i, col].Elevation != 0 && nodes[i, col].Type != exclude)
			{
				return i;
			}
		}
		return -1;
	}

	public static int GetLastFilledRowAtCol(LevelNode[,] nodes, int col, LevelNodeType exclude)
	{
		if (col < 0 || col >= nodes.GetLength(1))
		{
			return -1;
		}
		for (int num = nodes.GetLength(0) - 1; num >= 0; num--)
		{
			if (nodes[num, col].Elevation != 0 && nodes[num, col].Type != exclude)
			{
				return num;
			}
		}
		return -1;
	}

	public static int GetFirstFilledColAtRow(LevelNode[,] nodes, int row)
	{
		if (row < 0 || row >= nodes.GetLength(0))
		{
			return -1;
		}
		for (int i = 0; i < nodes.GetLength(1); i++)
		{
			if (nodes[row, i].Elevation != 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static int GetLastFilledColAtRow(LevelNode[,] nodes, int row)
	{
		if (row < 0 || row >= nodes.GetLength(0))
		{
			return -1;
		}
		for (int num = nodes.GetLength(1) - 1; num >= 0; num--)
		{
			if (nodes[row, num].Elevation != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public static Vector3 NodeToVector3(LevelNode node)
	{
		if (node == null)
		{
			Debug.LogError("Cannot convert NULL to Vector3 - returning Vector3.zero");
			return Vector3.zero;
		}
		return new Vector3(node.Col, node.Elevation, node.Row);
	}

	public static LevelNode Vector3ToNode(Vector3 pos, LevelNode[,] nodes)
	{
		int num = Mathf.RoundToInt(pos.z);
		int num2 = Mathf.RoundToInt(pos.x);
		if (PointInsideBounds(new LevelPoint(num, num2), nodes))
		{
			return nodes[num, num2];
		}
		return null;
	}

	public static LevelPoint PositionToPoint(Vector3 position)
	{
		int col = Mathf.RoundToInt(position.x);
		int row = Mathf.RoundToInt(position.z);
		return new LevelPoint(row, col);
	}

	public static void PositionToNode(Vector3 position, out int col, out int row)
	{
		col = Mathf.RoundToInt(position.x);
		row = Mathf.RoundToInt(position.z);
	}

	public static void PositionToNode(Vector3 position, out int col, out int row, out int elevation)
	{
		col = Mathf.RoundToInt(position.x);
		row = Mathf.RoundToInt(position.z);
		elevation = Mathf.RoundToInt(position.y);
	}

	public static bool LineRectangleIntersection2d(Vector2 from, Vector2 to, float rMinX, float rMinY, float rMaxX, float rMaxY)
	{
		float num = from.x;
		float num2 = to.x;
		if (from.x > to.x)
		{
			num = to.x;
			num2 = from.x;
		}
		if (num2 > rMaxX)
		{
			num2 = rMaxX;
		}
		if (num < rMinX)
		{
			num = rMinX;
		}
		if (num > num2)
		{
			return false;
		}
		float num3 = from.y;
		float num4 = to.y;
		float num5 = to.x - from.x;
		if ((double)Mathf.Abs(num5) > 1E-06)
		{
			float num6 = (to.y - from.y) / num5;
			float num7 = from.y - num6 * from.x;
			num3 = num6 * num + num7;
			num4 = num6 * num2 + num7;
		}
		if (num3 > num4)
		{
			float num8 = num4;
			num4 = num3;
			num3 = num8;
		}
		if (num4 > rMaxY)
		{
			num4 = rMaxY;
		}
		if (num3 < rMinY)
		{
			num3 = rMinY;
		}
		if (num3 > num4)
		{
			return false;
		}
		return true;
	}

	public static int RandomRangeToMax(int a, int b)
	{
		if (a == b)
		{
			return a;
		}
		if (b > a)
		{
			return UnityEngine.Random.Range(a, b + 1);
		}
		Debug.LogError("Wrong input to RandomRangeWithMax");
		return 0;
	}

	public static Quaternion OrientationForDirection(LevelDirection dir)
	{
		switch (dir)
		{
		default:
			return Quaternion.identity;
		case LevelDirection.West:
			return Quaternion.Euler(new Vector3(0f, -90f, 0f));
		case LevelDirection.South:
			return Quaternion.Euler(new Vector3(0f, 180f, 0f));
		case LevelDirection.East:
			return Quaternion.Euler(new Vector3(0f, 90f, 0f));
		}
	}

	public static LevelDirection? GetDirectionForNeighbor(LevelNode from, LevelNode to)
	{
		if (from.Row == to.Row)
		{
			if (from.Col < to.Col)
			{
				return LevelDirection.East;
			}
			if (from.Col > to.Col)
			{
				return LevelDirection.West;
			}
		}
		if (from.Col == to.Col)
		{
			if (from.Row < to.Row)
			{
				return LevelDirection.North;
			}
			if (from.Row > to.Row)
			{
				return LevelDirection.South;
			}
		}
		return null;
	}

	public static List<LevelNode> BuildPatrolPath(LevelNode startingNode, LevelNode[,] nodes)
	{
		List<LevelNode> list = new List<LevelNode>();
		list.Add(startingNode);
		List<LevelNode> list2 = new List<LevelNode>();
		list2.Add(startingNode);
		ExtendPatrolPath(list, startingNode, nodes, list2);
		Debug.DrawLine(NodeToVector3(startingNode), NodeToVector3(startingNode) + Vector3.up * 0.5f, Color.Lerp(Color.red, Color.yellow, 0.3f), 60f);
		return list;
	}

	public static void ExtendPatrolPath(List<LevelNode> waypoints, LevelNode startingNode, LevelNode[,] nodes, List<LevelNode> visitedNodes, LevelDirection? lastDirection = null)
	{
		List<LevelDirection> list = new List<LevelDirection>(Enum.GetValues(typeof(LevelDirection)) as LevelDirection[]);
		if (lastDirection.HasValue)
		{
			list.Remove(lastDirection.Value);
		}
		for (int num = list.Count - 1; num > 0; num--)
		{
			list[UnityEngine.Random.Range(0, num)] = list[num];
		}
		for (int i = 0; i < list.Count; i++)
		{
			LevelNode levelNode = null;
			int num2 = 0;
			List<LevelNode> list2 = new List<LevelNode>();
			LevelNode neighbourAt = GetNeighbourAt(startingNode.Location, list[i], nodes);
			while (num2 < 8 && neighbourAt != null && neighbourAt.levelPlateau == startingNode.levelPlateau && neighbourAt.Elevation == startingNode.Elevation && !NodeHasBoundary(neighbourAt, nodes, list[i]) && !visitedNodes.Contains(neighbourAt))
			{
				num2++;
				list2.Add(neighbourAt);
				levelNode = neighbourAt;
				neighbourAt = GetNeighbourAt(neighbourAt.Location, list[i], nodes);
			}
			if (num2 <= 1 || levelNode == null)
			{
				continue;
			}
			visitedNodes.AddRange(list2);
			foreach (LevelNode item in list2)
			{
				Debug.DrawLine(NodeToVector3(item), NodeToVector3(item) + Vector3.up * 0.3f, Color.Lerp(Color.red, Color.black, 0.3f), 60f);
			}
			waypoints.Add(levelNode);
			Debug.DrawLine(NodeToVector3(levelNode), NodeToVector3(levelNode) + Vector3.up * 0.5f, Color.red, 60f);
			if (waypoints.Count < 3)
			{
				ExtendPatrolPath(waypoints, levelNode, nodes, visitedNodes, list[i]);
			}
			break;
		}
	}

	public static bool NodeHasBoundary(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		if (node.GetNodeEdgeValue(direction) != LevelNodeEdgeValue.Empty)
		{
			return true;
		}
		LevelNode neighbourAt = GetNeighbourAt(node.Location, direction, nodes);
		return neighbourAt == null || neighbourAt.GetNodeEdgeValue(OppositeDirection(direction)) != LevelNodeEdgeValue.Empty || node.Elevation != neighbourAt.Elevation;
	}

	public static bool NodeHasSpinjitzuBoundry(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		if (node.GetNodeEdgeValue(direction) != LevelNodeEdgeValue.Empty && !node.HasLadderAnySize(direction))
		{
			return true;
		}
		if (node.HasLadderAnySize(direction))
		{
			return false;
		}
		LevelNode neighbourAt = GetNeighbourAt(node.Location, direction, nodes);
		if (neighbourAt != null && neighbourAt.HasLadderAnySize(OppositeDirection(direction)))
		{
			return false;
		}
		return neighbourAt == null || neighbourAt.GetNodeEdgeValue(OppositeDirection(direction)) != LevelNodeEdgeValue.Empty || node.Elevation != neighbourAt.Elevation;
	}

	public static bool NodeHasOpenDoor(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		return NodeHasDoorWithState(node, nodes, direction, Door.DoorState.Open);
	}

	public static bool NodeHasClosedDoor(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		return NodeHasDoorWithState(node, nodes, direction, Door.DoorState.Closed);
	}

	public static bool NodeHasDoorWithState(LevelNode node, LevelNode[,] nodes, LevelDirection direction, Door.DoorState doorState)
	{
		Door door = null;
		if (Door.IsDoorEdgeValue(node.GetNodeEdgeValue(direction)))
		{
			GameObject nodeEdgeObject = node.GetNodeEdgeObject(direction);
			door = ((!(nodeEdgeObject == null)) ? nodeEdgeObject.GetComponentInChildren<Door>() : door);
		}
		LevelNode neighbourAt = GetNeighbourAt(node.Location, direction, nodes);
		if (neighbourAt != null && Door.IsDoorEdgeValue(neighbourAt.GetNodeEdgeValue(OppositeDirection(direction))))
		{
			GameObject nodeEdgeObject2 = neighbourAt.GetNodeEdgeObject(OppositeDirection(direction));
			door = ((!(nodeEdgeObject2 == null)) ? nodeEdgeObject2.GetComponentInChildren<Door>() : door);
		}
		return door != null && door.State == doorState;
	}

	public static bool NodeHasBoundaryAnyDirection(LevelNode node, LevelNode[,] nodes)
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (NodeHasBoundary(node, nodes, levelDirection))
			{
				return true;
			}
		}
		return false;
	}

	public static bool NodeEdgeIsEmpty(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		if (node.GetNodeEdgeValue(direction) != LevelNodeEdgeValue.Empty)
		{
			return false;
		}
		return true;
	}

	public static bool NodeEdgeIsEmptyAnyDirection(LevelNode node, LevelNode[,] nodes)
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (!NodeEdgeIsEmpty(node, nodes, levelDirection))
			{
				return false;
			}
		}
		return true;
	}

	public static bool NodeHasLadder(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		if (node.HasLadderAnySize(direction))
		{
			return true;
		}
		LevelNode neighbourAt = GetNeighbourAt(node.Location, direction, nodes);
		if (neighbourAt != null && neighbourAt.Elevation < node.Elevation && neighbourAt.HasLadderAnySize(OppositeDirection(direction)))
		{
			return true;
		}
		return false;
	}

	public static bool NodeHasLadderAnyDirection(LevelNode node, LevelNode[,] nodes)
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (NodeHasLadder(node, nodes, levelDirection))
			{
				return true;
			}
		}
		return false;
	}

	public static Vector3 VectorForAngle(float angle)
	{
		return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
	}

	public static int GetMaxElevation(Level level)
	{
		int num = 0;
		LevelNode[,] nodes = level.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = nodes[i, j];
				if (levelNode.Elevation > num)
				{
					num = levelNode.Elevation;
				}
			}
		}
		return num;
	}
}
