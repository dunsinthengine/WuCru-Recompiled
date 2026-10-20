using UnityEngine;
using UnityEngine.UI;

public class MessageBoxYesNo : MonoBehaviour
{
	public delegate void OnOkDelegate();

	public delegate void OnCancelDelegate();

	public string msg = "Message...";

	public Text textElement;

	public Button buttonOk;

	public Button buttonCancel;

	public OnOkDelegate onOk;

	public OnCancelDelegate onCancel;

	private void Start()
	{
		textElement.text = msg;
		buttonOk.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			if (onOk != null)
			{
				onOk();
			}
		});
		buttonCancel.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			if (onCancel != null)
			{
				onCancel();
			}
		});
	}

	private void Update()
	{
	}
}
