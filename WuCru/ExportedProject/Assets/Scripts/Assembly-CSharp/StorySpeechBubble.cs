using System.Collections;
using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class StorySpeechBubble : MonoBehaviour
{
	public Image imageBubble;

	public Text textBubble;

	public float displayWordDelay = 0.05f;

	public StorySpeechBubbleDirection direction;

	private bool isDisplayingText;

	[SerializeField]
	private string wordAppearsAudioEvent = "fx/story/text/appears";

	private bool skipWordDelay;

	public bool IsDisplayingText
	{
		get
		{
			return isDisplayingText;
		}
	}

	private void Awake()
	{
		textBubble.text = string.Empty;
		SetDirection(direction);
		Hide();
	}

	private void Update()
	{
		if (isDisplayingText && Input.GetMouseButtonDown(0))
		{
			skipWordDelay = true;
		}
	}

	public void SetDirection(StorySpeechBubbleDirection direction)
	{
		switch (direction)
		{
		case StorySpeechBubbleDirection.Left:
			imageBubble.rectTransform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case StorySpeechBubbleDirection.Right:
			imageBubble.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
			break;
		}
	}

	public void DisplayText(string sentence)
	{
		if (!imageBubble.gameObject.activeSelf)
		{
			imageBubble.gameObject.SetActive(true);
		}
		if (Localization.Instance.Direction == Directionality.RightToLeft)
		{
			textBubble.text = sentence;
			textBubble.alignment = TextAnchor.MiddleRight;
		}
		else
		{
			StartCoroutine(DoDisplayText(sentence));
		}
	}

	public void Hide()
	{
		textBubble.text = string.Empty;
		if (imageBubble.gameObject.activeSelf)
		{
			imageBubble.gameObject.SetActive(false);
		}
	}

	private IEnumerator DoDisplayText(string sentence)
	{
		isDisplayingText = true;
		textBubble.text = string.Empty;
		string[] words = sentence.Split(' ');
		for (int i = 0; i < words.Length; i++)
		{
			AudioPlayer.PlaySound(wordAppearsAudioEvent);
			Text text = textBubble;
			text.text = text.text + words[i] + " ";
			if (!skipWordDelay)
			{
				yield return new WaitForSeconds(displayWordDelay);
			}
		}
		isDisplayingText = false;
		skipWordDelay = false;
	}
}
