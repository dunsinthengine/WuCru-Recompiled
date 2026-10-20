using UnityEngine;

public class ScreenCutsceneHandsOfTimeIntro : ScreenBase
{
	private float _delay = 1f;

	private const float CUTSCENE_DUR = 32f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		Time.timeScale = 1f;
		_delay = 1f;
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
		AudioPlayer.PlaySound("cutscenes/intro_hot");
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if ((Input.anyKey && _delay <= 0f) || num >= 32f)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			Navigate(instance.GetScreenInstance<ScreenWorldMap>());
			AudioPlayer.StopSound("cutscenes/intro_hot");
		}
		_delay -= RealTime.deltaTime;
	}
}
