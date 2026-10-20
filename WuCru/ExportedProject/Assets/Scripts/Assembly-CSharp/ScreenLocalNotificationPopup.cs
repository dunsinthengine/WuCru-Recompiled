using UnityEngine;

public class ScreenLocalNotificationPopup : ScreenBase
{
	public ScreenObjectTransitionWidget _wuMessageWidget;

	public ScreenObjectTransitionWidget _messageBoxVisibilityWidget;

	public ScreenObjectTransitionWidget _messageBoxWidget;

	public UIButton _denyButton;

	public UIButton _approveButton;

	public UILabel _wuMessage;

	private bool _wantDenyMessage;

	private bool _showingDenyMessage;

	protected override void OnExitScreen()
	{
		base.OnExitScreen();
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_wuMessageWidget.SetStateConsideredTweenedIn(false);
		_wuMessageWidget.SetToTweenInStartColor();
		_wuMessageWidget.TweenIn();
		ScreenHeaderNavigation._pInstance.DisableButtons();
		_wuMessage.text = Localise("Notifications.Prompt");
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();
		if (!_showingDenyMessage && !_wantDenyMessage && Input.GetKeyDown(KeyCode.Escape))
		{
			OnDeny();
		}
		if (_showingDenyMessage && _messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE && (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Escape)))
		{
			_showingDenyMessage = false;
			EndFlow();
			GameController.Instance.PlayerProfile._pSeenNotificationPrompt = true;
		}
		if (_wantDenyMessage && !_messageBoxWidget._pConsideredTweenedIn && _messageBoxWidget._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			_wuMessage.text = Localise("Notifications.DenyMessage");
			_messageBoxWidget.TweenIn();
			_wuMessageWidget.TweenIn();
			_wantDenyMessage = false;
			_showingDenyMessage = true;
		}
	}

	public void OnConfirm()
	{
		if (AmuzoMonoSingleton<LocalPushNotificationFacade>._pInstance != null)
		{
			AmuzoMonoSingleton<LocalPushNotificationFacade>._pInstance.EnablePushNotifications();
		}
		EndFlow();
		GameController.Instance.PlayerProfile._pSeenNotificationPrompt = true;
	}

	public void OnDeny()
	{
		_wuMessageWidget.TweenOut();
		_messageBoxWidget.TweenOut();
		_wantDenyMessage = true;
		_approveButton.gameObject.SetActive(false);
		_denyButton.gameObject.SetActive(false);
	}
}
