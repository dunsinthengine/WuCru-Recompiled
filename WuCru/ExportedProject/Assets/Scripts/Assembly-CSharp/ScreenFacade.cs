using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ScreenFacade : MonoBehaviour
{
	private const bool DO_DEBUG = true;

	private static Comparison<ScreenFlow> _sCompareScreenFlowDepth = (ScreenFlow f1, ScreenFlow f2) =>
	{
		ScreenBase pCurrentScreen = f1._pCurrentScreen;
		ScreenBase pCurrentScreen2 = f2._pCurrentScreen;
		return (pCurrentScreen == null) ? ((!(pCurrentScreen2 == null)) ? 1 : 0) : ((!(pCurrentScreen2 == null)) ? pCurrentScreen._pDepth.CompareTo(pCurrentScreen2._pDepth) : (-1));
	};

	private List<ScreenBase> _screens;

	private Dictionary<Type, ScreenBase> _screenSingletons;

	public UICamera _mainUICamera;

	[SerializeField]
	private int _screenFlowCountMax = 4;

	private ObjectPool<ScreenFlow> _screenFlowPool;

	private ScreenFlow _mainScreenFlow;

	private List<ScreenFlow> _activeScreenFlows;

	private ScreenBase _currentScreen;

	private StringBuilder _currentLocationBuilder;

	private string _currentLocation;

	private string _previousLocation;

	public ScreenBase _pCurrentScreen
	{
		get
		{
			return _currentScreen;
		}
	}

	public string _pCurrentLocation
	{
		get
		{
			return _currentLocation;
		}
	}

	public string _pPreviousLocation
	{
		get
		{
			return _previousLocation;
		}
	}

	public ScreenFlow _pMainScreenFlow
	{
		get
		{
			return _mainScreenFlow;
		}
	}

	public bool _pIsAnyScreenTweening
	{
		get
		{
			if (_activeScreenFlows == null)
			{
				return false;
			}
			for (int i = 0; i < _activeScreenFlows.Count; i++)
			{
				if (_activeScreenFlows[i] != null && _activeScreenFlows[i]._pIsTransitioning)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static event Action<ScreenBase> _pOnRegisterScreen;

	public static event Action _pOnCurrentScreenChanged;

	public static event Action _pOnCurrentLocationChanged;

	public void Awake()
	{
		Facades<ScreenFacade>.Register(this);
		_screens = new List<ScreenBase>();
		_screenSingletons = new Dictionary<Type, ScreenBase>();
		_currentLocationBuilder = new StringBuilder();
		InitScreenFlows();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RegisterScreen(ScreenBase screen)
	{
		if (!_screens.Contains(screen))
		{
			_screens.Add(screen);
		}
		Type type = screen.GetType();
		if (!_screenSingletons.ContainsKey(type))
		{
			_screenSingletons.Add(type, screen);
		}
		else if (_screenSingletons[type] != screen)
		{
			Debug.LogError(string.Concat("[ScreenFacade] Different ", type, " singleton already registered!"));
		}
		if (_pOnRegisterScreen != null)
		{
			_pOnRegisterScreen(screen);
		}
		Log(string.Concat("Screen Registered [", type, "]"));
	}

	public void ShowScreen(string screenName, bool doClearHistory = false, bool doAddCurrentScreenToHistory = true, Action onComplete = null)
	{
		if (!string.IsNullOrEmpty(screenName))
		{
			ScreenBase screenBase = FindScreen(screenName);
			if (screenBase == null)
			{
				Debug.LogError("ScreenFacade: Failed to find screen: " + screenName);
			}
			else
			{
				ShowScreen(screenBase, doClearHistory, doAddCurrentScreenToHistory, onComplete);
			}
		}
	}

	public void ShowScreen(ScreenBase screen, bool doClearHistory = false, bool doAddCurrentScreenToHistory = true, Action onComplete = null)
	{
		if (_mainScreenFlow == null)
		{
			Debug.LogError("ScreenFacade: ShowScreen: Null _mainScreenFlow");
		}
		else
		{
			_mainScreenFlow.ShowScreen(screen, doClearHistory, doAddCurrentScreenToHistory, onComplete);
		}
	}

	public void ShowPreviousScreen(Action onComplete = null)
	{
		if (_mainScreenFlow == null)
		{
			Debug.LogError("ScreenFacade: ShowPreviousScreen: Null _mainScreenFlow");
		}
		else
		{
			_mainScreenFlow.ShowPreviousScreen(onComplete);
		}
	}

	public void ShowPopupOverlay(ScreenBase screen, Action onShowComplete = null, Action onFlowEnd = null)
	{
		ScreenFlow newFlow = null;
		newFlow = CreateScreenFlow(() =>
		{
			if (onFlowEnd != null)
			{
				onFlowEnd();
			}
			DestroyScreenFlow(newFlow);
		});
		if (newFlow == null)
		{
			Debug.LogError("ScreenFacade: ShowScreenInNewFlow: Failed to create new flow");
		}
		else
		{
			newFlow.ShowScreen(screen, false, true, onShowComplete);
		}
	}

	public bool IsScreenOnTop(ScreenBase screen)
	{
		return screen._pScreenFlow == _activeScreenFlows[_activeScreenFlows.Count - 1];
	}

	public void ForEachActiveScreen(Action<ScreenBase> action)
	{
		if (_activeScreenFlows == null || action == null)
		{
			return;
		}
		for (int i = 0; i < _activeScreenFlows.Count; i++)
		{
			if (_activeScreenFlows[i] != null && !(_activeScreenFlows[i]._pCurrentScreen == null))
			{
				_activeScreenFlows[i]._pCurrentScreen.ForEachLayer(true, action);
			}
		}
	}

	public ScreenBase FindActiveScreen(Predicate<ScreenBase> pred)
	{
		if (_activeScreenFlows == null)
		{
			return null;
		}
		if (pred == null)
		{
			return null;
		}
		ScreenBase screenBase = null;
		for (int i = 0; i < _activeScreenFlows.Count; i++)
		{
			if (_activeScreenFlows[i] != null && !(_activeScreenFlows[i]._pCurrentScreen == null))
			{
				screenBase = _activeScreenFlows[i]._pCurrentScreen.FindLayer(true, pred);
				if (screenBase != null)
				{
					break;
				}
			}
		}
		return screenBase;
	}

	public T FindActiveScreenOfType<T>() where T : class
	{
		return FindActiveScreen((ScreenBase s) => s is T) as T;
	}

	public ScreenBase FindScreen(string screenName)
	{
		if (_screens == null)
		{
			return null;
		}
		return _screens.Find((ScreenBase s) => s.gameObject.name == screenName);
	}

	public T GetScreenInstance<T>() where T : ScreenBase
	{
		if (_screenSingletons == null)
		{
			return (T)null;
		}
		Type typeFromHandle = typeof(T);
		if (!_screenSingletons.ContainsKey(typeFromHandle))
		{
			return (T)null;
		}
		return _screenSingletons[typeFromHandle] as T;
	}

	private void InitScreenFlows()
	{
		_screenFlowPool = new ObjectPool<ScreenFlow>(_screenFlowCountMax, () =>
		{
			ScreenFlow newFlow = new ScreenFlow();
			newFlow._pOnScreenChanged += () =>
			{
				OnScreenChanged(newFlow);
			};
			return newFlow;
		}, null);
		_mainScreenFlow = _screenFlowPool.Allocate();
		_activeScreenFlows = new List<ScreenFlow>(_screenFlowCountMax);
		_activeScreenFlows.Add(_mainScreenFlow);
	}

	private ScreenFlow CreateScreenFlow(Action onFlowEnd)
	{
		if (_screenFlowPool == null)
		{
			return null;
		}
		ScreenFlow screenFlow = _screenFlowPool.Allocate();
		if (screenFlow == null)
		{
			return null;
		}
		screenFlow.Reset();
		screenFlow._pOnEnd += onFlowEnd;
		if (_activeScreenFlows != null && !_activeScreenFlows.Contains(screenFlow))
		{
			_activeScreenFlows.Add(screenFlow);
		}
		return screenFlow;
	}

	private void DestroyScreenFlow(ScreenFlow flow)
	{
		if (flow != null)
		{
			if (_activeScreenFlows != null && _activeScreenFlows.Contains(flow))
			{
				_activeScreenFlows.Remove(flow);
			}
			if (_screenFlowPool != null)
			{
				_screenFlowPool.Free(flow);
			}
		}
	}

	private void OnScreenChanged(ScreenFlow screenFlow)
	{
		RefreshActiveScreenOrder();
		RefreshCurrentScreen();
		RefreshCurrentLocation();
	}

	private void RefreshActiveScreenOrder()
	{
		_activeScreenFlows.Sort(_sCompareScreenFlowDepth);
	}

	private void RefreshCurrentScreen()
	{
		ScreenBase screenBase = null;
		int num = _activeScreenFlows.Count - 1;
		while (num >= 0 && screenBase == null)
		{
			screenBase = _activeScreenFlows[num]._pCurrentScreen;
			num--;
		}
		if (_currentScreen != screenBase)
		{
			ScreenBase currentScreen = _currentScreen;
			_currentScreen = screenBase;
			OnCurrentScreenChanged();
		}
	}

	private void RefreshCurrentLocation()
	{
		_currentLocationBuilder.Length = 0;
		for (int i = 0; i < _activeScreenFlows.Count; i++)
		{
			if (!(_activeScreenFlows[i]._pCurrentScreen == null))
			{
				if (i > 0)
				{
					_currentLocationBuilder.Append('.');
				}
				_currentLocationBuilder.Append(_activeScreenFlows[i]._pCurrentScreen._pScreenName);
			}
		}
		string text = _currentLocationBuilder.ToString();
		if (_currentLocation != text)
		{
			_previousLocation = _currentLocation;
			_currentLocation = text;
			OnCurrentLocationChanged();
		}
	}

	private void OnCurrentScreenChanged()
	{
		if (_pOnCurrentScreenChanged != null)
		{
			_pOnCurrentScreenChanged();
		}
	}

	private void OnCurrentLocationChanged()
	{
		if (_pOnCurrentLocationChanged != null)
		{
			_pOnCurrentLocationChanged();
		}
	}

	public void DebugRender()
	{
		int num = 25;
		if (_activeScreenFlows == null)
		{
			return;
		}
		for (int i = 0; i < _activeScreenFlows.Count; i++)
		{
			if (_activeScreenFlows[i] != null)
			{
				_activeScreenFlows[i].DebugRender(num, 25, 180, 25);
				num += 180;
			}
		}
	}

	public static void Log(string message, UnityEngine.Object o = null)
	{
	}
}
