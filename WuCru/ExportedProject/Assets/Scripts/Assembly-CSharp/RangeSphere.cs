using System.Collections;
using UnityEngine;

public class RangeSphere : MonoBehaviour
{
	[SerializeField]
	public AnimationCurve animCurve;

	private float duration = 1.7f;

	private float fadeDuration = 0.8f;

	private MeshRenderer meshRenderer;

	private Color originalColor;

	private Color fadeColor;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		originalColor = meshRenderer.material.color;
		fadeColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
		base.gameObject.SetActive(false);
	}

	public void Show(float range, float duration)
	{
		base.gameObject.SetActive(true);
		meshRenderer.material.color = originalColor;
		this.duration = duration;
		StartCoroutine(DoShow(range));
	}

	private IEnumerator DoShow(float range)
	{
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			base.gameObject.transform.localScale = new Vector3(animCurve.Evaluate(t) * range, animCurve.Evaluate(t) * range * 0.5f, animCurve.Evaluate(t) * range);
			yield return 0;
		}
		startTime = Time.time;
		while (Time.time - startTime < fadeDuration)
		{
			float t2 = (Time.time - startTime) / fadeDuration;
			meshRenderer.material.color = Color.Lerp(originalColor, fadeColor, t2);
			yield return 0;
		}
		base.gameObject.SetActive(false);
	}
}
