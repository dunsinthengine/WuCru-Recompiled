using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsExampleScript : MonoBehaviour
{
	private const string areaMapMusic = "music/area/map";

	private void Start()
	{
		if (!AudioPlayer.IsPlaying("Audio_Music_music_area_map"))
		{
			AudioPlayer.PlaySound("music/area/map");
		}
		Credits.getInstance().endListeners += creditsEnded;
	}

	private void creditsEnded(Credits c)
	{
		Debug.Log("Credit roll finished!");
		SceneManager.LoadScene("SplashScreen");
	}
}
