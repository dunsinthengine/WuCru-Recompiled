using Cape.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsGUI : BaseGUI
{
	[SerializeField]
	private Button _closeButton;

	[SerializeField]
	private Button _creditsButton;

	[SerializeField]
	private Toggle _musicToggle;

	[SerializeField]
	private Toggle _soundFXToggle;

	[SerializeField]
	private Button _languageSelectionButton;

	[SerializeField]
	private Button _legalButton;

	private void Awake()
	{
		Register();
		_closeButton.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			Object.Destroy(base.gameObject);
			TopBar topBar = Object.FindObjectOfType<TopBar>();
			topBar.isSettingsBoxActive = false;
		});
		_creditsButton.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			Object.Destroy(base.gameObject);
			if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
			{
				GameController.Instance.SetState(GameController.Instance.gameObject.GetComponent<GameControllerStateIdle>());
			}
			SceneManager.LoadScene("Credits");
		});
		_languageSelectionButton.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			Object.Destroy(base.gameObject);
			if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
			{
				GameController.Instance.SetState(GameController.Instance.gameObject.GetComponent<GameControllerStateIdle>());
			}
			LanguageSelectionGUI.ReturnToSceneName = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene("LanguageSelection");
		});
		_legalButton.onClick.AddListener(() =>
		{
			if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
			{
				GameController.Instance.SetState(GameController.Instance.gameObject.GetComponent<GameControllerStateIdle>());
			}
			LegoCoreSdkLegal.previousScene = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene("LegoSdkLegalPolicy");
		});
	}

	private void Start()
	{
		if (GameController.Instance.PlayerProfile.MusicDisabled)
		{
			_musicToggle.isOn = false;
		}
		else
		{
			_musicToggle.isOn = true;
		}
		_musicToggle.onValueChanged.AddListener((bool enabled) =>
		{
			if (GameController.Instance.PlayerProfile.MusicDisabled == enabled)
			{
				GameController.Instance.PlayerProfile.MusicDisabled = !enabled;
				GameController.Instance.PlayerProfile.Save();
				GameAudioManager.Instance.MusicMuted = !enabled;
			}
			AudioPlayer.PlaySound("ui/button/click");
		});
		if (GameController.Instance.PlayerProfile.SoundFXDisabled)
		{
			_soundFXToggle.isOn = false;
		}
		else
		{
			_soundFXToggle.isOn = true;
		}
		_soundFXToggle.onValueChanged.AddListener((bool enabled) =>
		{
			if (GameController.Instance.PlayerProfile.SoundFXDisabled == enabled)
			{
				GameController.Instance.PlayerProfile.SoundFXDisabled = !enabled;
				GameController.Instance.PlayerProfile.Save();
				GameAudioManager.Instance.FxMuted = !enabled;
			}
			AudioPlayer.PlaySound("ui/button/click");
		});
	}

	private void LanguageSelected(Dropdown target)
	{
		string language = Localization.Instance.AvailableLanguages[target.value];
		Localization.Instance.SelectLanguage(language);
		if (GameController.WuCruMissionsEnabled)
		{
		}
		GameController.Instance.PlayerProfile.Language = Localization.Instance.AvailableLanguages[target.value];
		GameController.Instance.PlayerProfile.Save();
	}

	private void OnDestroy()
	{
		Deregister();
	}
}
