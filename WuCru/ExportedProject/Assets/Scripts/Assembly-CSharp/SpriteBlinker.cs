using System.Collections;
using UnityEngine;

public class SpriteBlinker : MonoBehaviour
{
	private SpriteRenderer sprite;

	private void Awake()
	{
		sprite = GetComponentInChildren<SpriteRenderer>();
	}

	public void Blink(Color color, float duration)
	{
		StartCoroutine(DoBlink(color, duration));
	}

	public void Reset()
	{
		sprite.material.SetFloat("_BlinkValue", 0f);
	}

	private IEnumerator DoBlink(Color color, float duration)
	{
		float startTime = Time.time;
		Material m = sprite.material;
		m.SetColor("_BlinkColor", color);
		while (Time.time - startTime <= duration)
		{
			float t = (Time.time - startTime) / duration;
			m.SetFloat("_BlinkValue", 1f - t);
			yield return 0;
		}
		m.SetFloat("_BlinkValue", 0f);
	}
}
