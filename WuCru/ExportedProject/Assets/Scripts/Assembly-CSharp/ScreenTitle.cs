using System.Collections.Generic;
using Fabric;
using LEGO;
using LEGO.CoreSDK;
using UnityEngine;

public class ScreenTitle : ScreenBase
{
	private const string TEXT_MOVIERELEASE_DEFAULT_ENUS = "In US Theatres September 22";

	private const string TEXT_MOVIERELEASE_OTHER = "NINJAGO Movie, ";

	private readonly Dictionary<string, string> LOOKUPTABLE_RELEASEDATE = new Dictionary<string, string>
	{
		{ "AE", "21/09/17" },
		{ "AR", "28/09/17" },
		{ "CS", "21/09/17" },
		{ "DA", "21/09/17" },
		{ "DE", "21/09/17" },
		{ "EN", "13/10/17" },
		{ "ES", "22/09/17" },
		{ "FI", "22/09/17" },
		{ "FR", "11/10/17" },
		{ "HR", "21/09/17" },
		{ "HU", "21/09/17" },
		{ "IT", "12/10/17" },
		{ "JA", "30/09/17" },
		{ "KO", "28/09/17" },
		{ "NB", "22/09/17" },
		{ "NL", "27/09/17" },
		{ "PL", "22/09/17" },
		{ "PT", "28/09/17" },
		{ "RO", "22/09/17" },
		{ "RU", "21/09/17" },
		{ "SV", "28/09/17" },
		{ "TR", "29/09/17" },
		{ "UA", "21/09/17" },
		{ "ZH", "28/09/17" }
	};

	public UILabel _releaseText;

	private bool _transitionedOut;

	private float _startTime;

	private bool _canPressStart = true;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		AudioPlayer.PlaySound("music/departed/splash");
		_transitionedOut = false;
		_startTime = RealTime.time;
		_canPressStart = true;
		if (GameController.Instance.PlayerProfile.MusicDisabled)
		{
			GameAudioManager.Instance.MusicMuted = true;
		}
		if (GameController.Instance.PlayerProfile.SoundFXDisabled)
		{
			GameAudioManager.Instance.FxMuted = true;
		}
		if (LEGOSDK.ApplicationConfigurationService != null)
		{
			LEGOSDK.ApplicationConfigurationService.ApplicationConfigurationChanged += SetMovieText;
		}
		SetMovieText(null);
	}

	protected override void OnExitScreen()
	{
		base.OnExitScreen();
		if (LEGOSDK.ApplicationConfigurationService != null)
		{
			LEGOSDK.ApplicationConfigurationService.ApplicationConfigurationChanged -= SetMovieText;
		}
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _startTime;
		if (!AudioPlayer.IsPlaying("Audio_Music_music_splash_screen_departed") && !AudioPlayer.IsPlaying("Audio_Music_Departed_music_departed_exploration") && num >= 10f && !_transitionedOut)
		{
			Fabric.Component componentByName = FabricManager.Instance.GetComponentByName("Audio_Music_Departed_music_departed_exploration");
			if (componentByName != null)
			{
				componentByName.FadeIn(4000f, 0.5f);
				componentByName.Play(base.gameObject);
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		else if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
		{
			OnStartButton();
		}
	}

	public void OnStartButton()
	{
		Debug.Log("OnStartButton - " + _canPressStart);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		if (instance == null || !_canPressStart)
		{
			return;
		}
		_canPressStart = false;
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		int totalLevelsPlayed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
		_transitionedOut = true;
		AudioPlayer.PlaySound("ui/button/sword/shing");
		AudioPlayer.StopSound("music/departed/splash");
		AudioPlayer.StopSound("music/departed/exploration");

		ScreenCreateCharacter screenInstance = instance.GetScreenInstance<ScreenCreateCharacter>();
		ScreenCutsceneHandsOfTimeIntro screenInstance2 = instance.GetScreenInstance<ScreenCutsceneHandsOfTimeIntro>();
		ScreenSkills screenInstance3 = instance.GetScreenInstance<ScreenSkills>();
		ScreenWorldMap screenInstance4 = instance.GetScreenInstance<ScreenWorldMap>();
		ScreenMap screenInstance5 = instance.GetScreenInstance<ScreenMap>();

		if (chapterCursor < 1 || totalLevelsPlayed < 1)
		{
			GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
			GameController.Instance.PlayerProfile.SetChapterCursor(1, 1);
			GameController.Instance.ObjectiveController.InitiateObjective(1, 1);
			GameController.Instance.PlayerProfile.Save();
			if (screenInstance != null)
			{
				Navigate(screenInstance);
			}
			else if (screenInstance5 != null)
			{
				Navigate(screenInstance5);
			}
			return;
		}
		if (GlobalInGameData.ShouldLaunchHandsOfTimeCampaign() && screenInstance2 != null)
		{
			GlobalInGameData.LaunchHandsOfTimeCampaign();
			Navigate(screenInstance2);
			return;
		}
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (playerProfile.AvailableSkillPoints > 0 && playerProfile.AgilityLevel == 0 && playerProfile.SpinjitzuLevel == 0 && playerProfile.StaminaLevel == 0 && playerProfile.StrengthLevel == 0 && playerProfile.WeaponLevel == 0 && screenInstance3 != null)
		{
			Navigate(screenInstance3);
		}
		else if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap() && screenInstance4 != null)
		{
			ScreenMap.UpdateCompletedObjectives();
			Navigate(screenInstance4);
		}
		else if (screenInstance5 != null)
		{
			Navigate(screenInstance5);
		}
		else if (screenInstance != null)
		{
			Navigate(screenInstance);
		}
	}

	public void SetMovieText(IApplicationConfiguration config)
	{
		if (LocalisationFacade.Instance != null && _releaseText != null)
		{
			if (LocalisationFacade.Instance.language != "EN")
			{
				if (LOOKUPTABLE_RELEASEDATE.ContainsKey(LocalisationFacade.Instance.language))
				{
					_releaseText.text = LOOKUPTABLE_RELEASEDATE[LocalisationFacade.Instance.language];
				}
				else
				{
					_releaseText.text = LOOKUPTABLE_RELEASEDATE["EN"];
				}
			}
			else if (LEGOSDK.ApplicationConfigurationService != null &&
			         LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration != null &&
			         LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.ContentLanguage != null &&
			         LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.ContentLanguage.EndsWith("US"))
			{
				_releaseText.text = "In US Theatres September 22";
			}
			else
			{
				_releaseText.text = LOOKUPTABLE_RELEASEDATE["EN"];
			}
		}
	}
}
