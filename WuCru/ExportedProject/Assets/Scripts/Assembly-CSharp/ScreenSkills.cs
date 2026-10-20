using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSkills : ScreenBase
{
	private static ScreenSkills _instance;

	public GoldenTrailFX _minifigPartGoldenTrail;

	public BoxTransitionFX _skillBacking;

	public UISprite _cursor;

	public UIButton _statsButton;

	public UIButton _rosterButton;

	public UILabel _skillPoints;

	public UILabel _defaultInfo;

	public SkillIcon[] _skillIcons;

	public UILabel _selectedSkillName;

	public UILabel _selectedSkillSubTitle;

	public UILabel _selectedSkillDescription;

	public UILabel _skillCost;

	public UISprite _selectedSkillIcon;

	public UIButton _purchaseButton;

	public UISprite _padlock;

	private StringBuilder _dummySB = new StringBuilder();

	private SkillIcon _currentSkillIcon;

	private float _circleFXTimer;

	private bool _ftueLocked;

	public static ScreenSkills _pInstance
	{
		get
		{
			return _instance;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		ScreenHeaderNavigation._pInstance.OpenHeader();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		_ftueLocked = playerProfile.AvailableSkillPoints > 0 && playerProfile.AgilityLevel == 0 && playerProfile.SpinjitzuLevel == 0 && playerProfile.StaminaLevel == 0 && playerProfile.StrengthLevel == 0 && playerProfile.WeaponLevel == 0;
		bool showGarageGoldenTrail = UnlockableVehicleParts._pInstance.vehicleGoldenTrail();
		int[] unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
		bool showDojoGoldenTrail = unseenCards != null && unseenCards.Length > 0;
		ScreenHeaderNavigation._pInstance.ConfigureHeader(null, OnMap, OnGarage, OnDojo, OnSettings, false, showGarageGoldenTrail, showDojoGoldenTrail);
		if (_ftueLocked)
		{
			ScreenHeaderNavigation._pInstance.DisableButtons();
			_statsButton.isEnabled = false;
			_rosterButton.isEnabled = false;
		}
		bool visible = GameController.Instance.PlayerProfile.HasAnyUnseenMinfigParts();
		_minifigPartGoldenTrail.SetVisible(visible);
		_skillBacking.Begin();
		_padlock.enabled = false;
		_cursor.enabled = false;
		_skillPoints.text = playerProfile.AvailableSkillPoints.ToString();
		_selectedSkillDescription.enabled = false;
		_selectedSkillIcon.enabled = false;
		_selectedSkillName.enabled = false;
		_selectedSkillSubTitle.enabled = false;
		_purchaseButton.gameObject.SetActive(false);
		_defaultInfo.enabled = true;
		RefreshIcons();
	}

	protected override void Update()
	{
		base.Update();
		_circleFXTimer += Time.deltaTime;
		if (_circleFXTimer > 2f)
		{
			_circleFXTimer = 0f;
		}
		_cursor.transform.localScale = Vector3.one * (1f - Mathf.Sin(Time.time * 8f) * 0.02f);
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
			if (!screenInstance.gameObject.activeInHierarchy && !_ftueLocked)
			{
				ScreenHeaderNavigation._pInstance.OnMap();
			}
		}
	}

	public void OnMap()
	{
		SceneManager.LoadScene("Empty");
		if (GlobalInGameData.ShouldLaunchHandsOfTimeCampaign())
		{
			GlobalInGameData.LaunchHandsOfTimeCampaign();
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenHeaderNavigation._pInstance.CloseHeader();
			ScreenCutsceneHandsOfTimeIntro screenInstance = instance.GetScreenInstance<ScreenCutsceneHandsOfTimeIntro>();
			Navigate(screenInstance);
			return;
		}
		ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
		bool flag = GameController.Instance.PlayerProfile.TotalLevelsPlayed <= 1;
		if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap() || flag)
		{
			ScreenWorldMap screenInstance2 = instance2.GetScreenInstance<ScreenWorldMap>();
			Navigate(screenInstance2);
		}
		else
		{
			ScreenMap screenInstance3 = instance2.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance3);
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
		screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_SKILLS;
		instance.ShowPopupOverlay(screenInstance);
	}

	public void OnGarage()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenGarage screenInstance = instance.GetScreenInstance<ScreenGarage>();
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

	public void OnDojo()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		SceneManager.LoadScene("Dojo");
		Navigate(screenInstance);
	}

	public void OnStats()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		SceneManager.LoadScene("Empty");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenStats screenInstance = instance.GetScreenInstance<ScreenStats>();
		Navigate(screenInstance);
	}

	public void OnSkillPressed(SkillIcon icon)
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		_cursor.enabled = true;
		_cursor.transform.position = icon.transform.position;
		_selectedSkillDescription.enabled = true;
		_selectedSkillIcon.enabled = true;
		_selectedSkillName.enabled = true;
		_selectedSkillSubTitle.enabled = true;
		_purchaseButton.gameObject.SetActive(true);
		_defaultInfo.enabled = false;
		_dummySB.Length = 0;
		_dummySB.Append("Skill.Name_");
		_dummySB.Append(icon._skillType);
		_selectedSkillName.text = Localise(_dummySB.ToString());
		_dummySB.Length = 0;
		_dummySB.Append("Skill.SubTitle_");
		_dummySB.Append(icon._skillType);
		_selectedSkillSubTitle.text = Localise(_dummySB.ToString());
		_dummySB.Length = 0;
		_dummySB.Append("Skill.Description_");
		_dummySB.Append(icon._skillType);
		_dummySB.Append("_Level");
		_dummySB.Append(icon._buttonLevel);
		_selectedSkillDescription.text = Localise(_dummySB.ToString());
		int cost = icon.GetCost();
		_skillCost.text = cost.ToString();
		_purchaseButton.gameObject.SetActive(!icon._pHasPurchased);
		_purchaseButton.isEnabled = icon._pIsPurchasable;
		_selectedSkillIcon.spriteName = icon._icon.spriteName;
		_currentSkillIcon = icon;
		_padlock.enabled = !icon._pHasPurchased && !icon._pPurchaseableLevel;
		_selectedSkillIcon.color = ((!icon._pIsPurchasable && !icon._pHasPurchased) ? Color.grey : Color.white);
	}

	public void OnConfirmUpgradeSkill()
	{
		if (!(_currentSkillIcon == null) && _currentSkillIcon._pIsPurchasable && !_currentSkillIcon._pHasPurchased)
		{
			PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
			switch (_currentSkillIcon._skillType)
			{
			case SkillIcon.SKILLTYPE.WEAPON:
				playerProfile.WeaponLevel++;
				break;
			case SkillIcon.SKILLTYPE.SPINJITZU:
				playerProfile.SpinjitzuLevel++;
				break;
			case SkillIcon.SKILLTYPE.STAMINA:
				playerProfile.StaminaLevel++;
				break;
			case SkillIcon.SKILLTYPE.AGILITY:
				playerProfile.AgilityLevel++;
				break;
			case SkillIcon.SKILLTYPE.STRENGTH:
				playerProfile.StrengthLevel++;
				break;
			}
			playerProfile.Save();
			AudioPlayer.PlaySound("music/level/up");
			_skillPoints.text = playerProfile.AvailableSkillPoints.ToString();
			RefreshIcons();
			int num = playerProfile.TotalSkillPointsAcquired - playerProfile.AvailableSkillPoints;
			OnSkillPressed(_currentSkillIcon);
			if (_ftueLocked)
			{
				_ftueLocked = false;
				_statsButton.isEnabled = true;
				_rosterButton.isEnabled = true;
				ScreenHeaderNavigation._pInstance.EnableButtons();
				ScreenHeaderNavigation._pInstance._ftueMapArrow.enabled = true;
			}
		}
	}

	private void RefreshIcons()
	{
		int num = _skillIcons.Length;
		for (int i = 0; i < num; i++)
		{
			_skillIcons[i].ConfigureButton();
		}
	}
}
