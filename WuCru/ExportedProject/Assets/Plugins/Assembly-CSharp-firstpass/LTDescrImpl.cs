using System;
using UnityEngine;
using UnityEngine.UI;

public class LTDescrImpl : LTDescr
{
	private uint _id;

	internal Vector3 fromInternal;

	internal Vector3 toInternal;

	internal Vector3 diffInternal;

	public RectTransform rectTransform;

	public Text uiText;

	public Image uiImage;

	public Sprite[] sprites;

	private static uint global_counter;

	public bool toggle { get; set; }

	public bool useEstimatedTime { get; set; }

	public bool useFrames { get; set; }

	public bool useManualTime { get; set; }

	public bool hasInitiliazed { get; set; }

	public bool hasPhysics { get; set; }

	public bool onCompleteOnRepeat { get; set; }

	public bool onCompleteOnStart { get; set; }

	public bool useRecursion { get; set; }

	public float passed { get; set; }

	public float delay { get; set; }

	public float time { get; set; }

	public float speed { get; set; }

	public float lastVal { get; set; }

	public int loopCount { get; set; }

	public uint counter { get; set; }

	public float direction { get; set; }

	public float directionLast { get; set; }

	public float overshoot { get; set; }

	public float period { get; set; }

	public bool destroyOnComplete { get; set; }

	public Transform trans { get; set; }

	public Transform toTrans { get; set; }

	public LTRect ltRect { get; set; }

	public Vector3 from
	{
		get
		{
			return fromInternal;
		}
		set
		{
			fromInternal = value;
		}
	}

	public Vector3 to
	{
		get
		{
			return toInternal;
		}
		set
		{
			toInternal = value;
		}
	}

	public Vector3 diff
	{
		get
		{
			return diffInternal;
		}
		set
		{
			diffInternal = value;
		}
	}

	public Vector3 point { get; set; }

	public Vector3 axis { get; set; }

	public Quaternion origRotation { get; set; }

	public LTBezierPath path { get; set; }

	public LTSpline spline { get; set; }

	public TweenAction type { get; set; }

	public LeanTweenType tweenType { get; set; }

	public AnimationCurve animationCurve { get; set; }

	public LeanTweenType loopType { get; set; }

	public bool hasUpdateCallback { get; set; }

	public Action<float> onUpdateFloat { get; set; }

	public Action<float, float> onUpdateFloatRatio { get; set; }

	public Action<float, object> onUpdateFloatObject { get; set; }

	public Action<Vector2> onUpdateVector2 { get; set; }

	public Action<Vector3> onUpdateVector3 { get; set; }

	public Action<Vector3, object> onUpdateVector3Object { get; set; }

	public Action<Color> onUpdateColor { get; set; }

	public Action<Color, object> onUpdateColorObject { get; set; }

	public Action onComplete { get; set; }

	public Action<object> onCompleteObject { get; set; }

	public object onCompleteParam { get; set; }

	public object onUpdateParam { get; set; }

	public Action onStart { get; set; }

	public SpriteRenderer spriteRen { get; set; }

	public int uniqueId
	{
		get
		{
			return (int)(_id | (counter << 16));
		}
	}

	public int id
	{
		get
		{
			return uniqueId;
		}
	}

	public override string ToString()
	{
		return string.Concat((!(trans != null)) ? "gameObject:null" : ("gameObject:" + trans.gameObject), " toggle:", toggle, " passed:", passed, " time:", time, " delay:", delay, " direction:", direction, " from:", from, " to:", to, " type:", type, " ease:", tweenType, " useEstimatedTime:", useEstimatedTime, " id:", id, " hasInitiliazed:", hasInitiliazed);
	}

	[Obsolete("Use 'LeanTween.cancel( id )' instead")]
	public LTDescr cancel(GameObject gameObject)
	{
		if (gameObject == trans.gameObject)
		{
			LeanTween.removeTween((int)_id, uniqueId);
		}
		return this;
	}

	public void reset()
	{
		bool flag = (useRecursion = true);
		toggle = flag;
		trans = null;
		float num = (lastVal = 0f);
		num = (delay = num);
		passed = num;
		flag = (useManualTime = false);
		flag = (onCompleteOnStart = flag);
		flag = (destroyOnComplete = flag);
		flag = (onCompleteOnRepeat = flag);
		flag = (hasInitiliazed = flag);
		flag = (useFrames = flag);
		flag = (useEstimatedTime = flag);
		hasUpdateCallback = flag;
		animationCurve = null;
		tweenType = LeanTweenType.linear;
		loopType = LeanTweenType.once;
		loopCount = 0;
		num = (overshoot = 1f);
		num = (directionLast = num);
		direction = num;
		period = 0.3f;
		speed = -1f;
		point = Vector3.zero;
		cleanup();
		global_counter++;
		if (global_counter > 32768)
		{
			global_counter = 0u;
		}
	}

	public void cleanup()
	{
		onUpdateFloat = null;
		onUpdateFloatRatio = null;
		onUpdateVector2 = null;
		onUpdateVector3 = null;
		onUpdateFloatObject = null;
		onUpdateVector3Object = null;
		onUpdateColor = null;
		onComplete = null;
		onCompleteObject = null;
		onCompleteParam = null;
		onStart = null;
		rectTransform = null;
		uiText = null;
		uiImage = null;
		sprites = null;
	}

	public void init()
	{
		hasInitiliazed = true;
		if (onStart != null)
		{
			onStart();
		}
		switch (type)
		{
		case TweenAction.MOVE:
		case TweenAction.MOVE_TO_TRANSFORM:
			from = trans.position;
			break;
		case TweenAction.MOVE_X:
			fromInternal.x = trans.position.x;
			break;
		case TweenAction.MOVE_Y:
			fromInternal.x = trans.position.y;
			break;
		case TweenAction.MOVE_Z:
			fromInternal.x = trans.position.z;
			break;
		case TweenAction.MOVE_LOCAL_X:
			fromInternal.x = trans.localPosition.x;
			break;
		case TweenAction.MOVE_LOCAL_Y:
			fromInternal.x = trans.localPosition.y;
			break;
		case TweenAction.MOVE_LOCAL_Z:
			fromInternal.x = trans.localPosition.z;
			break;
		case TweenAction.SCALE_X:
			fromInternal.x = trans.localScale.x;
			break;
		case TweenAction.SCALE_Y:
			fromInternal.x = trans.localScale.y;
			break;
		case TweenAction.SCALE_Z:
			fromInternal.x = trans.localScale.z;
			break;
		case TweenAction.ALPHA:
		{
			SpriteRenderer component2 = trans.gameObject.GetComponent<SpriteRenderer>();
			if (component2 != null)
			{
				fromInternal.x = component2.color.a;
			}
			else if (trans.gameObject.GetComponent<Renderer>() != null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_Color"))
			{
				fromInternal.x = trans.gameObject.GetComponent<Renderer>().material.color.a;
			}
			else if (trans.gameObject.GetComponent<Renderer>() != null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				Color color5 = trans.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
				fromInternal.x = color5.a;
			}
			else
			{
				if (trans.childCount <= 0)
				{
					break;
				}
				foreach (Transform tran in trans)
				{
					if (tran.gameObject.GetComponent<Renderer>() != null)
					{
						Color color6 = tran.gameObject.GetComponent<Renderer>().material.color;
						fromInternal.x = color6.a;
						break;
					}
				}
			}
			break;
		}
		case TweenAction.MOVE_LOCAL:
			from = trans.localPosition;
			break;
		case TweenAction.MOVE_CURVED:
		case TweenAction.MOVE_CURVED_LOCAL:
		case TweenAction.MOVE_SPLINE:
		case TweenAction.MOVE_SPLINE_LOCAL:
			fromInternal.x = 0f;
			break;
		case TweenAction.ROTATE:
			from = trans.eulerAngles;
			to = new Vector3(LeanTween.closestRot(fromInternal.x, toInternal.x), LeanTween.closestRot(from.y, to.y), LeanTween.closestRot(from.z, to.z));
			break;
		case TweenAction.ROTATE_X:
			fromInternal.x = trans.eulerAngles.x;
			toInternal.x = LeanTween.closestRot(fromInternal.x, toInternal.x);
			break;
		case TweenAction.ROTATE_Y:
			fromInternal.x = trans.eulerAngles.y;
			toInternal.x = LeanTween.closestRot(fromInternal.x, toInternal.x);
			break;
		case TweenAction.ROTATE_Z:
			fromInternal.x = trans.eulerAngles.z;
			toInternal.x = LeanTween.closestRot(fromInternal.x, toInternal.x);
			break;
		case TweenAction.ROTATE_AROUND:
			lastVal = 0f;
			fromInternal.x = 0f;
			origRotation = trans.rotation;
			break;
		case TweenAction.ROTATE_AROUND_LOCAL:
			lastVal = 0f;
			fromInternal.x = 0f;
			origRotation = trans.localRotation;
			break;
		case TweenAction.ROTATE_LOCAL:
			from = trans.localEulerAngles;
			to = new Vector3(LeanTween.closestRot(fromInternal.x, toInternal.x), LeanTween.closestRot(from.y, to.y), LeanTween.closestRot(from.z, to.z));
			break;
		case TweenAction.SCALE:
			from = trans.localScale;
			break;
		case TweenAction.GUI_MOVE:
			from = new Vector3(ltRect.rect.x, ltRect.rect.y, 0f);
			break;
		case TweenAction.GUI_MOVE_MARGIN:
			from = new Vector2(ltRect.margin.x, ltRect.margin.y);
			break;
		case TweenAction.GUI_SCALE:
			from = new Vector3(ltRect.rect.width, ltRect.rect.height, 0f);
			break;
		case TweenAction.GUI_ALPHA:
			fromInternal.x = ltRect.alpha;
			break;
		case TweenAction.GUI_ROTATE:
			if (!ltRect.rotateEnabled)
			{
				ltRect.rotateEnabled = true;
				ltRect.resetForRotation();
			}
			fromInternal.x = ltRect.rotation;
			break;
		case TweenAction.ALPHA_VERTEX:
			fromInternal.x = (int)trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
			break;
		case TweenAction.CALLBACK_COLOR:
			diff = new Vector3(1f, 0f, 0f);
			break;
		case TweenAction.COLOR:
		{
			SpriteRenderer component = trans.gameObject.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				Color color = component.color;
				setFromColor(color);
			}
			else if (trans.gameObject.GetComponent<Renderer>() != null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_Color"))
			{
				Color color2 = trans.gameObject.GetComponent<Renderer>().material.color;
				setFromColor(color2);
			}
			else if (trans.gameObject.GetComponent<Renderer>() != null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				Color color3 = trans.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
				setFromColor(color3);
			}
			else
			{
				if (trans.childCount <= 0)
				{
					break;
				}
				foreach (Transform tran2 in trans)
				{
					if (tran2.gameObject.GetComponent<Renderer>() != null)
					{
						Color color4 = tran2.gameObject.GetComponent<Renderer>().material.color;
						setFromColor(color4);
						break;
					}
				}
			}
			break;
		}
		case TweenAction.CANVAS_ALPHA:
			uiImage = trans.gameObject.GetComponent<Image>();
			if (uiImage != null)
			{
				fromInternal.x = uiImage.color.a;
			}
			else
			{
				fromInternal.x = 1f;
			}
			break;
		case TweenAction.CANVAS_COLOR:
			uiImage = trans.gameObject.GetComponent<Image>();
			if (uiImage != null)
			{
				setFromColor(uiImage.color);
			}
			else
			{
				setFromColor(Color.white);
			}
			break;
		case TweenAction.CANVASGROUP_ALPHA:
			fromInternal.x = trans.gameObject.GetComponent<CanvasGroup>().alpha;
			break;
		case TweenAction.TEXT_ALPHA:
			uiText = trans.gameObject.GetComponent<Text>();
			if (uiText != null)
			{
				fromInternal.x = uiText.color.a;
			}
			else
			{
				fromInternal.x = 1f;
			}
			break;
		case TweenAction.TEXT_COLOR:
			uiText = trans.gameObject.GetComponent<Text>();
			if (uiText != null)
			{
				setFromColor(uiText.color);
			}
			else
			{
				setFromColor(Color.white);
			}
			break;
		case TweenAction.CANVAS_MOVE:
			fromInternal = rectTransform.anchoredPosition3D;
			break;
		case TweenAction.CANVAS_MOVE_X:
			fromInternal.x = rectTransform.anchoredPosition3D.x;
			break;
		case TweenAction.CANVAS_MOVE_Y:
			fromInternal.x = rectTransform.anchoredPosition3D.y;
			break;
		case TweenAction.CANVAS_MOVE_Z:
			fromInternal.x = rectTransform.anchoredPosition3D.z;
			break;
		case TweenAction.CANVAS_ROTATEAROUND:
		case TweenAction.CANVAS_ROTATEAROUND_LOCAL:
			lastVal = 0f;
			fromInternal.x = 0f;
			origRotation = rectTransform.rotation;
			break;
		case TweenAction.CANVAS_SCALE:
			from = rectTransform.localScale;
			break;
		case TweenAction.CANVAS_PLAYSPRITE:
			uiImage = trans.gameObject.GetComponent<Image>();
			fromInternal.x = 0f;
			break;
		}
		if (type != TweenAction.CALLBACK_COLOR && type != TweenAction.COLOR && type != TweenAction.TEXT_COLOR && type != TweenAction.CANVAS_COLOR)
		{
			diff = to - from;
		}
		if (onCompleteOnStart)
		{
			if (onComplete != null)
			{
				onComplete();
			}
			else if (onCompleteObject != null)
			{
				onCompleteObject(onCompleteParam);
			}
		}
		if (speed >= 0f)
		{
			if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
			{
				time = path.distance / speed;
			}
			else if (type == TweenAction.MOVE_SPLINE || type == TweenAction.MOVE_SPLINE_LOCAL)
			{
				time = spline.distance / speed;
			}
			else
			{
				time = (to - from).magnitude / speed;
			}
		}
	}

	public LTDescr setFromColor(Color col)
	{
		from = new Vector3(0f, col.a, 0f);
		diff = new Vector3(1f, 0f, 0f);
		axis = new Vector3(col.r, col.g, col.b);
		return this;
	}

	public LTDescr pause()
	{
		if (direction != 0f)
		{
			directionLast = direction;
			direction = 0f;
		}
		return this;
	}

	public LTDescr resume()
	{
		direction = directionLast;
		return this;
	}

	public LTDescr setAxis(Vector3 axis)
	{
		this.axis = axis;
		return this;
	}

	public LTDescr setDelay(float delay)
	{
		if (useEstimatedTime)
		{
			this.delay = delay;
		}
		else
		{
			this.delay = delay;
		}
		return this;
	}

	public LTDescr setEase(LeanTweenType easeType)
	{
		tweenType = easeType;
		return this;
	}

	public LTDescr setOvershoot(float overshoot)
	{
		this.overshoot = overshoot;
		return this;
	}

	public LTDescr setPeriod(float period)
	{
		this.period = period;
		return this;
	}

	public LTDescr setEase(AnimationCurve easeCurve)
	{
		animationCurve = easeCurve;
		return this;
	}

	public LTDescr setTo(Vector3 to)
	{
		if (hasInitiliazed)
		{
			this.to = to;
			diff = to - from;
		}
		else
		{
			this.to = to;
		}
		return this;
	}

	public LTDescr setTo(Transform to)
	{
		toTrans = to;
		return this;
	}

	public LTDescr setFrom(Vector3 from)
	{
		if ((bool)trans)
		{
			init();
		}
		this.from = from;
		diff = to - this.from;
		return this;
	}

	public LTDescr setFrom(float from)
	{
		return setFrom(new Vector3(from, 0f, 0f));
	}

	public LTDescr setDiff(Vector3 diff)
	{
		this.diff = diff;
		return this;
	}

	public LTDescr setHasInitialized(bool has)
	{
		hasInitiliazed = has;
		return this;
	}

	public LTDescr setId(uint id)
	{
		_id = id;
		counter = global_counter;
		return this;
	}

	public LTDescr setTime(float time)
	{
		float num = passed / this.time;
		passed = time * num;
		this.time = time;
		return this;
	}

	public LTDescr setSpeed(float speed)
	{
		this.speed = speed;
		return this;
	}

	public LTDescr setRepeat(int repeat)
	{
		loopCount = repeat;
		if ((repeat > 1 && loopType == LeanTweenType.once) || (repeat < 0 && loopType == LeanTweenType.once))
		{
			loopType = LeanTweenType.clamp;
		}
		if (type == TweenAction.CALLBACK || type == TweenAction.CALLBACK_COLOR)
		{
			setOnCompleteOnRepeat(true);
		}
		return this;
	}

	public LTDescr setLoopType(LeanTweenType loopType)
	{
		this.loopType = loopType;
		return this;
	}

	public LTDescr setUseEstimatedTime(bool useEstimatedTime)
	{
		this.useEstimatedTime = useEstimatedTime;
		return this;
	}

	public LTDescr setIgnoreTimeScale(bool useUnScaledTime)
	{
		useEstimatedTime = useUnScaledTime;
		return this;
	}

	public LTDescr setUseFrames(bool useFrames)
	{
		this.useFrames = useFrames;
		return this;
	}

	public LTDescr setUseManualTime(bool useManualTime)
	{
		this.useManualTime = useManualTime;
		return this;
	}

	public LTDescr setLoopCount(int loopCount)
	{
		loopType = LeanTweenType.clamp;
		this.loopCount = loopCount;
		return this;
	}

	public LTDescr setLoopOnce()
	{
		loopType = LeanTweenType.once;
		return this;
	}

	public LTDescr setLoopClamp()
	{
		loopType = LeanTweenType.clamp;
		if (loopCount == 0)
		{
			loopCount = -1;
		}
		return this;
	}

	public LTDescr setLoopClamp(int loops)
	{
		loopCount = loops;
		return this;
	}

	public LTDescr setLoopPingPong()
	{
		loopType = LeanTweenType.pingPong;
		if (loopCount == 0)
		{
			loopCount = -1;
		}
		return this;
	}

	public LTDescr setLoopPingPong(int loops)
	{
		loopType = LeanTweenType.pingPong;
		loopCount = ((loops != -1) ? (loops * 2) : loops);
		return this;
	}

	public LTDescr setOnComplete(Action onComplete)
	{
		this.onComplete = onComplete;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete)
	{
		onCompleteObject = onComplete;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete, object onCompleteParam)
	{
		onCompleteObject = onComplete;
		if (onCompleteParam != null)
		{
			this.onCompleteParam = onCompleteParam;
		}
		return this;
	}

	public LTDescr setOnCompleteParam(object onCompleteParam)
	{
		this.onCompleteParam = onCompleteParam;
		return this;
	}

	public LTDescr setOnUpdate(Action<float> onUpdate)
	{
		onUpdateFloat = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateRatio(Action<float, float> onUpdate)
	{
		onUpdateFloatRatio = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateObject(Action<float, object> onUpdate)
	{
		onUpdateFloatObject = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateVector2(Action<Vector2> onUpdate)
	{
		onUpdateVector2 = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateVector3(Action<Vector3> onUpdate)
	{
		onUpdateVector3 = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateColor(Action<Color> onUpdate)
	{
		onUpdateColor = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateColor(Action<Color, object> onUpdate)
	{
		onUpdateColorObject = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<Color> onUpdate)
	{
		onUpdateColor = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<Color, object> onUpdate)
	{
		onUpdateColorObject = onUpdate;
		hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null)
	{
		onUpdateFloatObject = onUpdate;
		hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null)
	{
		onUpdateVector3Object = onUpdate;
		hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector2> onUpdate, object onUpdateParam = null)
	{
		onUpdateVector2 = onUpdate;
		hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null)
	{
		onUpdateVector3 = onUpdate;
		hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdateParam(object onUpdateParam)
	{
		this.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOrientToPath(bool doesOrient)
	{
		if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
		{
			if (path == null)
			{
				path = new LTBezierPath();
			}
			path.orientToPath = doesOrient;
		}
		else
		{
			spline.orientToPath = doesOrient;
		}
		return this;
	}

	public LTDescr setOrientToPath2d(bool doesOrient2d)
	{
		setOrientToPath(doesOrient2d);
		if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
		{
			path.orientToPath2d = doesOrient2d;
		}
		else
		{
			spline.orientToPath2d = doesOrient2d;
		}
		return this;
	}

	public LTDescr setRect(LTRect rect)
	{
		ltRect = rect;
		return this;
	}

	public LTDescr setRect(Rect rect)
	{
		ltRect = new LTRect(rect);
		return this;
	}

	public LTDescr setPath(LTBezierPath path)
	{
		this.path = path;
		return this;
	}

	public LTDescr setPoint(Vector3 point)
	{
		this.point = point;
		return this;
	}

	public LTDescr setDestroyOnComplete(bool doesDestroy)
	{
		destroyOnComplete = doesDestroy;
		return this;
	}

	public LTDescr setAudio(object audio)
	{
		onCompleteParam = audio;
		return this;
	}

	public LTDescr setOnCompleteOnRepeat(bool isOn)
	{
		onCompleteOnRepeat = isOn;
		return this;
	}

	public LTDescr setOnCompleteOnStart(bool isOn)
	{
		onCompleteOnStart = isOn;
		return this;
	}

	public LTDescr setRect(RectTransform rect)
	{
		rectTransform = rect;
		return this;
	}

	public LTDescr setSprites(Sprite[] sprites)
	{
		this.sprites = sprites;
		return this;
	}

	public LTDescr setFrameRate(float frameRate)
	{
		time = (float)sprites.Length / frameRate;
		return this;
	}

	public LTDescr setOnStart(Action onStart)
	{
		this.onStart = onStart;
		return this;
	}

	public LTDescr setDirection(float direction)
	{
		if (this.direction != -1f && this.direction != 1f)
		{
			Debug.LogWarning("You have passed an incorrect direction of '" + direction + "', direction must be -1f or 1f");
			return this;
		}
		if (this.direction != direction)
		{
			if (hasInitiliazed)
			{
				this.direction = direction;
			}
			else if (path != null)
			{
				path = new LTBezierPath(LTUtility.reverse(path.pts));
			}
			else if (spline != null)
			{
				spline = new LTSpline(LTUtility.reverse(spline.pts));
			}
		}
		return this;
	}

	public LTDescr setRecursive(bool useRecursion)
	{
		this.useRecursion = useRecursion;
		return this;
	}
}
