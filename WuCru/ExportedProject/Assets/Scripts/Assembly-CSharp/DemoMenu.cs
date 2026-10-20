using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMenu : MonoBehaviour
{
	public void OnAvatarDemo()
	{
		SceneManager.LoadScene("LevelTest");
	}

	public void OnLevelsDemo()
	{
		SceneManager.LoadScene("LevelGenerationOverview");
	}

	public void OnExit()
	{
		Application.Quit();
	}
}
