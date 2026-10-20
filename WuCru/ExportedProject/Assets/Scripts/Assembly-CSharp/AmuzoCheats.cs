using System.Collections.Generic;
using UnityEngine;

public class AmuzoCheats : MonoBehaviour
{
	private void Awake()
	{
	}

	private void AddDebugMenu()
	{
		AmuzoDebugMenu menu = new AmuzoDebugMenu("NINJAGO CHEATS");
		menu.AddInfoTextFunction(TextInfo);
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Unlock All Levels", () =>
		{
			foreach (KeyValuePair<string, AreaMapMetaLayer> areaMap in GameController.Instance.areaMapDB.areaMaps)
			{
				int season = areaMap.Value.season;
				int num = areaMap.Value.rowArray.Length;
				int baseSeed = areaMap.Value.baseSeed;
				for (int i = 0; i < num; i++)
				{
					int num2 = areaMap.Value.rowArray[i].colArray.Length;
					for (int j = 0; j < num2; j++)
					{
						AreaMapNode areaMapNode = areaMap.Value.rowArray[i].colArray[j];
						int num3 = baseSeed + i + areaMap.Value.rowArray.Length * j;
						if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(num3) && !areaMapNode.isBlocked)
						{
							LevelUnlockData levelUnlockData = new LevelUnlockData
							{
								season = season,
								seed = num3
							};
							GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData.seed, levelUnlockData);
						}
					}
				}
			}
			int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(2);
			if (chapterCursor <= 0)
			{
				GameController.Instance.PlayerProfile.SetChapterCursor(2, 1);
			}
			GameController.Instance.PlayerProfile.Save();
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add 1 Stud", () =>
		{
			int studs = GameController.Instance.PlayerProfile.Studs;
			studs++;
			int level = PlayerProfile.LevelForXP(studs);
			GameController.Instance.PlayerProfile.Studs = studs;
			GameController.Instance.PlayerProfile.Level = level;
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add 10 Studs", () =>
		{
			int studs = GameController.Instance.PlayerProfile.Studs;
			studs += 10;
			int level = PlayerProfile.LevelForXP(studs);
			GameController.Instance.PlayerProfile.Studs = studs;
			GameController.Instance.PlayerProfile.Level = level;
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add 50 Studs", () =>
		{
			int studs = GameController.Instance.PlayerProfile.Studs;
			studs += 50;
			int level = PlayerProfile.LevelForXP(studs);
			GameController.Instance.PlayerProfile.Studs = studs;
			GameController.Instance.PlayerProfile.Level = level;
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add 100 Studs", () =>
		{
			int studs = GameController.Instance.PlayerProfile.Studs;
			studs += 100;
			int level = PlayerProfile.LevelForXP(studs);
			GameController.Instance.PlayerProfile.Studs = studs;
			GameController.Instance.PlayerProfile.Level = level;
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add 500 Studs", () =>
		{
			int studs = GameController.Instance.PlayerProfile.Studs;
			studs += 500;
			int level = PlayerProfile.LevelForXP(studs);
			GameController.Instance.PlayerProfile.Studs = studs;
			GameController.Instance.PlayerProfile.Level = level;
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add Crystals Zane", () =>
		{
			if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Zane, 0)))
			{
				int num = GameController.Instance.PlayerProfile.ZaneTokens + 1;
				GameController.Instance.PlayerProfile.ZaneLevel = Team.LevelForShards(num);
				GameController.Instance.PlayerProfile.ZaneTokens = num;
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add Crystals Kai", () =>
		{
			if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Kai, 0)))
			{
				int num = GameController.Instance.PlayerProfile.KaiTokens + 1;
				GameController.Instance.PlayerProfile.KaiLevel = Team.LevelForShards(num);
				GameController.Instance.PlayerProfile.KaiTokens = num;
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add Crystals Jay", () =>
		{
			if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Jay, 0)))
			{
				int num = GameController.Instance.PlayerProfile.JayTokens + 1;
				GameController.Instance.PlayerProfile.JayLevel = Team.LevelForShards(num);
				GameController.Instance.PlayerProfile.JayTokens = num;
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add Crystals Lloyd", () =>
		{
			if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Lloyd, 0)))
			{
				int num = GameController.Instance.PlayerProfile.LloydTokens + 1;
				GameController.Instance.PlayerProfile.LloydLevel = Team.LevelForShards(num);
				GameController.Instance.PlayerProfile.LloydTokens = num;
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add Crystals Nya", () =>
		{
			if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Nya, 0)))
			{
				int num = GameController.Instance.PlayerProfile.NyaTokens + 1;
				GameController.Instance.PlayerProfile.NyaLevel = Team.LevelForShards(num);
				GameController.Instance.PlayerProfile.NyaTokens = num;
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Add Crystals Cole", () =>
		{
			if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Cole, 0)))
			{
				int num = GameController.Instance.PlayerProfile.ColeTokens + 1;
				GameController.Instance.PlayerProfile.ColeLevel = Team.LevelForShards(num);
				GameController.Instance.PlayerProfile.ColeTokens = num;
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Win Current Level", () =>
		{
			if (GamePlayer.Instance != null)
			{
				GameController.Instance.GetComponent<GameControllerStatePlaying>().CompletionMgr.CheatComplete();
				GameController.Instance.MissionStats.ReportLevelCompleted();
				GameController.Instance.GetComponent<GameControllerStatePlaying>().OnCharacterReachEndFlag();
			}
			if (GameController.Instance.currentState is GameControllerStateTraversalGame)
			{
				StartCoroutine(Object.FindObjectOfType<TraversalController>().OnPlayerCompletedLevel(true));
			}
			menu.RefreshText();
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Unlock Vehicles", () =>
		{
			int num = UnlockableVehicleParts._pInstance._vehicleDefinitions.Length;
			for (int i = 0; i < num; i++)
			{
				UnlockableVehicleParts._pInstance.forceVehicleUnlocked(i);
			}
		}));
		menu.AddButton(new AmuzoDebugMenuButton(menu, () => "Toggle UI", () =>
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			Camera component = instance._mainUICamera.GetComponent<Camera>();
			component.enabled = !component.enabled;
		}));
		AmuzoDebugMenuManager.RegisterRootDebugMenu(menu);
	}

	private string TextInfo()
	{
		string text = ((PlayerProfile.xpForLevel(GameController.Instance.PlayerProfile.Level + 1) != 0) ? (" (Next Level=" + PlayerProfile.xpForLevel(GameController.Instance.PlayerProfile.Level + 1) + ")") : " [Max Level]");
		int num = Team.ShardsForLevel(GameController.Instance.PlayerProfile.ZaneLevel + 1);
		int num2 = Team.ShardsForLevel(GameController.Instance.PlayerProfile.KaiLevel + 1);
		int num3 = Team.ShardsForLevel(GameController.Instance.PlayerProfile.JayLevel + 1);
		int num4 = Team.ShardsForLevel(GameController.Instance.PlayerProfile.LloydLevel + 1);
		int num5 = Team.ShardsForLevel(GameController.Instance.PlayerProfile.NyaLevel + 1);
		int num6 = Team.ShardsForLevel(GameController.Instance.PlayerProfile.ColeLevel + 1);
		string text2 = ((num != -1) ? (" [Next Level:" + num + "]") : " [MAX]");
		string text3 = ((num2 != -1) ? ("  [Next Level:" + num2 + "]") : " [MAX]");
		string text4 = ((num3 != -1) ? ("  [Next Level:" + num3 + "]") : " [MAX]");
		string text5 = ((num4 != -1) ? ("  [Next Level:" + num4 + "]") : " [MAX]");
		string text6 = ((num5 != -1) ? ("  [Next Level:" + num5 + "]") : " [MAX]");
		string text7 = ((num6 != -1) ? ("  [Next Level:" + num6 + "]") : " [MAX]");
		return "Player Level: " + GameController.Instance.PlayerProfile.Level + " XP/Studs: " + GameController.Instance.PlayerProfile.Studs + text + " \nZane Level: " + GameController.Instance.PlayerProfile.ZaneLevel + " Crystals: " + GameController.Instance.PlayerProfile.ZaneTokens + text2 + "\nKai Level: " + GameController.Instance.PlayerProfile.KaiLevel + " Crystals: " + GameController.Instance.PlayerProfile.KaiTokens + text3 + "\nJay Level: " + GameController.Instance.PlayerProfile.JayLevel + " Crystals: " + GameController.Instance.PlayerProfile.JayTokens + text4 + "\nLloyd Level: " + GameController.Instance.PlayerProfile.LloydLevel + " Crystals: " + GameController.Instance.PlayerProfile.LloydTokens + text5 + "\nNya Level: " + GameController.Instance.PlayerProfile.NyaLevel + " Crystals: " + GameController.Instance.PlayerProfile.NyaTokens + text6 + "\nCole Level: " + GameController.Instance.PlayerProfile.ColeLevel + " Crystals: " + GameController.Instance.PlayerProfile.ColeTokens + text7 + "\n\n\nNote: To use ninja crystal cheats, unlock the ninja first";
	}
}
