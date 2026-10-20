using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StudsForGUI : MonoBehaviour
{
	public delegate void OnReachTargetDelegate();

	public Image imageStud;

	public OnReachTargetDelegate OnReachTarget;

	public void Fly(Vector3 from, Vector3 to, float duration)
	{
		StartCoroutine(DoFly(from, to, duration));
	}

	private IEnumerator DoFly(Vector3 from, Vector3 to, float duration)
	{
		float startTime = Time.time;
		imageStud.transform.localPosition = from;
		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			imageStud.transform.localPosition = Vector3.Lerp(from, to, t);
			yield return 0;
		}
		if (OnReachTarget != null)
		{
			OnReachTarget();
		}
		imageStud.enabled = false;
		Object.Destroy(base.gameObject);
	}
}
