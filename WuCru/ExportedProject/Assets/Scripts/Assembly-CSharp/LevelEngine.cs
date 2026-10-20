using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelEngine
{
	private LevelNode[,] nodes;

	private int[,] elevationMap;

	private bool generateElevationMap = true;

	private bool fillHolesInMainPath = true;

	private bool fillHolesInSideQuests = true;

	private bool removeLonelyCells = true;

	private LevelPoint startPoint;

	private LevelPoint endPoint;

	private PathFinding pathFinding;

	private LevelChunkDB chunkDB;

	private static Color[] c = new Color[7]
	{
		Color.white,
		Color.green,
		Color.blue,
		Color.red,
		Color.yellow,
		Color.magenta,
		Color.cyan
	};

	public LevelChunkDB ChunkDB
	{
		get
		{
			return chunkDB;
		}
	}

	public LevelPoint StartPoint
	{
		get
		{
			return startPoint;
		}
	}

	public LevelPoint EndPoint
	{
		get
		{
			return endPoint;
		}
	}

	public PathFinding PathFinding
	{
		get
		{
			return pathFinding;
		}
	}

	public LevelNode[,] Nodes
	{
		get
		{
			return nodes;
		}
	}

	public Level GenerateLevel(LevelThemeCollection themeCollection, LevelTheme theme, LevelStyle style, int seed, ElementalPower primaryPower, ElementalPower secondaryPower)
	{
		chunkDB = new LevelChunkDB();
		bool flag = false;
		if (theme.index == themeCollection.themes.Count - 1)
		{
			flag = true;
		}
		string text = seed.ToString();
		text += theme.index;
		int seed2 = Convert.ToInt32(text);
		CreateEmptyNodes(theme.rows, theme.cols);
		pathFinding = new PathFinding(nodes);
		UnityEngine.Random.seed = seed2;
		int minElevation = style.minElevation;
		int num = style.maxElevation + theme.elevationImpact;
		if (num < minElevation)
		{
			num = minElevation;
		}
		elevationMap = GenerateElevationMap(minElevation, num, style.minElevationHills, style.maxElevationHills, style.minElevationPlateauRows, style.maxElevationPlateauRows, style.minElevationPlateuaCols, style.maxElevationPlateuaCols);
		startPoint = new LevelPoint(LevelHelper.RandomRangeToMax(theme.minStartPointRow, theme.maxStartPointRow), 0);
		if (!flag)
		{
			endPoint = new LevelPoint(LevelHelper.RandomRangeToMax(theme.minEndPointRow, theme.maxEndPointRow), theme.cols - 1);
		}
		else
		{
			endPoint = new LevelPoint(LevelHelper.RandomRangeToMax(theme.minEndPointRow, theme.maxEndPointRow), theme.cols - 6);
		}
		UnityEngine.Random.seed = seed2;
		GenerateMainPath(startPoint, endPoint, theme.mainPathMinWidth, theme.mainPathMaxWidth, UnityEngine.Random.Range(theme.mainPathMinSegmentLength, theme.mainPathMaxSegmentLength), UnityEngine.Random.Range(theme.mainPathMinVarianceFactor, theme.mainPathMaxVarianceFactor));
		int num2 = theme.maxRectangleRooms;
		if (num2 > theme.minRectangleRooms)
		{
			num2++;
		}
		int num3 = theme.maxEllipseRooms;
		if (num3 > theme.minEllipseRooms)
		{
			num3++;
		}
		int num4 = UnityEngine.Random.Range(theme.minRectangleRooms, num2);
		int num5 = UnityEngine.Random.Range(theme.minEllipseRooms, num3);
		int num6 = num4 + num5;
		int num7 = 0;
		int num8 = 0;
		bool flag2 = false;
		UnityEngine.Random.seed = seed2;
		for (int i = 0; i < num6; i++)
		{
			if (num7 < num4 && num8 < num5)
			{
				flag2 = !(UnityEngine.Random.value <= 0.5f);
			}
			else if (num7 < num4)
			{
				flag2 = true;
			}
			else
			{
				if (num8 >= num5)
				{
					Debug.LogError("Something is rotten in room creation!");
					break;
				}
				flag2 = false;
			}
			LevelPoint center = default(LevelPoint);
			LevelPoint extent = default(LevelPoint);
			if (flag2)
			{
				center.col = theme.cols / (num6 + 1) * (i + 1) + LevelHelper.RandomRangeToMax(theme.rectRoomMinColOffset, theme.rectRoomMaxColOffset);
				int firstFilledRowAtCol = LevelHelper.GetFirstFilledRowAtCol(nodes, center.col);
				int lastFilledRowAtCol = LevelHelper.GetLastFilledRowAtCol(nodes, center.col);
				extent.row = LevelHelper.RandomRangeToMax(theme.rectRoomMinRows, theme.rectRoomMaxRows);
				extent.col = LevelHelper.RandomRangeToMax(theme.rectRoomMinCols, theme.rectRoomMaxCols);
				switch (theme.rectRoomPlacement)
				{
				case LevelRoomPlacement.OnMainPath:
					center.row = (firstFilledRowAtCol + lastFilledRowAtCol) / 2;
					break;
				case LevelRoomPlacement.OffMainPath:
					if (theme.rows - lastFilledRowAtCol > firstFilledRowAtCol)
					{
						center.row = lastFilledRowAtCol + extent.row;
					}
					else
					{
						center.row = firstFilledRowAtCol - extent.row;
					}
					break;
				}
				GenerateRectangleRoom(center, extent, minElevation, num, theme.rectRoomElevationVariance, theme.rectRoomMinPadding, theme.rectRoomMaxPadding, theme.rectRoomPaddingProbability);
				num7++;
				continue;
			}
			center.col = theme.cols / (num6 + 1) * (i + 1) + LevelHelper.RandomRangeToMax(theme.ellipseRoomMinColOffset, theme.ellipseRoomMaxColOffset);
			int firstFilledRowAtCol2 = LevelHelper.GetFirstFilledRowAtCol(nodes, center.col);
			int lastFilledRowAtCol2 = LevelHelper.GetLastFilledRowAtCol(nodes, center.col);
			extent.row = LevelHelper.RandomRangeToMax(theme.ellipseRoomMinRows, theme.ellipseRoomMaxRows);
			extent.col = LevelHelper.RandomRangeToMax(theme.ellipseRoomMinCols, theme.ellipseRoomMaxCols);
			switch (theme.rectRoomPlacement)
			{
			case LevelRoomPlacement.OnMainPath:
				center.row = (firstFilledRowAtCol2 + lastFilledRowAtCol2) / 2;
				break;
			case LevelRoomPlacement.OffMainPath:
				if (theme.rows - lastFilledRowAtCol2 > firstFilledRowAtCol2)
				{
					center.row = lastFilledRowAtCol2 + extent.row;
				}
				else
				{
					center.row = firstFilledRowAtCol2 - extent.row;
				}
				break;
			}
			GenerateEllipseRoom(center, extent, minElevation, num, theme.ellipseRoomElevationVariance, theme.ellipseRoomPadding);
			num8++;
		}
		if (flag)
		{
			GenerateEndLevelArea(endPoint, new LevelPoint(10, 10), minElevation, num, 0, 2, theme.useSquareEndArea);
		}
		UnityEngine.Random.seed = seed2;
		int num9 = LevelHelper.RandomRangeToMax(theme.minSideQuests, theme.maxSideQuests);
		for (int j = 0; j < num9; j++)
		{
			int col = nodes.GetLength(1) / (num9 + 1) * (j + 1) + LevelHelper.RandomRangeToMax(theme.sideQuestMinColOffset, theme.sideQuestMaxColOffset);
			int firstFilledRowAtCol3 = LevelHelper.GetFirstFilledRowAtCol(nodes, col, LevelNodeType.SideQuest);
			int lastFilledRowAtCol3 = LevelHelper.GetLastFilledRowAtCol(nodes, col, LevelNodeType.SideQuest);
			LevelDirection levelDirection;
			int row;
			if (theme.rows - lastFilledRowAtCol3 < firstFilledRowAtCol3)
			{
				row = firstFilledRowAtCol3;
				levelDirection = LevelDirection.South;
			}
			else
			{
				row = lastFilledRowAtCol3;
				levelDirection = LevelDirection.North;
			}
			LevelPoint start = new LevelPoint(row, col);
			col = start.col + LevelHelper.RandomRangeToMax(theme.sideQuestMinEndPointColOffset, theme.sideQuestMaxEndPointColOffset);
			if (col < 0)
			{
				col = 0;
			}
			if (col >= theme.cols)
			{
				col = theme.cols - 1;
			}
			row = ((levelDirection == LevelDirection.North) ? (theme.rows - 1) : 0);
			GenerateSideQuest(end: new LevelPoint(row, col), start: start, minPathWidth: theme.sideQuestMinWidth, maxPathWidth: theme.sideQuestMaxWidth, segmentLength: UnityEngine.Random.Range(theme.sideQuestMinSegmentLength, theme.sideQuestMaxSegmentLength), varianceFactor: UnityEngine.Random.Range(theme.sideQuestMinVarianceFactor, theme.sideQuestMaxVarianceFactor));
		}
		if (fillHolesInMainPath)
		{
			FillHolesInPath(LevelNodeType.MainPath);
		}
		if (fillHolesInSideQuests)
		{
			FillHolesInPath(LevelNodeType.SideQuest);
		}
		if (removeLonelyCells)
		{
			LonelyCellRemoval();
		}
		LevelPathGenerator path = chunkDB.GetChunk<MainPathChunk>().Path;
		if (path != null)
		{
			HandlePathElevationChange(path);
		}
		List<SideQuestChunk> chunks = chunkDB.GetChunks<SideQuestChunk>();
		foreach (SideQuestChunk item in chunks)
		{
			LevelPathGenerator path2 = item.Path;
			if (path2 != null)
			{
				HandlePathElevationChange(path2);
			}
		}
		UnityEngine.Random.seed = seed2;
		MainPathChunk chunk = chunkDB.GetChunk<MainPathChunk>();
		HandleMainPathConnection(chunk);
		foreach (RectangleRoomChunk chunk2 in chunkDB.GetChunks<RectangleRoomChunk>())
		{
			HandleRoomConnections(chunk2);
		}
		foreach (EllipseRoomChunk chunk3 in chunkDB.GetChunks<EllipseRoomChunk>())
		{
			HandleRoomConnections(chunk3);
		}
		foreach (SideQuestChunk chunk4 in chunkDB.GetChunks<SideQuestChunk>())
		{
			HandleSideQuestConnections(chunk4);
		}
		HandleIslands();
		chunkDB.LevelPlateaus.AddRange(BuildLevelPlateaus());
		BuildPlateauSegments(chunkDB.LevelPlateaus);
		SanityCheckChunkDB();
		Level level = new Level(seed, nodes, chunkDB, primaryPower, secondaryPower);
		level.LevelThemeCollection = themeCollection;
		level.LevelTheme = theme;
		level.LevelStyle = style;
		level.StartPoint = startPoint;
		level.EndPoint = endPoint;
		GameplayEngine gameplayEngine = new GameplayEngine(ref level);
		gameplayEngine.BuildHouses(this, level);
		chunkDB.SetupGameplay(this);
		gameplayEngine.SetupGameplay((level.LevelTheme.index != level.LevelThemeCollection.themes.Count - 1) ? (level.NumFilledTiles / 5) : (level.NumFilledTiles / 7));
		UnityEngine.Random.seed = seed2;
		gameplayEngine.PlaceLoot(this, level, 3, 3);
		gameplayEngine.PlaceStuds(this, 30);
		gameplayEngine.PlaceElementalDoors(this, level, 0.3f);
		gameplayEngine.PlaceLamps(this);
		SetupTrees(style, theme, gameplayEngine);
		chunkDB.SetupAI(this);
		UnityEngine.Random.seed = seed2;
		LevelChallengeSelector levelChallengeSelector = new LevelChallengeSelector();
		levelChallengeSelector.SetupChallenge(level, gameplayEngine);
		if (EditorStateMachine.Instance != null)
		{
			EditorStateMachine.Instance.GameplayEngine = gameplayEngine;
		}
		return level;
	}

	private void CreateEmptyNodes(int rows, int cols)
	{
		nodes = new LevelNode[rows, cols];
		for (int i = 0; i < cols; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				nodes[j, i] = new LevelNode(0, j, i);
				nodes[j, i].Type = LevelNodeType.None;
			}
		}
	}

	private void SanityCheckChunkDB()
	{
		int num = 0;
		foreach (LevelChunk chunk in chunkDB.Chunks)
		{
			num += chunk.NodeList.Count;
			foreach (LevelNode node in chunk.NodeList)
			{
				if (chunk.NodeType != node.Type)
				{
					Debug.LogError(string.Concat("Wrong node-type in chunk-id: ", chunk.Id, ", type: ", chunk.GetType().ToString(), ", expected nodetype: ", chunk.NodeType, ", got node-type: ", node.Type));
				}
			}
		}
		int num2 = 0;
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].Type != LevelNodeType.None && nodes[i, j].ChunkId <= 0)
				{
					num2++;
				}
			}
		}
		int num3 = 0;
		for (int k = 0; k < nodes.GetLength(0); k++)
		{
			for (int l = 0; l < nodes.GetLength(1); l++)
			{
				if (nodes[k, l].Type != LevelNodeType.None)
				{
					num3++;
				}
			}
		}
		if (num2 > 0)
		{
			Debug.LogWarning("Orphans: " + num2);
		}
		if (num - num3 > 0)
		{
			Debug.LogWarning("Un-accounted nodes: " + (num - num3));
		}
	}

	public void Clear()
	{
		chunkDB.Clear();
		nodes = null;
	}

	public void SetFromDeserializer(ref LevelNode[,] nodes)
	{
		this.nodes = nodes;
	}

	private void HandleIslands()
	{
		chunkDB.LevelIslands = IslandDetection();
	}

	private List<LevelIsland> IslandDetection()
	{
		List<LevelIsland> list = new List<LevelIsland>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].Elevation > 0 && !nodes[i, j].isCheckedForIsland && !DoesPathExist(startPoint, nodes[i, j].Location) && (startPoint.row != i || startPoint.col != j) && !nodes[i, j].ContainsObjectType(LevelNodeObjectType.LampPost) && !nodes[i, j].ContainsObjectType(LevelNodeObjectType.Tree))
				{
					List<LevelNode> list2 = new List<LevelNode>();
					LevelIsland levelIsland = new LevelIsland();
					DetectIsland(new LevelPoint(i, j), list2);
					levelIsland.islandNodes = list2.ToArray();
					list.Add(levelIsland);
				}
			}
		}
		return list;
	}

	private void DetectIsland(LevelPoint islandPoint, List<LevelNode> islandNodes)
	{
		LevelNode levelNode = nodes[islandPoint.row, islandPoint.col];
		islandNodes.Add(levelNode);
		levelNode.isCheckedForIsland = true;
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbourAt = LevelHelper.GetNeighbourAt(islandPoint, levelDirection, nodes);
			if (neighbourAt != null && neighbourAt.Elevation == levelNode.Elevation && !neighbourAt.isCheckedForIsland && !neighbourAt.ContainsObjectType(LevelNodeObjectType.LampPost) && !neighbourAt.ContainsObjectType(LevelNodeObjectType.Tree) && PathFinding.DoesNeighbourHaveAccess(levelNode, neighbourAt))
			{
				DetectIsland(neighbourAt.Location, islandNodes);
			}
		}
	}

	public List<LevelPlateau> BuildLevelPlateaus()
	{
		bool[,] array = new bool[nodes.GetLength(0), nodes.GetLength(1)];
		List<LevelPlateau> list = new List<LevelPlateau>();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].Elevation > 0 && !array[i, j])
				{
					LevelPlateau levelPlateau = new LevelPlateau();
					levelPlateau.min = new LevelPoint(i, j);
					levelPlateau.max = new LevelPoint(i, j);
					list.Add(levelPlateau);
					FillPlateau(nodes[i, j], levelPlateau, array);
				}
			}
		}
		foreach (LevelPlateau item in list)
		{
			LevelNode[,] array2 = new LevelNode[nodes.GetLength(0), nodes.GetLength(1)];
			for (int k = 0; k < nodes.GetLength(0); k++)
			{
				for (int l = 0; l < nodes.GetLength(1); l++)
				{
					array2[k, l] = new LevelNode(0, k, l);
				}
			}
			foreach (LevelNode node in item.nodes)
			{
				array2[node.Row, node.Col].Type = node.Type;
				array2[node.Row, node.Col].Elevation = node.Elevation;
			}
			DetectRectangles(item, nodes.GetLength(0), nodes.GetLength(1), array2);
			item.rects.OrderByDescending((LevelPlateauRect r) => r.width * r.height);
		}
		return list;
	}

	private void FillPlateau(LevelNode currentNode, LevelPlateau currentPlateau, bool[,] visited)
	{
		currentNode.levelPlateau = currentPlateau;
		currentPlateau.nodes.Add(currentNode);
		visited[currentNode.Row, currentNode.Col] = true;
		if (currentPlateau.min.row > currentNode.Row)
		{
			currentPlateau.min.row = currentNode.Row;
		}
		if (currentPlateau.min.col > currentNode.Col)
		{
			currentPlateau.min.col = currentNode.Col;
		}
		if (currentPlateau.max.row < currentNode.Row)
		{
			currentPlateau.max.row = currentNode.Row;
		}
		if (currentPlateau.max.col < currentNode.Col)
		{
			currentPlateau.max.col = currentNode.Col;
		}
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			if (currentNode.GetNodeEdgeValue(levelDirection) != LevelNodeEdgeValue.Wall)
			{
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(currentNode.Location, levelDirection, nodes);
				if (neighbourAt != null && neighbourAt.Elevation == currentNode.Elevation && !visited[neighbourAt.Row, neighbourAt.Col])
				{
					FillPlateau(neighbourAt, currentPlateau, visited);
				}
			}
		}
	}

	public void BuildPlateauSegments(List<LevelPlateau> plateaus)
	{
		for (int i = 0; i < plateaus.Count; i++)
		{
			LevelPlateau levelPlateau = plateaus[i];
			int[,] array = new int[nodes.GetLength(0), nodes.GetLength(1)];
			for (int j = 0; j < levelPlateau.nodes.Count; j++)
			{
				LevelNode levelNode = levelPlateau.nodes[j];
				for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
				{
					if ((array[levelNode.Row, levelNode.Col] & (1 << (int)levelDirection)) == 1 << (int)levelDirection)
					{
						continue;
					}
					if (LevelHelper.NodeHasBoundary(levelNode, nodes, levelDirection))
					{
						Segment segment = new Segment();
						levelPlateau.segments.Add(segment);
						levelPlateau.endPoints.Add(segment.left);
						levelPlateau.endPoints.Add(segment.right);
						segment.left.position = GetLeftCorner(levelNode, levelDirection);
						segment.right.position = GetRightCorner(levelNode, levelDirection);
						array[levelNode.Row, levelNode.Col] |= 1 << (int)levelDirection;
						LevelNode neighbourAt = LevelHelper.GetNeighbourAt(levelNode.Location, LevelHelper.GetLeftDirection(levelDirection), nodes);
						while (neighbourAt != null && neighbourAt.Elevation == levelNode.Elevation && LevelHelper.NodeHasBoundary(neighbourAt, nodes, levelDirection))
						{
							segment.left.position = GetLeftCorner(neighbourAt, levelDirection);
							array[neighbourAt.Row, neighbourAt.Col] |= 1 << (int)levelDirection;
							neighbourAt = LevelHelper.GetNeighbourAt(neighbourAt.Location, LevelHelper.GetLeftDirection(levelDirection), nodes);
						}
						LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(levelNode.Location, LevelHelper.GetRightDirection(levelDirection), nodes);
						while (neighbourAt2 != null && neighbourAt2.Elevation == levelNode.Elevation && LevelHelper.NodeHasBoundary(neighbourAt2, nodes, levelDirection))
						{
							segment.right.position = GetRightCorner(neighbourAt2, levelDirection);
							array[neighbourAt2.Row, neighbourAt2.Col] |= 1 << (int)levelDirection;
							neighbourAt2 = LevelHelper.GetNeighbourAt(neighbourAt2.Location, LevelHelper.GetRightDirection(levelDirection), nodes);
						}
					}
					else
					{
						array[levelNode.Row, levelNode.Col] |= 1 << (int)levelDirection;
					}
				}
			}
		}
	}

	private void DetectRectangles(LevelPlateau plateau, int rows, int cols, LevelNode[,] nodes)
	{
		int[,] array = new int[rows, cols];
		for (int i = 0; i < cols; i++)
		{
			array[0, i] = ((nodes[0, i].Type == LevelNodeType.None) ? (-1) : 0);
		}
		for (int j = 1; j < rows; j++)
		{
			for (int k = 0; k < cols; k++)
			{
				if (nodes[j, k].Type == LevelNodeType.None)
				{
					array[j, k] = -1;
				}
				else
				{
					array[j, k] = array[j - 1, k] + 1;
				}
			}
		}
		int[,] array2 = new int[rows, cols];
		for (int l = 0; l < cols; l++)
		{
			array2[rows - 1, l] = ((nodes[rows - 1, l].Type == LevelNodeType.None) ? (-1) : 0);
		}
		for (int num = rows - 2; num >= 0; num--)
		{
			for (int m = 0; m < cols; m++)
			{
				if (nodes[num, m].Type == LevelNodeType.None)
				{
					array2[num, m] = -1;
				}
				else
				{
					array2[num, m] = array2[num + 1, m] + 1;
				}
			}
		}
		for (int num2 = cols - 1; num2 >= 0; num2--)
		{
			int num3 = rows;
			for (int num4 = rows - 1; num4 >= 0; num4--)
			{
				num3++;
				if (nodes[num4, num2].Type != LevelNodeType.None && (num2 == 0 || nodes[num4, num2 - 1].Type == LevelNodeType.None))
				{
					int num5 = array[num4, num2];
					int num6 = array2[num4, num2];
					int n;
					for (n = 1; num2 + n < cols && nodes[num4, num2 + n].Type != LevelNodeType.None; n++)
					{
						int num7 = array[num4, num2 + n];
						int num8 = array2[num4, num2 + n];
						if ((num7 < num5) | (num8 < num6))
						{
							if (num6 < num3)
							{
								AddRectangleToPlateau(plateau, num2, num4 - num5, n, num5 + num6 + 1);
							}
							if (num7 < num5)
							{
								num5 = num7;
							}
							if (num8 < num6)
							{
								num6 = num8;
							}
						}
					}
					if (num6 < num3)
					{
						AddRectangleToPlateau(plateau, num2, num4 - num5, n, num5 + num6 + 1);
					}
					num3 = 0;
				}
			}
		}
	}

	private void AddRectangleToPlateau(LevelPlateau plateau, int col, int row, int width, int height)
	{
		LevelPlateauRect item = new LevelPlateauRect(row, col, width, height, nodes, plateau);
		plateau.rects.Add(item);
	}

	private Vector3 GetLeftCorner(LevelNode node, LevelDirection direction)
	{
		Vector3 vector = LevelHelper.NodeToVector3(node);
		switch (direction)
		{
		default:
			return vector + new Vector3(-0.5f, 0f, 0.5f);
		case LevelDirection.East:
			return vector + new Vector3(0.5f, 0f, 0.5f);
		case LevelDirection.South:
			return vector + new Vector3(0.5f, 0f, -0.5f);
		case LevelDirection.West:
			return vector + new Vector3(-0.5f, 0f, -0.5f);
		}
	}

	private Vector3 GetRightCorner(LevelNode node, LevelDirection direction)
	{
		Vector3 vector = LevelHelper.NodeToVector3(node);
		switch (direction)
		{
		default:
			return vector + new Vector3(0.5f, 0f, 0.5f);
		case LevelDirection.East:
			return vector + new Vector3(0.5f, 0f, -0.5f);
		case LevelDirection.South:
			return vector + new Vector3(-0.5f, 0f, -0.5f);
		case LevelDirection.West:
			return vector + new Vector3(-0.5f, 0f, 0.5f);
		}
	}

	public float[,] GenerateHeightMap()
	{
		float[,] array = new float[nodes.GetLength(0), nodes.GetLength(1)];
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				float num = Mathf.PerlinNoise((float)i / (float)(nodes.GetLength(0) - 1), (float)j / (float)(nodes.GetLength(1) - 1));
				array[i, j] = num;
			}
		}
		return array;
	}

	public int[,] GenerateElevationMap(int minHeight, int maxHeight, int minHills, int maxHills, int minPlateauRows, int maxPlateauRows, int minPlateauCols, int maxPlateauCols)
	{
		if (minHeight < 1)
		{
			throw new Exception("ElevationMap cannot have minHeight lower than 1");
		}
		int[,] array = new int[nodes.GetLength(0), nodes.GetLength(1)];
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				array[i, j] = minHeight;
			}
		}
		if (generateElevationMap && maxHeight - minHeight > 0)
		{
			int num = LevelHelper.RandomRangeToMax(minHills, maxHills);
			for (int k = 0; k < num; k++)
			{
				int num2 = maxHeight - minHeight;
				int col = (nodes.GetLength(1) - 1) / (num + 1) * (k + 1) + UnityEngine.Random.Range(-4, 4);
				LevelPoint center = new LevelPoint(UnityEngine.Random.Range(nodes.GetLength(0) - minPlateauRows + ((minPlateauRows % 2 != 1) ? 1 : 2), nodes.GetLength(0) + 2), col);
				int num3 = (nodes.GetLength(0) - (nodes.GetLength(0) - center.row)) / num2;
				int num4 = (nodes.GetLength(1) - center.col) / num2;
				LevelPoint levelPoint = new LevelPoint(UnityEngine.Random.Range(minPlateauRows, maxPlateauRows), UnityEngine.Random.Range(minPlateauCols, maxPlateauCols));
				int num5 = UnityEngine.Random.Range(num3 + 1, num3 * 2);
				int num6 = UnityEngine.Random.Range(num4, num4 * 2);
				for (int num7 = num2 - 1; num7 >= 0; num7--)
				{
					LevelPoint levelPoint2 = new LevelPoint(levelPoint.row + num5 * num7, levelPoint.col + num6 * num7);
					for (int l = center.row - levelPoint2.row / 2; l <= center.row + levelPoint2.row / 2; l++)
					{
						for (int m = center.col - levelPoint2.col / 2; m <= center.col + levelPoint2.col / 2; m++)
						{
							if (LevelHelper.PointInsideBounds(l, m, nodes) && LevelHelper.PointInsideEllipse(center, levelPoint2.row, levelPoint2.col, new LevelPoint(l, m)) && array[l, m] < maxHeight - num7)
							{
								array[l, m] = maxHeight - num7;
							}
						}
					}
				}
			}
		}
		return array;
	}

	public void GenerateMainPath(LevelPoint start, LevelPoint end, int minPathWidth, int maxPathWidth, float segmentLength, float varianceFactor)
	{
		MainPathChunk mainPathChunk = (MainPathChunk)chunkDB.RegisterNewChunk(nodes.GetLength(0), nodes.GetLength(1), LevelChunkType.MainPath);
		LevelPathGenerator path = GenerateOrganicPath(start, end, minPathWidth, maxPathWidth, segmentLength, varianceFactor, LevelNodeType.MainPath, true, mainPathChunk);
		mainPathChunk.Path = path;
		nodes[start.row, start.col].AddObjectType(LevelNodeObjectType.StartFlag, chunkDB, string.Empty);
		nodes[end.row, end.col].AddObjectType(LevelNodeObjectType.EndFlag, chunkDB, string.Empty);
	}

	public void GenerateSideQuests(int numSideQuests, int minPathWidth, int maxPathWidth)
	{
		for (int i = 0; i < numSideQuests; i++)
		{
			int col = nodes.GetLength(1) / (numSideQuests + 1) * (i + 1) + UnityEngine.Random.Range(-4, 4);
			int firstFilledRowAtCol = LevelHelper.GetFirstFilledRowAtCol(nodes, col, LevelNodeType.SideQuest);
			int lastFilledRowAtCol = LevelHelper.GetLastFilledRowAtCol(nodes, col, LevelNodeType.SideQuest);
			LevelDirection levelDirection;
			int row;
			if (nodes.GetLength(0) - lastFilledRowAtCol < firstFilledRowAtCol)
			{
				row = firstFilledRowAtCol;
				levelDirection = LevelDirection.South;
			}
			else
			{
				row = lastFilledRowAtCol;
				levelDirection = LevelDirection.North;
			}
			LevelPoint start = new LevelPoint(row, col);
			col = UnityEngine.Random.Range(start.col - 10, start.col + 10);
			if (col < 0)
			{
				col = 0;
			}
			if (col >= nodes.GetLength(1))
			{
				col = nodes.GetLength(1) - 1;
			}
			row = ((levelDirection == LevelDirection.North) ? (nodes.GetLength(0) - 1) : 0);
			GenerateSideQuest(end: new LevelPoint(row, col), start: start, minPathWidth: minPathWidth, maxPathWidth: maxPathWidth, segmentLength: 2f, varianceFactor: 0.6f);
		}
	}

	public void GenerateSideQuest(LevelPoint start, LevelPoint end, int minPathWidth, int maxPathWidth, float segmentLength, float varianceFactor)
	{
		SideQuestChunk sideQuestChunk = (SideQuestChunk)chunkDB.RegisterNewChunk(nodes.GetLength(0), nodes.GetLength(1), LevelChunkType.SideQuest);
		LevelPathGenerator path = GenerateOrganicPath(start, end, minPathWidth, maxPathWidth, segmentLength, varianceFactor, LevelNodeType.SideQuest, false, sideQuestChunk);
		sideQuestChunk.Path = path;
	}

	public RectangleRoomChunk GenerateRectangleRoom(LevelPoint center, LevelPoint extent, int minElevation, int maxElevation, int elevationVariance = 0, int minPadding = 0, int maxPadding = 0, float paddingProbability = 0.7f)
	{
		List<LevelNode> list = new List<LevelNode>();
		int num = (int)Mathf.Ceil((float)extent.row / 2f);
		int num2 = (int)Mathf.Ceil((float)extent.col / 2f);
		if (center.row - num < 0)
		{
			center.row = num - ((extent.row % 2 != 1) ? 1 : 0);
		}
		if (center.col - num2 < 0)
		{
			center.col = num2 - ((extent.col % 2 != 1) ? 1 : 0);
		}
		if (center.row + num >= nodes.GetLength(0))
		{
			center.row = nodes.GetLength(0) - num - ((extent.row % 2 != 1) ? 1 : 0);
		}
		if (center.col + num2 >= nodes.GetLength(1))
		{
			center.col = nodes.GetLength(1) - num2 - ((extent.col % 2 != 1) ? 1 : 0);
		}
		int num3 = GetAvgElevationNorthOrSouth(center, extent.row, extent.col, LevelDirection.North);
		if (elevationVariance > 0)
		{
			num3 += UnityEngine.Random.Range(-elevationVariance, elevationVariance + 1);
			num3 = Mathf.Clamp(num3, minElevation, maxElevation);
		}
		for (int i = center.row - num; i <= center.row + num; i++)
		{
			for (int j = center.col - num2; j <= center.col + num2; j++)
			{
				if (LevelHelper.PointInsideBounds(i, j, nodes))
				{
					chunkDB.ResetNode(nodes[i, j]);
					nodes[i, j].Elevation = num3;
					nodes[i, j].Type = LevelNodeType.Room;
					list.Add(nodes[i, j]);
				}
			}
		}
		RectangleRoomChunk rectangleRoomChunk = (RectangleRoomChunk)chunkDB.RegisterNewChunk(nodes.GetLength(0), nodes.GetLength(1), LevelChunkType.RectangleRoom);
		rectangleRoomChunk.AddNodes(chunkDB, list);
		rectangleRoomChunk.Center = center;
		rectangleRoomChunk.Extent = extent;
		MainPathChunk chunk = chunkDB.GetChunk<MainPathChunk>();
		for (int k = 1; k <= maxPadding; k++)
		{
			int num4 = center.row + num + k;
			for (int l = center.col - num2 - 1; l <= center.col + num2 + 1; l++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num4, l), nodes) && nodes[num4, l].Type == LevelNodeType.None && LevelHelper.GetNeighbourAt(new LevelPoint(num4, l), LevelDirection.South, nodes, true) != null && (k <= minPadding || UnityEngine.Random.value < paddingProbability))
				{
					chunk.AddNode(chunkDB, nodes[num4, l]);
					nodes[num4, l].Type = LevelNodeType.MainPath;
					nodes[num4, l].Elevation = num3;
				}
			}
		}
		for (int m = 1; m <= maxPadding; m++)
		{
			int num5 = center.row - num - m;
			for (int n = center.col - num2 - 1; n <= center.col + num2 + 1; n++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num5, n), nodes) && nodes[num5, n].Type == LevelNodeType.None && LevelHelper.GetNeighbourAt(new LevelPoint(num5, n), LevelDirection.North, nodes, true) != null && (m <= minPadding || UnityEngine.Random.value < paddingProbability))
				{
					chunk.AddNode(chunkDB, nodes[num5, n]);
					nodes[num5, n].Type = LevelNodeType.MainPath;
					nodes[num5, n].Elevation = num3;
				}
			}
		}
		for (int num6 = 1; num6 <= maxPadding; num6++)
		{
			int num7 = center.col + num2 + num6;
			for (int num8 = center.row - num - 1; num8 <= center.row + num + 1; num8++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num8, num7), nodes) && nodes[num8, num7].Type == LevelNodeType.None && LevelHelper.GetNeighbourAt(new LevelPoint(num8, num7), LevelDirection.West, nodes, true) != null && (num6 <= minPadding || UnityEngine.Random.value < paddingProbability))
				{
					chunk.AddNode(chunkDB, nodes[num8, num7]);
					nodes[num8, num7].Type = LevelNodeType.MainPath;
					nodes[num8, num7].Elevation = num3;
				}
			}
		}
		for (int num9 = 1; num9 <= maxPadding; num9++)
		{
			int num10 = center.col - num2 - num9;
			for (int num11 = center.row - num - 1; num11 <= center.row + num + 1; num11++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num11, num10), nodes) && nodes[num11, num10].Type == LevelNodeType.None && LevelHelper.GetNeighbourAt(new LevelPoint(num11, num10), LevelDirection.East, nodes, true) != null && (num9 <= minPadding || UnityEngine.Random.value < paddingProbability))
				{
					chunk.AddNode(chunkDB, nodes[num11, num10]);
					nodes[num11, num10].Type = LevelNodeType.MainPath;
					nodes[num11, num10].Elevation = num3;
				}
			}
		}
		int num12 = 0;
		int num13 = 0;
		for (int num14 = center.row + num + 1; num14 <= center.row + num + 5; num14++)
		{
			for (int num15 = center.col - num2 - 1; num15 <= center.col + num2 + 1; num15++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num14, num15), nodes) && nodes[num14, num15].Elevation > 0)
				{
					int num16 = num3 + num12 - nodes[num14, num15].Elevation;
					if (num16 > 0)
					{
						nodes[num14, num15].Elevation = num3 + num12;
					}
				}
			}
			if (num13 == 0 || num13 % 2 == 0)
			{
				num12--;
			}
			num13++;
		}
		for (int num17 = center.row - (num + 1); num17 >= center.row - (num + 2); num17--)
		{
			for (int num18 = center.col - num2 - 1; num18 <= center.col + num2 + 1; num18++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(num17, num18), nodes) && nodes[num17, num18].Elevation > 0)
				{
					int num19 = num3 - nodes[num17, num18].Elevation;
					if (num19 < 0)
					{
						nodes[num17, num18].Elevation = num3;
					}
				}
			}
		}
		return rectangleRoomChunk;
	}

	public EllipseRoomChunk GenerateEllipseRoom(LevelPoint center, LevelPoint extent, int minElevation, int maxElevation, int elevationVariance = 0, int padding = 0)
	{
		List<LevelNode> list = new List<LevelNode>();
		int row = extent.row;
		int col = extent.col;
		if (center.row - row < 0)
		{
			center.row -= center.row - row;
		}
		if (center.col - col < 0)
		{
			center.col -= center.col - col;
		}
		if (center.row + row >= nodes.GetLength(0))
		{
			center.row = nodes.GetLength(0) - row;
		}
		if (center.col + col >= nodes.GetLength(1))
		{
			center.col = nodes.GetLength(1) - col;
		}
		int num = GetAvgElevation(center, extent.row, extent.col);
		if (elevationVariance > 0)
		{
			num += UnityEngine.Random.Range(-elevationVariance, elevationVariance + 1);
			num = Mathf.Clamp(num, minElevation, maxElevation);
		}
		for (int i = center.row - row; i <= center.row + row; i++)
		{
			for (int j = center.col - col; j <= center.col + col; j++)
			{
				if (LevelHelper.PointInsideEllipse(center, extent.row * 2, extent.col * 2, new LevelPoint(i, j)) && LevelHelper.PointInsideBounds(i, j, nodes))
				{
					chunkDB.ResetNode(nodes[i, j]);
					nodes[i, j].Elevation = num;
					nodes[i, j].Type = LevelNodeType.Room;
					list.Add(nodes[i, j]);
				}
			}
		}
		EllipseRoomChunk ellipseRoomChunk = (EllipseRoomChunk)chunkDB.RegisterNewChunk(nodes.GetLength(0), nodes.GetLength(1), LevelChunkType.EllipseRoom);
		ellipseRoomChunk.AddNodes(chunkDB, list);
		ellipseRoomChunk.Center = center;
		ellipseRoomChunk.Extent = extent;
		MainPathChunk chunk = chunkDB.GetChunk<MainPathChunk>();
		LevelPoint levelPoint = new LevelPoint(extent.row + padding * 2, extent.col + padding * 2);
		for (int k = center.row - row - padding; k <= center.row + row + padding; k++)
		{
			for (int l = center.col - col - padding; l <= center.col + col + padding; l++)
			{
				if (LevelHelper.PointInsideEllipse(center, levelPoint.row, levelPoint.col, new LevelPoint(k, l)) && LevelHelper.PointInsideBounds(k, l, nodes) && nodes[k, l].Type == LevelNodeType.None && nodes[k, l].ChunkId == 0)
				{
					nodes[k, l].Elevation = num;
					nodes[k, l].Type = LevelNodeType.MainPath;
					chunk.AddNode(chunkDB, nodes[k, l]);
				}
			}
		}
		return ellipseRoomChunk;
	}

	public EndLevelAreaChunk GenerateEndLevelArea(LevelPoint center, LevelPoint extent, int minElevation, int maxElevation, int elevationVariance = 0, int padding = 0, bool wantSquareEnd = false)
	{
		List<LevelNode> list = new List<LevelNode>();
		int num = (int)Mathf.Ceil((float)extent.row / 2f);
		int num2 = (int)Mathf.Ceil((float)extent.col / 2f);
		if (center.row - num < 0)
		{
			center.row = num - ((extent.row % 2 != 1) ? 1 : 0);
		}
		if (center.col - num2 < 0)
		{
			center.col = num2 - ((extent.col % 2 != 1) ? 1 : 0);
		}
		if (center.row + num >= nodes.GetLength(0))
		{
			center.row = nodes.GetLength(0) - num - ((extent.row % 2 != 1) ? 1 : 0);
		}
		if (center.col + num2 >= nodes.GetLength(1))
		{
			center.col = nodes.GetLength(1) - num2 - ((extent.col % 2 != 1) ? 1 : 0);
		}
		int num3 = GetAvgElevation(center, extent.row, extent.col);
		if (elevationVariance > 0)
		{
			num3 += UnityEngine.Random.Range(-elevationVariance, elevationVariance + 1);
			num3 = Mathf.Clamp(num3, minElevation, maxElevation);
		}
		for (int i = center.row - num; i <= center.row + num; i++)
		{
			for (int j = center.col - num2; j <= center.col + num2; j++)
			{
				if (wantSquareEnd)
				{
					if (LevelHelper.PointInsideBounds(i, j, nodes))
					{
						chunkDB.ResetNode(nodes[i, j]);
						nodes[i, j].Elevation = num3;
						nodes[i, j].Type = LevelNodeType.EndBossArea;
						list.Add(nodes[i, j]);
					}
				}
				else if (LevelHelper.PointInsideEllipse(center, extent.row, extent.col, new LevelPoint(i, j)) && LevelHelper.PointInsideBounds(i, j, nodes))
				{
					chunkDB.ResetNode(nodes[i, j]);
					nodes[i, j].Elevation = num3;
					nodes[i, j].Type = LevelNodeType.EndBossArea;
					list.Add(nodes[i, j]);
				}
			}
		}
		EndLevelAreaChunk endLevelAreaChunk = (EndLevelAreaChunk)chunkDB.RegisterNewChunk(nodes.GetLength(0), nodes.GetLength(1), LevelChunkType.EndLevelArea);
		endLevelAreaChunk.AddNodes(chunkDB, list);
		endLevelAreaChunk.Center = center;
		endLevelAreaChunk.Extent = extent;
		MainPathChunk chunk = chunkDB.GetChunk<MainPathChunk>();
		LevelPoint levelPoint = new LevelPoint(extent.row + padding * 2, extent.col + padding * 2);
		for (int k = center.row - num - padding; k <= center.row + num + padding; k++)
		{
			for (int l = center.col - num2 - padding; l <= center.col + num2 + padding; l++)
			{
				if (!wantSquareEnd && LevelHelper.PointInsideEllipse(center, levelPoint.row, levelPoint.col, new LevelPoint(k, l)) && LevelHelper.PointInsideBounds(k, l, nodes) && nodes[k, l].Type == LevelNodeType.None && nodes[k, l].ChunkId == 0)
				{
					nodes[k, l].Elevation = num3;
					nodes[k, l].Type = LevelNodeType.MainPath;
					chunk.AddNode(chunkDB, nodes[k, l]);
				}
			}
		}
		return endLevelAreaChunk;
	}

	public LevelPathGenerator GenerateOrganicPath(LevelPoint startPoint, LevelPoint endPoint, int minPathWidth, int maxPathWidth, float segmentLength, float varianceFactor, LevelNodeType type, bool allowOverwrite, LevelChunk chunk)
	{
		LevelPathGenerator levelPathGenerator = new LevelPathGenerator();
		List<LevelNode> list = levelPathGenerator.Generate(nodes, new Vector2(startPoint.col, startPoint.row), new Vector2(endPoint.col, endPoint.row), segmentLength, varianceFactor, minPathWidth, maxPathWidth);
		foreach (LevelNode item in list)
		{
			if (allowOverwrite || item.Type == LevelNodeType.None)
			{
				ChunkDB.ResetNode(item);
				item.Elevation = elevationMap[item.Row, item.Col];
				item.Type = type;
				chunk.AddNode(chunkDB, item);
			}
		}
		return levelPathGenerator;
	}

	public void HandleRoomConnections(LevelChunk chunk)
	{
		LevelPoint center;
		if (chunk is RectangleRoomChunk)
		{
			center = (chunk as RectangleRoomChunk).Center;
		}
		else
		{
			if (!(chunk is EllipseRoomChunk))
			{
				throw new Exception("Type of room incorrect!");
			}
			center = (chunk as EllipseRoomChunk).Center;
		}
		if (DoesPathExist(startPoint, center))
		{
			return;
		}
		List<LevelNode> pathLevelNodes = pathFinding.GetPathLevelNodes(nodes[startPoint.row, startPoint.col], nodes[center.row, center.col], (PathFinding.Node n1, PathFinding.Node n2) =>
		{
			if (Mathf.Abs(n1.levelNode.Elevation - n2.levelNode.Elevation) > 4)
			{
				return false;
			}
			return (n1.levelNode.Type != LevelNodeType.None && n2.levelNode.Type != LevelNodeType.None) ? true : false;
		});
		if (pathLevelNodes != null)
		{
			HandlePathElevationChange(pathLevelNodes);
		}
	}

	public void HandleSideQuestConnections(SideQuestChunk chunk)
	{
		LevelPoint to = new LevelPoint((int)chunk.Path.to.y, (int)chunk.Path.to.x);
		if (DoesPathExist(startPoint, to))
		{
			return;
		}
		List<LevelNode> pathLevelNodes = pathFinding.GetPathLevelNodes(nodes[startPoint.row, startPoint.col], nodes[to.row, to.col], (PathFinding.Node n1, PathFinding.Node n2) =>
		{
			if (Mathf.Abs(n1.levelNode.Elevation - n2.levelNode.Elevation) > 4)
			{
				return false;
			}
			return (n1.levelNode.Type != LevelNodeType.None && n2.levelNode.Type != LevelNodeType.None) ? true : false;
		});
		if (pathLevelNodes != null)
		{
			HandlePathElevationChange(pathLevelNodes);
		}
	}

	public void HandleMainPathConnection(MainPathChunk chunk)
	{
		if (DoesPathExist(startPoint, endPoint))
		{
			return;
		}
		List<LevelNode> pathLevelNodes = pathFinding.GetPathLevelNodes(nodes[startPoint.row, startPoint.col], nodes[endPoint.row, endPoint.col], (PathFinding.Node n1, PathFinding.Node n2) =>
		{
			if (Mathf.Abs(n1.levelNode.Elevation - n2.levelNode.Elevation) > 4)
			{
				return false;
			}
			return (n1.levelNode.Type != LevelNodeType.None && n2.levelNode.Type != LevelNodeType.None) ? true : false;
		});
		if (pathLevelNodes != null)
		{
			HandlePathElevationChange(pathLevelNodes);
		}
	}

	public void HandlePathElevationChange(LevelPathGenerator path)
	{
		List<LevelNode> pathNodes = path.GetPathNodes(nodes, pathFinding);
		HandlePathElevationChange(pathNodes);
	}

	public void HandlePathElevationChange(List<LevelNode> pathNodes)
	{
		for (int i = 0; i < pathNodes.Count - 1; i++)
		{
			pathNodes[i].MakePathNode();
			int num = pathNodes[i + 1].Elevation - pathNodes[i].Elevation;
			if (num != 0)
			{
				if (pathNodes[i].Row < pathNodes[i + 1].Row)
				{
					if (!DoesPathExist(startPoint, pathNodes[i + 1].Location))
					{
						if (num == 1)
						{
							pathNodes[i].NorthEdgeValue = LevelNodeEdgeValue.Ladder;
						}
						else if (num == -1)
						{
							pathNodes[i + 1].SouthEdgeValue = LevelNodeEdgeValue.Ladder;
						}
						else if (num == 2)
						{
							pathNodes[i].NorthEdgeValue = LevelNodeEdgeValue.DoubleLadder;
						}
						else if (num == -2)
						{
							pathNodes[i + 1].SouthEdgeValue = LevelNodeEdgeValue.DoubleLadder;
						}
						else if (num == 3)
						{
							pathNodes[i].NorthEdgeValue = LevelNodeEdgeValue.DownDrain;
						}
						else if (num == -3)
						{
							pathNodes[i + 1].SouthEdgeValue = LevelNodeEdgeValue.DownDrain;
						}
						else if (num == 4)
						{
							pathNodes[i].NorthEdgeValue = LevelNodeEdgeValue.Tree;
						}
						else if (num == -4)
						{
							pathNodes[i + 1].SouthEdgeValue = LevelNodeEdgeValue.Tree;
						}
						else if (num > 0)
						{
							HandleLargeGap(pathNodes, i, i + 1, LevelDirection.North);
						}
						else
						{
							HandleLargeGap(pathNodes, i, i + 1, LevelDirection.South);
						}
					}
				}
				else if (pathNodes[i].Row > pathNodes[i + 1].Row)
				{
					if (!DoesPathExist(startPoint, pathNodes[i + 1].Location))
					{
						if (num == 1)
						{
							pathNodes[i].SouthEdgeValue = LevelNodeEdgeValue.Ladder;
						}
						else if (num == -1)
						{
							pathNodes[i + 1].NorthEdgeValue = LevelNodeEdgeValue.Ladder;
						}
						else if (num == 2)
						{
							pathNodes[i].SouthEdgeValue = LevelNodeEdgeValue.DoubleLadder;
						}
						else if (num == -2)
						{
							pathNodes[i + 1].NorthEdgeValue = LevelNodeEdgeValue.DoubleLadder;
						}
						else if (num == 3)
						{
							pathNodes[i].SouthEdgeValue = LevelNodeEdgeValue.DownDrain;
						}
						else if (num == -3)
						{
							pathNodes[i + 1].NorthEdgeValue = LevelNodeEdgeValue.DownDrain;
						}
						else if (num == 4)
						{
							pathNodes[i].SouthEdgeValue = LevelNodeEdgeValue.Tree;
						}
						else if (num == -4)
						{
							pathNodes[i + 1].NorthEdgeValue = LevelNodeEdgeValue.Tree;
						}
						else if (num > 0)
						{
							HandleLargeGap(pathNodes, i, i + 1, LevelDirection.South);
						}
						else
						{
							HandleLargeGap(pathNodes, i, i + 1, LevelDirection.North);
						}
					}
				}
				else if (pathNodes[i].Col < pathNodes[i + 1].Col)
				{
					if (!DoesPathExist(startPoint, pathNodes[i + 1].Location))
					{
						if (num == 1)
						{
							pathNodes[i].EastEdgeValue = LevelNodeEdgeValue.Ladder;
						}
						else if (num == -1)
						{
							pathNodes[i + 1].WestEdgeValue = LevelNodeEdgeValue.Ladder;
						}
						else if (num == 2)
						{
							pathNodes[i].EastEdgeValue = LevelNodeEdgeValue.DoubleLadder;
						}
						else if (num == -2)
						{
							pathNodes[i + 1].WestEdgeValue = LevelNodeEdgeValue.DoubleLadder;
						}
						else if (num == 3)
						{
							pathNodes[i].EastEdgeValue = LevelNodeEdgeValue.DownDrain;
						}
						else if (num == -3)
						{
							pathNodes[i + 1].WestEdgeValue = LevelNodeEdgeValue.DownDrain;
						}
						else if (num == 4)
						{
							pathNodes[i].EastEdgeValue = LevelNodeEdgeValue.Tree;
						}
						else if (num == -4)
						{
							pathNodes[i + 1].WestEdgeValue = LevelNodeEdgeValue.Tree;
						}
						else if (num > 0)
						{
							HandleLargeGap(pathNodes, i, i + 1, LevelDirection.East);
						}
						else
						{
							HandleLargeGap(pathNodes, i, i + 1, LevelDirection.West);
						}
					}
				}
				else if (pathNodes[i].Col > pathNodes[i + 1].Col && !DoesPathExist(startPoint, pathNodes[i + 1].Location))
				{
					if (num == 1)
					{
						pathNodes[i].WestEdgeValue = LevelNodeEdgeValue.Ladder;
					}
					else if (num == -1)
					{
						pathNodes[i + 1].EastEdgeValue = LevelNodeEdgeValue.Ladder;
					}
					else if (num == 2)
					{
						pathNodes[i].WestEdgeValue = LevelNodeEdgeValue.DoubleLadder;
					}
					else if (num == -2)
					{
						pathNodes[i + 1].EastEdgeValue = LevelNodeEdgeValue.DoubleLadder;
					}
					else if (num == 3)
					{
						pathNodes[i].WestEdgeValue = LevelNodeEdgeValue.DownDrain;
					}
					else if (num == -3)
					{
						pathNodes[i + 1].EastEdgeValue = LevelNodeEdgeValue.DownDrain;
					}
					else if (num == 4)
					{
						pathNodes[i].WestEdgeValue = LevelNodeEdgeValue.Tree;
					}
					else if (num == -4)
					{
						pathNodes[i + 1].EastEdgeValue = LevelNodeEdgeValue.Tree;
					}
					else if (num > 0)
					{
						HandleLargeGap(pathNodes, i, i + 1, LevelDirection.West);
					}
					else
					{
						HandleLargeGap(pathNodes, i, i + 1, LevelDirection.East);
					}
				}
			}
			else
			{
				int num2 = pathNodes[i + 1].Col - pathNodes[i].Col;
				int num3 = pathNodes[i + 1].Row - pathNodes[i].Row;
				if (num2 > 0 && pathNodes[i + 1].WestEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i + 1].WestEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num2 > 0 && pathNodes[i].EastEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i].EastEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num2 < 0 && pathNodes[i + 1].EastEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i + 1].EastEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num2 < 0 && pathNodes[i].WestEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i].WestEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num3 > 0 && pathNodes[i + 1].SouthEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i + 1].SouthEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num3 > 0 && pathNodes[i].NorthEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i].NorthEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num3 < 0 && pathNodes[i + 1].NorthEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i + 1].NorthEdgeValue = LevelNodeEdgeValue.Empty;
				}
				if (num3 < 0 && pathNodes[i].SouthEdgeValue != LevelNodeEdgeValue.Empty)
				{
					pathNodes[i].SouthEdgeValue = LevelNodeEdgeValue.Empty;
				}
			}
		}
	}

	private void HandleLargeGap(List<LevelNode> pathNodes, int fromIndex, int toIndex, LevelDirection direction)
	{
		if (fromIndex > 0)
		{
			if (pathNodes[fromIndex].Elevation < pathNodes[toIndex].Elevation)
			{
				pathNodes[fromIndex].Elevation += 3;
			}
			else
			{
				pathNodes[fromIndex].Elevation -= 3;
			}
			List<LevelNode> list = new List<LevelNode>();
			list.Add(pathNodes[fromIndex - 1]);
			list.Add(pathNodes[fromIndex]);
			list.Add(pathNodes[toIndex]);
			HandlePathElevationChange(list);
		}
		else
		{
			Debug.LogError("Cannot handle large gab on first node of level!");
		}
	}

	public void SetupTrees(LevelStyle style, LevelTheme theme, GameplayEngine gameplayEngine)
	{
		LevelPlacementHelper levelPlacementHelper = new LevelPlacementHelper(nodes, chunkDB);
		int a = Mathf.RoundToInt((float)style.minTrees * theme.treeFactor);
		int b = Mathf.RoundToInt((float)style.maxTrees * theme.treeFactor);
		int num = LevelHelper.RandomRangeToMax(a, b);
		int num2 = Mathf.CeilToInt((float)num / (float)style.treeClusterSize);
		if (num2 == 0)
		{
			num2 = 1;
		}
		int num3 = Mathf.RoundToInt((float)num / (float)num2);
		List<LevelPlateau> list = new List<LevelPlateau>();
		for (int i = 0; i < num2; i++)
		{
			LevelPlateau levelPlateau = null;
			int num4 = 0;
			foreach (LevelPlateau levelPlateau2 in chunkDB.LevelPlateaus)
			{
				if (list.Contains(levelPlateau2))
				{
					continue;
				}
				int num5 = Mathf.RoundToInt((float)levelPlateau2.Elevation * 10f * style.treePlacementTopFactor);
				if (levelPlateau2.nodes.Count > num3 * 5)
				{
					num5 += 25;
				}
				int num6 = 0;
				int num7 = 0;
				foreach (LevelNode node in levelPlateau2.nodes)
				{
					if (chunkDB.GetChunk(node.ChunkId).ChunkType == LevelChunkType.MainPath || chunkDB.GetChunk(node.ChunkId).ChunkType == LevelChunkType.SideQuest)
					{
						num6++;
					}
					if (node.ObjectTypes.Count > 0)
					{
						num7++;
					}
				}
				num5 += Mathf.RoundToInt((float)num6 / (float)levelPlateau2.nodes.Count * 50f);
				num5 += Mathf.RoundToInt((1f - (float)num7 / (float)levelPlateau2.nodes.Count) * 25f);
				if (num5 > num4)
				{
					num4 = num5;
					levelPlateau = levelPlateau2;
				}
			}
			if (levelPlateau == null)
			{
				continue;
			}
			for (int j = 0; j < num3; j++)
			{
				LevelNode levelNode = null;
				int num8 = 100;
				do
				{
					levelNode = levelPlateau.nodes[UnityEngine.Random.Range(0, levelPlateau.nodes.Count)];
					num8--;
				}
				while ((!levelPlacementHelper.EvaluateNode(levelNode, LevelNodeObjectType.Tree) || levelNode.Type == LevelNodeType.House) && num8 > 0);
				if (levelNode != null && num8 > 0)
				{
					levelNode.AddObjectType(LevelNodeObjectType.Tree, chunkDB, string.Empty);
				}
			}
			list.Add(levelPlateau);
		}
	}

	public void FillHolesInPath(LevelNodeType type)
	{
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].Elevation == 0)
				{
					List<LevelNode> neighboursOfType = LevelHelper.GetNeighboursOfType(new LevelPoint(i, j), type, nodes);
					if (neighboursOfType.Count == 3)
					{
						chunkDB.ResetNode(nodes[i, j]);
						nodes[i, j].Elevation = neighboursOfType[0].Elevation;
						nodes[i, j].Type = type;
						LevelChunk chunk = chunkDB.GetChunk(neighboursOfType[0].ChunkId);
						chunk.AddNode(chunkDB, nodes[i, j]);
					}
				}
			}
		}
	}

	public void LonelyCellRemoval()
	{
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				int elevation = nodes[i, j].Elevation;
				if (elevation == 0)
				{
					continue;
				}
				List<LevelNode> neighbours = LevelHelper.GetNeighbours(new LevelPoint(i, j), nodes);
				int num = 0;
				float num2 = 0f;
				foreach (LevelNode item in neighbours)
				{
					if (item.Elevation != elevation)
					{
						num++;
					}
					num2 += (float)item.Elevation;
				}
				num2 /= (float)neighbours.Count;
				if (neighbours.Count == num && num > 0)
				{
					nodes[i, j].Elevation = (int)Mathf.Ceil(num2);
				}
			}
		}
	}

	private int GetAvgElevation(LevelPoint center, int rowExtent, int colExtent)
	{
		int num = 0;
		int num2 = 0;
		for (int i = center.row - rowExtent / 2 - ((rowExtent % 2 == 1) ? 1 : 0); i <= center.row + rowExtent / 2 - ((rowExtent % 2 == 1) ? 1 : 0); i++)
		{
			for (int j = center.col - colExtent / 2 - ((colExtent % 2 == 1) ? 1 : 0); j <= center.col + colExtent / 2 - ((colExtent % 2 == 1) ? 1 : 0); j++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(i, j), nodes))
				{
					num2 += elevationMap[i, j];
					num++;
				}
			}
		}
		if (num == 0)
		{
			return 1;
		}
		return Mathf.Max((int)Mathf.Ceil(num2 / num), 1);
	}

	private int GetAvgElevationNorthOrSouth(LevelPoint center, int rowExtent, int colExtent, LevelDirection edge)
	{
		if (edge != LevelDirection.North && edge != LevelDirection.South)
		{
			Debug.LogError("Method can only take North or South as input, not " + edge);
			return 1;
		}
		int num = 0;
		switch (edge)
		{
		case LevelDirection.North:
			num = center.row + rowExtent / 2 - ((rowExtent % 2 == 1) ? 1 : 0);
			break;
		case LevelDirection.South:
			num = center.row - rowExtent / 2 - ((rowExtent % 2 == 1) ? 1 : 0);
			break;
		}
		int num2 = 0;
		int num3 = 0;
		for (int i = center.col - colExtent / 2 - ((colExtent % 2 == 1) ? 1 : 0); i <= center.col + colExtent / 2 - ((colExtent % 2 == 1) ? 1 : 0); i++)
		{
			if (LevelHelper.PointInsideBounds(new LevelPoint(num, i), nodes))
			{
				num3 += elevationMap[num, i];
				num2++;
			}
		}
		if (num2 == 0)
		{
			return 1;
		}
		return Mathf.Max((int)Mathf.Ceil(num3 / num2), 1);
	}

	public bool DoesPathExist(LevelPoint from, LevelPoint to, bool ignoreKeyDoors = true)
	{
		if (ignoreKeyDoors && pathFinding.GetPathIgnoreDoors(nodes[from.row, from.col], nodes[to.row, to.col], null) == null)
		{
			return false;
		}
		if (!ignoreKeyDoors && pathFinding.GetPathIncludeDoors(nodes[from.row, from.col], nodes[to.row, to.col], null) == null)
		{
			return false;
		}
		return true;
	}

	private LevelNode GetClosetNodeOfType(LevelPoint point, LevelNodeType type)
	{
		float num = float.MaxValue;
		LevelNode result = null;
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				if (nodes[i, j].Type == type)
				{
					float num2 = LevelHelper.Distance(point, new LevelPoint(i, j));
					if (num2 < num)
					{
						num = num2;
						result = nodes[i, j];
					}
				}
			}
		}
		return result;
	}
}
