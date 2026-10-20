using System.Collections.Generic;
using System.Text;
using AmuzoEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenMap : ScreenBase
{
	private enum FTUE_STATE
	{
		NONE = 0,
		INTRO = 1,
		TRANSITION_INTRO = 2,
		UNVISITED_AREAS = 3,
		TRANSITION_UNVISITED = 4,
		MISSION_AREAS = 5,
		TRANSITION_OUT = 6,
		FADE_OUT = 7,
		VEHICLE_PARTS = 8
	}

	public UILabel _debug;

	public float _collapseScrollTime = 7f;

	public BoxCollider _messageBoxCollider;

	public Transform _templateMissionParent;

	public UIWidget _templateMission;

	public UITexture _mapTexture;

	public UITexture _mapBoundaryLeft;

	public UITexture _mapBoundaryRight;

	public UITexture _mapBoundaryTop;

	public UITexture _mapBoundaryBottom;

	public Transform _screenBottomLeft;

	public Transform _screenBottomRight;

	public Transform _mapLeft;

	public Transform _mapRight;

	public Transform _mapBottom;

	public Transform _mapTop;

	public UIButton _zoomOutButton;

	public ScreenObjectTransitionWidget _exclamationBubble;

	public ScreenObjectTransitionWidget _wuMessageWidget;

	public ScreenObjectTransitionWidget _messageBoxVisibilityWidget;

	public ScreenObjectTransitionWidget _messageBoxWidget;

	public UIWidget _wuQuestionMark;

	public UIWidget _wuScroll;

	public UIWidget _wuScrollHolder;

	public UILabel _wuMessage;

	public UISprite _ftueFade;

	public GameObject _vehiclePartTutorialArrow;

	public UITexture _vehiclePartArrowTexture;

	public Transform _vehiclePartTutorialArrowWrapper;

	public GameObject _mapTutorialArrow;

	public UITexture _mapArrowTexture;

	public Transform _mapTutorialArrowWrapper;

	public UILabel _timeBladeCount;

	public UISprite[] _timeBlades;

	public UIWidget _timeBladeRoot;

	public UISprite[] _timeBladeGlow;

	public float _boundaryTopRatio;

	public MapButtonInfo _mbi;

	private bool _mbiParented;

	private bool _moveVehicle;

	private List<MapButtonInfo> _allMissionIcons = new List<MapButtonInfo>();

	private bool _awaitingMapLoad;

	private int _lastKnownScreenX;

	private int _lastKnownScreenY;

	private bool _stopFrame;

	private int _firstFrame;

	private Texture2D _fogOfWar;

	private Material _dummyMaterial1;

	private Material _dummyMaterial2;

	private bool _currentMaterial;

	private FTUE_STATE _ftueState;

	private float _ftueStateChangeTime;

	private string _objectiveMessage;

	private bool _seenFtueThisSession;

	private float _topBorderAspectRatio;

	private float _wuScrollTimer;

	private Texture _playerMugshot;

	private WuCruNotifications _localNotifications;

	public Texture _pPlayerMugshot
	{
		get
		{
			return _playerMugshot;
		}
		set
		{
			_playerMugshot = value;
		}
	}

	public static void UpdateCompletedObjectives(bool force = false)
	{
		Debug.Log("<color=yellow>---------</color>");
		if ((GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.IsCurrentObjectiveCompleted()) || force)
		{
			Debug.Log("Current Objective completed");
			int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
			int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
			chapterCursor++;
			int num = GameController.Instance.NumChapters(lastPlayedSeason);
			Debug.Log("<color=green>Setting Objective: S" + lastPlayedSeason + " C" + chapterCursor + " (numchapters=" + num + ")</color>");
			if (num > chapterCursor)
			{
				GameController.Instance.PlayerProfile.SetChapterCursor(lastPlayedSeason, chapterCursor);
				GameController.Instance.ObjectiveController.InitiateObjective(lastPlayedSeason, chapterCursor);
				GameController.Instance.PlayerProfile.Save();
			}
		}
	}

	public static void CheckForSeasonUnlocks()
	{
		bool flag = HandleSeason2Unlock();
		if (GameController.Instance.areaMapMetaLayer.IsFirstTimeInArea() && GameController.Instance.areaMapMetaLayer.season == 2)
		{
			LevelUnlockData levelUnlockData = new LevelUnlockData();
			levelUnlockData.seed = GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(GameController.Instance.areaMapMetaLayer.startRow, GameController.Instance.areaMapMetaLayer.startCol);
			levelUnlockData.season = GameController.Instance.areaMapMetaLayer.season;
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData.seed, levelUnlockData);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData.seed + "</color>");
			GameController.Instance.PlayerProfile.Save();
			Debug.Log("FirstTime in AreaMap -> unlocked level seed: " + levelUnlockData.seed);
		}
		flag = HandleSeason3Unlock();
		if (GameController.Instance.areaMapMetaLayer.IsFirstTimeInArea() && GameController.Instance.areaMapMetaLayer.season == 3)
		{
			LevelUnlockData levelUnlockData2 = new LevelUnlockData();
			levelUnlockData2.seed = GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(GameController.Instance.areaMapMetaLayer.startRow, GameController.Instance.areaMapMetaLayer.startCol);
			levelUnlockData2.season = GameController.Instance.areaMapMetaLayer.season;
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData2.seed, levelUnlockData2);
			Debug.Log("<color=yellow>Level Unlock" + levelUnlockData2.seed + "</color>");
			GameController.Instance.PlayerProfile.Save();
			Debug.Log("FirstTime in AreaMap -> unlocked level seed: " + levelUnlockData2.seed);
		}
		HandleSeasonChange();
	}

	private static void HandleSeasonChange()
	{
		Debug.Log("Name: " + GameController.Instance.areaMapMetaLayer.areaName);
		string areaName = GameController.Instance.areaMapMetaLayer.areaName;
		int season = GameController.Instance.areaMapMetaLayer.season;
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(season);
		Debug.Log("Cursor: " + chapterCursor);
		if (chapterCursor != -1)
		{
			GameController.Instance.ObjectiveController.InitiateObjective(season, chapterCursor);
			GameController.Instance.PlayerProfile.Save();
		}
		else
		{
			Debug.LogError("Attempting to change season failed - [Season:" + season + "] the season has not been started yet");
		}
		Debug.Log("Desired Area Name: " + areaName + " / " + GameController.Instance.areaMapMetaLayer.areaName);
		if (areaName != GameController.Instance.areaMapMetaLayer.areaName)
		{
			GameController.Instance.SetAreaMapMetaLayer(GameController.Instance.areaMapDB.areaMaps[areaName]);
		}
	}

	private static bool HandleSeason2Unlock()
	{
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(2);
		Debug.Log("<color=green>S2 Chapter: " + chapterCursor + "</color>");
		if (chapterCursor == 1)
		{
			int startRow = GameController.Instance.areaMapDB.areaMaps["S2A1"].startRow;
			int startCol = GameController.Instance.areaMapDB.areaMaps["S2A1"].startCol;
			int seedFromPosition = GameController.Instance.areaMapDB.areaMaps["S2A1"].GetSeedFromPosition(startRow, startCol);
			bool flag = GameController.Instance.areaMapMetaLayer.IsUnlocked(seedFromPosition);
			Debug.Log("Chapter HoT Start? " + startRow + " / " + startCol + " = " + flag);
			if (!flag)
			{
				GameController.Instance.PlayerProfile.SetChapterCursor(2, 1);
				GameController.Instance.ObjectiveController.InitiateObjective(2, 1);
				GameController.Instance.SetAreaMapMetaLayer(GameController.Instance.areaMapDB.areaMaps["S2A1"]);
				GameController.Instance.PlayerProfile.Save();
				return true;
			}
		}
		return false;
	}

	private static bool HandleSeason3Unlock()
	{
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(3);
		Debug.Log("<color=green>S3 Chapter: " + chapterCursor + "</color>");
		if (chapterCursor == 1)
		{
			int startRow = GameController.Instance.areaMapDB.areaMaps["S3A1"].startRow;
			int startCol = GameController.Instance.areaMapDB.areaMaps["S3A1"].startCol;
			int seedFromPosition = GameController.Instance.areaMapDB.areaMaps["S3A1"].GetSeedFromPosition(startRow, startCol);
			bool flag = GameController.Instance.areaMapMetaLayer.IsUnlocked(seedFromPosition);
			Debug.Log("Chapter City Start? " + startRow + " / " + startCol + " = " + flag);
			if (!flag)
			{
				GameController.Instance.PlayerProfile.SetChapterCursor(3, 1);
				GameController.Instance.ObjectiveController.InitiateObjective(3, 1);
				GameController.Instance.SetAreaMapMetaLayer(GameController.Instance.areaMapDB.areaMaps["S3A1"]);
				GameController.Instance.PlayerProfile.Save();
				return true;
			}
		}
		return false;
	}

	protected override void OnExitScreen()
	{
		UICamera.fallThrough = null;
		base.OnExitScreen();
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
	}

	protected override void OnShowScreen()
	{
		if (_localNotifications == null)
		{
			_localNotifications = new WuCruNotifications();
			_localNotifications.GenerateNotifications();
		}
		else
		{
			_localNotifications.GenerateNotifications();
		}
		_mbiParented = false;
		_wuScrollTimer = _collapseScrollTime;
		_firstFrame = 1;
		base.OnShowScreen();
		Time.timeScale = 1f;
		_vehiclePartTutorialArrow.SetActive(false);
		UICamera.fallThrough = base.gameObject;
		ScreenHud._pInstance.ClearPlayerTexture();
		if (_playerMugshot == null)
		{
			Texture2D[] array = CharacterPreviewRenderer.Generate(new int[1], 64, 64);
			array[0].wrapMode = TextureWrapMode.Clamp;
			_playerMugshot = array[0];
		}
		bool showMinifigGoldenTrail = GameController.Instance.PlayerProfile.HasAnyUnseenMinfigParts();
		bool showGarageGoldenTrail = UnlockableVehicleParts._pInstance.vehicleGoldenTrail();
		int[] unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
		bool showDojoGoldenTrail = unseenCards != null && unseenCards.Length > 0;
		ScreenHeaderNavigation._pInstance.OpenHeader();
		ScreenHeaderNavigation._pInstance.ConfigureHeader(OnSkills, null, OnGarage, OnDojo, OnSettings, showMinifigGoldenTrail, showGarageGoldenTrail, showDojoGoldenTrail);
		if (_fogOfWar == null)
		{
			_fogOfWar = new Texture2D(10, 8, TextureFormat.ARGB32, false);
			_fogOfWar.filterMode = FilterMode.Point;
		}
		if (_dummyMaterial1 == null)
		{
			_dummyMaterial1 = new Material(_mapTexture.material);
		}
		if (_dummyMaterial2 == null)
		{
			_dummyMaterial2 = new Material(_mapTexture.material);
		}
		if (!AudioPlayer.IsEventActive("music/area/map"))
		{
			AudioPlayer.PlaySound("music/area/map");
		}
		Debug.Log("MapScreen");
		RefreshMap();
		int num = 0;
		Dictionary<string, AreaMapMetaLayer>.Enumerator enumerator = GameController.Instance.areaMapDB.areaMaps.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = enumerator.Current.Key;
			int startRow = GameController.Instance.areaMapDB.areaMaps[key].startRow;
			int startCol = GameController.Instance.areaMapDB.areaMaps[key].startCol;
			int seedFromPosition = GameController.Instance.areaMapDB.areaMaps[key].GetSeedFromPosition(startRow, startCol);
			if (GameController.Instance.areaMapDB.areaMaps[key].IsUnlocked(seedFromPosition))
			{
				num++;
			}
		}
		_zoomOutButton.isEnabled = _ftueState == FTUE_STATE.NONE;
		_exclamationBubble.SetStateConsideredTweenedIn(false);
		_exclamationBubble.SetToTweenInStartColor();
		_messageBoxCollider.enabled = true;
		_messageBoxWidget.SetStateConsideredTweenedIn(false);
		_messageBoxWidget.SetToTweenInStartPos();
		_messageBoxWidget.TweenIn();
		_wuMessageWidget.SetStateConsideredTweenedIn(false);
		_wuMessageWidget.SetToTweenInStartColor();
		_wuMessageWidget.TweenIn();
		_wuScroll.enabled = true;
		_wuScrollHolder.enabled = true;
		_wuQuestionMark.enabled = false;
		_messageBoxVisibilityWidget.SetStateConsideredTweenedIn(false);
		_messageBoxVisibilityWidget.SetToTweenInStartColor();
		_messageBoxVisibilityWidget.TweenIn();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (playerProfile.NumS1A1LevelsPlayed <= 1 && !_seenFtueThisSession && GameController.Instance.areaMapMetaLayer.baseSeed == 1000)
		{
			_objectiveMessage = _wuMessage.text;
			_seenFtueThisSession = true;
			ChangeFTUEState(FTUE_STATE.INTRO);
		}
		else if (playerProfile.NumS1A1LevelsPlayed > 2 && !GameController.Instance.PlayerProfile._pSeenVehiclePartFTUE && GameController.Instance.areaMapMetaLayer.baseSeed == 1000)
		{
			GameController.Instance.PlayerProfile._pSeenVehiclePartFTUE = true;
			_objectiveMessage = _wuMessage.text;
			ChangeFTUEState(FTUE_STATE.VEHICLE_PARTS);
		}
		else
		{
			_ftueFade.enabled = false;
		}
		if (_ftueState == FTUE_STATE.NONE && !GameController.Instance.PlayerProfile._pSeenNotificationPrompt)
		{
			ShowNotificationPopup();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_topBorderAspectRatio = (float)_mapBoundaryTop.height / (float)_mapBoundaryTop.width;
		_debug.enabled = false;
	}

	private void HandleLog(string str, string stack, LogType type)
	{
		if (type == LogType.Error)
		{
			_debug.text = "\n[FF0000]" + str + "[-]" + _debug.text;
		}
		else
		{
			_debug.text = "\n" + str + _debug.text;
		}
	}

	protected override void Update()
	{
		base.Update();
		_localNotifications.TryIssueMessage();
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
			ScreenMapConfirm screenInstance2 = instance.GetScreenInstance<ScreenMapConfirm>();
			ScreenLocalNotificationPopup screenInstance3 = instance.GetScreenInstance<ScreenLocalNotificationPopup>();
			if (!screenInstance.gameObject.activeInHierarchy && !screenInstance2.gameObject.activeInHierarchy && !screenInstance3.gameObject.activeInHierarchy)
			{
				if (_ftueState == FTUE_STATE.NONE)
				{
					ScreenTitle screenInstance4 = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenTitle>();
					Navigate(screenInstance4);
					ScreenHeaderNavigation._pInstance.CloseHeader();
				}
				else
				{
					OnClick();
				}
			}
		}
		if (_messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE && _messageBoxCollider.enabled && _ftueState == FTUE_STATE.NONE && _wuScrollTimer > 0f)
		{
			_wuScrollTimer -= Time.deltaTime;
		}
		if (_messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE && _messageBoxWidget._pConsideredTweenedIn && _wuScrollTimer <= 0f && _messageBoxCollider.enabled)
		{
			_exclamationBubble.TweenIn();
			_messageBoxWidget.TweenOut();
			_wuMessageWidget.TweenOut();
			_messageBoxVisibilityWidget.TweenOut();
			_messageBoxCollider.enabled = false;
		}
		_mapBoundaryTop.width = (int)((_mapRight.localPosition.x - _mapLeft.localPosition.x) * _boundaryTopRatio);
		_mapBoundaryBottom.width = _mapBoundaryTop.width;
		_mapBoundaryTop.height = (int)((float)_mapBoundaryTop.width * _topBorderAspectRatio);
		_mapBoundaryBottom.height = _mapBoundaryTop.height;
		switch (_ftueState)
		{
		case FTUE_STATE.NONE:
			UpdateFtueNone();
			break;
		case FTUE_STATE.INTRO:
			UpdateFtueIntro();
			break;
		case FTUE_STATE.TRANSITION_INTRO:
			UpdateTransitionIntro();
			break;
		case FTUE_STATE.UNVISITED_AREAS:
			UpdateUnvisitedAreas();
			break;
		case FTUE_STATE.TRANSITION_UNVISITED:
			UpdateTransitionUnvisited();
			break;
		case FTUE_STATE.MISSION_AREAS:
			UpdateMissionAreas();
			break;
		case FTUE_STATE.TRANSITION_OUT:
			UpdateTransitionOut();
			break;
		case FTUE_STATE.FADE_OUT:
			UpdateFadeOut();
			break;
		case FTUE_STATE.VEHICLE_PARTS:
			UpdateFtueVehicleParts();
			break;
		}
		if (_firstFrame == 0)
		{
			_firstFrame++;
		}
		else if (_firstFrame == 1)
		{
			_firstFrame++;
			for (int i = 0; i < _allMissionIcons.Count; i++)
			{
				_allMissionIcons[i].gameObject.SetActive(true);
			}
			_mapTexture.enabled = true;
			_mapBoundaryBottom.enabled = true;
			_mapBoundaryLeft.enabled = true;
			_mapBoundaryRight.enabled = true;
			_mapBoundaryTop.enabled = true;
		}
		if (_mbi != null && !ScreenHeaderNavigation._pInstance._pIsTweening && !_mbiParented)
		{
			if (_moveVehicle)
			{
				_mbi._vehicleMarker.gameObject.SetActive(false);
				Vector3 localPosition = _mbi._vehicleMarker.transform.localPosition;
				_mbi._vehicleMarker.transform.parent = ScreenHeaderNavigation._pInstance._garage.transform;
				_mbi._vehicleMarker.depth = 16;
			}
			else
			{
				_mbi._wardrobeMarker.gameObject.SetActive(false);
				Vector3 localPosition2 = _mbi._wardrobeMarker.transform.localPosition;
				_mbi._wardrobeMarker.transform.parent = ScreenHeaderNavigation._pInstance._skills.transform;
				_mbi._wardrobeMarker.depth = 16;
			}
			_mbiParented = true;
		}
		if (_mbiParented)
		{
			if (_moveVehicle)
			{
				if (!_mbi._vehicleMarker.gameObject.activeSelf)
				{
					_mbi._vehicleMarker.gameObject.SetActive(true);
				}
				Vector3 vector = Vector3.MoveTowards(_mbi._vehicleMarker.transform.localPosition, Vector3.zero, 512f * Time.deltaTime);
				_mbi._vehicleMarker.transform.localPosition = vector;
				Vector3 vector2 = vector;
				if (vector2 == Vector3.zero)
				{
					_mbi._vehicleMarker.gameObject.SetActive(false);
					_mbi = null;
					_mbiParented = false;
					ScreenHeaderNavigation._pInstance.playGarageFX();
				}
			}
			else
			{
				if (!_mbi._wardrobeMarker.gameObject.activeSelf)
				{
					_mbi._wardrobeMarker.gameObject.SetActive(true);
				}
				Vector3 vector = Vector3.MoveTowards(_mbi._wardrobeMarker.transform.localPosition, Vector3.zero, 512f * Time.deltaTime);
				_mbi._wardrobeMarker.transform.localPosition = vector;
				Vector3 vector3 = vector;
				if (vector3 == Vector3.zero)
				{
					_mbi._wardrobeMarker.gameObject.SetActive(false);
					_mbi = null;
					_mbiParented = false;
					ScreenHeaderNavigation._pInstance.playWardrobeFX();
				}
			}
		}
		UpdateMapAnchoring();
	}

	private void UpdateHandsOfTimeUI(int season, int chapter)
	{
		Debug.Log("Chapter: " + chapter);
		_timeBladeRoot.alpha = ((season == 2) ? 1 : 0);
		for (int i = 0; i < 4; i++)
		{
			bool flag = chapter - 1 > i;
			switch (i)
			{
			case 0:
				_timeBlades[i].spriteName = ((!flag) ? "Green_Empty" : "Green_Collected");
				break;
			case 1:
				_timeBlades[i].spriteName = ((!flag) ? "Blue_Empty" : "Blue_Collected");
				break;
			case 2:
				_timeBlades[i].spriteName = ((!flag) ? "Red_Empty" : "Red_Collected");
				break;
			case 3:
				_timeBlades[i].spriteName = ((!flag) ? "Yellow_Empty" : "Yellow_Collected");
				break;
			}
			_timeBladeGlow[i].enabled = flag;
		}
		_timeBladeCount.text = Mathf.Clamp(chapter - 1, 0, 4) + "/4";
	}

	private void UpdateMapAnchoring()
	{
		int width = Screen.width;
		int height = Screen.height;
		if (width == _lastKnownScreenX && height == _lastKnownScreenY)
		{
			return;
		}
		if (!_stopFrame)
		{
			_stopFrame = true;
			return;
		}
		_stopFrame = false;
		_lastKnownScreenX = width;
		_lastKnownScreenY = height;
		int num = 1920;
		int num2 = 1080;
		float num3 = (float)num / (float)num2;
		float num4 = (float)num2 / (float)num;
		Debug.Log("UpdateMapAnchoring() - " + width + "/" + height + " || " + num + "/" + num2 + " || " + num3);
		Vector3 localPosition = _mapLeft.localPosition;
		Vector3 localPosition2 = _mapRight.localPosition;
		Vector3 localPosition3 = _mapTop.localPosition;
		Vector3 localPosition4 = _mapBottom.localPosition;
		localPosition.x = _screenBottomLeft.localPosition.x;
		localPosition2.x = _screenBottomRight.localPosition.x;
		float num5 = localPosition2.x - localPosition.x;
		float num6 = num5 * num4;
		localPosition4.y = localPosition3.y - num6;
		if (localPosition4.y < _screenBottomLeft.localPosition.y)
		{
			localPosition4.y = _screenBottomLeft.localPosition.y;
			float num7 = localPosition3.y - localPosition4.y;
			float num8 = num7 * num3;
			localPosition.x = 0f - num8 * 0.5f;
			localPosition2.x = num8 * 0.5f;
		}
		_mapRight.localPosition = localPosition2;
		_mapLeft.localPosition = localPosition;
		_mapBottom.localPosition = localPosition4;
		int count = _allMissionIcons.Count;
		int num9 = GameController.Instance.areaMapMetaLayer.rowArray.Length;
		int num10 = GameController.Instance.areaMapMetaLayer.rowArray[0].colArray.Length;
		float num11 = (localPosition2.x - localPosition.x) / (float)num10;
		float num12 = (localPosition3.y - localPosition4.y) / (float)num9;
		for (int i = 0; i < count; i++)
		{
			Vector3 localPosition5 = _allMissionIcons[i].transform.localPosition;
			localPosition5.x = localPosition.x + (float)_allMissionIcons[i]._pCoordX * num11;
			localPosition5.y = localPosition4.y + (float)_allMissionIcons[i]._pCoordY * num12;
			_allMissionIcons[i].transform.localPosition = localPosition5;
			UIWidget component = _allMissionIcons[i].GetComponent<UIWidget>();
			component.width = (int)num11;
			component.height = (int)num12;
		}
		Debug.Log("--- done align ---");
	}

	public void OnToggleWu()
	{
		if (_ftueState != FTUE_STATE.NONE)
		{
			HandleFtueProgression();
		}
		else if (_messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			if (_messageBoxWidget._pConsideredTweenedIn)
			{
				_exclamationBubble.TweenIn();
				_messageBoxWidget.TweenOut();
				_wuMessageWidget.TweenOut();
				_messageBoxVisibilityWidget.TweenOut();
				_messageBoxCollider.enabled = false;
			}
			else
			{
				_wuScrollTimer = _collapseScrollTime;
				_exclamationBubble.TweenOut();
				_messageBoxWidget.TweenIn();
				_wuMessageWidget.TweenIn();
				_messageBoxVisibilityWidget.TweenIn();
				_messageBoxCollider.enabled = true;
			}
		}
	}

	private void RefreshMap()
	{
		Debug.Log("RefreshMap()");
		UpdateCompletedObjectives();
		CheckForSeasonUnlocks();
		int num = GameController.Instance.areaMapMetaLayer.rowArray.Length;
		int num2 = GameController.Instance.areaMapMetaLayer.rowArray[0].colArray.Length;
		string mapImagePath = GameController.Instance.areaMapMetaLayer.mapImagePath;
		Texture mainTexture = Resources.Load<Texture>(GameController.Instance.areaMapMetaLayer.mapImagePath);
		Debug.Log("Loading Sprite: " + GameController.Instance.areaMapMetaLayer.mapImagePath);
		_mapTexture.mainTexture = mainTexture;
		string path = mapImagePath + "_Left";
		string path2 = mapImagePath + "_Right";
		string path3 = mapImagePath + "_Bottom";
		string path4 = mapImagePath + "_Top";
		Texture mainTexture2 = Resources.Load<Texture>(path);
		Texture mainTexture3 = Resources.Load<Texture>(path2);
		Texture mainTexture4 = Resources.Load<Texture>(path3);
		Texture mainTexture5 = Resources.Load<Texture>(path4);
		_mapBoundaryLeft.mainTexture = mainTexture2;
		_mapBoundaryRight.mainTexture = mainTexture3;
		_mapBoundaryBottom.mainTexture = mainTexture4;
		_mapBoundaryTop.mainTexture = mainTexture5;
		if (_allMissionIcons.Count == 0)
		{
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					GameObject gameObject = Object.Instantiate(_templateMission.gameObject);
					gameObject.transform.parent = _templateMissionParent;
					gameObject.transform.localScale = _templateMission.transform.localScale;
					gameObject.transform.localPosition = new Vector3(77 * j, 77 * i, 0f);
					MapButtonInfo component = gameObject.transform.GetComponent<MapButtonInfo>();
					component._pCoordX = j;
					component._pCoordY = i;
					int num3 = i;
					component._unexplored.depth += num3;
					component._dotdMarker.depth += num3;
					component._wardrobeMarker.depth += num3;
					component._vehicleMarker.depth += num3;
					component._label.depth += num3;
					component._playerImage.depth += num3;
					component._currentPosition.GetComponent<UISprite>().depth += num3;
					component.GetComponent<UIWidget>().depth += num3;
					int num4 = component._stars.Length;
					for (int k = 0; k < num4; k++)
					{
						component._stars[k].depth += num3;
					}
					_allMissionIcons.Add(component);
				}
			}
		}
		_mapTexture.enabled = false;
		_mapBoundaryBottom.enabled = false;
		_mapBoundaryLeft.enabled = false;
		_mapBoundaryRight.enabled = false;
		_mapBoundaryTop.enabled = false;
		_templateMission.gameObject.SetActive(false);
		int count = _allMissionIcons.Count;
		int num5 = 0;
		if (GameController.Instance.ObjectiveController.CurrentObjective != null)
		{
			num5 = GameController.Instance.ObjectiveController.CurrentObjective.lines.Length;
		}
		int season = GameController.Instance.areaMapMetaLayer.season;
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(season);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Objectives.S");
		stringBuilder.Append(season);
		stringBuilder.Append("_C");
		stringBuilder.Append(chapterCursor);
		string key = stringBuilder.ToString();
		_wuMessage.text = Localise(key);
		GameController.Instance.LevelExtensionController.SetSeedIndexesToZero("DayOfTheDeparted");
		for (int l = 0; l < count; l++)
		{
			int pSeed = _allMissionIcons[l]._pSeed;
			_allMissionIcons[l]._dropShadow.enabled = false;
			_allMissionIcons[l].ShowObjectiveFTUE(false, null);
			_allMissionIcons[l].ShowUnexploredFTUE(false);
			_allMissionIcons[l].ShowVehiclePartFTUE(false);
			bool flag = GameController.Instance.areaMapMetaLayer.IsUnlocked(pSeed);
			bool flag2 = false;
			bool flag3 = GameController.Instance.areaMapMetaLayer.IsPlayed(pSeed);
			if (GameController.Instance.ObjectiveController.CurrentObjective != null)
			{
				int pSeed2 = _allMissionIcons[l]._pSeed;
				for (int m = 0; m < num5; m++)
				{
					ObjectiveLine objectiveLine = GameController.Instance.ObjectiveController.CurrentObjective.lines[m];
					if (!objectiveLine.Completed && objectiveLine.seed == pSeed2)
					{
						flag2 = true;
					}
				}
			}
			bool flag4 = false;
			if (UnlockableMinifigParts._pInstance != null)
			{
				flag4 = UnlockableMinifigParts._pInstance.IsSeedAvailable(_allMissionIcons[l]._pSeed);
				if (flag4 || GameController.Instance._collectedClothingPart)
				{
				}
				_allMissionIcons[l].SetWardrobeMarkerActive(flag4);
			}
			int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
			bool flag5 = false;
			if (UnlockableVehicleParts._pInstance != null)
			{
				flag5 = UnlockableVehicleParts._pInstance.IsSeedAvailable(_allMissionIcons[l]._pSeed);
			}
			_allMissionIcons[l].SetVehicleMarkerActive(flag5);
			if (_allMissionIcons[l]._pCoordX == 3 && _allMissionIcons[l]._pCoordY == 3)
			{
				_vehiclePartTutorialArrow.transform.parent = _allMissionIcons[l]._vehicleMarker.transform;
				_vehiclePartTutorialArrow.transform.localPosition = Vector3.zero;
			}
			_allMissionIcons[l]._button.isEnabled = flag || flag2;
			_allMissionIcons[l].SetUnexploredVisible(!flag3 && flag && !flag2 && !flag4 && !flag5);
			_allMissionIcons[l].SetStarsVisible(flag3);
			_allMissionIcons[l].SetStars(GameController.Instance.areaMapMetaLayer.GetNumStars(pSeed));
			Color color = ((!flag) ? Color.black : Color.white);
			if (!flag3 && flag)
			{
				color = Color.grey;
			}
			_fogOfWar.SetPixel(_allMissionIcons[l]._pCoordX, _allMissionIcons[l]._pCoordY, color);
			_allMissionIcons[l].SetCurrentObjective(flag2);
			_allMissionIcons[l].SetLocked(!flag);
			bool currentPositionVisible = false;
			if (_allMissionIcons[l]._pCoordX == GameController.Instance.areaMapMetaLayer.currentCol && _allMissionIcons[l]._pCoordY == GameController.Instance.areaMapMetaLayer.currentRow)
			{
				currentPositionVisible = true;
			}
			GameObject areaMapPointer = GameController.Instance.LevelExtensionController.GetAreaMapPointer(_allMissionIcons[l]._pSeed);
			_allMissionIcons[l].SetDoTDActive(areaMapPointer != null);
			_allMissionIcons[l].SetCurrentPositionVisible(currentPositionVisible);
			_moveVehicle = false;
			if (_allMissionIcons[l]._currentPosition.activeSelf)
			{
				if (!flag5 && GameController.Instance._collectedVehiclePart)
				{
					_moveVehicle = true;
					GameController.Instance._collectedVehiclePart = false;
					_mbi = _allMissionIcons[l];
					_allMissionIcons[l].SetVehicleMarkerActive(true);
				}
				else if (!flag4 && GameController.Instance._collectedClothingPart)
				{
					GameController.Instance._collectedClothingPart = false;
					_mbi = _allMissionIcons[l];
					_allMissionIcons[l].SetWardrobeMarkerActive(true);
				}
			}
			if (_allMissionIcons[l]._wantDebug)
			{
				MapButtonInfo mapButtonInfo = _allMissionIcons[l];
				mapButtonInfo._label.enabled = true;
				string text = mapButtonInfo._pCoordY + "-" + mapButtonInfo._pCoordX;
				if (GameController.Instance.ObjectiveController.CurrentObjective != null)
				{
					int pSeed3 = mapButtonInfo._pSeed;
					for (int n = 0; n < num5; n++)
					{
						ObjectiveLine objectiveLine2 = GameController.Instance.ObjectiveController.CurrentObjective.lines[n];
						if (!objectiveLine2.Completed && objectiveLine2.seed == pSeed3)
						{
							text += "*";
						}
					}
				}
				if (areaMapPointer != null)
				{
					text = text + "(" + areaMapPointer.name + ")";
				}
				text = text + "\n" + mapButtonInfo._pSeed;
				if (GameController.Instance.areaMapMetaLayer.IsBlocked(mapButtonInfo._pCoordY, mapButtonInfo._pCoordX))
				{
					mapButtonInfo._label.alpha = 0.25f;
				}
				else
				{
					mapButtonInfo._label.alpha = 1f;
				}
				mapButtonInfo._label.text = text;
			}
			else
			{
				_allMissionIcons[l]._label.enabled = false;
				_allMissionIcons[l].gameObject.SetActive(false);
			}
			if (_currentMaterial)
			{
				_dummyMaterial1.SetTexture("_FogOfWar", _fogOfWar);
				_mapTexture.material = _dummyMaterial1;
			}
			else
			{
				_dummyMaterial2.SetTexture("_FogOfWar", _fogOfWar);
				_mapTexture.material = _dummyMaterial2;
			}
			_currentMaterial = !_currentMaterial;
		}
		int width = _fogOfWar.width;
		int height = _fogOfWar.height;
		for (int num6 = 0; num6 < width; num6++)
		{
			for (int num7 = 0; num7 < height; num7++)
			{
				Color pixel = _fogOfWar.GetPixel(num6, num7);
				if (pixel.r > 0f)
				{
					continue;
				}
				if (num6 < width - 1)
				{
					Color pixel2 = _fogOfWar.GetPixel(num6 + 1, num7);
					pixel.g = 0f;
					if (pixel2.r > 0f)
					{
						pixel.g = 0.75f;
					}
				}
				if (num6 > 0 && _fogOfWar.GetPixel(num6 - 1, num7).r > 0f)
				{
					if (pixel.g == 0.75f)
					{
						pixel.g = 0.5f;
					}
					else
					{
						pixel.g = 0.25f;
					}
				}
				if (num7 > 0 && _fogOfWar.GetPixel(num6, num7 - 1).r > 0f)
				{
					pixel.b = 0.25f;
				}
				if (num7 < height - 1 && _fogOfWar.GetPixel(num6, num7 + 1).r > 0f)
				{
					if (pixel.b == 0.25f)
					{
						pixel.b = 0.5f;
					}
					else
					{
						pixel.b = 0.75f;
					}
					int index = (num7 + 1) * num2 + num6;
					_allMissionIcons[index]._dropShadow.enabled = true;
				}
				_fogOfWar.SetPixel(num6, num7, pixel);
			}
		}
		_fogOfWar.Apply();
		_awaitingMapLoad = true;
		UpdateHandsOfTimeUI(GameController.Instance.areaMapMetaLayer.season, GameController.Instance.ObjectiveController.CurrentObjective.chapter);
		if (_mbi != null)
		{
		}
		Debug.Log("Done Refresh");
	}

	public void OnZoomOut()
	{
		if (!Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenMapConfirm>().gameObject.activeInHierarchy)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenWorldMap screenInstance = instance.GetScreenInstance<ScreenWorldMap>();
			Navigate(screenInstance);
		}
	}

	public void OnDojo()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		SceneManager.LoadScene("Dojo");
		Navigate(screenInstance);
	}

	public void OnSkills()
	{
		if (!Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenMapConfirm>().gameObject.activeInHierarchy)
		{
			SceneManager.LoadScene("Empty");
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSkills screenInstance = instance.GetScreenInstance<ScreenSkills>();
			Navigate(screenInstance);
		}
	}

	public void OnSettings()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
		if (Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenSettings>().gameObject.activeInHierarchy)
		{
			screenInstance.EndFlow();
			return;
		}
		screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_MAP;
		instance.ShowPopupOverlay(screenInstance);
	}

	public void ShowNotificationPopup()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenLocalNotificationPopup screenInstance = instance.GetScreenInstance<ScreenLocalNotificationPopup>();
		instance.ShowPopupOverlay(screenInstance);
	}

	public void OnRoster()
	{
		if (!Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenMapConfirm>().gameObject.activeInHierarchy)
		{
			SceneManager.LoadScene("Empty");
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenRoster screenInstance = instance.GetScreenInstance<ScreenRoster>();
			Navigate(screenInstance);
		}
	}

	public void OnGarage()
	{
		if (!Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenMapConfirm>().gameObject.activeInHierarchy)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenGarage screenInstance = instance.GetScreenInstance<ScreenGarage>();
			Navigate(screenInstance);
		}
	}

	public void OnMapAreaClicked(GameObject source)
	{
		if (Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenMapConfirm>().gameObject.activeInHierarchy)
		{
			return;
		}
		AudioPlayer.PlaySound("ui/button/sword/shing");
		if (_ftueState != FTUE_STATE.NONE)
		{
			HandleFtueProgression();
			return;
		}
		MapButtonInfo component = source.transform.GetComponent<MapButtonInfo>();
		if (component._pIsObjective && component._pIsLocked)
		{
			OnToggleWu();
			return;
		}
		ScreenMapConfirm._chosenMapInfo = component;
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		instance.ShowPopupOverlay(instance.GetScreenInstance<ScreenMapConfirm>());
	}

	private void ChangeFTUEState(FTUE_STATE newState)
	{
		_ftueState = newState;
		_ftueStateChangeTime = RealTime.time;
		switch (_ftueState)
		{
		case FTUE_STATE.NONE:
			StartFtueNone();
			break;
		case FTUE_STATE.INTRO:
			StartFtueIntro();
			break;
		case FTUE_STATE.TRANSITION_INTRO:
			StartTransitionIntro();
			break;
		case FTUE_STATE.UNVISITED_AREAS:
			StartUnvisitedAreas();
			break;
		case FTUE_STATE.TRANSITION_UNVISITED:
			StartTransitionUnvisited();
			break;
		case FTUE_STATE.MISSION_AREAS:
			StartMissionAreas();
			break;
		case FTUE_STATE.TRANSITION_OUT:
			StartTransitionOut();
			break;
		case FTUE_STATE.FADE_OUT:
			StartFadeOut();
			break;
		case FTUE_STATE.VEHICLE_PARTS:
			StartFtueVehicleParts();
			break;
		}
		_zoomOutButton.isEnabled = _ftueState == FTUE_STATE.NONE;
	}

	private void StartFtueNone()
	{
	}

	private void UpdateFtueNone()
	{
	}

	private void StartFtueIntro()
	{
		ScreenHeaderNavigation._pInstance.DisableButtons();
		_wuScroll.enabled = false;
		_wuScrollHolder.enabled = false;
		_wuQuestionMark.enabled = false;
		_ftueFade.enabled = true;
		_wuMessage.text = Localise("FTUE.MapIntro");
	}

	private void UpdateFtueIntro()
	{
	}

	private void StartTransitionIntro()
	{
		_messageBoxWidget.TweenOut();
		_wuMessageWidget.TweenOut();
	}

	private void UpdateTransitionIntro()
	{
		if (!_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType != ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			_wuMessage.text = Localise("FTUE.MapUnvisitedAreas");
			_messageBoxWidget.TweenIn();
			_wuMessageWidget.TweenIn();
			int count = _allMissionIcons.Count;
			for (int i = 0; i < count; i++)
			{
				_allMissionIcons[i].ShowUnexploredFTUE(true);
			}
			_wuScroll.enabled = false;
			_wuScrollHolder.enabled = true;
			_wuQuestionMark.enabled = true;
		}
		else if (_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			ChangeFTUEState(FTUE_STATE.UNVISITED_AREAS);
		}
	}

	private void StartUnvisitedAreas()
	{
	}

	private void UpdateUnvisitedAreas()
	{
	}

	private void StartTransitionUnvisited()
	{
		_messageBoxWidget.TweenOut();
		_wuMessageWidget.TweenOut();
	}

	private void UpdateTransitionUnvisited()
	{
		if (!_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType != ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			_wuScroll.enabled = true;
			_wuScrollHolder.enabled = true;
			_wuQuestionMark.enabled = false;
			int count = _allMissionIcons.Count;
			for (int i = 0; i < count; i++)
			{
				_allMissionIcons[i].ShowUnexploredFTUE(false);
				_allMissionIcons[i].ShowObjectiveFTUE(true, _mapTutorialArrow);
			}
			_wuMessage.text = Localise("FTUE.MissionAreas");
			_messageBoxWidget.TweenIn();
			_wuMessageWidget.TweenIn();
		}
		else if (_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			ChangeFTUEState(FTUE_STATE.MISSION_AREAS);
		}
	}

	private void StartMissionAreas()
	{
		_mapArrowTexture.mainTexture = Resources.Load<Texture>("FTUI_RedArrow");
		_mapTutorialArrow.SetActive(true);
	}

	private void UpdateMissionAreas()
	{
		float num = Mathf.Sin(Time.time * 6f);
		_mapTutorialArrowWrapper.localPosition = _mapTutorialArrowWrapper.up * (1f - num) * 10f;
	}

	private void StartTransitionOut()
	{
		_messageBoxWidget.TweenIn();
		_wuMessageWidget.TweenIn();
	}

	private void UpdateTransitionOut()
	{
		if (!_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType != ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			_wuMessage.text = _objectiveMessage;
			_messageBoxWidget.TweenIn();
			_wuMessageWidget.TweenIn();
		}
		else if (_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			_wuMessage.text = _objectiveMessage;
			ChangeFTUEState(FTUE_STATE.FADE_OUT);
		}
	}

	private void StartFadeOut()
	{
		_mapTutorialArrow.SetActive(false);
		_vehiclePartTutorialArrow.SetActive(false);
		_vehiclePartArrowTexture.mainTexture = null;
		int count = _allMissionIcons.Count;
		for (int i = 0; i < count; i++)
		{
			_allMissionIcons[i].ShowUnexploredFTUE(false);
			_allMissionIcons[i].ShowObjectiveFTUE(false, null);
		}
	}

	private void UpdateFadeOut()
	{
		float num = RealTime.time - _ftueStateChangeTime;
		if (num < 0.5f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			_ftueFade.alpha = Mathf.Lerp(23f / 32f, 0f, t);
		}
		else
		{
			ScreenHeaderNavigation._pInstance.EnableButtons();
			_ftueFade.enabled = false;
			ChangeFTUEState(FTUE_STATE.NONE);
		}
	}

	private void StartFtueVehicleParts()
	{
		_wuScroll.enabled = false;
		_wuScrollHolder.enabled = false;
		_wuQuestionMark.enabled = false;
		_ftueFade.enabled = true;
		_ftueFade.alpha = 23f / 32f;
		_wuMessage.text = Localise("FTUE.MapVehicleParts");
		_vehiclePartArrowTexture.mainTexture = Resources.Load<Texture>("FTUI_RedArrow");
		_vehiclePartTutorialArrow.SetActive(true);
		int count = _allMissionIcons.Count;
		for (int i = 0; i < count; i++)
		{
			_allMissionIcons[i].ShowVehiclePartFTUE(true);
		}
	}

	private void UpdateFtueVehicleParts()
	{
		ScreenHeaderNavigation._pInstance.DisableButtons();
		float num = Mathf.Sin(Time.time * 6f);
		_vehiclePartTutorialArrowWrapper.localPosition = _vehiclePartTutorialArrowWrapper.up * (1f - num) * 10f;
	}

	private void HandleFtueProgression()
	{
		Debug.Log("FTUE PROG: " + _ftueState);
		if (_ftueState == FTUE_STATE.INTRO)
		{
			ChangeFTUEState(FTUE_STATE.TRANSITION_INTRO);
		}
		else if (_ftueState == FTUE_STATE.UNVISITED_AREAS)
		{
			ChangeFTUEState(FTUE_STATE.TRANSITION_UNVISITED);
		}
		else if (_ftueState == FTUE_STATE.MISSION_AREAS)
		{
			ChangeFTUEState(FTUE_STATE.TRANSITION_OUT);
		}
		else if (_ftueState == FTUE_STATE.VEHICLE_PARTS)
		{
			int count = _allMissionIcons.Count;
			for (int i = 0; i < count; i++)
			{
				_allMissionIcons[i].ShowVehiclePartFTUE(false);
			}
			ChangeFTUEState(FTUE_STATE.TRANSITION_OUT);
		}
	}

	private void OnClick()
	{
		if (_ftueState != FTUE_STATE.NONE)
		{
			HandleFtueProgression();
		}
	}
}
