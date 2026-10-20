using System;
using UnityEngine;

public interface LTDescr
{
	bool toggle { get; set; }

	bool useEstimatedTime { get; set; }

	bool useFrames { get; set; }

	bool useManualTime { get; set; }

	bool hasInitiliazed { get; set; }

	bool hasPhysics { get; set; }

	bool onCompleteOnRepeat { get; set; }

	bool onCompleteOnStart { get; set; }

	bool useRecursion { get; set; }

	float passed { get; set; }

	float delay { get; set; }

	float time { get; set; }

	float lastVal { get; set; }

	int loopCount { get; set; }

	uint counter { get; set; }

	float direction { get; set; }

	float directionLast { get; set; }

	float overshoot { get; set; }

	float period { get; set; }

	bool destroyOnComplete { get; set; }

	Transform trans { get; set; }

	Transform toTrans { get; set; }

	LTRect ltRect { get; set; }

	Vector3 from { get; set; }

	Vector3 to { get; set; }

	Vector3 diff { get; set; }

	Vector3 point { get; set; }

	Vector3 axis { get; set; }

	Quaternion origRotation { get; set; }

	LTBezierPath path { get; set; }

	LTSpline spline { get; set; }

	TweenAction type { get; set; }

	LeanTweenType tweenType { get; set; }

	AnimationCurve animationCurve { get; set; }

	LeanTweenType loopType { get; set; }

	bool hasUpdateCallback { get; set; }

	Action<float> onUpdateFloat { get; set; }

	Action<float, float> onUpdateFloatRatio { get; set; }

	Action<float, object> onUpdateFloatObject { get; set; }

	Action<Vector2> onUpdateVector2 { get; set; }

	Action<Vector3> onUpdateVector3 { get; set; }

	Action<Vector3, object> onUpdateVector3Object { get; set; }

	Action<Color> onUpdateColor { get; set; }

	Action onComplete { get; set; }

	Action<object> onCompleteObject { get; set; }

	object onCompleteParam { get; set; }

	object onUpdateParam { get; set; }

	Action onStart { get; set; }

	int uniqueId { get; }

	int id { get; }

	SpriteRenderer spriteRen { get; set; }

	[Obsolete("Use 'LeanTween.cancel( id )' instead")]
	LTDescr cancel(GameObject gameObject);

	void cleanup();

	void init();

	LTDescr pause();

	void reset();

	LTDescr resume();

	LTDescr setAudio(object audio);

	LTDescr setAxis(Vector3 axis);

	LTDescr setDelay(float delay);

	LTDescr setDestroyOnComplete(bool doesDestroy);

	LTDescr setDiff(Vector3 diff);

	LTDescr setDirection(float direction);

	LTDescr setEase(LeanTweenType easeType);

	LTDescr setEase(AnimationCurve easeCurve);

	LTDescr setFrom(float from);

	LTDescr setFrom(Vector3 from);

	LTDescr setFromColor(Color col);

	LTDescr setHasInitialized(bool has);

	LTDescr setId(uint id);

	LTDescr setIgnoreTimeScale(bool useUnScaledTime);

	LTDescr setSpeed(float speed);

	LTDescr setLoopClamp();

	LTDescr setLoopClamp(int loops);

	LTDescr setLoopCount(int loopCount);

	LTDescr setLoopOnce();

	LTDescr setLoopPingPong();

	LTDescr setLoopPingPong(int loops);

	LTDescr setLoopType(LeanTweenType loopType);

	LTDescr setOnComplete(Action onComplete);

	LTDescr setOnComplete(Action<object> onComplete);

	LTDescr setOnComplete(Action<object> onComplete, object onCompleteParam);

	LTDescr setOnCompleteOnRepeat(bool isOn);

	LTDescr setOnCompleteOnStart(bool isOn);

	LTDescr setOnCompleteParam(object onCompleteParam);

	LTDescr setOnStart(Action onStart);

	LTDescr setOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null);

	LTDescr setOnUpdate(Action<float> onUpdate);

	LTDescr setOnUpdate(Action<Color> onUpdate);

	LTDescr setOnUpdate(Action<Color, object> onUpdate);

	LTDescr setOnUpdate(Action<Vector2> onUpdate, object onUpdateParam = null);

	LTDescr setOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null);

	LTDescr setOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null);

	LTDescr setOnUpdateColor(Action<Color> onUpdate);

	LTDescr setOnUpdateColor(Action<Color, object> onUpdate);

	LTDescr setOnUpdateObject(Action<float, object> onUpdate);

	LTDescr setOnUpdateParam(object onUpdateParam);

	LTDescr setOnUpdateRatio(Action<float, float> onUpdate);

	LTDescr setOnUpdateVector2(Action<Vector2> onUpdate);

	LTDescr setOnUpdateVector3(Action<Vector3> onUpdate);

	LTDescr setOrientToPath(bool doesOrient);

	LTDescr setOrientToPath2d(bool doesOrient2d);

	LTDescr setOvershoot(float overshoot);

	LTDescr setPath(LTBezierPath path);

	LTDescr setPeriod(float period);

	LTDescr setPoint(Vector3 point);

	LTDescr setRect(LTRect rect);

	LTDescr setRect(Rect rect);

	LTDescr setRepeat(int repeat);

	LTDescr setRecursive(bool useRecursion);

	LTDescr setRect(RectTransform rect);

	LTDescr setSprites(Sprite[] sprites);

	LTDescr setFrameRate(float frameRate);

	LTDescr setTime(float time);

	LTDescr setTo(Transform to);

	LTDescr setTo(Vector3 to);

	LTDescr setUseEstimatedTime(bool useEstimatedTime);

	LTDescr setUseFrames(bool useFrames);

	LTDescr setUseManualTime(bool useManualTime);

	new string ToString();
}
