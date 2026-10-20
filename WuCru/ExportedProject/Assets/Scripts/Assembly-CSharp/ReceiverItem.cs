using System;
using System.Collections;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class ReceiverItem
{
	public enum ArgType
	{
		None = 0,
		String = 1
	}

	public GameObject receiver;

	public string action = "OnSignal";

	public float delay;

	public bool receiveSender;

	public ArgType altArgType;

	public string stringArg;

	public ReceiverItem()
	{
	}

	public ReceiverItem(GameObject receiver, string action, float delay = 0f, bool receiveSender = false, ArgType altArgType = ArgType.None, string stringArg = null)
	{
		this.receiver = receiver;
		this.action = action;
		this.delay = delay;
		this.receiveSender = receiveSender;
		this.altArgType = altArgType;
		this.stringArg = stringArg;
	}

	public void SendSignal(MonoBehaviour sender)
	{
		if (receiver != null)
		{
			if (receiveSender)
			{
				receiver.SendMessage(action, sender, SendMessageOptions.DontRequireReceiver);
			}
			else if (altArgType == ArgType.String)
			{
				receiver.SendMessage(action, stringArg, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				receiver.SendMessage(action, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Debug.LogWarning("No receiver of signal \"" + action + "\" on object " + sender.name + " (" + sender.GetType().Name + ")", sender);
		}
	}

	public IEnumerator SendWithDelay(MonoBehaviour sender)
	{
		yield return new WaitForSeconds(delay);
		SendSignal(sender);
		yield return null;
	}
}
