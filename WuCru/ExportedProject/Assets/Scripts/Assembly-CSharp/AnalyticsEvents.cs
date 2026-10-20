using System.Collections.Generic;
using UnityEngine;

public static class AnalyticsEvents
{
	private const string StartLevel = "StartLevel";

	private const string StartLevelTimed = "StartLevelTimed";

	private const string EndLevel = "EndLevel";

	private const string LevelResult = "LevelResult";

	private const string CharacterCreated = "CharacterCreated";

	private const string CrystalSmash = "CrystalSmash";

	private const string ChestOpen = "ChestOpen";

	private const string TrapDismantling = "TrapDismantling";

	private const string DistractionTriggering = "DistractionTriggering";

	private const string SpinjitzuUse = "SpinjitzuUse";

	private const string StartCombatTimed = "StartCombatTimed";

	private const string LoadSceneName = "LoadScene";

	private const string LevelUp = "LevelUp";

	private const string BlueprintCollected = "BlueprintCollected";

	private const string VehicleUnlocked = "VehicleUnlocked";

	private const string VehicleSelected = "VehicleSelected";

	private const string StoryViewerStart = "StoryViewerStart";

	private const string StoryViewerEnd = "StoryViewerEnd";

	private const string PlayerDied = "PlayerDied";

	private const string EnemyDied = "EnemyDied";

	private const string WuCruMissionEnded = "WuCruMissionEnded";

	private const string ChallengeEnded = "ChallengeEnded";

	private const string ObjectiveCompleted = "ObjectiveCompleted";

	private const string LegoIdLoginWindowOpened = "LegoIdLoginWindowOpened";

	private const string LegoIdLogin = "LegoIdLogin";

	public static void LogStartLevel(int sequence)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Seed, GameController.Instance.seed.ToString());
		dictionary.Add(AnalyticsVars.Style, GameController.Instance.styleDB._styles[GameController.Instance.styleIndex].ToString());
		dictionary.Add(AnalyticsVars.Theme, GameController.Instance.themeDB.themeCollections.values[GameController.Instance.themeCollectionIndex].ToString());
		dictionary.Add(AnalyticsVars.Sequence, sequence.ToString());
	}

	public static void LogEndLevel(bool result)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.LevelCompletionResult, result.ToString());
	}

	public static void LogLevelResult(int seed, int numChests, int numChestsCollected, int numCrystalShards, int numCrystalShardsCollected, int numStars, int numEnemies, int numEnemiesKilled, int numDeadNinjas, int numStudsCollected, int numChallenges, int numChallengesCompleted)
	{
		Debug.LogFormat("[AnalyticsEvents.LogLevelResult] Seed : {0}, NumChests : {1}, NumChestsCollected : {2}", seed, numChests, numChestsCollected);
		Debug.LogFormat("[AnalyticsEvents.LogLevelResult] NumCrystalShards : {0}, NumCrystalShardsCollected : {1}", numCrystalShards, numCrystalShardsCollected);
		Debug.LogFormat("[AnalyticsEvents.LogLevelResult] NumStars : {0}, NumEnemies : {1}, NumEnemiesKilled : {2}, NumDeadNinjas : {3}", numStars, numEnemies, numEnemiesKilled, numDeadNinjas);
		Debug.LogFormat("[AnalyticsEvents.LogLevelResult] NumStudsCollected : {0}, NumChallenges : {1}, NumChallengesCompleted : {2}", numStudsCollected, numChallenges, numChallengesCompleted);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Seed, seed.ToString());
		dictionary.Add(AnalyticsVars.NumChests, numChests.ToString());
		dictionary.Add(AnalyticsVars.NumChestsCollected, numChestsCollected.ToString());
		dictionary.Add(AnalyticsVars.NumCrystalShards, numCrystalShards.ToString());
		dictionary.Add(AnalyticsVars.NumCrystalShardsCollected, numCrystalShardsCollected.ToString());
		dictionary.Add(AnalyticsVars.NumStars, numStars.ToString());
		dictionary.Add(AnalyticsVars.NumEnemies, numEnemies.ToString());
		dictionary.Add(AnalyticsVars.NumEnemiesKilled, numEnemiesKilled.ToString());
		dictionary.Add(AnalyticsVars.NumDeadNinjas, numDeadNinjas.ToString());
		dictionary.Add(AnalyticsVars.NumStudsCollected, numStudsCollected.ToString());
		dictionary.Add(AnalyticsVars.NumChallenges, numChallenges.ToString());
		dictionary.Add(AnalyticsVars.NumChallengesCompleted, numChallengesCompleted.ToString());
	}

	public static void LogCharacterCreation()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		dictionary.Add(AnalyticsVars.HairModel, playerProfile.HairModel);
		string value = ColorTypeConverter.RgbaToHex(playerProfile.HairColor).ToString();
		dictionary.Add(AnalyticsVars.HairColor, value);
		dictionary.Add(AnalyticsVars.FaceTexture, playerProfile.FaceTexture);
		dictionary.Add(AnalyticsVars.TorsoTexture, playerProfile.TorsoTexture);
		dictionary.Add(AnalyticsVars.PelvisTexture, playerProfile.PelvisTexture);
		dictionary.Add(AnalyticsVars.LegsTexture, playerProfile.LegsTexture);
		string text = ColorTypeConverter.RgbaToHex(playerProfile.TorsoColor).ToString();
		dictionary.Add(AnalyticsVars.SuitColor, playerProfile.TorsoColor.ToString());
		dictionary.Add(AnalyticsVars.HelmetModel, playerProfile.HelmetModel);
		string value2 = ColorTypeConverter.RgbaToHex(playerProfile.HelmetColor).ToString();
		dictionary.Add(AnalyticsVars.HelmetColor, value2);
		dictionary.Add(AnalyticsVars.Weapon, playerProfile.Weapon.ToString());
		dictionary.Add(AnalyticsVars.ElementalPower, playerProfile.ElementalPower.ToString());
	}

	public static void LogCrystalSmash(bool result)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.CrystalResult, result.ToString());
	}

	public static void LogChestOpen(bool result)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.ChestResult, result.ToString());
	}

	public static void LogTrapDismantling(bool result)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.TrapResult, result.ToString());
	}

	public static void LogDistrationTriggering(bool result)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.DistractionResult, result.ToString());
	}

	public static void LogSpinjitzuUse()
	{
	}

	public static void LogStartCombat()
	{
	}

	public static void LogEndCombat()
	{
	}

	public static void LogLoadScene(string sceneName)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.SceneName, sceneName);
	}

	public static void LogLevelUp(int level)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Level, level.ToString());
	}

	public static void LogVehicleUnlocked(int vehicleID)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.VehicleID, vehicleID.ToString());
	}

	public static void LogVehicleSelected(int vehicleID)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.VehicleID, vehicleID.ToString());
	}

	public static void LogStoryViewerStart(int season, int chapter)
	{
		Debug.LogFormat("[AnalyticsEvents.LogStoryViewerStart] Season : {0}, Chapter : {1}", season, chapter);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Season, season.ToString());
		dictionary.Add(AnalyticsVars.Chapter, chapter.ToString());
	}

	public static void LogStoryViewerEnd(int season, int chapter, bool skipped)
	{
		Debug.LogFormat("[AnalyticsEvents.LogStoryViewerEnd] Season : {0}, Chapter : {1}, Skipped : {2}", season, chapter, skipped);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Season, season.ToString());
		dictionary.Add(AnalyticsVars.Chapter, chapter.ToString());
		dictionary.Add(AnalyticsVars.Skipped, skipped.ToString());
	}

	public static void LogPlayerDied(AIAgentType agentType, DamageType damageType)
	{
		Debug.LogFormat("[AnalyticsEvents.LogPlayerDied] Agent Type : {0}, Damage Type : {1}", agentType, damageType);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.AgentType, agentType.ToString());
		dictionary.Add(AnalyticsVars.DamageType, damageType.ToString());
	}

	public static void LogEnemyDied(DamageType damageType)
	{
		Debug.LogFormat("[AnalyticsEvents.LogEnemyDied] Damage Type : {0}", damageType);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.DamageType, damageType.ToString());
	}

	public static void LogChallengeEnded(string type, bool completed, int seed)
	{
		Debug.LogFormat("[AnalyticsEvents.LogChallengeEnded] Type : {0}, Completed : {1}, Seed : {2}", type, completed, seed);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Type, type.ToString());
		dictionary.Add(AnalyticsVars.Completed, completed.ToString());
		dictionary.Add(AnalyticsVars.Seed, seed.ToString());
	}

	public static void LogWuCruMissionEnded(string id, bool completed, int seed)
	{
		Debug.LogFormat("[AnalyticsEvents.LogWuCruMissionEnded] Id : {0}, Completed : {1}, Seed : {2}", id, completed, seed);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Id, id.ToString());
		dictionary.Add(AnalyticsVars.Completed, completed.ToString());
		dictionary.Add(AnalyticsVars.Seed, seed.ToString());
	}

	public static void LogObjectiveCompleted(int season, int chapter)
	{
		Debug.LogFormat("[AnalyticsEvents.LogObjectiveCompleted] Season : {0}, Chapter : {1}", season, chapter);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.Season, season.ToString());
		dictionary.Add(AnalyticsVars.Chapter, chapter.ToString());
	}

	public static void LogLegoIdwindowOpened()
	{
		Debug.Log("[AnalyticsEvents.LogLegoIdwindowOpened]");
	}

	public static void LogLegoIdLogin(bool success)
	{
		Debug.Log("[AnalyticsEvents.LegoIdLogin. Success: " + success + "]");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(AnalyticsVars.LoggedIn, success.ToString());
	}
}
