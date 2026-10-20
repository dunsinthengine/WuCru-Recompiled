using System.Collections;
using UnityEngine;

public class ScreenFadeFX : MonoBehaviour
{
	public delegate void OnDoneDelegate();

	public Material material;

	public OnDoneDelegate OnDone;

	public void FadeIn(float duration)
	{
		StartCoroutine(DoFadeIn(duration));
	}

	public void FadeOut(float duration)
	{
		StartCoroutine(DoFadeOut(duration));
	}

	private IEnumerator DoFadeIn(float duration)
	{
		yield return null;
		if (OnDone != null)
		{
			OnDone();
		}
		yield return null;
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DoFadeOut(float duration)
	{
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - startTime < duration)
		{
			yield return 0;
		}
		yield return null;
		Object.Destroy(base.gameObject);
		if (OnDone != null)
		{
			OnDone();
		}
	}
}
