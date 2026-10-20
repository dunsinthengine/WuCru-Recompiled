using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenRoster : ScreenBase
{
	public RosterInfo[] _rosterInfo;

	public GoldenTrailFX _minifigPartGoldenTrail;

	private bool _inRadialMenu;

	public ScreenObjectTransitionWidget _radialMenuBacking;

	public ScreenObjectTransitionWidget _radialMenuCircle;

	public UITexture[] _slots;

	public RosterSlot[] _rosterSlots;

	public Texture _textureLocked;

	public Texture _textureEmpty;

	public Texture _textureZane;

	public Texture _textureJay;

	public Texture _textureCole;

	public Texture _textureNya;

	public Texture _textureLloyd;

	public Texture _textureKai;

	public UIButton _cancelButton;

	public UIButton _goButton;

	public UITexture _buttonSheen;

	public GameObject _wuFtueRoot;

	public ScreenObjectTransitionWidget[] _ftueWidgets;

	public NinjaRankUpEffect[] _rosterSlotRankUpEffects;

	public NinjaRankUpEffect[] _rosterWheelRankUpEffects;

	protected bool _isFTUE;

	private int _selectedSlot;

	public int _pSelectedSlot
	{
		get
		{
			return _selectedSlot;
		}
	}

	public bool _pInRadialMenu
	{
		get
		{
			return _inRadialMenu;
		}
	}

	protected override void Update()
	{
		base.Update();
		Vector3 vector = Vector3.one * (1f + Mathf.Sin(Time.time * 2f) * 0.025f);
		int num = _rosterInfo.Length;
		for (int i = 0; i < num; i++)
		{
			if (_rosterInfo[i]._pIsUnlocked && !_rosterInfo[i]._pUnselectable)
			{
				Vector3 wrapperScale = vector;
				if (_rosterInfo[i]._pIsHovered)
				{
					wrapperScale += Vector3.one * 0.1f;
				}
				_rosterInfo[i].SetWrapperScale(wrapperScale);
			}
			else
			{
				_rosterInfo[i].SetWrapperScale(Vector3.one);
			}
		}
		if (_isFTUE)
		{
			_rosterSlots[0].transform.localScale = Vector3.one + Vector3.one * Mathf.Sin(Time.time * 5f) * 0.01f;
		}
		if (_rosterSlotRankUpEffects != null)
		{
			int num2 = _rosterSlotRankUpEffects.Length;
			for (int j = 0; j < num2; j++)
			{
				_rosterSlotRankUpEffects[j].OnUpdate();
			}
		}
		if (_rosterWheelRankUpEffects != null)
		{
			int num3 = _rosterWheelRankUpEffects.Length;
			for (int k = 0; k < num3; k++)
			{
				_rosterWheelRankUpEffects[k].OnUpdate();
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			HandleDeviceBackButton();
		}
	}

	protected virtual void HandleDeviceBackButton()
	{
		if (base._pCurrentTweenType != ScreenTweenType.Idle)
		{
			return;
		}
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
		if (screenInstance.gameObject.activeInHierarchy)
		{
			return;
		}
		if (_isFTUE)
		{
			if (!_pInRadialMenu)
			{
				OnSlot1();
				return;
			}
			int num = _rosterInfo.Length;
			for (int i = 0; i < num; i++)
			{
				if (_rosterInfo[i]._minifigType == MinifigType.Zane)
				{
					OnHeroTap(_rosterInfo[i]);
					break;
				}
			}
		}
		else if (_pInRadialMenu)
		{
			OnClick();
		}
		else
		{
			ScreenHeaderNavigation._pInstance.OnMap();
		}
	}

	protected void OnClick()
	{
		if (_inRadialMenu)
		{
			TweenRadialMenuOut();
		}
	}

	protected override void OnExitScreen()
	{
		base.OnExitScreen();
		UICamera.fallThrough = null;
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		int studs = GameController.Instance.PlayerProfile.Studs;
		int num = PlayerProfile.LevelForXP(studs);
		int level = GameController.Instance.PlayerProfile.Level;
		if (num > level)
		{
			GameController.Instance.PlayerProfile.Level = num;
		}
		Time.timeScale = 1f;
		bool showGarageGoldenTrail = UnlockableVehicleParts._pInstance.vehicleGoldenTrail();
		if (!(this is ScreenMapConfirm))
		{
			ScreenHeaderNavigation._pInstance.OpenHeader();
			int[] unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
			bool showDojoGoldenTrail = unseenCards != null && unseenCards.Length > 0;
			ScreenHeaderNavigation._pInstance.ConfigureHeader(null, OnMap, OnGarage, OnDojo, OnSettings, false, showGarageGoldenTrail, showDojoGoldenTrail);
		}
		UICamera.fallThrough = base.gameObject;
		bool visible = GameController.Instance.PlayerProfile.HasAnyUnseenMinfigParts();
		if (_minifigPartGoldenTrail != null)
		{
			_minifigPartGoldenTrail.SetVisible(visible);
		}
		_inRadialMenu = false;
		_radialMenuBacking.SetToTweenInStartColor();
		_radialMenuBacking.SetStateConsideredTweenedIn(false);
		_radialMenuCircle.SetToTweenInStartColor();
		_radialMenuCircle.SetToTweenInStartScale();
		_radialMenuCircle.SetStateConsideredTweenedIn(false);
		int num2 = _rosterInfo.Length;
		for (int i = 0; i < num2; i++)
		{
			_rosterInfo[i]._pScreenRef = this;
			_rosterInfo[i].RefreshIconStatus();
			_rosterInfo[i].SetTransitionWidgetsToDefaultState();
			_rosterInfo[i]._pParentScreen = this;
		}
		RefreshSlots();
		int num3 = GameController.Instance.PlayerProfile.NinjaTeamSlotCount - 1;
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		_isFTUE = false;
		if (num3 > 0 && playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Zane, playerProfile.ZaneLevel)))
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(1);
			if (ninjaInTeam == -1)
			{
				_isFTUE = true;
			}
		}
		_wuFtueRoot.SetActive(_isFTUE);
		if (_isFTUE)
		{
			int num4 = _ftueWidgets.Length;
			for (int j = 0; j < num4; j++)
			{
				_ftueWidgets[j].TweenIn();
			}
			if (_goButton != null)
			{
				_goButton.isEnabled = false;
			}
			if (_cancelButton != null)
			{
				_cancelButton.isEnabled = false;
			}
			if (!(this is ScreenMapConfirm))
			{
				ScreenHeaderNavigation._pInstance.DisableButtons();
			}
		}
		else
		{
			_rosterSlots[0].transform.localScale = Vector3.one;
		}
		if (_buttonSheen != null)
		{
			_buttonSheen.enabled = !_isFTUE;
		}
	}

	public void OnSettings()
	{
		if (!_inRadialMenu)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
			if (Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenSettings>().gameObject.activeInHierarchy)
			{
				screenInstance.EndFlow();
				return;
			}
			screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_ROSTER;
			instance.ShowPopupOverlay(screenInstance);
		}
	}

	public void OnGarage()
	{
		if (!_inRadialMenu)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenGarage screenInstance = instance.GetScreenInstance<ScreenGarage>();
			Navigate(screenInstance);
		}
	}

	public void OnDojo()
	{
		if (!_inRadialMenu)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
			SceneManager.LoadScene("Dojo");
			Navigate(screenInstance);
		}
	}

	public void OnMap()
	{
		if (!_inRadialMenu)
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
	}

	public void OnSkills()
	{
		if (!_inRadialMenu)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			SceneManager.LoadScene("Empty");
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSkills screenInstance = instance.GetScreenInstance<ScreenSkills>();
			Navigate(screenInstance);
		}
	}

	public void OnStats()
	{
		if (!_inRadialMenu)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			SceneManager.LoadScene("Empty");
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenStats screenInstance = instance.GetScreenInstance<ScreenStats>();
			Navigate(screenInstance);
		}
	}

	public void OnSlot1()
	{
		if (!_inRadialMenu && !_rosterSlots[0]._pLocked)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_selectedSlot = 1;
			TweenRadialMenuIn();
		}
	}

	public void OnSlot2()
	{
		if (!_inRadialMenu && !_rosterSlots[1]._pLocked)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_selectedSlot = 2;
			TweenRadialMenuIn();
		}
	}

	public void OnSlot3()
	{
		if (!_inRadialMenu && !_rosterSlots[2]._pLocked)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_selectedSlot = 3;
			TweenRadialMenuIn();
		}
	}

	public void OnSlot4()
	{
		if (!_inRadialMenu && !_rosterSlots[3]._pLocked)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_selectedSlot = 4;
			TweenRadialMenuIn();
		}
	}

	public virtual void OnHeroTap(RosterInfo source)
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int iD = Team.GetID(source._minifigType, source._pLevel);
		if (source._pAlreadyRostered)
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(_selectedSlot);
			if (iD == ninjaInTeam)
			{
				TweenRadialMenuOut();
			}
			return;
		}
		TweenRadialMenuOut();
		Debug.Log("Setting ninja in team slot: " + _selectedSlot);
		playerProfile.SetNinjaInTeam(iD, _selectedSlot);
		AudioPlayer.PlaySound("ui/button/sword/shing");
		RefreshSlots();
		GameController.Instance.PlayerProfile.Save();
		if (_isFTUE)
		{
			_isFTUE = false;
			if (_goButton != null)
			{
				_goButton.isEnabled = true;
			}
			if (_cancelButton != null)
			{
				_cancelButton.isEnabled = true;
			}
			if (_buttonSheen != null)
			{
				_buttonSheen.enabled = true;
			}
			_wuFtueRoot.SetActive(false);
			if (!(this is ScreenMapConfirm))
			{
				ScreenHeaderNavigation._pInstance.EnableButtons();
			}
		}
	}

	private void TweenRadialMenuIn()
	{
		_inRadialMenu = true;
		int num = _rosterInfo.Length;
		for (int i = 0; i < num; i++)
		{
			_rosterInfo[i].TweenIn();
			_rosterInfo[i].UpdateLockedStatus();
			if (_rosterWheelRankUpEffects != null && _rosterInfo[i]._pIsUnlocked && !_rosterInfo[i]._pUnselectable && _rosterInfo[i].ShouldShowRankUpEffect(_rosterInfo[i]._minifigType))
			{
				string text = _rosterInfo[i]._lblCrystalCount.text;
				string text2 = _rosterInfo[i]._lblLevel.text;
				string previousCrystalCountString = _rosterInfo[i].GetPreviousCrystalCountString();
				string previousLevelString = _rosterInfo[i].GetPreviousLevelString();
				if (i < _rosterWheelRankUpEffects.Length)
				{
					_rosterWheelRankUpEffects[i].Display(text, text2, previousCrystalCountString, previousLevelString);
					_rosterInfo[i].MarkRankUpAsSeen(_rosterInfo[i]._minifigType);
				}
			}
		}
		_radialMenuBacking.TweenIn();
		_radialMenuCircle.TweenIn();
	}

	private void TweenRadialMenuOut()
	{
		_inRadialMenu = false;
		int num = _rosterInfo.Length;
		for (int i = 0; i < num; i++)
		{
			_rosterInfo[i].TweenOut();
		}
		_radialMenuBacking.TweenOut();
		_radialMenuCircle.TweenOut();
	}

	private void RefreshSlots()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int num = GameController.Instance.PlayerProfile.NinjaTeamSlotCount - 1;
		int num2 = 0;
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Cole, playerProfile.ColeLevel)))
		{
			num2++;
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Jay, playerProfile.JayLevel)))
		{
			num2++;
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Kai, playerProfile.KaiLevel)))
		{
			num2++;
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Lloyd, playerProfile.LloydLevel)))
		{
			num2++;
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Nya, playerProfile.NyaLevel)))
		{
			num2++;
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Zane, playerProfile.ZaneLevel)))
		{
			num2++;
		}
		int num3 = _slots.Length;
		for (int i = 0; i < num3; i++)
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(i + 1);
			if (i >= num)
			{
				_slots[i].mainTexture = _textureLocked;
				string reason = string.Empty;
				if (playerProfile.Level < 1 && i == 0)
				{
					reason = "LockedReason.UnlockedAtLevel1";
				}
				else if (playerProfile.Level < 2 && i == 1)
				{
					reason = "LockedReason.UnlockedAtLevel2";
				}
				else if (playerProfile.Level < 8 && i == 2)
				{
					reason = "LockedReason.UnlockedAtLevel8";
				}
				else if (playerProfile.Level < 15 && i == 3)
				{
					reason = "LockedReason.UnlockedAtLevel15";
				}
				_rosterSlots[i].ConfigureForLocked(reason);
				continue;
			}
			if (i > num2 - 1)
			{
				_slots[i].mainTexture = _textureLocked;
				_rosterSlots[i].ConfigureForLocked("LockedReason.RescueMoreNinjas");
				continue;
			}
			if (ninjaInTeam == -1)
			{
				_slots[i].mainTexture = _textureEmpty;
				_rosterSlots[i].ConfigureForAdd();
				continue;
			}
			MinifigType typeForID = Team.GetTypeForID(ninjaInTeam);
			switch (typeForID)
			{
			case MinifigType.Zane:
				_slots[i].mainTexture = _textureZane;
				break;
			case MinifigType.Jay:
				_slots[i].mainTexture = _textureJay;
				break;
			case MinifigType.Kai:
				_slots[i].mainTexture = _textureKai;
				break;
			case MinifigType.Lloyd:
				_slots[i].mainTexture = _textureLloyd;
				break;
			case MinifigType.Nya:
				_slots[i].mainTexture = _textureNya;
				break;
			case MinifigType.Cole:
				_slots[i].mainTexture = _textureCole;
				break;
			}
			_rosterSlots[i].ConfigureForCharacter(typeForID);
			if (_rosterSlotRankUpEffects != null && _rosterSlots[i].ShouldShowRankUpEffect(typeForID))
			{
				string text = _rosterSlots[i]._lblCrystalCount.text;
				string text2 = _rosterSlots[i]._lblLevel.text;
				string previousCrystalCountString = _rosterSlots[i].GetPreviousCrystalCountString(typeForID);
				string previousLevelString = _rosterSlots[i].GetPreviousLevelString(typeForID);
				if (i < _rosterSlotRankUpEffects.Length)
				{
					_rosterSlotRankUpEffects[i].Display(text, text2, previousCrystalCountString, previousLevelString);
					_rosterSlots[i].MarkRankUpAsSeen(typeForID);
				}
			}
		}
	}
}
