using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playing : EditorState
{
	private GameObject inGameGUI;

	private Vector3 originalCamPos;

	private Quaternion originalCamRot;

	public Button stopButton;

	public Button debugMenuButton;

	public Dropdown dropdownChallenge;

	public LevelNode mouseDownNode;

	public bool overrideStartPosition;

	public override void Update()
	{
	}

	public override void Begin()
	{
		Debug.Log("Editor Playing state - Begin");
		originalCamPos = Camera.main.gameObject.transform.position;
		originalCamRot = Camera.main.gameObject.transform.rotation;
		GamePlayer.Instance.Reset();
		stopButton.gameObject.SetActive(true);
		debugMenuButton.gameObject.SetActive(true);
		dropdownChallenge.gameObject.SetActive(true);
		EnableGUI();
		if (overrideStartPosition)
		{
			GamePlayer.Instance.EditorPlayFromHere(mouseDownNode.Row, mouseDownNode.Col);
		}
		inGameGUI = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/InGameGUI"));
	}

	public override void End()
	{
		Camera.main.gameObject.transform.position = originalCamPos;
		Camera.main.gameObject.transform.rotation = originalCamRot;
		DisableGUI();
		stopButton.gameObject.SetActive(false);
		debugMenuButton.gameObject.SetActive(false);
		overrideStartPosition = false;
		Object.Destroy(inGameGUI);
		InGameCam pInstance = InGameCam._pInstance;
		if (pInstance != null)
		{
			Object.Destroy(pInstance);
		}
	}

	public override void Pause()
	{
		DisableGUI();
	}

	public override void Resume()
	{
		EnableGUI();
	}

	private void EnableGUI()
	{
		stopButton.onClick.AddListener(() =>
		{
			GamePlayer.Instance.SetState(GamePlayerState.Idle);
			EditorStateMachine.Instance.SetState(GetComponent<Editing>());
		});
		debugMenuButton.onClick.AddListener(() =>
		{
			if (Object.FindObjectOfType<CheatMenu>() == null)
			{
				Object.Instantiate(Resources.Load("Prefabs/Dialogs/CheatMenu"));
			}
		});
		dropdownChallenge.options.Clear();
		LevelChallengeSelector levelChallengeSelector = new LevelChallengeSelector();
		foreach (LevelChallengeBase potentialChallenge in levelChallengeSelector.GetPotentialChallenges(EditorStateMachine.Instance.level, EditorStateMachine.Instance.GameplayEngine))
		{
			dropdownChallenge.options.Add(new Dropdown.OptionData(potentialChallenge.GetType().ToString()));
		}
		dropdownChallenge.captionText.text = "Challenge";
		dropdownChallenge.onValueChanged.AddListener(OnChallengeSelect);
	}

	private void OnChallengeSelect(int i)
	{
		LevelChallengeSelector levelChallengeSelector = new LevelChallengeSelector();
		List<LevelChallengeBase> potentialChallenges = levelChallengeSelector.GetPotentialChallenges(EditorStateMachine.Instance.level, EditorStateMachine.Instance.GameplayEngine);
		EditorStateMachine.Instance.level.Challenge = potentialChallenges[i];
		ChallengeGUI challengeGUI = Object.FindObjectOfType<ChallengeGUI>();
		challengeGUI.Initialize();
	}

	private void DisableGUI()
	{
		stopButton.onClick.RemoveAllListeners();
		dropdownChallenge.onValueChanged.RemoveAllListeners();
	}
}
