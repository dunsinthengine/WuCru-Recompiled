using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeaFarmGUI : MonoBehaviour
{
	public Button buttonMasterWu;

	public Button buttonVehicle;

	public Button buttonMyNinja;

	public Button buttonMap;

	public MenuCharacter menuCharacter;

	public GameObject vehicleContainer;

	public GameObject vehiclePointerContainer;

	public GameObject ninjaPointer;

	public GameObject mapPointer;

	public GameObject mapRoot;

	public WuCruGUI wuCruGUI;

	public DayOfTheDepartedGUI dayOfTheDepartedGUI;

	[SerializeField]
	private TopBar _topBar;

	[SerializeField]
	private GameObject _clickBlocker;

	private void Awake()
	{
		AnalyticsEvents.LogLoadScene("TeaFarm");
		Resources.UnloadUnusedAssets();
		wuCruGUI.gameObject.SetActive(false);
		wuCruGUI.ClickBlockerEvent += HandleClickBlockerEvent;
		if (GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
		{
			GameController.Instance.SetState(new GameControllerStateIdle());
		}
		if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			OnAllButtonsEnabled();
			_topBar.EnableTopbarButtons();
		}
		else
		{
			onNinjaButtonEnabled();
		}
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		int totalLevelsPlayed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
		Debug.Log("TeaFarm - TotalLEvelsPlayer" + totalLevelsPlayed);
		if (chapterCursor < 1 || totalLevelsPlayed < 1)
		{
			Debug.Log("TeaFarm - New Game - starting from scratch - Character Creation");
			GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
			GameController.Instance.PlayerProfile.SetChapterCursor(1, 1);
			GameController.Instance.ObjectiveController.InitiateObjective(1, 1);
			GameController.Instance.PlayerProfile.Save();
			SceneManager.LoadScene("CharacterCreation");
			return;
		}
		if (chapterCursor == 2 && totalLevelsPlayed == 1)
		{
			Debug.Log("TeaFarm - Played the first level - Play chapter 2 storyviewer");
			GameController.Instance.PlayerProfile.SetChapterCursor(1, 2);
			GameController.Instance.ObjectiveController.InitiateObjective(1, 2);
			GameController.Instance.PlayerProfile.Save();
			StoryViewer.Load();
			return;
		}
		if (GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.IsCurrentObjectiveCompleted())
		{
			int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
			int chapterCursor2 = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
			chapterCursor2++;
			if (GameController.Instance.NumChapters(lastPlayedSeason) >= chapterCursor2)
			{
				GameController.Instance.PlayerProfile.SetChapterCursor(lastPlayedSeason, chapterCursor2);
				GameController.Instance.ObjectiveController.InitiateObjective(lastPlayedSeason, chapterCursor2);
				GameController.Instance.PlayerProfile.Save();
				StoryViewer.Load();
				return;
			}
		}
		else
		{
			if (GameController.Instance.ObjectiveController.CurrentObjective != null)
			{
				int lastPlayedSeason2 = GameController.Instance.PlayerProfile.LastPlayedSeason;
				int chapterCursor3 = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason2);
				if (GameController.Instance.ObjectiveController.CurrentObjective.season != lastPlayedSeason2 || GameController.Instance.ObjectiveController.CurrentObjective.chapter != chapterCursor3)
				{
					Debug.LogWarning("Story and Objectives out of sync - force updating objective to season: " + lastPlayedSeason2 + ", chapter: " + chapterCursor3);
					ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
					foreach (ObjectiveLine objectiveLine in lines)
					{
						objectiveLine.Completed = true;
					}
				}
				GameController.Instance.ObjectiveController.InitiateObjective(lastPlayedSeason2, chapterCursor3);
				GameController.Instance.PlayerProfile.Save();
			}
			if (PlayerProfile.newlyUnlockedNinjas.Count > 0)
			{
				menuCharacter.gameObject.SetActive(false);
				NewNinjaUnlockedGUI component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/NewNinjaUnlockedGUI"))).GetComponent<NewNinjaUnlockedGUI>();
				component.SetNinja(PlayerProfile.newlyUnlockedNinjas[0]);
				component.OnClose = (NewNinjaUnlockedGUI.OnCloseDelegate)Delegate.Combine(component.OnClose, new NewNinjaUnlockedGUI.OnCloseDelegate(OnNewNinjaUnlockedGUIClose));
			}
			int totalLevelsPlayed2 = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
			if (GameController.Instance.PlayerProfile.ShowSpinjitzuUnlock)
			{
				NewAbilityUnlockedGUI component2 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/NewAbilityUnlockedGUI"))).GetComponent<NewAbilityUnlockedGUI>();
				component2.SetAbility(AbilityType.ABILITY_SPINJITZU);
				component2.OnClose = (NewAbilityUnlockedGUI.OnCloseDelegate)Delegate.Combine(component2.OnClose, new NewAbilityUnlockedGUI.OnCloseDelegate(OnNewAbilityUnlockedGUIClose));
				GameController.Instance.PlayerProfile.ShowSpinjitzuUnlock = false;
				GameController.Instance.PlayerProfile.Save();
			}
		}
		chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
		if (chapterCursor == 2)
		{
			SceneManager.LoadScene("AreaMap");
		}
	}

	private void OnDestroy()
	{
		wuCruGUI.ClickBlockerEvent -= HandleClickBlockerEvent;
	}

	private void OnNewNinjaUnlockedGUIClose()
	{
		PlayerProfile.newlyUnlockedNinjas.Remove(PlayerProfile.newlyUnlockedNinjas[0]);
		if (PlayerProfile.newlyUnlockedNinjas.Count > 0)
		{
			menuCharacter.gameObject.SetActive(false);
			NewNinjaUnlockedGUI component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/NewNinjaUnlockedGUI"))).GetComponent<NewNinjaUnlockedGUI>();
			component.SetNinja(PlayerProfile.newlyUnlockedNinjas[0]);
			component.OnClose = (NewNinjaUnlockedGUI.OnCloseDelegate)Delegate.Combine(component.OnClose, new NewNinjaUnlockedGUI.OnCloseDelegate(OnNewNinjaUnlockedGUIClose));
		}
		else
		{
			menuCharacter.gameObject.SetActive(true);
		}
	}

	private void OnNewAbilityUnlockedGUIClose()
	{
	}

	private void onNinjaButtonEnabled()
	{
		buttonMyNinja.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			SceneManager.LoadScene("NinjaScreen");
		});
		mapRoot.SetActive(false);
		buttonMasterWu.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			StoryViewer.Load();
		});
	}

	private void OnAllButtonsEnabled()
	{
		buttonMasterWu.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			StoryViewer.Load();
		});
		buttonVehicle.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			SceneManager.LoadScene("Garage");
		});
		buttonMyNinja.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			SceneManager.LoadScene("NinjaScreen");
		});
		mapRoot.SetActive(true);
		buttonMap.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			if (GameController.Instance.PlayerProfile.GetChapterCursor(1) > 6)
			{
				SceneManager.LoadScene("WorldMap");
			}
			else
			{
				SceneManager.LoadScene("AreaMap");
			}
		});
	}

	private void Start()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		menuCharacter.gameObject.SetActive(true);
		MinifigDefinition minifigDefinition = playerProfile.GetMinifigDefinition();
		Texture2D characterTexture = CharacterBuilder.ConfigureTexture(menuCharacter, minifigDefinition);
		CharacterBuilder.ConfigureCharacter(menuCharacter, minifigDefinition, characterTexture);
		Weapon weapon = menuCharacter.GetWeapon();
		if ((bool)weapon && weapon.type == WeaponType.Nunchuck)
		{
			StartCoroutine(MyNinjaGUI.NunchuckHack(weapon));
		}
		int key = 0;
		bool flag = false;
		if (flag)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Vehicles/" + VehicleDefinition.vehicleDefinitions[key].model));
			gameObject.transform.SetParent(vehicleContainer.transform, false);
			vehicleContainer.transform.localScale = Vector3.one * VehicleDefinition.vehicleDefinitions[key].teafarmScale;
		}
		vehicleContainer.SetActive(flag);
		buttonVehicle.gameObject.SetActive(flag);
		if (PlayerProfile.newlyUnlockedNinjas.Count > 0 || NinjaScreenController.OpeningPage == NinjaScreenOpeningPage.NinjaStats)
		{
			ninjaPointer.SetActive(true);
			vehiclePointerContainer.SetActive(false);
			mapPointer.SetActive(false);
		}
		else if (PlayerProfile.newlyUnlockedVehicles.Count > 0)
		{
			ninjaPointer.SetActive(false);
			vehiclePointerContainer.SetActive(true);
			mapPointer.SetActive(false);
		}
		else
		{
			ninjaPointer.SetActive(false);
			vehiclePointerContainer.SetActive(false);
			mapPointer.SetActive(true);
		}
		if (NetworkUtils.HasNetworkConnection())
		{
			GameController.Instance.MissionController.Online = true;
		}
		else
		{
			GameController.Instance.MissionController.Online = false;
		}
		if (GameController.WuCruMissionsEnabled && GameController.Instance.MissionController.Online && GameController.Instance.PlayerProfile.TotalUniqueLevelsPlayed > 2)
		{
			wuCruGUI.gameObject.SetActive(true);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		HandlePause(pause);
	}

	private void OnApplicationFocus(bool focus)
	{
		HandlePause(!focus);
	}

	private void HandlePause(bool pause)
	{
		if (!GameController.WuCruMissionsEnabled || pause)
		{
			return;
		}
		if (GameController.Instance.MissionController.Online)
		{
			if (!NetworkUtils.HasNetworkConnection())
			{
				GameController.Instance.MissionController.Online = false;
				wuCruGUI.gameObject.SetActive(false);
			}
		}
		else if (NetworkUtils.HasNetworkConnection())
		{
			GameController.Instance.MissionController.Online = true;
			wuCruGUI.gameObject.SetActive(true);
		}
	}

	private void HandleClickBlockerEvent(object sender, DataEventArgs<bool> args)
	{
		if (args != null)
		{
			Debug.LogFormat("Tea Farm clickblocker {0}", (!args.data) ? "off" : "on");
			_clickBlocker.SetActive(args.data);
			_topBar.SetClickBlockerActive(args.data);
		}
	}
}
