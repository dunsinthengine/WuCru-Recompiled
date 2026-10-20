using System;
using System.Collections;
using Cape.Localization;
using Cape.Utils.UI;
using Fabric;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
	[SerializeField]
	private FadeOverlay _fadeOverlay;

	[SerializeField]
	private Button _startButton;

	[SerializeField]
	private Mask _startButtonMask;

	private const string areaMapMusic = "music/area/map";

	private const string androidID = "Y6PM9SSVCJBPB7JKCKBN";

	private const string iOSID = "9YH3S7W3DYPYKH9TR8M3";

	private void Awake()
	{
		Application.targetFrameRate = 60;
		if (GameController.Instance.PlayerProfile.MusicDisabled)
		{
			GameAudioManager.Instance.MusicMuted = true;
		}
		if (GameController.Instance.PlayerProfile.SoundFXDisabled)
		{
			GameAudioManager.Instance.FxMuted = true;
		}
		_startButton.gameObject.SetActive(false);
		_startButtonMask.gameObject.SetActive(false);
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
		_fadeOverlay.FadeInDone += FadeOverlay_FadeInDone;
		_fadeOverlay.StartFadeIn();
		AudioPlayer.PlaySound("music/departed/splash");
	}

	private void Update()
	{
		if (_startButton.gameObject.activeSelf && !_fadeOverlay.IsFadingOut())
		{
			if (Input.GetMouseButtonDown(0))
			{
				_startButton.interactable = false;
			}
			if (Input.GetMouseButtonUp(0))
			{
				_startButton.interactable = true;
				AudioPlayer.PlaySound("ui/button/sword/shing");
				AudioPlayer.StopSound("music/departed/splash");
				AudioPlayer.StopSound("music/departed/exploration");
				StartCoroutine(DoButtonBounce(0.5f, 5f, 0.05f));
			}
			if (!AudioPlayer.IsPlaying("Audio_Music_music_splash_screen_departed") && !AudioPlayer.IsPlaying("Audio_Music_Departed_music_departed_exploration") && Time.realtimeSinceStartup > 18f)
			{
				Fabric.Component componentByName = FabricManager.Instance.GetComponentByName("Audio_Music_Departed_music_departed_exploration");
				componentByName.FadeIn(4000f, 0.5f);
				componentByName.Play(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		_startButton = null;
		_startButtonMask = null;
		_fadeOverlay = null;
	}

	private void FadeOverlay_FadeInDone(object sender, EventArgs e)
	{
		_fadeOverlay.FadeInDone += FadeOverlay_FadeInDone;
		StartCoroutine(DoWait(1.4f));
	}

	private void FadeOverlay_FadeOutDone(object sender, EventArgs e)
	{
		_fadeOverlay.FadeOutDone += FadeOverlay_FadeOutDone;
		StartCoroutine(DoLoad());
	}

	private IEnumerator DoButtonBounce(float duration, float frequency, float amount)
	{
		float counter = 0f;
		Vector3 scale = Vector3.zero;
		while (counter < duration)
		{
			counter += Time.deltaTime;
			float damping = 1f - counter / duration;
			float bounce = amount * Mathf.Sin((float)Math.PI * 2f * frequency * counter) * damping;
			scale.x = 1f + bounce;
			scale.y = scale.x;
			scale.z = scale.x;
			_startButton.transform.localScale = scale;
			yield return null;
		}
		_fadeOverlay.FadeOutDone += FadeOverlay_FadeOutDone;
		_fadeOverlay.StartFadeOut();
	}

	private IEnumerator DoWait(float time)
	{
		float counter = 0f;
		while (counter < time)
		{
			counter += Time.deltaTime;
			yield return null;
		}
		_startButton.gameObject.SetActive(true);
		_startButtonMask.gameObject.SetActive(true);
	}

	private IEnumerator DoLoad()
	{
		AudioPlayer.PlaySound("music/area/map");
		yield return null;
		SceneManager.LoadScene("TeaFarm");
	}
}
