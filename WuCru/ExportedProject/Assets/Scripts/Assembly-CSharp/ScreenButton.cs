using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class ScreenButton : ScreenInputElementBase
{
	private const string LOG_TAG = "[ScreenButton] ";

	private UIButton _buttonImpl;

	private string _pLogTag
	{
		get
		{
			return "[ScreenButton:" + base.name + "] ";
		}
	}

	protected override MonoBehaviour _pCoreInputImpl
	{
		get
		{
			return _buttonImpl;
		}
	}

	protected UIButton _pButtonImpl
	{
		get
		{
			return _buttonImpl;
		}
	}

	public void SimulatePress()
	{
		if (_buttonImpl == null)
		{
			Debug.LogWarning(_pLogTag + "Can not simulate press because no button implementation");
		}
		else
		{
			_buttonImpl.SimulateButtonPressAttempt();
		}
	}

	protected override void OnInitialiseInputElement()
	{
		_buttonImpl = GetComponent<UIButton>();
	}

	protected override void SetInputEnabledImpl(bool wantEnabled)
	{
		if (_buttonImpl != null)
		{
			_buttonImpl.isEnabled = wantEnabled;
		}
	}
}
