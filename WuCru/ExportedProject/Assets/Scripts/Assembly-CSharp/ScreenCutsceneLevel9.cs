public class ScreenCutsceneLevel9 : ScreenBase
{
	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (playerProfile.AvailableSkillPoints > 0)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			Navigate(instance.GetScreenInstance<ScreenSkills>());
			return;
		}
		ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
		if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap())
		{
			ScreenWorldMap screenInstance = instance2.GetScreenInstance<ScreenWorldMap>();
			Navigate(screenInstance);
		}
		else
		{
			ScreenMap screenInstance2 = instance2.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance2);
		}
	}
}
