public class ScreenCutsceneLevel11 : ScreenBase
{
	protected override void OnShowScreen()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (playerProfile.AvailableSkillPoints > 0)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			Navigate(instance.GetScreenInstance<ScreenSkills>());
		}
		else
		{
			ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
			Navigate(instance2.GetScreenInstance<ScreenMap>());
		}
		base.OnShowScreen();
	}
}
