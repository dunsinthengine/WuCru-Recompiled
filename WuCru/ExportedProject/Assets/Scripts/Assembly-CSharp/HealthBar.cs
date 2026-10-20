using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Image imageFill;

	private int maxHealth = 100;

	private Coroutine blinkCoroutine;

	private void Start()
	{
	}

	public void UpdateMaxHealth(int maxHealth)
	{
		this.maxHealth = maxHealth;
	}

	public void UpdateHealth(int absoluteHealth, bool blink = true)
	{
		if (blink && blinkCoroutine != null)
		{
			StopCoroutine(blinkCoroutine);
			blinkCoroutine = null;
		}
		imageFill.enabled = true;
		imageFill.fillAmount = (float)absoluteHealth / (float)maxHealth;
		if (blink)
		{
			blinkCoroutine = StartCoroutine(DoHealthBarBlink());
		}
	}

	private IEnumerator DoHealthBarBlink()
	{
		float startTime = Time.time;
		float duration = 2.3f;
		float interval = 0.5f * imageFill.fillAmount;
		float blinkTime = Time.time + interval;
		while (Time.time - startTime <= duration)
		{
			if (Time.time > blinkTime)
			{
				blinkTime = Time.time + interval;
				imageFill.enabled = !imageFill.enabled;
			}
			yield return 0;
		}
		imageFill.enabled = true;
	}
}
