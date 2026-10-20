using UnityEngine;

public class GlobalInGameData
{
	public static bool DO_ENABLE_ARCADE = true;

	public const string SAVEGAME_FILENAME = "savegame.txt";

	public static bool ShouldLaunchHandsOfTimeCampaign()
	{
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		int chapterCursor2 = GameController.Instance.PlayerProfile.GetChapterCursor(2);
		bool flag = GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.IsCurrentObjectiveCompleted();
		return chapterCursor >= 6 && chapterCursor2 == -1 && flag;
	}

	public static bool ShouldLaunchMovieCampaign()
	{
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(3);
		Debug.Log("<color=green>Season 3 Chapter Cursor: " + chapterCursor + "</color>");
		return chapterCursor == -1;
	}

	public static void LaunchHandsOfTimeCampaign()
	{
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		if (chapterCursor >= 6)
		{
			ScreenMap.UpdateCompletedObjectives(true);
		}
		Debug.Log("<color=green>Launch Hands Of Time</color>");
		GameController.Instance.PlayerProfile.SetChapterCursor(2, 1);
	}

	public static void LaunchMovieCampaign()
	{
		Debug.Log("Launch Movie Campaign");
		GameController.Instance.PlayerProfile.SetChapterCursor(3, 1);
	}

	public static void SaveState(int studsToRemove)
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		playerProfile.Studs += studsToRemove;
		playerProfile.TotalLevelsPlayed++;
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Kai, 0)))
		{
			playerProfile.KaiLevel = Team.LevelForShards(playerProfile.KaiTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Jay, 0)))
		{
			playerProfile.JayLevel = Team.LevelForShards(playerProfile.JayTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Cole, 0)))
		{
			playerProfile.ColeLevel = Team.LevelForShards(playerProfile.ColeTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Zane, 0)))
		{
			playerProfile.ZaneLevel = Team.LevelForShards(playerProfile.ZaneTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Nya, 0)))
		{
			playerProfile.NyaLevel = Team.LevelForShards(playerProfile.NyaTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Lloyd, 0)))
		{
			playerProfile.LloydLevel = Team.LevelForShards(playerProfile.LloydTokens);
		}
		for (int i = 1; i < playerProfile.NinjaTeamSlotCount; i++)
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam < 0)
			{
				continue;
			}
			MinifigType typeForID = Team.GetTypeForID(ninjaInTeam);
			if (typeForID != MinifigType.None)
			{
				int ninjaLevel = playerProfile.GetNinjaLevel(typeForID);
				int num = Team.GetIDsForType(typeForID)[ninjaLevel];
				playerProfile.SetNinjaInTeam(num, i);
				if (!playerProfile.UnlockedNinjaSKUs.Contains(num))
				{
					playerProfile.UnlockedNinjaSKUs.Add(num);
				}
			}
		}
		bool flag = GameController.Instance.MissionStats.NumChestsToCollect - GameController.Instance.MissionStats.NumChestsCollected <= 0 && GameController.Instance.MissionStats.NumCrystalsToCollect - GameController.Instance.MissionStats.NumCrystalsCollected <= 0;
		bool flag2 = true;
		foreach (LevelChallengeBase key in GameController.Instance.MissionStats.Challenges.Keys)
		{
			if (!GameController.Instance.MissionStats.Challenges[key])
			{
				flag2 = false;
			}
		}
		LevelUnlockData levelUnlockData = new LevelUnlockData();
		if (GameController.Instance == null)
		{
			Debug.LogError("Null GC");
		}
		else if (GameController.Instance.CurrentLevel == null)
		{
			Debug.LogError("Null Current Level");
		}
		levelUnlockData.seed = GameController.Instance.CurrentLevel.Seed;
		levelUnlockData.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
		Debug.Log("LAST PLAYED SEASON: " + levelUnlockData.season);
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(levelUnlockData.seed))
		{
			flag3 = GameController.Instance.PlayerProfile.LevelUnlocked[levelUnlockData.seed].starCompletion;
			flag4 = GameController.Instance.PlayerProfile.LevelUnlocked[levelUnlockData.seed].starCompletion;
			flag5 = GameController.Instance.PlayerProfile.LevelUnlocked[levelUnlockData.seed].starCompletion;
		}
		levelUnlockData.starCompletion = true;
		levelUnlockData.starObjects = flag | flag4;
		levelUnlockData.starChallenges = flag2 | flag5;
		int num2 = 0;
		if (levelUnlockData.starCompletion)
		{
			num2++;
		}
		if (levelUnlockData.starObjects)
		{
			num2++;
		}
		if (levelUnlockData.starChallenges)
		{
			num2++;
		}
		GameController.Instance.LevelExtensionController.HandleLevelComplete(levelUnlockData.seed);
		GameController.Instance.areaMapMetaLayer.UnlockAfterPlay(levelUnlockData);
		GameController.Instance.PlayerProfile.Save();
	}
}
