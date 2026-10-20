using System;
using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
	public delegate void DOnDeviceBackButtonPressed(ref bool isHandled);

	private const string LOG_TAG = "[BackButtonHandler] ";

	[SerializeField]
	private ETargetPlatform[] _hideBackButtonsOnPlatforms;

	private List<UIButton> _backButtons;

	private BooleanStateRequests _backButtonDisableRequests;

	private bool _isBackButtonDisabled;

	public static event DOnDeviceBackButtonPressed _pOnDeviceBackButtonPressed;

	private void Awake()
	{
		_backButtonDisableRequests = new BooleanStateRequests((bool newState) =>
		{
			_isBackButtonDisabled = newState;
		}, () => _isBackButtonDisabled, false);
		ScreenFacade._pOnRegisterScreen += OnRegisterScreen;
	}

	private void Update()
	{
		HandleDeviceBackButton();
	}

	private void OnRegisterScreen(ScreenBase screen)
	{
		FindBackButtons(screen);
	}

	public void BlockBackButton(bool wantBlocked, object source)
	{
		if (_backButtonDisableRequests != null)
		{
			Debug.Log("[ScreenFacade] BlockBackButton: " + wantBlocked + ", " + source);
			_backButtonDisableRequests.RequestState(wantBlocked, source, 0, true);
		}
	}

	private void FindBackButtons(ScreenBase screen)
	{
		if (_backButtons == null)
		{
			_backButtons = new List<UIButton>();
		}
		bool isHideBackButtons = _hideBackButtonsOnPlatforms.Contains(ETargetPlatform.ANDROID);
		ScreenElementLinks links = screen.GetComponent<ScreenElementLinks>();
		FindBackButtons(screen.transform, (UIButton button, bool isReallyBackButton) =>
		{
			if (!_backButtons.Contains(button))
			{
				_backButtons.Add(button);
			}
			if (isHideBackButtons && isReallyBackButton)
			{
				if (links != null)
				{
					UIWidget component = button.GetComponent<UIWidget>();
					if (component != null)
					{
						links.IgnoreWidget(component);
					}
				}
				ClearTextures(button.transform);
			}
		});
	}

	private static void FindBackButtons(Transform parent, Action<UIButton, bool> onFound)
	{
		if (parent.tag == "BackButton")
		{
			UIButton component = parent.GetComponent<UIButton>();
			if (component != null)
			{
				onFound(component, true);
			}
		}
		else if (parent.tag == "PseudoBackButton")
		{
			UIButton component2 = parent.GetComponent<UIButton>();
			if (component2 != null)
			{
				onFound(component2, false);
			}
		}
		else
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				FindBackButtons(parent.GetChild(i), onFound);
			}
		}
	}

	private static void ClearTextures(Transform parent)
	{
		UITexture component = parent.GetComponent<UITexture>();
		if (component != null)
		{
			component.mainTexture = null;
		}
		UISprite component2 = parent.GetComponent<UISprite>();
		if (component2 != null)
		{
			component2.spriteName = null;
		}
		UIButton component3 = parent.GetComponent<UIButton>();
		if (component3 != null)
		{
			component3.normalSprite = null;
			component3.hoverSprite = null;
			component3.pressedSprite = null;
			component3.disabledSprite = null;
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			ClearTextures(parent.GetChild(i));
		}
	}

	private void HandleDeviceBackButton()
	{
		if (!_isBackButtonDisabled)
		{
			bool flag = false;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnDeviceBackButtonPressed();
			}
		}
	}

	private void OnDeviceBackButtonPressed()
	{
		bool isBackButtonPressHandled = false;
		if (_pOnDeviceBackButtonPressed != null)
		{
			_pOnDeviceBackButtonPressed(ref isBackButtonPressHandled);
		}
		if (Facades<ScreenFacade>.Instance != null)
		{
			Facades<ScreenFacade>.Instance.ForEachActiveScreen((ScreenBase s) =>
			{
				s.OnDeviceBackButtonPressed(ref isBackButtonPressHandled);
			});
		}
		if (isBackButtonPressHandled)
		{
			return;
		}
		UIButton uIButton = null;
		int num = 0;
		int num2 = 0;
		Camera mainCamera = UICamera.mainCamera;
		for (int num3 = 0; num3 < _backButtons.Count; num3++)
		{
			if (_backButtons[num3] == null || _backButtons[num3].transform.lossyScale.sqrMagnitude == 0f || !NGUITools.GetActive(_backButtons[num3]) || !_backButtons[num3].isEnabled)
			{
				continue;
			}
			UIWidget component = _backButtons[num3].GetComponent<UIWidget>();
			if (!(component == null) && component.isVisible)
			{
				Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(component.transform);
				Vector3 vector = mainCamera.WorldToScreenPoint(bounds.min);
				Vector3 vector2 = mainCamera.WorldToScreenPoint(bounds.max);
				if (!(vector2.x < 0f) && !(vector.x > (float)Screen.width) && !(vector2.y < 0f) && !(vector.y > (float)Screen.height) && (uIButton == null || component.panel.depth > num2 || (component.panel.depth == num2 && component.depth > num)))
				{
					uIButton = _backButtons[num3];
					num = component.depth;
					num2 = component.panel.depth;
				}
			}
		}
		if (uIButton != null)
		{
			uIButton.SimulateButtonPressAttempt();
		}
	}
}
