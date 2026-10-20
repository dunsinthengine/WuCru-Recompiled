using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelDialog : MonoBehaviour
{
	public delegate void OnCloseDelegate(int seed);

	public InputField inputFieldSeed;

	public Button buttonOK;

	public OnCloseDelegate OnClose;

	private void Start()
	{
		inputFieldSeed.text = "1";
		buttonOK.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			if (OnClose != null)
			{
				OnClose(Convert.ToInt32(inputFieldSeed.text));
			}
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}
}
