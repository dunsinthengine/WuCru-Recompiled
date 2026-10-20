using UnityEngine;

public static class SimulateButtonPress
{
	private const string LOG_TAG = "[SimulateButtonPress] ";

	public static void SimulateButtonPressSuccess(this UIButton button)
	{
		button.SendMessage("OnClick");
	}

	public static void SimulateButtonPressAttempt(this UIButton button)
	{
		Collider component = button.gameObject.GetComponent<Collider>();
		Vector3 position = ((!(component != null)) ? button.gameObject.transform.position : button.gameObject.transform.TransformPoint(component.bounds.center));
		position = UICamera.mainCamera.WorldToScreenPoint(position);
		UICamera.Touch buttonTouch = new UICamera.Touch
		{
			position = position,
			fingerId = 1,
			phase = TouchPhase.Began,
			tapCount = 0
		};
		int overrideCallCount1 = 0;
		int overrideCallCount2 = 0;
		UICamera.GetInputTouchCount = () =>
		{
			overrideCallCount1++;
			if (overrideCallCount1 == 2)
			{
				UICamera.GetInputTouchCount = null;
			}
			return 1;
		};
		UICamera.GetInputTouch = (int touchIdx) =>
		{
			overrideCallCount2++;
			if (overrideCallCount2 == 2)
			{
				UICamera.GetInputTouch = null;
				buttonTouch.phase = TouchPhase.Ended;
			}
			return buttonTouch;
		};
	}
}
