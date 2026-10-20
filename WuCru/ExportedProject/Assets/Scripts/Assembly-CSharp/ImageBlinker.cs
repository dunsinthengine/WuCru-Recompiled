using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlinker : MonoBehaviour
{
	[SerializeField]
	private Image image;

	private void Awake()
	{
		if (image == null)
		{
			image = GetComponentInChildren<Image>();
		}
		Material material = Object.Instantiate(image.material);
		image.material = material;
	}

	public void Blink(Color color, float duration)
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(DoBlink(color, duration));
		}
	}

	public void Reset()
	{
		image.material.SetFloat("_BlinkValue", 0f);
	}

	private IEnumerator DoBlink(Color color, float duration)
	{
		float startTime = Time.time;
		Material m = image.material;
		m.SetColor("_BlinkColor", color);
		while (Time.time - startTime <= duration)
		{
			float t = (Time.time - startTime) / duration;
			m.SetFloat("_BlinkValue", 1f - t);
			yield return 0;
		}
		m.SetFloat("_BlinkValue", 0f);
	}

	public void BlinkReverse(Color color, float duration)
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(DoBlinkReverese(color, duration));
		}
	}

	private IEnumerator DoBlinkReverese(Color color, float duration)
	{
		float startTime = Time.time;
		Material m = image.material;
		m.SetColor("_BlinkColor", color);
		m.SetFloat("_BlinkValue", 0f);
		while (Time.time - startTime <= duration)
		{
			float t = Mathf.Clamp01((Time.time - startTime) / duration);
			m.SetFloat("_BlinkValue", t);
			yield return 0;
		}
		m.SetFloat("_BlinkValue", 1f);
	}

	public void SetBlinkValue(Color color, float value)
	{
		Material material = image.material;
		material.SetColor("_BlinkColor", color);
		material.SetFloat("_BlinkValue", value);
	}
}
