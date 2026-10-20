using AmuzoEngine;
using UnityEngine;

public class InGameCardFX : MonoBehaviour
{
	private const float ENEMY_TO_MIDDLE_TIME = 1f;

	private const float MIDDLE_TO_CORNER_TIME = 1f;

	private const float MIDDLE_STAY_TIME = 0.5f;

	private bool _hasBegun;

	private float _startTime;

	private Vector3 _startPos;

	private float _toCornerPositionWait;

	private float _cornerPositionStartTime;

	private Camera _cam;

	public void Begin()
	{
		_hasBegun = true;
		_startTime = Time.time;
		_startPos = base.transform.position + Vector3.up;
		_toCornerPositionWait = 0.5f;
		_cam = InGameCam._pInstance.GetComponent<Camera>();
	}

	private void Update()
	{
		if (!_hasBegun)
		{
			return;
		}
		if (_toCornerPositionWait > 0f)
		{
			Vector3 position = InGameCam._pInstance.transform.position;
			float num = Time.time - _startTime;
			Vector3 vector = position + InGameCam._pInstance.transform.forward * 5f;
			if (num < 1f)
			{
				float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 1f, 0f, 1f);
				base.transform.position = Vector3.Lerp(_startPos, vector, t);
				return;
			}
			base.transform.position = vector;
			_toCornerPositionWait -= Time.deltaTime;
			if (_toCornerPositionWait <= 0f)
			{
				_cornerPositionStartTime = Time.time;
			}
		}
		else
		{
			Vector3 position2 = InGameCam._pInstance.transform.position;
			Vector3 a = position2 + InGameCam._pInstance.transform.forward * 5f;
			Vector3 b = _cam.ViewportToWorldPoint(new Vector3(-0.1f, 1.1f, 5f));
			float num2 = Time.time - _cornerPositionStartTime;
			if (num2 < 1f)
			{
				float t2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num2, 1f, 0f, 1f);
				base.transform.position = Vector3.Lerp(a, b, t2);
				base.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t2);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
