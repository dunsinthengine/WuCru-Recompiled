using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Fabric;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerStatePlaying : GameControllerState
{
	public const float AMBIENT_MULTIPLIER = 0.25f;

	private GameObject inGameLoadScreenRoot;

	private GameObject inGameGUI;

	private InGameGUIController inGameGUIController;

	private GameObject gamePlayerGO;

	private bool hasEndedLevel;

	private LevelCompletionManager completionMgr;

	private int environmentSettingsIndex = -1;

	private int spinjitzuCount;

	private bool isConfirmingQuit;

	private int currentNinjaID;

	private float fadeTime = 1.2f;

	private GameObject blockerGO;

	private Dictionary<int, float> ninjaTeamHealth = new Dictionary<int, float>();

	private Dictionary<int, HealthBarNinjaTeam> ninjaTeamHealtBars = new Dictionary<int, HealthBarNinjaTeam>();

	private Dictionary<int, int> ninjaTeamMissiles = new Dictionary<int, int>();

	private float ninjaMana;

	public bool IsConfirmingQuit
	{
		get
		{
			return isConfirmingQuit;
		}
	}

	public LevelCompletionManager CompletionMgr
	{
		get
		{
			return completionMgr;
		}
	}

	public int SpinjitzuCount
	{
		get
		{
			return spinjitzuCount;
		}
		set
		{
			spinjitzuCount = value;
		}
	}

	public int CurrentNinjaID
	{
		get
		{
			return currentNinjaID;
		}
		set
		{
			currentNinjaID = value;
		}
	}

	public InGameGUIController InGameGUIController
	{
		get
		{
			return inGameGUIController;
		}
	}

	public bool FadeInProgress { get; private set; }

	public override void BeginState()
	{
		Resources.UnloadUnusedAssets();
		CharacterBuilder.CleanupInGameCharacterTextures();
		CharacterBuilder.CleanupMenuCharacterTextures();
		inGameLoadScreenRoot = null;
		inGameGUI = null;
		inGameGUIController = null;
		gamePlayerGO = null;
		hasEndedLevel = false;
		isConfirmingQuit = false;
		spinjitzuCount = PlayerProfile.SpinjitzuCountForLevel(GameController.Instance.PlayerProfile.SpinjitzuLevel);
		currentNinjaID = 0;
		StartLevel();
		inGameGUI = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/InGameGUI"));
		inGameGUIController = inGameGUI.GetComponent<InGameGUIController>();
		int num = 0;
		int[] ninjaTeam = GameController.Instance.PlayerProfile.NinjaTeam;
		foreach (int num2 in ninjaTeam)
		{
			if (num2 != -1)
			{
				MinifigDefinition minifigDefinition = ((num2 != 0) ? CharacterBuilder.minifigDefinitions[num2] : GameController.Instance.PlayerProfile.GetMinifigDefinition());
				if (!ninjaTeamHealth.ContainsKey(num2))
				{
					ninjaTeamHealth.Add(num2, 1f);
				}
				else
				{
					ninjaTeamHealth[num2] = 1f;
				}
				if (!ninjaTeamHealtBars.ContainsKey(num2))
				{
				}
				if (!ninjaTeamMissiles.ContainsKey(num2))
				{
					ninjaTeamMissiles.Add(num2, (minifigDefinition.weapon == WeaponType.Shuriken) ? 2 : 0);
				}
				else
				{
					ninjaTeamMissiles[num2] = ((minifigDefinition.weapon == WeaponType.Shuriken) ? 2 : 0);
				}
			}
			num++;
		}
		GameController.Instance.MissionStats.deadNinjaIDs.Clear();
		LevelChallengeBase challenge = GameController.Instance.CurrentLevel.Challenge;
		challenge.OnCompletion = (LevelChallengeBase.OnCompletionDelegate)Delegate.Combine(challenge.OnCompletion, new LevelChallengeBase.OnCompletionDelegate(OnChallengeCompletion));
		GameController instance = GameController.Instance;
		instance.OnReachFlag = (GameController.OnReachFlagDelegate)Delegate.Combine(instance.OnReachFlag, new GameController.OnReachFlagDelegate(GameController.Instance.CurrentLevel.Challenge.OnEndFlagReached));
		ScreenFadeFX component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
		component.OnDone = (ScreenFadeFX.OnDoneDelegate)Delegate.Combine(component.OnDone, new ScreenFadeFX.OnDoneDelegate(OnFadeDone));
		FadeInProgress = true;
		component.FadeIn(fadeTime);
		ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
		ScreenLevelLoading screenInstance = instance2.GetScreenInstance<ScreenLevelLoading>();
		ScreenResultsCard._acquiredCard = ECOLLECTABLE_CARD.INVALID;
		screenInstance.OnLevelReady();
	}

	public override void EndState()
	{
		if (!ScreenResults._pInResults)
		{
			Time.timeScale = 1f;
		}
	}

	public override void UpdateState()
	{
		if (!hasEndedLevel)
		{
			if (GameController.Instance.CurrentLevel != null && GameController.Instance.CurrentLevel.Challenge != null)
			{
				GameController.Instance.CurrentLevel.Challenge.Update();
			}
			if (GamePlayer.Instance.Level.LevelTheme.index == GamePlayer.Instance.Level.LevelThemeCollection.themes.Count - 1 && completionMgr.HasCompleted())
			{
				OnLevelCompleted();
			}
		}
	}

	public void OnChallengeCompletion(LevelChallengeBase challenge)
	{
		Debug.Log("CHALLENGE COMPLETED: " + challenge.GetType().ToString());
	}

	public void OnIngameObjectiveReached(bool moveCam = true)
	{
		AudioPlayer.PlaySound("fx/sub/goal/reached");
		if (moveCam)
		{
			InGameCam pInstance = InGameCam._pInstance;
			pInstance.Zoom(6f, 2f, 1f);
		}
	}

	public void SetNinjaHealth(int ninjaID, float healthFactor)
	{
		if (ninjaTeamHealth.ContainsKey(ninjaID))
		{
			Debug.Log("SetNinjaHealth(" + ninjaID + "/" + healthFactor + ")");
			ninjaTeamHealth[ninjaID] = healthFactor;
		}
		else
		{
			Debug.LogError("Cannot set ninja health, as ninja is not in team!");
		}
	}

	public float GetNinjaHealthFactor(int ninjaID)
	{
		if (!ninjaTeamHealth.ContainsKey(ninjaID))
		{
			return 0f;
		}
		return ninjaTeamHealth[ninjaID];
	}

	public float GetNinjaMana()
	{
		return ninjaMana;
	}

	public void SetNinjaMana(float value)
	{
		ninjaMana = value;
	}

	public int GetNinjaMissilesLeft(int ninjaID)
	{
		if (!ninjaTeamMissiles.ContainsKey(ninjaID))
		{
			return 0;
		}
		return ninjaTeamMissiles[ninjaID];
	}

	public void SetNinjaMissilesLeft(int ninjaID, int value)
	{
		if (ninjaTeamMissiles.ContainsKey(ninjaID))
		{
			ninjaTeamMissiles[ninjaID] = value;
		}
	}

	public void OnCharacterReachEndFlag()
	{
		if (GameController.Instance.OnReachFlag != null)
		{
			GameController.Instance.OnReachFlag(GameController.Instance.CurrentLevel);
		}
		if (GamePlayer.Instance.Level.LevelTheme.index < GamePlayer.Instance.Level.LevelThemeCollection.themes.Count - 1)
		{
			OnLevelCompleted();
		}
	}

	public void OnQuitPlaying()
	{
		Debug.Log("OnQuitPlaying - InGamePauseGUI");
		isConfirmingQuit = true;
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenPause screenInstance = instance.GetScreenInstance<ScreenPause>();
		screenInstance._onContinueAction = (Action)Delegate.Combine(screenInstance._onContinueAction, new Action(CancelQuit));
		screenInstance._onMapAction = (Action)Delegate.Combine(screenInstance._onMapAction, new Action(ConfirmQuit));
		instance.ShowPopupOverlay(screenInstance);
	}

	public void ConfirmQuit()
	{
		Debug.Log("Confirm Quit");
		if (GameController.Instance.MissionController.HasActiveMission())
		{
			GameController.Instance.MissionController.ResetMission();
			string missionID = GameController.Instance.MissionController.CurrentMission.MissionID;
			AnalyticsEvents.LogWuCruMissionEnded(missionID, false, GameController.Instance.CurrentLevel.Seed);
		}
		Time.timeScale = 1f;
		currentNinjaID = 0;
		GameController.Instance.PlayerProfile.LoadLocal();
		GameController.Instance.LevelExtensionController.LoadFromPlayerProfile();
		DoQuit();
	}

	private void DoQuit()
	{
		LevelChallengeBase challenge = GameController.Instance.CurrentLevel.Challenge;
		challenge.OnCompletion = (LevelChallengeBase.OnCompletionDelegate)Delegate.Remove(challenge.OnCompletion, new LevelChallengeBase.OnCompletionDelegate(OnChallengeCompletion));
		GameController instance = GameController.Instance;
		instance.OnReachFlag = (GameController.OnReachFlagDelegate)Delegate.Remove(instance.OnReachFlag, new GameController.OnReachFlagDelegate(GameController.Instance.CurrentLevel.Challenge.OnEndFlagReached));
		FadeInProgress = false;
		CharacterBuilder.CleanupInGameCharacterTextures();
		CharacterBuilder.CleanupMenuCharacterTextures();
		UnityEngine.Object.Destroy(GameController.Instance.LevelVisualizer.levelRoot);
		UnityEngine.Object.Destroy(GameController.Instance.LevelVisualizer.LevelMeshGO);
		GameController.Instance.CurrentLevel = null;
		Resources.UnloadUnusedAssets();
		GamePlayer.Instance.SetState(GamePlayerState.Idle);
		isConfirmingQuit = false;
		AudioPlayer.MoveFabricAudioListener(GamePlayer.Instance.Player.Character.gameObject, FabricManager.Instance.gameObject);
		GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
	}

	public void CancelQuit()
	{
		InGameGUIController.EnableButtons();
		Time.timeScale = 1f;
		isConfirmingQuit = false;
	}

	public void OnLevelCompleted()
	{
		GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.missionAccomplished);
		StudsCounter componentInChildren = inGameGUIController.GetComponentInChildren<StudsCounter>();
		GameController.Instance.MissionStats.NumStudsCollected = ((componentInChildren != null) ? componentInChildren.StudsCollected : 0);
		LevelChallengeBase challenge = GameController.Instance.CurrentLevel.Challenge;
		challenge.OnCompletion = (LevelChallengeBase.OnCompletionDelegate)Delegate.Remove(challenge.OnCompletion, new LevelChallengeBase.OnCompletionDelegate(OnChallengeCompletion));
		GameController instance = GameController.Instance;
		instance.OnReachFlag = (GameController.OnReachFlagDelegate)Delegate.Remove(instance.OnReachFlag, new GameController.OnReachFlagDelegate(GameController.Instance.CurrentLevel.Challenge.OnEndFlagReached));
		if (GamePlayer.Instance.Level.LevelTheme.index == GamePlayer.Instance.Level.LevelThemeCollection.themes.Count - 1)
		{
			AnalyticsEvents.LogEndLevel(true);
			EndLevel();
			return;
		}
		ScreenFadeFX component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
		FadeInProgress = true;
		component.OnDone = OnFadeOutDone;
		component.FadeOut(fadeTime);
	}

	public void OnPlayerDead()
	{
		GameController.Instance.MissionStats.ReportPlayerDead();
		NinjaTeamGUI componentInChildren = inGameGUI.GetComponentInChildren<NinjaTeamGUI>(true);
		if (componentInChildren != null)
		{
			componentInChildren.HideSlotWithNinjaID(GamePlayer.Instance.Player.Character.NinjaID);
		}
		bool flag = false;
		int[] ninjaTeam = GameController.Instance.PlayerProfile.NinjaTeam;
		foreach (int item in ninjaTeam)
		{
			if (!GameController.Instance.MissionStats.deadNinjaIDs.Contains(item))
			{
				flag = true;
			}
		}
		if (flag)
		{
			StartCoroutine(DelayedPlayerRespawn());
			return;
		}
		AnalyticsEvents.LogEndLevel(false);
		EndLevel(false);
	}

	private IEnumerator DelayedPlayerRespawn()
	{
		GamePlayer.Instance.SetTimeScale(1f, 0.1f);
		yield return new WaitForSeconds(3.5f);
		GamePlayer.Instance.Player.Character.Respawn();
	}

	private void OnFadeOutDone()
	{
		FadeInProgress = false;
		AudioPlayer.MoveFabricAudioListener(GamePlayer.Instance.Player.Character.gameObject, FabricManager.Instance.gameObject);
		blockerGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenBlockerFX"));
		UnityEngine.Object.DontDestroyOnLoad(blockerGO);
		UnityEngine.Object.Destroy(inGameGUI);
		StartCoroutine(DoLoadScene());
	}

	private IEnumerator DoLoadScene()
	{
		SceneManager.LoadScene("EmptyLoadScene");
		yield return 0;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		yield return 0;
		SceneManager.LoadScene("LevelPlayerNew");
	}

	public void DelayedLoadNextSequence()
	{
		CharacterBuilder.CleanupInGameCharacterTextures();
		CharacterBuilder.CleanupMenuCharacterTextures();
		inGameGUI = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/InGameGUI"));
		inGameGUIController = inGameGUI.GetComponent<InGameGUIController>();
		inGameGUIController.GetComponentInChildren<StudsCounter>().SetAmount(GameController.Instance.MissionStats.NumStudsCollected);
		inGameGUIController.ShowGUI(false);
		NinjaTeamGUI component = inGameGUIController.ninjaTeamRoot.GetComponent<NinjaTeamGUI>();
		int num = 0;
		int[] ninjaTeam = GameController.Instance.PlayerProfile.NinjaTeam;
		foreach (int num2 in ninjaTeam)
		{
			if (num2 != -1)
			{
				if (!ninjaTeamHealth.ContainsKey(num2))
				{
					ninjaTeamHealth.Add(num2, 1f);
				}
				if (!ninjaTeamHealtBars.ContainsKey(num2))
				{
					if (num == 0)
					{
						ninjaTeamHealtBars.Add(num2, component.portraits[1].health);
					}
					if (num == 1)
					{
						ninjaTeamHealtBars.Add(num2, component.portraits[2].health);
					}
					if (num == 2)
					{
						ninjaTeamHealtBars.Add(num2, component.portraits[3].health);
					}
					if (num == 3)
					{
						ninjaTeamHealtBars.Add(num2, component.portraits[4].health);
					}
				}
				else
				{
					ninjaTeamHealtBars[num2].UpdateHealth(Mathf.RoundToInt((GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaHealthFactor(num2) * (float)GamePlayer.Instance.Player.Character.MaxHealth));
				}
			}
			num++;
		}
		StartCoroutine(DoDelayedLoadNextSequence());
	}

	private IEnumerator DoDelayedLoadNextSequence()
	{
		inGameLoadScreenRoot = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/InGameLoadScreen"));
		yield return 0;
		LoadNextSequence();
		Character character = GamePlayer.Instance.Player.Character;
		character.onMissionAccomplished = (Character.OnMissionAccomplishedDelegate)Delegate.Combine(character.onMissionAccomplished, new Character.OnMissionAccomplishedDelegate(OnCharacterReachEndFlag));
		Character character2 = GamePlayer.Instance.Player.Character;
		character2.onPlayerDead = (Character.OnPlayerDeadDelegate)Delegate.Combine(character2.onPlayerDead, new Character.OnPlayerDeadDelegate(OnPlayerDead));
		completionMgr.SetupLevelObjectSubscribers();
		LevelChallengeBase challenge = GameController.Instance.CurrentLevel.Challenge;
		challenge.OnCompletion = (LevelChallengeBase.OnCompletionDelegate)Delegate.Combine(challenge.OnCompletion, new LevelChallengeBase.OnCompletionDelegate(OnChallengeCompletion));
		GameController instance = GameController.Instance;
		instance.OnReachFlag = (GameController.OnReachFlagDelegate)Delegate.Combine(instance.OnReachFlag, new GameController.OnReachFlagDelegate(GameController.Instance.CurrentLevel.Challenge.OnEndFlagReached));
		yield return 0;
		UnityEngine.Object.Destroy(inGameLoadScreenRoot);
		inGameLoadScreenRoot = null;
		UnityEngine.Object.Destroy(blockerGO);
		ScreenFadeFX fadeFX = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
		fadeFX.OnDone = (ScreenFadeFX.OnDoneDelegate)Delegate.Combine(fadeFX.OnDone, new ScreenFadeFX.OnDoneDelegate(OnFadeDone));
		FadeInProgress = true;
		fadeFX.FadeIn(fadeTime);
		inGameGUIController.ShowGUI(true);
	}

	private void OnFadeDone()
	{
		FadeInProgress = false;
	}

	private void StartLevel()
	{
		GameController.Instance._collectedClothingPart = false;
		GameController.Instance._collectedVehiclePart = false;
		LevelEngine levelEngine = new LevelEngine();
		LevelStyle style = UnityEngine.Object.Instantiate(GameController.Instance.StyleDB._styles[GameController.Instance.StyleIndex]);
		LevelExtension levelExtension = GameController.Instance.LevelExtensionController.GetLevelExtension(GameController.Instance.Seed);
		if (GameController.Instance.PlayerProfile.NinjaTeam.Length <= 1)
		{
			levelExtension = null;
		}
		if (levelExtension != null)
		{
			levelExtension.OverwriteStyleValuesOn(style);
		}
		GameController.Instance.CurrentLevel = levelEngine.GenerateLevel(GameController.Instance.ThemeDB.themeCollections.values[GameController.Instance.ThemeCollectionIndex], GameController.Instance.ThemeDB.themeCollections.values[GameController.Instance.ThemeCollectionIndex].themes.values[0], style, GameController.Instance.Seed, GameController.Instance.areaMapMetaLayer.GetPrimaryPowerForPosition(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol), GameController.Instance.areaMapMetaLayer.GetSecondaryPowerForPosition(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol));
		if (levelExtension != null)
		{
			levelExtension.DoLevelAlternations(GameController.Instance.CurrentLevel);
		}
		UnityEngine.Object reference = GUIMonitor.Instance.GetReference(GUITypeFlag.LoadScreen);
		UnityEngine.Object.Destroy(reference);
		if (gamePlayerGO != null)
		{
			UnityEngine.Object.Destroy(gamePlayerGO);
		}
		gamePlayerGO = new GameObject();
		gamePlayerGO.name = "GamePlayer";
		GamePlayer gamePlayer = gamePlayerGO.AddComponent<GamePlayer>();
		gamePlayer.Initialize(GameController.Instance.CurrentLevel);
		GameController.Instance.LevelVisualizer.BuildLevel(GameController.Instance.CurrentLevel, style);
		GamePlayer.Instance.Reset();
		Character character = GamePlayer.Instance.Player.Character;
		character.onMissionAccomplished = (Character.OnMissionAccomplishedDelegate)Delegate.Combine(character.onMissionAccomplished, new Character.OnMissionAccomplishedDelegate(OnCharacterReachEndFlag));
		Character character2 = GamePlayer.Instance.Player.Character;
		character2.onPlayerDead = (Character.OnPlayerDeadDelegate)Delegate.Combine(character2.onPlayerDead, new Character.OnPlayerDeadDelegate(OnPlayerDead));
		if (completionMgr != null)
		{
			UnityEngine.Object.Destroy(completionMgr);
		}
		completionMgr = base.gameObject.AddComponent<LevelCompletionManager>();
		AudioPlayer.MoveFabricAudioListener(FabricManager.Instance.gameObject, GamePlayer.Instance.Player.Character.gameObject);
		AnalyticsEvents.LogStartLevel(0);
	}

	private void LoadNextSequence()
	{
		StartCoroutine(DoLoadNextSequence());
	}

	public void RevertEnvironmentSettings()
	{
		Debug.Log("RevertEnvironmentSettings");
		if (GamePlayer.Instance.ScreenFlash != null)
		{
			GamePlayer.Instance.ScreenFlash.FlashAlpha(0.8f, 1.5f, new Color(1f, 138f / 255f, 0f, 1f));
		}
		AudioPlayer.PlaySound("fx/sub/goal/reached");
		EnvironmentSetting environmentSetting = GameController.Instance.LevelVisualizer.environmentSetting;
		List<GameObject> environmentSettings = GameController.Instance.StyleDB._styles[GameController.Instance.StyleIndex].environmentSettings;
		EnvironmentSetting component = environmentSettings[0].GetComponent<EnvironmentSetting>();
		float num = 10f;
		StartCoroutine(AnimateEnvironmentSettings(environmentSetting, component, num));
		LevelStyle levelStyle = GameController.Instance.CurrentLevel.LevelStyle;
		LevelStyle to = UnityEngine.Object.Instantiate(GameController.Instance.StyleDB._styles[GameController.Instance.StyleIndex]);
		StartCoroutine(CrossFadeStyleMusic(levelStyle, to));
		GameObject replaceLampGameObject = GameController.Instance.LevelExtensionController.GetReplaceLampGameObject(GameController.Instance.CurrentLevel.Seed);
		if ((bool)replaceLampGameObject)
		{
			StartCoroutine(ReplaceLampPosts(GameController.Instance.CurrentLevel.GetNodesWithObject(LevelNodeObjectType.LampPost), replaceLampGameObject, num * 0.6f));
		}
		List<GameObject> replaceDecorationForegrounds = GameController.Instance.LevelExtensionController.GetReplaceDecorationForegrounds(GameController.Instance.CurrentLevel.Seed);
		if (replaceDecorationForegrounds.Count > 0)
		{
			StartCoroutine(ReplaceForegroundDecorations(GameController.Instance.LevelVisualizer.decorationForegrounds, replaceDecorationForegrounds, num * 0.6f));
		}
	}

	private IEnumerator AnimateEnvironmentSettings(EnvironmentSetting from, EnvironmentSetting to, float totalTime)
	{
		float startTime = Time.realtimeSinceStartup;
		NinjagoFog ninjagoFog = UnityEngine.Object.FindObjectOfType<NinjagoFog>();
		OcclusionRenderer occlusionRenderer = UnityEngine.Object.FindObjectOfType<OcclusionRenderer>();
		float progress = 0f;
		while (progress < 1f)
		{
			progress = Easing.QuadraticEaseOut(Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / totalTime));
			RenderSettings.ambientLight = Color.Lerp(from.ambientLight, to.ambientLight, progress) * 0.25f;
			RenderSettings.ambientIntensity = Mathf.Lerp(from.ambientIntensity, to.ambientIntensity, progress);
			RenderSettings.fogColor = Color.Lerp(from.fogColor, to.fogColor, progress);
			RenderSettings.fogDensity = Mathf.Lerp(from.fogDensity, to.fogDensity, progress);
			RenderSettings.fogStartDistance = Mathf.Lerp(from.fogStartDistance, to.fogStartDistance, progress);
			RenderSettings.fogEndDistance = Mathf.Lerp(from.fogEndDistance, to.fogEndDistance, progress);
			Camera.main.backgroundColor = Color.Lerp(from.fogColor, to.fogColor, progress);
			if ((bool)ninjagoFog)
			{
				ninjagoFog.heightStart = Mathf.Lerp(from.fogHeightStart, to.fogHeightStart, progress);
				ninjagoFog.heightEnd = Mathf.Lerp(from.fogHeightEnd, to.fogHeightEnd, progress);
			}
			if ((bool)occlusionRenderer)
			{
				occlusionRenderer.SetOccludedColor(Color.Lerp(from.occlusionColor, to.occlusionColor, progress));
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator CrossFadeStyleMusic(LevelStyle from, LevelStyle to)
	{
		AudioPlayer.StopSound(from.AmbienceAudioEventName);
		AudioPlayer.StopSound(from.MusicAudioEventName);
		AudioPlayer.PlaySound(to.AmbienceAudioEventName);
		AudioPlayer.PlaySound(to.MusicAudioEventName);
		switch (GamePlayer.Instance.State)
		{
		case GamePlayerState.Idle:
			AudioPlayer.StopSound(to.AmbienceAudioEventName);
			AudioPlayer.StopSound(to.MusicAudioEventName);
			break;
		case GamePlayerState.Exploring:
			AudioPlayer.SetSwitch(to.MusicAudioEventName, to.ExplorationMusicComponentName);
			break;
		case GamePlayerState.Combat:
			AudioPlayer.SetSwitch(to.MusicAudioEventName, to.CombatMusicComponentName);
			break;
		}
		yield return new WaitForEndOfFrame();
		GamePlayer.Instance.Level.LevelStyle = to;
	}

	private IEnumerator ReplaceLampPosts(LevelNode[] nodes, GameObject toLampPost, float totalTime)
	{
		List<GameObject> oldLamps = new List<GameObject>();
		float intervalTime = (totalTime - 0.2f) / (float)nodes.Length;
		for (int i = 0; i < nodes.Length; i++)
		{
			ReadOnlyCollection<GameObject> existing = nodes[i].Objects;
			for (int j = 0; j < existing.Count; j++)
			{
				GameObject newLamp = UnityEngine.Object.Instantiate(toLampPost, existing[j].transform.position, Quaternion.identity);
				newLamp.transform.parent = existing[j].transform.parent;
				nodes[i].AddObject(newLamp);
				MeshRenderer[] meshRenderers = existing[j].GetComponentsInChildren<MeshRenderer>();
				for (int k = 0; k < meshRenderers.Length; k++)
				{
					meshRenderers[k].enabled = false;
				}
				ParticleSystem[] particleSystems = existing[j].GetComponentsInChildren<ParticleSystem>();
				for (int l = 0; l < particleSystems.Length; l++)
				{
					particleSystems[l].Stop();
				}
				oldLamps.Add(existing[j]);
				yield return new WaitForSeconds(intervalTime + UnityEngine.Random.Range(0f, 0.2f));
				nodes[i].RemoveObject(existing[j]);
			}
		}
		yield return new WaitForSeconds(2f);
		for (int m = 0; m < oldLamps.Count; m++)
		{
			UnityEngine.Object.Destroy(oldLamps[m]);
		}
	}

	private IEnumerator ReplaceForegroundDecorations(List<DecorationForeground> foregroundDecorations, List<GameObject> replaceForegrounds, float totalTime)
	{
		float intervalTime = (totalTime - 0.2f) / (float)foregroundDecorations.Count;
		for (int i = 0; i < foregroundDecorations.Count; i++)
		{
			if (!(foregroundDecorations[i] == null))
			{
				int replaceIndex = Mathf.Min(foregroundDecorations[i].decorationIndex, replaceForegrounds.Count - 1);
				if (replaceIndex > replaceForegrounds.Count)
				{
					Debug.Log("count : " + replaceForegrounds.Count + "index : " + replaceIndex);
				}
				foregroundDecorations[i].ReplaceWith(replaceForegrounds[replaceIndex]);
				yield return new WaitForSeconds(intervalTime + UnityEngine.Random.Range(0f, 0.2f));
			}
		}
	}

	private IEnumerator DoLoadNextSequence()
	{
		LevelEngine engine = new LevelEngine();
		LevelThemeCollection themeCollection = GamePlayer.Instance.Level.LevelThemeCollection;
		LevelTheme theme = themeCollection.themes.values[GamePlayer.Instance.Level.LevelTheme.index + 1];
		LevelStyle style = GamePlayer.Instance.Level.LevelStyle;
		AudioPlayer.StopSound(style.AmbienceAudioEventName);
		AudioPlayer.StopSound(style.MusicAudioEventName);
		LevelExtension styleExtension = GameController.Instance.LevelExtensionController.GetLevelExtension(GameController.Instance.Seed);
		if (styleExtension != null)
		{
			styleExtension.OverwriteStyleValuesOn(style);
		}
		GameController.Instance.CurrentLevel = engine.GenerateLevel(themeCollection, theme, style, GamePlayer.Instance.Level.Seed, GameController.Instance.areaMapMetaLayer.GetPrimaryPowerForPosition(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol), GameController.Instance.areaMapMetaLayer.GetSecondaryPowerForPosition(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol));
		if (styleExtension != null)
		{
			styleExtension.DoLevelAlternations(GameController.Instance.CurrentLevel);
		}
		if (gamePlayerGO != null)
		{
			UnityEngine.Object.Destroy(gamePlayerGO);
		}
		gamePlayerGO = new GameObject();
		gamePlayerGO.name = "GamePlayer";
		GamePlayer gamePlayer = gamePlayerGO.AddComponent<GamePlayer>();
		gamePlayer.Initialize(GameController.Instance.CurrentLevel);
		GameController.Instance.LevelVisualizer.BuildLevel(GameController.Instance.CurrentLevel, GameController.Instance.CurrentLevel.LevelStyle, environmentSettingsIndex);
		GamePlayer.Instance.Reset();
		AudioPlayer.MoveFabricAudioListener(FabricManager.Instance.gameObject, GamePlayer.Instance.Player.Character.gameObject);
		AnalyticsEvents.LogStartLevel(GamePlayer.Instance.Level.LevelTheme.index);
		yield return 0;
	}

	public void EndLevel(bool wasCompleted = true)
	{
		if (wasCompleted && GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.IsCurrentObjectiveCompleted())
		{
			ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
			foreach (ObjectiveLine line in lines)
			{
				GameController.Instance.ObjectiveController.OnObjectiveCompleted(line);
			}
		}
		AudioPlayer.MoveFabricAudioListener(GamePlayer.Instance.Player.Character.gameObject, FabricManager.Instance.gameObject);
		hasEndedLevel = true;
		UnityEngine.Object.Destroy(completionMgr);
		if (GameController.Instance.OnEndLevel != null && wasCompleted)
		{
			GameController.Instance.OnEndLevel(GameController.Instance.CurrentLevel);
		}
		ScreenFadeFX component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
		component.OnDone = (ScreenFadeFX.OnDoneDelegate)Delegate.Combine(component.OnDone, new ScreenFadeFX.OnDoneDelegate(DelayedEndLevel));
		FadeInProgress = true;
		component.FadeOut(fadeTime);
	}

	private void DelayedEndLevel()
	{
		FadeInProgress = false;
		GameController.Instance.SetState(GetComponent<GameControllerStateLevelCompleted>());
	}
}
