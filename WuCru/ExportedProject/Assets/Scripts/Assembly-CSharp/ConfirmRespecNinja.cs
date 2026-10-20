using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmRespecNinja : MonoBehaviour
{
	public Button buttonOK;

	public Button buttonCancel;

	public Action onOKAction;

	public Action onCancelAction;

	private void Start()
	{
		buttonOK.onClick.AddListener(() =>
		{
			onOKAction();
			UnityEngine.Object.Destroy(base.gameObject);
		});
		buttonCancel.onClick.AddListener(() =>
		{
			onCancelAction();
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}
}
