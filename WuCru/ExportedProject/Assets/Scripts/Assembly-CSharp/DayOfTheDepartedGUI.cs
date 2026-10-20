using UnityEngine;
using UnityEngine.UI;

public class DayOfTheDepartedGUI : MonoBehaviour
{
	[SerializeField]
	private Image fill;

	public void Awake()
	{
		LevelExtensionController.LevelExtensionSettings levelExtensionSettings = GameController.Instance.LevelExtensionController.GetLevelExtensionSettings("DayOfTheDeparted");
		if (levelExtensionSettings.progress == 0f || levelExtensionSettings.progress == 1f)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			fill.fillAmount = levelExtensionSettings.progress;
		}
	}
}
