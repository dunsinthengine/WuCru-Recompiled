using UnityEngine;

public class ScreenCutsceneCityIntro : ScreenBase
{
	private const float CUTSCENE_DUR = 26f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		ScreenHeaderNavigation._pInstance.CloseHeader();
		Time.timeScale = 1f;
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (Input.anyKey || num >= 26f)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			Navigate(instance.GetScreenInstance<ScreenMap>());
			ScreenHeaderNavigation._pInstance.OpenHeader();
		}
	}
}
