using UnityEngine;

public class ScreenCutsceneIntro : ScreenBase
{
	private const float CUTSCENE_DUR = 25f;

	private float _st;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_st = RealTime.time;
		Time.timeScale = 1f;
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
		AudioPlayer.PlaySound("cutscenes/intro");
	}

	protected override void Update()
	{
		base.Update();
		float num = RealTime.time - _st;
		if (Input.anyKey || num >= 25f)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenLevelLoading screenInstance = instance.GetScreenInstance<ScreenLevelLoading>();
			Navigate(screenInstance);
			AudioPlayer.StopSound("cutscenes/intro");
			GameController.Instance.PlayerProfile.LastPlayedSeason = GameController.Instance.areaMapMetaLayer.season;
			int seedFromPosition = GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(GameController.Instance.areaMapMetaLayer.startRow, GameController.Instance.areaMapMetaLayer.startCol);
			if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(seedFromPosition))
			{
				LevelUnlockData levelUnlockData = new LevelUnlockData();
				levelUnlockData.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
				levelUnlockData.seed = seedFromPosition;
				Debug.Log("<color=yellow>Level Unlock" + seedFromPosition + "</color>");
				GameController.Instance.PlayerProfile.LevelUnlocked.Add(seedFromPosition, levelUnlockData);
				GameController.Instance.seed = 1049;
				GameController.Instance.styleIndex = 5;
				GameController.Instance.themeCollectionIndex = 4;
				GameAudioManager.Instance.StopAllMusic(false);
				GameAudioManager.Instance.StopAmbience(false);
			}
		}
	}
}
