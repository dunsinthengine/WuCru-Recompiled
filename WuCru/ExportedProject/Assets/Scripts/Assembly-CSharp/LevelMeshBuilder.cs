using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelMeshBuilder
{
	public static string shaderName = "Shader Forge/VertexColor_HalfLambert";

	public static string shaderNameTextured = "Shader Forge/Textured_HalfLambert";

	public static GameObject CreateMesh(Level level, LevelStyle style, int rowStart, int rowEnd, int columnStart, int columnEnd, Dictionary<int, Vector3> vertexMap)
	{
		float num = 0.5f;
		int num2 = 0;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		int maxElevation = LevelHelper.GetMaxElevation(level);
		List<List<int>> list4 = new List<List<int>>();
		for (int i = 0; i < maxElevation; i++)
		{
			list4.Add(new List<int>());
		}
		List<int> list5 = new List<int>();
		List<int> list6 = new List<int>();
		List<Vector2> list7 = new List<Vector2>();
		List<Vector2> list8 = new List<Vector2>();
		int num3 = 0;
		for (float num4 = rowStart; num4 < (float)rowEnd; num4 += num)
		{
			for (float num5 = columnStart; num5 < (float)columnEnd; num5 += num)
			{
				LevelNode node = level.Nodes[Mathf.Min(Mathf.FloorToInt(num4), rowEnd - 1), Mathf.Min(Mathf.FloorToInt(num5), columnEnd - 1)];
				float num6 = node.Elevation;
				if (num6 == 0f)
				{
					num6 = 0.1f;
				}
				int num7 = 0;
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(Mathf.FloorToInt(num4), Mathf.FloorToInt(num5)), LevelDirection.North, level.Nodes);
				LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(new LevelPoint(Mathf.FloorToInt(num4), Mathf.FloorToInt(num5)), LevelDirection.South, level.Nodes);
				LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(new LevelPoint(Mathf.FloorToInt(num4), Mathf.FloorToInt(num5)), LevelDirection.East, level.Nodes);
				LevelNode neighbourAt4 = LevelHelper.GetNeighbourAt(new LevelPoint(Mathf.FloorToInt(num4), Mathf.FloorToInt(num5)), LevelDirection.West, level.Nodes);
				float num8 = 0f;
				float num9 = 0f;
				if (num4 % style.uvMapSize > 0f)
				{
					num8 = num4 % style.uvMapSize;
				}
				if (num5 % style.uvMapSize > 0f)
				{
					num9 = num5 % style.uvMapSize;
				}
				if (node.Type == LevelNodeType.None)
				{
					continue;
				}
				if (((neighbourAt2 != null && neighbourAt2.Elevation < node.Elevation) || num4 == 0f) && Mathf.FloorToInt(num4) - Mathf.FloorToInt(num4 + num) == 0)
				{
					float num10 = num2;
					if (neighbourAt2 != null && neighbourAt2.Elevation > 0)
					{
						num10 = neighbourAt2.Elevation;
					}
					for (float num11 = num10; num11 < num6; num11 += num)
					{
						list.Add(new Vector3(num5, num11 + num, num4));
						list.Add(new Vector3(num5 + num, num11 + num, num4));
						list.Add(new Vector3(num5 + num, num11, num4));
						list.Add(new Vector3(num5, num11, num4));
						list2.Add(Vector3.back);
						list2.Add(Vector3.back);
						list2.Add(Vector3.back);
						list2.Add(Vector3.back);
						list7.Add(new Vector2(num9 / style.uvMapSize, (num11 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num9) / style.uvMapSize, (num11 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num9) / style.uvMapSize, num11 / style.uvMapSize));
						list7.Add(new Vector2(num9 / style.uvMapSize, num11 / style.uvMapSize));
						list8.Add(new Vector2(num9 / style.uvMapSize, (num11 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num9) / style.uvMapSize, (num11 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num9) / style.uvMapSize, num11 / style.uvMapSize));
						list8.Add(new Vector2(num9 / style.uvMapSize, num11 / style.uvMapSize));
						CollectTriangles(list3, num3);
						SetLevelNodeTriangles(ref node, num3, LevelNodeTriangleFace.Front);
						CollectTriangles(list6, num3);
						num3 += 4;
						num7++;
					}
				}
				if (((neighbourAt3 != null && neighbourAt3.Elevation < node.Elevation) || num5 >= (float)columnEnd - 1.5f) && Mathf.FloorToInt(num5) - Mathf.FloorToInt(num5 + num) != 0)
				{
					float num12 = num2;
					if (neighbourAt3 != null && neighbourAt3.Elevation > 0)
					{
						num12 = neighbourAt3.Elevation;
					}
					for (float num13 = num12; num13 < num6; num13 += num)
					{
						list.Add(new Vector3(num5 + num, num13 + num, num4));
						list.Add(new Vector3(num5 + num, num13 + num, num4 + num));
						list.Add(new Vector3(num5 + num, num13, num4 + num));
						list.Add(new Vector3(num5 + num, num13, num4));
						list2.Add(Vector3.right);
						list2.Add(Vector3.right);
						list2.Add(Vector3.right);
						list2.Add(Vector3.right);
						list7.Add(new Vector2(num8 / style.uvMapSize, (num13 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num8) / style.uvMapSize, (num13 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num8) / style.uvMapSize, num13 / style.uvMapSize));
						list7.Add(new Vector2(num8 / style.uvMapSize, num13 / style.uvMapSize));
						list8.Add(new Vector2(num8 / style.uvMapSize, (num13 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num8) / style.uvMapSize, (num13 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num8) / style.uvMapSize, num13 / style.uvMapSize));
						list8.Add(new Vector2(num8 / style.uvMapSize, num13 / style.uvMapSize));
						CollectTriangles(list3, num3);
						SetLevelNodeTriangles(ref node, num3, LevelNodeTriangleFace.Right);
						CollectTriangles(list6, num3);
						num3 += 4;
						num7++;
					}
				}
				if (((neighbourAt != null && neighbourAt.Elevation < node.Elevation) || num4 >= (float)rowEnd - 1.5f) && Mathf.FloorToInt(num4) - Mathf.FloorToInt(num4 + num) != 0)
				{
					float num14 = num2;
					if (neighbourAt != null && neighbourAt.Elevation > 0)
					{
						num14 = neighbourAt.Elevation;
					}
					for (float num15 = num14; num15 < num6; num15 += num)
					{
						list.Add(new Vector3(num5 + num, num15 + num, num4 + num));
						list.Add(new Vector3(num5, num15 + num, num4 + num));
						list.Add(new Vector3(num5, num15, num4 + num));
						list.Add(new Vector3(num5 + num, num15, num4 + num));
						list2.Add(Vector3.forward);
						list2.Add(Vector3.forward);
						list2.Add(Vector3.forward);
						list2.Add(Vector3.forward);
						list7.Add(new Vector2(num9 / style.uvMapSize, (num15 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num9) / style.uvMapSize, (num15 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num9) / style.uvMapSize, num15 / style.uvMapSize));
						list7.Add(new Vector2(num9 / style.uvMapSize, num15 / style.uvMapSize));
						list8.Add(new Vector2(num9 / style.uvMapSize, (num15 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num9) / style.uvMapSize, (num15 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num9) / style.uvMapSize, num15 / style.uvMapSize));
						list8.Add(new Vector2(num9 / style.uvMapSize, num15 / style.uvMapSize));
						CollectTriangles(list3, num3);
						SetLevelNodeTriangles(ref node, num3, LevelNodeTriangleFace.Back);
						CollectTriangles(list6, num3);
						num3 += 4;
						num7++;
					}
				}
				if (((neighbourAt4 != null && neighbourAt4.Elevation < node.Elevation) || num5 == 0f) && Mathf.FloorToInt(num5) - Mathf.FloorToInt(num5 + num) == 0)
				{
					float num16 = num2;
					if (neighbourAt4 != null && neighbourAt4.Elevation > 0)
					{
						num16 = neighbourAt4.Elevation;
					}
					for (float num17 = num16; num17 < num6; num17 += num)
					{
						list.Add(new Vector3(num5, num17 + num, num4 + num));
						list.Add(new Vector3(num5, num17 + num, num4));
						list.Add(new Vector3(num5, num17, num4));
						list.Add(new Vector3(num5, num17, num4 + num));
						list2.Add(Vector3.left);
						list2.Add(Vector3.left);
						list2.Add(Vector3.left);
						list2.Add(Vector3.left);
						list7.Add(new Vector2(num8 / style.uvMapSize, (num17 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num8) / style.uvMapSize, (num17 + num) / style.uvMapSize));
						list7.Add(new Vector2((num + num8) / style.uvMapSize, num17 / style.uvMapSize));
						list7.Add(new Vector2(num8 / style.uvMapSize, num17 / style.uvMapSize));
						list8.Add(new Vector2(num8 / style.uvMapSize, (num17 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num8) / style.uvMapSize, (num17 + num) / style.uvMapSize));
						list8.Add(new Vector2((num + num8) / style.uvMapSize, num17 / style.uvMapSize));
						list8.Add(new Vector2(num8 / style.uvMapSize, num17 / style.uvMapSize));
						CollectTriangles(list3, num3);
						SetLevelNodeTriangles(ref node, num3, LevelNodeTriangleFace.Left);
						CollectTriangles(list6, num3);
						num3 += 4;
						num7++;
					}
				}
				list.Add(new Vector3(num5, num6, num4 + num));
				list.Add(new Vector3(num5 + num, num6, num4 + num));
				list.Add(new Vector3(num5 + num, num6, num4));
				list.Add(new Vector3(num5, num6, num4));
				list2.Add(Vector3.up);
				list2.Add(Vector3.up);
				list2.Add(Vector3.up);
				list2.Add(Vector3.up);
				list7.Add(new Vector2(num9 / style.uvMapSize, (num + num8) / style.uvMapSize));
				list7.Add(new Vector2((num + num9) / style.uvMapSize, (num + num8) / style.uvMapSize));
				list7.Add(new Vector2((num + num9) / style.uvMapSize, num8 / style.uvMapSize));
				list7.Add(new Vector2(num9 / style.uvMapSize, num8 / style.uvMapSize));
				list8.Add(new Vector2(num9 / style.uvMapSize, (num + num8) / style.uvMapSize));
				list8.Add(new Vector2((num + num9) / style.uvMapSize, (num + num8) / style.uvMapSize));
				list8.Add(new Vector2((num + num9) / style.uvMapSize, num8 / style.uvMapSize));
				list8.Add(new Vector2(num9 / style.uvMapSize, num8 / style.uvMapSize));
				CollectTriangles(list3, num3);
				SetLevelNodeTriangles(ref node, num3, LevelNodeTriangleFace.Top);
				if (level.ChunkDB.GetChunk(node.ChunkId) is HouseChunk)
				{
					CollectTriangles(list5, num3);
				}
				else
				{
					CollectTriangles(list4[(int)num6 - 1], num3);
				}
				num3 += 4;
				num7++;
			}
		}
		CreateVariance(list, list2, level, style, VarianceAlgorithm.Random, vertexMap);
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.normals = list2.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.uv = list7.ToArray();
		mesh.uv2 = list8.ToArray();
		List<Color> list9 = new List<Color>(list.Count);
		for (int j = 0; j < list.Count; j++)
		{
			list9.Add(new Color(1f, 1f, 1f, 1f));
		}
		int num18 = 1;
		foreach (List<int> item in list4)
		{
			foreach (int item2 in item)
			{
				list9[item2] = GetElevationFloorColor(style, num18);
			}
			num18++;
		}
		foreach (int item3 in list6)
		{
			list9[item3] = style.sideColor;
		}
		foreach (int item4 in list5)
		{
			list9[item4] = style.houseFloorColor + new Color(Random.Range(-0.1f, 0f), Random.Range(-0.1f, 0f), Random.Range(-0.1f, 0f));
		}
		mesh.colors = list9.ToArray();
		mesh.RecalculateBounds();
		GameObject gameObject = new GameObject();
		gameObject.name = "LevelMesh " + rowStart + "-" + rowEnd + ", " + columnStart + "-" + columnEnd;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find(shaderName));
		meshRenderer.material.renderQueue = 1990;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		if (level.LevelStyle.seededPrefabs != null)
		{
			int count = level.LevelStyle.seededPrefabs.Count;
			for (int k = 0; k < count; k++)
			{
				if (level.LevelStyle.seededPrefabs[k].seed == level.Seed && level.LevelStyle.seededPrefabs[k].hideBaseMesh)
				{
					Debug.Log("Hiding mesh in level: " + level.Seed);
					meshRenderer.enabled = false;
				}
			}
		}
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = false;
		meshCollider.isTrigger = false;
		gameObject.tag = "LevelMesh";
		gameObject.isStatic = true;
		return gameObject;
	}

	public static void CollectTriangles(List<int> triangles, int triOffset)
	{
		triangles.Add(triOffset);
		triangles.Add(triOffset + 1);
		triangles.Add(triOffset + 2);
		triangles.Add(triOffset + 2);
		triangles.Add(triOffset + 3);
		triangles.Add(triOffset);
	}

	private static void SetLevelNodeTriangles(ref LevelNode node, int triOffset, LevelNodeTriangleFace face)
	{
		node.SetTriangles(face, new int[6]
		{
			triOffset,
			triOffset + 1,
			triOffset + 2,
			triOffset + 2,
			triOffset + 3,
			triOffset
		});
	}

	private static void CreateVertexLightAndColor(List<Color> colors, List<Vector3> vertices, List<Vector3> normals, LevelStyle inputData, Level level)
	{
		float[] array = new float[vertices.Count];
		LightSource[] lightSources = level.LightSources;
		foreach (LightSource lightSource in lightSources)
		{
			Vector3 vector = lightSource.transform.position + new Vector3(0.5f, 0f, 0.5f);
			int num = 0;
			foreach (Vector3 vertex in vertices)
			{
				Vector3 normalized = (vertex - vector).normalized;
				if (Vector3.Dot(normals[num], normalized) < 0f)
				{
					float a = Vector3.Distance(vertex, vector) + 0.05f;
					float num2 = (lightSource.range - Mathf.Min(a, lightSource.range)) / lightSource.range * lightSource.intensity;
					if (num2 > array[num])
					{
						array[num] = num2;
					}
				}
				num++;
			}
		}
		for (int j = 0; j < vertices.Count; j++)
		{
			Color color = new Color(array[j], array[j], array[j]);
			if (color.grayscale < inputData.ambientLight.grayscale)
			{
				color = inputData.ambientLight;
			}
			colors[j] *= color;
		}
	}

	private static List<Color> CreateGradients(List<Color> colors, List<Vector3> vertices, List<Vector3> normals, LevelStyle style, Level level, int maxElevation)
	{
		float num = (float)style.maxElevation - (float)style.minElevation;
		LevelNode[,] nodes = level.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = nodes[i, j];
				List<int> list = new List<int>();
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.South, level.Nodes, true);
				if (neighbourAt == null || neighbourAt.Elevation < levelNode.Elevation)
				{
					list.AddRange(levelNode.GetTriangles(LevelNodeTriangleFace.Front));
				}
				LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.East, level.Nodes, true);
				if (neighbourAt2 == null || neighbourAt2.Elevation < levelNode.Elevation)
				{
					list.AddRange(levelNode.GetTriangles(LevelNodeTriangleFace.Right));
				}
				LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(new LevelPoint(levelNode.Row, levelNode.Col), LevelDirection.West, level.Nodes, true);
				if (neighbourAt3 == null || neighbourAt3.Elevation < levelNode.Elevation)
				{
					list.AddRange(levelNode.GetTriangles(LevelNodeTriangleFace.Left));
				}
				HashSet<int> hashSet = new HashSet<int>(list);
				foreach (int item in hashSet)
				{
					Color color = style.heightGradient.Evaluate(vertices[item].y / num);
					colors[item] *= color;
				}
			}
		}
		return colors;
	}

	private static List<Color> CreateFloorNoiseBasedOnHeight(List<Color> colors, List<Vector3> vertices, List<Vector3> normals, LevelStyle style, Level level, int rowStart, int rowEnd, int columnStart, int columnEnd, Dictionary<int, Vector3> vertexMap)
	{
		for (int i = rowStart; i < rowEnd; i++)
		{
			for (int j = columnStart; j < columnEnd; j++)
			{
				LevelNode levelNode = level.Nodes[i, j];
				if (levelNode.Elevation <= 0)
				{
					continue;
				}
				int[] triangles = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
				foreach (int index in triangles)
				{
					int key = CustomVertexHashCode(vertices[index]);
					float y = vertices[index].y;
					if (vertexMap.ContainsKey(key))
					{
						y = vertexMap[key].y;
					}
					colors[index] *= 1f + (y - Mathf.Round(y)) * style.floorHeightNoiseFactor;
				}
			}
		}
		return colors;
	}

	public static void CreateFloorNoiseBasedOnHeight(Vector3[] vertices, Color[] colors, Vector3[] normals, LevelStyle style)
	{
		for (int i = 0; i < vertices.Length; i++)
		{
			if (Vector3.Dot(normals[i], Vector3.up) >= 0.99f)
			{
				float y = vertices[i].y;
				colors[i] *= 1f + (y - Mathf.Round(y)) * style.floorHeightNoiseFactor;
			}
		}
	}

	private static bool CustomVertexEqual(Vector3 a, Vector3 b)
	{
		if (Mathf.Abs(a.x - b.x + (a.y - b.y) + (a.z - b.z)) < 0.01f)
		{
			return true;
		}
		return false;
	}

	public static int CustomVertexHashCode(Vector3 vertex)
	{
		return Mathf.RoundToInt(vertex.x * 2f) + Mathf.RoundToInt(vertex.z * 2f) * 2 * 50 + Mathf.RoundToInt(vertex.y * 2f) * 2 * 50 * 2 * 50;
	}

	public static void BuildVertexMap(List<Vector3> vertices, Dictionary<int, List<int>> vertexMap)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			int key = CustomVertexHashCode(vertices[i]);
			if (!vertexMap.ContainsKey(key))
			{
				vertexMap.Add(key, new List<int>());
			}
			vertexMap[key].Add(i);
		}
	}

	private static void CreateVariance(List<Vector3> vertices, List<Vector3> normals, Level level, LevelStyle inputData, VarianceAlgorithm algorithm, Dictionary<int, Vector3> vertexMap)
	{
		Dictionary<int, List<int>> vertexMap2 = new Dictionary<int, List<int>>();
		BuildVertexMap(vertices, vertexMap2);
		HashSet<int> hashSet = new HashSet<int>();
		foreach (HouseChunk chunk in level.ChunkDB.GetChunks<HouseChunk>())
		{
			foreach (LevelNode node in chunk.NodeList)
			{
				int[] triangles = node.GetTriangles(LevelNodeTriangleFace.Top);
				foreach (int item in triangles)
				{
					hashSet.Add(item);
				}
				int[] triangles2 = node.GetTriangles(LevelNodeTriangleFace.Front);
				foreach (int item2 in triangles2)
				{
					hashSet.Add(item2);
				}
				int[] triangles3 = node.GetTriangles(LevelNodeTriangleFace.Right);
				foreach (int item3 in triangles3)
				{
					hashSet.Add(item3);
				}
				int[] triangles4 = node.GetTriangles(LevelNodeTriangleFace.Back);
				foreach (int item4 in triangles4)
				{
					hashSet.Add(item4);
				}
				int[] triangles5 = node.GetTriangles(LevelNodeTriangleFace.Left);
				foreach (int item5 in triangles5)
				{
					hashSet.Add(item5);
				}
			}
		}
		switch (algorithm)
		{
		case VarianceAlgorithm.Random:
		{
			for (int n = 0; n < vertices.Count; n++)
			{
				int key = CustomVertexHashCode(vertices[n]);
				if (hashSet.Contains(n))
				{
					continue;
				}
				if (Vector3.Dot(normals[n], Vector3.up) > 0f)
				{
					Vector3 vector = new Vector3(0f, Random.Range(0f - inputData.floorVariance, inputData.floorVariance), 0f);
					if (!vertexMap.ContainsKey(key))
					{
						vertexMap[key] = vertices[n];
					}
					vertexMap[key] += vector;
				}
				else if (Vector3.Dot(normals[n], Vector3.back) > 0f)
				{
					Vector3 vector2 = new Vector3(0f, 0f, Random.Range(0f - inputData.sideVariance, inputData.sideVariance));
					if (!vertexMap.ContainsKey(key))
					{
						vertexMap[key] = vertices[n];
					}
					vertexMap[key] += vector2;
				}
				else if (Vector3.Dot(normals[n], Vector3.right) > 0f)
				{
					Vector3 vector3 = new Vector3(Random.Range(0f - inputData.sideVariance, inputData.sideVariance), 0f, 0f);
					if (!vertexMap.ContainsKey(key))
					{
						vertexMap[key] = vertices[n];
					}
					vertexMap[key] += vector3;
				}
				else if (Vector3.Dot(normals[n], Vector3.forward) > 0f)
				{
					Vector3 vector4 = new Vector3(0f, 0f, Random.Range(0f - inputData.sideVariance, inputData.sideVariance));
					if (!vertexMap.ContainsKey(key))
					{
						vertexMap[key] = vertices[n];
					}
					vertexMap[key] += vector4;
				}
				else if (Vector3.Dot(normals[n], Vector3.left) > 0f)
				{
					Vector3 vector5 = new Vector3(Random.Range(0f - inputData.sideVariance, inputData.sideVariance), 0f, 0f);
					if (!vertexMap.ContainsKey(key))
					{
						vertexMap[key] = vertices[n];
					}
					vertexMap[key] += vector5;
				}
			}
			break;
		}
		}
	}

	public static Vector3[] GetFloorEdgeVertices(Level level, List<Vector3> vertices)
	{
		List<Vector3> list = new List<Vector3>();
		LevelNode[] edgeNodes = GetEdgeNodes(level);
		LevelNode[] array = edgeNodes;
		foreach (LevelNode levelNode in array)
		{
			int[] triangles = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
			foreach (int index in triangles)
			{
				if (!list.Contains(vertices[index]))
				{
					list.Add(vertices[index]);
				}
			}
		}
		return list.ToArray();
	}

	public static Vector3[] GetSideEdgeVertices(Level level, List<Vector3> vertices)
	{
		List<Vector3> list = new List<Vector3>();
		LevelNode[] edgeNodes = GetEdgeNodes(level);
		LevelNode[] array = edgeNodes;
		foreach (LevelNode levelNode in array)
		{
			for (LevelNodeTriangleFace levelNodeTriangleFace = LevelNodeTriangleFace.Front; levelNodeTriangleFace <= LevelNodeTriangleFace.Left; levelNodeTriangleFace++)
			{
				int[] triangles = levelNode.GetTriangles(levelNodeTriangleFace);
				foreach (int index in triangles)
				{
					if (!list.Contains(vertices[index]))
					{
						list.Add(vertices[index]);
					}
				}
			}
		}
		return list.ToArray();
	}

	public static Vector3[] GetEdgeVertices(Level level, List<Vector3> vertices)
	{
		List<Vector3> list = new List<Vector3>();
		LevelNode[] edgeNodes = GetEdgeNodes(level);
		LevelNode[] array = edgeNodes;
		foreach (LevelNode levelNode in array)
		{
			for (LevelNodeTriangleFace levelNodeTriangleFace = LevelNodeTriangleFace.Front; levelNodeTriangleFace <= LevelNodeTriangleFace.Bottom; levelNodeTriangleFace++)
			{
				int[] triangles = levelNode.GetTriangles(levelNodeTriangleFace);
				foreach (int index in triangles)
				{
					if (!list.Contains(vertices[index]))
					{
						list.Add(vertices[index]);
					}
				}
			}
		}
		return list.ToArray();
	}

	public static LevelNode[] GetEdgeNodes(Level level)
	{
		List<LevelNode> list = new List<LevelNode>();
		for (int i = 0; i < level.Nodes.GetLength(0); i++)
		{
			for (int j = 0; j < level.Nodes.GetLength(1); j++)
			{
				LevelNode[] array = LevelHelper.GetNeighbours(new LevelPoint(i, j), level.Nodes, true).ToArray();
				if (array.Length < 4)
				{
					list.Add(level.Nodes[i, j]);
					continue;
				}
				LevelNode[] array2 = array;
				foreach (LevelNode levelNode in array2)
				{
					if (levelNode.Elevation < level.Nodes[i, j].Elevation)
					{
						list.Add(level.Nodes[i, j]);
						break;
					}
				}
			}
		}
		return list.ToArray();
	}

	public static float[] GetAlphaValues(Level level, Vector3 vertices)
	{
		List<float> list = new List<float>();
		for (int i = 0; i < level.Nodes.GetLength(0); i++)
		{
			for (int j = 0; j < level.Nodes.GetLength(1); j++)
			{
				LevelNode[] array = LevelHelper.GetNeighbours(new LevelPoint(i, j), level.Nodes, true).ToArray();
				bool flag = false;
				if (array.Length < 4)
				{
					flag = true;
				}
				else
				{
					LevelNode[] array2 = array;
					foreach (LevelNode levelNode in array2)
					{
						if (levelNode.Elevation < level.Nodes[i, j].Elevation)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					float item = 0.5f;
					list.Add(item);
				}
				else
				{
					list.Add(1f);
				}
			}
		}
		return list.ToArray();
	}

	private static List<Color> CreateAmbientOcclusion(List<Color> colors, List<Vector3> vertices, List<Vector3> normals, LevelStyle inputData, Level level)
	{
		Color value = new Color(0f, 0f, 0f, 0.1f);
		LevelNode[,] nodes = level.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = nodes[i, j];
				if (levelNode.Type == LevelNodeType.None || levelNode.Elevation <= 0)
				{
					continue;
				}
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(levelNode.Location, LevelDirection.North, level.Nodes, true);
				LevelNode neighbourAt2 = LevelHelper.GetNeighbourAt(levelNode.Location, LevelDirection.South, level.Nodes, true);
				LevelNode neighbourAt3 = LevelHelper.GetNeighbourAt(levelNode.Location, LevelDirection.East, level.Nodes, true);
				LevelNode neighbourAt4 = LevelHelper.GetNeighbourAt(levelNode.Location, LevelDirection.West, level.Nodes, true);
				LevelNode levelNode2 = null;
				LevelNode levelNode3 = null;
				if (neighbourAt != null)
				{
					levelNode2 = LevelHelper.GetNeighbourAt(neighbourAt.Location, LevelDirection.East, level.Nodes, true);
					levelNode3 = LevelHelper.GetNeighbourAt(neighbourAt.Location, LevelDirection.West, level.Nodes, true);
				}
				LevelNode levelNode4 = null;
				LevelNode levelNode5 = null;
				if (neighbourAt2 != null)
				{
					levelNode4 = LevelHelper.GetNeighbourAt(neighbourAt2.Location, LevelDirection.East, level.Nodes, true);
					levelNode5 = LevelHelper.GetNeighbourAt(neighbourAt2.Location, LevelDirection.West, level.Nodes, true);
				}
				if (neighbourAt != null)
				{
					if (neighbourAt.Elevation > levelNode.Elevation)
					{
						int[] triangles = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
						int[] array = triangles;
						foreach (int index in array)
						{
							if (AlmostEqual(vertices[index].z, levelNode.Location.row + 1))
							{
								colors[index] = new Color(colors[index].r, colors[index].g, colors[index].b, value.a);
							}
						}
						if (neighbourAt3 != null && neighbourAt3.Elevation == levelNode.Elevation)
						{
							triangles = neighbourAt3.GetTriangles(LevelNodeTriangleFace.Top);
							int[] array2 = triangles;
							foreach (int index2 in array2)
							{
								if (AlmostEqual(vertices[index2].z, neighbourAt3.Location.row + 1) && AlmostEqual(vertices[index2].x, neighbourAt3.Location.col))
								{
									colors[index2] = new Color(colors[index2].r, colors[index2].g, colors[index2].b, value.a);
								}
							}
						}
						if (neighbourAt4 != null && neighbourAt4.Elevation == levelNode.Elevation)
						{
							triangles = neighbourAt4.GetTriangles(LevelNodeTriangleFace.Top);
							int[] array3 = triangles;
							foreach (int index3 in array3)
							{
								if (AlmostEqual(vertices[index3].z, neighbourAt4.Location.row + 1) && AlmostEqual(vertices[index3].x, neighbourAt4.Location.col + 1))
								{
									colors[index3] = new Color(colors[index3].r, colors[index3].g, colors[index3].b, value.a);
								}
							}
						}
					}
					if (neighbourAt.Elevation < levelNode.Elevation)
					{
						int[] triangles2 = levelNode.GetTriangles(LevelNodeTriangleFace.Back);
						int[] array4 = triangles2;
						foreach (int index4 in array4)
						{
							if (AlmostEqual(vertices[index4].y, neighbourAt.Elevation))
							{
								colors[index4] = new Color(colors[index4].r, colors[index4].g, colors[index4].b, value.a);
							}
						}
					}
				}
				if (neighbourAt2 != null)
				{
					if (neighbourAt2.Elevation > levelNode.Elevation)
					{
						int[] triangles3 = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
						int[] array5 = triangles3;
						foreach (int index5 in array5)
						{
							if (AlmostEqual(vertices[index5].z, levelNode.Location.row))
							{
								colors[index5] = new Color(colors[index5].r, colors[index5].g, colors[index5].b, value.a);
							}
						}
						if (neighbourAt3 != null && neighbourAt3.Elevation == levelNode.Elevation)
						{
							triangles3 = neighbourAt3.GetTriangles(LevelNodeTriangleFace.Top);
							int[] array6 = triangles3;
							foreach (int index6 in array6)
							{
								if (AlmostEqual(vertices[index6].z, neighbourAt3.Location.row) && AlmostEqual(vertices[index6].x, neighbourAt3.Location.col))
								{
									colors[index6] = new Color(colors[index6].r, colors[index6].g, colors[index6].b, value.a);
								}
							}
						}
						if (neighbourAt4 != null && neighbourAt4.Elevation == levelNode.Elevation)
						{
							triangles3 = neighbourAt4.GetTriangles(LevelNodeTriangleFace.Top);
							int[] array7 = triangles3;
							foreach (int index7 in array7)
							{
								if (AlmostEqual(vertices[index7].z, neighbourAt4.Location.row) && AlmostEqual(vertices[index7].x, neighbourAt4.Location.col + 1))
								{
									colors[index7] = new Color(colors[index7].r, colors[index7].g, colors[index7].b, value.a);
								}
							}
						}
					}
					if (neighbourAt2.Elevation < levelNode.Elevation)
					{
						int[] triangles4 = levelNode.GetTriangles(LevelNodeTriangleFace.Front);
						int[] array8 = triangles4;
						foreach (int index8 in array8)
						{
							if (AlmostEqual(vertices[index8].y, neighbourAt2.Elevation))
							{
								colors[index8] = new Color(colors[index8].r, colors[index8].g, colors[index8].b, value.a);
							}
						}
					}
				}
				if (neighbourAt3 != null)
				{
					if (neighbourAt3.Elevation > levelNode.Elevation)
					{
						int[] triangles5 = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
						int[] array9 = triangles5;
						foreach (int index9 in array9)
						{
							if (AlmostEqual(vertices[index9].x, levelNode.Location.col + 1))
							{
								colors[index9] = new Color(colors[index9].r, colors[index9].g, colors[index9].b, value.a);
							}
						}
					}
					if (neighbourAt3.Elevation < levelNode.Elevation)
					{
						int[] triangles6 = levelNode.GetTriangles(LevelNodeTriangleFace.Right);
						int[] array10 = triangles6;
						foreach (int index10 in array10)
						{
							if (AlmostEqual(vertices[index10].y, neighbourAt3.Elevation))
							{
								colors[index10] = new Color(colors[index10].r, colors[index10].g, colors[index10].b, value.a);
							}
						}
					}
				}
				if (neighbourAt4 == null)
				{
					continue;
				}
				if (neighbourAt4.Elevation > levelNode.Elevation)
				{
					int[] triangles7 = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
					int[] array11 = triangles7;
					foreach (int index11 in array11)
					{
						if (AlmostEqual(vertices[index11].x, levelNode.Location.col))
						{
							colors[index11] = value;
						}
					}
				}
				if (neighbourAt4.Elevation >= levelNode.Elevation)
				{
					continue;
				}
				int[] triangles8 = levelNode.GetTriangles(LevelNodeTriangleFace.Left);
				int[] array12 = triangles8;
				foreach (int index12 in array12)
				{
					if (AlmostEqual(vertices[index12].y, neighbourAt4.Elevation))
					{
						colors[index12] = value;
					}
				}
			}
		}
		return colors;
	}

	private static bool AlmostEqual(float value, float compareValue)
	{
		if (Mathf.Abs(compareValue - value) < 0.25f)
		{
			return true;
		}
		return false;
	}

	public static Color GetElevationFloorColor(LevelStyle style, int elevation)
	{
		if (style.floorColors.Count == 0)
		{
			return Color.white;
		}
		if (elevation - 1 >= style.floorColors.Count)
		{
			return style.floorColors[style.floorColors.Count - 1];
		}
		return style.floorColors[elevation - 1];
	}

	public static void DebugColorIslands(Level level, List<Color> colors)
	{
		foreach (LevelIsland levelIsland in level.ChunkDB.LevelIslands)
		{
			LevelNode[] islandNodes = levelIsland.islandNodes;
			foreach (LevelNode levelNode in islandNodes)
			{
				int[] triangles = levelNode.GetTriangles(LevelNodeTriangleFace.Top);
				foreach (int index in triangles)
				{
					colors[index] = Color.black;
				}
			}
		}
	}
}
