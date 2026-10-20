using System;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public delegate void OnReachFlagDelegate(Level level);

	public delegate void OnEndLevelDelegate(Level level);

	public delegate void OnAlertStateBeginDelegate();

	public delegate void OnAlertStateEndDelegate();

	private static GameController instance;

	public LevelStyleDB styleDB;

	public LevelThemeDB themeDB;

	public AreaMapDB areaMapDB;

	public StoryDB storyDB;

	public CamSettingsDB camSettingsDB;

	public LevelExtensionController levelExtensionController;

	[SerializeField]
	private AreaMapMetaLayer _areaMapMetaLayer;

	public bool diagonalUnlock;

	public int styleIndex;

	public int themeCollectionIndex;

	public int seed;

	public bool debugTextOnCharacters;

	private LevelVisualizer levelVisualizer;

	private Level currentLevel;

	private float triggerCheatMenuStartTime;

	private float triggerCheatMenuDuration = 5f;

	private Vector2 triggerCheatMenuMaxPos = new Vector2(50f, 50f);

	private int numSeasons;

	private int[] numChapters;

	public GameControllerState currentState;

	private MissionStats missionStats;

	private PlayerProfile playerProfile;

	private ObjectiveController objectiveController;

	private MissionController missionController;

	private AIAlertManager alertManager;

	public OnReachFlagDelegate OnReachFlag;

	public OnEndLevelDelegate OnEndLevel;

	private bool isAlertState;

	public OnAlertStateBeginDelegate OnAlertStateBegin;

	public OnAlertStateEndDelegate OnAlertStateEnd;

	public ElementalPower _hudElemental;

	public bool _collectedVehiclePart;

	public bool _collectedClothingPart;

	public static readonly bool WuCruMissionsEnabled = true;

	public AreaMapMetaLayer areaMapMetaLayer
	{
		get
		{
			return _areaMapMetaLayer;
		}
		set
		{
			Debug.Log("Setting areaMapMetaLayer to " + value);
			_areaMapMetaLayer = value;
		}
	}

	public Level CurrentLevel
	{
		get
		{
			return currentLevel;
		}
		set
		{
			currentLevel = value;
		}
	}

	public LevelVisualizer LevelVisualizer
	{
		get
		{
			return levelVisualizer;
		}
	}

	public LevelStyleDB StyleDB
	{
		get
		{
			return styleDB;
		}
	}

	public LevelThemeDB ThemeDB
	{
		get
		{
			return themeDB;
		}
	}

	public int StyleIndex
	{
		get
		{
			return styleIndex;
		}
	}

	public int ThemeCollectionIndex
	{
		get
		{
			return themeCollectionIndex;
		}
	}

	public int Seed
	{
		get
		{
			return seed;
		}
	}

	public MissionStats MissionStats
	{
		get
		{
			return missionStats;
		}
	}

	public PlayerProfile PlayerProfile
	{
		get
		{
			return playerProfile;
		}
	}

	public ObjectiveController ObjectiveController
	{
		get
		{
			return objectiveController;
		}
	}

	public MissionController MissionController
	{
		get
		{
			return missionController;
		}
	}

	public LevelExtensionController LevelExtensionController
	{
		get
		{
			return levelExtensionController;
		}
	}

	public int NumSeasons
	{
		get
		{
			return numSeasons;
		}
	}

	public int AlertCount
	{
		get
		{
			return alertManager.AlertCount;
		}
	}

	public int SuspiciousCount
	{
		get
		{
			return alertManager.SuspiciousCount;
		}
	}

	public static bool _pGameControllerExists
	{
		get
		{
			return instance != null;
		}
	}

	public static GameController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<GameController>();
				if (instance == null)
				{
					instance = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameController/GameController"))).GetComponent<GameController>();
					instance.Initialize();
				}
			}
			return instance;
		}
	}

	public int NumChapters(int season)
	{
		if (season > numChapters.Length)
		{
			return 0;
		}
		return numChapters[season - 1];
	}

	public void ResetPlayerProfile()
	{
		playerProfile = new PlayerProfile();
	}

	public void Initialize()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		missionStats = base.gameObject.AddComponent<MissionStats>();
		objectiveController = base.gameObject.AddComponent<ObjectiveController>();
		missionController = base.gameObject.AddComponent<MissionController>();
		playerProfile = new PlayerProfile();
		playerProfile.LoadLocal();
		alertManager = base.gameObject.AddComponent<AIAlertManager>();
		levelExtensionController.LoadFromPlayerProfile();
		levelVisualizer = base.gameObject.AddComponent<LevelVisualizer>();
		levelVisualizer.useLevelMeshBuilder = true;
		GameController[] array = UnityEngine.Object.FindObjectsOfType<GameController>();
		GameController[] array2 = array;
		foreach (GameController gameController in array2)
		{
			if (gameController != Instance)
			{
				Debug.LogWarning("DESTROYING EXISTING GameController");
				UnityEngine.Object.Destroy(gameController.gameObject);
			}
		}
		string text = playerProfile.GetCurrentArea();
		if (text == null || text == string.Empty)
		{
			text = "S1A1";
		}
		SetAreaMapMetaLayer(areaMapDB.areaMaps[text]);
		numSeasons = GetLastSeason();
		numChapters = new int[numSeasons];
		for (int j = 0; j < numSeasons; j++)
		{
			numChapters[j] = GetLastChapter(j + 1);
		}
		SetState(GetComponent<GameControllerStateIdle>());
	}

	public void SetState(GameControllerState newState)
	{
		if (currentState != null)
		{
			currentState.EndState();
		}
		currentState = newState;
		currentState.BeginState();
	}

	private void Update()
	{
		if (currentState != null)
		{
			currentState.UpdateState();
		}
		if (!isAlertState && alertManager.AlertCount > 0)
		{
			isAlertState = true;
			if (OnAlertStateBegin != null)
			{
				OnAlertStateBegin();
			}
		}
		if (isAlertState && alertManager.AlertCount <= 0)
		{
			isAlertState = false;
			if (OnAlertStateEnd != null)
			{
				OnAlertStateEnd();
			}
		}
		if (Input.GetMouseButtonDown(0) && Input.mousePosition.x <= triggerCheatMenuMaxPos.x && Input.mousePosition.y <= triggerCheatMenuMaxPos.y)
		{
			triggerCheatMenuStartTime = Time.time;
		}
	}

	public void SetAreaMapMetaLayer(AreaMapMetaLayer newAreaMapMetaLayer)
	{
		areaMapMetaLayer = newAreaMapMetaLayer;
		Debug.Log("Setting map area: " + areaMapMetaLayer.areaName);
		playerProfile.SetCurrentArea(areaMapMetaLayer.areaName);
		AreaPosition currentAreaPosition = playerProfile.GetCurrentAreaPosition();
		if (currentAreaPosition == null)
		{
			playerProfile.SetCurrentAreaPosition(areaMapMetaLayer.startRow, areaMapMetaLayer.startCol);
			currentAreaPosition = playerProfile.GetCurrentAreaPosition();
		}
		areaMapMetaLayer.currentRow = currentAreaPosition.row;
		areaMapMetaLayer.currentCol = currentAreaPosition.col;
	}

	public int GetLastChapter(int season)
	{
		int num = 0;
		TextAsset storyFile = storyDB.GetStoryFile(season);
		using (StringReader stringReader = new StringReader(storyFile.text))
		{
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split('\t');
				if (array[0] == season.ToString())
				{
					int num2 = Convert.ToInt32(array[1]);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
		}
		Debug.Log("SETTING MAX CHAPTER FOR SEASON " + season + " = " + num);
		return num;
	}

	public int GetLastSeason()
	{
		return storyDB.storyFiles.Count;
	}

	private bool ShouldOpenSdkTestPopup()
	{
		return false;
	}
}
