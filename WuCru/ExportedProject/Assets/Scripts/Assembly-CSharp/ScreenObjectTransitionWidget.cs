using System;
using AmuzoEngine;
using UnityEngine;

public class ScreenObjectTransitionWidget : MonoBehaviour
{
	public enum TweenType
	{
		IDLE = 0,
		TWEEN_IN = 1,
		TWEEN_OUT = 2
	}

	public ScreenObjectTransitionWidgetProperties[] propertySets;

	private UIWidget _widgetRef;

	private TweenType _tweenType;

	private bool _consideredTweenedIn;

	private float _tweenPosStartTime;

	private float _tweenColStartTime;

	private float _tweenScaleStartTime;

	private int _currentPropertySetIndex = -1;

	public UIWidget _pWidgetRef
	{
		get
		{
			if (_widgetRef == null)
			{
				_widgetRef = GetComponentInChildren<UIWidget>();
			}
			return _widgetRef;
		}
	}

	public TweenType _pTweenType
	{
		get
		{
			return _tweenType;
		}
	}

	public bool _pConsideredTweenedIn
	{
		get
		{
			return _consideredTweenedIn;
		}
	}

	public ScreenObjectTransitionWidgetProperties _pCurrentPropertySet
	{
		get
		{
			EnsureCurrentPropertySet();
			if (_currentPropertySetIndex < 0)
			{
				return null;
			}
			return propertySets[_currentPropertySetIndex];
		}
	}

	public event Action _pOnNextTweenComplete;

	private void Awake()
	{
		EnsureCurrentPropertySet();
	}

	private void Update()
	{
		if (_pTweenType == TweenType.IDLE || _pWidgetRef == null)
		{
			return;
		}
		if (_pTweenType == TweenType.TWEEN_OUT)
		{
			bool flag = AdjustColorOut();
			flag &= AdjustPositionOut();
			if (flag & AdjustScaleOut())
			{
				if (_pCurrentPropertySet.setZeroAlphaAfterTweenOut)
				{
					_pWidgetRef.alpha = 0f;
				}
				OnTweenComplete();
			}
		}
		else if (_pTweenType == TweenType.TWEEN_IN)
		{
			bool flag2 = AdjustColorIn();
			flag2 &= AdjustPositionIn();
			if (flag2 & AdjustScaleIn())
			{
				OnTweenComplete();
			}
		}
	}

	public void TryChangeSet(string setName)
	{
		if (propertySets == null)
		{
			return;
		}
		int num = propertySets.Length;
		for (int i = 0; i < num; i++)
		{
			if (propertySets[i].setName == setName)
			{
				_currentPropertySetIndex = i;
				break;
			}
		}
	}

	public void SetAllAdditionalDelayPosIn(float delayPosIn)
	{
		if (propertySets != null)
		{
			int num = propertySets.Length;
			for (int i = 0; i < num; i++)
			{
				propertySets[i]._pAdditionalDelayPosIn = delayPosIn;
			}
		}
	}

	public void SetAllAdditionalDelayPosOut(float delayPosOut)
	{
		if (propertySets != null)
		{
			int num = propertySets.Length;
			for (int i = 0; i < num; i++)
			{
				propertySets[i]._pAdditionalDelayPosOut = delayPosOut;
			}
		}
	}

	public void SetAllAdditionalDelayColorIn(float delayColorIn)
	{
		if (propertySets != null)
		{
			int num = propertySets.Length;
			for (int i = 0; i < num; i++)
			{
				propertySets[i]._pAdditionalDelayColorIn = delayColorIn;
			}
		}
	}

	public void SetAllAdditionalDelayColorOut(float delayColorOut)
	{
		if (propertySets != null)
		{
			int num = propertySets.Length;
			for (int i = 0; i < num; i++)
			{
				propertySets[i]._pAdditionalDelayColorOut = delayColorOut;
			}
		}
	}

	public void SetAllAdditionalDelayScaleIn(float delayScaleIn)
	{
		if (propertySets != null)
		{
			int num = propertySets.Length;
			for (int i = 0; i < num; i++)
			{
				propertySets[i]._pAdditionalDelayScaleIn = delayScaleIn;
			}
		}
	}

	public void SetAllAdditionalDelayScaleOut(float delayScaleOut)
	{
		if (propertySets != null)
		{
			int num = propertySets.Length;
			for (int i = 0; i < num; i++)
			{
				propertySets[i]._pAdditionalDelayScaleOut = delayScaleOut;
			}
		}
	}

	public void TweenOut(bool isFromScreenTransition = false, Action onBegin = null, Action onComplete = null)
	{
		if (!isFromScreenTransition || _pCurrentPropertySet.autoTweenOutOnScreenTransition)
		{
			CheckEndTween();
			if (onBegin != null)
			{
				onBegin();
			}
			_tweenType = TweenType.TWEEN_OUT;
			_tweenPosStartTime = RealTime.time + _pCurrentPropertySet.positionOutDelay + _pCurrentPropertySet._pAdditionalDelayPosOut;
			_tweenColStartTime = RealTime.time + _pCurrentPropertySet.colorOutDelay + _pCurrentPropertySet._pAdditionalDelayColorOut;
			_tweenScaleStartTime = RealTime.time + _pCurrentPropertySet.scaleOutDelay + _pCurrentPropertySet._pAdditionalDelayScaleOut;
			_consideredTweenedIn = false;
			if (onComplete != null)
			{
				_pOnNextTweenComplete += onComplete;
			}
			Update();
		}
	}

	public void SetToTweenInStartColor()
	{
		_pWidgetRef.color = _pCurrentPropertySet.onColorStart;
	}

	public void SetToTweenInStartPos()
	{
		base.transform.localPosition = _pCurrentPropertySet.onScreenLocalPosStart;
		if (_pCurrentPropertySet.onScreenLocalTransformStart != null)
		{
			base.transform.localPosition = _pCurrentPropertySet.onScreenLocalTransformStart.localPosition;
		}
	}

	public void SetToTweenInStartScale()
	{
		base.transform.localScale = _pCurrentPropertySet.onScreenLocalScaleStart;
	}

	public void SetStateConsideredTweenedIn(bool consideredTweenedIn)
	{
		_consideredTweenedIn = consideredTweenedIn;
		CheckEndTween();
	}

	public void TweenIn(bool isFromScreenTransition = false, Action onBegin = null, Action onComplete = null)
	{
		if (!isFromScreenTransition || _pCurrentPropertySet.autoTweenInOnScreenTransition)
		{
			CheckEndTween();
			if (onBegin != null)
			{
				onBegin();
			}
			_tweenType = TweenType.TWEEN_IN;
			_tweenPosStartTime = RealTime.time + _pCurrentPropertySet.positionInDelay + _pCurrentPropertySet._pAdditionalDelayPosIn;
			_tweenColStartTime = RealTime.time + _pCurrentPropertySet.colorInDelay + _pCurrentPropertySet._pAdditionalDelayColorIn;
			_tweenScaleStartTime = RealTime.time + _pCurrentPropertySet.scaleInDelay + _pCurrentPropertySet._pAdditionalDelayScaleIn;
			_consideredTweenedIn = true;
			if (_pCurrentPropertySet.adjustsColorIn)
			{
				_pWidgetRef.color = _pCurrentPropertySet.onColorStart;
			}
			if (_pCurrentPropertySet.adjustsPositionIn)
			{
				_pWidgetRef.transform.localPosition = _pCurrentPropertySet.onScreenLocalPosStart;
			}
			if (_pCurrentPropertySet.adjustsScaleIn)
			{
				_pWidgetRef.transform.localScale = _pCurrentPropertySet.onScreenLocalScaleStart;
			}
			if (_pCurrentPropertySet.setFullAlphaBeforeTweenIn && _pWidgetRef != null)
			{
				_pWidgetRef.alpha = 1f;
			}
			if (onComplete != null)
			{
				_pOnNextTweenComplete += onComplete;
			}
			Update();
		}
	}

	private void EnsureCurrentPropertySet()
	{
		if (_currentPropertySetIndex == -1)
		{
			if (propertySets != null && propertySets.Length > 0)
			{
				_currentPropertySetIndex = 0;
			}
			if (_currentPropertySetIndex == -1)
			{
				Debug.LogWarning("Could not ensure a widget property set on: " + base.name, base.gameObject);
			}
		}
	}

	private bool AdjustColorIn()
	{
		if (_pCurrentPropertySet.adjustsColorIn)
		{
			float num = RealTime.time - _tweenColStartTime;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num <= _pCurrentPropertySet.colorInDuration)
			{
				_pWidgetRef.color = AmuzoEngine.Easing.Ease(_pCurrentPropertySet.onColorEasing, num, _pCurrentPropertySet.colorInDuration, _pCurrentPropertySet.onColorStart, _pCurrentPropertySet.onColorEnd);
				return false;
			}
			_pWidgetRef.color = _pCurrentPropertySet.onColorEnd;
			return true;
		}
		return true;
	}

	private bool AdjustColorOut()
	{
		if (_pCurrentPropertySet.adjustsColorOut)
		{
			float num = RealTime.time - _tweenColStartTime;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num <= _pCurrentPropertySet.colorOutDuration)
			{
				_pWidgetRef.color = AmuzoEngine.Easing.Ease(_pCurrentPropertySet.offColorEasing, num, _pCurrentPropertySet.colorOutDuration, _pCurrentPropertySet.offColorStart, _pCurrentPropertySet.offColorEnd);
				return false;
			}
			_pWidgetRef.color = _pCurrentPropertySet.offColorEnd;
			return true;
		}
		return true;
	}

	private bool AdjustScaleIn()
	{
		if (_pCurrentPropertySet.adjustsScaleIn)
		{
			float num = RealTime.time - _tweenScaleStartTime;
			if (num < 0f)
			{
				num = 0f;
			}
			Vector3 onScreenLocalScaleEnd = _pCurrentPropertySet.onScreenLocalScaleEnd;
			Vector3 onScreenLocalScaleStart = _pCurrentPropertySet.onScreenLocalScaleStart;
			if (num <= _pCurrentPropertySet.scaleInDuration)
			{
				if (onScreenLocalScaleStart == onScreenLocalScaleEnd)
				{
					base.transform.localScale = onScreenLocalScaleEnd;
				}
				else
				{
					base.transform.localScale = AmuzoEngine.Easing.Ease(_pCurrentPropertySet.onScaleEasing, num, _pCurrentPropertySet.scaleInDuration, onScreenLocalScaleStart, onScreenLocalScaleEnd);
				}
				return false;
			}
			base.transform.localScale = onScreenLocalScaleEnd;
			return true;
		}
		return true;
	}

	private bool AdjustScaleOut()
	{
		if (_pCurrentPropertySet.adjustsScaleOut)
		{
			float num = RealTime.time - _tweenScaleStartTime;
			if (num < 0f)
			{
				num = 0f;
			}
			Vector3 offScreenLocalScaleStart = _pCurrentPropertySet.offScreenLocalScaleStart;
			Vector3 offScreenLocalScaleEnd = _pCurrentPropertySet.offScreenLocalScaleEnd;
			if (num <= _pCurrentPropertySet.scaleOutDuration)
			{
				base.transform.localScale = AmuzoEngine.Easing.Ease(_pCurrentPropertySet.offScaleEasing, num, _pCurrentPropertySet.scaleOutDuration, offScreenLocalScaleStart, offScreenLocalScaleEnd);
				return false;
			}
			base.transform.localScale = offScreenLocalScaleEnd;
			return true;
		}
		return true;
	}

	private bool AdjustPositionIn()
	{
		if (_pCurrentPropertySet.adjustsPositionIn)
		{
			float num = RealTime.time - _tweenPosStartTime;
			if (num < 0f)
			{
				num = 0f;
			}
			Vector3 vector = _pCurrentPropertySet.onScreenLocalPosEnd;
			Vector3 start = _pCurrentPropertySet.onScreenLocalPosStart;
			if (_pCurrentPropertySet.onScreenLocalTransformStart != null)
			{
				start = _pCurrentPropertySet.onScreenLocalTransformStart.localPosition;
			}
			if (_pCurrentPropertySet.onScreenLocalTransformEnd != null)
			{
				vector = _pCurrentPropertySet.onScreenLocalTransformEnd.localPosition;
			}
			if (num <= _pCurrentPropertySet.positionInDuration)
			{
				base.transform.localPosition = AmuzoEngine.Easing.Ease(_pCurrentPropertySet.onPositionEasing, num, _pCurrentPropertySet.positionInDuration, start, vector);
				return false;
			}
			base.transform.localPosition = vector;
			return true;
		}
		return true;
	}

	private bool AdjustPositionOut()
	{
		if (_pCurrentPropertySet.adjustsPositionOut)
		{
			float num = RealTime.time - _tweenPosStartTime;
			if (num < 0f)
			{
				num = 0f;
			}
			Vector3 start = _pCurrentPropertySet.offScreenLocalPosStart;
			Vector3 vector = _pCurrentPropertySet.offScreenLocalPosEnd;
			if (_pCurrentPropertySet.offScreenLocalTransformEnd != null)
			{
				vector = _pCurrentPropertySet.offScreenLocalTransformEnd.localPosition;
			}
			if (_pCurrentPropertySet.offScreenLocalTransformStart != null)
			{
				start = _pCurrentPropertySet.offScreenLocalTransformStart.localPosition;
			}
			if (num <= _pCurrentPropertySet.positionOutDuration)
			{
				base.transform.localPosition = AmuzoEngine.Easing.Ease(_pCurrentPropertySet.offPositionEasing, num, _pCurrentPropertySet.positionOutDuration, start, vector);
				return false;
			}
			base.transform.localPosition = vector;
			return true;
		}
		return true;
	}

	public void ForceEndTween()
	{
		if (_tweenType == TweenType.TWEEN_IN)
		{
			if (_pCurrentPropertySet.adjustsColorIn)
			{
				_tweenColStartTime = RealTime.time - _pCurrentPropertySet.colorInDuration - 0.1f;
			}
			if (_pCurrentPropertySet.adjustsScaleIn)
			{
				_tweenScaleStartTime = RealTime.time - _pCurrentPropertySet.scaleInDuration - 0.1f;
			}
			if (_pCurrentPropertySet.adjustsPositionIn)
			{
				_tweenPosStartTime = RealTime.time - _pCurrentPropertySet.positionInDuration - 0.1f;
			}
		}
		if (_tweenType == TweenType.TWEEN_OUT)
		{
			if (_pCurrentPropertySet.adjustsColorOut)
			{
				_tweenColStartTime = RealTime.time - _pCurrentPropertySet.colorOutDuration - 0.1f;
			}
			if (_pCurrentPropertySet.adjustsScaleOut)
			{
				_tweenScaleStartTime = RealTime.time - _pCurrentPropertySet.scaleOutDuration - 0.1f;
			}
			if (_pCurrentPropertySet.adjustsPositionOut)
			{
				_tweenPosStartTime = RealTime.time - _pCurrentPropertySet.positionOutDuration - 0.1f;
			}
		}
		Update();
	}

	private void CheckEndTween()
	{
		if (_tweenType != TweenType.IDLE)
		{
			OnTweenComplete();
		}
	}

	private void OnTweenComplete()
	{
		_tweenType = TweenType.IDLE;
		if (_pOnNextTweenComplete != null)
		{
			Action pOnNextTweenComplete = _pOnNextTweenComplete;
			_pOnNextTweenComplete = null;
			pOnNextTweenComplete();
			pOnNextTweenComplete = null;
		}
	}
}
