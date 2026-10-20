using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInputElements
{
	public interface IScreen
	{
		GameObject _pGameObject { get; }
	}

	private const string LOG_TAG = "[ScreenInputElements] ";

	private IScreen _screen;

	private List<ScreenInputElementBase> _elements;

	public ScreenInputElements(IScreen screen)
	{
		if (screen == null)
		{
			Debug.LogError("[ScreenInputElements] screen is null");
			return;
		}
		_screen = screen;
		Initialize();
	}

	private ScreenInputElements()
	{
	}

	public void RequestDisableElements(bool wantDisabled, object context)
	{
		if (_elements == null)
		{
			return;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			if (!(_elements[i] == null))
			{
				_elements[i].RequestDisableInput(wantDisabled, context);
			}
		}
	}

	public void ForEachElement(Action<ScreenInputElementBase> action)
	{
		if (_elements == null || action == null)
		{
			return;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			if (!(_elements[i] == null))
			{
				action(_elements[i]);
			}
		}
	}

	public void ForEachElement<ElementT>(Action<ElementT> action) where ElementT : ScreenInputElementBase
	{
		if (_elements == null || action == null)
		{
			return;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			ElementT val = _elements[i] as ElementT;
			if (!(val == null))
			{
				action(val);
			}
		}
	}

	public ScreenInputElementBase FindElement(Predicate<ScreenInputElementBase> pred)
	{
		if (_elements == null)
		{
			return null;
		}
		if (pred == null)
		{
			return null;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			if (!(_elements[i] == null) && pred(_elements[i]))
			{
				return _elements[i];
			}
		}
		return null;
	}

	public ElementT FindElement<ElementT>(Predicate<ElementT> pred) where ElementT : ScreenInputElementBase
	{
		if (_elements == null)
		{
			return (ElementT)null;
		}
		if (pred == null)
		{
			return (ElementT)null;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			ElementT val = _elements[i] as ElementT;
			if (!(val == null) && pred(val))
			{
				return val;
			}
		}
		return (ElementT)null;
	}

	private void Initialize()
	{
		FindElements();
	}

	private void FindElements()
	{
		_elements = new List<ScreenInputElementBase>();
		FindButtons();
		FindGenericInputElements();
	}

	private void FindButtons()
	{
		UIButton[] componentsInChildren = _screen._pGameObject.GetComponentsInChildren<UIButton>(true);
		UIButton[] array = componentsInChildren;
		foreach (UIButton bhv in array)
		{
			ScreenButton item = bhv.EnsureComponent<ScreenButton>();
			_elements.Add(item);
		}
	}

	private void FindGenericInputElements()
	{
		UIDragObject[] componentsInChildren = _screen._pGameObject.GetComponentsInChildren<UIDragObject>(true);
		UIDragObject[] array = componentsInChildren;
		foreach (UIDragObject @do in array)
		{
			ScreenInputElementBase item = @do.EnsureComponent((ScreenGenericInputElement e) =>
			{
				e.SetCoreInputImpl(@do);
			});
			_elements.Add(item);
		}
		UIScrollView[] componentsInChildren2 = _screen._pGameObject.GetComponentsInChildren<UIScrollView>(true);
		UIScrollView[] array2 = componentsInChildren2;
		foreach (UIScrollView sv in array2)
		{
			ScreenInputElementBase item = sv.EnsureComponent((ScreenGenericInputElement e) =>
			{
				e.SetCoreInputImpl(sv);
			});
			_elements.Add(item);
		}
	}
}
