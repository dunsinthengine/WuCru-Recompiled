using System.Collections;
using UnityEngine;

public class TransparencyCaptureToFile_modded : MonoBehaviour
{
	public string NameOfScreenCaptureFile;

	public IEnumerator capture()
	{
		yield return new WaitForEndOfFrame();
		zzTransparencyCapture.captureScreenshot("Assets/UI/Splash/ImitateParticles/" + NameOfScreenCaptureFile + ".png");
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			StartCoroutine(capture());
		}
	}
}
