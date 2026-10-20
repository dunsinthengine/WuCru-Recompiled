using UnityEngine;

public class ScreenCutsceneVermillion : ScreenBase
{
	protected override void Update()
	{
		base.Update();
		if (Input.anyKey)
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
		}
	}
}
