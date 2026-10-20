using UnityEngine;
using UnityEngine.UI;

public class LevelBootstrapper : MonoBehaviour
{
	public Text levelInfo;

	public int debugPlateau;

	public int debugRect;

	private void Start()
	{
		AnalyticsEvents.LogLoadScene("LevelPlayerNew");
		if (GameController.Instance.currentState.GetType() != typeof(GameControllerStatePlaying))
		{
			Debug.Log("BootStrapper: First sequence");
			ScreenBlockerFX screenBlockerFX = Object.FindObjectOfType<ScreenBlockerFX>();
			if (screenBlockerFX != null)
			{
				Object.Destroy(screenBlockerFX.gameObject);
			}
			GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStatePlaying>());
		}
		else
		{
			Debug.Log("BootStrapper: Next sequence");
			(GameController.Instance.currentState as GameControllerStatePlaying).DelayedLoadNextSequence();
		}
		levelInfo.text = string.Empty;
	}
}
