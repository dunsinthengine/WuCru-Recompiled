using System.Collections;
using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class MasterWuReminderGUI : MonoBehaviour
{
	public Button buttonMasterWu;

	public Image imageSpeechBubble;

	public Text text;

	public Localize localize;

	public Button speechBubbleButton;

	private void Awake()
	{
		text.enabled = false;
		imageSpeechBubble.enabled = false;
		if (GameController.Instance.ObjectiveController.CurrentObjective == null)
		{
			base.gameObject.SetActive(false);
		}
		buttonMasterWu.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			ToggleShow();
		});
		speechBubbleButton.onClick.AddListener(() =>
		{
			if (GameController.Instance.ObjectiveController.CurrentObjective != null && imageSpeechBubble.gameObject.activeSelf && text.gameObject.activeSelf && imageSpeechBubble.enabled && text.enabled)
			{
				AudioPlayer.PlaySound("ui/button/click");
				ToggleShow();
			}
		});
	}

	private void Start()
	{
		ToggleShow();
		Localization.Instance.LocalizationChanged += OnLocalizationChanged;
	}

	public void OnLocalizationChanged()
	{
		if (this.text != null && this.text.enabled)
		{
			int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
			int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
			string key = string.Format("OBJECTIVE_S{0}_C{1}", lastPlayedSeason, chapterCursor);
			string text = Localization.Instance.Get(key);
			localize.SetValue(text + " " + GameController.Instance.ObjectiveController.GetNumCollectiblesRemainingText());
		}
	}

	private void ToggleShow()
	{
		if (GameController.Instance.ObjectiveController.CurrentObjective != null)
		{
			if (!imageSpeechBubble.enabled)
			{
				imageSpeechBubble.enabled = true;
				text.enabled = true;
				OnLocalizationChanged();
			}
			else
			{
				imageSpeechBubble.enabled = false;
				text.enabled = false;
			}
		}
	}

	private IEnumerator AutoHide()
	{
		yield return new WaitForSeconds(6f);
		if (imageSpeechBubble.enabled)
		{
			ToggleShow();
		}
	}
}
