using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
	public delegate void OnCloseDelegate();

	public string msg = "Message...";

	public Text textElement;

	public Text buttonLabel;

	public Button buttonOk;

	public OnCloseDelegate onClose;

	private void Start()
	{
		textElement.text = msg;
		buttonOk.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			if (onClose != null)
			{
				onClose();
			}
		});
	}
}
