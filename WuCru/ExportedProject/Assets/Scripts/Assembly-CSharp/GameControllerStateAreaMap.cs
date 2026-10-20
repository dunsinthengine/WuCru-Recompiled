using System.Collections;
using System.Collections.Generic;
using Cape.Utils.UI;
using Fabric;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerStateAreaMap : GameControllerState
{
	private AreaMap areaMap;

	public GameObject map;

	public RawImage mapImage;

	public GameObject iconContainer;

	public RawImage addonImage;

	public RawImage fowImage;

	public bool showAddon;

	public RawImage gridImage;

	public RawImage cloudImage;

	public RawImage fxImage;

	public Button wuReminderButton;

	public Image wuSpeechBubble;

	public Text wuText;

	public Texture lineTexture;

	private GameObject playerIconGO;

	private int rows = 25;

	private int cols = 25;

	private int emitters;

	private int tileWidth;

	private int tileHeight;

	private Vector3 lowerLeftCorner;

	private bool isInitialized;

	private bool isConfirmingPlay;

	private bool isPreparingPlay;

	private float newItemsInterval = 0.8f;

	private List<GameObject> icons = new List<GameObject>();

	private Sprite areaMapSprite;

	private Sprite areaMapAddonSprite;

	private int seedToPlay;

	private AreaMapNode nodeToPlay;

	private FadeOverlay _fadeOverlay;

	private int _numUnlockedLevels;

	public AreaMapNode AreaMapNodeToPlay
	{
		get
		{
			return nodeToPlay;
		}
	}

	public int AreaMapSeedToPlay
	{
		get
		{
			return seedToPlay;
		}
	}

	public override void BeginState()
	{
		_fadeOverlay = Object.FindObjectOfType<FadeOverlay>();
		CharacterBuilder.CleanupInGameCharacterTextures();
		CharacterBuilder.CleanupMenuCharacterTextures();
		_numUnlockedLevels = 0;
		isInitialized = false;
		isConfirmingPlay = false;
		isPreparingPlay = false;
		isInitialized = false;
		StartCoroutine(Initialize());
		GameAudioManager.Instance.StopAllMusicExcept("music_area_map", false, 1f);
		if (!EventManager.Instance.IsEventActive("music/area/map", FabricManager.Instance.gameObject))
		{
			AudioPlayer.PlaySound("music/area/map");
		}
	}

	public override void EndState()
	{
		Object.Destroy(areaMapSprite);
		Object.Destroy(areaMapAddonSprite);
		Resources.UnloadUnusedAssets();
		base.enabled = false;
		NinjaPortrait ninjaPortrait = Object.FindObjectOfType<NinjaPortrait>();
		if (ninjaPortrait != null)
		{
			RenderTexture renderTexture = ninjaPortrait.portrait.texture as RenderTexture;
			if ((bool)renderTexture)
			{
				renderTexture.Release();
				ninjaPortrait.portrait.texture = null;
			}
		}
		CharacterBuilder.CleanupInGameCharacterTextures();
		CharacterBuilder.CleanupMenuCharacterTextures();
		if (playerIconGO != null)
		{
			playerIconGO.gameObject.SetActive(false);
		}
	}

	private IEnumerator Initialize()
	{
		isPreparingPlay = false;
		rows = GameController.Instance.areaMapMetaLayer.rowArray.Length;
		cols = GameController.Instance.areaMapMetaLayer.rowArray[0].colArray.Length;
		emitters = GameController.Instance.areaMapMetaLayer.emitterArray.Length;
		if (GameController.Instance.areaMapMetaLayer.IsFirstTimeInArea())
		{
			LevelUnlockData levelUnlockData = new LevelUnlockData();
			levelUnlockData.seed = GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(GameController.Instance.areaMapMetaLayer.startRow, GameController.Instance.areaMapMetaLayer.startCol);
			levelUnlockData.season = GameController.Instance.areaMapMetaLayer.season;
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData.seed, levelUnlockData);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData.seed + "</color>");
			GameController.Instance.PlayerProfile.Save();
		}
		yield return 0;
		_fadeOverlay.StartFadeIn();
		areaMap = Object.FindObjectOfType<AreaMap>();
		if (areaMap == null)
		{
			Debug.LogError("Could not find map!");
		}
		mapImage = areaMap.mapImage;
		map = areaMap.map;
		Image mapRectsImage = map.GetComponent<Image>();
		iconContainer = areaMap.iconContainer;
		areaMapSprite = Object.Instantiate(Resources.Load<Sprite>(GameController.Instance.areaMapMetaLayer.mapImagePath));
		mapImage.texture = areaMapSprite.texture;
		areaMap.fowImage.enabled = false;
		addonImage = areaMap.addonImage;
		showAddon = GameController.Instance.areaMapMetaLayer.showAddon;
		int season = GameController.Instance.areaMapMetaLayer.season;
		int chapter = GameController.Instance.areaMapMetaLayer.addonUnlockChapter;
		int currentChapter = GameController.Instance.PlayerProfile.GetChapterCursor(season);
		int currentSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
		if (showAddon && currentChapter >= chapter && currentSeason >= season && addonImage != null && GameController.Instance.areaMapMetaLayer.addonImagePath != null)
		{
			AreaMapMetaLayer areaMapMetaLayer = GameController.Instance.areaMapMetaLayer;
			addonImage.enabled = true;
			showAddon = true;
			areaMapAddonSprite = Object.Instantiate(Resources.Load<Sprite>(GameController.Instance.areaMapMetaLayer.addonImagePath));
			addonImage.texture = areaMapAddonSprite.texture;
			float x = addonImage.gameObject.GetComponentInParent<Canvas>().GetComponent<RectTransform>().transform.localScale.x;
			float y = addonImage.gameObject.GetComponentInParent<Canvas>().GetComponent<RectTransform>().transform.localScale.y;
			addonImage.rectTransform.sizeDelta = new Vector2((float)((areaMapMetaLayer.addonEndCol - areaMapMetaLayer.addonStartCol + 1) * tileWidth) * (1f / x), (float)((areaMapMetaLayer.addonEndRow - areaMapMetaLayer.addonStartRow + 1) * tileHeight) * (1f / y));
			addonImage.rectTransform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * areaMapMetaLayer.addonStartCol), lowerLeftCorner.y + (float)(tileHeight * areaMapMetaLayer.addonStartRow));
		}
		else
		{
			addonImage.enabled = false;
		}
		fowImage = areaMap.fowImage;
		gridImage = areaMap.gridImage;
		cloudImage = areaMap.cloudImage;
		fxImage = areaMap.fxImage;
		wuReminderButton = areaMap.wuReminderButton;
		wuSpeechBubble = areaMap.wuSpeechBubble;
		wuText = areaMap.wuText;
		wuReminderButton.gameObject.SetActive(false);
		wuSpeechBubble.gameObject.SetActive(false);
		wuText.gameObject.SetActive(false);
		Vector3[] corners = new Vector3[4];
		mapRectsImage.rectTransform.GetWorldCorners(corners);
		lowerLeftCorner = corners[0];
		float width = corners[3].x - corners[0].x;
		float height = corners[1].y - corners[0].y;
		tileHeight = Mathf.RoundToInt(height / (float)rows);
		tileWidth = Mathf.RoundToInt(width / (float)cols);
		fowImage.gameObject.SetActive(false);
		fowImage.texture = GenerateFogOfWarTexture();
		yield return 0;
		fowImage.gameObject.SetActive(true);
		mapImage.enabled = true;
		fowImage.enabled = true;
		mapImage.material.color = new Color(1f, 1f, 1f, 1f);
		float emitterCamOffsetX = -880f;
		float emitterCamOffsetY = 5f;
		float emitterCamOffsetZ = 500f;
		float emitterYmoveFactor = 27.7f;
		float emitterXmoveFactor = 35f;
		Vector3 position = default(Vector3);
		for (int i = 0; i < emitters; i++)
		{
			GameObject gameObject = Object.Instantiate(GameController.Instance.areaMapMetaLayer.emitterArray[i].emitter);
			position.x = (float)GameController.Instance.areaMapMetaLayer.emitterArray[i].emitterCol * emitterXmoveFactor + emitterCamOffsetX + GameController.Instance.areaMapMetaLayer.emitterArray[i].emitterLocalOffsetX;
			position.y = (float)GameController.Instance.areaMapMetaLayer.emitterArray[i].emitterRow * emitterYmoveFactor + emitterCamOffsetY + GameController.Instance.areaMapMetaLayer.emitterArray[i].emitterLocalOffsetY;
			position.z = emitterCamOffsetZ;
			gameObject.transform.position = position;
			ParticleSystem component = gameObject.transform.GetComponent<ParticleSystem>();
			component.Simulate(0.5f);
			component.Play();
		}
		for (int j = 0; j < rows; j++)
		{
			for (int k = 0; k < cols; k++)
			{
				if (((GameController.Instance.areaMapMetaLayer.IsUnlocked(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(j, k)) && !GameController.Instance.areaMapMetaLayer.IsPlayed(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(j, k)) && !GameController.Instance.areaMapMetaLayer.IsLocked(j, k)) || GameController.Instance.areaMapMetaLayer.IsDiscoverable(j, k)) && !GameController.Instance.areaMapMetaLayer.IsLockedUntilChapter(j, k))
				{
					if (!GameController.Instance.areaMapMetaLayer.NewlyUnlockedLevels.Contains(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(j, k)))
					{
						GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Unlocked"));
						icons.Add(gameObject2);
						gameObject2.transform.SetParent(iconContainer.transform);
						Image componentInChildren = gameObject2.GetComponentInChildren<Image>();
						componentInChildren.rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
						gameObject2.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * k) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * j) + (float)tileHeight * 0.5f);
					}
					_numUnlockedLevels++;
				}
				if (GameController.Instance.areaMapMetaLayer.IsPlayed(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(j, k)))
				{
					GameObject gameObject3 = null;
					switch (GameController.Instance.areaMapMetaLayer.GetNumStars(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(j, k)))
					{
					case 3:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Played3Star"));
						break;
					case 2:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Played2Star"));
						break;
					case 1:
						gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Played1Star"));
						break;
					default:
						Debug.LogError("Wrong num stars returned from PlayerProfile...");
						break;
					}
					if (gameObject3 != null)
					{
						icons.Add(gameObject3);
						gameObject3.transform.SetParent(iconContainer.transform);
						Image componentInChildren2 = gameObject3.GetComponentInChildren<Image>();
						componentInChildren2.rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
						gameObject3.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * k) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * j) + (float)tileHeight * 0.5f);
					}
				}
				if (GameController.Instance.areaMapMetaLayer.IsLocked(j, k))
				{
					GameObject gameObject4 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Locked"));
					icons.Add(gameObject4);
					gameObject4.transform.parent = iconContainer.transform;
					Image componentInChildren3 = gameObject4.GetComponentInChildren<Image>();
					componentInChildren3.rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
					gameObject4.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * k) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * j) + (float)tileHeight * 0.5f);
				}
			}
		}
		ShowPlayerIcon();
		isInitialized = true;
		GameAudioManager.Instance.StopAmbience(true, 2f);
		StartCoroutine(DisplayNewlyUnlockedLevels());
	}

	private IEnumerator DisplayNewlyUnlockedLevels()
	{
		List<LevelPoint> pointList = new List<LevelPoint>();
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (GameController.Instance.ObjectiveController.CurrentObjective != null && (!GameController.Instance.areaMapMetaLayer.IsLockedUntilChapter(i, j) || GameController.Instance.areaMapMetaLayer.IsDiscoverable(i, j)) && GameController.Instance.areaMapMetaLayer.NewlyUnlockedLevels.Contains(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(i, j)))
				{
					pointList.Add(new LevelPoint(i, j));
				}
			}
		}
		foreach (LevelPoint point in pointList)
		{
			yield return new WaitForSeconds(newItemsInterval);
			if (!GameController.Instance.areaMapMetaLayer.IsDiscoverable(point.row, point.col))
			{
				AudioPlayer.PlaySound("ui/area/map/symbol/appear");
				GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Unlocked"));
				icons.Add(gameObject);
				gameObject.transform.parent = iconContainer.transform;
				Image componentInChildren = gameObject.GetComponentInChildren<Image>();
				componentInChildren.rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
				gameObject.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * point.col) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * point.row) + (float)tileHeight * 0.5f);
			}
			GameController.Instance.areaMapMetaLayer.NewlyUnlockedLevels.Remove(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(point.row, point.col));
			fowImage.texture = GenerateFogOfWarTexture();
		}
		GameController.Instance.areaMapMetaLayer.NewlyUnlockedLevels.Clear();
		StartCoroutine(DisplayObjectives());
	}

	private IEnumerator DisplayObjectives()
	{
		yield return new WaitForSeconds(newItemsInterval);
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (GameController.Instance.ObjectiveController.CurrentObjective == null)
				{
					continue;
				}
				int seedFromPosition = GetSeedFromPosition(new Vector2(lowerLeftCorner.x + (float)(tileWidth * j) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * i) + (float)tileHeight * 0.5f));
				ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
				foreach (ObjectiveLine objectiveLine in lines)
				{
					if (!objectiveLine.Completed && objectiveLine.seed == seedFromPosition)
					{
						GameObject gameObject = ((objectiveLine.patternID == ObjectivePatternID.RescueCole) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveCole"))) : ((objectiveLine.patternID == ObjectivePatternID.RescueZane) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveZane"))) : ((objectiveLine.patternID == ObjectivePatternID.RescueKai) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveKai"))) : ((objectiveLine.patternID == ObjectivePatternID.RescueJay) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveJay"))) : ((objectiveLine.patternID == ObjectivePatternID.FindTheHoloLog) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveLloyd"))) : ((objectiveLine.patternID == ObjectivePatternID.FindTheBiteBike) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveVehicle"))) : ((objectiveLine.patternID == ObjectivePatternID.SamuraiXCave) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveNya"))) : ((objectiveLine.patternID == ObjectivePatternID.RescueNyah) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveNya"))) : ((objectiveLine.patternID == ObjectivePatternID.ConstrictaiBoss) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveTomb_Constrictai"))) : ((objectiveLine.patternID == ObjectivePatternID.VenomariBoss) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveTomb_Venomari"))) : ((objectiveLine.patternID == ObjectivePatternID.FangpyreBoss) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveTomb_Fangpyre"))) : ((objectiveLine.patternID == ObjectivePatternID.HypnobraiBoss) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveTomb_Hypnobrai"))) : ((objectiveLine.patternID == ObjectivePatternID.FangpyreBoss) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveTomb_Fangpyre"))) : ((objectiveLine.patternID != ObjectivePatternID.Oroborous) ? ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Objective"))) : ((GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/ObjectiveOroborous")))))))))))))))));
						icons.Add(gameObject);
						gameObject.transform.SetParent(iconContainer.transform);
						Image componentInChildren = gameObject.GetComponentInChildren<Image>();
						componentInChildren.rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
						gameObject.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * j) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * i) + (float)tileHeight * 0.5f);
						AudioPlayer.PlaySound("ui/button/sword/shing");
					}
				}
				GameObject areaMapPointer = GameController.Instance.LevelExtensionController.GetAreaMapPointer(seedFromPosition);
				if (areaMapPointer != null)
				{
					GameObject gameObject2 = Object.Instantiate(areaMapPointer);
					AreaMapIconStoneWarrior component = gameObject2.GetComponent<AreaMapIconStoneWarrior>();
					component.Seed = seedFromPosition;
					icons.Add(gameObject2);
					gameObject2.transform.SetParent(iconContainer.transform);
					Image componentInChildren2 = gameObject2.GetComponentInChildren<Image>();
					componentInChildren2.rectTransform.sizeDelta = new Vector2((float)tileWidth * 1.8f, (float)tileHeight * 1.8f);
					gameObject2.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * j) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * i) + (float)tileHeight * 0.5f);
				}
			}
		}
		StartCoroutine(DisplayWuButton());
	}

	private IEnumerator DisplayWuButton()
	{
		yield return new WaitForSeconds(1.5f);
		if (wuReminderButton == null)
		{
			Debug.LogError("Missing Wu Button");
		}
		else
		{
			wuReminderButton.gameObject.SetActive(true);
		}
		StartCoroutine(DisplayWuSpeechBubble());
	}

	private IEnumerator DisplayWuSpeechBubble()
	{
		yield return new WaitForSeconds(1.5f);
		if (wuSpeechBubble != null)
		{
			wuSpeechBubble.gameObject.SetActive(true);
			wuText.gameObject.SetActive(true);
		}
		else
		{
			Debug.LogWarning("GameControllerStateAreaMap: wuSpeechBubble is null during DisplayWuSpeechBubble()");
		}
		playerIconGO.transform.SetAsLastSibling();
	}

	public override void UpdateState()
	{
		if (Input.GetMouseButtonDown(0) && isInitialized && !isConfirmingPlay && !isPreparingPlay && GamePlayer.Instance == null && !GUIMonitor.Instance.HasType(GUITypeFlag.Settings))
		{
			AreaMapNode pickingNode = GetPickingNode();
			int pickingSeed = GetPickingSeed();
			if (pickingNode != null && !GameController.Instance.areaMapMetaLayer.IsLockedUntilChapter(pickingNode.row, pickingNode.col) && GameController.Instance.areaMapMetaLayer.IsUnlocked(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(pickingNode.row, pickingNode.col)) && !GameController.Instance.areaMapMetaLayer.IsBlocked(pickingNode.row, pickingNode.col) && !GameController.Instance.areaMapMetaLayer.IsLocked(pickingNode.row, pickingNode.col) && !GameController.Instance.areaMapMetaLayer.IsDiscoverable(pickingNode.row, pickingNode.col))
			{
				ConfirmPlayLevel(pickingSeed, pickingNode);
			}
			else if (pickingNode != null && GameController.Instance.areaMapMetaLayer.IsDiscoverable(pickingNode.row, pickingNode.col) && GameController.Instance.areaMapMetaLayer.IsUnlocked(pickingSeed))
			{
				GameController.Instance.PlayerProfile.DiscoverLevel(pickingSeed);
				GameController.Instance.ObjectiveController.CompleteAll();
				GameController.Instance.PlayerProfile.Save();
				SceneManager.LoadScene("TeaFarm");
			}
		}
	}

	private void ConfirmPlayLevel(int seed, AreaMapNode node)
	{
		isConfirmingPlay = true;
		seedToPlay = seed;
		nodeToPlay = node;
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/ConfirmPlayGUI"));
		ConfirmPlayGUI component = gameObject.GetComponent<ConfirmPlayGUI>();
		component.onOKAction = ConfirmOK;
		component.onCancelAction = ConfirmCancel;
	}

	public void ConfirmOK()
	{
		isConfirmingPlay = false;
		AudioPlayer.StopSound("music/area/map");
		StopAllCoroutines();
		StartCoroutine(HandlePlayLevel(seedToPlay, nodeToPlay));
	}

	public void ConfirmCancel()
	{
		isConfirmingPlay = false;
	}

	private IEnumerator HandlePlayLevel(int seed, AreaMapNode node)
	{
		isPreparingPlay = true;
		wuReminderButton.transform.parent.gameObject.SetActive(false);
		foreach (GameObject icon in icons)
		{
			Object.Destroy(icon);
		}
		Vector3 mapScale = mapImage.transform.localScale;
		float startTime = Time.time;
		float duration = 0.6f;
		Vector3 appliedScale = mapScale;
		float appliedAlpha = 1f;
		float t = 0f;
		GameAudioManager.Instance.StopAllMusic(false);
		GameAudioManager.Instance.StopAmbience(true, 1f);
		while (t < 1f)
		{
			t = (Time.time - startTime) / duration;
			appliedScale = Vector3.Lerp(mapScale, new Vector3(5f, 5f, 5f), t);
			appliedAlpha = Mathf.Lerp(1f, 0f, t);
			areaMap.map.transform.localScale = appliedScale;
			mapImage.material.color = new Color(appliedAlpha, appliedAlpha, appliedAlpha, appliedAlpha);
			if (showAddon)
			{
				addonImage.material.color = new Color(appliedAlpha, appliedAlpha, appliedAlpha, appliedAlpha);
			}
			gridImage.material.color = new Color(appliedAlpha, appliedAlpha, appliedAlpha, appliedAlpha);
			cloudImage.material.color = new Color(appliedAlpha, appliedAlpha, appliedAlpha, appliedAlpha);
			fxImage.material.color = new Color(appliedAlpha, appliedAlpha, appliedAlpha, appliedAlpha);
			yield return null;
		}
		EventManager.Instance.PostEvent("music", EventAction.UnloadAudio);
		EventManager.Instance.PostEvent("ambience", EventAction.UnloadAudio);
		Object loadScreen = Object.Instantiate(Resources.Load("Prefabs/Dialogs/InGameLoadScreen"));
		Object.DontDestroyOnLoad(loadScreen);
		GameController.Instance.PlayerProfile.LastPlayedSeason = GameController.Instance.areaMapMetaLayer.season;
		yield return null;
		GameController.Instance.styleIndex = node.styleIndex;
		GameController.Instance.themeCollectionIndex = node.themeCollectionIndex;
		GameController.Instance.seed = seed;
		GameController.Instance.areaMapMetaLayer.currentRow = node.row;
		GameController.Instance.areaMapMetaLayer.currentCol = node.col;
		GameController.Instance.PlayerProfile.SetCurrentAreaPosition(node.row, node.col);
		GameController.Instance.PlayerProfile.Save();
		SceneManager.LoadScene("LevelPlayerNew");
		yield return 0;
	}

	private Texture2D GenerateFogOfWarTexture()
	{
		float time = Time.time;
		int num = cols;
		int num2 = rows;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.ARGB32, false);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				int num3 = j;
				int num4 = i;
				if (num3 >= rows)
				{
					num3 = rows - 1;
				}
				if (num4 >= cols)
				{
					num4 = cols - 1;
				}
				if (GameController.Instance.areaMapMetaLayer.IsUnlocked(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(num3, num4)) && !GameController.Instance.areaMapMetaLayer.NewlyUnlockedLevels.Contains(GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(num3, num4)) && !GameController.Instance.areaMapMetaLayer.IsLocked(num3, num4) && !GameController.Instance.areaMapMetaLayer.IsLockedUntilChapter(num3, num4))
				{
					texture2D.SetPixel(i, j, new Color(1f, 1f, 1f, 0f));
				}
				else
				{
					texture2D.SetPixel(i, j, new Color(0f, 0f, 0f, 0.6f));
				}
			}
		}
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		return texture2D;
	}

	private void ShowPlayerIcon()
	{
		playerIconGO = (GameObject)Object.Instantiate(Resources.Load("Prefabs/AreaMapIcons/Player"));
		icons.Add(playerIconGO);
		playerIconGO.transform.SetParent(iconContainer.transform, false);
		Image componentInChildren = playerIconGO.GetComponentInChildren<Image>();
		NinjaPortrait componentInChildren2 = playerIconGO.GetComponentInChildren<NinjaPortrait>();
		int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(0);
		Texture2D[] array = CharacterPreviewRenderer.Generate(new int[1] { ninjaInTeam }, (int)componentInChildren2.portrait.rectTransform.sizeDelta.x, (int)componentInChildren2.portrait.rectTransform.sizeDelta.y);
		Texture portraitTexture = array[0];
		componentInChildren2.Configure(ninjaInTeam, portraitTexture);
		int num = GameController.Instance.areaMapMetaLayer.currentRow;
		int num2 = GameController.Instance.areaMapMetaLayer.currentCol;
		if (_numUnlockedLevels <= 1)
		{
			num = GameController.Instance.areaMapMetaLayer.startRow;
			num2 = GameController.Instance.areaMapMetaLayer.startCol;
		}
		playerIconGO.transform.position = new Vector3(lowerLeftCorner.x + (float)(tileWidth * num2) + (float)tileWidth * 0.5f, lowerLeftCorner.y + (float)(tileHeight * num) + (float)tileHeight * 0.5f);
	}

	private AreaMapNode GetPickingNode()
	{
		int num = Mathf.FloorToInt((Input.mousePosition.x - lowerLeftCorner.x) / (float)tileWidth);
		int num2 = Mathf.FloorToInt((Input.mousePosition.y - lowerLeftCorner.y) / (float)tileHeight);
		if (num >= 0 && num < cols && num2 >= 0 && num2 < rows)
		{
			return GameController.Instance.areaMapMetaLayer.GetNodeAt(num2, num);
		}
		return null;
	}

	private int GetPickingSeed()
	{
		return GetSeedFromPosition(Input.mousePosition);
	}

	private int GetSeedFromPosition(Vector3 position)
	{
		int col = Mathf.FloorToInt((position.x - lowerLeftCorner.x) / (float)tileWidth);
		int row = Mathf.FloorToInt((position.y - lowerLeftCorner.y) / (float)tileHeight);
		return GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(row, col);
	}
}
