using UnityEngine;

public class ScreenElementLinksTestScreen : ScreenBase
{
	private const string LOG_TAG = "[ScreenElementLinksTestScreen] ";

	protected override void Awake()
	{
		base.Awake();
		ScreenElementLinks._pOnFocusElementChanged += (UIWidget oldFocus, UIWidget newFocus) =>
		{
			if (oldFocus != null)
			{
				UIButton component = oldFocus.GetComponent<UIButton>();
				if (component != null)
				{
					component.state = UIButtonColor.State.Normal;
				}
			}
			if (newFocus != null)
			{
				UIButton component = newFocus.GetComponent<UIButton>();
				if (component != null)
				{
					component.state = UIButtonColor.State.Hover;
				}
			}
		};
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ScreenElementLinks.ChangeFocus(Vector2.up);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			ScreenElementLinks.ChangeFocus(Vector2.down);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			ScreenElementLinks.ChangeFocus(Vector2.left);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			ScreenElementLinks.ChangeFocus(Vector2.right);
		}
		else
		{
			if (!Input.GetKeyDown(KeyCode.Space))
			{
				return;
			}
			UIWidget pFocusElement = ScreenElementLinks._pFocusElement;
			if (pFocusElement != null)
			{
				UIButton component = pFocusElement.GetComponent<UIButton>();
				if (component != null)
				{
					component.SimulateButtonPressAttempt();
				}
			}
		}
	}
}
