using UnityEngine;

public class ScreenCutsceneLevel7 : ScreenBase
{
	private const float CUTSCENE_DUR = 26f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		AudioPlayer.PlaySound("cutscenes/level7");
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (!Input.anyKey && !(num >= 26f))
		{
			return;
		}
		AudioPlayer.StopSound("cutscenes/level7");
		UnlockC2();
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

	private void UnlockC2()
	{
		GameController.Instance.PlayerProfile.UnlockArea("S1A2");
		AreaMapMetaLayer areaMapMetaLayer = GameController.Instance.areaMapDB.areaMaps["S1A2"];
		int startCol = areaMapMetaLayer.startCol;
		int startRow = areaMapMetaLayer.startRow;
		int baseSeed = areaMapMetaLayer.baseSeed;
		AreaMapNode areaMapNode = areaMapMetaLayer.rowArray[startRow].colArray[startCol];
		int num = baseSeed + startRow + areaMapMetaLayer.rowArray.Length * startCol;
		if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(num) && !areaMapNode.isBlocked)
		{
			LevelUnlockData levelUnlockData = new LevelUnlockData();
			levelUnlockData.season = areaMapMetaLayer.season;
			levelUnlockData.seed = num;
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData.seed, levelUnlockData);
			Debug.Log("<color=yellow>Level Unlock" + num + "</color>");
		}
	}
}
