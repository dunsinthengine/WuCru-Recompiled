using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerStateLevelCompleted : GameControllerState
{
	private GameObject guiRoot;

	private bool missionAccomplishedGUIDone;

	private bool gameOver;

	private bool canLeave;

	private bool doneTransferToCounter;

	private bool missionAccomplishedGUIShown;

	private bool xpScreenShown;

	private MissionAccomplishedGUI missionAccomplishedGUI;

	private const string areaMapMusic = "music/area/map";

	private int CountStarsToGive()
	{
		MissionStats missionStats = GameController.Instance.MissionStats;
		bool flag = true;
		bool flag2 = missionStats.CollectedAllObjects();
		bool flag3 = missionStats.CompletedAllChallenges();
		if (GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(GameController.Instance.CurrentLevel.Seed))
		{
			LevelUnlockData levelUnlockData = GameController.Instance.PlayerProfile.LevelUnlocked[GameController.Instance.CurrentLevel.Seed];
			if (levelUnlockData.starCompletion)
			{
				flag = false;
			}
			if (levelUnlockData.starObjects)
			{
				flag2 = false;
			}
			if (levelUnlockData.starChallenges)
			{
				flag3 = false;
			}
		}
		int num = 0;
		if (flag)
		{
			num++;
		}
		if (flag2)
		{
			num++;
		}
		if (flag3)
		{
			num++;
		}
		return num;
	}

	public override void BeginState()
	{
		InGameGUIController inGameGUIController = UnityEngine.Object.FindObjectOfType<InGameGUIController>();
		if (inGameGUIController != null)
		{
			inGameGUIController.ShowGUI(false);
			inGameGUIController.ShowGUIElement(InGameGUIElement.StudsCounter, true);
			inGameGUIController.ShowGUIElement(InGameGUIElement.Blocker, true);
		}
		guiRoot = null;
		missionAccomplishedGUIDone = false;
		gameOver = false;
		canLeave = false;
		missionAccomplishedGUI = null;
		doneTransferToCounter = false;
		missionAccomplishedGUIShown = false;
		xpScreenShown = false;
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
			SendLevelResultAnalytics();
			HandleMission();
			return;
		}
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenHud screenInstance = instance.GetScreenInstance<ScreenHud>();
		screenInstance.OnLevelFailed();
		GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
	}

	private void SendLevelResultAnalytics()
	{
		Level currentLevel = GameController.Instance.CurrentLevel;
		MissionStats missionStats = GameController.Instance.MissionStats;
		int numStars = (missionStats.CollectedAllObjects() ? 1 : 0) + (missionStats.CompletedAllChallenges() ? 1 : 0) + 1;
		AnalyticsEvents.LogLevelResult(currentLevel.Seed, missionStats.NumChestsToCollect, missionStats.NumChestsCollected, missionStats.NumCrystalsToCollect, missionStats.NumCrystalsCollected, numStars, missionStats.NumEnemies, missionStats.NumEnemiesKilled, missionStats.NumDeadNinjas, missionStats.NumStudsCollected, missionStats.NumChallenges, missionStats.NumChallengesCompleted);
	}

	private void HandleMission()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenHud screenInstance = instance.GetScreenInstance<ScreenHud>();
		screenInstance.OnLevelComplete();
	}

	private void HandleMissionCompletedEvent(object sender, DataEventArgs<bool> args)
	{
		GameController.Instance.MissionController.MissionCompletedEvent -= HandleMissionCompletedEvent;
		string missionID = GameController.Instance.MissionController.CurrentMission.MissionID;
		if (args != null && args.data)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/WuCruMissionUnlockedGUI"));
			MissionPopupGUI component = gameObject.GetComponent<MissionPopupGUI>();
			component.SetImage(true);
			component.OnClose = (MissionPopupGUI.OnCloseDelegate)Delegate.Combine(component.OnClose, new MissionPopupGUI.OnCloseDelegate(ShowMissionStarGUI));
			AnalyticsEvents.LogWuCruMissionEnded(missionID, true, GameController.Instance.CurrentLevel.Seed);
		}
		else
		{
			ShowMissionStarGUI();
			AnalyticsEvents.LogWuCruMissionEnded(missionID, false, GameController.Instance.CurrentLevel.Seed);
		}
	}

	private void ShowMissionStarGUI()
	{
		if (CountStarsToGive() > 0)
		{
			guiRoot = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/MissionStarGUI"));
			MissionStarGUI componentInChildren = guiRoot.GetComponentInChildren<MissionStarGUI>();
			componentInChildren.OnDoneTransferToCounter = (MissionStarGUI.OnDoneTransferToCounterDelegate)Delegate.Combine(componentInChildren.OnDoneTransferToCounter, new MissionStarGUI.OnDoneTransferToCounterDelegate(OnDoneTransferToCounter));
		}
		else
		{
			doneTransferToCounter = true;
		}
	}

	public override void EndState()
	{
		UnityEngine.Object.Destroy(guiRoot);
		StudsCounter studsCounter = UnityEngine.Object.FindObjectOfType<StudsCounter>();
		if (studsCounter != null)
		{
			UnityEngine.Object.Destroy(studsCounter.gameObject.transform.parent.gameObject);
		}
		Debug.Log("--------------------DESTROYING CURRENTLEVEL----------------");
		GameController.Instance.CurrentLevel = null;
	}

	public override void UpdateState()
	{
		if (doneTransferToCounter && !missionAccomplishedGUIShown && (Input.GetMouseButtonDown(0) || CountStarsToGive() == 0))
		{
			UnityEngine.Object.Destroy(guiRoot);
			guiRoot = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/MissionAccomplishedGUI"));
			missionAccomplishedGUI = guiRoot.GetComponentInChildren<MissionAccomplishedGUI>();
			MissionAccomplishedGUI obj = missionAccomplishedGUI;
			obj.OnDone = (MissionAccomplishedGUI.OnDoneDelegate)Delegate.Combine(obj.OnDone, new MissionAccomplishedGUI.OnDoneDelegate(OnMissionAccomplishedGUIDone));
			missionAccomplishedGUIShown = true;
		}
		if (!gameOver && missionAccomplishedGUIDone && Input.GetMouseButtonDown(0) && !canLeave && !xpScreenShown)
		{
			UnityEngine.Object.Destroy(guiRoot);
			xpScreenShown = true;
			guiRoot = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/MissionXPGUI"));
			MissionXPGUI componentInChildren = guiRoot.GetComponentInChildren<MissionXPGUI>();
			componentInChildren.OnDoneTransferToXP = (MissionXPGUI.OnDoneTransferToXPDelegate)Delegate.Combine(componentInChildren.OnDoneTransferToXP, new MissionXPGUI.OnDoneTransferToXPDelegate(OnDoneTransferToXP));
		}
		else if (!gameOver && missionAccomplishedGUIDone && Input.GetMouseButtonDown(0) && canLeave)
		{
			int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
			int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
			chapterCursor++;
			if (GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.IsCurrentObjectiveCompleted() && GameController.Instance.NumChapters(lastPlayedSeason) >= chapterCursor)
			{
				GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
				StartCoroutine(DoLoadScene("TeaFarm"));
			}
			else if (GameController.Instance.levelExtensionController.GetLevelExtensionComplete("DayOfTheDeparted") && !GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Lloyd, 0)))
			{
				GameController.Instance.PlayerProfile.UnlockNinja(Team.GetID(MinifigType.Lloyd, 0));
				GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
				StartCoroutine(DoLoadScene("TeaFarm"));
			}
			else if (GameController.Instance.PlayerProfile.TotalUniqueLevelsPlayed == 3 || (GameController.Instance.PlayerProfile.CompletedMissions.Count < 4 && GameController.Instance.MissionController.CurrentMission == null))
			{
				GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
				StartCoroutine(DoLoadScene("TeaFarm"));
			}
			else
			{
				GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
				StartCoroutine(DoLoadScene("AreaMap"));
			}
		}
		if (gameOver && Input.GetMouseButtonDown(0))
		{
			if (!AudioPlayer.IsPlaying("Audio_Music_music_area_map"))
			{
				AudioPlayer.PlaySound("music/area/map");
			}
			if (GameController.Instance.PlayerProfile.TotalLevelsPlayed < 1)
			{
				GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
				StartCoroutine(DoLoadScene("CharacterCreation"));
			}
			else
			{
				StartCoroutine(DoLoadScene("AreaMap"));
			}
		}
	}

	private void SkipLevelCompleteSequence()
	{
		guiRoot = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/MissionXPGUI"));
		guiRoot.GetComponentInChildren<MissionXPGUI>().SaveState();
		UnityEngine.Object.Destroy(guiRoot);
		DoExit();
	}

	private void DoExit()
	{
		if (!AudioPlayer.IsPlaying("Audio_Music_music_area_map"))
		{
			AudioPlayer.PlaySound("music/area/map");
		}
		int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
		chapterCursor++;
		if (GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.IsCurrentObjectiveCompleted() && GameController.Instance.NumChapters(lastPlayedSeason) >= chapterCursor)
		{
			GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
			StartCoroutine(DoLoadScene("TeaFarm"));
		}
		else if (gameOver && GameController.Instance.PlayerProfile.TotalLevelsPlayed < 1)
		{
			GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
			StartCoroutine(DoLoadScene("CharacterCreation"));
		}
		else
		{
			GameController.Instance.SetState(GetComponent<GameControllerStateIdle>());
			StartCoroutine(DoLoadScene("AreaMap"));
		}
	}

	private IEnumerator DoLoadScene(string sceneName)
	{
		UnityEngine.Object.Destroy(GameController.Instance.LevelVisualizer.levelRoot);
		SceneManager.LoadScene("EmptyLoadScene");
		yield return 0;
		GC.Collect();
		yield return 0;
		SceneManager.LoadScene(sceneName);
	}

	public void OnDoneTransferToCounter()
	{
		doneTransferToCounter = true;
	}

	public void OnMissionAccomplishedGUIDone()
	{
		missionAccomplishedGUIDone = true;
	}

	public void OnDoneTransferToXP()
	{
		canLeave = true;
	}
}
