using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
	[SerializeField]
	private GameObject _clickBlocker;

	public Button buttonWorldMap;

	public Button buttonHomeBase;

	public Button buttonNinja;

	public Button buttonGarage;

	public Button buttonBack;

	public Button buttonQuit;

	public Image alert_Garage;

	public Image alert_WorldMap;

	public Image alert_MyNinja;

	public Image alert_TeaFarm;

	public Image hi_WorldMap;

	public Image hi_HomeBase;

	public Image hi_Ninja;

	public Image hi_Garage;

	public string sceneName;

	public bool isSettingsBoxActive;

	public bool disabledByTutorial;

	private GameObject settingsBoxGO;

	private void Awake()
	{
		sceneName = SceneManager.GetActiveScene().name;
		alert_Garage.gameObject.SetActive(false);
		alert_WorldMap.gameObject.SetActive(false);
		alert_TeaFarm.gameObject.SetActive(false);
		alert_MyNinja.gameObject.SetActive(false);
		Initialize();
		buttonBack.gameObject.SetActive(false);
		if (!GameController.Instance.PlayerProfile.ShowGarageInTopBar)
		{
			buttonGarage.gameObject.SetActive(false);
		}
		else
		{
			buttonGarage.gameObject.SetActive(true);
		}
	}

	private void Initialize()
	{
		EnableTopbarButtons();
		HighlightCurrentButton();
	}

	public void EnableTopbarButtons()
	{
		ShowAlertIcons();
		if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Count < 2 && !GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			buttonWorldMap.onClick.RemoveAllListeners();
			buttonHomeBase.onClick.RemoveAllListeners();
			buttonNinja.onClick.RemoveAllListeners();
			buttonGarage.onClick.RemoveAllListeners();
			SetButtonColor(buttonHomeBase, 0.5f);
			SetButtonColor(buttonNinja, 0.5f);
			buttonBack.onClick.AddListener(() =>
			{
				AudioPlayer.PlaySound("ui/button/back/click");
			});
			buttonQuit.onClick.AddListener(() =>
			{
				if (!isSettingsBoxActive)
				{
					AudioPlayer.PlaySound("ui/button/back/click");
					settingsBoxGO = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/SettingsGUI"));
					isSettingsBoxActive = true;
				}
			});
			SetButtonColor(buttonBack, 1f);
			SetButtonColor(buttonQuit, 1f);
			return;
		}
		if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			buttonWorldMap.onClick.RemoveAllListeners();
			buttonHomeBase.onClick.RemoveAllListeners();
			buttonGarage.onClick.RemoveAllListeners();
			SetButtonColor(buttonWorldMap, 0.5f);
			if (sceneName == Paths.sceneNameTeaFarm)
			{
				SetButtonColor(buttonHomeBase, 1f);
			}
			else
			{
				SetButtonColor(buttonHomeBase, 0.5f);
			}
			buttonBack.onClick.AddListener(() =>
			{
				AudioPlayer.PlaySound("ui/button/back/click");
			});
			buttonQuit.onClick.AddListener(() =>
			{
				if (!isSettingsBoxActive)
				{
					AudioPlayer.PlaySound("ui/button/back/click");
					settingsBoxGO = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/SettingsGUI"));
					isSettingsBoxActive = true;
				}
			});
			buttonNinja.onClick.AddListener(() =>
			{
				if (settingsBoxGO != null)
				{
					Object.Destroy(settingsBoxGO);
				}
				if (SceneManager.GetActiveScene().name != "NinjaScreen")
				{
					if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
					{
						GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
					}
					AudioPlayer.PlaySound("ui/button/click");
					Debug.Log("Loading NinjaScreen");
					SceneManager.LoadScene("NinjaScreen");
				}
			});
			SetButtonColor(buttonNinja, 1f);
			SetButtonColor(buttonBack, 1f);
			SetButtonColor(buttonQuit, 1f);
			return;
		}
		if (sceneName != Paths.sceneNameNinjaScreen)
		{
			SetButtonColor(buttonNinja, 1f);
		}
		if (sceneName != Paths.sceneNameWorldMap)
		{
			SetButtonColor(buttonWorldMap, 1f);
		}
		if (sceneName != Paths.sceneNameTeaFarm)
		{
			SetButtonColor(buttonHomeBase, 1f);
		}
		if (buttonGarage.enabled && sceneName != Paths.sceneNameGarage)
		{
			SetButtonColor(buttonGarage, 1f);
		}
		buttonWorldMap.onClick.AddListener(() =>
		{
			if (settingsBoxGO != null)
			{
				Object.Destroy(settingsBoxGO);
			}
			if (SceneManager.GetActiveScene().name != "WorldMap")
			{
				if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
				{
					GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
				}
				AudioPlayer.PlaySound("ui/button/click");
				if (GameController.Instance.PlayerProfile.GetChapterCursor(1) > 6)
				{
					Debug.Log("Loading WorldMap (S1 Chater above 6)");
					SceneManager.LoadScene("WorldMap");
				}
				else
				{
					Debug.Log("Loading AreadMap (S1 Chapter below 7)");
					SceneManager.LoadScene("AreaMap");
				}
			}
		});
		buttonHomeBase.onClick.AddListener(() =>
		{
			if (settingsBoxGO != null)
			{
				Object.Destroy(settingsBoxGO);
			}
			if (SceneManager.GetActiveScene().name != "TeaFarm")
			{
				if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
				{
					GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
				}
				AudioPlayer.PlaySound("ui/button/click");
				Debug.Log("Loading TeaFarm");
				SceneManager.LoadScene("TeaFarm");
			}
		});
		buttonNinja.onClick.AddListener(() =>
		{
			if (settingsBoxGO != null)
			{
				Object.Destroy(settingsBoxGO);
			}
			if (SceneManager.GetActiveScene().name != "NinjaScreen")
			{
				if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
				{
					GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
				}
				AudioPlayer.PlaySound("ui/button/click");
				Debug.Log("Loading NinjaScreen");
				SceneManager.LoadScene("NinjaScreen");
			}
		});
		buttonGarage.onClick.AddListener(() =>
		{
			if (settingsBoxGO != null)
			{
				Object.Destroy(settingsBoxGO);
			}
			if (SceneManager.GetActiveScene().name != "Garage")
			{
				if (GameController.Instance.currentState.GetType() != typeof(GameControllerStateIdle))
				{
					GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
				}
				AudioPlayer.PlaySound("ui/button/click");
				Debug.Log("Loading Garage");
				SceneManager.LoadScene("Garage");
			}
		});
		buttonBack.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			Debug.Log("Back button...currently disabled");
		});
		buttonQuit.onClick.AddListener(() =>
		{
			if (!isSettingsBoxActive)
			{
				AudioPlayer.PlaySound("ui/button/back/click");
				Debug.Log("Close button...");
				settingsBoxGO = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/SettingsGUI"));
				isSettingsBoxActive = true;
			}
		});
	}

	private void ShowAlertIcons()
	{
		alert_WorldMap.gameObject.SetActive(false);
		alert_Garage.gameObject.SetActive(false);
		alert_MyNinja.gameObject.SetActive(false);
		alert_TeaFarm.gameObject.SetActive(false);
		if (GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Count < 2 && !GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			if (sceneName != Paths.sceneNameWorldMap && sceneName != Paths.sceneNameAreaMap)
			{
				alert_WorldMap.gameObject.SetActive(true);
			}
		}
		else if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			if (sceneName != Paths.sceneNameNinjaScreen)
			{
				alert_MyNinja.gameObject.SetActive(true);
			}
		}
		else if (NinjaScreenController.OpeningPage == NinjaScreenOpeningPage.NinjaStats)
		{
			if (sceneName != Paths.sceneNameNinjaScreen)
			{
				alert_MyNinja.gameObject.SetActive(true);
			}
		}
		else if (PlayerProfile.newlyUnlockedVehicles.Count > 0)
		{
			if (sceneName != Paths.sceneNameGarage)
			{
				alert_Garage.gameObject.SetActive(true);
			}
		}
		else if (sceneName != Paths.sceneNameWorldMap && sceneName != Paths.sceneNameAreaMap)
		{
			alert_WorldMap.gameObject.SetActive(true);
		}
	}

	private void HighlightCurrentButton()
	{
		string text = SceneManager.GetActiveScene().name;
		hi_WorldMap.gameObject.SetActive(text == Paths.sceneNameWorldMap || text == Paths.sceneNameAreaMap);
		hi_HomeBase.gameObject.SetActive(text == Paths.sceneNameTeaFarm);
		hi_Ninja.gameObject.SetActive(text == Paths.sceneNameNinjaScreen);
		hi_Garage.gameObject.SetActive(text == Paths.sceneNameGarage);
	}

	private void SetButtonColor(Button button, float alpha)
	{
		Color color = button.image.color;
		color.a = alpha;
		button.image.color = color;
	}

	public void SetClickBlockerActive(bool visible)
	{
		_clickBlocker.SetActive(visible);
	}
}
