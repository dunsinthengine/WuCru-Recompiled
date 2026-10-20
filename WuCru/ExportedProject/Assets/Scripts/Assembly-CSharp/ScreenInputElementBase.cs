using System;
using UnityEngine;

public abstract class ScreenInputElementBase : ScreenElementBase
{
	private const string LOG_TAG = "[ScreenInputElementBase] ";

	[SerializeField]
	private bool _isAllowAnonymousInputDisable;

	[SerializeField]
	private bool _isForceTweenedInOnInputEnabled;

	private BooleanStateRequests _isInputEnabledRequests;

	private bool _wantInputEnabledSelf = true;

	protected bool _isInputEnabled = true;

	private ScreenObjectTransitionWidget _transitionWidget;

	public bool _pIsAllowAnonymousInputDisable
	{
		get
		{
			return _isAllowAnonymousInputDisable;
		}
		set
		{
			_isAllowAnonymousInputDisable = value;
		}
	}

	public bool _pIsForceTweenedInOnInputEnabled
	{
		get
		{
			return _isForceTweenedInOnInputEnabled;
		}
		set
		{
			_isForceTweenedInOnInputEnabled = value;
		}
	}

	public bool _pWantInputEnabledSelf
	{
		get
		{
			return _wantInputEnabledSelf;
		}
		set
		{
			_wantInputEnabledSelf = value;
			RequestInputEnabled(!value, this, 0, false);
		}
	}

	public bool _pIsInputEnabled
	{
		get
		{
			return _isInputEnabled;
		}
	}

	protected abstract MonoBehaviour _pCoreInputImpl { get; }

	private event Action _onInputEnabledStateChanged;

	public event Action _pOnInputEnabledStateChanged
	{
		add
		{
			_onInputEnabledStateChanged += value;
		}
		remove
		{
			_onInputEnabledStateChanged -= value;
		}
	}

	protected virtual void OnInitialiseInputElement()
	{
	}

	protected virtual void SetInputEnabledImpl(bool wantEnabled)
	{
		DefaultSetInputEnabledImpl(wantEnabled);
	}

	public void RequestDisableInput(bool wantDisable, object context)
	{
		RequestInputEnabled(wantDisable, context, 0, false);
	}

	public void DemandEnableInput(bool wantDemandEnable, object context)
	{
		RequestInputEnabled(wantDemandEnable, context, 1, true);
	}

	public void DebugForEachInputEnabledRequest(Action<object, int, bool> requestAction)
	{
		if (_isInputEnabledRequests != null)
		{
			_isInputEnabledRequests.ForEachStateRequest(requestAction);
		}
	}

	protected override void OnInitialiseElement()
	{
		InitialiseInputElement();
	}

	private void InitialiseInputElement()
	{
		_transitionWidget = GetComponent<ScreenObjectTransitionWidget>();
		_isInputEnabledRequests = new BooleanStateRequests(SetInputEnabled, IsInputEnabled, true);
		OnInitialiseInputElement();
	}

	private void RequestInputEnabled(bool wantRequest, object source, int priority, bool wantEnabled)
	{
		CheckInitialiseElement();
		if (wantRequest)
		{
		}
		if (_isInputEnabledRequests != null)
		{
			_isInputEnabledRequests.RequestState(wantRequest, source, priority, wantEnabled);
		}
	}

	private void SetInputEnabled(bool wantEnabled)
	{
		if (_isInputEnabled != wantEnabled)
		{
			if (wantEnabled && _isForceTweenedInOnInputEnabled && _transitionWidget != null && !_transitionWidget._pConsideredTweenedIn)
			{
				_transitionWidget.SetStateConsideredTweenedIn(true);
			}
			SetInputEnabledImpl(wantEnabled);
			_isInputEnabled = wantEnabled;
			if (_onInputEnabledStateChanged != null)
			{
				_onInputEnabledStateChanged();
			}
		}
	}

	private bool IsInputEnabled()
	{
		return _isInputEnabled;
	}

	private void DefaultSetInputEnabledImpl(bool wantEnabled)
	{
		if (_pCoreInputImpl != null)
		{
			_pCoreInputImpl.enabled = wantEnabled;
		}
	}
}
