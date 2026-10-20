using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFlow
{
	private const string LOG_TAG = "[ScreenFlow] ";

	private ScreenBase _currentScreen;

	private List<ScreenBase> _screenHistory;

	private ScreenTransition _screenTransition;

	public ScreenBase _pCurrentScreen
	{
		get
		{
			return _currentScreen ?? _screenTransition._pCurrentScreen;
		}
	}

	public bool _pIsTransitioning
	{
		get
		{
			return _screenTransition._pCurrentStage != ScreenTransition.EStage.NULL;
		}
	}

	public event Action _pOnScreenChanged;

	public event Action _pOnEnd;

	public ScreenFlow()
	{
		_screenHistory = new List<ScreenBase>();
		_screenTransition = new ScreenTransition();
		Reset();
	}

	public void Reset()
	{
		_pOnEnd = null;
		_currentScreen = null;
		_screenHistory.Clear();
		_screenTransition.Reset();
	}

	public void ShowScreen(ScreenBase nextScreen, bool doClearHistory = false, bool doAddCurrentScreenToHistory = true, Action onComplete = null)
	{
		if (nextScreen == null)
		{
			Debug.LogError("[ScreenFlow] Null nextScreen");
		}
		else
		{
			DoScreenTransition(doClearHistory, doAddCurrentScreenToHistory && !doClearHistory, nextScreen, onComplete);
		}
	}

	public void ShowPreviousScreen(Action onComplete = null)
	{
		if ((_currentScreen == null || _screenHistory == null || _screenHistory.Count == 0) && onComplete != null)
		{
			onComplete();
		}
		ScreenBase screenBase = _screenHistory[_screenHistory.Count - 1];
		_screenHistory.RemoveAt(_screenHistory.Count - 1);
		if (screenBase == null)
		{
			Debug.LogError("[ScreenFlow] Null prevScreen");
		}
		else
		{
			DoScreenTransition(false, false, screenBase, onComplete);
		}
	}

	public void End(Action onComplete = null)
	{
		DoScreenTransition(false, true, null, () =>
		{
			if (_pOnEnd != null)
			{
				_pOnEnd();
			}
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	public void DebugRender(int leftX, int topY, int columnWidth, int lineHeight)
	{
		Rect position = new Rect(leftX, topY, columnWidth, lineHeight);
		if (_screenTransition != null && _screenTransition._pCurrentStage != ScreenTransition.EStage.NULL)
		{
			position.y = lineHeight >> 1;
			if (_screenTransition._pScreenIn != null)
			{
				GUI.Box(position, _screenTransition._pScreenIn.name);
			}
			else
			{
				GUI.Box(position, string.Empty);
			}
			position.y += lineHeight;
			if (_screenTransition._pScreenOut != null)
			{
				GUI.Box(position, _screenTransition._pScreenOut.name);
			}
			else
			{
				GUI.Box(position, string.Empty);
			}
			position.y += lineHeight;
		}
		else
		{
			position.y = lineHeight;
			if (_currentScreen != null)
			{
				GUI.Box(position, _currentScreen.name);
			}
			else
			{
				GUI.Box(position, string.Empty);
			}
			position.y += lineHeight;
		}
		position.y = 3 * lineHeight;
		if (_screenHistory == null)
		{
			return;
		}
		for (int num = _screenHistory.Count - 1; num >= 0; num--)
		{
			if (_screenHistory[num] != null)
			{
				GUI.Box(position, _screenHistory[num].name);
			}
			else
			{
				GUI.Box(position, "null");
			}
			position.y += lineHeight;
		}
	}

	private void DoScreenTransition(bool doClearHistory, bool doAddCurrScreenToHistory, ScreenBase nextScreen, Action onComplete)
	{
		if (doClearHistory)
		{
			_screenHistory.Clear();
		}
		Action onComplete2 = () =>
		{
			_currentScreen = nextScreen;
			if (_pOnScreenChanged != null)
			{
				_pOnScreenChanged();
			}
			if (onComplete != null)
			{
				onComplete();
			}
		};
		if (_screenTransition._pCurrentStage != ScreenTransition.EStage.NULL)
		{
			if (_screenTransition._pCurrentStage == ScreenTransition.EStage.OUT)
			{
				_screenTransition.Modify(nextScreen, onComplete2);
				return;
			}
			if (_screenTransition._pCurrentStage == ScreenTransition.EStage.IN)
			{
				_screenTransition.Cancel();
			}
		}
		if (_currentScreen != null)
		{
			if (doAddCurrScreenToHistory)
			{
				_screenHistory.Add(_currentScreen);
			}
			ScreenBase currentScreen = _currentScreen;
			_currentScreen = null;
			_screenTransition.Init(currentScreen, nextScreen, onComplete2);
		}
		else
		{
			_screenTransition.Init(this, nextScreen, onComplete2);
		}
	}
}
