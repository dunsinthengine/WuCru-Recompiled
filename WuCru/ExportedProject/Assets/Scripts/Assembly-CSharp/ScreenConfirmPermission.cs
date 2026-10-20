using System;
using UnityEngine;

public class ScreenConfirmPermission : ScreenBase
{
	public GameObject[] _storageElements;

	public GameObject[] _phoneElements;

	private Action _onNextConfirm;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
	}

	public void SetStorageElementsEnabled(bool e)
	{
		for (int i = 0; i < _storageElements.Length; i++)
		{
			_storageElements[i].SetActive(e);
			UILabel[] componentsInChildren = _storageElements[i].GetComponentsInChildren<UILabel>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].color = Color.white;
			}
		}
	}

	public void SetPhoneElementsEnabled(bool e)
	{
		for (int i = 0; i < _phoneElements.Length; i++)
		{
			_phoneElements[i].SetActive(e);
			UILabel[] componentsInChildren = _phoneElements[i].GetComponentsInChildren<UILabel>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].color = Color.white;
			}
		}
	}

	public void OnConfirm()
	{
		if (_onNextConfirm != null)
		{
			Action onNextConfirm = _onNextConfirm;
			_onNextConfirm = null;
			onNextConfirm();
		}
	}

	public void SetOnConfirmAction(Action onConfirm)
	{
		_onNextConfirm = onConfirm;
	}
}
