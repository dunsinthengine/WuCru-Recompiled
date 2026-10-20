using System.Collections.Generic;
using Cape.LegoSdk.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSettings : ScreenBase
{
	public enum RETURN_SCREEN_TYPE
	{
		SCREEN_MAP = 0,
		SCREEN_GARAGE = 1,
		SCREEN_SKILLS = 2,
		SCREEN_ROSTER = 3,
		SCREEN_STATS = 4,
		SCREEN_WORLD = 5,
		SCREEN_DOJO = 6
	}

	private RETURN_SCREEN_TYPE _pReturnLoc;

	public UISprite _musicTick;

	public UISprite _SfxTick;

	public UILabel _versionLabel;

	private UIButton[] _buttonList;

	private BoxCollider[] _bcList;

	private List<BoxCollider> _activeList = new List<BoxCollider>();

	public RETURN_SCREEN_TYPE _returnLoc
	{
		set
		{
			_pReturnLoc = value;
		}
	}

	protected override void OnShowScreen()
	{
		_activeList.Clear();
		base.OnShowScreen();
		_versionLabel.text = "v_" + AnvilBuildInfo._pBambooVersion;
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenHeaderNavigation._pInstance.DisableButtonsSettings();
		if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_MAP)
		{
			ScreenMap screenInstance = instance.GetScreenInstance<ScreenMap>();
			_buttonList = screenInstance.GetComponentsInChildren<UIButton>();
			for (int i = 0; i < _buttonList.Length; i++)
			{
				BoxCollider component = _buttonList[i].gameObject.GetComponent<BoxCollider>();
				if (component != null && component.enabled)
				{
					_activeList.Add(component);
					component.enabled = false;
				}
			}
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_GARAGE)
		{
			ScreenGarage screenInstance2 = instance.GetScreenInstance<ScreenGarage>();
			_bcList = screenInstance2.GetComponentsInChildren<BoxCollider>();
			for (int j = 0; j < _bcList.Length; j++)
			{
				BoxCollider boxCollider = _bcList[j];
				if (boxCollider != null && boxCollider.enabled)
				{
					_activeList.Add(boxCollider);
					boxCollider.enabled = false;
				}
			}
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_SKILLS)
		{
			ScreenSkills screenInstance3 = instance.GetScreenInstance<ScreenSkills>();
			_buttonList = screenInstance3.GetComponentsInChildren<UIButton>();
			for (int k = 0; k < _buttonList.Length; k++)
			{
				BoxCollider component2 = _buttonList[k].gameObject.GetComponent<BoxCollider>();
				if (component2 != null && component2.enabled)
				{
					_activeList.Add(component2);
					component2.enabled = false;
				}
			}
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_ROSTER)
		{
			ScreenRoster screenInstance4 = instance.GetScreenInstance<ScreenRoster>();
			_buttonList = screenInstance4.GetComponentsInChildren<UIButton>();
			for (int l = 0; l < _buttonList.Length; l++)
			{
				BoxCollider component3 = _buttonList[l].gameObject.GetComponent<BoxCollider>();
				if (component3 != null && component3.enabled)
				{
					_activeList.Add(component3);
					component3.enabled = false;
				}
			}
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_STATS)
		{
			ScreenStats screenInstance5 = instance.GetScreenInstance<ScreenStats>();
			_buttonList = screenInstance5.GetComponentsInChildren<UIButton>();
			for (int m = 0; m < _buttonList.Length; m++)
			{
				BoxCollider component4 = _buttonList[m].gameObject.GetComponent<BoxCollider>();
				if (component4 != null && component4.enabled)
				{
					_activeList.Add(component4);
					component4.enabled = false;
				}
			}
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_WORLD)
		{
			ScreenWorldMap screenInstance6 = instance.GetScreenInstance<ScreenWorldMap>();
			_bcList = screenInstance6.GetComponentsInChildren<BoxCollider>();
			for (int n = 0; n < _bcList.Length; n++)
			{
				BoxCollider boxCollider2 = _bcList[n];
				if (boxCollider2 != null && boxCollider2.enabled)
				{
					_activeList.Add(boxCollider2);
					boxCollider2.enabled = false;
				}
			}
		}
		_musicTick.enabled = true;
		_SfxTick.enabled = true;
		if (GameController.Instance.PlayerProfile.MusicDisabled)
		{
			_musicTick.enabled = false;
		}
		if (GameController.Instance.PlayerProfile.SoundFXDisabled)
		{
			_SfxTick.enabled = false;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			OnBackButton();
		}
	}

	public void toggleMusic()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		if (GameController.Instance.PlayerProfile.MusicDisabled)
		{
			if (GameAudioManager.Instance == null)
			{
				Debug.LogError("No Game Audio Manager");
				return;
			}
			GameAudioManager.Instance.MusicMuted = false;
			GameController.Instance.PlayerProfile.MusicDisabled = false;
		}
		else
		{
			if (GameAudioManager.Instance == null)
			{
				Debug.LogError("No Game Audio Manager");
				return;
			}
			GameAudioManager.Instance.MusicMuted = true;
			GameController.Instance.PlayerProfile.MusicDisabled = true;
		}
		_musicTick.enabled = !GameAudioManager.Instance.MusicMuted;
	}

	public void toggleSFX()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		if (GameController.Instance.PlayerProfile.SoundFXDisabled)
		{
			if (GameAudioManager.Instance == null)
			{
				Debug.LogError("No Game Audio Manager");
				return;
			}
			GameController.Instance.PlayerProfile.SoundFXDisabled = false;
			GameAudioManager.Instance.FxMuted = false;
		}
		else
		{
			if (GameAudioManager.Instance == null)
			{
				Debug.LogError("No Game Audio Manager");
				return;
			}
			GameController.Instance.PlayerProfile.SoundFXDisabled = true;
			GameAudioManager.Instance.FxMuted = true;
		}
		_SfxTick.enabled = !GameAudioManager.Instance.FxMuted;
	}

	public void onTerms()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		LegoCoreSdk.Instance.ShowAllPolicies();
	}

	public void onCredits()
	{
	}

	public void onSettings()
	{
		OnBackButton();
	}

	public void OnBackButton()
	{
		AudioPlayer.PlaySound("ui/button/back/click");
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		int totalLevelsPlayed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
		if (_pReturnLoc != RETURN_SCREEN_TYPE.SCREEN_DOJO)
		{
			SceneManager.LoadScene("Empty");
		}
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		for (int i = 0; i < _activeList.Count; i++)
		{
			_activeList[i].enabled = true;
		}
		if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_MAP)
		{
			ScreenMap screenInstance = instance.GetScreenInstance<ScreenMap>();
			ScreenHeaderNavigation._pInstance.EnableButtonsSettings();
			EndFlow();
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_GARAGE)
		{
			ScreenGarage screenInstance2 = instance.GetScreenInstance<ScreenGarage>();
			ScreenHeaderNavigation._pInstance.EnableButtons();
			EndFlow();
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_SKILLS)
		{
			ScreenSkills screenInstance3 = instance.GetScreenInstance<ScreenSkills>();
			ScreenHeaderNavigation._pInstance.EnableButtons();
			EndFlow();
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_ROSTER)
		{
			ScreenRoster screenInstance4 = instance.GetScreenInstance<ScreenRoster>();
			ScreenHeaderNavigation._pInstance.EnableButtons();
			EndFlow();
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_STATS)
		{
			ScreenStats screenInstance5 = instance.GetScreenInstance<ScreenStats>();
			ScreenHeaderNavigation._pInstance.EnableButtons();
			EndFlow();
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_WORLD)
		{
			ScreenWorldMap screenInstance6 = instance.GetScreenInstance<ScreenWorldMap>();
			ScreenHeaderNavigation._pInstance.EnableButtons();
			EndFlow();
		}
		else if (_pReturnLoc == RETURN_SCREEN_TYPE.SCREEN_DOJO)
		{
			ScreenDojo screenInstance7 = instance.GetScreenInstance<ScreenDojo>();
			ScreenHeaderNavigation._pInstance.EnableButtons();
			EndFlow();
		}
	}
}
