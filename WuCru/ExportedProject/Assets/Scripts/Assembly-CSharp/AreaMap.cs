using UnityEngine;
using UnityEngine.UI;

public class AreaMap : MonoBehaviour
{
	public GameObject map;

	public GameObject iconContainer;

	public RawImage mapImage;

	public RawImage fowImage;

	public RawImage gridImage;

	public RawImage cloudImage;

	public RawImage fxImage;

	public RawImage addonImage;

	public Button wuReminderButton;

	public Image wuSpeechBubble;

	public Text wuText;

	private void Start()
	{
		AnalyticsEvents.LogLoadScene("AreaMap");
		GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateAreaMap>());
	}
}
