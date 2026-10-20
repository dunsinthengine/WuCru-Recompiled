using AmuzoEngine;
using UnityEngine;

public class HUDItemTrackerTexture : MonoBehaviour
{
	public enum DISPLAY_STATE
	{
		TWEEN_IN = 0,
		IDLE = 1,
		TWEEN_OUT = 2
	}

	public UITexture[] _pool;

	public Transform[] _axis;

	public bool _screenEdgeOnly;

	private Transform[] _trackTransforms;

	private float[] _timer;

	private DISPLAY_STATE[] _displayState;

	private bool[] _autoDismiss;

	private int _poolSize;

	private Camera _gameCam;

	private Camera _screenCam;

	private float _tweenInTime;

	private float _tweenOutTime;

	public void OnShowScreen()
	{
		_poolSize = _pool.Length;
		if (_trackTransforms == null)
		{
			_trackTransforms = new Transform[_poolSize];
			_displayState = new DISPLAY_STATE[_poolSize];
			_timer = new float[_poolSize];
			_autoDismiss = new bool[_poolSize];
		}
		for (int i = 0; i < _poolSize; i++)
		{
			_pool[i].enabled = false;
			_trackTransforms[i] = null;
		}
	}

	public void DisplayIcon(Transform transformToTrack, bool autoDismiss = false, float tweenInTime = 0.5f, float tweenOutTime = 0.5f)
	{
		_tweenInTime = tweenInTime;
		_tweenOutTime = tweenOutTime;
		for (int i = 0; i < _poolSize; i++)
		{
			if (_trackTransforms[i] == transformToTrack)
			{
				return;
			}
		}
		for (int j = 0; j < _poolSize; j++)
		{
			if (_trackTransforms[j] == null)
			{
				_pool[j].transform.localScale = Vector3.one;
				_trackTransforms[j] = transformToTrack;
				_pool[j].enabled = true;
				_displayState[j] = DISPLAY_STATE.TWEEN_IN;
				_timer[j] = Time.time;
				_autoDismiss[j] = autoDismiss;
				OnUpdate();
				break;
			}
		}
	}

	public void DismissIcon(Transform transformToTrack)
	{
		for (int i = 0; i < _poolSize; i++)
		{
			if (_trackTransforms[i] == transformToTrack)
			{
				if (_displayState[i] != DISPLAY_STATE.TWEEN_OUT)
				{
					Debug.Log("Dismiss: " + i + " " + transformToTrack.name, transformToTrack);
					_displayState[i] = DISPLAY_STATE.TWEEN_OUT;
					_timer[i] = RealTime.time;
				}
				break;
			}
		}
	}

	public void DismissAll()
	{
		for (int i = 0; i < _poolSize; i++)
		{
			if (_displayState[i] != DISPLAY_STATE.TWEEN_OUT)
			{
				_displayState[i] = DISPLAY_STATE.TWEEN_OUT;
				_timer[i] = RealTime.time;
			}
		}
	}

	public void OnUpdate()
	{
		if (_trackTransforms == null || InGameCam._pInstance == null)
		{
			return;
		}
		if (_gameCam == null)
		{
			_gameCam = InGameCam._pInstance.GetComponent<Camera>();
		}
		if (_screenCam == null)
		{
			_screenCam = Facades<ScreenFacade>.Instance._mainUICamera.GetComponent<Camera>();
		}
		if (_gameCam == null || _screenCam == null)
		{
			return;
		}
		for (int i = 0; i < _poolSize; i++)
		{
			if (!(_trackTransforms[i] != null))
			{
				continue;
			}
			Debug.DrawLine(_trackTransforms[i].position, _trackTransforms[i].position + Vector3.up * 30f, Color.blue);
			Vector3 vector = _gameCam.WorldToViewportPoint(_trackTransforms[i].position + Vector3.up);
			bool flag = true;
			Vector3 vector2 = vector;
			if (_screenEdgeOnly)
			{
				if (vector.x > 1f || vector.x < 0f || vector.y > 1f || vector.y < 0f)
				{
					vector.x = Mathf.Clamp01(vector.x);
					vector.y = Mathf.Clamp01(vector.y);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			Vector3 position = _screenCam.ViewportToWorldPoint(vector);
			position.z = 0f;
			_axis[i].transform.position = position;
			Vector3 localPosition = _axis[i].transform.localPosition;
			if (_screenEdgeOnly)
			{
				if (vector.x > 0.5f)
				{
					localPosition.x -= (float)_pool[i].width * 0.5f;
					localPosition.x -= 30f;
				}
				if (vector.x < 0.5f)
				{
					localPosition.x += (float)_pool[i].width * 0.5f;
					localPosition.x += 30f;
				}
				if (vector.y > 0.5f)
				{
					localPosition.y -= (float)_pool[i].height * 0.5f;
					localPosition.y -= 30f;
				}
				if (vector.y < 0.5f)
				{
					localPosition.y += (float)_pool[i].height * 0.5f;
					localPosition.y += 30f;
				}
			}
			_axis[i].transform.localPosition = localPosition;
			float num = 0f;
			float num2 = RealTime.time - _timer[i];
			switch (_displayState[i])
			{
			case DISPLAY_STATE.TWEEN_IN:
				if (num2 < _tweenInTime)
				{
					num = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num2, _tweenInTime, 0f, 1f);
					_pool[i].alpha = ((!flag) ? 0f : num);
				}
				else
				{
					_displayState[i] = DISPLAY_STATE.IDLE;
					_pool[i].alpha = (flag ? 1 : 0);
				}
				break;
			case DISPLAY_STATE.TWEEN_OUT:
				if (num2 < _tweenOutTime)
				{
					num = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num2, _tweenOutTime, 0f, 1f);
					_pool[i].alpha = ((!flag) ? 0f : (1f - num));
				}
				else
				{
					_displayState[i] = DISPLAY_STATE.IDLE;
					_pool[i].alpha = 0f;
					_trackTransforms[i] = null;
				}
				break;
			case DISPLAY_STATE.IDLE:
				if (_screenEdgeOnly)
				{
					_pool[i].alpha = (flag ? 1 : 0);
				}
				else
				{
					_pool[i].alpha = 1f;
				}
				if (_autoDismiss[i] && num2 > 2f)
				{
					DismissIcon(_trackTransforms[i]);
				}
				break;
			}
		}
	}

	public void SetScale(Vector3 newScale, Transform targ)
	{
		for (int i = 0; i < _poolSize; i++)
		{
			if (_trackTransforms[i] == targ)
			{
				_pool[i].transform.localScale = newScale;
				break;
			}
		}
	}
}
