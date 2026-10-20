using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Editing : EditorState
{
	public Button playButton;

	public Button playSequenceButton;

	public Button createLevelButton;

	public Button nextSeedButton;

	public Button saveLevelButton;

	public Button loadLevelButton;

	public Dropdown dropdownStyle;

	public Dropdown dropdownThemeCollection;

	public Dropdown dropdownThemeIndices;

	public Dropdown dropdownPrimaryPower;

	public Dropdown dropdownSecondaryPower;

	public Text textEnvironmentSetting;

	public DynamicContextMenu contextMenu;

	public Canvas editorGUI;

	private EditorCam editorCam;

	private Camera cam;

	private LevelNode mouseDownNode;

	private Vector3 lastPickingScreenPosition;

	private string filePath = "C:/NinjagoLevels";

	private bool dropDownInitialized;

	public int currentSeed
	{
		get
		{
			return PlayerPrefs.GetInt("NinjagoEditorSeed", 0);
		}
		set
		{
			PlayerPrefs.SetInt("NinjagoEditorSeed", value);
			PlayerPrefs.Save();
		}
	}

	public override void Begin()
	{
		editorGUI.enabled = true;
		cam = Camera.main;
		editorCam = cam.gameObject.AddComponent<EditorCam>();
		EnableGUI();
		if (!dropDownInitialized)
		{
			dropdownStyle.options.Clear();
			for (int i = 0; i < EditorStateMachine.Instance.styleDB._styles.Count; i++)
			{
				if (!(EditorStateMachine.Instance.styleDB._styles[i] == null))
				{
					dropdownStyle.options.Add(new Dropdown.OptionData(EditorStateMachine.Instance.styleDB._styles[i].styleName));
				}
			}
			int num = PlayerPrefs.GetInt("style", 0);
			dropdownStyle.captionText.text = EditorStateMachine.Instance.styleDB._styles[num].styleName;
			dropdownStyle.value = num;
			dropdownThemeCollection.options.Clear();
			foreach (string key in EditorStateMachine.Instance.themeDB.themeCollections.Keys)
			{
				dropdownThemeCollection.options.Add(new Dropdown.OptionData(key));
			}
			int num2 = PlayerPrefs.GetInt("theme", 0);
			dropdownThemeCollection.captionText.text = EditorStateMachine.Instance.themeDB.themeCollections.keys[num2];
			dropdownThemeCollection.value = num2;
			dropdownThemeCollection.onValueChanged.AddListener(OnThemeCollectionSelection);
			dropdownThemeIndices.options.Clear();
			foreach (string key2 in EditorStateMachine.Instance.themeDB.themeCollections.values[dropdownThemeCollection.value].themes.keys)
			{
				dropdownThemeIndices.options.Add(new Dropdown.OptionData(key2));
			}
			int num3 = PlayerPrefs.GetInt("seq", 0);
			dropdownThemeIndices.captionText.text = EditorStateMachine.Instance.themeDB.themeCollections.values[dropdownThemeCollection.value].themes.keys[num3];
			dropdownThemeIndices.value = num3;
			dropDownInitialized = true;
			dropdownPrimaryPower.options.Clear();
			string[] names = Enum.GetNames(typeof(ElementalPower));
			foreach (string text in names)
			{
				dropdownPrimaryPower.options.Add(new Dropdown.OptionData(text));
			}
			int num4 = PlayerPrefs.GetInt("primpow", 0);
			dropdownPrimaryPower.captionText.text = dropdownPrimaryPower.options[num4].text;
			dropdownPrimaryPower.value = num4;
			dropdownSecondaryPower.options.Clear();
			string[] names2 = Enum.GetNames(typeof(ElementalPower));
			foreach (string text2 in names2)
			{
				dropdownSecondaryPower.options.Add(new Dropdown.OptionData(text2));
			}
			int num5 = PlayerPrefs.GetInt("secpow", 0);
			dropdownSecondaryPower.captionText.text = dropdownSecondaryPower.options[num5].text;
			dropdownSecondaryPower.value = num5;
		}
		textEnvironmentSetting.text = EditorStateMachine.Instance.levelVisualizer.environmentName;
	}

	public void OnThemeCollectionSelection(int index)
	{
		dropdownThemeIndices.options.Clear();
		foreach (string key in EditorStateMachine.Instance.themeDB.themeCollections.values[dropdownThemeCollection.value].themes.keys)
		{
			dropdownThemeIndices.options.Add(new Dropdown.OptionData(key));
		}
		dropdownThemeIndices.captionText.text = EditorStateMachine.Instance.themeDB.themeCollections.values[dropdownThemeCollection.value].themes.keys[0];
	}

	public override void End()
	{
		editorGUI.enabled = false;
		UnityEngine.Object.Destroy(editorCam);
		DisableGUI();
	}

	public override void Pause()
	{
		DisableGUI();
	}

	public override void Resume()
	{
		EnableGUI();
		textEnvironmentSetting.text = EditorStateMachine.Instance.levelVisualizer.environmentName;
	}

	public override void Update()
	{
	}

	private void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(1))
		{
			mouseDownNode = GetPickingLevelNode(true);
		}
		if (Input.GetMouseButtonUp(1))
		{
			LevelNode pickingLevelNode = GetPickingLevelNode(true);
			if (pickingLevelNode != null && mouseDownNode == pickingLevelNode)
			{
				contextMenu.screenPosition = lastPickingScreenPosition;
				contextMenu.AddMenuItem("PlayFromHere", PlayFromHere);
				contextMenu.AddMenuItem("Add Object", AddObject);
			}
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			CreateLevel();
		}
	}

	private void PlayFromHere()
	{
		Debug.Log("PlayFromHere");
		Playing component = GetComponent<Playing>();
		component.mouseDownNode = mouseDownNode;
		component.overrideStartPosition = true;
		EditorStateMachine.Instance.SetState(component);
	}

	private void AddObject()
	{
		Debug.Log("Add Object....should open new context menu with types?");
	}

	private void EnableGUI()
	{
		playButton.onClick.AddListener(() =>
		{
			if (EditorStateMachine.Instance.level == null)
			{
				CreateLevel();
			}
			EditorStateMachine.Instance.SetState(GetComponent<Playing>());
		});
		playSequenceButton.onClick.AddListener(() =>
		{
			if (EditorStateMachine.Instance.level == null)
			{
				CreateLevel();
			}
			EditorStateMachine.Instance.SetState(GetComponent<PlayingSequence>());
			(EditorStateMachine.Instance.CurrentState as PlayingSequence).styleIndex = dropdownStyle.value;
			(EditorStateMachine.Instance.CurrentState as PlayingSequence).themeCollectionIndex = dropdownThemeCollection.value;
		});
		createLevelButton.onClick.AddListener(() =>
		{
			ShowCreateLevel();
		});
		nextSeedButton.onClick.AddListener(() =>
		{
			NextSeed();
		});
		saveLevelButton.onClick.AddListener(() =>
		{
			SaveLevel();
		});
		loadLevelButton.onClick.AddListener(() =>
		{
			LoadLevel();
		});
	}

	private void NextSeed()
	{
		currentSeed++;
		CreateLevel();
	}

	private void CreateLevel()
	{
		if (EditorStateMachine.Instance.levelVisualizer.levelRoot != null)
		{
			UnityEngine.Object.Destroy(EditorStateMachine.Instance.levelVisualizer.levelRoot);
		}
		if (EditorStateMachine.Instance.levelVisualizer.LevelMeshGO != null)
		{
			UnityEngine.Object.Destroy(EditorStateMachine.Instance.levelVisualizer.LevelMeshGO);
		}
		int value = dropdownStyle.value;
		int value2 = dropdownThemeCollection.value;
		int value3 = dropdownThemeIndices.value;
		int value4 = EditorStateMachine.Instance.GetComponent<Editing>().dropdownPrimaryPower.value;
		int value5 = EditorStateMachine.Instance.GetComponent<Editing>().dropdownSecondaryPower.value;
		PlayerPrefs.SetInt("style", value);
		PlayerPrefs.SetInt("theme", value2);
		PlayerPrefs.SetInt("seq", value3);
		PlayerPrefs.SetInt("seed", currentSeed);
		PlayerPrefs.SetInt("primpow", value4);
		PlayerPrefs.SetInt("secpow", value5);
		PlayerPrefs.SetInt("seed", currentSeed);
		PlayerPrefs.Save();
		LevelEngine levelEngine = new LevelEngine();
		LevelThemeCollection levelThemeCollection = EditorStateMachine.Instance.themeDB.themeCollections.values[dropdownThemeCollection.value];
		LevelTheme levelTheme = levelThemeCollection.themes.values[dropdownThemeIndices.value];
		EditorStateMachine.Instance.level = levelEngine.GenerateLevel(levelThemeCollection, levelTheme, EditorStateMachine.Instance.styleDB._styles[dropdownStyle.value], currentSeed, (ElementalPower)value4, (ElementalPower)value5);
		GameController.Instance.CurrentLevel = EditorStateMachine.Instance.level;
		CreateLevel component = EditorStateMachine.Instance.GetComponent<CreateLevel>();
		if (component.gamePlayerGO != null)
		{
			UnityEngine.Object.Destroy(component.gamePlayerGO);
		}
		component.gamePlayerGO = new GameObject();
		component.gamePlayerGO.name = "GamePlayer";
		GamePlayer gamePlayer = component.gamePlayerGO.AddComponent<GamePlayer>();
		gamePlayer.Initialize(EditorStateMachine.Instance.level);
		EditorStateMachine.Instance.levelVisualizer.BuildLevel(EditorStateMachine.Instance.level, dropdownStyle.value);
		string text = dropdownStyle.captionText.text;
		EditorStateMachine.Instance.debugGUI.SetLevelInfo(currentSeed, text, levelThemeCollection.collectionName, levelTheme.themeName);
		textEnvironmentSetting.text = EditorStateMachine.Instance.levelVisualizer.environmentName;
	}

	private void DisableGUI()
	{
		playButton.onClick.RemoveAllListeners();
		playSequenceButton.onClick.RemoveAllListeners();
		createLevelButton.onClick.RemoveAllListeners();
		nextSeedButton.onClick.RemoveAllListeners();
		saveLevelButton.onClick.RemoveAllListeners();
		loadLevelButton.onClick.RemoveAllListeners();
	}

	private void ShowCreateLevel()
	{
		EditorStateMachine.Instance.PushState(base.gameObject.GetComponent<CreateLevel>());
		(EditorStateMachine.Instance.CurrentState as CreateLevel).styleIndex = dropdownStyle.value;
		(EditorStateMachine.Instance.CurrentState as CreateLevel).themeCollectionIndex = dropdownThemeCollection.value;
		(EditorStateMachine.Instance.CurrentState as CreateLevel).themeIndicesIndex = dropdownThemeIndices.value;
	}

	private void SaveLevel()
	{
		if (EditorStateMachine.Instance.level == null || EditorStateMachine.Instance.level.Nodes == null || EditorStateMachine.Instance.level.Nodes.Length <= 0)
		{
			Debug.Log("Level Visualizer is null");
			Message component = GetComponent<Message>();
			component.msg = "No level in memory!";
			EditorStateMachine.Instance.PushState(component);
			return;
		}
		byte[] array = LevelSerializer.SerializeLevel(EditorStateMachine.Instance.level);
		string path = filePath + "/" + EditorStateMachine.Instance.level.Seed + ".lvl";
		if (!Directory.Exists(filePath))
		{
			Directory.CreateDirectory(filePath);
		}
		if (File.Exists(path))
		{
			File.Create(path);
		}
		FileStream fileStream = File.OpenWrite(path);
		fileStream.Write(array, 0, array.Length);
		Message component2 = GetComponent<Message>();
		component2.msg = "Successfully saved file";
		EditorStateMachine.Instance.PushState(component2);
	}

	private void LoadLevel()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/LoadLevelDialog"));
		LoadLevelDialog component = gameObject.GetComponent<LoadLevelDialog>();
		component.OnClose = (LoadLevelDialog.OnCloseDelegate)Delegate.Combine(component.OnClose, new LoadLevelDialog.OnCloseDelegate(LoadLevelResponse));
	}

	private void LoadLevelResponse(int loadSeed)
	{
		string path = filePath + "/" + loadSeed + ".lvl";
		if (!File.Exists(path))
		{
			Message component = GetComponent<Message>();
			component.msg = "No file exists for seed " + loadSeed;
			EditorStateMachine.Instance.PushState(component);
			return;
		}
		byte[] bytes = File.ReadAllBytes(path);
		EditorStateMachine.Instance.level = LevelSerializer.DeserializeLevel(bytes);
		UnityEngine.Random.seed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
		int environmentSettingsIndex = UnityEngine.Random.Range(0, GameController.Instance.CurrentLevel.LevelStyle.environmentSettings.Count - 1);
		EditorStateMachine.Instance.levelVisualizer.BuildLevel(EditorStateMachine.Instance.level, 0, environmentSettingsIndex);
		Debug.Log("Loaded level with " + EditorStateMachine.Instance.level.ChunkDB.Chunks.Count + " chunks");
	}

	private LevelNode GetPickingLevelNode(bool onlyUpwardsFaces)
	{
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] array = Physics.RaycastAll(ray);
		if (array.Length > 0)
		{
			LevelNode result = null;
			float num = float.PositiveInfinity;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				MeshCollider component = raycastHit.collider.GetComponent<MeshCollider>();
				if (component != null)
				{
					lastPickingScreenPosition = cam.WorldToScreenPoint(raycastHit.point);
					if ((!onlyUpwardsFaces || Vector3.Dot(raycastHit.normal, Vector3.up) > 0.8f) && raycastHit.distance < num)
					{
						result = LevelHelper.Vector3ToNode(ray.GetPoint(raycastHit.distance), GamePlayer.Instance.Level.Nodes);
						num = raycastHit.distance;
					}
				}
			}
			return result;
		}
		return null;
	}
}
