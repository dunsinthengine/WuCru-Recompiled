public class ScreenCutsceneLevel1 : ScreenBase
{
	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		Navigate(instance.GetScreenInstance<ScreenSkills>());
	}

	protected override void Update()
	{
		base.Update();
	}
}
