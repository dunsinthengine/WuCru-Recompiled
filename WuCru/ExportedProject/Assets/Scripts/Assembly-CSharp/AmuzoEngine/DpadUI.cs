using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmuzoEngine
{
	public class DpadUI
	{
		public interface IScreen
		{
			int _pElementCount { get; }

			IElement _pInitialFocusElement { get; }

			int _pDepth { get; }

			bool _pIsBlocking { get; }

			bool _pIsScrollingHorizontal { get; }

			bool _pIsScrollingVertical { get; }

			IElement GetElement(int i);

			bool GetBlockingElementDepth(out int blockingElementDepth);
		}

		public interface IElement
		{
			IScreen _pScreen { get; }

			Rect _pRect { get; }

			int _pDepth { get; }

			bool _pIsVisible { get; }

			void RefreshRect();
		}

		public delegate void DOnFocusElementChanged(IElement oldFocusElement, IElement newFocusElement);

		private const float CLOSE_ANGLE_MAX_DEFAULT = 10f;

		private const float ALLOW_ANGLE_MAX_DEFAULT = 60f;

		private string _instanceId;

		private List<IScreen> _activeScreens = new List<IScreen>();

		private List<IElement> _activeElements = new List<IElement>();

		private IElement _focusElem;

		private int? _blockingScreenDepth;

		private int? _blockingElementDepth;

		private string _pLogTag
		{
			get
			{
				return "[DpadUI:" + _instanceId + "] ";
			}
		}

		public IElement _pFocusElement
		{
			get
			{
				return _focusElem;
			}
			set
			{
				SetFocusElementInt(value);
			}
		}

		public event DOnFocusElementChanged _pOnFocusElementChanged;

		public DpadUI(string name)
		{
			_instanceId = name;
		}

		public void ChangeFocus(Vector2 dir)
		{
			if (_focusElem == null)
			{
				Debug.LogError(_pLogTag + "Cannot change focus because no current focus");
				return;
			}
			IElement bestLinkedElement = GetBestLinkedElement(_focusElem, dir, 0.17453292f, (float)Math.PI / 3f);
			if (bestLinkedElement != null)
			{
				SetFocusElementInt(bestLinkedElement);
			}
		}

		public void OnShowScreen(IScreen screen)
		{
			ActivateScreen(screen);
			EnsureValidFocusElement();
		}

		public void OnShowScreenComplete(IScreen screen)
		{
			ActivateElements(screen);
			EnsureValidFocusElement(screen._pInitialFocusElement);
		}

		public void OnExitScreen(IScreen screen)
		{
			DeactivateElements(screen);
			EnsureValidFocusElement();
		}

		public void OnExitScreenComplete(IScreen screen)
		{
			DeactivateScreen(screen);
			EnsureValidFocusElement();
		}

		public void OnScreenLayoutChanged(IScreen screen, bool hasContentChanged = false)
		{
			if (hasContentChanged)
			{
				DeactivateElements(screen);
				ActivateElements(screen);
			}
			RefreshElementRects(screen);
			RefreshBlockingDepth();
			EnsureValidFocusElement();
		}

		private void SetFocusElementInt(IElement newFocusElem)
		{
			if (_focusElem != newFocusElem)
			{
				IElement focusElem = _focusElem;
				_focusElem = newFocusElem;
				if (_pOnFocusElementChanged != null)
				{
					_pOnFocusElementChanged(focusElem, newFocusElem);
				}
			}
		}

		private void RefreshElementRects(IScreen screen)
		{
			for (int i = 0; i < screen._pElementCount; i++)
			{
				screen.GetElement(i).RefreshRect();
			}
		}

		private void RefreshBlockingDepth()
		{
			_blockingScreenDepth = null;
			_blockingElementDepth = null;
			for (int i = 0; i < _activeScreens.Count; i++)
			{
				IScreen screen = _activeScreens[i];
				int blockingElementDepth;
				if (screen == null)
				{
					Debug.LogError(_pLogTag + "Expected screen to have IScreen");
				}
				else if (screen._pIsBlocking)
				{
					if (!_blockingScreenDepth.HasValue || screen._pDepth > _blockingScreenDepth.Value)
					{
						_blockingScreenDepth = screen._pDepth;
						_blockingElementDepth = null;
					}
				}
				else if ((!_blockingScreenDepth.HasValue || screen._pDepth >= _blockingScreenDepth.Value) && screen.GetBlockingElementDepth(out blockingElementDepth) && (!_blockingScreenDepth.HasValue || screen._pDepth > _blockingScreenDepth.Value || !_blockingElementDepth.HasValue || blockingElementDepth > _blockingElementDepth.Value))
				{
					_blockingScreenDepth = screen._pDepth;
					_blockingElementDepth = blockingElementDepth;
				}
			}
		}

		private void EnsureValidFocusElement(IElement initFocus = null)
		{
			if (_focusElem == null || !_activeElements.Contains(_focusElem) || !IsElementTargetable(_focusElem, null))
			{
				IElement focusElementInt = ((initFocus == null || !_activeElements.Contains(initFocus) || !IsElementTargetable(initFocus, null)) ? _activeElements.Find((IElement e) => IsElementTargetable(e, null)) : initFocus);
				SetFocusElementInt(focusElementInt);
			}
		}

		private bool IsElementTargetable(IElement elem, Rect? elemRect)
		{
			if (elem == null)
			{
				return false;
			}
			if (!elem._pIsVisible)
			{
				return false;
			}
			if (_blockingScreenDepth.HasValue && elem._pScreen != null && elem._pScreen._pDepth < _blockingScreenDepth.Value)
			{
				return false;
			}
			if (_blockingElementDepth.HasValue && _blockingScreenDepth.HasValue && elem._pScreen != null && elem._pScreen._pDepth == _blockingScreenDepth.Value && elem._pDepth < _blockingElementDepth.Value)
			{
				return false;
			}
			if (!elemRect.HasValue)
			{
				elemRect = elem._pRect;
			}
			if ((elemRect.Value.xMax < 0f || elemRect.Value.xMin > (float)Screen.width) && !elem._pScreen._pIsScrollingHorizontal)
			{
				return false;
			}
			if ((elemRect.Value.yMax < 0f || elemRect.Value.yMin > (float)Screen.height) && !elem._pScreen._pIsScrollingVertical)
			{
				return false;
			}
			return true;
		}

		private IElement GetBestLinkedElement(IElement fromElem, Vector2 inDir, float closeAngleMax, float allowAngleMax)
		{
			IElement element = null;
			float num = 0f;
			bool flag = false;
			inDir.Normalize();
			float num2 = Mathf.Cos(closeAngleMax);
			float num3 = Mathf.Cos(allowAngleMax);
			for (int i = 0; i < _activeElements.Count; i++)
			{
				IElement element2 = _activeElements[i];
				if (element2 == fromElem || !IsElementTargetable(element2, null))
				{
					continue;
				}
				Vector2 dir;
				float dist;
				GetLinkVector(fromElem._pRect, element2._pRect, out dir, out dist);
				float num4 = Vector2.Dot(dir, inDir);
				if (!(num4 < num3))
				{
					bool flag2 = num4 >= num2;
					if ((!flag || flag2) && (element == null || (flag2 && !flag) || dist < num))
					{
						element = element2;
						num = dist;
						flag = flag2;
					}
				}
			}
			return element;
		}

		private static void GetLinkVector(Rect fromRect, Rect toRect, out Vector2 dir, out float dist)
		{
			Vector2 zero = Vector2.zero;
			if (fromRect.xMax < toRect.xMin)
			{
				zero.x = toRect.xMin - fromRect.xMax;
			}
			else if (fromRect.xMin > toRect.xMax)
			{
				zero.x = toRect.xMax - fromRect.xMin;
			}
			if (fromRect.yMax < toRect.yMin)
			{
				zero.y = toRect.yMin - fromRect.yMax;
			}
			else if (fromRect.yMin > toRect.yMax)
			{
				zero.y = toRect.yMax - fromRect.yMin;
			}
			dist = zero.magnitude;
			if (dist == 0f)
			{
				dir = toRect.center - fromRect.center;
			}
			else
			{
				dir = 1f / dist * zero;
			}
		}

		private void ActivateScreen(IScreen screen)
		{
			if (!_activeScreens.Contains(screen))
			{
				_activeScreens.Add(screen);
			}
			RefreshBlockingDepth();
		}

		private void ActivateElements(IScreen screen)
		{
			for (int i = 0; i < screen._pElementCount; i++)
			{
				if (!_activeElements.Contains(screen.GetElement(i)))
				{
					_activeElements.Add(screen.GetElement(i));
				}
				screen.GetElement(i).RefreshRect();
			}
		}

		private void DeactivateElements(IScreen screen)
		{
			for (int i = 0; i < _activeElements.Count; i++)
			{
				if (_activeElements[i]._pScreen == screen)
				{
					_activeElements.RemoveAt(i--);
				}
			}
		}

		private void DeactivateScreen(IScreen screen)
		{
			int num = _activeScreens.IndexOf(screen);
			if (num >= 0)
			{
				_activeScreens.RemoveAt(num);
			}
			RefreshBlockingDepth();
		}
	}
}
