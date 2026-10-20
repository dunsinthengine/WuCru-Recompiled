using System.Collections;
using Cape.Localization;
using DentedPixel.LTEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashLoader : MonoBehaviour
{
	private const string areaMapMusic = "music/area/map";

	private const string androidID = "Y6PM9SSVCJBPB7JKCKBN";

	private const string iOSID = "9YH3S7W3DYPYKH9TR8M3";

	public Button startButton;

	private float timeGone;

	private float tweenTime;

	private void Awake()
	{
		LeanTweenVisual component = startButton.GetComponent<LeanTweenVisual>();
		tweenTime = LeanTweenHelper.GetDuration(component, true);
		timeGone = 0f;
		if (GameController.Instance.PlayerProfile.MusicDisabled)
		{
			GameAudioManager.Instance.MusicMuted = true;
		}
		if (GameController.Instance.PlayerProfile.SoundFXDisabled)
		{
			GameAudioManager.Instance.FxMuted = true;
		}
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		string language = "en-US";
		switch (Application.systemLanguage)
		{
		case SystemLanguage.German:
			language = "de-DE";
			break;
		case SystemLanguage.Japanese:
			language = "ja-JP";
			break;
		case SystemLanguage.Dutch:
			language = "nl-NL";
			break;
		case SystemLanguage.Korean:
			language = "ko-KR";
			break;
		case SystemLanguage.Danish:
			language = "da-DK";
			break;
		case SystemLanguage.Finnish:
			language = "fi-FI";
			break;
		case SystemLanguage.Swedish:
			language = "sv-SE";
			break;
		case SystemLanguage.Norwegian:
			language = "nb-NO";
			break;
		case SystemLanguage.French:
			language = "fr-FR";
			break;
		case SystemLanguage.Polish:
			language = "pl-PL";
			break;
		case SystemLanguage.Italian:
			language = "it-IT";
			break;
		case SystemLanguage.Czech:
			language = "cs-CZ";
			break;
		case SystemLanguage.Hungarian:
			language = "hu-HU";
			break;
		case SystemLanguage.Spanish:
			language = "es-ES";
			break;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
		case SystemLanguage.ChineseTraditional:
			language = "zh-CN";
			break;
		case SystemLanguage.Russian:
			language = "ru-RU";
			break;
		case SystemLanguage.Arabic:
			language = "ar-AE";
			break;
		case SystemLanguage.Ukrainian:
			language = "uk-UA";
			break;
		case SystemLanguage.Turkish:
			language = "tr-TR";
			break;
		case SystemLanguage.Romanian:
			language = "ro-RO";
			break;
		case SystemLanguage.SerboCroatian:
			language = "hr-HR";
			break;
		}
		GameController.Instance.PlayerProfile.Language = language;
		GameController.Instance.PlayerProfile.Save();
		Localization.Instance.SelectLanguage(GameController.Instance.PlayerProfile.Language);
	}

	private void Update()
	{
		if (timeGone < tweenTime)
		{
			timeGone += Time.deltaTime;
		}
		if (Input.GetMouseButtonUp(0) && timeGone >= tweenTime)
		{
			DoStartButtonAction();
		}
	}

	private void DoStartButtonAction()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		StartCoroutine(DoLoad());
	}

	private IEnumerator DoLoad()
	{
		Object.Instantiate(Resources.Load("Prefabs/Dialogs/InGameLoadScreen"));
		AudioPlayer.StopSound("music/splash/screen");
		AudioPlayer.PlaySound("music/area/map");
		yield return 0;
		SceneManager.LoadScene("TeaFarm");
	}
}
