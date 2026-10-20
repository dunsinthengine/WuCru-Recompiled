using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayingSequence : EditorState
{
	public Canvas inGameGUI;

	public Button stopButton;

	public int styleIndex;

	public int themeCollectionIndex;

	private int seed;

	private Vector3 originalCamPos;

	private Quaternion originalCamRot;

	private GameObject gamePlayerGO;

	private int currentSeq;

	public override void Update()
	{
	}

	public override void Begin()
	{
		originalCamPos = Camera.main.gameObject.transform.position;
		originalCamRot = Camera.main.gameObject.transform.rotation;
		seed = EditorStateMachine.Instance.GetComponent<Editing>().currentSeed;
		currentSeq = 0;
		CreateMap(currentSeq);
		GamePlayer.Instance.Reset();
		inGameGUI.enabled = true;
		EnableGUI();
	}

	public override void End()
	{
		Camera.main.gameObject.transform.position = originalCamPos;
		Camera.main.gameObject.transform.rotation = originalCamRot;
		inGameGUI.enabled = false;
		DisableGUI();
	}

	public override void Pause()
	{
		DisableGUI();
	}

	public override void Resume()
	{
		EnableGUI();
	}

	private void CreateMap(int seq)
	{
		LevelEngine levelEngine = new LevelEngine();
		LevelThemeCollection levelThemeCollection = EditorStateMachine.Instance.themeDB.themeCollections.values[themeCollectionIndex];
		LevelTheme levelTheme = levelThemeCollection.themes.values[seq];
		EditorStateMachine.Instance.level = levelEngine.GenerateLevel(levelThemeCollection, levelTheme, EditorStateMachine.Instance.styleDB._styles[styleIndex], seed, ElementalPower.None, ElementalPower.None);
		if (gamePlayerGO != null)
		{
			UnityEngine.Object.Destroy(gamePlayerGO);
		}
		gamePlayerGO = new GameObject();
		gamePlayerGO.name = "GamePlayer";
		GamePlayer gamePlayer = gamePlayerGO.AddComponent<GamePlayer>();
		gamePlayer.Initialize(EditorStateMachine.Instance.level);
		UnityEngine.Random.seed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
		int environmentSettingsIndex = UnityEngine.Random.Range(0, GameController.Instance.CurrentLevel.LevelStyle.environmentSettings.Count - 1);
		EditorStateMachine.Instance.levelVisualizer.BuildLevel(EditorStateMachine.Instance.level, styleIndex, environmentSettingsIndex);
		GamePlayer.Instance.SetState(GamePlayerState.Exploring);
		Character character = GamePlayer.Instance.Player.Character;
		character.onMissionAccomplished = (Character.OnMissionAccomplishedDelegate)Delegate.Combine(character.onMissionAccomplished, new Character.OnMissionAccomplishedDelegate(OnMissionAccomplished));
		string text = EditorStateMachine.Instance.GetComponent<Editing>().dropdownStyle.captionText.text;
		EditorStateMachine.Instance.debugGUI.SetLevelInfo(seed, text, levelThemeCollection.collectionName, levelTheme.themeName);
	}

	public void OnMissionAccomplished()
	{
		currentSeq++;
		LevelThemeCollection levelThemeCollection = EditorStateMachine.Instance.themeDB.themeCollections.values[themeCollectionIndex];
		if (currentSeq >= levelThemeCollection.themes.Count)
		{
			Message component = EditorStateMachine.Instance.GetComponent<Message>();
			component.msg = "Mission Accomplished!";
			component.OnCloseAction = EndSequence;
			EditorStateMachine.Instance.PushState(component);
		}
		else
		{
			CreateMap(currentSeq);
		}
	}

	private void EndSequence()
	{
		DisableGUI();
		GamePlayer.Instance.SetState(GamePlayerState.Idle);
		EditorStateMachine.Instance.SetState(GetComponent<Editing>());
	}

	private void EnableGUI()
	{
		stopButton.onClick.AddListener(() =>
		{
			GamePlayer.Instance.SetState(GamePlayerState.Idle);
			EditorStateMachine.Instance.SetState(GetComponent<Editing>());
			Camera.main.GetComponent<AudioListener>().enabled = true;
		});
	}

	private void DisableGUI()
	{
		stopButton.onClick.RemoveAllListeners();
	}
}
