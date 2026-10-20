using AmuzoEngine;
using UnityEngine;

public class HUDChestMinigame : MonoBehaviour
{
	public const float GAME_DUR = 1.8f;

	public GameObject _minigameRoot;

	public Transform _keyTransform;

	public UISprite _successFlash;

	public Transform _leftBar;

	public Transform _rightBar;

	public Transform _farLeft;

	public Transform _farRight;

	private Camera _gameCam;

	private Camera _screenCam;

	private Transform _chestTransform;

	private float _startTime;

	private bool _inProgress;

	public bool _pInProgress
	{
		get
		{
			return _inProgress;
		}
	}

	public bool _pIsInSafeZone
	{
		get
		{
			return _keyTransform.localPosition.x > _leftBar.localPosition.x && _keyTransform.localPosition.x < _rightBar.localPosition.x;
		}
	}

	public void OnShowScreen()
	{
		_minigameRoot.SetActive(false);
		_inProgress = false;
	}

	public void BeginGame(Transform chestTransform)
	{
		if (!(Time.time - _startTime < 0.1f))
		{
			_chestTransform = chestTransform;
			_minigameRoot.SetActive(true);
			_successFlash.enabled = false;
			_inProgress = true;
			_startTime = Time.time;
			if (_gameCam == null)
			{
				_gameCam = InGameCam._pInstance.GetComponent<Camera>();
			}
			if (_screenCam == null)
			{
				_screenCam = Facades<ScreenFacade>.Instance._mainUICamera.GetComponent<Camera>();
			}
			OnUpdate();
		}
	}

	public void CancelMinigame()
	{
		_startTime = Time.time;
		_inProgress = false;
		_chestTransform = null;
		_minigameRoot.SetActive(false);
	}

	public void OnUpdate()
	{
		if (!(_gameCam == null) && !(_screenCam == null) && !(_chestTransform == null))
		{
			float num = Time.time - _startTime;
			if (num > 1.8f)
			{
				_minigameRoot.SetActive(false);
				_chestTransform = null;
				_inProgress = false;
				return;
			}
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.Linear, num, 1.8f, 0f, 1f);
			_keyTransform.localPosition = Vector3.Lerp(_farLeft.localPosition, _farRight.localPosition, t);
			_successFlash.enabled = _pIsInSafeZone;
			Vector3 position = _gameCam.WorldToViewportPoint(_chestTransform.position + Vector3.up);
			Vector3 position2 = _screenCam.ViewportToWorldPoint(position);
			position2.z = 0f;
			_minigameRoot.transform.position = position2;
		}
	}
}
