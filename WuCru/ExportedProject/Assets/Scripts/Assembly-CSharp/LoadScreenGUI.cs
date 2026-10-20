using UnityEngine;
using UnityEngine.UI;

public class LoadScreenGUI : BaseGUI
{
	public Image spinner;

	public RawImage ninjaImage;

	private RenderTexture renderTexture;

	private void Awake()
	{
		Register();
	}

	private void Start()
	{
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		if (chapterCursor != -1)
		{
			ninjaImage.enabled = true;
			renderTexture = CharacterFullbodyRenderer.Generate(0, (int)ninjaImage.rectTransform.sizeDelta.x, (int)ninjaImage.rectTransform.sizeDelta.y);
			ninjaImage.texture = renderTexture;
		}
		else
		{
			ninjaImage.enabled = false;
		}
	}

	private void OnDestroy()
	{
		Deregister();
		if (renderTexture != null)
		{
			renderTexture.Release();
		}
	}

	private void Update()
	{
		spinner.rectTransform.transform.Rotate(0f, 0f, -120f * Time.deltaTime);
	}
}
