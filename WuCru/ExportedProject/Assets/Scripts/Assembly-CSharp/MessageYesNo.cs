using System;
using UnityEngine;

public class MessageYesNo : EditorState
{
	public string msg = "Message...";

	private GameObject msgBoxGO;

	private MessageBoxYesNo msgBox;

	private Action onOkAction;

	private Action onCancelAction;

	public Action OnOkAction
	{
		set
		{
			onOkAction = value;
		}
	}

	public Action OnCancelAction
	{
		set
		{
			onCancelAction = value;
		}
	}

	public override void Begin()
	{
		msgBoxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/MessageBoxYesNo"));
		msgBox = msgBoxGO.GetComponent<MessageBoxYesNo>();
		msgBox.msg = msg;
		MessageBoxYesNo messageBoxYesNo = msgBox;
		messageBoxYesNo.onOk = (MessageBoxYesNo.OnOkDelegate)Delegate.Combine(messageBoxYesNo.onOk, new MessageBoxYesNo.OnOkDelegate(OnOk));
		MessageBoxYesNo messageBoxYesNo2 = msgBox;
		messageBoxYesNo2.onCancel = (MessageBoxYesNo.OnCancelDelegate)Delegate.Combine(messageBoxYesNo2.onCancel, new MessageBoxYesNo.OnCancelDelegate(OnCancel));
	}

	public override void End()
	{
		UnityEngine.Object.Destroy(msgBoxGO);
	}

	public void OnOk()
	{
		EditorStateMachine.Instance.PopState();
		if (onOkAction != null)
		{
			onOkAction();
		}
	}

	public void OnCancel()
	{
		EditorStateMachine.Instance.PopState();
		if (onCancelAction != null)
		{
			onCancelAction();
		}
	}
}
