using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenWorldMap : ScreenBase
{
	public UIButton[] _areaButtons;

	public UILabel _newAreaLabel;

	public Texture[] _cloudSprites;

	public UITexture[] _cloudTextures;

	public float _animSpeed;

	private bool _animating;

	private int _animIndex;

	private float _aTimer;

	private bool _cloudSFX;

	private UITexture _currentCloudTexture;

	protected override void OnShowScreen()
	{
		_cloudSFX = false;
		base.OnShowScreen();
		bool showMinifigGoldenTrail = GameController.Instance.PlayerProfile.HasAnyUnseenMinfigParts();
		bool showGarageGoldenTrail = UnlockableVehicleParts._pInstance.vehicleGoldenTrail();
		int[] unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
		bool showDojoGoldenTrail = unseenCards != null && unseenCards.Length > 0;
		ScreenHeaderNavigation._pInstance.OpenHeader();
		ScreenHeaderNavigation._pInstance.ConfigureHeader(OnSkills, null, OnGarage, OnDojo, OnSettings, showMinifigGoldenTrail, showGarageGoldenTrail, showDojoGoldenTrail);
		_newAreaLabel.enabled = false;
		_currentCloudTexture = null;
		ScreenMap.UpdateCompletedObjectives();
		ScreenMap.CheckForSeasonUnlocks();
		if (ObjectiveController._desiredUnlock != null)
		{
			bool flag = GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(ObjectiveController._desiredUnlock.seed);
			Debug.Log("<color=yellow>Area: " + ObjectiveController._desiredUnlock.seed + " Marked for force unlock. IsUnlocked = " + flag + "</color>");
			if (!flag)
			{
				Debug.Log("<color=yellow>Force unlocking: " + ObjectiveController._desiredUnlock.seed + "</color>");
				GameController.Instance.PlayerProfile.LevelUnlocked.Add(ObjectiveController._desiredUnlock.seed, ObjectiveController._desiredUnlock);
			}
			ObjectiveController._desiredUnlock = null;
		}
		int num = _areaButtons.Length;
		for (int i = 0; i < num; i++)
		{
			string text = _areaButtons[i].name;
			bool flag2 = GameController.Instance.areaMapDB.IsMapAvalible(text);
			bool flag3 = GameController.Instance.areaMapDB.HasMapBeenPlayed(text);
			if (!flag3 && flag2)
			{
				if (_currentCloudTexture != null)
				{
					_currentCloudTexture.enabled = false;
				}
				_animating = true;
				_animIndex = 0;
				_currentCloudTexture = _cloudTextures[i];
				_currentCloudTexture.mainTexture = _cloudSprites[0];
				_cloudTextures[i].enabled = true;
			}
			else if (!flag3 && !flag2 && text != "S2A1" && text != "S3A1")
			{
				_cloudTextures[i].enabled = true;
			}
			else
			{
				_cloudTextures[i].enabled = false;
			}
			if (text == "S2A1")
			{
				_newAreaLabel.enabled = !flag3;
			}
			switch (text)
			{
			case "S1A1":
			case "S1A2":
			case "S1A3":
				_areaButtons[i].isEnabled = flag2;
				break;
			default:
				_areaButtons[i].gameObject.SetActive(flag2);
				break;
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		int num = _areaButtons.Length;
		float num2 = 1f + Mathf.Sin(Time.time * 2f) * 0.025f;
		for (int i = 0; i < num; i++)
		{
			if (_areaButtons[i].isEnabled)
			{
				_areaButtons[i].transform.localScale = Vector3.one * num2;
			}
			else
			{
				_areaButtons[i].transform.localScale = Vector3.one;
			}
		}
		if (_animating)
		{
			if (!_cloudSFX)
			{
				AudioPlayer.PlaySound("player/change/smokebomb");
				_cloudSFX = true;
			}
			_aTimer += Time.deltaTime;
			if (_aTimer > _animSpeed)
			{
				_aTimer = 0f;
				_animIndex++;
				if (_animIndex >= _cloudSprites.Length)
				{
					_animating = false;
					_currentCloudTexture.enabled = false;
					Debug.Log("Nulling Current Cloud Tex: " + _currentCloudTexture, _currentCloudTexture);
					_currentCloudTexture = null;
				}
				else
				{
					_currentCloudTexture.mainTexture = _cloudSprites[_animIndex];
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenMap screenInstance = instance.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance);
		}
	}

	public void OnGarage()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenGarage screenInstance = instance.GetScreenInstance<ScreenGarage>();
		Navigate(screenInstance);
	}

	public void OnSkills()
	{
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSkills screenInstance = instance.GetScreenInstance<ScreenSkills>();
		Navigate(screenInstance);
	}

	public void OnMapButton(string mapName)
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		Debug.Log("OnMapButton(" + mapName + ")");
		GameController.Instance.SetAreaMapMetaLayer(GameController.Instance.areaMapDB.areaMaps[mapName]);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		bool flag = GameController.Instance.areaMapDB.HasMapBeenPlayed(mapName);
		if (mapName == "S3A1" && !flag)
		{
			ScreenCutsceneCityIntro screenInstance = instance.GetScreenInstance<ScreenCutsceneCityIntro>();
			Navigate(screenInstance);
		}
		else
		{
			ScreenMap screenInstance2 = instance.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance2);
		}
	}

	public void OnDojo()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		SceneManager.LoadScene("Dojo");
		Navigate(screenInstance);
	}

	public void OnSettings()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
		if (Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenSettings>().gameObject.activeInHierarchy)
		{
			screenInstance.EndFlow();
			return;
		}
		screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_WORLD;
		instance.ShowPopupOverlay(screenInstance);
	}
}
