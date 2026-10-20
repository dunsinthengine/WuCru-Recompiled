using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

internal class CharacterBuilder
{
	public static Dictionary<int, MinifigDefinition> minifigDefinitions;

	public static List<int> ninjas;

	public static Dictionary<MinifigType, Dictionary<MinifigClass, int>> enemies;

	public static List<string> weaponModels;

	public static Dictionary<int, Texture2D> menuCharacterTextures;

	public static Dictionary<int, Texture2D> inGameCharacterTextures;

	public static List<Texture2D> excessTextures;

	private const int faceOffsetX = 10;

	private const int faceOffsetY = 126;

	private const int faceWidth = 248;

	private const int faceHeight = 148;

	private const int torsoOffsetX = 6;

	private const int torsoOffsetY = 284;

	private const int torsoWidth = 498;

	private const int torsoHeight = 213;

	private const int pelvisOffsetX = 11;

	private const int pelvisOffsetY = 21;

	private const int pelvisWidth = 337;

	private const int pelvisHeight = 95;

	private const int legsOffsetX = 263;

	private const int legsOffsetY = 74;

	private const int legsWidth = 242;

	private const int legsHeight = 195;

	private const int colorSlotsOffsetX = 353;

	private const int colorSlotsOffsetY = 23;

	private const int colorSlotsWidth = 153;

	private const int colorSlotsHeight = 41;

	private const int colorSlotCount = 16;

	private const int colorSlotSize = 14;

	private const int colorSlotOffsetX = 355;

	private const int colorSlotOffsetY = 47;

	private const int colorSlotOffset = 19;

	static CharacterBuilder()
	{
		minifigDefinitions = new Dictionary<int, MinifigDefinition>();
		ninjas = new List<int>();
		enemies = new Dictionary<MinifigType, Dictionary<MinifigClass, int>>();
		weaponModels = new List<string>
		{
			string.Empty,
			"Weapons/Katana",
			"Weapons/Shuriken",
			"Weapons/Nunchuck",
			"Weapons/Scythe",
			"Weapons/Axe",
			"Weapons/Staff",
			"Weapons/Scimitar",
			"Weapons/Spear",
			"Weapons/VM_Hammer",
			"Weapons/VM_Scythe",
			"Weapons/VM_Sword",
			"Weapons/SharkArmy_Sword",
			"Weapons/SharkArmy_Staff",
			"Weapons/SharkArmy_Spear",
			"Weapons/SharkArmy_Hammer"
		};
		menuCharacterTextures = new Dictionary<int, Texture2D>();
		inGameCharacterTextures = new Dictionary<int, Texture2D>();
		excessTextures = new List<Texture2D>();
		TextAsset textAsset = Resources.Load("minifigs") as TextAsset;
		using (StringReader stringReader = new StringReader(textAsset.text))
		{
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split(',');
				int result;
				if (int.TryParse(array[0], out result))
				{
					int result2 = 0;
					if (!int.TryParse(array[1], out result2))
					{
					}
					MinifigType minifigType = MinifigType.None;
					if (Enum.IsDefined(typeof(MinifigType), array[2]))
					{
						minifigType = (MinifigType)Enum.Parse(typeof(MinifigType), array[2]);
					}
					MinifigClass minifigClass = MinifigClass.None;
					if (Enum.IsDefined(typeof(MinifigClass), array[3]))
					{
						minifigClass = (MinifigClass)Enum.Parse(typeof(MinifigClass), array[3]);
					}
					WeaponType weapon = WeaponType.None;
					if (Enum.IsDefined(typeof(WeaponType), array[14]))
					{
						weapon = (WeaponType)Enum.Parse(typeof(WeaponType), array[14]);
					}
					ElementalPower elementalPower = ElementalPower.Earth;
					if (Enum.IsDefined(typeof(ElementalPower), array[15]))
					{
						elementalPower = (ElementalPower)Enum.Parse(typeof(ElementalPower), array[15]);
					}
					MinifigDefinition value = new MinifigDefinition
					{
						id = result,
						sku = result2,
						type = minifigType,
						minifigClass = minifigClass,
						hairModel = array[4],
						helmetModel = array[5],
						showHelmet = (array[6] == "TRUE"),
						helmetReplacesHead = (array[7] == "TRUE"),
						helmetHidesHair = (array[8] == "TRUE"),
						shoulderModel = array[16],
						colorSlots = array[9],
						faceDecal = array[10],
						legDecal = array[11],
						pelvisDecal = array[12],
						torsoDecal = array[13],
						weapon = weapon,
						elementalPower = elementalPower
					};
					minifigDefinitions.Add(result, value);
					if (IsEnemy(minifigType))
					{
						if (!enemies.ContainsKey(minifigType))
						{
							enemies[minifigType] = new Dictionary<MinifigClass, int>();
						}
						enemies[minifigType][minifigClass] = result;
					}
					else
					{
						ninjas.Add(result);
					}
				}
			}
		}
		Resources.UnloadAsset(textAsset);
	}

	public static void CleanupMenuCharacterTextures()
	{
		foreach (Texture2D value in menuCharacterTextures.Values)
		{
			UnityEngine.Object.Destroy(value);
		}
		menuCharacterTextures.Clear();
		foreach (Texture2D excessTexture in excessTextures)
		{
			UnityEngine.Object.Destroy(excessTexture);
		}
		excessTextures.Clear();
	}

	public static void CleanupMenuCharacterTexture(int ninjaID)
	{
		if (menuCharacterTextures.ContainsKey(ninjaID))
		{
			UnityEngine.Object.Destroy(menuCharacterTextures[ninjaID]);
			menuCharacterTextures.Remove(ninjaID);
		}
	}

	public static void CleanupInGameCharacterTextures()
	{
		foreach (Texture2D value in inGameCharacterTextures.Values)
		{
			UnityEngine.Object.Destroy(value);
		}
		inGameCharacterTextures.Clear();
		foreach (Texture2D excessTexture in excessTextures)
		{
			UnityEngine.Object.Destroy(excessTexture);
		}
		excessTextures.Clear();
	}

	public static void CleanupInGameCharacterTexture(int ninjaID)
	{
		if (inGameCharacterTextures.ContainsKey(ninjaID))
		{
			UnityEngine.Object.Destroy(inGameCharacterTextures[ninjaID]);
			inGameCharacterTextures.Remove(ninjaID);
		}
	}

	public static bool IsEnemy(MinifigType type)
	{
		switch (type)
		{
		case MinifigType.Constrictai:
		case MinifigType.Fangpyre:
		case MinifigType.Hynobrai:
		case MinifigType.Venomari:
		case MinifigType.VermillionWarrior1:
		case MinifigType.VermillionWarrior2:
		case MinifigType.VermillionWarrior3:
		case MinifigType.VermillionWarrior4:
		case MinifigType.SharkArmy1:
		case MinifigType.SharkArmy2:
		case MinifigType.SharkArmy3:
		case MinifigType.SharkArmy4:
			return true;
		default:
			return false;
		}
	}

	public static Texture2D ConfigureTexture(Character character, MinifigDefinition def)
	{
		if (inGameCharacterTextures.ContainsKey(def.id))
		{
			SetTexture(character.bodyparts, inGameCharacterTextures[def.id]);
			return inGameCharacterTextures[def.id];
		}
		Texture2D texture2D = ConfigureTexture(character.bodyparts, def.faceDecal, def.torsoDecal, def.pelvisDecal, def.legDecal, def.colorSlots, def.colors);
		texture2D.name = "Minifig_ingame_" + def.id;
		inGameCharacterTextures.Add(def.id, texture2D);
		return texture2D;
	}

	public static Texture2D ConfigureTexture(MenuCharacter character, MinifigDefinition def)
	{
		if (menuCharacterTextures.ContainsKey(character.ninjaID))
		{
			CleanupMenuCharacterTexture(character.ninjaID);
		}
		Texture2D texture2D = ConfigureTexture(character.bodyparts, def.faceDecal, def.torsoDecal, def.pelvisDecal, def.legDecal, def.colorSlots, def.colors);
		texture2D.name = "Minifig_menu_" + def.id;
		if (menuCharacterTextures.ContainsKey(def.id))
		{
			excessTextures.Add(menuCharacterTextures[def.id]);
			menuCharacterTextures.Remove(def.id);
		}
		menuCharacterTextures.Add(def.id, texture2D);
		return texture2D;
	}

	public static Texture2D ConfigureTexture(GameObject[] bodyparts, string faceTextureName, string torsoTextureName, string pelvisTextureName, string legsTextureName, string colorsTextureName, Color32[] colors)
	{
		Texture2D texture2D = new Texture2D(512, 512, TextureFormat.RGBA32, true);
		Color32[] array = new Color32[262144];
		for (int i = 0; i < 262144; i++)
		{
			array[i] = new Color32(0, 0, 0, 0);
		}
		texture2D.SetPixels32(array);
		Texture2D texture2D2 = Resources.Load(Paths.characterTextureBasePath + faceTextureName) as Texture2D;
		if ((bool)texture2D2)
		{
			texture2D.SetPixels32(10, 126, 248, 148, texture2D2.GetPixels32());
			Resources.UnloadAsset(texture2D2);
		}
		Texture2D texture2D3 = Resources.Load(Paths.characterTextureBasePath + torsoTextureName) as Texture2D;
		if ((bool)texture2D3)
		{
			texture2D.SetPixels32(6, 284, 498, 213, texture2D3.GetPixels32());
			Resources.UnloadAsset(texture2D3);
		}
		Texture2D texture2D4 = Resources.Load(Paths.characterTextureBasePath + pelvisTextureName) as Texture2D;
		if ((bool)texture2D4)
		{
			texture2D.SetPixels32(11, 21, 337, 95, texture2D4.GetPixels32());
			Resources.UnloadAsset(texture2D4);
		}
		Texture2D texture2D5 = Resources.Load(Paths.characterTextureBasePath + legsTextureName) as Texture2D;
		if ((bool)texture2D5)
		{
			texture2D.SetPixels32(263, 74, 242, 195, texture2D5.GetPixels32());
			Resources.UnloadAsset(texture2D5);
		}
		if (!string.IsNullOrEmpty(colorsTextureName))
		{
			Texture2D texture2D6 = Resources.Load(Paths.characterTextureBasePath + colorsTextureName) as Texture2D;
			if ((bool)texture2D6)
			{
				texture2D.SetPixels32(353, 23, 153, 41, texture2D6.GetPixels32());
				Resources.UnloadAsset(texture2D6);
			}
			Debug.Log(Paths.characterTextureBasePath + colorsTextureName);
		}
		if (colors != null)
		{
			for (int j = 0; j < colors.Length; j++)
			{
				Color32[] array2 = new Color32[196];
				for (int k = 0; k < 196; k++)
				{
					array2[k] = colors[j];
				}
				texture2D.SetPixels32(355 + j % 8 * 19, 47 - j / 8 * 19, 14, 14, array2);
			}
		}
		texture2D.Apply(true, true);
		SetTexture(bodyparts, texture2D);
		return texture2D;
	}

	public static void SetTexture(GameObject[] bodyparts, Texture2D texture)
	{
		for (int i = 0; i < bodyparts.Length; i++)
		{
			Renderer component = bodyparts[i].GetComponent<Renderer>();
			component.material.SetTexture("_DecalAsset", texture);
		}
	}

	public static void ConfigureCharacter(Character character, MinifigDefinition def, Texture2D characterTexture, bool optimize = false)
	{
		if (character.hairGameObject != null)
		{
			UnityEngine.Object.Destroy(character.hairGameObject);
		}
		if (character.helmetGameObject != null)
		{
			UnityEngine.Object.Destroy(character.helmetGameObject);
		}
		ConfigureCharacter(character.bodyparts, character.headTransform, ref character.swordTransform, character.shoulderTransform, def.hairModel, (!def.showHelmet) ? null : def.helmetModel, def.helmetHidesHair, def.helmetReplacesHead, def.shoulderModel, def.weapon, characterTexture, out character.hairGameObject, out character.helmetGameObject, out character.weaponGameObject);
		character.RefreshMeshRenderers();
	}

	public static void ConfigureCharacter(MenuCharacter character, MinifigDefinition def, Texture2D characterTexture)
	{
		if (character.hairGameObject != null)
		{
			UnityEngine.Object.Destroy(character.hairGameObject);
		}
		if (character.helmetGameObject != null)
		{
			UnityEngine.Object.Destroy(character.helmetGameObject);
		}
		ConfigureCharacter(character.bodyparts, character.headTransform, ref character.swordTransform, character.shoulderTransform, def.hairModel, (!def.showHelmet) ? null : def.helmetModel, def.helmetHidesHair, def.helmetReplacesHead, def.shoulderModel, def.weapon, characterTexture, out character.hairGameObject, out character.helmetGameObject, out character.weaponGameObject);
		character.ninjaID = def.id;
	}

	private static void ConfigureCharacter(GameObject[] bodyparts, Transform headTransform, ref Transform swordTransform, Transform shoulderTransform, string hairModelName, string helmetModelName, bool helmetHidesHair, bool helmetReplacesHead, string shoulderModelName, WeaponType weaponType, Texture2D characterTexture, out GameObject hairGameObject, out GameObject helmetGameObject, out GameObject weaponGameObject)
	{
		Transform transform = headTransform.Find("NinjaHelmet");
		if ((bool)transform)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		if (!string.IsNullOrEmpty(hairModelName) && (string.IsNullOrEmpty(helmetModelName) || !helmetHidesHair))
		{
			hairGameObject = UnityEngine.Object.Instantiate(Resources.Load(Paths.characterPrefabBasePath + hairModelName) as GameObject);
			hairGameObject.transform.SetParent(headTransform, false);
			Renderer[] componentsInChildren = hairGameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.material.HasProperty("_MapUV1"))
				{
					renderer.material.SetTexture("_MapUV1", characterTexture);
				}
			}
			hairGameObject.SetLayerRecursivly(headTransform.gameObject.layer);
		}
		else
		{
			hairGameObject = null;
		}
		if (!string.IsNullOrEmpty(helmetModelName))
		{
			bodyparts[0].SetActive(!helmetReplacesHead);
			helmetGameObject = UnityEngine.Object.Instantiate(Resources.Load(Paths.characterPrefabBasePath + helmetModelName) as GameObject);
			helmetGameObject.transform.SetParent(headTransform, false);
			Renderer[] componentsInChildren2 = helmetGameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer2 in componentsInChildren2)
			{
				if (renderer2.material.HasProperty("_MapUV1"))
				{
					renderer2.material.SetTexture("_MapUV1", characterTexture);
				}
			}
			helmetGameObject.SetLayerRecursivly(headTransform.gameObject.layer);
		}
		else
		{
			bodyparts[0].SetActive(true);
			helmetGameObject = null;
		}
		if (weaponType != WeaponType.None)
		{
			weaponGameObject = UnityEngine.Object.Instantiate(Resources.Load(Paths.characterPrefabBasePath + weaponModels[(int)weaponType]) as GameObject);
			Transform parent = swordTransform.parent;
			Vector3 localPosition = swordTransform.localPosition;
			Quaternion localRotation = swordTransform.localRotation;
			UnityEngine.Object.Destroy(swordTransform.gameObject);
			weaponGameObject.transform.parent = parent;
			swordTransform = weaponGameObject.transform;
			weaponGameObject.transform.localPosition = localPosition;
			weaponGameObject.transform.localRotation = localRotation;
			Renderer[] componentsInChildren3 = weaponGameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer3 in componentsInChildren3)
			{
				if (renderer3.material.HasProperty("_MapUV1"))
				{
					renderer3.material.SetTexture("_MapUV1", characterTexture);
				}
			}
			weaponGameObject.SetLayerRecursivly(headTransform.gameObject.layer);
		}
		else
		{
			weaponGameObject = null;
			if (swordTransform != null)
			{
				UnityEngine.Object.Destroy(swordTransform.gameObject);
			}
		}
		if (string.IsNullOrEmpty(shoulderModelName))
		{
			return;
		}
		Debug.Log("SHOULDER: " + shoulderModelName);
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(Paths.characterPrefabBasePath + shoulderModelName) as GameObject);
		gameObject.transform.SetParent(shoulderTransform, false);
		Renderer[] componentsInChildren4 = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer4 in componentsInChildren4)
		{
			if (renderer4.material.HasProperty("_MapUV1"))
			{
				renderer4.material.SetTexture("_MapUV1", characterTexture);
			}
		}
		gameObject.SetLayerRecursivly(headTransform.gameObject.layer);
	}

	public static void UpdateMenuCharacterHelmet(MenuCharacter character, Texture2D characterTexture, MinifigDefinition def)
	{
		if (def.showHelmet && character.helmetGameObject == null)
		{
			character.bodyparts[0].SetActive(!def.helmetReplacesHead);
			character.helmetGameObject = UnityEngine.Object.Instantiate(Resources.Load(Paths.characterPrefabBasePath + def.helmetModel) as GameObject);
			character.helmetGameObject.transform.SetParent(character.headTransform, false);
			Renderer[] componentsInChildren = character.helmetGameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.material.HasProperty("_MapUV1"))
				{
					renderer.material.SetTexture("_MapUV1", characterTexture);
				}
			}
			character.helmetGameObject.SetLayerRecursivly(character.headTransform.gameObject.layer);
		}
		if (!def.showHelmet && character.helmetGameObject != null)
		{
			character.bodyparts[0].SetActive(true);
			UnityEngine.Object.Destroy(character.helmetGameObject);
			character.helmetGameObject = null;
		}
	}

	public static void ConfigureMinifig(Character character, int id)
	{
		if (character == null) return;
		MinifigDefinition minifigDefinition = null;
		if (id == 0 && GameController.Instance != null && GameController.Instance.PlayerProfile != null)
		{
			minifigDefinition = GameController.Instance.PlayerProfile.GetMinifigDefinition();
		}
		else if (minifigDefinitions.ContainsKey(id))
		{
			minifigDefinition = minifigDefinitions[id];
		}
		else if (minifigDefinitions.Count > 0)
		{
			Debug.LogWarning("MinifigDefinition not found for id: " + id + ", using fallback");
			foreach (var def in minifigDefinitions.Values) { minifigDefinition = def; break; }
		}
		if (minifigDefinition == null) return;
		Texture2D characterTexture = ConfigureTexture(character, minifigDefinition);
		ConfigureCharacter(character, minifigDefinition, characterTexture);
		character.elementalPower = minifigDefinition.elementalPower;
		Debug.Log("configure minifig: " + character.gameObject.name + " = " + character.elementalPower);
		character.NinjaID = id;
	}

	public static void ConfigureMinifig(MenuCharacter character, int id)
	{
		if (character == null) return;
		MinifigDefinition def = null;
		if (id == 0 && GameController.Instance != null && GameController.Instance.PlayerProfile != null)
		{
			def = GameController.Instance.PlayerProfile.GetMinifigDefinition();
		}
		else if (minifigDefinitions.ContainsKey(id))
		{
			def = minifigDefinitions[id];
		}
		else if (minifigDefinitions.Count > 0)
		{
			Debug.LogWarning("MinifigDefinition not found for id: " + id + ", using fallback");
			foreach (var d in minifigDefinitions.Values) { def = d; break; }
		}
		if (def == null) return;
		Texture2D characterTexture = ConfigureTexture(character, def);
		ConfigureCharacter(character, def, characterTexture);
	}

	public static void RenderToImage(RawImage targetImage, CanvasScaler scaler, Camera targetCamera, Transform parent, int ninjaID = 0)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CharacterCreation/MenuCharacter"));
		MenuCharacter component = gameObject.GetComponent<MenuCharacter>();
		MinifigDefinition def = null;
		if (ninjaID == 0 && GameController.Instance != null && GameController.Instance.PlayerProfile != null)
		{
			def = GameController.Instance.PlayerProfile.GetMinifigDefinition();
		}
		else if (minifigDefinitions.ContainsKey(ninjaID))
		{
			def = minifigDefinitions[ninjaID];
		}
		else if (minifigDefinitions.Count > 0)
		{
			foreach (var d in minifigDefinitions.Values) { def = d; break; }
		}
		if (def == null) return;
		Texture2D characterTexture = ConfigureTexture(component, def);
		ConfigureCharacter(component, def, characterTexture);
		gameObject.transform.SetParent(parent, false);
		bool fog = RenderSettings.fog;
		RenderSettings.fog = false;
		Vector2 vector = new Vector2(scaler.referenceResolution.x / (float)Screen.width, scaler.referenceResolution.y / (float)Screen.height);
		float num = targetImage.rectTransform.sizeDelta.x / scaler.referenceResolution.x;
		float num2 = targetImage.rectTransform.sizeDelta.y / targetImage.rectTransform.sizeDelta.x;
		float num3 = (float)Screen.width * num;
		float num4 = (float)Screen.width * num * num2;
		RenderTexture texture = (targetCamera.targetTexture = new RenderTexture((int)num3, (int)num4, 24, RenderTextureFormat.ARGB32));
		targetCamera.Render();
		targetCamera.targetTexture = null;
		targetCamera.gameObject.SetActive(false);
		targetImage.texture = texture;
		RenderSettings.fog = fog;
		UnityEngine.Object.Destroy(gameObject);
		gameObject.SetActive(false);
	}

	public static void Optimize(GameObject compositeGameObject, GameObject swordTransform)
	{
		List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>(compositeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>());
		SkinnedMeshRenderer componentInChildren = swordTransform.GetComponentInChildren<SkinnedMeshRenderer>();
		list.Remove(componentInChildren);
		List<Transform> list2 = new List<Transform>();
		List<BoneWeight> list3 = new List<BoneWeight>();
		List<CombineInstance> list4 = new List<CombineInstance>();
		Material material = list[0].material;
		int num = 0;
		foreach (SkinnedMeshRenderer item3 in list)
		{
			num += item3.sharedMesh.subMeshCount;
		}
		int num2 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = list[i];
			BoneWeight[] boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
			BoneWeight[] array = boneWeights;
			foreach (BoneWeight boneWeight in array)
			{
				BoneWeight item = boneWeight;
				item.boneIndex0 += num2;
				item.boneIndex1 += num2;
				item.boneIndex2 += num2;
				item.boneIndex3 += num2;
				list3.Add(item);
			}
			num2 += skinnedMeshRenderer.bones.Length;
			Transform[] bones = skinnedMeshRenderer.bones;
			Transform[] array2 = bones;
			foreach (Transform item2 in array2)
			{
				list2.Add(item2);
			}
			list4.Add(new CombineInstance
			{
				mesh = skinnedMeshRenderer.sharedMesh,
				transform = Matrix4x4.identity
			});
			UnityEngine.Object.Destroy(skinnedMeshRenderer.gameObject);
		}
		List<Matrix4x4> list5 = new List<Matrix4x4>();
		for (int l = 0; l < list2.Count; l++)
		{
			list5.Add(list2[l].worldToLocalMatrix * list[0].transform.parent.localToWorldMatrix);
		}
		SkinnedMeshRenderer skinnedMeshRenderer2 = compositeGameObject.AddComponent<SkinnedMeshRenderer>();
		skinnedMeshRenderer2.sharedMesh = new Mesh();
		skinnedMeshRenderer2.sharedMesh.CombineMeshes(list4.ToArray(), true, true);
		skinnedMeshRenderer2.sharedMaterial = material;
		skinnedMeshRenderer2.bones = list2.ToArray();
		skinnedMeshRenderer2.sharedMesh.boneWeights = list3.ToArray();
		skinnedMeshRenderer2.sharedMesh.bindposes = list5.ToArray();
		skinnedMeshRenderer2.sharedMesh.RecalculateBounds();
	}
}
