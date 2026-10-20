using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedNumberFX : MonoBehaviour
{
	public TypefaceAnimator typefaceAnimator;

	private Text textComponent;

	private Localize localizeComponent;

	private Transform parentTransform;

	private void Awake()
	{
		textComponent = typefaceAnimator.GetComponent<Text>();
		localizeComponent = GetComponentInChildren<Localize>();
		typefaceAnimator.onComplete.AddListener(() =>
		{
			Object.Destroy(base.gameObject);
		});
	}

	public void SetText(Transform parent, string animText, bool localize)
	{
		if (localize)
		{
			if (localizeComponent != null)
			{
				localizeComponent.SetKey(animText);
			}
			else
			{
				textComponent.text = animText;
			}
		}
		else if (localizeComponent != null)
		{
			localizeComponent.SetValue(animText);
		}
		else
		{
			textComponent.text = animText;
		}
		parentTransform = parent;
		Vector2 vector = Camera.main.WorldToViewportPoint(parentTransform.position);
		textComponent.rectTransform.anchorMin = vector;
		textComponent.rectTransform.anchorMax = vector;
	}

	private void Update()
	{
		if (parentTransform != null && !(Camera.main == null) && !(parentTransform == null))
		{
			Vector2 vector = Camera.main.WorldToViewportPoint(parentTransform.position);
			textComponent.rectTransform.anchorMin = vector;
			textComponent.rectTransform.anchorMax = vector;
		}
	}
}
