using UnityEngine;

public class HUDZoneTapMinigame : MonoBehaviour
{
	public delegate void OnClickDelegate(bool success, float amount);

	public OnClickDelegate OnClick;

	public GameObject _minigameRoot;

	public Transform _swords;

	public Transform _current;

	public Transform _left;

	public Transform _right;

	private Camera _gameCam;

	private Camera _screenCam;

	private float _direction;

	public bool _pIsActive
	{
		get
		{
			return _minigameRoot.activeInHierarchy;
		}
	}

	public void OnShowScreen()
	{
		_minigameRoot.SetActive(false);
		_direction = 1f;
	}

	public void OnUpdate()
	{
		if (_gameCam == null || _screenCam == null || !_minigameRoot.activeInHierarchy)
		{
			return;
		}
		if (GamePlayer.Instance.State == GamePlayerState.Combat)
		{
			if (OnClick != null)
			{
				OnClick(false, 0f);
			}
			OnClick = null;
			_minigameRoot.SetActive(false);
			return;
		}
		Vector3 localPosition = _swords.localPosition;
		localPosition.x += Time.deltaTime * _direction * 700f;
		localPosition.x = Mathf.Clamp(localPosition.x, _left.localPosition.x, _right.localPosition.x);
		if (localPosition.x >= _right.localPosition.x)
		{
			_direction = -1f;
		}
		else if (localPosition.x <= _left.localPosition.x)
		{
			_direction = 1f;
		}
		_swords.localPosition = localPosition;
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		if (localPosition.x >= _current.localPosition.x)
		{
			float amount = (localPosition.x - _left.localPosition.x) / (_right.localPosition.x - _left.localPosition.x);
			if (OnClick != null)
			{
				OnClick(true, amount);
			}
			OnClick = null;
			_minigameRoot.SetActive(false);
			GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.swingHitSucceed);
		}
		else
		{
			if (OnClick != null)
			{
				OnClick(false, 0f);
			}
			OnClick = null;
			_minigameRoot.SetActive(false);
			GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.swingHitFail);
		}
	}

	public void BeginMinigame()
	{
		if (GamePlayer.Instance.State != GamePlayerState.Exploring)
		{
			if (OnClick != null)
			{
				OnClick(false, 0f);
			}
			OnClick = null;
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
		Vector3 localPosition = _swords.transform.localPosition;
		localPosition.x = _left.localPosition.x + 1f;
		_swords.transform.localPosition = localPosition;
		_direction = 1f;
		_minigameRoot.SetActive(true);
		GamePlayer.Instance.Player.Character.EquipWeapon(true);
		GamePlayer.Instance.Player.Character.ResetPath();
		GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.swingHitPrepare);
	}

	public void CloseMinigame()
	{
		_minigameRoot.SetActive(false);
	}
}
