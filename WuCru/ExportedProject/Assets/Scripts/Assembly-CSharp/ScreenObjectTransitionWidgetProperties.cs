using System;
using AmuzoEngine;
using UnityEngine;

[Serializable]
public class ScreenObjectTransitionWidgetProperties
{
	public string setName = "Default";

	public bool autoTweenInOnScreenTransition = true;

	public bool autoTweenOutOnScreenTransition = true;

	public bool setFullAlphaBeforeTweenIn = true;

	public bool setZeroAlphaAfterTweenOut = true;

	public bool adjustsPositionIn;

	public bool adjustsPositionOut;

	public Vector3 onScreenLocalPosStart;

	public Vector3 onScreenLocalPosEnd;

	public Vector3 offScreenLocalPosStart;

	public Vector3 offScreenLocalPosEnd;

	public Transform onScreenLocalTransformStart;

	public Transform onScreenLocalTransformEnd;

	public Transform offScreenLocalTransformStart;

	public Transform offScreenLocalTransformEnd;

	public AmuzoEngine.Easing.EaseType onPositionEasing;

	public AmuzoEngine.Easing.EaseType offPositionEasing;

	public float positionInDelay;

	public float positionInDuration = 0.5f;

	public float positionOutDelay;

	public float positionOutDuration = 0.5f;

	public bool adjustsColorIn;

	public bool adjustsColorOut;

	public Color onColorStart = new Color(1f, 1f, 1f, 0f);

	public Color onColorEnd = new Color(1f, 1f, 1f, 1f);

	public Color offColorStart = new Color(1f, 1f, 1f, 1f);

	public Color offColorEnd = new Color(1f, 1f, 1f, 0f);

	public AmuzoEngine.Easing.EaseType onColorEasing;

	public AmuzoEngine.Easing.EaseType offColorEasing;

	public float colorInDelay;

	public float colorInDuration = 0.5f;

	public float colorOutDelay;

	public float colorOutDuration = 0.5f;

	public bool adjustsScaleIn;

	public bool adjustsScaleOut;

	public Vector3 onScreenLocalScaleStart;

	public Vector3 onScreenLocalScaleEnd;

	public Vector3 offScreenLocalScaleStart;

	public Vector3 offScreenLocalScaleEnd;

	public AmuzoEngine.Easing.EaseType onScaleEasing;

	public AmuzoEngine.Easing.EaseType offScaleEasing;

	public float scaleInDelay;

	public float scaleInDuration = 0.5f;

	public float scaleOutDelay;

	public float scaleOutDuration = 0.5f;

	private float _tweenPosStartTime;

	private float _tweenColStartTime;

	private float _tweenScaleStartTime;

	private float _additionalDelayPosIn;

	private float _additionalDelayPosOut;

	private float _additionalDelayColorIn;

	private float _additionalDelayColorOut;

	private float _additionalDelayScaleIn;

	private float _additionalDelayScaleOut;

	public float _pTweenPosStartTime
	{
		get
		{
			return _tweenPosStartTime;
		}
		set
		{
			_tweenPosStartTime = value;
		}
	}

	public float _pTweenColStartTime
	{
		get
		{
			return _tweenColStartTime;
		}
		set
		{
			_tweenColStartTime = value;
		}
	}

	public float _pTweenScaleStartTime
	{
		get
		{
			return _tweenScaleStartTime;
		}
		set
		{
			_tweenScaleStartTime = value;
		}
	}

	public float _pAdditionalDelayPosIn
	{
		get
		{
			return _additionalDelayPosIn;
		}
		set
		{
			_additionalDelayPosIn = value;
		}
	}

	public float _pAdditionalDelayPosOut
	{
		get
		{
			return _additionalDelayPosOut;
		}
		set
		{
			_additionalDelayPosOut = value;
		}
	}

	public float _pAdditionalDelayColorIn
	{
		get
		{
			return _additionalDelayColorIn;
		}
		set
		{
			_additionalDelayColorIn = value;
		}
	}

	public float _pAdditionalDelayColorOut
	{
		get
		{
			return _additionalDelayColorOut;
		}
		set
		{
			_additionalDelayColorOut = value;
		}
	}

	public float _pAdditionalDelayScaleIn
	{
		get
		{
			return _additionalDelayScaleIn;
		}
		set
		{
			_additionalDelayScaleIn = value;
		}
	}

	public float _pAdditionalDelayScaleOut
	{
		get
		{
			return _additionalDelayScaleOut;
		}
		set
		{
			_additionalDelayScaleOut = value;
		}
	}
}
