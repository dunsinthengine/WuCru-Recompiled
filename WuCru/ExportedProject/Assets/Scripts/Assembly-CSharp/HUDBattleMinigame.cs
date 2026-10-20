using UnityEngine;

public class HUDBattleMinigame : MonoBehaviour
{
	public GameObject _minigameRoot;

	public Transform _current;

	public Transform _left;

	public Transform _right;

	private Camera _gameCam;

	private Camera _screenCam;

	public void OnShowScreen()
	{
		_minigameRoot.SetActive(false);
	}

	public void OnUpdate()
	{
		if (!(_gameCam == null) && !(_screenCam == null) && _minigameRoot.activeInHierarchy && !(GamePlayer.Instance == null) && !(GamePlayer.Instance.Player == null) && !(GamePlayer.Instance.Player.Character == null))
		{
			Vector3 position = _gameCam.WorldToViewportPoint(GamePlayer.Instance.Player.Character.transform.position - Vector3.up);
			Vector3 position2 = _screenCam.ViewportToWorldPoint(position);
			position2.z = 0f;
			_minigameRoot.transform.position = position2;
			_current.transform.localPosition = Vector3.Lerp(_left.localPosition, _right.localPosition, GamePlayer.Instance.CombatManager.DuellingStatus);
		}
	}

	public void BeginMinigame()
	{
		if (_gameCam == null)
		{
			_gameCam = InGameCam._pInstance.GetComponent<Camera>();
		}
		if (_screenCam == null)
		{
			_screenCam = Facades<ScreenFacade>.Instance._mainUICamera.GetComponent<Camera>();
		}
		_minigameRoot.SetActive(true);
	}

	public void CloseMinigame()
	{
		_minigameRoot.SetActive(false);
	}
}
