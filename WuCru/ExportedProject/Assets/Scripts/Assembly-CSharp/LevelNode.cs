using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LevelNode
{
	private int chunkId;

	private int row;

	private int col;

	private Dictionary<LevelNodeTriangleFace, int[]> triangles = new Dictionary<LevelNodeTriangleFace, int[]>();

	private int elevation;

	private List<LevelNodeObjectTypeAndTag> objectTypes = new List<LevelNodeObjectTypeAndTag>();

	private List<GameObject> objects = new List<GameObject>();

	private List<CharacterCollider> objectCharacterColliders = new List<CharacterCollider>();

	private LevelNodeEdgeValue[] edgeValues = new LevelNodeEdgeValue[4];

	private GameObject[] edgeObjects = new GameObject[4];

	private LevelNodeType type;

	private bool isPathNode;

	private bool isAIPatrolNode;

	public bool isCheckedForIsland;

	public LevelPlateau levelPlateau;

	public LevelNodeType Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public int ChunkId
	{
		get
		{
			return chunkId;
		}
	}

	public int Elevation
	{
		get
		{
			return elevation;
		}
		set
		{
			elevation = value;
		}
	}

	public int Row
	{
		get
		{
			return row;
		}
	}

	public int Col
	{
		get
		{
			return col;
		}
	}

	public LevelPoint Location
	{
		get
		{
			return new LevelPoint(row, col);
		}
	}

	public bool IsPathNode
	{
		get
		{
			return isPathNode;
		}
	}

	public bool IsAIPatrolNode
	{
		get
		{
			return isAIPatrolNode;
		}
		set
		{
			isAIPatrolNode = value;
		}
	}

	public ReadOnlyCollection<LevelNodeObjectTypeAndTag> ObjectTypes
	{
		get
		{
			return objectTypes.AsReadOnly();
		}
	}

	public ReadOnlyCollection<GameObject> Objects
	{
		get
		{
			return objects.AsReadOnly();
		}
	}

	public ReadOnlyCollection<CharacterCollider> CharacterColliders
	{
		get
		{
			return objectCharacterColliders.AsReadOnly();
		}
	}

	public LevelNodeEdgeValue NorthEdgeValue
	{
		get
		{
			return edgeValues[0];
		}
		set
		{
			edgeValues[0] = value;
		}
	}

	public LevelNodeEdgeValue SouthEdgeValue
	{
		get
		{
			return edgeValues[1];
		}
		set
		{
			edgeValues[1] = value;
		}
	}

	public LevelNodeEdgeValue EastEdgeValue
	{
		get
		{
			return edgeValues[2];
		}
		set
		{
			edgeValues[2] = value;
		}
	}

	public LevelNodeEdgeValue WestEdgeValue
	{
		get
		{
			return edgeValues[3];
		}
		set
		{
			edgeValues[3] = value;
		}
	}

	public GameObject[] EdgeObjects
	{
		get
		{
			return edgeObjects;
		}
		set
		{
			edgeObjects = value;
		}
	}

	public GameObject NorthEdgeObject
	{
		get
		{
			return edgeObjects[0];
		}
		set
		{
			edgeObjects[0] = value;
		}
	}

	public GameObject SouthEdgeObject
	{
		get
		{
			return edgeObjects[1];
		}
		set
		{
			edgeObjects[1] = value;
		}
	}

	public GameObject EastEdgeObject
	{
		get
		{
			return edgeObjects[2];
		}
		set
		{
			edgeObjects[2] = value;
		}
	}

	public GameObject WestEdgeObject
	{
		get
		{
			return edgeObjects[3];
		}
		set
		{
			edgeObjects[3] = value;
		}
	}

	public LevelNode(int elevation, int row, int col)
	{
		this.elevation = elevation;
		this.row = row;
		this.col = col;
	}

	public int GetCustomHashCode(LevelNode[,] nodes)
	{
		return row + nodes.GetLength(0) * col;
	}

	public bool IsNeighbourToAIPatrolNode(LevelNode[,] nodes)
	{
		LevelNode[] array = LevelHelper.GetNeighbours(new LevelPoint(row, col), nodes, true).ToArray();
		LevelNode[] array2 = array;
		foreach (LevelNode levelNode in array2)
		{
			if (levelNode.isAIPatrolNode)
			{
				return true;
			}
		}
		return false;
	}

	public Ladder GetLadder()
	{
		for (int i = 0; i < EdgeObjects.Length; i++)
		{
			if (!(EdgeObjects[i] == null))
			{
				Ladder componentInChildren = EdgeObjects[i].GetComponentInChildren<Ladder>();
				if (componentInChildren != null)
				{
					return componentInChildren;
				}
			}
		}
		return null;
	}

	public bool HasLadderAnySize(LevelDirection edge)
	{
		if (edgeValues[(int)edge] == LevelNodeEdgeValue.Ladder || edgeValues[(int)edge] == LevelNodeEdgeValue.DoubleLadder || edgeValues[(int)edge] == LevelNodeEdgeValue.DownDrain || edgeValues[(int)edge] == LevelNodeEdgeValue.Tree)
		{
			return true;
		}
		return false;
	}

	public bool HasLadderAnySizeAnyDirection()
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (HasLadderAnySize(levelDirection))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasNeighbourLadderAnySize(LevelNode[,] nodes)
	{
		LevelNode neighbour = GetNeighbour(nodes, LevelDirection.North);
		LevelNode neighbour2 = GetNeighbour(nodes, LevelDirection.South);
		LevelNode neighbour3 = GetNeighbour(nodes, LevelDirection.East);
		LevelNode neighbour4 = GetNeighbour(nodes, LevelDirection.West);
		if (neighbour != null && neighbour.HasLadderAnySize(LevelDirection.South))
		{
			return true;
		}
		if (neighbour2 != null && neighbour2.HasLadderAnySize(LevelDirection.North))
		{
			return true;
		}
		if (neighbour3 != null && neighbour3.HasLadderAnySize(LevelDirection.West))
		{
			return true;
		}
		if (neighbour4 != null && neighbour4.HasLadderAnySize(LevelDirection.East))
		{
			return true;
		}
		return false;
	}

	public LevelNode GetNeighbourLadderNode(LevelNode[,] nodes, LevelDirection direction)
	{
		LevelNode neighbour = GetNeighbour(nodes, direction);
		if (neighbour == null)
		{
			return null;
		}
		switch (direction)
		{
		case LevelDirection.North:
			if (neighbour.HasLadderAnySize(LevelDirection.South))
			{
				return neighbour;
			}
			break;
		case LevelDirection.South:
			if (neighbour.HasLadderAnySize(LevelDirection.North))
			{
				return neighbour;
			}
			break;
		case LevelDirection.East:
			if (neighbour.HasLadderAnySize(LevelDirection.West))
			{
				return neighbour;
			}
			break;
		case LevelDirection.West:
			if (neighbour.HasLadderAnySize(LevelDirection.East))
			{
				return neighbour;
			}
			break;
		}
		return null;
	}

	public bool HasWallAnyType(LevelDirection edge)
	{
		if (edgeValues[(int)edge] == LevelNodeEdgeValue.Wall || edgeValues[(int)edge] == LevelNodeEdgeValue.AltWall || edgeValues[(int)edge] == LevelNodeEdgeValue.WallWithHidingSpot)
		{
			return true;
		}
		return false;
	}

	public bool HasWallAnyTypeAnyDirection()
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (HasWallAnyType(levelDirection))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasNeighbourWithWallAnyTypeAnyDirection(LevelNode[,] nodes)
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbour = GetNeighbour(nodes, levelDirection);
			if (neighbour != null && neighbour.HasWallAnyTypeAnyDirection())
			{
				return true;
			}
		}
		return false;
	}

	public bool HasDoorAnyType(LevelDirection edge)
	{
		if (Door.IsDoorEdgeValue(edgeValues[(int)edge]))
		{
			return true;
		}
		return false;
	}

	public bool HasKeyDoor(LevelDirection edge)
	{
		if (edgeValues[(int)edge] == LevelNodeEdgeValue.AltKeyDoor || edgeValues[(int)edge] == LevelNodeEdgeValue.KeyDoor)
		{
			return true;
		}
		return false;
	}

	public bool HasElementalDoor(LevelDirection edge)
	{
		if (edgeValues[(int)edge] == LevelNodeEdgeValue.ElementalDoor_earth || edgeValues[(int)edge] == LevelNodeEdgeValue.ElementalDoor_electricity || edgeValues[(int)edge] == LevelNodeEdgeValue.ElementalDoor_energy || edgeValues[(int)edge] == LevelNodeEdgeValue.ElementalDoor_fire || edgeValues[(int)edge] == LevelNodeEdgeValue.ElementalDoor_ice || edgeValues[(int)edge] == LevelNodeEdgeValue.ElementalDoor_water)
		{
			return true;
		}
		return false;
	}

	public bool HasTriggeredDoor(LevelDirection edge)
	{
		return edgeValues[(int)edge] == LevelNodeEdgeValue.TriggeredDoor;
	}

	public void SetChunkId(int chunkId)
	{
		this.chunkId = chunkId;
	}

	public void ResetChunkId()
	{
		chunkId = 0;
	}

	public void SetNodeEdgeValue(LevelDirection? direction, LevelNodeEdgeValue value)
	{
		if (direction.HasValue)
		{
			edgeValues[(int)direction.Value] = value;
		}
	}

	public LevelNodeEdgeValue GetNodeEdgeValue(LevelDirection? direction)
	{
		if (!direction.HasValue)
		{
			return LevelNodeEdgeValue.Empty;
		}
		return edgeValues[(int)direction.Value];
	}

	public void SetNodeEdgeObject(LevelDirection? direction, GameObject value)
	{
		if (direction.HasValue)
		{
			edgeObjects[(int)direction.Value] = value;
		}
	}

	public GameObject GetNodeEdgeObject(LevelDirection? direction)
	{
		if (!direction.HasValue)
		{
			return null;
		}
		return edgeObjects[(int)direction.Value];
	}

	public void SetTriangles(LevelNodeTriangleFace face, int[] triangles)
	{
		if (this.triangles.ContainsKey(face))
		{
			int[] array = this.triangles[face];
			this.triangles[face] = new int[array.Length + triangles.Length];
			array.CopyTo(this.triangles[face], 0);
			triangles.CopyTo(this.triangles[face], array.Length);
		}
		else
		{
			this.triangles.Add(face, triangles);
		}
	}

	public int[] GetTriangles(LevelNodeTriangleFace face)
	{
		if (!triangles.ContainsKey(face))
		{
			return new int[0];
		}
		return triangles[face];
	}

	public int GetNumFilledEdgeNodes()
	{
		int num = 0;
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (GetNodeEdgeValue(levelDirection) != LevelNodeEdgeValue.Empty)
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumEmptyEdgeNodes()
	{
		int num = 0;
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (GetNodeEdgeValue(levelDirection) == LevelNodeEdgeValue.Empty)
			{
				num++;
			}
		}
		return num;
	}

	public void AddObjectType(LevelNodeObjectType obj, LevelChunkDB chunkDB, string tag = "")
	{
		if (elevation == 0)
		{
			Debug.LogWarning("Adding object '" + obj.ToString() + "' to a LevelNode that is of elevation zero");
		}
		if (type == LevelNodeType.None)
		{
			Debug.LogWarning("Adding object '" + obj.ToString() + "' to a LevelNode that is of type 'None'!");
		}
		foreach (LevelNodeObjectTypeAndTag objectType in objectTypes)
		{
			if (objectType.objectType == obj)
			{
				Debug.LogError("Object '" + obj.ToString() + "' already in LevelNode objects. Can't add same object twice!");
				return;
			}
		}
		if (tag == string.Empty)
		{
			objectTypes.Add(new LevelNodeObjectTypeAndTag(obj));
		}
		else
		{
			objectTypes.Add(new LevelNodeObjectTypeAndTag(obj, tag));
		}
		if (chunkDB.objectTypeCounter.ContainsKey(obj))
		{
			chunkDB.objectTypeCounter[obj]++;
		}
		else
		{
			chunkDB.objectTypeCounter.Add(obj, 1);
		}
	}

	public void AddObject(GameObject go)
	{
		objects.Add(go);
	}

	public void RemoveObject(GameObject go)
	{
		objects.Remove(go);
	}

	public void UpdateObjectCharacterColliders()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].GetComponent<AIAgent>() == null)
			{
				objectCharacterColliders.AddRange(objects[i].GetComponentsInChildren<CharacterCollider>());
			}
		}
	}

	public void ClearObjectTypes(LevelChunkDB chunkDB)
	{
		foreach (LevelNodeObjectTypeAndTag objectType in objectTypes)
		{
			if (chunkDB.objectTypeCounter.ContainsKey(objectType.objectType))
			{
				chunkDB.objectTypeCounter[objectType.objectType]--;
			}
		}
		objectTypes.Clear();
	}

	public void ClearObjectTypesExcept(LevelChunkDB chunkDB, LevelNodeObjectType typeException = LevelNodeObjectType.None)
	{
		foreach (LevelNodeObjectTypeAndTag objectType in objectTypes)
		{
			if (objectType.objectType != typeException && chunkDB.objectTypeCounter.ContainsKey(objectType.objectType))
			{
				chunkDB.objectTypeCounter[objectType.objectType]--;
			}
		}
		objectTypes.RemoveAll((LevelNodeObjectTypeAndTag x) => x.objectType != typeException);
	}

	public bool ContainsObjectType(LevelNodeObjectType type)
	{
		foreach (LevelNodeObjectTypeAndTag objectType in objectTypes)
		{
			if (objectType.objectType == type)
			{
				return true;
			}
		}
		return false;
	}

	public LevelNode GetNeighbour(LevelNode[,] nodes, LevelDirection direction)
	{
		switch (direction)
		{
		case LevelDirection.North:
			if (row == nodes.GetLength(0) - 1)
			{
				return null;
			}
			return nodes[row + 1, col];
		case LevelDirection.South:
			if (row == 0)
			{
				return null;
			}
			return nodes[row - 1, col];
		case LevelDirection.East:
			if (col == nodes.GetLength(1) - 1)
			{
				return null;
			}
			return nodes[row, col + 1];
		case LevelDirection.West:
			if (col == 0)
			{
				return null;
			}
			return nodes[row, col - 1];
		default:
			return null;
		}
	}

	public void MakePathNode()
	{
		isPathNode = true;
	}

	public override string ToString()
	{
		return "[r: " + row + " c: " + col + " e: " + elevation + "]";
	}
}
