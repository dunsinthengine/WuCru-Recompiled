using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelDecorationBuilder : MonoBehaviour
{
	private enum ColorMode
	{
		GroundColor = 0,
		WallColor = 1,
		UnColored = 2,
		Textured = 3
	}

	public List<DecorationForeground> decorationForegrounds = new List<DecorationForeground>();

	private int maxElevation;

	private int depthLevel = 10;

	private List<LevelPoint> usedList = new List<LevelPoint>();

	private bool useDebugColors;

	private int groundOffset = 20;

	private float groundHeightVariation = 0.3f;

	public void DecorateLevel(Level level, LevelStyle style, GameObject levelRoot, List<GameObject> levelMeshGO, Dictionary<int, Vector3> vertexMap)
	{
		usedList.Clear();
		maxElevation = LevelHelper.GetMaxElevation(level);
		bool flag = false;
		if (style.decorationInserts.Count > 0 && style.decorationInsertsFree.Count > 0 && !flag)
		{
			DecorateWithInserts(level, style, levelMeshGO);
		}
		if (style.decorationAddOns.Count > 0 && !flag)
		{
			DecorateWithAddOns(level, style, levelMeshGO, ColorMode.GroundColor, style.decorationAddOns);
		}
		if (style.decorationAddOnsColored.Count > 0 && !flag)
		{
			DecorateWithAddOns(level, style, levelMeshGO, ColorMode.UnColored, style.decorationAddOnsColored);
		}
		if (style.decorationAddOnsWallColor.Count > 0 && !flag)
		{
			DecorateWithAddOns(level, style, levelMeshGO, ColorMode.WallColor, style.decorationAddOnsWallColor);
		}
		if (style.decorationHangouts.Count > 0)
		{
			DecorateWithHangouts(level, levelMeshGO, vertexMap);
		}
		if (style.decorationInserts.Count > 0 && style.decorationInsertsFree.Count > 0)
		{
			DecorateOutsideLevel(level, levelRoot);
		}
		if (style.backDecorationInsertsHuge.Count > 0)
		{
			DecorateBackgroundHuge(level, style, levelRoot);
		}
		if (style.decorationForeground.Count > 0)
		{
			DecorateForeground(level, style, levelRoot);
		}
		if (style.seededPrefabs != null)
		{
			int count = style.seededPrefabs.Count;
			for (int i = 0; i < count; i++)
			{
				if (style.seededPrefabs[i].seed == level.Seed)
				{
					int num = style.seededPrefabs[i].prefabList.Length;
					for (int j = 0; j < num; j++)
					{
						Object.Instantiate(style.seededPrefabs[i].prefabList[j]);
					}
					num = style.seededPrefabs[i].resourceLoadedPrefabs.Length;
					for (int k = 0; k < num; k++)
					{
						GameObject original = Resources.Load<GameObject>(style.seededPrefabs[i].resourceLoadedPrefabs[k]);
						Object.Instantiate(original);
					}
				}
			}
		}
		DecorateGround(level, style, levelRoot);
	}

	private void DecorateWithInserts(Level level, LevelStyle style, List<GameObject> levelMeshGO)
	{
		int num = level.Nodes.GetLength(1) / 5 + ((level.Nodes.GetLength(1) % 5 > 0) ? 1 : 0);
		foreach (LevelPlateau levelPlateau in level.ChunkDB.LevelPlateaus)
		{
			List<LevelNode> perimeterNodes = levelPlateau.GetPerimeterNodes(level);
			foreach (LevelNode item5 in perimeterNodes)
			{
				LevelNode neighbour = item5.GetNeighbour(level.Nodes, LevelDirection.North);
				LevelNode neighbour2 = item5.GetNeighbour(level.Nodes, LevelDirection.South);
				LevelNode neighbour3 = item5.GetNeighbour(level.Nodes, LevelDirection.East);
				LevelNode neighbour4 = item5.GetNeighbour(level.Nodes, LevelDirection.West);
				DecorationEvalResult decorationEvalResult = EvaluateNodeForInsert(neighbour, level.Nodes, LevelDirection.North);
				DecorationEvalResult decorationEvalResult2 = EvaluateNodeForInsert(neighbour2, level.Nodes, LevelDirection.South);
				DecorationEvalResult decorationEvalResult3 = EvaluateNodeForInsert(neighbour3, level.Nodes, LevelDirection.East);
				DecorationEvalResult decorationEvalResult4 = EvaluateNodeForInsert(neighbour4, level.Nodes, LevelDirection.West);
				int num2 = item5.Row / 5;
				int num3 = item5.Col / 5;
				GameObject levelRoot = levelMeshGO[num3 + num2 * num];
				if (neighbour == null || neighbour.Elevation < item5.Elevation)
				{
					LevelPoint item = new LevelPoint(item5.Row + 1, item5.Col);
					if (!usedList.Contains(item))
					{
						int num4 = item5.Elevation;
						if (neighbour != null)
						{
							num4 = item5.Elevation - neighbour.Elevation;
						}
						Vector3 scale = new Vector3(1f, num4, 1f);
						Vector3 pos = new Vector3(item5.Col, item5.Elevation - num4, item5.Row);
						switch (decorationEvalResult)
						{
						case DecorationEvalResult.Free:
							if (Random.value <= style.decoBackFreeProbability)
							{
								pos += new Vector3(0f, -depthLevel, style.decoFreeOffset);
								scale += new Vector3(0f, 1f + Random.Range(0f - style.decoBackFreeScaleVariation, style.decoBackFreeScaleVariation) + (float)depthLevel, 0f);
								InstantiateInsertMesh(pos, scale, level, levelRoot, item5, decorationEvalResult);
								usedList.Add(item);
							}
							break;
						case DecorationEvalResult.Medium:
							if (Random.value <= style.decoBackProbability)
							{
								pos += new Vector3(0f, 0f, style.decoMediumOffset);
								scale += new Vector3(0f, -0.1f + Random.Range(0f - style.decoBackMediumScaleVariation, style.decoBackMediumScaleVariation), 0f);
								InstantiateInsertMesh(pos, scale, level, levelRoot, item5, decorationEvalResult);
								usedList.Add(item);
							}
							break;
						case DecorationEvalResult.Restricted:
							if (Random.value <= style.decoBackProbability)
							{
								pos += new Vector3(0f, 0f, style.decoRestrictedOffset);
								scale += new Vector3(0f, -0.25f + Random.Range(0f - style.decoBackRestrictedScaleVariation, style.decoBackRestrictedScaleVariation), 0f);
								InstantiateInsertMesh(pos, scale, level, levelRoot, item5, decorationEvalResult);
								usedList.Add(item);
							}
							break;
						}
					}
				}
				if (neighbour2 == null || neighbour2.Elevation < item5.Elevation)
				{
					LevelPoint item2 = new LevelPoint(item5.Row - 1, item5.Col);
					if (!usedList.Contains(item2))
					{
						int num5 = item5.Elevation;
						if (neighbour2 != null)
						{
							num5 = item5.Elevation - neighbour2.Elevation;
						}
						Vector3 scale2 = new Vector3(1f, num5, 1f);
						Vector3 pos2 = new Vector3(item5.Col, item5.Elevation - num5, item5.Row);
						switch (decorationEvalResult2)
						{
						case DecorationEvalResult.Free:
							if (Random.value <= style.decoOtherFreeProbability)
							{
								pos2 += new Vector3(0f, -depthLevel, 0f - style.decoFreeOffset);
								scale2 += new Vector3(0f, 0.2f + Random.Range(0f - style.decoOtherFreeScaleVariation, style.decoOtherFreeScaleVariation) + (float)depthLevel, 0f);
								InstantiateInsertMesh(pos2, scale2, level, levelRoot, item5, decorationEvalResult2);
								usedList.Add(item2);
							}
							break;
						case DecorationEvalResult.Medium:
							if (Random.value <= style.decoOtherProbability)
							{
								pos2 += new Vector3(0f, 0f, 0f - style.decoMediumOffset);
								scale2 += new Vector3(0f, -0.15f + Random.Range(0f - style.decoOtherMediumScaleVariation, style.decoOtherMediumScaleVariation), 0f);
								InstantiateInsertMesh(pos2, scale2, level, levelRoot, item5, decorationEvalResult2);
								usedList.Add(item2);
							}
							break;
						case DecorationEvalResult.Restricted:
							if (Random.value <= style.decoOtherProbability)
							{
								pos2 += new Vector3(0f, 0f, 0f - style.decoRestrictedOffset);
								scale2 += new Vector3(0f, -0.3f + Random.Range(0f - style.decoOtherRestrictedScaleVariation, style.decoOtherRestrictedScaleVariation), 0f);
								InstantiateInsertMesh(pos2, scale2, level, levelRoot, item5, decorationEvalResult2);
								usedList.Add(item2);
							}
							break;
						}
					}
				}
				if (neighbour3 == null || neighbour3.Elevation < item5.Elevation)
				{
					LevelPoint item3 = new LevelPoint(item5.Row, item5.Col + 1);
					if (!usedList.Contains(item3))
					{
						int num6 = item5.Elevation;
						if (neighbour3 != null)
						{
							num6 = item5.Elevation - neighbour3.Elevation;
						}
						Vector3 scale3 = new Vector3(1f, num6, 1f);
						Vector3 pos3 = new Vector3(item5.Col, item5.Elevation - num6, item5.Row);
						switch (decorationEvalResult3)
						{
						case DecorationEvalResult.Free:
							if (Random.value <= style.decoOtherFreeProbability)
							{
								pos3 += new Vector3(style.decoFreeOffset, -depthLevel, 0f);
								scale3 += new Vector3(0f, 0.1f + Random.Range(0f - style.decoOtherFreeScaleVariation, style.decoOtherFreeScaleVariation) + (float)depthLevel, 0f);
								InstantiateInsertMesh(pos3, scale3, level, levelRoot, item5, decorationEvalResult3);
								usedList.Add(item3);
							}
							break;
						case DecorationEvalResult.Medium:
							if (Random.value <= style.decoOtherProbability)
							{
								pos3 += new Vector3(style.decoMediumOffset, 0f, 0f);
								scale3 += new Vector3(0f, -0.15f + Random.Range(0f - style.decoOtherMediumScaleVariation, style.decoOtherMediumScaleVariation), 0f);
								InstantiateInsertMesh(pos3, scale3, level, levelRoot, item5, decorationEvalResult3);
								usedList.Add(item3);
							}
							break;
						case DecorationEvalResult.Restricted:
							if (Random.value <= style.decoOtherProbability)
							{
								pos3 += new Vector3(style.decoRestrictedOffset, 0f, 0f);
								scale3 += new Vector3(0f, -0.3f + Random.Range(0f - style.decoOtherRestrictedScaleVariation, style.decoOtherRestrictedScaleVariation), 0f);
								InstantiateInsertMesh(pos3, scale3, level, levelRoot, item5, decorationEvalResult3);
								usedList.Add(item3);
							}
							break;
						}
					}
				}
				if (neighbour4 != null && neighbour4.Elevation >= item5.Elevation)
				{
					continue;
				}
				LevelPoint item4 = new LevelPoint(item5.Row, item5.Col - 1);
				if (usedList.Contains(item4))
				{
					continue;
				}
				int num7 = item5.Elevation;
				if (neighbour4 != null)
				{
					num7 = item5.Elevation - neighbour4.Elevation;
				}
				Vector3 scale4 = new Vector3(1f, num7, 1f);
				Vector3 pos4 = new Vector3(item5.Col, item5.Elevation - num7, item5.Row);
				switch (decorationEvalResult4)
				{
				case DecorationEvalResult.Free:
					if (Random.value <= style.decoOtherFreeProbability)
					{
						pos4 += new Vector3(0f - style.decoFreeOffset, -depthLevel, 0f);
						scale4 += new Vector3(0f, 0.1f + Random.Range(0f - style.decoOtherFreeScaleVariation, style.decoOtherFreeScaleVariation) + (float)depthLevel, 0f);
						InstantiateInsertMesh(pos4, scale4, level, levelRoot, item5, decorationEvalResult4);
						usedList.Add(item4);
					}
					break;
				case DecorationEvalResult.Medium:
					if (Random.value <= style.decoOtherProbability)
					{
						pos4 += new Vector3(0f - style.decoMediumOffset, 0f, 0f);
						scale4 += new Vector3(0f, -0.15f + Random.Range(0f - style.decoOtherMediumScaleVariation, style.decoOtherMediumScaleVariation), 0f);
						InstantiateInsertMesh(pos4, scale4, level, levelRoot, item5, decorationEvalResult4);
						usedList.Add(item4);
					}
					break;
				case DecorationEvalResult.Restricted:
					if (Random.value <= style.decoOtherProbability)
					{
						pos4 += new Vector3(0f - style.decoRestrictedOffset, 0f, 0f);
						scale4 += new Vector3(0f, -0.3f + Random.Range(0f - style.decoOtherRestrictedScaleVariation, style.decoOtherRestrictedScaleVariation), 0f);
						InstantiateInsertMesh(pos4, scale4, level, levelRoot, item5, decorationEvalResult4);
						usedList.Add(item4);
					}
					break;
				}
			}
		}
	}

	private DecorationEvalResult EvaluateNodeForInsert(LevelNode node, LevelNode[,] nodes, LevelDirection direction)
	{
		if (node == null || node.Type == LevelNodeType.None || node.Elevation == 0)
		{
			return DecorationEvalResult.Free;
		}
		if (!LevelHelper.NodeEdgeIsEmptyAnyDirection(node, nodes))
		{
			return DecorationEvalResult.Rejected;
		}
		if (node.Type == LevelNodeType.House)
		{
			return DecorationEvalResult.Rejected;
		}
		if (node.Objects.Count > 0)
		{
			return DecorationEvalResult.Rejected;
		}
		LevelNode neighbour = node.GetNeighbour(nodes, LevelDirection.North);
		LevelNode neighbour2 = node.GetNeighbour(nodes, LevelDirection.South);
		LevelNode neighbour3 = node.GetNeighbour(nodes, LevelDirection.East);
		LevelNode neighbour4 = node.GetNeighbour(nodes, LevelDirection.West);
		if (neighbour != null && neighbour.HasLadderAnySize(LevelDirection.South))
		{
			return DecorationEvalResult.Rejected;
		}
		if (neighbour2 != null && neighbour2.HasLadderAnySize(LevelDirection.North))
		{
			return DecorationEvalResult.Rejected;
		}
		if (neighbour3 != null && neighbour3.HasLadderAnySize(LevelDirection.West))
		{
			return DecorationEvalResult.Rejected;
		}
		if (neighbour4 != null && neighbour4.HasLadderAnySize(LevelDirection.East))
		{
			return DecorationEvalResult.Rejected;
		}
		if (neighbour != null && neighbour.HasLadderAnySizeAnyDirection())
		{
			return DecorationEvalResult.Restricted;
		}
		if (neighbour2 != null && neighbour2.HasLadderAnySizeAnyDirection())
		{
			return DecorationEvalResult.Restricted;
		}
		if (neighbour3 != null && neighbour3.HasLadderAnySizeAnyDirection())
		{
			return DecorationEvalResult.Restricted;
		}
		if (neighbour4 != null && neighbour4.HasLadderAnySizeAnyDirection())
		{
			return DecorationEvalResult.Restricted;
		}
		if (direction == LevelDirection.North && LevelHelper.GetLastFilledRowAtCol(nodes, node.Col) < node.Row)
		{
			return DecorationEvalResult.Free;
		}
		if (direction == LevelDirection.South && LevelHelper.GetFirstFilledRowAtCol(nodes, node.Col) > node.Row)
		{
			return DecorationEvalResult.Free;
		}
		if (direction == LevelDirection.East && LevelHelper.GetLastFilledColAtRow(nodes, node.Row) < node.Col)
		{
			return DecorationEvalResult.Free;
		}
		if (direction == LevelDirection.West && LevelHelper.GetFirstFilledColAtRow(nodes, node.Row) > node.Col)
		{
			return DecorationEvalResult.Free;
		}
		return DecorationEvalResult.Medium;
	}

	private void InstantiateInsertMeshOutsideLevel(Vector3 pos, Vector3 scale, Level level, GameObject levelRoot, bool Back)
	{
		int num = (int)Mathf.Ceil(Mathf.Max(scale.x, scale.z));
		int num2 = Mathf.RoundToInt(pos.z);
		int num3 = Mathf.RoundToInt(pos.x);
		for (int i = num2 - num; i <= num2 + num; i++)
		{
			for (int j = num3 - num; j <= num3 + num; j++)
			{
				if (!LevelHelper.PointInsideBounds(new LevelPoint(i, j), level.Nodes))
				{
					continue;
				}
				LevelNode levelNode = level.Nodes[i, j];
				if (levelNode == null || levelNode.Elevation <= 0)
				{
					continue;
				}
				if (LevelHelper.PointInsideBounds(new LevelPoint(num2, num3), level.Nodes))
				{
					levelNode = level.Nodes[num2, num3];
					if (levelNode != null && levelNode.Elevation > 0)
					{
						return;
					}
					scale = new Vector3(1f, scale.y, 1f);
				}
				else
				{
					scale = new Vector3(1f, scale.y, 1f);
				}
			}
		}
		GameObject original = ((!Back || level.LevelStyle.backDecorationInsertsFree.Count <= 0) ? level.LevelStyle.decorationInsertsFree[Random.Range(0, level.LevelStyle.decorationInsertsFree.Count)] : level.LevelStyle.backDecorationInsertsFree[Random.Range(0, level.LevelStyle.backDecorationInsertsFree.Count)]);
		float num4 = 0f;
		float value = Random.value;
		if ((double)value >= 0.25 && value < 0.5f)
		{
			num4 = 90f;
		}
		if (value >= 0.5f && value < 0.75f)
		{
			num4 = 180f;
		}
		if (value >= 0.75f)
		{
			num4 = -90f;
		}
		GameObject gameObject = Object.Instantiate(original, pos, Quaternion.Euler(0f, num4 + Random.Range(-5f, 5f), 0f));
		scale = new Vector3(scale.x, scale.y / 10f, scale.z);
		gameObject.transform.localScale = scale;
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
		gameObject.tag = "LevelMesh";
		MeshFilter componentInChildren = gameObject.GetComponentInChildren<MeshFilter>();
		MeshRenderer componentInChildren2 = gameObject.GetComponentInChildren<MeshRenderer>();
		Color[] array = new Color[componentInChildren.mesh.vertices.Length];
		for (int k = 0; k < array.Length; k++)
		{
			array[k] = level.LevelStyle.sideColor;
		}
		componentInChildren.mesh.colors = array;
		componentInChildren2.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		componentInChildren2.receiveShadows = false;
		componentInChildren2.shadowCastingMode = ShadowCastingMode.Off;
	}

	private void InstantiateInsertMesh(Vector3 pos, Vector3 scale, Level level, GameObject levelRoot, LevelNode node, DecorationEvalResult evalResult)
	{
		Color color = GetColorFromNode(node, level, true);
		if (useDebugColors)
		{
			switch (evalResult)
			{
			case DecorationEvalResult.Free:
				color = Color.green;
				break;
			case DecorationEvalResult.Medium:
				color = Color.yellow;
				break;
			case DecorationEvalResult.Restricted:
				color = Color.red;
				break;
			}
		}
		float num = 0f;
		float value = Random.value;
		if ((double)value >= 0.25 && value < 0.5f)
		{
			num = 90f;
		}
		if (value >= 0.5f && value < 0.75f)
		{
			num = 180f;
		}
		if (value >= 0.75f)
		{
			num = -90f;
		}
		GameObject gameObject = null;
		if (evalResult == DecorationEvalResult.Free)
		{
			scale = new Vector3(scale.x, scale.y / 10f, scale.z);
			int index = Random.Range(0, level.LevelStyle.decorationInsertsFree.Count);
			gameObject = Object.Instantiate(level.LevelStyle.decorationInsertsFree[index], pos, Quaternion.Euler(0f, num + Random.Range(-10f, 10f), 0f));
		}
		else
		{
			int index2 = Random.Range(0, level.LevelStyle.decorationInserts.Count);
			gameObject = Object.Instantiate(level.LevelStyle.decorationInserts[index2], pos, Quaternion.Euler(0f, num + Random.Range(-5f, 5f), 0f));
		}
		gameObject.transform.localScale = scale;
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
		gameObject.tag = "LevelMesh";
		MeshFilter componentInChildren = gameObject.GetComponentInChildren<MeshFilter>();
		MeshRenderer componentInChildren2 = gameObject.GetComponentInChildren<MeshRenderer>();
		Color[] array = new Color[componentInChildren.mesh.vertices.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		componentInChildren.mesh.colors = array;
		componentInChildren2.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		componentInChildren2.receiveShadows = false;
		componentInChildren2.shadowCastingMode = ShadowCastingMode.Off;
	}

	private void InstantiateAddOnMesh(Vector3 pos, Vector3 scale, Level level, GameObject levelRoot, LevelNode node, ColorMode colorMode, List<GameObject> InstantiateList)
	{
		Color color = Color.white;
		if (colorMode == ColorMode.GroundColor)
		{
			color = GetColorFromNode(node, level, false);
		}
		if (colorMode == ColorMode.WallColor)
		{
			color = GetColorFromNode(node, level, true);
		}
		GameObject gameObject = null;
		if (colorMode == ColorMode.UnColored)
		{
			int index = Random.Range(0, InstantiateList.Count);
			if (InstantiateList[index] == null)
			{
				Debug.LogError("Missing item in index");
			}
			gameObject = Object.Instantiate(InstantiateList[index], pos, Quaternion.Euler(0f, Random.Range(-45f, 45f), 0f));
		}
		else
		{
			int index2 = Random.Range(0, InstantiateList.Count);
			gameObject = Object.Instantiate(InstantiateList[index2], pos, Quaternion.Euler(0f, Random.Range(-45f, 45f), 0f));
		}
		gameObject.transform.localScale = scale;
		gameObject.transform.parent = levelRoot.transform;
		gameObject.isStatic = true;
		MeshFilter componentInChildren = gameObject.GetComponentInChildren<MeshFilter>();
		MeshRenderer componentInChildren2 = gameObject.GetComponentInChildren<MeshRenderer>();
		if (colorMode == ColorMode.GroundColor || colorMode == ColorMode.WallColor)
		{
			Color[] array = new Color[componentInChildren.mesh.vertices.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			componentInChildren.mesh.colors = array;
		}
		if (componentInChildren2.material.shader != Shader.Find(LevelMeshBuilder.shaderNameTextured))
		{
			componentInChildren2.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		}
		componentInChildren2.receiveShadows = false;
		componentInChildren2.shadowCastingMode = ShadowCastingMode.Off;
	}

	private void InstantiateHangout(Vector3 pos, Level level, GameObject levelRoot, LevelNode node, LevelDirection direction)
	{
		Color colorFromNode = GetColorFromNode(node, level, true);
		Color colorFromNode2 = GetColorFromNode(node, level, false);
		float y = 0f;
		if (direction == LevelDirection.East)
		{
			y = 90f;
		}
		if (direction == LevelDirection.South)
		{
			y = 180f;
		}
		if (direction == LevelDirection.West)
		{
			y = -90f;
		}
		int index = Random.Range(0, level.LevelStyle.decorationHangouts.Count);
		GameObject gameObject = Object.Instantiate(level.LevelStyle.decorationHangouts[index], pos, Quaternion.Euler(0f, y, 0f));
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
		Hangout component = gameObject.GetComponent<Hangout>();
		Color[] array = new Color[component.side.mesh.vertices.Length];
		Color[] array2 = new Color[component.top.mesh.vertices.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = colorFromNode;
		}
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = colorFromNode2;
		}
		component.side.mesh.colors = array;
		component.top.mesh.colors = array2;
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array3 = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array3)
		{
			meshRenderer.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
			meshRenderer.receiveShadows = false;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		}
	}

	private Color GetColorFromNode(LevelNode node, Level level, bool fromSide)
	{
		LevelNodeType type = node.Type;
		if (type == LevelNodeType.Room)
		{
			if (fromSide)
			{
				return level.LevelStyle.sideColor;
			}
			if (node.Type != LevelNodeType.House)
			{
				return LevelMeshBuilder.GetElevationFloorColor(level.LevelStyle, node.Elevation);
			}
			return level.LevelStyle.houseFloorColor;
		}
		if (fromSide)
		{
			return level.LevelStyle.sideColor;
		}
		return LevelMeshBuilder.GetElevationFloorColor(level.LevelStyle, node.Elevation);
	}

	private void DecorateWithAddOns(Level level, LevelStyle style, List<GameObject> levelMeshGO, ColorMode colorMode, List<GameObject> InstantiationList)
	{
		int num = level.Nodes.GetLength(1) / 5 + ((level.Nodes.GetLength(1) % 5 > 0) ? 1 : 0);
		LevelNode[,] nodes = level.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = nodes[i, j];
				if (levelNode.Objects.Count != 0)
				{
					continue;
				}
				int num2 = levelNode.Row / 5;
				int num3 = levelNode.Col / 5;
				GameObject levelRoot = levelMeshGO[num3 + num2 * num];
				if (levelNode.Type == LevelNodeType.MainPath)
				{
					MainPathChunk mainPathChunk = (MainPathChunk)level.ChunkDB.GetChunk(levelNode.ChunkId);
					float num4 = mainPathChunk.Path.ClosestDistanceToPoint(levelNode.Location);
					float num5 = 1f - Mathf.Clamp01((float)level.LevelTheme.mainPathMinWidth / 2f - num4);
					if (Random.value < style.decoAddOnProbability && num5 > style.decoAddOnDistToPathFactor)
					{
						Vector3 scale = Vector3.one + new Vector3(Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation), Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation), Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation));
						InstantiateAddOnMesh(LevelHelper.NodeToVector3(levelNode) + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)), scale, level, levelRoot, levelNode, colorMode, InstantiationList);
					}
				}
				else if (levelNode.Type == LevelNodeType.SideQuest)
				{
					SideQuestChunk sideQuestChunk = (SideQuestChunk)level.ChunkDB.GetChunk(levelNode.ChunkId);
					float num6 = sideQuestChunk.Path.ClosestDistanceToPoint(levelNode.Location);
					float num7 = 1f - Mathf.Clamp01((float)level.LevelTheme.sideQuestMinWidth / 2f - num6);
					if (Random.value < style.decoAddOnProbability && num7 > style.decoAddOnDistToPathFactor)
					{
						Vector3 scale2 = Vector3.one + new Vector3(Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation), Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation), Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation));
						InstantiateAddOnMesh(LevelHelper.NodeToVector3(levelNode) + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)), scale2, level, levelRoot, levelNode, colorMode, InstantiationList);
					}
				}
				else if (levelNode.Type == LevelNodeType.Room && Random.value < style.decoAddOnProbability)
				{
					Vector3 scale3 = Vector3.one + new Vector3(Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation), Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation), Random.Range(0f - style.decoAddOnScaleVariation, style.decoAddOnScaleVariation));
					InstantiateAddOnMesh(LevelHelper.NodeToVector3(levelNode) + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)), scale3, level, levelRoot, levelNode, colorMode, InstantiationList);
				}
			}
		}
	}

	private void DecorateWithHangouts(Level level, List<GameObject> levelMeshGO, Dictionary<int, Vector3> vertexMap)
	{
		int num = level.Nodes.GetLength(0) / 5 + ((level.Nodes.GetLength(0) % 5 > 0) ? 1 : 0);
		int num2 = level.Nodes.GetLength(1) / 5 + ((level.Nodes.GetLength(1) % 5 > 0) ? 1 : 0);
		for (int i = 0; i < num; i++)
		{
			int num3 = i * 5;
			int num4 = Mathf.Min(num3 + 5, level.Nodes.GetLength(0));
			for (int j = 0; j < num2; j++)
			{
				int num5 = j * 5;
				int num6 = Mathf.Min(num5 + 5, level.Nodes.GetLength(1));
				GameObject gameObject = levelMeshGO[j + i * num2];
				Vector3[] vertices = gameObject.GetComponentInChildren<MeshFilter>().mesh.vertices;
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						LevelNode node = level.Nodes[k, l];
						LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(k, l), LevelDirection.North, level.Nodes);
						LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(new LevelPoint(k, l), LevelDirection.South, level.Nodes);
						LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(new LevelPoint(k, l), LevelDirection.East, level.Nodes);
						LevelNode neighbourAt4 = LevelHelper.GetNeighbourAt(new LevelPoint(k, l), LevelDirection.West, level.Nodes);
						if (EvaluateNodeAndDirForHangout(level, node, LevelDirection.North) && Random.value <= level.LevelStyle.decoHangoutProbabilityBack)
						{
							FixVerticesForHangouts(level, node, LevelDirection.North, vertices, vertexMap);
							InstantiateHangout(LevelHelper.NodeToVector3(node), level, gameObject, node, LevelDirection.North);
						}
						if (EvaluateNodeAndDirForHangout(level, node, LevelDirection.South) && Random.value <= level.LevelStyle.decoHangoutProbabilityFront)
						{
							FixVerticesForHangouts(level, node, LevelDirection.South, vertices, vertexMap);
							InstantiateHangout(LevelHelper.NodeToVector3(node), level, gameObject, node, LevelDirection.South);
						}
						if (EvaluateNodeAndDirForHangout(level, node, LevelDirection.East) && Random.value <= level.LevelStyle.decoHangoutProbabilitySide)
						{
							FixVerticesForHangouts(level, node, LevelDirection.East, vertices, vertexMap);
							InstantiateHangout(LevelHelper.NodeToVector3(node), level, gameObject, node, LevelDirection.East);
						}
						if (EvaluateNodeAndDirForHangout(level, node, LevelDirection.West) && Random.value <= level.LevelStyle.decoHangoutProbabilitySide)
						{
							FixVerticesForHangouts(level, node, LevelDirection.West, vertices, vertexMap);
							InstantiateHangout(LevelHelper.NodeToVector3(node), level, gameObject, node, LevelDirection.West);
						}
					}
				}
			}
		}
	}

	private bool EvaluateNodeAndDirForHangout(Level level, LevelNode node, LevelDirection dir)
	{
		if (node.Type == LevelNodeType.None || node.Elevation == 0)
		{
			return false;
		}
		if (node.Type == LevelNodeType.House)
		{
			return false;
		}
		if (usedList.Contains(new LevelPoint(node.Row, node.Col)))
		{
			return false;
		}
		LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(node.Row, node.Col), dir, level.Nodes, true);
		if (neighbourAt != null)
		{
			if (neighbourAt.HasLadderAnySizeAnyDirection())
			{
				return false;
			}
			if (neighbourAt.Elevation >= node.Elevation)
			{
				return false;
			}
		}
		LevelPoint item = default(LevelPoint);
		switch (dir)
		{
		case LevelDirection.North:
			item = new LevelPoint(node.Row + 1, node.Col);
			break;
		case LevelDirection.South:
			item = new LevelPoint(node.Row - 1, node.Col);
			break;
		case LevelDirection.East:
			item = new LevelPoint(node.Row, node.Col + 1);
			break;
		case LevelDirection.West:
			item = new LevelPoint(node.Row, node.Col - 1);
			break;
		}
		if (usedList.Contains(item))
		{
			return false;
		}
		return true;
	}

	private void FixVerticesForHangouts(Level level, LevelNode node, LevelDirection dir, Vector3[] vertices, Dictionary<int, Vector3> vertexMap)
	{
		Vector3 vector = LevelHelper.NodeToVector3(node);
		int[] array = null;
		switch (dir)
		{
		case LevelDirection.North:
			array = node.GetTriangles(LevelNodeTriangleFace.Back);
			break;
		case LevelDirection.South:
			array = node.GetTriangles(LevelNodeTriangleFace.Front);
			break;
		case LevelDirection.East:
			array = node.GetTriangles(LevelNodeTriangleFace.Right);
			break;
		case LevelDirection.West:
			array = node.GetTriangles(LevelNodeTriangleFace.Left);
			break;
		}
		foreach (int num in array)
		{
			if (Mathf.Abs(vertices[num].y - vector.y) < 0.25f)
			{
				int key = LevelMeshBuilder.CustomVertexHashCode(vertices[num]);
				vertexMap.Remove(key);
			}
		}
	}

	public void DecorateOutsideLevel(Level level, GameObject levelRoot)
	{
		for (int i = 0; i < level.Nodes.GetLength(1); i++)
		{
			int firstFilledRowAtCol = LevelHelper.GetFirstFilledRowAtCol(level.Nodes, i);
			int lastFilledRowAtCol = LevelHelper.GetLastFilledRowAtCol(level.Nodes, i);
			if (firstFilledRowAtCol > -1 && Random.value < 0.3f)
			{
				Vector3 scale = new Vector3(level.LevelStyle.decoOutsideScale, level.Nodes[firstFilledRowAtCol, i].Elevation + depthLevel, level.LevelStyle.decoOutsideScale);
				scale += new Vector3(0f, Random.Range(0f, 0.4f), 0f);
				InstantiateInsertMeshOutsideLevel(new Vector3(i, -depthLevel, firstFilledRowAtCol - Random.Range(2, 6)), scale, level, levelRoot, false);
			}
			if (lastFilledRowAtCol > -1 && Random.value < 0.3f)
			{
				Vector3 scale2 = new Vector3(level.LevelStyle.decoOutsideScale, level.Nodes[lastFilledRowAtCol, i].Elevation + depthLevel, level.LevelStyle.decoOutsideScale);
				float num = Random.Range(2, 8);
				scale2 += new Vector3(num / 8f, Random.Range(1f, 3f) + num / 8f * 2f, num / 8f);
				InstantiateInsertMeshOutsideLevel(new Vector3(i, -depthLevel, (float)lastFilledRowAtCol + num), scale2, level, levelRoot, true);
			}
		}
		for (int j = 0; j < level.Nodes.GetLength(0); j++)
		{
			int firstFilledColAtRow = LevelHelper.GetFirstFilledColAtRow(level.Nodes, j);
			int lastFilledColAtRow = LevelHelper.GetLastFilledColAtRow(level.Nodes, j);
			Vector3 pos = new Vector3(firstFilledColAtRow - Random.Range(2, 6), -depthLevel, j);
			Vector3 pos2 = new Vector3(lastFilledColAtRow + Random.Range(2, 6), -depthLevel, j);
			if (firstFilledColAtRow > -1 && OutsideLevelEval(level, pos) && Random.value < 0.5f)
			{
				Vector3 scale3 = new Vector3(level.LevelStyle.decoOutsideScale, level.Nodes[j, firstFilledColAtRow].Elevation + depthLevel, level.LevelStyle.decoOutsideScale);
				scale3 += new Vector3(0f, Random.Range(0.5f, 1.2f), 0f);
				InstantiateInsertMeshOutsideLevel(pos, scale3, level, levelRoot, false);
			}
			if (lastFilledColAtRow > -1 && OutsideLevelEval(level, pos2) && Random.value < 0.5f)
			{
				Vector3 scale4 = new Vector3(level.LevelStyle.decoOutsideScale, level.Nodes[j, lastFilledColAtRow].Elevation + depthLevel, level.LevelStyle.decoOutsideScale);
				scale4 += new Vector3(0f, Random.Range(0.5f, 1.2f), 0f);
				InstantiateInsertMeshOutsideLevel(new Vector3(lastFilledColAtRow + Random.Range(2, 6), -depthLevel, j), scale4, level, levelRoot, true);
			}
		}
		for (int k = 0; k < 20; k++)
		{
			float num2 = Random.Range(-15f, (float)level.Nodes.GetLength(1) + 15f);
			float num3 = 0f;
			if (num2 < 0f)
			{
				num3 = LevelHelper.GetLastFilledRowAtCol(level.Nodes, 0);
				if (num3 == -1f)
				{
					num3 = level.Nodes.GetLength(0);
				}
			}
			else if (num2 > (float)(level.Nodes.GetLength(1) - 1))
			{
				num3 = LevelHelper.GetLastFilledRowAtCol(level.Nodes, level.Nodes.GetLength(1) - 1);
				if (num3 == -1f)
				{
					num3 = level.Nodes.GetLength(0);
				}
			}
			else
			{
				num3 = LevelHelper.GetLastFilledRowAtCol(level.Nodes, (int)num2);
				if (num3 == -1f)
				{
					num3 = level.Nodes.GetLength(0);
				}
			}
			float num4 = Random.Range(8, 15);
			int firstFilledColAtRow2 = LevelHelper.GetFirstFilledColAtRow(level.Nodes, (int)num3 + (int)num4);
			int lastFilledColAtRow2 = LevelHelper.GetLastFilledColAtRow(level.Nodes, (int)num3 + (int)num4);
			float num5 = 0f;
			num5 = ((num2 < (float)firstFilledColAtRow2 && firstFilledColAtRow2 > -1) ? ((float)firstFilledColAtRow2 - num2) : ((!(num2 > (float)lastFilledColAtRow2) || lastFilledColAtRow2 <= -1) ? 5f : (num2 - (float)lastFilledColAtRow2)));
			float b = num5;
			Vector3 pos3 = new Vector3(num2, -depthLevel, num3 + num4);
			Vector3 scale5 = new Vector3(Random.Range(Mathf.Min(2f, b), Mathf.Min(5f, b)) * level.LevelStyle.decoOutsideScale, (float)depthLevel + Random.Range(3f, 10f), Random.Range(Mathf.Min(2f, b), Mathf.Min(5f, b)) * level.LevelStyle.decoOutsideScale);
			InstantiateInsertMeshOutsideLevel(pos3, scale5, level, levelRoot, true);
		}
	}

	public void DecorateBackgroundHuge(Level level, LevelStyle style, GameObject levelRoot)
	{
		int numDecoInsertHugeItems = style.numDecoInsertHugeItems;
		for (int i = 0; i < numDecoInsertHugeItems; i++)
		{
			Vector3 pos = new Vector3(Random.Range(-groundOffset, level.Nodes.GetLength(1) + groundOffset), -5f, Random.Range((float)level.Nodes.GetLength(0) + 0.5f + style.decoHugeMinDistToLevelMesh, (float)level.Nodes.GetLength(0) + 0.5f + style.decoHugeMaxDistToLevelmesh));
			Vector3 rot = new Vector3(0f, Random.Range(0, 360), 0f);
			InstantiateHugeInsert(pos, rot, style, levelRoot);
		}
	}

	public void DecorateForeground(Level level, LevelStyle style, GameObject levelRoot)
	{
		for (int i = 0; i < level.Nodes.GetLength(1); i++)
		{
			if (Random.value < style.decoForegroundProbability)
			{
				Vector3 position = new Vector3(i, 0f, (float)LevelHelper.GetFirstFilledRowAtCol(level.Nodes, i) - Random.Range(style.decoForegroundMinDistToLevelMesh, style.decoForegroundMaxDistToLevelmesh));
				Vector3 euler = new Vector3(0f, new Vector3(0f, 0f, 0f).y + Random.Range(-5f, 5f));
				int num = Random.Range(0, style.decorationForeground.Count);
				GameObject gameObject = Object.Instantiate(style.decorationForeground[num], position, Quaternion.Euler(euler));
				DecorationForeground decorationForeground = gameObject.AddComponent<DecorationForeground>();
				decorationForeground.decorationIndex = num;
				gameObject.transform.parent = levelRoot.transform;
				decorationForegrounds.Add(decorationForeground);
			}
		}
	}

	private GameObject InstantiateHugeInsert(Vector3 pos, Vector3 rot, LevelStyle style, GameObject levelRoot)
	{
		GameObject gameObject = Object.Instantiate(style.backDecorationInsertsHuge[Random.Range(0, style.backDecorationInsertsHuge.Count)], pos, Quaternion.Euler(rot));
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
		MeshFilter componentInChildren = gameObject.GetComponentInChildren<MeshFilter>();
		MeshRenderer componentInChildren2 = gameObject.GetComponentInChildren<MeshRenderer>();
		Color[] colors = componentInChildren.mesh.colors;
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i] *= style.sideColor;
		}
		componentInChildren.mesh.colors = colors;
		componentInChildren2.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		componentInChildren2.receiveShadows = false;
		componentInChildren2.shadowCastingMode = ShadowCastingMode.Off;
		return gameObject;
	}

	private bool OutsideLevelEval(Level level, Vector3 pos)
	{
		LevelNode levelNode = LevelHelper.Vector3ToNode(pos, level.Nodes);
		if (levelNode == null)
		{
			return true;
		}
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbour = levelNode.GetNeighbour(level.Nodes, levelDirection);
			if (neighbour != null && neighbour.Elevation > 0)
			{
				return false;
			}
		}
		return true;
	}

	private void BuildSkirt(Level level, GameObject levelRoot)
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		List<Color> list4 = new List<Color>();
		int num = 0;
		for (int i = 0; i < level.Nodes.GetLength(0); i++)
		{
			for (int j = 0; j < level.Nodes.GetLength(1); j++)
			{
				LevelNode levelNode = level.Nodes[i, j];
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(i, j), LevelDirection.North, level.Nodes);
				LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(new LevelPoint(i, j), LevelDirection.South, level.Nodes);
				LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(new LevelPoint(i, j), LevelDirection.East, level.Nodes);
				LevelNode neighbourAt4 = LevelHelper.GetNeighbourAt(new LevelPoint(i, j), LevelDirection.West, level.Nodes);
				if (levelNode.Type != LevelNodeType.None && levelNode.Elevation > 0)
				{
					if (neighbourAt2 == null || neighbourAt2.Elevation == 0)
					{
						list.Add(new Vector3(j, 0f, i));
						list.Add(new Vector3((float)j + 1f, 0f, i));
						list.Add(new Vector3((float)j + 1f, -depthLevel, i));
						list.Add(new Vector3(j, -depthLevel, i));
						list2.Add(Vector3.back);
						list2.Add(Vector3.back);
						list2.Add(Vector3.back);
						list2.Add(Vector3.back);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						LevelMeshBuilder.CollectTriangles(list3, num);
						num += 4;
					}
					if (neighbourAt3 == null || neighbourAt3.Elevation == 0)
					{
						list.Add(new Vector3((float)j + 1f, 0f, i));
						list.Add(new Vector3((float)j + 1f, 0f, (float)i + 1f));
						list.Add(new Vector3((float)j + 1f, -depthLevel, (float)i + 1f));
						list.Add(new Vector3((float)j + 1f, -depthLevel, i));
						list2.Add(Vector3.right);
						list2.Add(Vector3.right);
						list2.Add(Vector3.right);
						list2.Add(Vector3.right);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						LevelMeshBuilder.CollectTriangles(list3, num);
						num += 4;
					}
					if (neighbourAt == null || neighbourAt.Elevation == 0)
					{
						list.Add(new Vector3((float)j + 1f, 0f, (float)i + 1f));
						list.Add(new Vector3(j, 0f, (float)i + 1f));
						list.Add(new Vector3(j, -depthLevel, (float)i + 1f));
						list.Add(new Vector3((float)j + 1f, -depthLevel, (float)i + 1f));
						list2.Add(Vector3.forward);
						list2.Add(Vector3.forward);
						list2.Add(Vector3.forward);
						list2.Add(Vector3.forward);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						LevelMeshBuilder.CollectTriangles(list3, num);
						num += 4;
					}
					if (neighbourAt4 == null || neighbourAt4.Elevation == 0)
					{
						list.Add(new Vector3(j, 0f, (float)i + 1f));
						list.Add(new Vector3(j, 0f, i));
						list.Add(new Vector3(j, -depthLevel, i));
						list.Add(new Vector3(j, -depthLevel, (float)i + 1f));
						list2.Add(Vector3.left);
						list2.Add(Vector3.left);
						list2.Add(Vector3.left);
						list2.Add(Vector3.left);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						list4.Add(level.LevelStyle.sideColor);
						LevelMeshBuilder.CollectTriangles(list3, num);
						num += 4;
					}
				}
			}
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.normals = list2.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.colors = list4.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		GameObject gameObject = new GameObject();
		gameObject.name = "LevelSkirtMesh";
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = false;
		meshCollider.isTrigger = false;
		gameObject.transform.localPosition = new Vector3(-0.5f, 0f, -0.5f);
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
	}

	private GameObject BuildShorelineGround(Level level, LevelStyle style, GameObject levelRoot, int startRow)
	{
		int num = level.Nodes.GetLength(1) + groundOffset + groundOffset;
		int num2 = level.Nodes.GetLength(0) + groundOffset + (groundOffset - startRow);
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Vector3> list3 = new List<Vector3>();
		List<Color> list4 = new List<Color>();
		List<Vector2> list5 = new List<Vector2>();
		for (int i = 0; i < num; i++)
		{
			for (int j = startRow; j < startRow + num2; j++)
			{
				float y = 0.5f + Random.Range(0f, groundHeightVariation);
				if (j == startRow)
				{
					y = -0.5f;
				}
				list.Add(new Vector3((float)i - (float)groundOffset, y, j));
				list3.Add(Vector3.up);
				list4.Add(style.groundColor);
			}
		}
		for (int k = 0; k < num - 1; k++)
		{
			for (int l = 0; l < num2 - 1; l++)
			{
				list2.Add(l + k * num2);
				list2.Add(l + 1 + k * num2);
				list2.Add(l + 1 + (k + 1) * num2);
				list2.Add(l + 1 + (k + 1) * num2);
				list2.Add(l + (k + 1) * num2);
				list2.Add(l + k * num2);
			}
		}
		float num3 = 0.125f;
		for (int m = 0; m < list.Count; m++)
		{
			list5.Add(new Vector2(list[m].x * num3, list[m].z * num3));
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.normals = list3.ToArray();
		mesh.triangles = list2.ToArray();
		mesh.colors = list4.ToArray();
		mesh.uv = list5.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		GameObject gameObject = new GameObject();
		gameObject.name = "ShorelineGroundMesh";
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		meshRenderer.shadowCastingMode = ShadowCastingMode.On;
		meshRenderer.receiveShadows = true;
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = false;
		meshCollider.isTrigger = false;
		return gameObject;
	}

	private GameObject BuildGround(Level level, LevelStyle style, GameObject levelRoot)
	{
		int num = level.Nodes.GetLength(1) + groundOffset + groundOffset;
		int num2 = level.Nodes.GetLength(0) + groundOffset + groundOffset;
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Vector3> list3 = new List<Vector3>();
		List<Color> list4 = new List<Color>();
		List<Vector2> list5 = new List<Vector2>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				list.Add(new Vector3((float)i - (float)groundOffset, Random.Range(0f, groundHeightVariation), (float)j - (float)groundOffset / 3f));
				list3.Add(Vector3.up);
				list4.Add(style.groundColor);
			}
		}
		for (int k = 0; k < num - 1; k++)
		{
			for (int l = 0; l < num2 - 1; l++)
			{
				list2.Add(l + k * num2);
				list2.Add(l + 1 + k * num2);
				list2.Add(l + 1 + (k + 1) * num2);
				list2.Add(l + 1 + (k + 1) * num2);
				list2.Add(l + (k + 1) * num2);
				list2.Add(l + k * num2);
			}
		}
		float num3 = 0.125f;
		for (int m = 0; m < list.Count; m++)
		{
			list5.Add(new Vector2(list[m].x * num3, list[m].z * num3));
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.normals = list3.ToArray();
		mesh.triangles = list2.ToArray();
		mesh.colors = list4.ToArray();
		mesh.uv = list5.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		GameObject gameObject = new GameObject();
		gameObject.name = "GroundMesh";
		gameObject.transform.parent = levelRoot.transform;
		gameObject.SetLayerRecursivly("Occluder");
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		meshRenderer.shadowCastingMode = ShadowCastingMode.On;
		meshRenderer.receiveShadows = true;
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = false;
		meshCollider.isTrigger = false;
		return gameObject;
	}

	private void BuildWater(Level level, LevelStyle style, GameObject levelRoot)
	{
		GameObject gameObject = BuildGround(level, style, levelRoot);
		Renderer component = gameObject.GetComponent<Renderer>();
		component.material = Object.Instantiate(component.material);
		component.material.shader = Shader.Find("Unlit/WaterShader");
		component.material.SetTexture("_MainTex", style.WaterTexture);
		component.material.SetTexture("_WaveTex", style.WaveTexture);
		component.material.SetFloat("_WaveSpeed", style.WaveSpeed);
		component.material.SetFloat("_WaveHeight", style.WaveHeight);
		component.material.SetColor("_ShoreColor", style.ShorelineCol);
		component.material.SetFloat("_ShoreWidth", style.ShorelineWidth);
		component.material.EnableKeyword("Use_Waves");
	}

	private void BuildTileTerrain(Level level, LevelStyle style, GameObject levelRoot)
	{
		for (int i = -groundOffset; i < level.Nodes.GetLength(1) + groundOffset; i += 10)
		{
			for (int j = -(groundOffset / 3); j < level.Nodes.GetLength(0) + groundOffset + groundOffset; j += 10)
			{
				GameObject gameObject = Object.Instantiate(style.groundTiles[Random.Range(0, style.groundTiles.Count)]);
				gameObject.transform.position = new Vector3(i, 0f, j);
				gameObject.transform.parent = levelRoot.transform;
			}
		}
	}

	private void DecorateGround(Level level, LevelStyle style, GameObject levelRoot)
	{
		LevelStyle.e_LevelGroundType e_LevelGroundType = style.LevelGroundType;
		if (GameController.Instance.areaMapMetaLayer != null && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			LevelStyle.e_LevelGroundType groundTypeOverwrite = GameController.Instance.areaMapMetaLayer.GetNodeAt(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol).groundTypeOverwrite;
			if (groundTypeOverwrite != LevelStyle.e_LevelGroundType.None)
			{
				e_LevelGroundType = groundTypeOverwrite;
			}
		}
		switch (e_LevelGroundType)
		{
		case LevelStyle.e_LevelGroundType.Abys:
			BuildSkirt(level, levelRoot);
			break;
		case LevelStyle.e_LevelGroundType.Ground:
			BuildGround(level, style, levelRoot);
			break;
		case LevelStyle.e_LevelGroundType.Shore:
		{
			BuildWater(level, style, levelRoot);
			int num = int.MaxValue;
			int num2 = 0;
			if (style.decorationInserts.Count > 0)
			{
				for (int i = 0; i < level.Nodes.GetLength(1); i++)
				{
					int firstFilledRowAtCol = LevelHelper.GetFirstFilledRowAtCol(level.Nodes, i);
					if (firstFilledRowAtCol < num)
					{
						num = firstFilledRowAtCol;
					}
					if (firstFilledRowAtCol > num2)
					{
						num2 = firstFilledRowAtCol;
					}
				}
			}
			BuildShorelineGround(level, style, levelRoot, (num + num2) / 2 - 1);
			break;
		}
		case LevelStyle.e_LevelGroundType.Water:
			BuildWater(level, style, levelRoot);
			break;
		case LevelStyle.e_LevelGroundType.TileTerrain:
			BuildTileTerrain(level, style, levelRoot);
			break;
		}
	}
}
