using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class SignalSender
{
	public bool onlyOnce;

	public ReceiverItem[] receivers;

	private bool _hasFired;

	public void SendSignals(MonoBehaviour sender)
	{
		EnsureReceivers();
		if (_hasFired && onlyOnce)
		{
			return;
		}
		for (int i = 0; i < receivers.Length; i++)
		{
			sender.gameObject.SetActive(true);
			if (receivers[i].delay <= 0f)
			{
				receivers[i].SendSignal(sender);
			}
			else
			{
				sender.StartCoroutine(receivers[i].SendWithDelay(sender));
			}
		}
		_hasFired = true;
	}

	private void EnsureReceivers()
	{
		if (receivers == null)
		{
			receivers = new ReceiverItem[0];
		}
	}
}
