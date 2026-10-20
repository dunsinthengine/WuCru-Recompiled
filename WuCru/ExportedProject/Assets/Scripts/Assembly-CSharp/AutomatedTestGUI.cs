using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomatedTestGUI : MonoBehaviour
{
	public LevelStyleDB styleDB;

	public LevelThemeDB themeDB;

	public AreaMapMetaLayer areaMapMetaLayer;

	public Button buttonBegin;

	private Dictionary<string, TestResult> results = new Dictionary<string, TestResult>();

	private void Start()
	{
		buttonBegin.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			buttonBegin.enabled = false;
			StartCoroutine(DoTest());
		});
	}

	private IEnumerator DoTest()
	{
		int numTested = 0;
		for (int r = 0; r < areaMapMetaLayer.rowArray.Length; r++)
		{
			for (int c = 0; c < areaMapMetaLayer.rowArray[0].colArray.Length; c++)
			{
				if (areaMapMetaLayer.IsBlocked(r, c))
				{
					continue;
				}
				bool exception = false;
				int seed = areaMapMetaLayer.baseSeed + areaMapMetaLayer.GetCustomHashCode(r, c);
				AreaMapNode aNode = areaMapMetaLayer.GetNodeAt(r, c);
				int numSegments = themeDB.themeCollections.values[aNode.themeCollectionIndex].themes.Count;
				for (int segment = 0; segment < numSegments; segment++)
				{
					string levelName = seed + styleDB._styles[aNode.styleIndex].styleName.Substring(0, 1) + themeDB.themeCollections.values[aNode.themeCollectionIndex].collectionName.Substring(0, 1) + segment;
					LevelEngine engine = new LevelEngine();
					Level level = null;
					try
					{
						level = engine.GenerateLevel(themeDB.themeCollections.values[aNode.themeCollectionIndex], themeDB.themeCollections.values[aNode.themeCollectionIndex].themes.values[segment], styleDB._styles[aNode.styleIndex], seed, ElementalPower.None, ElementalPower.None);
					}
					catch
					{
						exception = true;
						results.Add(levelName, TestResult.ExceptionDuringGeneration);
					}
					if (!exception)
					{
						LevelNode levelNode = null;
						LevelNode levelNode2 = null;
						LevelNode levelNode3 = null;
						LevelNode levelNode4 = null;
						LevelNode[,] nodes = level.Nodes;
						int length = nodes.GetLength(0);
						int length2 = nodes.GetLength(1);
						for (int i = 0; i < length; i++)
						{
							for (int j = 0; j < length2; j++)
							{
								LevelNode levelNode5 = nodes[i, j];
								if (levelNode5.ContainsObjectType(LevelNodeObjectType.Key))
								{
									levelNode3 = levelNode5;
								}
								if (levelNode5.ContainsObjectType(LevelNodeObjectType.StartFlag))
								{
									levelNode = levelNode5;
								}
								if (levelNode5.ContainsObjectType(LevelNodeObjectType.EndFlag))
								{
									levelNode2 = levelNode5;
								}
								if (levelNode5.ContainsObjectType(LevelNodeObjectType.DoorTrigger))
								{
									levelNode4 = levelNode5;
								}
							}
						}
						if (levelNode == null || levelNode2 == null)
						{
							results.Add(levelName, TestResult.NoStartOrEndFound);
						}
						else if (levelNode3 == null && levelNode4 == null)
						{
							if (!engine.DoesPathExist(new LevelPoint(levelNode.Row, levelNode.Col), new LevelPoint(levelNode2.Row, levelNode2.Col), false))
							{
								results.Add(levelName, TestResult.PathIsBlocked);
							}
						}
						else if (levelNode3 == null && levelNode4 != null)
						{
							if (!engine.DoesPathExist(new LevelPoint(levelNode.Row, levelNode.Col), new LevelPoint(levelNode4.Row, levelNode4.Col), false))
							{
								results.Add(levelName, TestResult.CannotReachTrigger);
							}
							else if (!engine.DoesPathExist(new LevelPoint(levelNode4.Row, levelNode4.Col), new LevelPoint(levelNode2.Row, levelNode2.Col)))
							{
								results.Add(levelName, TestResult.TriggerToEndBlocked);
							}
						}
						else if (levelNode3 != null && levelNode4 == null)
						{
							if (!engine.DoesPathExist(new LevelPoint(levelNode.Row, levelNode.Col), new LevelPoint(levelNode3.Row, levelNode3.Col), false))
							{
								results.Add(levelName, TestResult.CannotReachKey);
							}
							else if (!engine.DoesPathExist(new LevelPoint(levelNode3.Row, levelNode3.Col), new LevelPoint(levelNode2.Row, levelNode2.Col)))
							{
								results.Add(levelName, TestResult.KeyToEndBlocked);
							}
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							if (engine.DoesPathExist(new LevelPoint(levelNode.Row, levelNode.Col), new LevelPoint(levelNode4.Row, levelNode4.Col), false))
							{
								flag = true;
							}
							if (engine.DoesPathExist(new LevelPoint(levelNode.Row, levelNode.Col), new LevelPoint(levelNode3.Row, levelNode3.Col), false))
							{
								flag2 = true;
							}
							if (!flag && !flag2)
							{
								results.Add(levelName, TestResult.CannotReachKeyOrTrigger);
							}
							else if (flag && !flag2)
							{
								if (!engine.DoesPathExist(new LevelPoint(levelNode4.Row, levelNode4.Col), new LevelPoint(levelNode3.Row, levelNode3.Col)))
								{
									results.Add(levelName, TestResult.CannotReachKey);
								}
								else if (!engine.DoesPathExist(new LevelPoint(levelNode3.Row, levelNode3.Col), new LevelPoint(levelNode2.Row, levelNode2.Col)))
								{
									results.Add(levelName, TestResult.KeyToEndBlocked);
								}
							}
							else if (!flag && flag2)
							{
								if (!engine.DoesPathExist(new LevelPoint(levelNode3.Row, levelNode3.Col), new LevelPoint(levelNode4.Row, levelNode4.Col)))
								{
									results.Add(levelName, TestResult.CannotReachTrigger);
								}
								else if (!engine.DoesPathExist(new LevelPoint(levelNode4.Row, levelNode4.Col), new LevelPoint(levelNode2.Row, levelNode2.Col)))
								{
									results.Add(levelName, TestResult.TriggerToEndBlocked);
								}
							}
							else if (!engine.DoesPathExist(new LevelPoint(levelNode3.Row, levelNode3.Col), new LevelPoint(levelNode2.Row, levelNode2.Col)))
							{
								results.Add(levelName, TestResult.KeyToEndBlocked);
							}
							else if (!engine.DoesPathExist(new LevelPoint(levelNode4.Row, levelNode4.Col), new LevelPoint(levelNode2.Row, levelNode2.Col)))
							{
								results.Add(levelName, TestResult.TriggerToEndBlocked);
							}
						}
					}
					numTested++;
					yield return null;
				}
			}
		}
		Debug.Log("TEST IS DONE");
		Debug.Log("Tested " + numTested + " Levels. " + results.Count + " failed!");
		foreach (string key in results.Keys)
		{
			Debug.Log(key + " : " + results[key]);
		}
		yield return 0;
	}
}
