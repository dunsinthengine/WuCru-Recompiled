using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenStats : ScreenBase
{
	public UITexture[] _ninjas;

	public UILabel _coinCount;

	public UILabel _crystalCount;

	public UILabel _starCount;

	public Transform _xpBarStart;

	public Transform _xpBarCurrent;

	public Transform _xpBarEnd;

	public UITexture _mugShot;

	public UISprite _xpFill;

	public UILabel _levelNum;

	public GoldenTrailFX _minifigPartGoldenTrail;

	private Texture2D _playerMugshot;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		for (int i = 0; i < _ninjas.Length; i++)
		{
			_ninjas[i].color = new Color(0.1f, 0.1f, 0.1f);
		}
		List<int> unlockedNinjaSKUs = GameController.Instance.PlayerProfile.UnlockedNinjaSKUs;
		for (int j = 0; j < unlockedNinjaSKUs.Count; j++)
		{
			switch (Team.GetTypeForID(unlockedNinjaSKUs[j]))
			{
			case MinifigType.Kai:
				_ninjas[0].color = Color.white;
				break;
			case MinifigType.Nya:
				_ninjas[1].color = Color.white;
				break;
			case MinifigType.Jay:
				_ninjas[2].color = Color.white;
				break;
			case MinifigType.Lloyd:
				_ninjas[3].color = Color.white;
				break;
			case MinifigType.Cole:
				_ninjas[4].color = Color.white;
				break;
			case MinifigType.Zane:
				_ninjas[5].color = Color.white;
				break;
			}
		}
		bool showGarageGoldenTrail = UnlockableVehicleParts._pInstance.vehicleGoldenTrail();
		int[] unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
		bool showDojoGoldenTrail = unseenCards != null && unseenCards.Length > 0;
		ScreenHeaderNavigation._pInstance.OpenHeader();
		ScreenHeaderNavigation._pInstance.ConfigureHeader(null, OnMap, OnGarage, OnDojo, OnSettings, false, showGarageGoldenTrail, showDojoGoldenTrail);
		bool visible = GameController.Instance.PlayerProfile.HasAnyUnseenMinfigParts();
		_minifigPartGoldenTrail.SetVisible(visible);
		RefreshStats();
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
			if (!screenInstance.gameObject.activeInHierarchy)
			{
				ScreenHeaderNavigation._pInstance.OnMap();
			}
		}
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
		screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_STATS;
		instance.ShowPopupOverlay(screenInstance);
	}

	public void OnGarage()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenGarage screenInstance = instance.GetScreenInstance<ScreenGarage>();
		Navigate(screenInstance);
	}

	public void OnMap()
	{
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap())
		{
			ScreenWorldMap screenInstance = instance.GetScreenInstance<ScreenWorldMap>();
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

	public void OnSkills()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSkills screenInstance = instance.GetScreenInstance<ScreenSkills>();
		Navigate(screenInstance);
	}

	public void OnRoster()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenRoster screenInstance = instance.GetScreenInstance<ScreenRoster>();
		Navigate(screenInstance);
	}

	public void OnCreateCharacter()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		ScreenHeaderNavigation._pInstance.CloseHeader();
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenCreateCharacter screenInstance = instance.GetScreenInstance<ScreenCreateCharacter>();
		Navigate(screenInstance);
	}

	private void RefreshStats()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		_coinCount.text = playerProfile.AvailableSkillPoints.ToString();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		Dictionary<string, AreaMapMetaLayer>.Enumerator enumerator = GameController.Instance.areaMapDB.areaMaps.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = enumerator.Current.Key;
			int num4 = GameController.Instance.areaMapDB.areaMaps[key].rowArray.Length;
			int num5 = GameController.Instance.areaMapDB.areaMaps[key].rowArray[0].colArray.Length;
			for (int i = 0; i < num4; i++)
			{
				for (int j = 0; j < num5; j++)
				{
					int seedFromPosition = GameController.Instance.areaMapDB.areaMaps[key].GetSeedFromPosition(i, j);
					bool flag = GameController.Instance.areaMapDB.areaMaps[key].IsPlayed(seedFromPosition);
					bool flag2 = !GameController.Instance.areaMapDB.areaMaps[key].IsBlocked(i, j);
					num2 += GameController.Instance.areaMapDB.areaMaps[key].GetNumStars(seedFromPosition);
					if (flag)
					{
						num++;
					}
					if (flag2)
					{
						num3 += 3;
					}
				}
			}
		}
		_crystalCount.text = num.ToString();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(num2);
		stringBuilder.Append("/");
		stringBuilder.Append(num3);
		_starCount.text = stringBuilder.ToString();
	}
}
