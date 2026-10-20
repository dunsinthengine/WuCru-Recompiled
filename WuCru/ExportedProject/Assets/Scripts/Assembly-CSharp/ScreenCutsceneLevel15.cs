using UnityEngine;

public class ScreenCutsceneLevel15 : ScreenBase
{
	private const float CUTSCENE_DUR = 64f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		Time.timeScale = 1f;
		AudioPlayer.PlaySound("cutscenes/vanillafinal");
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (Input.anyKey || num >= 64f)
		{
			AudioPlayer.StopSound("cutscenes/vanillafinal");
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
