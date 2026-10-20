using Fabric;
using UnityEngine;

public class SplashAudio : MonoBehaviour
{
	private bool playedAudio;

	private void Update()
	{
		while (!playedAudio)
		{
			if (FabricManager.IsInitialised())
			{
				AudioPlayer.PlaySound("music/splash/screen");
				playedAudio = true;
			}
		}
	}
}
