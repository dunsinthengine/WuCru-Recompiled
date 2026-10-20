using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
	public Canvas canvas;

	public Button buttonResetProgress;

	public Button buttonClose;

	public Button buttonKillAll;

	public Button buttonWinLevel;

	public Button buttonNextMap;

	public Button buttonRestartMap;

	public Button buttonLowQuality;

	public Button buttonHighQuality;

	public Button buttonHalfScrenResolution;

	public Button buttonUnlockAllLevels;

	public Button buttonShowDebugText;

	public Button buttonUnlockAllNinjas;

	public Button buttonUnlockLloyd;

	public Button buttonUnlockBiteBike;

	public Button buttonToggleAIPaths;

	public Button buttonNextChapter;

	public Button buttonLevelUpCheat;

	public Button buttonSkipTutorial;

	public Button buttonQuit;

	public Button buttonLogin;

	public Button buttonLogout;

	public Button buttonCompleteMission;

	public Button buttonUnlockMissions;

	public Button buttonSkipFastPlay;

	public InputField inputFieldProfileStuds;

	public InputField inputFieldProfileLevel;

	public InputField inputFieldProfileWeaponLevel;

	public InputField inputFieldProfileSpinjitzuLevel;

	public InputField inputFieldBlueprintCount;

	public InputField inputFieldTargetFPS;

	public Button buttonSaveProfile;

	public Button buttonDOTDProgress;

	public Button buttonDOTDComplete;

	public Button buttonDOTDKillStoneWarriors;

	private void Awake()
	{
		buttonResetProgress.onClick.AddListener(() =>
		{
			PlayerProfile.Reset();
			GameController.Instance.ResetPlayerProfile();
			Application.Quit();
		});
		buttonClose.onClick.AddListener(() =>
		{
			Object.Destroy(base.gameObject);
		});
		buttonWinLevel.onClick.AddListener(() =>
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
			Object.Destroy(base.gameObject);
		});
		buttonNextMap.onClick.AddListener(() =>
		{
			if (GamePlayer.Instance != null)
			{
				GameController.Instance.MissionStats.ReportLevelCompleted();
				GameController.Instance.GetComponent<GameControllerStatePlaying>().OnCharacterReachEndFlag();
			}
		});
		buttonKillAll.onClick.AddListener(() =>
		{
			if (GameController.Instance != null)
			{
				AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
				foreach (AIAgent aIAgent in agents)
				{
					aIAgent.Character.TakeDamage(aIAgent.Character.Health + 10, aIAgent.Character.transform.position, DamageType.Explosion);
				}
			}
			Object.Destroy(base.gameObject);
		});
		buttonRestartMap.onClick.AddListener(() =>
		{
			if (GamePlayer.Instance != null)
			{
				GamePlayer.Instance.Player.Character.Respawn();
			}
		});
		buttonLowQuality.onClick.AddListener(() =>
		{
			QualitySettings.SetQualityLevel(0);
			buttonLowQuality.GetComponentInChildren<Text>().text = "Quality level: " + QualitySettings.GetQualityLevel();
		});
		buttonHighQuality.onClick.AddListener(() =>
		{
		});
		buttonUnlockAllLevels.onClick.AddListener(() =>
		{
			GameController instance = GameController.Instance;
			if (instance != null)
			{
				for (int i = 0; i < instance.areaMapMetaLayer.rowArray.Length; i++)
				{
					for (int j = 0; j < instance.areaMapMetaLayer.rowArray[0].colArray.Length; j++)
					{
						if (!instance.areaMapMetaLayer.IsBlocked(i, j) && instance.areaMapMetaLayer.GetNodeAt(i, j).lockUntilChapter <= instance.PlayerProfile.GetChapterCursor(instance.areaMapMetaLayer.season))
						{
							int seedFromPosition = instance.areaMapMetaLayer.GetSeedFromPosition(i, j);
							if (!instance.PlayerProfile.LevelUnlocked.ContainsKey(seedFromPosition))
							{
								LevelUnlockData value = new LevelUnlockData
								{
									season = instance.PlayerProfile.LastPlayedSeason,
									seed = seedFromPosition
								};
								instance.PlayerProfile.LevelUnlocked.Add(seedFromPosition, value);
								Debug.Log("<color=yellow>Level Unlock" + seedFromPosition + "</color>");
							}
						}
					}
				}
			}
		});
		buttonHalfScrenResolution.onClick.AddListener(() =>
		{
			Screen.SetResolution(960, 600, true);
		});
		buttonSaveProfile.onClick.AddListener(() =>
		{
			if (inputFieldProfileStuds.text.Length > 0)
			{
				int studs = int.Parse(inputFieldProfileStuds.text);
				GameController.Instance.PlayerProfile.Studs = studs;
			}
			if (inputFieldProfileLevel.text.Length > 0)
			{
				int level = int.Parse(inputFieldProfileLevel.text);
				GameController.Instance.PlayerProfile.Level = level;
			}
			if (inputFieldProfileWeaponLevel.text.Length > 0)
			{
				int weaponLevel = int.Parse(inputFieldProfileWeaponLevel.text);
				GameController.Instance.PlayerProfile.WeaponLevel = weaponLevel;
			}
			if (inputFieldProfileSpinjitzuLevel.text.Length > 0)
			{
				int spinjitzuLevel = int.Parse(inputFieldProfileSpinjitzuLevel.text);
				GameController.Instance.PlayerProfile.SpinjitzuLevel = spinjitzuLevel;
			}
			if (inputFieldBlueprintCount.text.Length > 0)
			{
			}
			GameController.Instance.PlayerProfile.Save();
		});
		buttonShowDebugText.onClick.AddListener(() =>
		{
			GameController.Instance.debugTextOnCharacters = !GameController.Instance.debugTextOnCharacters;
		});
		buttonUnlockAllNinjas.onClick.AddListener(() =>
		{
			Debug.Log("Unlocking all ninjas");
			int[] array = new int[6]
			{
				Team.GetID(MinifigType.Cole, 0),
				Team.GetID(MinifigType.Nya, 0),
				Team.GetID(MinifigType.Jay, 0),
				Team.GetID(MinifigType.Kai, 0),
				Team.GetID(MinifigType.Zane, 0),
				Team.GetID(MinifigType.Lloyd, 0)
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (!GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(array[i]))
				{
					GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Add(array[i]);
				}
				GameController.Instance.PlayerProfile.SetNinjaInTeam(array[i]);
			}
		});
		buttonUnlockLloyd.onClick.AddListener(() =>
		{
			int iD = Team.GetID(MinifigType.Lloyd, 0);
			if (!GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(iD))
			{
				GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Add(iD);
			}
			GameController.Instance.PlayerProfile.SetNinjaInTeam(iD);
		});
		buttonUnlockBiteBike.onClick.AddListener(() =>
		{
		});
		buttonToggleAIPaths.onClick.AddListener(() =>
		{
			GamePlayer.Instance.ShowAIPaths = !GamePlayer.Instance.ShowAIPaths;
			GameObject[] array = GameObject.FindGameObjectsWithTag("AIPath");
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(GamePlayer.Instance.ShowAIPaths);
			}
		});
		buttonNextChapter.onClick.AddListener(() =>
		{
			if (GameController.Instance.NumChapters(GameController.Instance.PlayerProfile.LastPlayedSeason) > GameController.Instance.PlayerProfile.GetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason))
			{
				if (GameController.Instance.ObjectiveController.CurrentObjective == null)
				{
					GameController.Instance.ObjectiveController.InitiateObjective(GameController.Instance.PlayerProfile.LastPlayedSeason, GameController.Instance.PlayerProfile.GetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason), true);
				}
				LevelCompletionManager levelCompletionManager = base.gameObject.AddComponent<LevelCompletionManager>();
				levelCompletionManager.Initialize();
				levelCompletionManager.CheatComplete();
				GameController.Instance.PlayerProfile.SetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason, GameController.Instance.PlayerProfile.GetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason) + 1);
				buttonNextChapter.gameObject.GetComponentInChildren<Text>().text = "Next Chapter (current: " + GameController.Instance.PlayerProfile.GetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason) + ")";
				GameController.Instance.PlayerProfile.Save();
				Object.Destroy(levelCompletionManager);
				GameController.Instance.ObjectiveController.InitiateObjective(GameController.Instance.PlayerProfile.LastPlayedSeason, GameController.Instance.PlayerProfile.GetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason), true);
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		});
		buttonLevelUpCheat.onClick.AddListener(() =>
		{
			NinjaScreenController.OpeningPage = NinjaScreenOpeningPage.NinjaStats;
			GameController.Instance.PlayerProfile.Level++;
			GameController.Instance.PlayerProfile.Studs = PlayerProfile.xpForLevel(GameController.Instance.PlayerProfile.Level);
			GameController.Instance.PlayerProfile.Save();
		});
		buttonSkipTutorial.onClick.AddListener(() =>
		{
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.EquipCruMember);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.UseSkillPoint);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.Duelling);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.PlayerAttack);
			GameController.Instance.PlayerProfile.TotalLevelsPlayed++;
			GameController.Instance.PlayerProfile.Save();
		});
		buttonQuit.onClick.AddListener(() =>
		{
			Application.Quit();
		});
		buttonLogin.onClick.AddListener(() =>
		{
		});
		buttonLogout.onClick.AddListener(() =>
		{
		});
		buttonCompleteMission.onClick.AddListener(() =>
		{
			GameController.Instance.MissionController.CompleteMission("{ED8A5593-039B-490B-A525-893FAECEB95A}");
		});
		buttonNextChapter.gameObject.GetComponentInChildren<Text>().text = "Next Chapter (current: " + GameController.Instance.PlayerProfile.GetChapterCursor(GameController.Instance.PlayerProfile.LastPlayedSeason) + ")";
		if (Application.targetFrameRate > 0)
		{
			inputFieldTargetFPS.text = Application.targetFrameRate.ToString();
		}
		inputFieldTargetFPS.onEndEdit.AddListener((string stringValue) =>
		{
			int result;
			if (!int.TryParse(stringValue, out result))
			{
				result = 0;
				inputFieldTargetFPS.text = string.Empty;
			}
			Application.targetFrameRate = result;
		});
		buttonDOTDProgress.onClick.AddListener(() =>
		{
			if (GameController.Instance != null)
			{
				LevelExtension levelExtension = GameController.Instance.levelExtensionController.GetLevelExtension(GameController.Instance.Seed);
				GameObject enemyDropItem = levelExtension.enemyDropItem;
				PlayerController playerController = Object.FindObjectOfType<PlayerController>();
				LevelCompletionManager levelCompletionManager = Object.FindObjectOfType<LevelCompletionManager>();
				levelCompletionManager.HandleStoneWarriorDeath(playerController.Character);
				Object.Destroy(base.gameObject);
			}
		});
		buttonDOTDComplete.onClick.AddListener(() =>
		{
			if (GameController.Instance != null)
			{
				LevelCompletionManager levelCompletionManager = Object.FindObjectOfType<LevelCompletionManager>();
				LevelExtensionController.LevelExtensionSettings levelExtensionSettings = GameController.Instance.LevelExtensionController.GetLevelExtensionSettings("DayOfTheDeparted");
				levelExtensionSettings.collected = levelExtensionSettings.total - 1;
				levelCompletionManager.HandleStoneWarriorDeath();
			}
			Object.Destroy(base.gameObject);
		});
		buttonDOTDKillStoneWarriors.onClick.AddListener(() =>
		{
			if (GameController.Instance != null)
			{
				AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
				foreach (AIAgent aIAgent in agents)
				{
					if (aIAgent.GetAgentType() == AIAgentType.StoneWarrior)
					{
						aIAgent.Character.TakeDamage(aIAgent.Character.Health + 10, aIAgent.Character.transform.position, DamageType.Explosion);
					}
				}
			}
		});
		buttonSkipFastPlay.onClick.AddListener(() =>
		{
			GameController.Instance.PlayerProfile.SetChapterCursor(1, 3);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.EquipCruMember);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.UseSkillPoint);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.Duelling);
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.PlayerAttack);
			int[] array = new int[6] { 1033, 1040, 1041, 1042, 1049, 1050 };
			LevelUnlockData levelUnlockData = new LevelUnlockData
			{
				season = 1
			};
			for (int i = 0; i < array.Length; i++)
			{
				levelUnlockData.seed = array[i];
				levelUnlockData.starCompletion = true;
				if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(levelUnlockData.seed))
				{
					GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData.seed, levelUnlockData);
					Debug.Log("<color=yellow>Level Unlock" + levelUnlockData.seed + "</color>");
				}
			}
			GameController.Instance.PlayerProfile.TotalLevelsPlayed = 2;
			GameController.Instance.PlayerProfile.UnlockNinja(Team.GetID(MinifigType.Zane, 0));
			GameController.Instance.PlayerProfile.Level = 2;
			if (GameController.Instance.ObjectiveController.CurrentObjective == null)
			{
				GameController.Instance.ObjectiveController.InitiateObjective(1, 3, true);
			}
			if (GameController.Instance.PlayerProfile.FaceTexture == null)
			{
				GameController.Instance.PlayerProfile.FaceTexture = "Face_Male_GenB_A_0001";
				GameController.Instance.PlayerProfile.LegsTexture = "70504_Zane Kimono_Legs";
				GameController.Instance.PlayerProfile.PelvisTexture = "70504_Zane Kimono_Pelvis";
				GameController.Instance.PlayerProfile.TorsoTexture = "70504_Zane Kimono_Torso";
				GameController.Instance.PlayerProfile.HeadModel = string.Empty;
				GameController.Instance.PlayerProfile.HairModel = "Equipment/Hair_10048";
				GameController.Instance.PlayerProfile.HelmetModel = "Equipment/FullMask";
				GameController.Instance.PlayerProfile.HelmetHidesHair = true;
				GameController.Instance.PlayerProfile.Weapon = WeaponType.Shuriken;
				GameController.Instance.PlayerProfile.ElementalPower = ElementalPower.Energy;
				GameController.Instance.PlayerProfile.HeadColor = new Color32(byte.MaxValue, 198, 0, 0);
				GameController.Instance.PlayerProfile.HandColor = new Color32(17, 17, 17, 0);
				GameController.Instance.PlayerProfile.ArmColor = new Color32(54, 54, 54, byte.MaxValue);
				GameController.Instance.PlayerProfile.LegColor = new Color32(54, 54, 54, byte.MaxValue);
				GameController.Instance.PlayerProfile.TorsoColor = new Color32(54, 54, 54, byte.MaxValue);
				GameController.Instance.PlayerProfile.PelvisColor = new Color32(54, 54, 54, byte.MaxValue);
				GameController.Instance.PlayerProfile.QuiverColor = new Color32(30, 30, 30, 0);
				GameController.Instance.PlayerProfile.HelmetColor = new Color32(203, 40, 40, byte.MaxValue);
				GameController.Instance.PlayerProfile.HairColor = Color.magenta;
			}
			GameController.Instance.PlayerProfile.Save();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		});
	}

	private void OnEnable()
	{
		buttonLowQuality.GetComponentInChildren<Text>().text = "Quality level: " + QualitySettings.GetQualityLevel();
		PlayerController playerController = Object.FindObjectOfType<PlayerController>();
		inputFieldProfileStuds.text = GameController.Instance.PlayerProfile.Studs.ToString();
		inputFieldProfileLevel.text = GameController.Instance.PlayerProfile.Level.ToString();
		inputFieldProfileWeaponLevel.text = GameController.Instance.PlayerProfile.WeaponLevel.ToString();
		inputFieldProfileSpinjitzuLevel.text = GameController.Instance.PlayerProfile.SpinjitzuLevel.ToString();
	}
}
