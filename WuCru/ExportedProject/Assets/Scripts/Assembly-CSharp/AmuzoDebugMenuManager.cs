using System.Collections.Generic;
using UnityEngine;

public class AmuzoDebugMenuManager : AmuzoMonoSingleton<AmuzoDebugMenuManager>
{
	private const string DEBUG_MENU_VERSION = "1.0";

	private const float HOLD_TIME_TO_SHOW_MENU = 3f;

	private const float TEXT_AREA_PADDING = 5f;

	private const string DPADUI_BUTTON_NAME_DEFAULT = "Submit";

	private const string DPADUI_BUTTON_NAME = "Submit";

	private static List<AmuzoDebugMenu> _rootMenus = new List<AmuzoDebugMenu>();

	[SerializeField]
	private Rect _titleArea;

	[SerializeField]
	private Rect _infoDisplayArea;

	[SerializeField]
	private Rect _buttonArea;

	public int _buttonColoumns = 6;

	public int _buttonRows = 4;

	public float _buttonSpacing = 5f;

	public float _scrollBarWidth = 20f;

	public GUIStyle _titleTextStyle;

	public GUIStyle _infoTextStyle;

	public GUIStyle _buttonTextStyle;

	public GUISkin _debugMenuSkin;

	[HideInInspector]
	public Rect _infoTextDisplayArea;

	private float _buttonWidth;

	private float _buttonHeight;

	private float _menuTimer;

	private List<AmuzoDebugMenu> _navigationStack = new List<AmuzoDebugMenu>();

	private Texture2D _blackPixel;

	private float _titleTextFontHeight = -1f;

	private float _infoTextFontHeight = -1f;

	private float _buttonTextFont = -1f;

	private float _lastScreenWidth = -1f;

	private float _lastScreenHeight = -1f;

	public Rect _menuTitleArea { get; private set; }

	public Rect _menuInfoDisplayArea { get; private set; }

	public Rect _menuButtonArea { get; private set; }

	protected override void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
	}

	public void NavigateToPrevious()
	{
		HideMenu(_navigationStack[_navigationStack.Count - 1], true);
		_navigationStack.RemoveAt(_navigationStack.Count - 1);
		if (_navigationStack.Count > 0)
		{
			ShowMenu(_navigationStack[_navigationStack.Count - 1], true);
		}
	}

	public void NavigateTo(AmuzoDebugMenu newDebugMenu)
	{
		if (_navigationStack.Count > 0)
		{
			HideMenu(_navigationStack[_navigationStack.Count - 1], false);
		}
		_navigationStack.Add(newDebugMenu);
		ShowMenu(newDebugMenu, false);
	}

	public Rect GetButtonPosition(int position)
	{
		float x = _buttonSpacing + (_buttonWidth + _buttonSpacing) * (float)(position % _buttonColoumns);
		float y = _buttonSpacing + (_buttonHeight + _buttonSpacing) * Mathf.Floor(position / _buttonColoumns);
		return new Rect(x, y, _buttonWidth, _buttonHeight);
	}

	public static void NavigateBack()
	{
		if (AmuzoMonoSingleton<AmuzoDebugMenuManager>._pExists)
		{
			AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.NavigateToPrevious();
		}
	}

	public static void RegisterRootDebugMenu(AmuzoDebugMenu newRootMenu)
	{
		if (!_rootMenus.Contains(newRootMenu))
		{
			_rootMenus.Add(newRootMenu);
		}
	}

	public bool IsDpadUIButtonPressed(AmuzoDebugMenu.IDpadUIButton button)
	{
		return false;
	}

	public void OnRenderDpadUIButton(AmuzoDebugMenu.IDpadUIButton button, Rect buttonRect)
	{
	}

	public void OnDpadUIButtonsChanged(AmuzoDebugMenu menu)
	{
	}

	protected override void OnInitialise()
	{
		base.OnInitialise();
		AmuzoDebugMenu_SystemInfo newRootMenu = new AmuzoDebugMenu_SystemInfo("SYSTEM INFO");
		RegisterRootDebugMenu(newRootMenu);
		AmuzoDebugMenu_BuildInfo newRootMenu2 = new AmuzoDebugMenu_BuildInfo("BUILD INFO");
		RegisterRootDebugMenu(newRootMenu2);
		AmuzoDebugMenu_SceneHierarchy newRootMenu3 = new AmuzoDebugMenu_SceneHierarchy("SCENE HIERARCHY");
		RegisterRootDebugMenu(newRootMenu3);
		_blackPixel = new Texture2D(1, 1);
		_blackPixel.SetPixel(0, 0, Color.black);
		_blackPixel.Apply();
		if (_titleTextStyle == null)
		{
			_titleTextFontHeight = 12f;
			_titleTextStyle = new GUIStyle();
		}
		else
		{
			_titleTextFontHeight = _titleTextStyle.fontSize;
		}
		if (_infoTextStyle == null)
		{
			_infoTextFontHeight = 8f;
			_infoTextStyle = new GUIStyle();
		}
		else
		{
			_infoTextFontHeight = _infoTextStyle.fontSize;
		}
		if (_buttonTextStyle == null)
		{
			_buttonTextFont = 10f;
			_buttonTextStyle = new GUIStyle();
		}
		else
		{
			_buttonTextFont = _buttonTextStyle.fontSize;
		}
		if (_debugMenuSkin == null)
		{
			_debugMenuSkin = new GUISkin();
		}
		RecalculateMenuSizes();
	}

	private void RecalculateMenuSizes()
	{
		_menuTitleArea = DecimaltoScreenRect(_titleArea);
		_menuInfoDisplayArea = DecimaltoScreenRect(_infoDisplayArea);
		_menuButtonArea = DecimaltoScreenRect(_buttonArea);
		_infoTextDisplayArea = _menuInfoDisplayArea;
		_infoTextDisplayArea.x += 5f;
		_infoTextDisplayArea.width -= 10f;
		_infoTextDisplayArea.y += 5f;
		_infoTextDisplayArea.height -= 10f;
		_buttonWidth = (_menuButtonArea.width - _buttonSpacing * (float)(_buttonColoumns + 1)) / (float)_buttonColoumns;
		_buttonHeight = (_menuButtonArea.height - _buttonSpacing * (float)(_buttonRows + 1)) / (float)_buttonRows;
		float num = (float)Screen.height / 768f;
		_titleTextStyle.fontSize = Mathf.FloorToInt(num * _titleTextFontHeight);
		_infoTextStyle.fontSize = Mathf.FloorToInt(num * _infoTextFontHeight);
		_buttonTextStyle.fontSize = Mathf.FloorToInt(num * _buttonTextFont);
		float f = (_scrollBarWidth - 2f) * num;
		float f2 = Mathf.FloorToInt(_scrollBarWidth * num);
		_debugMenuSkin.verticalScrollbar.fixedWidth = Mathf.FloorToInt(f2);
		_debugMenuSkin.verticalScrollbarThumb.fixedWidth = Mathf.FloorToInt(f2);
		_debugMenuSkin.verticalSlider.fixedWidth = Mathf.FloorToInt(f);
		_debugMenuSkin.verticalSliderThumb.fixedWidth = Mathf.FloorToInt(f);
		for (int i = 0; i < _navigationStack.Count; i++)
		{
			_navigationStack[i].RecalculateSizes();
		}
		_lastScreenWidth = Screen.width;
		_lastScreenHeight = Screen.height;
	}

	private void OnShow()
	{
		AmuzoDebugMenu amuzoDebugMenu = new AmuzoDebugMenu("MAIN DEBUG MENU");
		amuzoDebugMenu.AddInfoTextFunction(() => "AMUZO DEBUG MENU v1.0", false);
		int num = 0;
		for (int count = _rootMenus.Count; num < count; num++)
		{
			amuzoDebugMenu.AddButton(new AmuzoDebugMenuButton(amuzoDebugMenu, _rootMenus[num]));
		}
		NavigateTo(amuzoDebugMenu);
	}

	private void ShowMenu(AmuzoDebugMenu menu, bool isNavigateBack)
	{
		menu.Show(_navigationStack.FindIndex((AmuzoDebugMenu m) => m == menu));
		if (isNavigateBack)
		{
		}
	}

	private void HideMenu(AmuzoDebugMenu menu, bool isNavigateBack)
	{
		if (isNavigateBack)
		{
			menu.Hide();
		}
	}

	private Rect DecimaltoScreenRect(Rect decimalRect)
	{
		decimalRect.x *= Screen.width;
		decimalRect.y *= Screen.height;
		decimalRect.width *= Screen.width;
		decimalRect.height *= Screen.height;
		return decimalRect;
	}

	private bool IsMenuLaunchInputHeld_Keyboard()
	{
		return Input.GetKey(KeyCode.F12);
	}

	private bool IsMenuLaunchInputHeld_TouchScreen()
	{
		return Input.touchCount == 3;
	}

	private bool IsMenuLaunchInputHeld_AppleTV()
	{
		return Input.gyro.enabled && Input.gyro.gravity.z > 0.866f;
	}
}
