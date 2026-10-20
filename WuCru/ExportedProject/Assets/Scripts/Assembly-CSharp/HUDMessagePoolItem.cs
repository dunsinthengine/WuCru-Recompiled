using AmuzoEngine;
using UnityEngine;

public class HUDMessagePoolItem : MonoBehaviour
{
	public UILabel _label;

	private bool _isAvailable = true;

	private Vector3 _worldPos;

	private float _triggerTime;

	private Camera _gameCam;

	private Camera _screenCam;

	public bool _pIsAvailable
	{
		get
		{
			return _isAvailable;
		}
	}

	public void Init()
	{
		_isAvailable = true;
		_label.enabled = false;
		_label.alpha = 0f;
	}

	public void Trigger(Vector3 worldPos, string labelText)
	{
		_label.text = labelText;
		Trigger(worldPos);
	}

	public void Trigger(Vector3 worldPos)
	{
		_label.enabled = true;
		_label.alpha = 0f;
		_worldPos = worldPos;
		_isAvailable = false;
		_triggerTime = Time.time;
		_label.transform.localPosition = Vector3.zero;
		if (_gameCam == null)
		{
			_gameCam = InGameCam._pInstance.GetComponent<Camera>();
		}
		if (_screenCam == null)
		{
			_screenCam = Facades<ScreenFacade>.Instance._mainUICamera.GetComponent<Camera>();
		}
	}

	public void OnUpdate()
	{
		Vector3 position = _gameCam.WorldToViewportPoint(_worldPos);
		Vector3 position2 = _screenCam.ViewportToWorldPoint(position);
		position2.z = 0f;
		base.transform.position = position2;
		float num = Time.time - _triggerTime;
		if (num < 0.75f)
		{
			if (num > 0.5f)
			{
				float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num - 0.5f, 0.25f, 0f, 1f);
				_label.alpha = 1f - num2;
			}
			else
			{
				_label.alpha = 1f;
			}
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.75f, 0f, 1f);
			_label.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, 75f, 0f), t);
		}
		else
		{
			_isAvailable = true;
			_label.alpha = 0f;
			_label.enabled = false;
		}
	}
}
