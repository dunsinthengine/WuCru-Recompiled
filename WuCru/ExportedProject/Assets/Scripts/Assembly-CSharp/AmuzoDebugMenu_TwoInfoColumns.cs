using System;
using UnityEngine;

public class AmuzoDebugMenu_TwoInfoColumns : AmuzoDebugMenu
{
	private Func<string> _secondColoumTextFunction;

	private string _secondColumnDebugInfo;

	private bool _hasDefinedSecondColoumn;

	private Rect _leftSide;

	private Rect _rightSide;

	protected Vector2 _leftScrollPosition = Vector2.zero;

	protected Rect _leftTextContentRect;

	protected Vector2 _rightScrollPosition = Vector2.zero;

	protected Rect _rightTextContentRect;

	public AmuzoDebugMenu_TwoInfoColumns(string menuName)
		: base(menuName)
	{
	}

	public void AddInfoTextFunction(Func<string> textAreaFunction, Func<string> secondTextAreaFunction, bool addTextRefreshButton = true)
	{
		_refreshTextAreaFunction = textAreaFunction;
		_secondColoumTextFunction = secondTextAreaFunction;
		if (addTextRefreshButton)
		{
			AddButton(new AmuzoDebugMenuButton(this, "Refresh Info", RefreshText));
		}
	}

	protected override void OnPreShow()
	{
		base.OnPreShow();
		_leftSide = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextDisplayArea;
		_leftSide.width *= 0.5f;
		_rightSide = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextDisplayArea;
		_rightSide.width *= 0.5f;
		_rightSide.x += _rightSide.width;
	}

	public override void RefreshText()
	{
		if (_secondColoumTextFunction != null)
		{
			_secondColumnDebugInfo = _secondColoumTextFunction();
			_hasDefinedSecondColoumn = true;
		}
		base.RefreshText();
		_leftTextContentRect = _leftSide;
		_leftTextContentRect.width -= AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._scrollBarWidth + 5f;
		_leftTextContentRect.height = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextStyle.CalcHeight(new GUIContent(_debugText), _leftTextContentRect.width);
		_rightTextContentRect = _leftSide;
		_rightTextContentRect.width -= AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._scrollBarWidth + 5f;
		_rightTextContentRect.height = AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextStyle.CalcHeight(new GUIContent(_secondColumnDebugInfo), _rightTextContentRect.width);
	}

	protected override void DrawInfoText()
	{
		if (_hasTextDefined)
		{
			_leftScrollPosition = GUI.BeginScrollView(_leftSide, _leftScrollPosition, _leftTextContentRect);
			GUI.Label(_leftTextContentRect, _debugText, AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextStyle);
			GUI.EndScrollView();
			if (_hasDefinedSecondColoumn)
			{
				_rightScrollPosition = GUI.BeginScrollView(_rightSide, _rightScrollPosition, _rightTextContentRect);
				GUI.Label(_rightTextContentRect, _secondColumnDebugInfo, AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance._infoTextStyle);
				GUI.EndScrollView();
			}
		}
	}
}
