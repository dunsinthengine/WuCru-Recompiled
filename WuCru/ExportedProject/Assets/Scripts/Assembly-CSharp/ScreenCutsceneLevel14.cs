using UnityEngine;

public class ScreenCutsceneLevel14 : ScreenBase
{
	private const float CUTSCENE_DUR = 30f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		Time.timeScale = 1f;
		AudioPlayer.PlaySound("cutscenes/level14");
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (Input.anyKey || num >= 30f)
		{
			AudioPlayer.StopSound("cutscenes/level14");
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
