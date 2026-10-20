public class ScreenCutsceneLevel6 : ScreenBase
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
		GlobalInGameData.LaunchHandsOfTimeCampaign();
		ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
		ScreenCutsceneHandsOfTimeIntro screenInstance = instance2.GetScreenInstance<ScreenCutsceneHandsOfTimeIntro>();
		Navigate(screenInstance);
	}
}
