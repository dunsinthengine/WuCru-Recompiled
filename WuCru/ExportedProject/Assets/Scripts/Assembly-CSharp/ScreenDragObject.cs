using UnityEngine;

[RequireComponent(typeof(UIDragObject))]
public class ScreenDragObject : ScreenInputElementBase
{
	private const string LOG_TAG = "[ScreenDragObject] ";

	private UIDragObject _dragObjectImpl;

	protected override MonoBehaviour _pCoreInputImpl
	{
		get
		{
			return _dragObjectImpl;
		}
	}

	protected override void OnInitialiseInputElement()
	{
		_dragObjectImpl = GetComponent<UIDragObject>();
	}
}
