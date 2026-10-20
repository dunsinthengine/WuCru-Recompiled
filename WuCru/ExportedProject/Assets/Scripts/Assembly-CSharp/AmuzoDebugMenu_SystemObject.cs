using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class AmuzoDebugMenu_SystemObject : AmuzoDebugMenu
{
	private const string LOG_TAG = "[AmuzoDebugMenu_SystemObject] ";

	private const BindingFlags DEFAULT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private Stack<string> _pathStack;

	private Stack<Type> _typeStack;

	private Stack<WeakReference> _targetStack;

	private List<AmuzoDebugMenuButton> _myButtons;

	private StringBuilder _infoTextBuilder;

	private string _pCurrentPath
	{
		get
		{
			return (_pathStack.Count <= 0) ? string.Empty : _pathStack.Peek();
		}
	}

	private Type _pCurrentType
	{
		get
		{
			return (_typeStack.Count <= 0) ? null : _typeStack.Peek();
		}
	}

	private object _pCurrentTarget
	{
		get
		{
			return (_targetStack.Count <= 0) ? null : _targetStack.Peek().Target;
		}
	}

	public AmuzoDebugMenu_SystemObject(string menuName)
		: base(menuName)
	{
		_pathStack = new Stack<string>();
		_typeStack = new Stack<Type>();
		_targetStack = new Stack<WeakReference>();
		_myButtons = new List<AmuzoDebugMenuButton>();
		_infoTextBuilder = new StringBuilder();
		AddInfoTextFunction(GetInfoText);
	}

	public void Reset()
	{
		_pathStack.Clear();
		_typeStack.Clear();
		_targetStack.Clear();
		OnTargetChanged();
	}

	public void SetTarget(string path, Type type, object target)
	{
		if (!(_pCurrentPath == path))
		{
			_pathStack.Push(path);
			_typeStack.Push(type);
			_targetStack.Push(new WeakReference(target));
			OnTargetChanged();
		}
	}

	public void SetTargetPrevious()
	{
		if (_pathStack.Count != 0)
		{
			_pathStack.Pop();
			_typeStack.Pop();
			_targetStack.Pop();
			OnTargetChanged();
		}
	}

	private void OnTargetChanged()
	{
		RefreshMenuName();
		RefreshButtons();
		RefreshText();
	}

	private void RefreshMenuName()
	{
		if (string.IsNullOrEmpty(_pCurrentPath))
		{
			_debugMenuName = string.Empty;
			return;
		}
		_debugMenuName = _pCurrentPath;
		if (_pCurrentType != null)
		{
			_debugMenuName = _debugMenuName + " (" + _pCurrentType.Name + ")";
		}
	}

	private void RefreshButtons()
	{
		RemoveMyButtons();
		if (_pathStack.Count > 1)
		{
			AddMyButton(new AmuzoDebugMenuButton(this, "<--", () =>
			{
				SetTargetPrevious();
			}));
		}
		if (_pCurrentTarget == null)
		{
			return;
		}
		if (_pCurrentTarget.IsArrayOrList())
		{
			int arrayOrListSize = _pCurrentTarget.GetArrayOrListSize();
			for (int num = 0; num < arrayOrListSize; num++)
			{
				int capture_i = num;
				AddMyButton(new AmuzoDebugMenuButton(this, "Element " + num, () =>
				{
					SetTarget(_pCurrentPath + "[" + capture_i + "]", _pCurrentType.GetArrayOrListElementType(), _pCurrentTarget.GetArrayOrListElement(capture_i));
				}));
			}
		}
		else
		{
			if (_pCurrentType.IsPrimitive)
			{
				return;
			}
			FieldInfo[] fields = _pCurrentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int num2 = 0; num2 < fields.Length; num2++)
			{
				int capture_i2 = num2;
				AddMyButton(new AmuzoDebugMenuButton(this, fields[num2].Name, () =>
				{
					SetTarget(_pCurrentPath + "." + fields[capture_i2].Name, fields[capture_i2].FieldType, fields[capture_i2].GetValue(_pCurrentTarget));
				}));
			}
		}
	}

	private void AddMyButton(AmuzoDebugMenuButton button)
	{
		if (!_myButtons.Contains(button))
		{
			_myButtons.Add(button);
		}
		AddButton(button);
	}

	private void RemoveMyButtons()
	{
		for (int i = 0; i < _myButtons.Count; i++)
		{
			RemoveButton(_myButtons[i]);
		}
		_myButtons.Clear();
	}

	private string GetInfoText()
	{
		_infoTextBuilder.Length = 0;
		if (_pCurrentTarget != null)
		{
			_infoTextBuilder.Append(_pCurrentTarget.ToString());
		}
		else
		{
			_infoTextBuilder.Append("null");
		}
		return _infoTextBuilder.ToString();
	}
}
