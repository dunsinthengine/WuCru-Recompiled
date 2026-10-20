using Cape.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
	private Button buttonFlag;

	[SerializeField]
	private Language language;

	private void Awake()
	{
		buttonFlag = GetComponent<Button>();
		buttonFlag.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			SetLanguage(language);
		});
	}

	private void SetLanguage(Language lang)
	{
		string text = string.Empty;
		switch (lang)
		{
		case Language.US:
			text = "en-US";
			break;
		case Language.GB:
			text = "en-GB";
			break;
		case Language.DE:
			text = "de-DE";
			break;
		case Language.JP:
			text = "ja-JP";
			break;
		case Language.NL:
			text = "nl-NL";
			break;
		case Language.KR:
			text = "ko-KR";
			break;
		case Language.DK:
			text = "da-DK";
			break;
		case Language.FI:
			text = "fi-FI";
			break;
		case Language.SE:
			text = "sv-SE";
			break;
		case Language.NO:
			text = "nb-NO";
			break;
		case Language.FR:
			text = "fr-FR";
			break;
		case Language.PL:
			text = "pl-PL";
			break;
		case Language.IT:
			text = "it-IT";
			break;
		case Language.CZ:
			text = "cs-CZ";
			break;
		case Language.HU:
			text = "hu-HU";
			break;
		case Language.ES:
			text = "es-ES";
			break;
		case Language.AR:
			text = "es-AR";
			break;
		case Language.CN:
			text = "zh-CN";
			break;
		case Language.RU:
			text = "ru-RU";
			break;
		case Language.UK:
			text = "uk-UA";
			break;
		case Language.RO:
			text = "ro-RO";
			break;
		case Language.TR:
			text = "tr-TR";
			break;
		case Language.AE:
			text = "ar-AE";
			break;
		case Language.HR:
			text = "hr-HR";
			break;
		}
		Localization.Instance.SelectLanguage(text);
		if (GameController.WuCruMissionsEnabled)
		{
		}
		GameController.Instance.PlayerProfile.Language = text;
		GameController.Instance.PlayerProfile.Save();
		if (LanguageSelectionGUI.ReturnToSceneName.Length > 0)
		{
			string returnToSceneName = LanguageSelectionGUI.ReturnToSceneName;
			LanguageSelectionGUI.ReturnToSceneName = string.Empty;
			SceneManager.LoadScene(returnToSceneName);
		}
		else
		{
			SceneManager.LoadScene("TeaFarm");
		}
	}
}
