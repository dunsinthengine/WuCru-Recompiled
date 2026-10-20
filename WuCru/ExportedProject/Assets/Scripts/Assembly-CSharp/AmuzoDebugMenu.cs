using System;
using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class AmuzoDebugMenu : DpadUI.IScreen
{
	public interface IDpadUIButton : DpadUI.IElement
	{
		AmuzoDebugMenu _pMenu { get; }

		string _pButtonId { get; }

		void SetRect(Rect rect);
	}

	public static string NEW_LINE = string.Empty + '\n' + '\r';

	public static string TAB = string.Empty + '\t';

	public static string DOUBLE_TAB = string.Empty + '\t' + '\t';

	public string _debugMenuName;

	private List<AmuzoDebugMenuButton> _debugMenuButtons = new List<AmuzoDebugMenuButton>();

	private SimpleIntegerIdGenerator _buttonIdGenerator = new SimpleIntegerIdGenerator();

	protected Func<string> _refreshTextAreaFunction;

	protected string _debugText = string.Empty;

	protected bool _hasTextDefined;

	protected Vector2 _infoScrollPosition = Vector2.zero;

	protected Vector2 _buttonScrollPosition = Vector2.zero;

	protected Rect _textContentRect;

	protected Rect _buttonRect;

	protected bool _isShowing;

	private int _menuDepth = -1;

	int DpadUI.IScreen._pElementCount
	{
		get
		{
			return 0;
		}
	}

	DpadUI.IElement DpadUI.IScreen._pInitialFocusElement
	{
		get
		{
			return null;
		}
	}

	int DpadUI.IScreen._pDepth
	{
		get
		{
			return _menuDepth;
		}
	}

	bool DpadUI.IScreen._pIsBlocking
	{
		get
		{
			return true;
		}
	}

	bool DpadUI.IScreen._pIsScrollingHorizontal
	{
		get
		{
			return false;
		}
	}

	bool DpadUI.IScreen._pIsScrollingVertical
	{
		get
		{
			return true;
		}
	}

	public bool _pIsShowing
	{
		get
		{
			return _isShowing;
		}
	}

	public AmuzoDebugMenu(string menuName)
	{
		_debugMenuName = menuName;
		AddButton(new AmuzoDebugMenuButton(this, "...", AmuzoDebugMenuManager.NavigateBack));
	}

	public void AddButton(AmuzoDebugMenuButton newButton)
	{
		if (!_debugMenuButtons.Contains(newButton))
		{
			_debugMenuButtons.Add(newButton);
			RefreshButtonRect();
			if (_isShowing)
			{
				newButton.OnShow();
			}
		}
	}

	public void RemoveButton(AmuzoDebugMenuButton button)
	{
		if (_debugMenuButtons.Contains(button))
		{
			OnRemoveDpadUIButton(button);
			_debugMenuButtons.Remove(button);
			RefreshButtonRect();
		}
	}

	public void AddInfoTextFunction(Func<string> textAreaFunction, bool addTextRefreshButton = true)
	{
		_refreshTextAreaFunction = textAreaFunction;
		if (addTextRefreshButton)
		{
			AddButton(new AmuzoDebugMenuButton(this, "Refresh Info", RefreshText));
		}
	}

	public void Show(int menuDepth)
	{
		OnPreShow();
		_isShowing = true;
		RecalculateSizes();
		for (int i = 0; i < _debugMenuButtons.Count; i++)
		{
			if (_debugMenuButtons[i] != null)
			{
				_debugMenuButtons[i].OnShow();
			}
		}
		_menuDepth = menuDepth;
		OnShown();
	}

	public void Hide()
	{
		_isShowing = false;
		_menuDepth = -1;
		OnHidden();
	}

	public virtual void OnGUI()
	{
		GUI.Label(AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._menuTitleArea, _debugMenuName, AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._titleTextStyle);
		DrawButtons();
		DrawInfoText();
	}

	public virtual void RecalculateSizes()
	{
		_infoScrollPosition = Vector2.zero;
		_buttonScrollPosition = Vector2.zero;
		RefreshText();
		RefreshButtonRect();
	}

	public string GenerateButtonId()
	{
		return _debugMenuName + "_Button_" + _buttonIdGenerator._pNew;
	}

	public void OnRenderDpadUIButton(IDpadUIButton button, Rect buttonRect)
	{
	}

	public void OnRemoveDpadUIButton(IDpadUIButton button)
	{
	}

	protected virtual void OnPreShow()
	{
	}

	protected virtual void OnShown()
	{
	}

	protected virtual void OnHidden()
	{
	}

	public virtual void RefreshText()
	{
		if (_refreshTextAreaFunction != null)
		{
			_debugText = _refreshTextAreaFunction();
			_hasTextDefined = true;
			_textContentRect = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextDisplayArea;
			_textContentRect.width -= AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._scrollBarWidth + 5f;
			_textContentRect.height = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextStyle.CalcHeight(new GUIContent(_debugText), _textContentRect.width);
		}
	}

	private void RefreshButtonRect()
	{
		_buttonRect = _buttonRect.Combine(AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.GetButtonPosition(0));
		for (int i = 1; i < _debugMenuButtons.Count; i++)
		{
			_buttonRect = _buttonRect.Combine(AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.GetButtonPosition(i));
		}
		_buttonRect.width -= AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._scrollBarWidth + 5f;
	}

	protected virtual void DrawButtons()
	{
		_buttonScrollPosition = GUI.BeginScrollView(AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._menuButtonArea, _buttonScrollPosition, _buttonRect);
		GUI.enabled = true;
		for (int i = 0; i < _debugMenuButtons.Count; i++)
		{
			_debugMenuButtons[i].OnGUI(i);
		}
		GUI.enabled = false;
		Color color = GUI.color;
		float a = color.a;
		color.a = 0.5f;
		GUI.color = color;
		int j = _debugMenuButtons.Count;
		for (int num = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._buttonColoumns * AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._buttonRows; j < num; j++)
		{
			GUI.Button(AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.GetButtonPosition(j), string.Empty);
		}
		color.a = a;
		GUI.color = color;
		GUI.enabled = true;
		GUI.EndScrollView();
	}

	protected virtual void DrawInfoText()
	{
		if (_hasTextDefined)
		{
			_infoScrollPosition = GUI.BeginScrollView(AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextDisplayArea, _infoScrollPosition, _textContentRect);
			GUI.Label(_textContentRect, _debugText, AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextStyle);
			GUI.EndScrollView();
		}
	}

	DpadUI.IElement DpadUI.IScreen.GetElement(int i)
	{
		return null;
	}

	bool DpadUI.IScreen.GetBlockingElementDepth(out int blockingElementDepth)
	{
		blockingElementDepth = -1;
		return false;
	}
}
