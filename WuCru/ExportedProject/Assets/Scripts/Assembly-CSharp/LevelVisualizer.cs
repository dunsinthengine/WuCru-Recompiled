using System.Collections.Generic;
using UnityEngine;

public class LevelVisualizer : MonoBehaviour
{
	public bool useLevelMeshBuilder;

	public const int LevelSegmentWidth = 5;

	private Level currentLevel;

	public GameObject levelRoot;

	private GameObject levelMeshGO;

	private LevelDecorationBuilder decoBuilder;

	public int environmentIndex = -1;

	public string environmentName = string.Empty;

	public EnvironmentSetting environmentSetting;

	public static bool optimizeLevel = true;

	public static bool decorateLevel = true;

	public GameObject LevelMeshGO
	{
		get
		{
			return levelMeshGO;
		}
	}

	public List<DecorationForeground> decorationForegrounds
	{
		get
		{
			return decoBuilder.decorationForegrounds;
		}
	}

	private void Awake()
	{
		decoBuilder = base.gameObject.AddComponent<LevelDecorationBuilder>();
	}

	public void BuildLevel(Level level, int styleIndex, int environmentSettingsIndex = -1, bool useAI = true)
	{
		LevelStyle levelStyle = EditorStateMachine.Instance.styleDB._styles[styleIndex];
		if (levelStyle == null)
		{
			Debug.Log("Style is null");
		}
		else
		{
			BuildLevel(level, levelStyle, environmentSettingsIndex, useAI);
		}
	}

	public void BuildLevel(Level level, LevelStyle style, int environmentSettingsIndex = -1, bool useAI = true)
	{
		if (levelRoot != null)
		{
			Object.Destroy(levelRoot);
		}
		Debug.Log("----------BUILD LEVEL!------------");
		currentLevel = level;
		levelRoot = new GameObject();
		levelRoot.name = "LevelRoot";
		GameObject gameObject = null;
		LevelNode[,] nodes = level.Nodes;
		bool flag = ((level.LevelTheme.index == level.LevelThemeCollection.themes.keys.Count - 1) ? true : false);
		bool flag2 = false;
		Random.seed = Mathf.RoundToInt(Time.time * 100f);
		if (style.environmentSettings.Count > 0)
		{
			int num = -1;
			if (GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying) && GameController.Instance.areaMapMetaLayer != null)
			{
				num = GameController.Instance.areaMapMetaLayer.GetNodeAt(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol).environmentSettingOverwrite;
			}
			if (environmentSettingsIndex == -1 && num == -1)
			{
				Random.seed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
				environmentSettingsIndex = Random.Range(0, style.environmentSettings.Count);
			}
			else if (num > -1)
			{
				environmentSettingsIndex = num;
			}
			if (environmentSettingsIndex >= style.environmentSettings.Count)
			{
				environmentSettingsIndex = style.environmentSettings.Count - 1;
			}
			GameObject gameObject2 = style.environmentSettings[environmentSettingsIndex];
			environmentSetting = gameObject2.GetComponent<EnvironmentSetting>();
			environmentSetting.Setup(levelRoot, ref environmentName);
			flag2 = environmentSetting.isNight;
		}
		else
		{
			Debug.LogError("Style does not have any EnvironmentSettings - no lighting or colors set!");
		}
		List<Chest> list = new List<Chest>();
		Random.seed = level.Seed;
		for (int i = 0; i < nodes.GetLength(1); i++)
		{
			for (int j = 0; j < nodes.GetLength(0); j++)
			{
				if (nodes[j, i].Type == LevelNodeType.None || nodes[j, i].Elevation <= 0)
				{
					continue;
				}
				Vector3 position = new Vector3(i, nodes[j, i].Elevation, j);
				foreach (LevelNodeObjectTypeAndTag objectType in nodes[j, i].ObjectTypes)
				{
					GameObject gameObject3 = null;
					Chest chest = null;
					switch (objectType.objectType)
					{
					case LevelNodeObjectType.EndFlag:
						gameObject3 = (flag ? Object.Instantiate(style.endLevelObjectPrefab, position, Quaternion.identity) : ((GameObject)Object.Instantiate(Resources.Load("Prefabs/EndFlag"), position, Quaternion.identity)));
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.WoodChest:
						gameObject3 = Object.Instantiate(style.woodChestPrefab, position, Quaternion.Euler(0f, 180f, 0f));
						gameObject3.transform.parent = levelRoot.transform;
						chest = gameObject3.GetComponentInChildren<Chest>();
						if (chest != null)
						{
							list.Add(chest);
						}
						break;
					case LevelNodeObjectType.MetalChest:
						gameObject3 = Object.Instantiate(style.metalChestPrefab, position, Quaternion.Euler(0f, 180f, 0f));
						gameObject3.transform.parent = levelRoot.transform;
						chest = gameObject3.GetComponentInChildren<Chest>();
						if (chest != null)
						{
							list.Add(chest);
						}
						break;
					case LevelNodeObjectType.GoldChest:
						gameObject3 = Object.Instantiate(style.goldChestPrefab, position, Quaternion.Euler(0f, 180f, 0f));
						gameObject3.transform.parent = levelRoot.transform;
						chest = gameObject3.GetComponentInChildren<Chest>();
						if (chest != null)
						{
							list.Add(chest);
						}
						break;
					case LevelNodeObjectType.Bush:
					{
						gameObject3 = Object.Instantiate(style.hidingSpotPrefab, position, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
						gameObject3.transform.parent = levelRoot.transform;
						float num2 = Random.Range(0f, style.propScaleVariactionFactor);
						gameObject3.transform.localScale += new Vector3(num2, num2, num2);
						break;
					}
					case LevelNodeObjectType.LampPost:
						if (flag2)
						{
							gameObject3 = Object.Instantiate(style.lightPrefab, position, Quaternion.identity);
							gameObject3.transform.parent = levelRoot.transform;
						}
						else
						{
							gameObject3 = Object.Instantiate(style.lightPrefabDay, position, Quaternion.identity);
							gameObject3.transform.parent = levelRoot.transform;
						}
						break;
					case LevelNodeObjectType.ScoutEnemy:
					case LevelNodeObjectType.GruntEnemy:
					case LevelNodeObjectType.WarriorEnemy:
					case LevelNodeObjectType.SnakeBossEnemy:
					case LevelNodeObjectType.ColeFriend:
					case LevelNodeObjectType.ZaneFriend:
					case LevelNodeObjectType.KaiFriend:
					case LevelNodeObjectType.JayFriend:
					case LevelNodeObjectType.MasterWuFriend:
					case LevelNodeObjectType.FangpyreBossEnemy:
					case LevelNodeObjectType.VenomariBossEnemy:
					case LevelNodeObjectType.HypnobraiBossEnemy:
					case LevelNodeObjectType.ConstrictaiBossEnemy:
					case LevelNodeObjectType.StoneWarrior:
					case LevelNodeObjectType.NyahFriend:
					case LevelNodeObjectType.LloydFriend:
					case LevelNodeObjectType.ElementalBossEnemy:
						if (useAI)
						{
							string text = null;
							MinifigClass minifigClass = MinifigClass.None;
							switch (objectType.objectType)
							{
							default:
								text = "Prefabs/Characters/AIAgentElementalBossEnemy";
								minifigClass = MinifigClass.Scout;
								break;
							case LevelNodeObjectType.ScoutEnemy:
								text = "Prefabs/Characters/AIAgentScoutEnemy";
								minifigClass = MinifigClass.Scout;
								break;
							case LevelNodeObjectType.GruntEnemy:
								text = "Prefabs/Characters/AIAgentGruntEnemy";
								minifigClass = MinifigClass.Soldier;
								break;
							case LevelNodeObjectType.WarriorEnemy:
								text = "Prefabs/Characters/AIAgentWarriorEnemy";
								minifigClass = MinifigClass.Warrior;
								break;
							case LevelNodeObjectType.SnakeBossEnemy:
								text = "Prefabs/Characters/AIAgentSnakeBossEnemy";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.ColeFriend:
								text = "Prefabs/Characters/AIAgentColeFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.ZaneFriend:
								text = "Prefabs/Characters/AIAgentZaneFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.KaiFriend:
								text = "Prefabs/Characters/AIAgentKaiFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.JayFriend:
								text = "Prefabs/Characters/AIAgentJayFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.NyahFriend:
								text = "Prefabs/Characters/AIAgentNyahFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.LloydFriend:
								text = "Prefabs/Characters/AIAgentLloydFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.MasterWuFriend:
								text = "Prefabs/Characters/AIAgentMasterWuFriend";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.VenomariBossEnemy:
								text = "Prefabs/Characters/AIAgentVenomariBossEnemy";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.HypnobraiBossEnemy:
								text = "Prefabs/Characters/AIAgentHypnobraiBossEnemy";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.FangpyreBossEnemy:
								text = "Prefabs/Characters/AIAgentFangpyreBossEnemy";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.ConstrictaiBossEnemy:
								text = "Prefabs/Characters/AIAgentConstrictaiBossEnemy";
								minifigClass = MinifigClass.None;
								break;
							case LevelNodeObjectType.StoneWarrior:
								text = "Prefabs/Characters/AIAgentStoneWarrior";
								minifigClass = MinifigClass.None;
								break;
							}
							try
							{
								Object obj = Resources.Load(text);
								if (obj == null)
								{
									Debug.LogWarning("Couldn't load: " + text);
								}
								else
								{
									gameObject3 = (GameObject)Object.Instantiate(obj, position, Quaternion.identity);
									if (minifigClass != MinifigClass.None)
									{
										int seed = Random.seed;
										Random.seed = level.Seed;
										int maxPct = (style.totalMinifigTypePercentage > 0) ? style.totalMinifigTypePercentage : 1;
										MinifigType key = style.MinifigTypeForRandomValue(Random.Range(0, maxPct));
										Random.seed = seed;
										int id = -1;
										if (CharacterBuilder.enemies != null && CharacterBuilder.enemies.ContainsKey(key) && CharacterBuilder.enemies[key].ContainsKey(minifigClass))
										{
											id = CharacterBuilder.enemies[key][minifigClass];
										}
										else if (CharacterBuilder.enemies != null)
										{
											foreach (var pair in CharacterBuilder.enemies)
											{
												if (pair.Value.ContainsKey(minifigClass))
												{
													id = pair.Value[minifigClass];
													break;
												}
											}
										}
										if (id == -1 && CharacterBuilder.minifigDefinitions != null && CharacterBuilder.minifigDefinitions.Count > 0)
										{
											id = 1;
										}
										if (id != -1 && gameObject3 != null)
										{
											AIAgent ai = gameObject3.GetComponent<AIAgent>();
											if (ai != null && ai.Character != null)
											{
												CharacterBuilder.ConfigureMinifig(ai.Character, id);
												if (objectType.objectType == LevelNodeObjectType.ElementalBossEnemy)
												{
													ai.Character.elementalPower = level.PrimaryPower;
												}
											}
										}
									}
									gameObject3.transform.parent = levelRoot.transform;
									AIAgent component = gameObject3.GetComponent<AIAgent>();
									if (component != null)
									{
										level.RegisterAgent(component, nodes[j, i]);
										if (level.ChunkDB != null && level.ChunkDB.AIAgents != null && level.ChunkDB.AIAgents.ContainsKey(component.AgentId))
										{
											component.PathMode = level.ChunkDB.AIAgents[component.AgentId].pathMode;
										}
										component.AddObjectTag(objectType.objectTag);
										if (objectType.objectType == LevelNodeObjectType.ElementalBossEnemy && component is ElementalBossEnemy)
										{
											((ElementalBossEnemy)component).OnChosenElement();
										}
									}
								}
							}
							catch (System.Exception ex)
							{
								Debug.LogError("Failed to instantiate/configure AIAgent for " + objectType.objectType + ": " + ex);
							}
						}
						else
						{
							gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Characters/TestEnemy"), position, Quaternion.identity);
							gameObject3.transform.parent = levelRoot.transform;
						}
						break;
					case LevelNodeObjectType.BellTower:
						gameObject3 = Object.Instantiate(style.distractionPrefab, position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.Tree:
						if (style.treePrefabs.Count > 0)
						{
							gameObject3 = Object.Instantiate(style.treePrefabs[Random.Range(0, style.treePrefabs.Count)], position, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
							gameObject3.transform.parent = levelRoot.transform;
							float num2 = Random.Range(0f, style.propScaleVariactionFactor);
							gameObject3.transform.localScale += new Vector3(num2, num2, num2);
						}
						break;
					case LevelNodeObjectType.WeaponCloset:
						gameObject3 = Object.Instantiate(style.rectangleRoomHidingSpotPrefab, position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.BearTrap:
						gameObject3 = Object.Instantiate(style.trapPrefab, position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.Studs:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Knob_Pickup"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.Key:
						gameObject3 = Object.Instantiate(style.keyPrefab, position, Quaternion.identity);
						if ((bool)gameObject3.GetComponentInChildren<Key>())
						{
							gameObject3.GetComponentInChildren<Key>().keyID = 1;
						}
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.DoorTrigger:
						gameObject3 = Object.Instantiate(style.doorTriggerPrefab, position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CampFire:
						gameObject3 = Object.Instantiate(style.campfirePrefab, position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard:
						gameObject3 = Object.Instantiate(style.crystalShardPrefab, position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard_earth:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CrystalShard_earth"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard_fire:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CrystalShard_fire"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard_ice:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CrystalShard_ice"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard_electricity:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CrystalShard_electricity"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard_energy:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CrystalShard_energy"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CrystalShard_water:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CrystalShard_water"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.HoloLog:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/HoloLog"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.BiteBike:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/BiteBike"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.CaveEntrance:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/CaveEntrance"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					case LevelNodeObjectType.Blueprint:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Blueprint"), position, Quaternion.identity);
						gameObject3.transform.parent = levelRoot.transform;
						break;
					}
					if (gameObject3 != null)
					{
						nodes[j, i].AddObject(gameObject3);
						InteractableObject interactableObject = gameObject3.GetComponentInChildren<InteractableObject>();
						if (interactableObject == null)
						{
							interactableObject = gameObject3.GetComponent<InteractableObject>();
						}
						Collectible collectible = gameObject3.GetComponentInChildren<Collectible>();
						if (collectible == null)
						{
							collectible = gameObject3.GetComponent<Collectible>();
						}
						if (interactableObject != null)
						{
							interactableObject.levelNode = nodes[j, i];
							interactableObject.AddObjectTag(objectType.objectTag);
						}
						if (collectible != null)
						{
							collectible.levelNode = nodes[j, i];
							collectible.AddObjectTag(objectType.objectTag);
						}
						LightSource componentInChildren = gameObject3.GetComponentInChildren<LightSource>();
						if (componentInChildren != null)
						{
							level.RegisterLightSource(componentInChildren);
						}
					}
				}
				for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
				{
					Quaternion rotation = LevelHelper.OrientationForDirection(levelDirection);
					switch (nodes[j, i].GetNodeEdgeValue(levelDirection))
					{
					case LevelNodeEdgeValue.Empty:
						gameObject = null;
						break;
					case LevelNodeEdgeValue.Ladder:
						gameObject = Object.Instantiate(style.ladder1Prefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.DoubleLadder:
						gameObject = Object.Instantiate(style.ladder2Prefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.Wall:
						gameObject = Object.Instantiate(style.wallPrefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.WallWithHidingSpot:
						gameObject = Object.Instantiate(style.wallWithHidingSpotPrefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.DownDrain:
						gameObject = Object.Instantiate(style.ladder3Prefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.Tree:
						gameObject = Object.Instantiate(style.ladder4Prefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.EnemyDoor:
						gameObject = Object.Instantiate(style.enemyDoorPrefab, position, rotation);
						if ((bool)gameObject.GetComponentInChildren<Door>())
						{
							gameObject.GetComponentInChildren<Door>().Type = Door.DoorType.EnemyDoor;
						}
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.KeyDoor:
						gameObject = Object.Instantiate(style.keyDoorPrefab, position, rotation);
						if ((bool)gameObject.GetComponentInChildren<Door>())
						{
							gameObject.GetComponentInChildren<Door>().Type = Door.DoorType.KeyDoor;
							gameObject.GetComponentInChildren<Door>().keyID = 1;
						}
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.TriggeredDoor:
						gameObject = Object.Instantiate(style.triggeredDoorPrefab, position, rotation);
						if ((bool)gameObject.GetComponentInChildren<Door>())
						{
							gameObject.GetComponentInChildren<Door>().Type = Door.DoorType.TriggeredDoor;
						}
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.ElementalDoor_earth:
						gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ElementalDoor_earth"), position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.ElementalDoor_electricity:
						gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ElementalDoor_electricity"), position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.ElementalDoor_energy:
						gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ElementalDoor_energy"), position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.ElementalDoor_fire:
						gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ElementalDoor_fire"), position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.ElementalDoor_ice:
						gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ElementalDoor_ice"), position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.ElementalDoor_water:
						gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ElementalDoor_water"), position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.AltWall:
						gameObject = Object.Instantiate(style.altWallPrefab, position, rotation);
						gameObject.transform.parent = levelRoot.transform;
						break;
					case LevelNodeEdgeValue.AltKeyDoor:
					{
						gameObject = Object.Instantiate(style.altKeyDoorPrefab, position, rotation);
						Door[] componentsInChildren = gameObject.GetComponentsInChildren<Door>();
						Door[] array = componentsInChildren;
						foreach (Door door in array)
						{
							door.Type = Door.DoorType.KeyDoor;
							door.keyID = 1;
						}
						gameObject.transform.parent = levelRoot.transform;
						break;
					}
					}
					if (gameObject != null)
					{
						nodes[j, i].SetNodeEdgeObject(levelDirection, gameObject);
						InteractableObject componentInChildren2 = gameObject.GetComponentInChildren<InteractableObject>();
						if (componentInChildren2 != null)
						{
							componentInChildren2.levelNode = nodes[j, i];
						}
					}
				}
				nodes[j, i].UpdateObjectCharacterColliders();
			}
		}
		int count = list.Count;
		int num3 = Random.Range(0, count);
		if (GameController.Instance.PlayerProfile.TotalLevelsPlayed > 0)
		{
			for (int l = 0; l < count; l++)
			{
				list[l]._pCanDropSkillPoint = true;
			}
		}
		if (UnlockableMinifigParts._pInstance.IsSeedAvailable(level.Seed))
		{
			if (num3 > 0)
			{
				list[num3 - 1]._pCanDropWardrobeItem = true;
			}
			else if (list.Count > 1)
			{
				list[num3 + 1]._pCanDropWardrobeItem = true;
			}
			else
			{
				list[num3]._pCanDropWardrobeItem = true;
			}
		}
		if (UnlockableVehicleParts._pInstance.IsSeedAvailable(level.Seed))
		{
			if (num3 > 0)
			{
				list[num3 - 1]._pCanDropVehiclePart = true;
			}
			else if (list.Count > 1)
			{
				list[num3 + 1]._pCanDropVehiclePart = true;
			}
			else
			{
				list[num3]._pCanDropVehiclePart = true;
			}
		}
		List<GameObject> list2 = new List<GameObject>();
		Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
		if (useLevelMeshBuilder)
		{
			if (levelMeshGO != null)
			{
				Object.Destroy(levelMeshGO);
			}
			levelMeshGO = new GameObject("LevelMesh");
			for (int m = 0; m < level.LevelTheme.rows; m += 5)
			{
				for (int n = 0; n < level.LevelTheme.cols; n += 5)
				{
					GameObject gameObject4 = LevelMeshBuilder.CreateMesh(level, style, m, Mathf.Min(m + 5, level.LevelTheme.rows), n, Mathf.Min(n + 5, level.LevelTheme.cols), dictionary);
					gameObject4.transform.localPosition = new Vector3(-0.5f, 0f, -0.5f);
					gameObject4.SetLayerRecursivly("Occluder");
					gameObject4.transform.SetParent(levelMeshGO.transform, true);
					list2.Add(gameObject4);
				}
			}
		}
		Random.seed = level.Seed;
		decoBuilder.DecorateLevel(level, style, levelRoot, list2, dictionary);
		for (int num4 = 0; num4 < list2.Count; num4++)
		{
			MeshFilter component2 = list2[num4].GetComponent<MeshFilter>();
			Mesh mesh = component2.mesh;
			Vector3[] vertices = mesh.vertices;
			Color[] colors = mesh.colors;
			Vector3[] normals = mesh.normals;
			for (int num5 = 0; num5 < vertices.Length; num5++)
			{
				int key2 = LevelMeshBuilder.CustomVertexHashCode(vertices[num5]);
				if (dictionary.ContainsKey(key2))
				{
					vertices[num5] = dictionary[key2];
				}
			}
			LevelMeshBuilder.CreateFloorNoiseBasedOnHeight(vertices, colors, normals, style);
			if (optimizeLevel)
			{
				MeshFilter[] componentsInChildren2 = list2[num4].GetComponentsInChildren<MeshFilter>();
				int num6 = 0;
				int num7 = 0;
				for (int num8 = 0; num8 < componentsInChildren2.Length; num8++)
				{
					if (componentsInChildren2[num8].mesh != mesh)
					{
						num7 += componentsInChildren2[num8].mesh.triangles.Length;
						num6 += componentsInChildren2[num8].mesh.vertexCount;
					}
				}
				List<Vector3> list3 = new List<Vector3>(mesh.vertexCount + num6);
				List<Vector3> list4 = new List<Vector3>(mesh.vertexCount + num6);
				List<Color> list5 = new List<Color>(mesh.vertexCount + num6);
				List<int> list6 = new List<int>(mesh.triangles.Length + num7);
				list3.AddRange(vertices);
				list4.AddRange(normals);
				list5.AddRange(colors);
				list6.AddRange(mesh.triangles);
				for (int num9 = 0; num9 < componentsInChildren2.Length; num9++)
				{
					if (!(componentsInChildren2[num9].gameObject != component2.gameObject))
					{
						continue;
					}
					Transform transform = componentsInChildren2[num9].transform;
					if (!(componentsInChildren2[num9].GetComponentInChildren<Renderer>().material.shader != list2[num4].GetComponentInChildren<Renderer>().material.shader))
					{
						Matrix4x4 matrix4x = list2[num4].transform.worldToLocalMatrix * transform.localToWorldMatrix;
						int count2 = list3.Count;
						Vector3[] vertices2 = componentsInChildren2[num9].mesh.vertices;
						Vector3[] normals2 = componentsInChildren2[num9].mesh.normals;
						for (int num10 = 0; num10 < vertices2.Length; num10++)
						{
							vertices2[num10] = matrix4x.MultiplyPoint3x4(vertices2[num10]);
							normals2[num10] = matrix4x.MultiplyVector(normals2[num10]);
						}
						list3.AddRange(vertices2);
						list4.AddRange(normals2);
						list5.AddRange(componentsInChildren2[num9].mesh.colors);
						int[] triangles = componentsInChildren2[num9].mesh.triangles;
						for (int num11 = 0; num11 < triangles.Length; num11++)
						{
							triangles[num11] += count2;
						}
						list6.AddRange(triangles);
						Object.Destroy(componentsInChildren2[num9].gameObject);
					}
				}
				mesh.vertices = list3.ToArray();
				mesh.normals = list4.ToArray();
				mesh.uv = null;
				mesh.uv2 = null;
				mesh.colors = list5.ToArray();
				mesh.triangles = list6.ToArray();
				if (mesh.vertexCount == 0)
				{
					Object.Destroy(component2);
					Object.Destroy(list2[num4].GetComponent<Renderer>());
					Object.Destroy(list2[num4].GetComponent<Collider>());
				}
			}
			else
			{
				mesh.vertices = vertices;
				mesh.colors = colors;
			}
		}
		if (style.styleName.Length >= 7 && style.styleName.Substring(0, 7) == "Dungeon")
		{
			gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Audio/FullLevelReverbZone"));
			gameObject.transform.position = new Vector3(25f, 0f, 25f);
		}
		foreach (InterestZoneData interestZone in level.ChunkDB.interestZones)
		{
			gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Camera/InterestZone"));
			gameObject.transform.parent = levelRoot.transform;
			InterestZone component3 = gameObject.GetComponent<InterestZone>();
			component3.center = interestZone.center;
			component3.extent = new LevelPoint(interestZone.extent.row - 1, interestZone.extent.col - 1);
		}
		LevelNode[,] array2 = nodes;
		int length = array2.GetLength(0);
		int length2 = array2.GetLength(1);
		for (int num12 = 0; num12 < length; num12++)
		{
			for (int num13 = 0; num13 < length2; num13++)
			{
				LevelNode levelNode = array2[num12, num13];
				bool flag3 = false;
				for (LevelDirection levelDirection2 = LevelDirection.North; levelDirection2 <= LevelDirection.West; levelDirection2++)
				{
					if (levelNode.GetNodeEdgeValue(levelDirection2) == LevelNodeEdgeValue.AltKeyDoor || levelNode.GetNodeEdgeValue(levelDirection2) == LevelNodeEdgeValue.KeyDoor || levelNode.GetNodeEdgeValue(levelDirection2) == LevelNodeEdgeValue.TriggeredDoor)
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Camera/InterestPoint"));
					gameObject.transform.parent = levelRoot.transform;
					gameObject.transform.position = LevelHelper.NodeToVector3(levelNode) + new Vector3(0.5f, 0.5f, 0f);
				}
			}
		}
		if (level.Seed == 4030)
		{
			GameObject gameObject5 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Vehicles/70626_DawnofIronDoom"));
			gameObject5.transform.parent = levelMeshGO.transform;
			gameObject5.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			gameObject5.transform.position = new Vector3(31.16f, 0.25f, 20.79f);
			gameObject5.transform.Rotate(0f, 166.68f, 0f);
		}
	}

	private string GetPrefabName(LevelNodeType type)
	{
		switch (type)
		{
		case LevelNodeType.None:
			return "Prefabs/LevelTileNone";
		case LevelNodeType.Fill:
			return "Prefabs/LevelTile_fill";
		case LevelNodeType.MainPath:
			return "Prefabs/LevelTile_main";
		case LevelNodeType.Room:
			return "Prefabs/LevelTile_room";
		case LevelNodeType.SideQuest:
			return "Prefabs/LevelTile_sideQuest";
		default:
			return "Prefabs/LevelTileNone";
		}
	}
}
