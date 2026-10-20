using System;
using AmuzoEngine;
using UnityEngine;

public class AmuzoDebugMenuButton : AmuzoDebugMenu.IDpadUIButton, DpadUI.IElement
{
	protected class DpadUINestedButton : AmuzoDebugMenu.IDpadUIButton, DpadUI.IElement
	{
		private AmuzoDebugMenuButton _parentButton;

		private string _buttonId;

		private Rect _rect;

		AmuzoDebugMenu AmuzoDebugMenu.IDpadUIButton._pMenu
		{
			get
			{
				return _parentButton._parentMenu;
			}
		}

		string AmuzoDebugMenu.IDpadUIButton._pButtonId
		{
			get
			{
				return _buttonId;
			}
		}

		int DpadUI.IElement._pDepth
		{
			get
			{
				return 0;
			}
		}

		bool DpadUI.IElement._pIsVisible
		{
			get
			{
				return true;
			}
		}

		Rect DpadUI.IElement._pRect
		{
			get
			{
				return _rect;
			}
		}

		DpadUI.IScreen DpadUI.IElement._pScreen
		{
			get
			{
				return _parentButton._parentMenu;
			}
		}

		public string _pButtonId
		{
			get
			{
				return _buttonId;
			}
		}

		public DpadUINestedButton(AmuzoDebugMenuButton parentButton, string nestedButtonId)
		{
			_parentButton = parentButton;
			_buttonId = _parentButton._buttonId + "_" + nestedButtonId;
		}

		void AmuzoDebugMenu.IDpadUIButton.SetRect(Rect rect)
		{
			_rect = rect;
		}

		void DpadUI.IElement.RefreshRect()
		{
		}
	}

	private AmuzoDebugMenu _parentMenu;

	private string _buttonId;

	public string _buttonText;

	public Func<string> _getButtonText;

	public Action _onPress;

	public AmuzoDebugMenu _navigateToOnPress;

	private Rect _buttonRect;

	private Vector2 _lastTopLeft;

	AmuzoDebugMenu AmuzoDebugMenu.IDpadUIButton._pMenu
	{
		get
		{
			return _parentMenu;
		}
	}

	string AmuzoDebugMenu.IDpadUIButton._pButtonId
	{
		get
		{
			return _buttonId;
		}
	}

	DpadUI.IScreen DpadUI.IElement._pScreen
	{
		get
		{
			return _parentMenu;
		}
	}

	Rect DpadUI.IElement._pRect
	{
		get
		{
			return _buttonRect;
		}
	}

	int DpadUI.IElement._pDepth
	{
		get
		{
			return 0;
		}
	}

	bool DpadUI.IElement._pIsVisible
	{
		get
		{
			return true;
		}
	}

	public AmuzoDebugMenuButton(AmuzoDebugMenu parentMenu, string buttonText, Action onPress)
		: this(parentMenu)
	{
		_buttonText = buttonText;
		_getButtonText = null;
		_onPress = onPress;
	}

	public AmuzoDebugMenuButton(AmuzoDebugMenu parentMenu, Func<string> getButtonText, Action onPress)
		: this(parentMenu)
	{
		_buttonText = null;
		_getButtonText = getButtonText;
		_onPress = onPress;
	}

	public AmuzoDebugMenuButton(AmuzoDebugMenu parentMenu, AmuzoDebugMenu navigateToOnPress)
		: this(parentMenu)
	{
		_buttonText = navigateToOnPress._debugMenuName;
		_navigateToOnPress = navigateToOnPress;
	}

	private AmuzoDebugMenuButton(AmuzoDebugMenu parentMenu)
	{
		_parentMenu = parentMenu;
		_buttonId = parentMenu.GenerateButtonId();
	}

	public virtual void OnShow()
	{
		RefreshButtonText();
	}

	public virtual void OnGUI(int buttonIndex)
	{
		Rect buttonPosition = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.GetButtonPosition(buttonIndex);
		AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.OnRenderDpadUIButton(this, buttonPosition);
		_parentMenu.OnRenderDpadUIButton(this, buttonPosition);
		if (GUI.Button(buttonPosition, string.Empty) || AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.IsDpadUIButtonPressed(this))
		{
			if (_onPress != null)
			{
				_onPress();
			}
			else if (_navigateToOnPress != null)
			{
				AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.NavigateTo(_navigateToOnPress);
			}
			RefreshButtonText();
		}
		GUI.Label(buttonPosition, _buttonText, AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._buttonTextStyle);
	}

	public override string ToString()
	{
		return string.Format("{0} [{1}]", _buttonId, GetType().ToString());
	}

	protected void RefreshButtonText()
	{
		if (_getButtonText != null)
		{
			_buttonText = _getButtonText();
		}
	}

	void AmuzoDebugMenu.IDpadUIButton.SetRect(Rect rect)
	{
		_buttonRect = rect;
	}

	void DpadUI.IElement.RefreshRect()
	{
	}
}
