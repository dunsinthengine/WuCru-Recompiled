using UnityEngine;
using UnityEngine.UI;

public class CreditLineInstanceImage : CreditLineInstance
{
	public override void setVisual(string data, string data2)
	{
		RectTransform component = GetComponent<RectTransform>();
		Sprite sprite = Resources.Load<Sprite>(data);
		if (sprite != null)
		{
			Vector2 sizeDelta = component.sizeDelta;
			sizeDelta.y = sprite.rect.height;
			component.sizeDelta = sizeDelta;
			GetComponent<Image>().sprite = sprite;
		}
	}

	public override void fadeIn(float time)
	{
		GetComponent<Image>().CrossFadeAlpha(1f, time, false);
	}

	public override void fadeOut(float time)
	{
		GetComponent<Image>().CrossFadeAlpha(0f, time, false);
	}
}
