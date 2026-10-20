using System;
using System.Collections.Generic;
using UnityEngine;

public class AmuzoDebugMenu_SceneHierarchy : AmuzoDebugMenu
{
	private const string LOG_TAG = "[AmuzoDebugMenu_SceneHierarchy] ";

	private const string PARENT_BUTTON_LABEL = "^";

	private const string BUTTON_LABEL_ON = "[ON]";

	private const string BUTTON_LABEL_OFF = "[OFF]";

	private static List<WeakReference> _rootObjects;

	private string _menuName;

	private WeakReference _currObj;

	private List<AmuzoDebugMenuButton> _myButtons;

	private AmuzoDebugMenu_SystemObject _componentMenu;

	private Transform _pCurrObj
	{
		get
		{
			return _currObj.Target as Transform;
		}
		set
		{
			_currObj.Target = value;
		}
	}

	public AmuzoDebugMenu_SceneHierarchy(string menuName)
		: base(menuName)
	{
		_menuName = menuName;
		_currObj = new WeakReference(null);
		_myButtons = new List<AmuzoDebugMenuButton>();
		_componentMenu = new AmuzoDebugMenu_SystemObject("COMPONENT");
	}

	protected override void OnPreShow()
	{
		RefreshRootObjects();
		RefreshButtons();
		base.OnPreShow();
	}

	private static void RefreshRootObjects()
	{
		_rootObjects = new List<WeakReference>();
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		foreach (GameObject gameObject in array)
		{
			if (gameObject.transform.parent == null)
			{
				_rootObjects.Add(new WeakReference(gameObject, false));
			}
		}
		_rootObjects.Sort((WeakReference obj1, WeakReference obj2) => string.Compare((obj1.Target as GameObject).name, (obj2.Target as GameObject).name, true));
	}

	private void RefreshMenuName()
	{
		_debugMenuName = _menuName;
		if (_pCurrObj != null)
		{
			_debugMenuName = _debugMenuName + ": " + _pCurrObj.gameObject.GetFullName();
		}
	}

	private void RefreshButtons()
	{
		RemoveMyButtons();
		if (_pCurrObj != null)
		{
			AddMyButton(new AmuzoDebugMenuButton(this, () => GetActiveStateButtonLabel(_pCurrObj.gameObject), () =>
			{
				_pCurrObj.gameObject.SetActive(!_pCurrObj.gameObject.activeSelf);
			}));
			AddMyButton(new AmuzoDebugMenuButton(this, "^", () =>
			{
				SetCurrentObject(_pCurrObj.parent);
			}));
			for (int num = 0; num < _pCurrObj.childCount; num++)
			{
				AddObjectButton(_pCurrObj.GetChild(num));
			}
			Component[] components = _pCurrObj.GetComponents<Component>();
			for (int num2 = 0; num2 < components.Length; num2++)
			{
				AddComponentButton(components[num2]);
			}
			return;
		}
		for (int num3 = 0; num3 < _rootObjects.Count; num3++)
		{
			if (_rootObjects[num3].IsAlive)
			{
				AddObjectButton((_rootObjects[num3].Target as GameObject).transform);
			}
		}
	}

	private void AddObjectButton(Transform obj)
	{
		AddMyButton(new AmuzoDebugMenuButton(this, obj.gameObject.name, () =>
		{
			SetCurrentObject(obj);
		}));
	}

	private void AddComponentButton(Component comp)
	{
		AddMyButton(new AmuzoDebugMenuButton(this, "# " + comp.GetType().Name, () =>
		{
			ViewComponent(comp);
		}));
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

	private void SetCurrentObject(Transform obj)
	{
		if (!(_pCurrObj == obj))
		{
			_pCurrObj = obj;
			OnCurrentObjectChanged();
		}
	}

	private void OnCurrentObjectChanged()
	{
		RefreshMenuName();
		RefreshButtons();
	}

	private void ViewComponent(Component comp)
	{
		_componentMenu.Reset();
		_componentMenu.SetTarget(_pCurrObj.gameObject.GetFullName() + "/" + comp.GetType().Name, comp.GetType(), comp);
		AmuzoMonoSingleton<AmuzoDebugMenuManager>._pInstance.NavigateTo(_componentMenu);
	}

	private static string GetActiveStateButtonLabel(GameObject obj)
	{
		return (!(obj != null) || !obj.activeSelf) ? "[OFF]" : "[ON]";
	}
}
