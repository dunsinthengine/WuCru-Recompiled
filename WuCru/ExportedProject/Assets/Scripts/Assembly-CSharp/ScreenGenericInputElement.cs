using UnityEngine;

public class ScreenGenericInputElement : ScreenInputElementBase
{
	private const string LOG_TAG = "[ScreenGenericInputElement] ";

	public MonoBehaviour _coreInputImpl;

	protected override MonoBehaviour _pCoreInputImpl
	{
		get
		{
			return _coreInputImpl;
		}
	}

	public void SetCoreInputImpl(MonoBehaviour impl)
	{
		_coreInputImpl = impl;
	}

	protected override void OnInitialiseInputElement()
	{
	}
}
