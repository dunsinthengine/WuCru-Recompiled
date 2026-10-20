public class ScreenCutsceneBlade2 : ScreenBase
{
	protected override void OnShowScreen()
	{
		base.OnShowScreen();
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
	}
}
