using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenDojo : ScreenBase
{
	public GameObject _back;

	public GameObject _skip;

	public ScreenObjectTransitionWidget _topBar;

	public UILabel _debug;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_debug.text = string.Empty;
		ShowBack(false);
		_topBar.SetToTweenInStartPos();
		bool showGarageGoldenTrail = UnlockableVehicleParts._pInstance.vehicleGoldenTrail();
		ScreenHeaderNavigation._pInstance.ConfigureHeader(OnSkills, OnMap, OnGarage, null, OnSettings, false, showGarageGoldenTrail, false);
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
		screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_DOJO;
		instance.ShowPopupOverlay(screenInstance);
	}

	public void OnGarage()
	{
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenGarage screenInstance = instance.GetScreenInstance<ScreenGarage>();
		Navigate(screenInstance);
	}

	public void OnRoster()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenRoster screenInstance = instance.GetScreenInstance<ScreenRoster>();
		Navigate(screenInstance);
	}

	public void OnSkills()
	{
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSkills screenInstance = instance.GetScreenInstance<ScreenSkills>();
		Navigate(screenInstance);
	}

	public void OnMap()
	{
		SceneManager.LoadScene("Empty");
		if (GlobalInGameData.ShouldLaunchHandsOfTimeCampaign())
		{
			GlobalInGameData.LaunchHandsOfTimeCampaign();
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenHeaderNavigation._pInstance.CloseHeader();
			ScreenCutsceneHandsOfTimeIntro screenInstance = instance.GetScreenInstance<ScreenCutsceneHandsOfTimeIntro>();
			Navigate(screenInstance);
			return;
		}
		ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
		bool flag = GameController.Instance.PlayerProfile.TotalLevelsPlayed <= 1;
		if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap() || flag)
		{
			ScreenWorldMap screenInstance2 = instance2.GetScreenInstance<ScreenWorldMap>();
			Navigate(screenInstance2);
		}
		else
		{
			ScreenMap screenInstance3 = instance2.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance3);
		}
	}

	public void OnBack()
	{
		DojoController._pInstance.OnReturnToRoot();
		ScreenStickerBook screenInstance = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenStickerBook>();
		if (screenInstance.gameObject.activeInHierarchy)
		{
			screenInstance.Close();
		}
	}

	public void ShowNoInternet()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenNoConnection screenInstance = instance.GetScreenInstance<ScreenNoConnection>();
		if (!screenInstance.gameObject.activeInHierarchy)
		{
			instance.ShowPopupOverlay(screenInstance);
		}
	}

	public void ShowBack(bool show)
	{
		_back.SetActive(show);
		if (show)
		{
			if (!_topBar._pConsideredTweenedIn)
			{
				_topBar.TweenIn();
			}
		}
		else if (_topBar._pConsideredTweenedIn)
		{
			_topBar.TweenOut();
		}
	}

	public void OnStickerbook()
	{
		ScreenStickerBook screenInstance = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenStickerBook>();
		ShowPopupOverlay(screenInstance);
	}

	public void OnSkip(bool arrive)
	{
		if (ControllerVideoPlayer._pInstance != null && ManagerCrossPromo._pInstance._pCanPlayVideos)
		{
			ControllerVideoPlayer._pInstance.Next();
		}
	}

	public void OnCinema(bool arrive)
	{
		if (_skip != null)
		{
			_skip.SetActive(arrive);
		}
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
	}

	protected override void Update()
	{
		base.Update();
	}
}
