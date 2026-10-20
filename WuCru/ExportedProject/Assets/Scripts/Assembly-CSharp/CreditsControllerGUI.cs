using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsControllerGUI : MonoBehaviour
{
	[SerializeField]
	private Button backButton;

	private const string areaMapMusic = "music/area/map";

	private void Awake()
	{
		backButton.gameObject.SetActive(false);
	}

	private void Start()
	{
		if (!AudioPlayer.IsPlaying("Audio_Music_music_area_map"))
		{
			AudioPlayer.PlaySound("music/area/map");
		}
		Credits.getInstance().endListeners += OnCreditsEnded;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (backButton.gameObject.activeSelf)
			{
				OnCreditsEnded(null);
			}
			else
			{
				backButton.gameObject.SetActive(true);
			}
		}
	}

	private void OnCreditsEnded(Credits c)
	{
		Debug.Log("Credit roll finished!");
		if (AudioPlayer.IsPlaying("Audio_Music_music_area_map"))
		{
			AudioPlayer.StopSound("music/area/map");
		}
		SceneManager.LoadScene("SplashScreen");
	}
}
