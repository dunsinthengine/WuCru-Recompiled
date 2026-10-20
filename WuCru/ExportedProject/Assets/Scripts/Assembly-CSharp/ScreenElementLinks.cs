using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class ScreenElementLinks : MonoBehaviour, DpadUI.IScreen
{
	private class Element : DpadUI.IElement
	{
		private ScreenElementLinks _screen;

		private UIWidget _widget;

		private Rect _rect;

		DpadUI.IScreen DpadUI.IElement._pScreen
		{
			get
			{
				return _screen;
			}
		}

		Rect DpadUI.IElement._pRect
		{
			get
			{
				return _rect;
			}
		}

		int DpadUI.IElement._pDepth
		{
			get
			{
				return (!(_widget != null)) ? (-1) : _widget.depth;
			}
		}

		bool DpadUI.IElement._pIsVisible
		{
			get
			{
				if (_widget == null)
				{
					return false;
				}
				if (!_widget.isVisible)
				{
					return false;
				}
				if (_widget.gameObject.transform.lossyScale.sqrMagnitude == 0f)
				{
					return false;
				}
				return true;
			}
		}

		public bool _pIsOrphan { get; set; }

		public Rect _pRect
		{
			get
			{
				return _rect;
			}
		}

		public UIWidget _pWidget
		{
			get
			{
				return _widget;
			}
		}

		public Element(ScreenElementLinks screen, UIWidget widget)
		{
			_screen = screen;
			_widget = widget;
			RefreshRect();
		}

		public void RefreshRect()
		{
			if (_widget != null)
			{
				_rect = GetWidgetRect(_widget);
			}
		}

		void DpadUI.IElement.RefreshRect()
		{
			RefreshRect();
		}
	}

	public delegate void DOnFocusElementChanged(UIWidget oldFocusElement, UIWidget newFocusElement);

	private const string LOG_TAG = "[ScreenElementLinks] ";

	[SerializeField]
	private GameObject _initialFocus;

	[SerializeField]
	private List<GameObject> _ignoreWidgets;

	[SerializeField]
	private bool _isBlocking;

	[SerializeField]
	private UIPanel _panelOverride;

	private List<Element> _elements;

	private ReadOnlyListWrapper<Element> _elementsReadOnly;

	private FullScreenCollider[] _fullScreenColliders;

	private ScreenBase _screenBase;

	private static DpadUI _dpadUI;

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

	int DpadUI.IScreen._pElementCount
	{
		get
		{
			return _elements.Count;
		}
	}

	DpadUI.IElement DpadUI.IScreen._pInitialFocusElement
	{
		get
		{
			return _elements.Find((Element e) => e._pWidget != null && e._pWidget.gameObject == _initialFocus);
		}
	}

	int DpadUI.IScreen._pDepth
	{
		get
		{
			return _screenBase._pDepth;
		}
	}

	bool DpadUI.IScreen._pIsBlocking
	{
		get
		{
			return _isBlocking;
		}
	}

	public static UIWidget _pFocusElement
	{
		get
		{
			Element element = _dpadUI._pFocusElement as Element;
			return (element == null) ? null : element._pWidget;
		}
	}

	private UIPanel _pPanel
	{
		get
		{
			return (!(_panelOverride != null)) ? GetComponent<UIPanel>() : _panelOverride;
		}
	}

	public static event DOnFocusElementChanged _pOnFocusElementChanged;

	private void Awake()
	{
		RefreshElements();
		FindFullScreenColliders();
		_dpadUI = new DpadUI("ScreenElementLinks");
		_dpadUI._pOnFocusElementChanged += (DpadUI.IElement oldFocus, DpadUI.IElement newFocus) =>
		{
			if (_pOnFocusElementChanged != null)
			{
				Element element = oldFocus as Element;
				Element element2 = newFocus as Element;
				_pOnFocusElementChanged((element == null) ? null : element._pWidget, (element2 == null) ? null : element2._pWidget);
			}
		};
		_screenBase = base.gameObject.GetComponent<ScreenBase>();
		if (_screenBase == null)
		{
			Debug.Log("[ScreenElementLinks] Requires ScreenBase component also!");
			return;
		}
		_screenBase._pOnShowScreen += () =>
		{
			_dpadUI.OnShowScreen(this);
		};
		_screenBase._pOnShowComplete += () =>
		{
			_dpadUI.OnShowScreenComplete(this);
		};
		_screenBase._pOnExitScreen += () =>
		{
			_dpadUI.OnExitScreen(this);
		};
		_screenBase._pOnExitComplete += () =>
		{
			_dpadUI.OnExitScreenComplete(this);
		};
		_screenBase._pOnLayoutChanged += () =>
		{
			_dpadUI.OnScreenLayoutChanged(this);
		};
	}

	private void Update()
	{
	}

	public void IgnoreWidget(UIWidget widget)
	{
		if (!(widget == null))
		{
			if (!_ignoreWidgets.Contains(widget.gameObject))
			{
				_ignoreWidgets.Add(widget.gameObject);
			}
			int num = _elements.FindIndex((Element e) => e._pWidget == widget);
			if (num >= 0)
			{
				_elements.RemoveAt(num);
			}
		}
	}

	public static void ChangeFocus(Vector2 dir)
	{
		_dpadUI.ChangeFocus(dir);
	}

	private void RefreshElements()
	{
		if (_elements == null)
		{
			_elements = new List<Element>();
			_elementsReadOnly = new ReadOnlyListWrapper<Element>(_elements);
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			_elements[i]._pIsOrphan = true;
		}
		UIWidget[] widgets = base.gameObject.GetComponentsInChildren<UIWidget>(true);
		for (int j = 0; j < widgets.Length; j++)
		{
			if (IsWidgetInteractive(widgets[j], _ignoreWidgets))
			{
				Element element = _elements.Find((Element e) => e._pWidget == widgets[j]);
				if (element != null)
				{
					element._pIsOrphan = false;
					continue;
				}
				element = new Element(this, widgets[j]);
				_elements.Add(element);
			}
		}
		for (int num = 0; num < _elements.Count; num++)
		{
			if (_elements[num]._pIsOrphan)
			{
				_elements.RemoveAt(num--);
			}
		}
	}

	private void FindFullScreenColliders()
	{
		_fullScreenColliders = base.gameObject.GetComponentsInChildren<FullScreenCollider>(true);
		for (int i = 0; i < _fullScreenColliders.Length; i++)
		{
			_fullScreenColliders[i].Initialize();
		}
	}

	private static bool IsWidgetInteractive(UIWidget widget, IList<GameObject> ignoreList = null)
	{
		if (widget == null)
		{
			return false;
		}
		if (ignoreList != null && ignoreList.Contains(widget.gameObject))
		{
			return false;
		}
		return widget.GetComponent<UIButton>() != null;
	}

	public static Rect GetWidgetRect(UIWidget widget)
	{
		Bounds bounds = widget.CalculateBounds();
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		min = widget.cachedTransform.localToWorldMatrix.MultiplyPoint3x4(min);
		max = widget.cachedTransform.localToWorldMatrix.MultiplyPoint3x4(max);
		min = UICamera.mainCamera.WorldToScreenPoint(min);
		max = UICamera.mainCamera.WorldToScreenPoint(max);
		return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
	}

	DpadUI.IElement DpadUI.IScreen.GetElement(int i)
	{
		return _elements[i];
	}

	bool DpadUI.IScreen.GetBlockingElementDepth(out int blockingElementDepth)
	{
		blockingElementDepth = -1;
		if (_fullScreenColliders == null)
		{
			return false;
		}
		for (int i = 0; i < _fullScreenColliders.Length; i++)
		{
			FullScreenCollider fullScreenCollider = _fullScreenColliders[i];
			if (!(fullScreenCollider == null) && fullScreenCollider._pIsActive && fullScreenCollider._pDepth > blockingElementDepth)
			{
				blockingElementDepth = fullScreenCollider._pDepth;
			}
		}
		return blockingElementDepth >= 0;
	}
}
