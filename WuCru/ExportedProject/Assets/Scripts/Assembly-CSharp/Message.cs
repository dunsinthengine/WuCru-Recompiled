using System;
using UnityEngine;

public class Message : EditorState
{
	public string msg = "Message...";

	private GameObject msgBoxGO;

	private MessageBox msgBox;

	private Action onCloseAction;

	public Action OnCloseAction
	{
		set
		{
			onCloseAction = value;
		}
	}

	public override void Begin()
	{
		msgBoxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/MessageBox"));
		msgBox = msgBoxGO.GetComponent<MessageBox>();
		msgBox.msg = msg;
		MessageBox messageBox = msgBox;
		messageBox.onClose = (MessageBox.OnCloseDelegate)Delegate.Combine(messageBox.onClose, new MessageBox.OnCloseDelegate(OnClose));
	}

	public override void End()
	{
		UnityEngine.Object.Destroy(msgBoxGO);
	}

	public void OnClose()
	{
		EditorStateMachine.Instance.PopState();
		if (onCloseAction != null)
		{
			onCloseAction();
		}
	}
}
