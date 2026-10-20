using AmuzoEngine;
using UnityEngine;

public class HUDElementDisplay : MonoBehaviour
{
	public enum DISPLAY_STATE
	{
		TWEEN_IN = 0,
		IDLE = 1,
		TWEEN_OUT = 2
	}

	public UITexture[] _pool;

	public Texture[] _elements;

	private Transform[] _trackTransforms;

	private float[] _timer;

	private DISPLAY_STATE[] _displayState;

	private bool[] _autoDismiss;

	private float _abilityScale = 1f;

	private float _maxScale = 1.2f;

	private float _minScale = 0.8f;

	private bool _scaleDir;

	private int _poolSize;

	private Camera _gameCam;

	private Camera _screenCam;

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
			_pool[i].mainTexture = null;
			_trackTransforms[i] = null;
		}
	}

	public void DisplayIcon(ElementalPower power, Transform transformToTrack, bool autoDismiss = false)
	{
		if (GameController.Instance != null)
		{
			GameController.Instance._hudElemental = power;
		}
		for (int i = 0; i < _poolSize; i++)
		{
			if (_trackTransforms[i] == transformToTrack)
			{
				return;
			}
		}
		for (int j = 0; j < _poolSize; j++)
		{
			if (_pool[j].mainTexture == null)
			{
				_pool[j].mainTexture = _elements[(int)power];
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
		GameController.Instance._hudElemental = ElementalPower.None;
		_abilityScale = 1f;
		_scaleDir = false;
		for (int i = 0; i < _poolSize; i++)
		{
			if (_trackTransforms[i] == transformToTrack)
			{
				_displayState[i] = DISPLAY_STATE.TWEEN_OUT;
				_timer[i] = Time.time;
				break;
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
			if (!(_pool[i].mainTexture != null) || !(_trackTransforms[i] != null))
			{
				continue;
			}
			Vector3 position = _gameCam.WorldToViewportPoint(_trackTransforms[i].position + Vector3.up * 1.5f);
			Vector3 position2 = _screenCam.ViewportToWorldPoint(position);
			position2.z = 0f;
			_pool[i].transform.position = position2;
			float num = 0f;
			float num2 = Time.time - _timer[i];
			switch (_displayState[i])
			{
			case DISPLAY_STATE.TWEEN_IN:
				if (num2 < 0.5f)
				{
					num = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num2, 0.5f, 0f, 1f);
					_pool[i].alpha = num;
				}
				else
				{
					_displayState[i] = DISPLAY_STATE.IDLE;
					_pool[i].alpha = 1f;
				}
				break;
			case DISPLAY_STATE.TWEEN_OUT:
				if (num2 < 0.5f)
				{
					num = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num2, 0.5f, 0f, 1f);
					_pool[i].alpha = 1f - num;
					break;
				}
				_displayState[i] = DISPLAY_STATE.IDLE;
				_pool[i].alpha = 0f;
				_pool[i].mainTexture = null;
				_trackTransforms[i] = null;
				break;
			case DISPLAY_STATE.IDLE:
			{
				float deltaTime = Time.deltaTime;
				Vector3 localScale = _pool[i].transform.localScale;
				localScale.x = _abilityScale;
				localScale.y = _abilityScale;
				_pool[i].transform.localScale = localScale;
				if (!_scaleDir)
				{
					_abilityScale += deltaTime;
					if (_abilityScale > _maxScale)
					{
						_abilityScale = _maxScale;
						_scaleDir = true;
					}
				}
				else
				{
					_abilityScale -= deltaTime;
					if (_abilityScale < _minScale)
					{
						_abilityScale = _minScale;
						_scaleDir = false;
					}
				}
				if (_autoDismiss[i] && num2 > 2f)
				{
					DismissIcon(_trackTransforms[i]);
				}
				break;
			}
			}
		}
	}
}
