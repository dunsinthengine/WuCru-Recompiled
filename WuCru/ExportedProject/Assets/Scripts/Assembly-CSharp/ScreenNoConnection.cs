using UnityEngine;

public class ScreenNoConnection : ScreenBase
{
	public BoxTransitionFX _boxTransitionFX;

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			OnDismiss();
		}
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		ScreenHeaderNavigation._pInstance.DisableButtons();
		_boxTransitionFX.Begin();
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
		ScreenHeaderNavigation._pInstance.EnableButtons();
	}

	public void OnDismiss()
	{
		EndFlow();
	}
}
