using UnityEngine;

public class HUDSwordTapTelegraph : MonoBehaviour
{
	public GameObject _minigameRoot;

	private Camera _gameCam;

	private Camera _screenCam;

	private SwordTappingIcon _swordTappingIcon;

	public void OnShowScreen()
	{
		_minigameRoot.SetActive(false);
	}

	public void Show(SwordTappingIcon icon)
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
		_swordTappingIcon = icon;
	}

	public void OnUpdate()
	{
		if (!(_gameCam == null) && !(_screenCam == null) && _minigameRoot.activeInHierarchy && !(_swordTappingIcon == null))
		{
			Vector3 position = _gameCam.WorldToViewportPoint(_swordTappingIcon.transform.position);
			Vector3 position2 = _screenCam.ViewportToWorldPoint(position);
			position2.z = 0f;
			_minigameRoot.transform.position = position2;
		}
	}

	public void Hide()
	{
		_minigameRoot.SetActive(false);
	}
}
