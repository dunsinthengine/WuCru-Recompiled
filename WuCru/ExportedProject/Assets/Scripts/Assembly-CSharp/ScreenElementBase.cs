using System;
using UnityEngine;

public class ScreenElementBase : MonoBehaviour
{
	public enum EHideMethod
	{
		NONE = 0,
		ALPHA = 1,
		ENABLED = 2,
		RENDERER = 3,
		ACTIVE = 4
	}

	private const string LOG_TAG = "[ScreenElementBase] ";

	[SerializeField]
	private EHideMethod _hideMethod = EHideMethod.ACTIVE;

	private bool _isInitialised;

	protected ScreenBase _screen;

	private UIWidget _widget;

	private Renderer _renderer;

	private BooleanStateRequests _isVisibleRequests;

	private bool _wantVisibleSelf = true;

	private bool _isVisible = true;

	private float _cachedAlpha;

	public bool _pIsVisible
	{
		get
		{
			return base.gameObject.activeInHierarchy && _isVisible;
		}
	}

	public EHideMethod _pHideMethod
	{
		get
		{
			return _hideMethod;
		}
		set
		{
			_hideMethod = value;
		}
	}

	public bool _pWantVisibleSelf
	{
		get
		{
			return _wantVisibleSelf;
		}
		set
		{
			_wantVisibleSelf = value;
			RequestVisible(!value, this, 0, false);
		}
	}

	public ScreenBase _pScreen
	{
		get
		{
			return _screen;
		}
	}

	private event Action _onVisibleStateChanged;

	public event Action _pOnVisibleStateChanged
	{
		add
		{
			_onVisibleStateChanged += value;
		}
		remove
		{
			_onVisibleStateChanged -= value;
		}
	}

	protected virtual void OnInitialiseElement()
	{
	}

	protected virtual void Awake()
	{
		CheckInitialiseElement();
	}

	public void RequestHide(bool wantHidden, object context)
	{
		RequestVisible(wantHidden, context, 0, false);
	}

	protected void CheckInitialiseElement()
	{
		if (!_isInitialised)
		{
			InitialiseElement();
		}
	}

	private void InitialiseElement()
	{
		_screen = GetComponentInParent<ScreenBase>();
		_widget = GetComponent<UIWidget>();
		_renderer = GetComponent<Renderer>();
		_isVisibleRequests = new BooleanStateRequests(SetVisible, IsVisible, true);
		OnInitialiseElement();
		_isInitialised = true;
	}

	private void RequestVisible(bool wantRequest, object source, int priority, bool wantVisible)
	{
		CheckInitialiseElement();
		if (wantRequest)
		{
		}
		if (_isVisibleRequests != null)
		{
			_isVisibleRequests.RequestState(wantRequest, source, priority, wantVisible);
		}
	}

	private void SetVisible(bool wantVisible)
	{
		if (_isVisible == wantVisible)
		{
			return;
		}
		switch (_hideMethod)
		{
		case EHideMethod.ALPHA:
			if (!(_widget == null))
			{
				if (wantVisible)
				{
					_widget.alpha = _cachedAlpha;
					break;
				}
				_cachedAlpha = _widget.alpha;
				_widget.alpha = 0f;
			}
			break;
		case EHideMethod.ENABLED:
			if (!(_widget == null))
			{
				_widget.enabled = wantVisible;
			}
			break;
		case EHideMethod.RENDERER:
			if (!(_renderer == null))
			{
				_renderer.enabled = wantVisible;
			}
			break;
		case EHideMethod.ACTIVE:
			base.gameObject.SetActive(wantVisible);
			break;
		}
		_isVisible = wantVisible;
		if (_onVisibleStateChanged != null)
		{
			_onVisibleStateChanged();
		}
	}

	private bool IsVisible()
	{
		return _isVisible;
	}
}
