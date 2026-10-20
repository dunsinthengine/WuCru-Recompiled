using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	public float frequency = 0.5f;

	public Text label;

	public int FramesPerSec { get; protected set; }

	private void Start()
	{
		base.enabled = false;
		label.text = string.Empty;
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
			FramesPerSec = Mathf.RoundToInt((float)frameCount / timeSpan);
			label.text = FramesPerSec + " fps";
		}
	}
}
