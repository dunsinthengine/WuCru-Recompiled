using System.Collections.Generic;
using UnityEngine;

public class LevelQuads
{
	public int numQuadsWidth;

	public int numQuadsHeight;

	public int width;

	public int height;

	public LevelQuad[,] quads;

	public LevelQuads(LevelNode[,] nodes, PathFinding pf, LevelPoint startPoint)
	{
		width = Mathf.RoundToInt((float)nodes.GetLength(1) / 10f);
		height = Mathf.RoundToInt((float)nodes.GetLength(0) / 5f);
		numQuadsWidth = Mathf.CeilToInt((float)nodes.GetLength(1) / (float)width);
		numQuadsHeight = Mathf.CeilToInt((float)nodes.GetLength(0) / (float)height);
		quads = new LevelQuad[numQuadsHeight, numQuadsWidth];
		for (int i = 0; i < numQuadsWidth; i++)
		{
			for (int j = 0; j < numQuadsHeight; j++)
			{
				quads[j, i] = new LevelQuad(j * height, i * width, height, width, nodes, pf, startPoint);
			}
		}
	}

	public LevelQuad GetQuad(int row, int col)
	{
		return quads[Mathf.FloorToInt((float)row / (float)height), Mathf.FloorToInt((float)col / (float)width)];
	}

	public List<LevelQuad> AddLootInfo(List<LevelNode> lootList)
	{
		LevelQuad[,] array = quads;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelQuad levelQuad = array[i, j];
				levelQuad.numLoot = 0;
			}
		}
		List<LevelQuad> list = new List<LevelQuad>();
		foreach (LevelNode loot in lootList)
		{
			LevelQuad quad = GetQuad(loot.Row, loot.Col);
			quad.numLoot++;
			if (!list.Contains(quad))
			{
				list.Add(quad);
			}
		}
		return list;
	}
}
