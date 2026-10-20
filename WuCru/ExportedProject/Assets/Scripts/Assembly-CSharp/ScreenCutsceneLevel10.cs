using UnityEngine;

public class ScreenCutsceneLevel10 : ScreenBase
{
	private const float CUTSCENE_DUR = 31f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		Time.timeScale = 1f;
		AudioPlayer.PlaySound("cutscenes/level10");
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (Input.anyKey || num >= 31f)
		{
			AudioPlayer.StopSound("cutscenes/level10");
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
