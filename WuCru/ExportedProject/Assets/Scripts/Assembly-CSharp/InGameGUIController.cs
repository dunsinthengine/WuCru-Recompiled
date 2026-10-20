using UnityEngine;

public class InGameGUIController : MonoBehaviour
{
	public GameObject ninjaTeamRoot;

	public GameObject activeNinjaRoot;

	public GameObject healthBarRoot;

	public GameObject ninjaActionsRoot;

	public GameObject spinjitzuRoot;

	public GameObject levelProgressRoot;

	public GameObject studsCounterRoot;

	public GameObject navigationRoot;

	public GameObject blocker;

	public GameObject dotdProgressRoot;

	public static InGameGUIController Instance { get; private set; }

	private void Awake()
	{
		ShowGUI(true);
		Instance = this;
	}

	public void OnApplicationFocus(bool pause)
	{
		if (pause)
		{
			ShowQuitDialog();
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			ShowQuitDialog();
		}
	}

	public void ShowQuitDialog()
	{
		if (!(ScreenHud._pInstance != null) || ScreenHud._pInstance._pFtueState == ScreenHud.FTUE_STATE.NONE)
		{
			DisableButtons();
			GameControllerStatePlaying gameControllerStatePlaying = GameController.Instance.currentState as GameControllerStatePlaying;
			if (gameControllerStatePlaying != null && !gameControllerStatePlaying.IsConfirmingQuit && !gameControllerStatePlaying.FadeInProgress)
			{
				AudioPlayer.PlaySound("ui/button/click");
				gameControllerStatePlaying.OnQuitPlaying();
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Menu) || Input.GetKeyDown(KeyCode.Escape))
		{
			ScreenPause screenInstance = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenPause>();
			if (screenInstance._pCurrentTweenType == ScreenBase.ScreenTweenType.Idle && !screenInstance.gameObject.activeInHierarchy)
			{
				ShowQuitDialog();
			}
		}
	}

	public void OnDestroy()
	{
		Instance = null;
	}

	public void ShowGUI(bool visible)
	{
		levelProgressRoot.gameObject.SetActive(visible);
		studsCounterRoot.gameObject.SetActive(visible);
		navigationRoot.gameObject.SetActive(visible);
		if (GameController.Instance.MissionController.CurrentMission != null)
		{
		}
		if (GameController.Instance.CurrentLevel == null || GameController.Instance.CurrentLevel.Challenge != null)
		{
		}
		dotdProgressRoot.SetActive(visible);
		blocker.SetActive(false);
	}

	public void DisableButtons()
	{
	}

	public void EnableButtons()
	{
	}

	public void ShowGUIElement(InGameGUIElement element, bool visible)
	{
		switch (element)
		{
		case InGameGUIElement.NinjaTeam:
			ninjaTeamRoot.gameObject.SetActive(visible);
			break;
		case InGameGUIElement.HealthBar:
			break;
		case InGameGUIElement.NinjaActions:
			break;
		case InGameGUIElement.SpinjitzuButton:
			if (GameController.Instance.PlayerProfile.TotalUniqueLevelsPlayed > 1)
			{
				spinjitzuRoot.gameObject.SetActive(visible);
			}
			else
			{
				spinjitzuRoot.gameObject.SetActive(false);
			}
			break;
		case InGameGUIElement.LevelProgress:
			levelProgressRoot.gameObject.SetActive(visible);
			break;
		case InGameGUIElement.StudsCounter:
			studsCounterRoot.gameObject.SetActive(visible);
			break;
		case InGameGUIElement.Navigation:
			navigationRoot.gameObject.SetActive(visible);
			break;
		case InGameGUIElement.Mission:
			break;
		case InGameGUIElement.Challenge:
			break;
		case InGameGUIElement.Blocker:
			blocker.SetActive(false);
			break;
		case InGameGUIElement.DOTDProgress:
			dotdProgressRoot.gameObject.SetActive(visible);
			break;
		}
	}
}
