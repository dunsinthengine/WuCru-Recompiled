using System;
using UnityEngine;

public class CreateLevel : EditorState
{
	public int styleIndex;

	public int themeCollectionIndex;

	public int themeIndicesIndex;

	private GameObject createLevelDialogGO;

	private CreateLevelDialog createLevelDialog;

	public GameObject gamePlayerGO;

	public override void Begin()
	{
		createLevelDialogGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/CreateLevelDialog"));
		createLevelDialog = createLevelDialogGO.GetComponent<CreateLevelDialog>();
		createLevelDialog.inputFieldSeed.text = EditorStateMachine.Instance.GetComponent<Editing>().currentSeed.ToString();
		createLevelDialog.buttonOK.onClick.AddListener(() =>
		{
			if (EditorStateMachine.Instance.levelVisualizer.levelRoot != null)
			{
				UnityEngine.Object.Destroy(EditorStateMachine.Instance.levelVisualizer.levelRoot);
			}
			if (EditorStateMachine.Instance.levelVisualizer.LevelMeshGO != null)
			{
				UnityEngine.Object.Destroy(EditorStateMachine.Instance.levelVisualizer.LevelMeshGO);
			}
			int num = Convert.ToInt32(createLevelDialog.inputFieldSeed.text);
			EditorStateMachine.Instance.GetComponent<Editing>().currentSeed = num;
			LevelEngine levelEngine = new LevelEngine();
			LevelThemeCollection levelThemeCollection = EditorStateMachine.Instance.themeDB.themeCollections.values[themeCollectionIndex];
			LevelTheme levelTheme = levelThemeCollection.themes.values[themeIndicesIndex];
			EditorStateMachine.Instance.level = levelEngine.GenerateLevel(levelThemeCollection, levelTheme, EditorStateMachine.Instance.styleDB._styles[styleIndex], num, (ElementalPower)EditorStateMachine.Instance.GetComponent<Editing>().dropdownPrimaryPower.value, (ElementalPower)EditorStateMachine.Instance.GetComponent<Editing>().dropdownSecondaryPower.value);
			GameController.Instance.CurrentLevel = EditorStateMachine.Instance.level;
			if (gamePlayerGO != null)
			{
				UnityEngine.Object.Destroy(gamePlayerGO);
			}
			gamePlayerGO = new GameObject();
			gamePlayerGO.name = "GamePlayer";
			GamePlayer gamePlayer = gamePlayerGO.AddComponent<GamePlayer>();
			gamePlayer.Initialize(EditorStateMachine.Instance.level);
			UnityEngine.Random.seed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
			int environmentSettingsIndex = UnityEngine.Random.Range(0, EditorStateMachine.Instance.level.LevelStyle.environmentSettings.Count - 1);
			EditorStateMachine.Instance.levelVisualizer.BuildLevel(EditorStateMachine.Instance.level, styleIndex, environmentSettingsIndex);
			string text = EditorStateMachine.Instance.GetComponent<Editing>().dropdownStyle.captionText.text;
			EditorStateMachine.Instance.debugGUI.SetLevelInfo(num, text, levelThemeCollection.collectionName, levelTheme.themeName);
			EditorStateMachine.Instance.PopState();
		});
	}

	public override void End()
	{
		UnityEngine.Object.Destroy(createLevelDialogGO);
	}
}
