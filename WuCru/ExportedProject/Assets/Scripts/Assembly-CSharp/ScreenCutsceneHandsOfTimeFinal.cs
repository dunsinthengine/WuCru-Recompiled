using UnityEngine;

public class ScreenCutsceneHandsOfTimeFinal : ScreenBase
{
	private const float CUTSCENE_DUR = 27.5f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		AudioPlayer.PlaySound("cutscenes/hotfinal");
		Time.timeScale = 1f;
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (Input.anyKey || num >= 27.5f)
		{
			AudioPlayer.StopSound("cutscenes/hotfinal");
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
