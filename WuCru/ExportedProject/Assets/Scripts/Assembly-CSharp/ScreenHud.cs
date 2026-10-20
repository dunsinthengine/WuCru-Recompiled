using System;
using System.Text;
using AmuzoEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenHud : ScreenBase
{
	public enum FTUE_STATE
	{
		NONE = 0,
		SPINJITSU = 1,
		SPINJITSU_OUT = 2,
		ROSTER_CHANGE_1 = 3,
		ROSTER_CHANGE_1_TRANSITION = 4,
		ROSTER_CHANGE_2 = 5,
		ROSTER_CHANGE_OUT = 6,
		LEVELLING_CRU = 7,
		LEVELLING_CRU_OUT = 8,
		FADE_OUT = 9,
		BUSHES = 10,
		BUSH_MOVE_TO = 11,
		BUSHES_OUT = 12
	}

	private const int FTUE_DEPTH_SHIFT = 700;

	private static ScreenHud _instance;

	public HUDElementDisplay _hudElementDisplay;

	public HUDChestMinigame _hudChestMinigame;

	public HUDBattleMinigame _hudBattleMinigame;

	public HUDItemTracker _enemyIconTracker;

	public HUDItemTracker _enemyIconTrackerCity;

	public HUDItemTracker _enemyIconSeenTracker;

	public HUDItemTracker _enemyIconSuspiciousTracker;

	public HUDItemTracker _keyIconTracker;

	public HUDItemTracker _doorIconTracker;

	public HUDItemTracker _switchIconTracker;

	public HUDSwordTapTelegraph _swordTapTelegraph;

	public HUDZoneTapMinigame _zoneTapMinigame;

	public HUDMessagePool _poolMessageSpinjitsu;

	public HUDMessagePool _poolMessageNumbers;

	public HUDMessagePool _poolMessagePerfectAttack;

	public HUDMessagePool _poolMessageTrap;

	public HUDMessagePool _poolMessageExplosion;

	public HUDMessagePool _poolMessageCounterAttack;

	public HUDMessagePool _poolMessageDodge;

	public HUDMessagePool _poolMessageDeflect;

	public HUDMessagePool _poolMessageSurprise;

	public HUDMessagePool _poolMessageSneakAttack;

	public HUDMessagePool _poolMessageSecondaryAttack;

	public HUDMessagePool _poolMessagesGreenNumbers;

	public Transform[] _rosterSlotPositions;

	public Transform _rosterSlotRoot;

	public HUDRosterSlot[] _rosterSlots;

	public UITexture _playerTexture;

	public UITexture _kaiTexture;

	public UITexture _lloydTexture;

	public UITexture _nyaTexture;

	public UITexture _jayTexture;

	public UITexture _zaneTexture;

	public UITexture _coleTexture;

	public Transform _healthStart;

	public Transform _healthEnd;

	public Transform _healthCurrent;

	private float _startingHealthVal;

	private float _targetHealthVal;

	private float _healthChangeTime;

	private float _currentHP;

	public UISprite _weaponIcon;

	public UISprite _abilityIcon;

	public UILabel _studCount;

	public UILabel _studCountShadow;

	public UIWidget _studCountWrapper;

	public UIButton _pauseButton;

	public UIButton _spinjitsuButton;

	public UISprite _spinjitsuIconBacking;

	public UILabel _spinjitsuCount;

	public UILabel _spinjitsuCountShadow;

	public UISprite _cooldown;

	public Transform _spinjitsuRoot;

	public ObjectiveBar _objectiveBar;

	public ParticleSystem _spinjitsuActiveParticles;

	public UISprite _ftueFade;

	public UIWidget[] _spinjitsuFTUEElements;

	public UIWidget[] _rosterFTUEElements;

	public ScreenObjectTransitionWidget[] _defeatedWidgets;

	public UITexture _ftueArrowSpinjitsu;

	public Transform _ftueArrowSpinjitsuRoot;

	public UITexture _ftueArrowRoster;

	public Transform _ftueArrowRosterRoot;

	public ScreenObjectTransitionWidget _ftueRosterArrowTransition;

	public ScreenObjectTransitionWidget _ftueSpinjitsuArrowTransition;

	public Transform _ftueSpinjitsuScaleRoot;

	public Transform _ftueRosterScaleRoot;

	public UIWidget[] _wuWidgets;

	public ScreenObjectTransitionWidget[] _wuTransitions;

	public UILabel _wuMessage;

	public HUDItemTrackerTexture _ftueBushArrows;

	private FTUE_STATE _ftueState;

	private float _ftueStateChangeTime;

	private float _studFXStartTime;

	private StringBuilder _dummySB = new StringBuilder();

	private Vector3 _studFXStartSize = new Vector3(1.5f, 1.5f, 1.5f);

	private Vector3 _studFXEndSize = Vector3.one;

	private int _desiredCharacterSwap = -1;

	private float _swapDelay;

	private HUDRosterSlot _currentSlot;

	private bool _wantRegisterForDeaths;

	private int _currentSpinjitsuCount;

	private ParticleSystem.EmissionModule _emissionModule;

	private float _spinjitsuPulseTime;

	private float _screenBeginTime;

	private bool _seenSpinjitsuFTUE;

	private bool _wantCharacterChangeFTUE;

	private bool _wantBushesFTUE;

	private Vector3 _ftueArrowRootPos;

	private float _ftueWait;

	private bool _hasFailed;

	private bool _showDefeated;

	private float _failTime;

	private bool _mouseDown;

	private float _mouseDownTime;

	private float _tutorialDelay;

	public static ScreenHud _pInstance
	{
		get
		{
			return _instance;
		}
	}

	public bool _pMouseDown
	{
		get
		{
			return _mouseDown;
		}
	}

	public float _pLastMouseButtonDownTime
	{
		get
		{
			return _mouseDownTime;
		}
	}

	public FTUE_STATE _pFtueState
	{
		get
		{
			return _ftueState;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
		_emissionModule = _spinjitsuActiveParticles.emission;
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
		if (_ftueState == FTUE_STATE.SPINJITSU)
		{
			int num = _spinjitsuFTUEElements.Length;
			for (int i = 0; i < num; i++)
			{
				_spinjitsuFTUEElements[i].depth -= 700;
			}
		}
		_ftueState = FTUE_STATE.NONE;
	}

	protected override void OnExitScreen()
	{
		UICamera.fallThrough = null;
		base.OnExitScreen();
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_hasFailed = false;
		_showDefeated = false;
		_mouseDown = false;
		_hudElementDisplay.OnShowScreen();
		_hudChestMinigame.OnShowScreen();
		_enemyIconTracker.OnShowScreen();
		_enemyIconTrackerCity.OnShowScreen();
		_enemyIconSeenTracker.OnShowScreen();
		_enemyIconSuspiciousTracker.OnShowScreen();
		_keyIconTracker.OnShowScreen();
		_doorIconTracker.OnShowScreen();
		_switchIconTracker.OnShowScreen();
		_hudBattleMinigame.OnShowScreen();
		_swordTapTelegraph.OnShowScreen();
		_zoneTapMinigame.OnShowScreen();
		_ftueBushArrows.OnShowScreen();
		_poolMessageSpinjitsu.OnShowScreen();
		_poolMessageNumbers.OnShowScreen();
		_poolMessagePerfectAttack.OnShowScreen();
		_poolMessageTrap.OnShowScreen();
		_poolMessageExplosion.OnShowScreen();
		_poolMessageCounterAttack.OnShowScreen();
		_poolMessageDodge.OnShowScreen();
		_poolMessageDeflect.OnShowScreen();
		_poolMessageSurprise.OnShowScreen();
		_poolMessageSecondaryAttack.OnShowScreen();
		_poolMessageSneakAttack.OnShowScreen();
		_poolMessagesGreenNumbers.OnShowScreen();
		_ftueArrowSpinjitsu.enabled = false;
		_ftueArrowRoster.enabled = false;
		UICamera.fallThrough = base.gameObject;
		_screenBeginTime = Time.time;
		_ftueFade.alpha = 23f / 32f;
		_ftueFade.enabled = false;
		int num = _wuWidgets.Length;
		for (int i = 0; i < num; i++)
		{
			_wuWidgets[i].enabled = false;
		}
		int num2 = _wuTransitions.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuTransitions[j].SetStateConsideredTweenedIn(false);
			_wuTransitions[j].SetToTweenInStartColor();
			_wuTransitions[j].SetToTweenInStartPos();
		}
		_objectiveBar.OnShowScreen();
		_cooldown.fillAmount = 1f;
		_emissionModule.enabled = false;
		_wantRegisterForDeaths = true;
		int num3 = GameController.Instance.PlayerProfile.NinjaTeam.Length;
		_rosterSlotRoot.localPosition = _rosterSlotPositions[num3 - 1].localPosition;
		_startingHealthVal = 0f;
		_targetHealthVal = 1f;
		_currentHP = 1f;
		_healthChangeTime = Time.time;
		_currentSlot = _rosterSlots[0];
		_wantCharacterChangeFTUE = false;
		if (_playerTexture != null && _playerTexture.mainTexture == null)
		{
			RefreshPlayerTexture();
		}
		for (int k = 0; k < 5; k++)
		{
			if (k < num3)
			{
				int num4 = GameController.Instance.PlayerProfile.NinjaTeam[k];
				MinifigType minifigType = MinifigType.None;
				if (num4 != 0)
				{
					minifigType = Team.GetTypeForID(num4);
				}
				_rosterSlots[k].Configure(minifigType, num4);
				if (_rosterSlots[k]._pMinifigType != MinifigType.None)
				{
					_wantCharacterChangeFTUE = true;
					_tutorialDelay = 1f;
				}
			}
			else
			{
				_rosterSlots[k].SetEnabled(false);
			}
		}
		int num5 = _defeatedWidgets.Length;
		for (int l = 0; l < num5; l++)
		{
			_defeatedWidgets[l].SetStateConsideredTweenedIn(false);
			_defeatedWidgets[l].SetToTweenInStartColor();
			_defeatedWidgets[l].SetToTweenInStartScale();
		}
		_wantCharacterChangeFTUE &= !GameController.Instance.PlayerProfile._pSeenCharacterSwitchTutorial;
		_rosterSlots[0].SetSelected(true);
		_spinjitsuButton.gameObject.SetActive(GameController.Instance.PlayerProfile.TotalLevelsPlayed > 0);
		SetCurrentAvatarAsPlayer();
		OnUnpause();
	}

	public Texture GetPlayerTexture()
	{
		if (_playerTexture != null && _playerTexture.mainTexture == null)
		{
			RefreshPlayerTexture();
		}
		return (_playerTexture != null) ? _playerTexture.mainTexture : null;
	}

	protected override void Update()
	{
		base.Update();
		_hudElementDisplay.OnUpdate();
		_objectiveBar.OnUpdate();
		_hudChestMinigame.OnUpdate();
		_enemyIconTracker.OnUpdate();
		_enemyIconTrackerCity.OnUpdate();
		_enemyIconSeenTracker.OnUpdate();
		_enemyIconSuspiciousTracker.OnUpdate();
		_keyIconTracker.OnUpdate();
		_doorIconTracker.OnUpdate();
		_switchIconTracker.OnUpdate();
		_hudBattleMinigame.OnUpdate();
		_swordTapTelegraph.OnUpdate();
		_zoneTapMinigame.OnUpdate();
		_ftueBushArrows.OnUpdate();
		_poolMessageSpinjitsu.OnUpdate();
		_poolMessageNumbers.OnUpdate();
		_poolMessagePerfectAttack.OnUpdate();
		_poolMessageTrap.OnUpdate();
		_poolMessageExplosion.OnUpdate();
		_poolMessageCounterAttack.OnUpdate();
		_poolMessageDodge.OnUpdate();
		_poolMessageDeflect.OnUpdate();
		_poolMessageSurprise.OnUpdate();
		_poolMessageSecondaryAttack.OnUpdate();
		_poolMessageSneakAttack.OnUpdate();
		_poolMessagesGreenNumbers.OnUpdate();
		bool flag = Time.time - _screenBeginTime > 4f || _wantBushesFTUE;
		_ftueWait -= Time.deltaTime;
		if (_hasFailed || _showDefeated)
		{
			if (!(RealTime.time - _failTime > 2f))
			{
				return;
			}
			if (_defeatedWidgets[0]._pConsideredTweenedIn && _defeatedWidgets[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
			{
				int num = _defeatedWidgets.Length;
				for (int i = 0; i < num; i++)
				{
					_defeatedWidgets[i].TweenOut();
				}
				_failTime = RealTime.time - 1.5f;
			}
			else if (!_defeatedWidgets[0]._pConsideredTweenedIn && _defeatedWidgets[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
			{
				if (_hasFailed)
				{
					ScreenFacade instance = Facades<ScreenFacade>.Instance;
					Navigate(instance.GetScreenInstance<ScreenMap>(), false, false, OnFailComplete);
				}
				else
				{
					_showDefeated = false;
				}
			}
			return;
		}
		if (flag && _ftueState == FTUE_STATE.NONE && _ftueWait <= 0f && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			Character character = GamePlayer.Instance.Player.Character;
			if (!character.IsClimbing() && !character.IsInCombat() && !character.IsInSpinjitzu() && !character.IsHiding() && !character.IsPreparingCombat())
			{
				if (_rosterSlots[1]._button.enabled && !_rosterSlots[1]._selectedCircle.enabled)
				{
					if (!_seenSpinjitsuFTUE && GameController.Instance.PlayerProfile.TotalLevelsPlayed == 1)
					{
						_seenSpinjitsuFTUE = true;
						ChangeFTUEState(FTUE_STATE.SPINJITSU);
					}
					else if (_wantCharacterChangeFTUE)
					{
						if (_tutorialDelay > 0f)
						{
							_tutorialDelay -= Time.deltaTime;
						}
						else
						{
							_wantCharacterChangeFTUE = false;
							ChangeFTUEState(FTUE_STATE.ROSTER_CHANGE_1);
						}
					}
				}
				else if (_wantBushesFTUE)
				{
					_wantBushesFTUE = false;
					ChangeFTUEState(FTUE_STATE.BUSHES);
				}
				else
				{
					_ftueWait = ((!GameController.Instance.PlayerProfile._pSeenBushesTutorial) ? 0.1f : 1f);
				}
			}
			else
			{
				_ftueWait = ((!GameController.Instance.PlayerProfile._pSeenBushesTutorial) ? 0.1f : 0.5f);
			}
		}
		switch (_ftueState)
		{
		case FTUE_STATE.NONE:
			UpdateFtueNone();
			break;
		case FTUE_STATE.SPINJITSU:
			UpdateFtueSpinjitsu();
			break;
		case FTUE_STATE.SPINJITSU_OUT:
			UpdateFtueSpinjitsuOut();
			break;
		case FTUE_STATE.ROSTER_CHANGE_1:
			UpdateFtueRosterChange1();
			break;
		case FTUE_STATE.ROSTER_CHANGE_1_TRANSITION:
			UpdateFtueRosterChange1Transition();
			break;
		case FTUE_STATE.ROSTER_CHANGE_2:
			UpdateFtueRosterChange2();
			break;
		case FTUE_STATE.ROSTER_CHANGE_OUT:
			UpdateFtueRosterChangeOut();
			break;
		case FTUE_STATE.LEVELLING_CRU:
			UpdateFtueLevellingCru();
			break;
		case FTUE_STATE.LEVELLING_CRU_OUT:
			UpdateFtueLevellingCruOut();
			break;
		case FTUE_STATE.BUSHES:
			UpdateFtueBushes();
			break;
		case FTUE_STATE.BUSH_MOVE_TO:
			UpdateFtueBushMoveTo();
			break;
		case FTUE_STATE.BUSHES_OUT:
			UpdateFtueBushesOut();
			break;
		case FTUE_STATE.FADE_OUT:
			UpdateFtueFadeOut();
			break;
		}
		if (!GameController.Instance.PlayerProfile._pSeenBushesTutorial && !_wantBushesFTUE && _ftueState == FTUE_STATE.NONE)
		{
			int seed = GamePlayer.Instance.Level.Seed;
			if (seed == 1049)
			{
				LevelNode currentPositionNode = GamePlayer.Instance.Player.Character.CurrentPositionNode;
				if (currentPositionNode.Col >= 12 && currentPositionNode.Row <= 13)
				{
					_wantBushesFTUE = true;
				}
			}
		}
		float num2 = Time.time - _spinjitsuPulseTime;
		float num3 = 0.5f;
		float num4 = num3 * 0.5f;
		float num5 = 1.2f;
		if (num2 < num3 && _spinjitsuButton.isEnabled && _currentSpinjitsuCount > 0 && _cooldown.fillAmount == 1f)
		{
			if (num2 < num4)
			{
				float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num2, num4, 0f, 1f);
				_spinjitsuRoot.localScale = Vector3.Lerp(Vector3.one, Vector3.one * num5, t);
			}
			else
			{
				float t2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num2 - num4, num4, 0f, 1f);
				_spinjitsuRoot.localScale = Vector3.Lerp(Vector3.one * num5, Vector3.one, t2);
			}
		}
		else
		{
			_spinjitsuRoot.localScale = Vector3.one;
		}
		float num6 = Time.time - _studFXStartTime;
		if (num6 < 0.2f)
		{
			float t3 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.Linear, num6, 0.2f, 0f, 1f);
			_studCountWrapper.transform.localScale = Vector3.Lerp(_studFXStartSize, _studFXEndSize, t3);
		}
		else
		{
			_studCountWrapper.transform.localScale = _studFXEndSize;
		}
		if (_desiredCharacterSwap != -1)
		{
			_swapDelay -= Time.deltaTime;
			if (_swapDelay <= 0f)
			{
				CompleteCharacterSwap();
			}
		}
		if (_wantRegisterForDeaths)
		{
			if (GamePlayer.Instance != null)
			{
				Character character2 = GamePlayer.Instance.Player.Character;
				character2.onPlayerDead = (Character.OnPlayerDeadDelegate)Delegate.Combine(character2.onPlayerDead, new Character.OnPlayerDeadDelegate(OnPlayerDead));
			}
			_wantRegisterForDeaths = false;
		}
		float num7 = Time.time - _healthChangeTime;
		if (num7 < 0.5f)
		{
			float num8 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num7, 0.5f, 0f, 1f);
			float t4 = Mathf.Lerp(_startingHealthVal, _targetHealthVal, num8);
			_currentHP = num8;
			_healthCurrent.localPosition = Vector3.Lerp(_healthStart.localPosition, _healthEnd.localPosition, t4);
		}
		else
		{
			_healthCurrent.localPosition = Vector3.Lerp(_healthStart.localPosition, _healthEnd.localPosition, _targetHealthVal);
			_currentHP = _targetHealthVal;
		}
	}

	public void SetHealth(int current, int max)
	{
		float targetHealthVal = (float)current / (float)max;
		_healthChangeTime = Time.time;
		_startingHealthVal = _currentHP;
		_targetHealthVal = targetHealthVal;
	}

	public void OnPlayerDead()
	{
		for (int i = 0; i < GameController.Instance.PlayerProfile.NinjaTeam.Length; i++)
		{
			if (GameController.Instance.PlayerProfile.GetNinjaInTeam(i) == GamePlayer.Instance.Player.Character.NinjaID)
			{
				_rosterSlots[i].SetDead();
			}
		}
	}

	public void RespawnCharacter(int characterID)
	{
		_currentSlot.SetSelected(false);
		int num = _rosterSlots.Length;
		for (int i = 0; i < num; i++)
		{
			if (_rosterSlots[i]._pCharacterID == characterID)
			{
				_rosterSlots[i].SetSelected(true);
				_currentSlot = _rosterSlots[i];
				break;
			}
		}
		SwapNinja(_currentSlot._pCharacterID);
	}

	public void OnSpinjitsu()
	{
		if (_desiredCharacterSwap == -1)
		{
			if (_ftueState == FTUE_STATE.SPINJITSU)
			{
				Time.timeScale = 1f;
				ChangeFTUEState(FTUE_STATE.SPINJITSU_OUT);
				GamePlayer.Instance.Player.Character.StartSpinjitzu(false);
			}
			else if (_ftueState == FTUE_STATE.NONE)
			{
				GamePlayer.Instance.Player.Character.StartSpinjitzu();
			}
		}
	}

	public void SetSpinjitsuCountdown(float cooldown)
	{
		_emissionModule.enabled = cooldown < 0.9f && cooldown > 0f;
		_spinjitsuRoot.localPosition = ((!(cooldown <= 0f)) ? (UnityEngine.Random.insideUnitSphere * 10f) : Vector3.zero);
		_spinjitsuButton.isEnabled = _currentSpinjitsuCount > 0 || !(cooldown <= 0f);
		_cooldown.fillAmount = ((cooldown != 0f) ? cooldown : 1f);
		if (cooldown == 0f)
		{
			_spinjitsuPulseTime = Time.time;
		}
	}

	public void SetSpinjitsuType(ElementalPower power)
	{
		switch (power)
		{
		case ElementalPower.Earth:
			_spinjitsuIconBacking.spriteName = "SpinjitzuFill_COLE";
			break;
		case ElementalPower.Electricity:
			_spinjitsuIconBacking.spriteName = "SpinjitzuFill_JAY";
			break;
		case ElementalPower.Energy:
			_spinjitsuIconBacking.spriteName = "SpinjitzuFill_LLOYD";
			break;
		case ElementalPower.Fire:
			_spinjitsuIconBacking.spriteName = "SpinjitzuFill_KAI";
			break;
		case ElementalPower.Ice:
			_spinjitsuIconBacking.spriteName = "SpinjitzuFill_ZANE";
			break;
		case ElementalPower.Water:
			_spinjitsuIconBacking.spriteName = "SpinjitzuFill_NYA";
			break;
		}
	}

	public void SetSpinjitsuCount(int amount)
	{
		Debug.Log("SetSpinjitsuCount");
		_spinjitsuCount.text = amount.ToString();
		_spinjitsuCountShadow.text = _spinjitsuCount.text;
		if (_currentSpinjitsuCount == 0 && amount > 0)
		{
			_spinjitsuButton.isEnabled = true;
		}
		_currentSpinjitsuCount = amount;
	}

	public void OnRosterSelection(HUDRosterSlot slot)
	{
		Debug.Log("Roster Selection");
		if (GamePlayer.Instance.Player.Character.NinjaID != slot._pCharacterID && _ftueState != FTUE_STATE.ROSTER_CHANGE_1 && _ftueState != FTUE_STATE.ROSTER_CHANGE_1_TRANSITION && !GameController.Instance.MissionStats.deadNinjaIDs.Contains(slot._pCharacterID) && !GamePlayer.Instance.Player.Character.IsPreparingCombat() && !GamePlayer.Instance.Player.Character.IsInCombat() && !GamePlayer.Instance.Player.Character.IsPreparingSneakAttack() && !GamePlayer.Instance.Player.Character.IsSneakAttacking() && !GamePlayer.Instance.Player.Character.IsClimbing() && !GamePlayer.Instance.Player.Character.IsInSpinjitzu() && _desiredCharacterSwap == -1)
		{
			_currentSlot.SetSelected(false);
			slot.SetSelected(true);
			_currentSlot = slot;
			SwapNinja(slot._pCharacterID);
			if (_ftueState == FTUE_STATE.ROSTER_CHANGE_2)
			{
				ChangeFTUEState(FTUE_STATE.ROSTER_CHANGE_OUT);
			}
		}
	}

	public void OnStudCountChanged(int val, bool showFX)
	{
		_dummySB.Length = 0;
		_dummySB.Append(val);
		string text = _dummySB.ToString();
		_studCount.text = text;
		_studCountShadow.text = text;
		if (showFX)
		{
			_studFXStartTime = Time.time;
		}
	}

	private void SetCurrentAvatarAsPlayer()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		switch (playerProfile.Weapon)
		{
		case WeaponType.Axe:
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		case WeaponType.Katana:
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		case WeaponType.Nunchuck:
			_weaponIcon.spriteName = "Weapon_Jay";
			break;
		case WeaponType.Scimitar:
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		case WeaponType.Scythe:
			_weaponIcon.spriteName = "Weapon_Cole";
			break;
		case WeaponType.Shuriken:
			_weaponIcon.spriteName = "Weapon_Zane";
			break;
		case WeaponType.Staff:
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		}
		switch (playerProfile.ElementalPower)
		{
		case ElementalPower.Earth:
			_abilityIcon.spriteName = "Element_Cole";
			break;
		case ElementalPower.Electricity:
			_abilityIcon.spriteName = "Element_Jay";
			break;
		case ElementalPower.Energy:
			_abilityIcon.spriteName = "Element_Lloyd";
			break;
		case ElementalPower.Fire:
			_abilityIcon.spriteName = "Element_Kai";
			break;
		case ElementalPower.Ice:
			_abilityIcon.spriteName = "Element_Zane";
			break;
		case ElementalPower.Water:
			_abilityIcon.spriteName = "Element_Nya";
			break;
		}
		if (_playerTexture != null && _playerTexture.mainTexture == null)
		{
			RefreshPlayerTexture();
		}
		_playerTexture.enabled = true;
		_kaiTexture.enabled = false;
		_lloydTexture.enabled = false;
		_nyaTexture.enabled = false;
		_jayTexture.enabled = false;
		_zaneTexture.enabled = false;
		_coleTexture.enabled = false;
	}

	private void SetCurrentAvatarAsKai()
	{
		_playerTexture.enabled = false;
		_kaiTexture.enabled = true;
		_lloydTexture.enabled = false;
		_nyaTexture.enabled = false;
		_jayTexture.enabled = false;
		_zaneTexture.enabled = false;
		_coleTexture.enabled = false;
		_abilityIcon.spriteName = "Element_Kai";
		_weaponIcon.spriteName = "Weapon_Lloyd";
	}

	private void SetCurrentAvatarAsLloyd()
	{
		_playerTexture.enabled = false;
		_kaiTexture.enabled = false;
		_lloydTexture.enabled = true;
		_nyaTexture.enabled = false;
		_jayTexture.enabled = false;
		_zaneTexture.enabled = false;
		_coleTexture.enabled = false;
		_abilityIcon.spriteName = "Element_Lloyd";
		_weaponIcon.spriteName = "Weapon_Lloyd";
	}

	private void SetCurrentAvatarAsNya()
	{
		_playerTexture.enabled = false;
		_kaiTexture.enabled = false;
		_lloydTexture.enabled = false;
		_nyaTexture.enabled = true;
		_jayTexture.enabled = false;
		_zaneTexture.enabled = false;
		_coleTexture.enabled = false;
		_abilityIcon.spriteName = "Element_Nya";
		_weaponIcon.spriteName = "Weapon_Lloyd";
	}

	private void SetCurrentAvatarAsJay()
	{
		_playerTexture.enabled = false;
		_kaiTexture.enabled = false;
		_lloydTexture.enabled = false;
		_nyaTexture.enabled = false;
		_jayTexture.enabled = true;
		_zaneTexture.enabled = false;
		_coleTexture.enabled = false;
		_abilityIcon.spriteName = "Element_Jay";
		_weaponIcon.spriteName = "Weapon_Jay";
	}

	private void SetCurrentAvatarAsZane()
	{
		_playerTexture.enabled = false;
		_kaiTexture.enabled = false;
		_lloydTexture.enabled = false;
		_nyaTexture.enabled = false;
		_jayTexture.enabled = false;
		_zaneTexture.enabled = true;
		_coleTexture.enabled = false;
		_abilityIcon.spriteName = "Element_Zane";
		_weaponIcon.spriteName = "Weapon_Zane";
	}

	private void SetCurrentAvatarAsCole()
	{
		_playerTexture.enabled = false;
		_kaiTexture.enabled = false;
		_lloydTexture.enabled = false;
		_nyaTexture.enabled = false;
		_jayTexture.enabled = false;
		_zaneTexture.enabled = false;
		_coleTexture.enabled = true;
		_abilityIcon.spriteName = "Element_Cole";
		_weaponIcon.spriteName = "Weapon_Cole";
	}

	public void CompleteCharacterSwap()
	{
		if (_desiredCharacterSwap != -1)
		{
			CharacterBuilder.ConfigureMinifig(GamePlayer.Instance.Player.Character, _desiredCharacterSwap);
			int ninjaLevel = GameController.Instance.PlayerProfile.GetNinjaLevel(Team.GetTypeForID(_desiredCharacterSwap));
			Team.Configure(GamePlayer.Instance.Player.Character, _desiredCharacterSwap, ninjaLevel);
			(GameController.Instance.currentState as GameControllerStatePlaying).CurrentNinjaID = _desiredCharacterSwap;
			if (GamePlayer.Instance.Player.Character.GetWeapon().type == WeaponType.Shuriken && GamePlayer.Instance.Player.Character.MissilesLeft <= 0)
			{
				(GamePlayer.Instance.Player.Character.Controller as PlayerController).ChangeWeapon(WeaponType.Katana);
			}
			GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.reset);
			GamePlayer.Instance.Player.Character.Health = Mathf.RoundToInt((GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaHealthFactor(_desiredCharacterSwap) * (float)GamePlayer.Instance.Player.Character.MaxHealth);
			_desiredCharacterSwap = -1;
		}
	}

	public void SwapNinja(int ninjaID)
	{
		_swapDelay = 0.6f;
		_desiredCharacterSwap = ninjaID;
		GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.switchNinja);
		ElementalPower elementalPower = CharacterBuilder.minifigDefinitions[ninjaID].elementalPower;
		if (ninjaID == 0)
		{
			elementalPower = GameController.Instance.PlayerProfile.GetMinifigDefinition().elementalPower;
		}
		GameObject gameObject = null;
		switch (elementalPower)
		{
		case ElementalPower.Earth:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Brown"));
			break;
		case ElementalPower.Electricity:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Blue"));
			break;
		case ElementalPower.Energy:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Green"));
			break;
		case ElementalPower.Fire:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Red"));
			break;
		case ElementalPower.Ice:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion White"));
			break;
		case ElementalPower.Water:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Dark Blue"));
			break;
		default:
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/SwapNinjaFX"));
			break;
		}
		gameObject.transform.position = GamePlayer.Instance.Player.Character.transform.position;
		SwapNinjaFX component = gameObject.GetComponent<SwapNinjaFX>();
		SetSpinjitsuType(elementalPower);
		for (int i = 0; i < GameController.Instance.PlayerProfile.NinjaTeam.Length; i++)
		{
			int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam == ninjaID)
			{
				switch (Team.GetTypeForID(ninjaID))
				{
				case MinifigType.Cole:
					SetCurrentAvatarAsCole();
					break;
				case MinifigType.Jay:
					SetCurrentAvatarAsJay();
					break;
				case MinifigType.Lloyd:
					SetCurrentAvatarAsLloyd();
					break;
				case MinifigType.Nya:
					SetCurrentAvatarAsNya();
					break;
				case MinifigType.Zane:
					SetCurrentAvatarAsZane();
					break;
				case MinifigType.Kai:
					SetCurrentAvatarAsKai();
					break;
				default:
					SetCurrentAvatarAsPlayer();
					break;
				}
			}
		}
	}

	public void RefreshPlayerTexture()
	{
		try
		{
			Texture2D[] array = CharacterPreviewRenderer.Generate(new int[1], 128, 128);
			if (array != null && array.Length > 0 && array[0] != null)
			{
				array[0].wrapMode = TextureWrapMode.Clamp;
				if (_playerTexture != null)
				{
					_playerTexture.mainTexture = array[0];
				}
				if (_rosterSlots != null)
				{
					for (int i = 0; i < _rosterSlots.Length; i++)
					{
						if (_rosterSlots[i] != null && _rosterSlots[i]._pMinifigType == MinifigType.None && _rosterSlots[i]._playerIcon != null)
						{
							_rosterSlots[i]._playerIcon.mainTexture = array[0];
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Error generating player texture: " + exception);
		}
	}

	public void ClearPlayerTexture()
	{
		_playerTexture.mainTexture = null;
	}

	public void OnLevelComplete()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		Navigate(instance.GetScreenInstance<ScreenResults>());
	}

	public void OnLevelFailed()
	{
		_hasFailed = true;
	}

	public void OnCharacterDefeated()
	{
		_showDefeated = true;
		int num = _defeatedWidgets.Length;
		_failTime = RealTime.time;
		for (int i = 0; i < num; i++)
		{
			_defeatedWidgets[i].TweenIn();
		}
	}

	public void OnPause()
	{
		if (base._pCurrentTweenType != ScreenTweenType.Idle)
		{
			return;
		}
		ScreenPause screenInstance = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenPause>();
		if (screenInstance._pCurrentTweenType != ScreenTweenType.Idle || screenInstance.gameObject.activeInHierarchy)
		{
			return;
		}
		AudioPlayer.PlaySound("ui/button/click");
		GameControllerStatePlaying gameControllerStatePlaying = GameController.Instance.currentState as GameControllerStatePlaying;
		if (gameControllerStatePlaying != null)
		{
			if (gameControllerStatePlaying.IsConfirmingQuit || gameControllerStatePlaying.FadeInProgress)
			{
				return;
			}
			gameControllerStatePlaying.OnQuitPlaying();
		}
		_pauseButton.isEnabled = false;
	}

	public void OnUnpause()
	{
		_pauseButton.isEnabled = true;
	}

	public void TriggerLevellingCruFTUE()
	{
		ChangeFTUEState(FTUE_STATE.LEVELLING_CRU);
	}

	private void ChangeFTUEState(FTUE_STATE newState)
	{
		Debug.Log(string.Concat("-------- Change FTUE State : ", newState, "---------"));
		_ftueState = newState;
		_ftueStateChangeTime = RealTime.time;
		switch (_ftueState)
		{
		case FTUE_STATE.NONE:
			StartFtueNone();
			break;
		case FTUE_STATE.SPINJITSU:
			StartFtueSpinjitsu();
			break;
		case FTUE_STATE.SPINJITSU_OUT:
			StartFtueSpinjitsuOut();
			break;
		case FTUE_STATE.ROSTER_CHANGE_1:
			StartFtueRosterChange1();
			break;
		case FTUE_STATE.ROSTER_CHANGE_1_TRANSITION:
			StartFtueRosterChange1Transition();
			break;
		case FTUE_STATE.ROSTER_CHANGE_2:
			StartFtueRosterChange2();
			break;
		case FTUE_STATE.ROSTER_CHANGE_OUT:
			StartFtueRosterChangeOut();
			break;
		case FTUE_STATE.LEVELLING_CRU:
			StartFtueLevellingCru();
			break;
		case FTUE_STATE.LEVELLING_CRU_OUT:
			StartFtueLevellingCruOut();
			break;
		case FTUE_STATE.FADE_OUT:
			StartFtueFadeOut();
			break;
		case FTUE_STATE.BUSHES:
			StartFtueBushes();
			break;
		case FTUE_STATE.BUSH_MOVE_TO:
			StartFtueBushMoveTo();
			break;
		case FTUE_STATE.BUSHES_OUT:
			StartFtueBushesOut();
			break;
		}
		_pauseButton.isEnabled = _ftueState == FTUE_STATE.NONE;
	}

	private void StartFtueNone()
	{
	}

	private void UpdateFtueNone()
	{
	}

	private void StartFtueSpinjitsu()
	{
		Debug.Log("StartFtueSpinjitsu");
		Time.timeScale = 0f;
		_ftueFade.enabled = true;
		UICamera.fallThrough = base.gameObject;
		int num = _spinjitsuFTUEElements.Length;
		for (int i = 0; i < num; i++)
		{
			_spinjitsuFTUEElements[i].depth += 700;
		}
		_wuMessage.text = Localise("FTUE.Spinjitsu");
		int num2 = _wuWidgets.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuWidgets[j].enabled = true;
		}
		int num3 = _wuTransitions.Length;
		for (int k = 0; k < num3; k++)
		{
			_wuTransitions[k].TweenIn();
		}
		_ftueArrowRootPos = _ftueArrowSpinjitsuRoot.localPosition;
		_ftueArrowSpinjitsu.enabled = true;
		_ftueArrowSpinjitsu.mainTexture = Resources.Load<Texture>("FTUI_RedArrow");
		_ftueSpinjitsuArrowTransition.TweenIn();
	}

	private void UpdateFtueSpinjitsu()
	{
		float num = RealTime.time - _ftueStateChangeTime;
		float num2 = Mathf.Sin(num * 6f);
		_ftueSpinjitsuScaleRoot.localScale = Vector3.one + Vector3.one * num2 * 0.05f;
		_ftueArrowSpinjitsuRoot.localPosition = _ftueArrowRootPos + _ftueArrowSpinjitsuRoot.up * (1f - num2) * 10f;
	}

	private void StartFtueSpinjitsuOut()
	{
		_ftueSpinjitsuScaleRoot.localScale = Vector3.one;
		int num = _spinjitsuFTUEElements.Length;
		for (int i = 0; i < num; i++)
		{
			_spinjitsuFTUEElements[i].depth -= 700;
		}
		int num2 = _wuTransitions.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuTransitions[j].TweenOut();
		}
		_ftueSpinjitsuArrowTransition.TweenOut();
	}

	private void UpdateFtueSpinjitsuOut()
	{
		if (!_wuTransitions[0]._pConsideredTweenedIn && _wuTransitions[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			int num = _wuWidgets.Length;
			for (int i = 0; i < num; i++)
			{
				_wuWidgets[i].enabled = false;
			}
			_ftueArrowSpinjitsu.mainTexture = null;
			_ftueArrowSpinjitsu.enabled = false;
			ChangeFTUEState(FTUE_STATE.FADE_OUT);
		}
	}

	private void StartFtueRosterChange1()
	{
		Time.timeScale = 0f;
		_ftueFade.enabled = true;
		UICamera.fallThrough = base.gameObject;
		int num = _spinjitsuFTUEElements.Length;
		for (int i = 0; i < num; i++)
		{
			_rosterFTUEElements[i].depth += 700;
		}
		_wuMessage.text = Localise("FTUE.ChangeCharacter1");
		int num2 = _wuWidgets.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuWidgets[j].enabled = true;
		}
		int num3 = _wuTransitions.Length;
		for (int k = 0; k < num3; k++)
		{
			_wuTransitions[k].TweenIn();
		}
	}

	private void UpdateFtueRosterChange1()
	{
	}

	private void StartFtueRosterChange1Transition()
	{
		int num = _wuTransitions.Length;
		for (int i = 0; i < num; i++)
		{
			_wuTransitions[i].TweenOut();
		}
	}

	private void UpdateFtueRosterChange1Transition()
	{
		if (!_wuTransitions[0]._pConsideredTweenedIn && _wuTransitions[0]._pTweenType != ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			_wuMessage.text = Localise("FTUE.ChangeCharacter2");
			int num = _wuTransitions.Length;
			for (int i = 0; i < num; i++)
			{
				_wuTransitions[i].TweenIn();
			}
		}
		else if (_wuTransitions[0]._pConsideredTweenedIn && _wuTransitions[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			ChangeFTUEState(FTUE_STATE.ROSTER_CHANGE_2);
		}
	}

	private void StartFtueRosterChange2()
	{
		_ftueArrowRootPos = _ftueArrowRosterRoot.localPosition;
		_ftueArrowRoster.enabled = true;
		_ftueArrowRoster.mainTexture = Resources.Load<Texture>("FTUI_RedArrow");
		_ftueRosterArrowTransition.TweenIn();
	}

	private void UpdateFtueRosterChange2()
	{
		float num = RealTime.time - _ftueStateChangeTime;
		float num2 = Mathf.Sin(num * 6f);
		_ftueRosterScaleRoot.localScale = Vector3.one + Vector3.one * num2 * 0.05f;
		_ftueArrowRosterRoot.localPosition = _ftueArrowRootPos + _ftueArrowRosterRoot.up * (1f - num2) * 10f;
	}

	private void StartFtueRosterChangeOut()
	{
		_ftueRosterScaleRoot.localScale = Vector3.one;
		Time.timeScale = 1f;
		GameController.Instance.PlayerProfile._pSeenCharacterSwitchTutorial = true;
		int num = _spinjitsuFTUEElements.Length;
		for (int i = 0; i < num; i++)
		{
			_rosterFTUEElements[i].depth -= 700;
		}
		int num2 = _wuTransitions.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuTransitions[j].TweenOut();
		}
		_ftueRosterArrowTransition.TweenOut();
	}

	private void UpdateFtueRosterChangeOut()
	{
		if (!_wuTransitions[0]._pConsideredTweenedIn && _wuTransitions[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			int num = _wuWidgets.Length;
			for (int i = 0; i < num; i++)
			{
				_wuWidgets[i].enabled = false;
			}
			_ftueArrowRoster.enabled = false;
			_ftueArrowRoster.mainTexture = null;
			ChangeFTUEState(FTUE_STATE.FADE_OUT);
		}
	}

	private void StartFtueLevellingCru()
	{
		Time.timeScale = 0f;
		_ftueFade.enabled = true;
		UICamera.fallThrough = base.gameObject;
		_wuMessage.text = Localise("FTUE.LevellingCru");
		int num = _wuWidgets.Length;
		for (int i = 0; i < num; i++)
		{
			_wuWidgets[i].enabled = true;
		}
		int num2 = _wuTransitions.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuTransitions[j].TweenIn();
		}
	}

	private void UpdateFtueLevellingCru()
	{
	}

	private void StartFtueLevellingCruOut()
	{
		_ftueRosterScaleRoot.localScale = Vector3.one;
		Time.timeScale = 1f;
		int num = _wuTransitions.Length;
		for (int i = 0; i < num; i++)
		{
			_wuTransitions[i].TweenOut();
		}
	}

	private void UpdateFtueLevellingCruOut()
	{
		if (!_wuTransitions[0]._pConsideredTweenedIn && _wuTransitions[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			int num = _wuWidgets.Length;
			for (int i = 0; i < num; i++)
			{
				_wuWidgets[i].enabled = false;
			}
			ChangeFTUEState(FTUE_STATE.FADE_OUT);
		}
	}

	private void StartFtueFadeOut()
	{
	}

	private void UpdateFtueFadeOut()
	{
		float num = RealTime.time - _ftueStateChangeTime;
		if (num < 0.5f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			_ftueFade.alpha = Mathf.Lerp(23f / 32f, 0f, t);
		}
		else
		{
			_ftueFade.enabled = false;
			ChangeFTUEState(FTUE_STATE.NONE);
		}
	}

	private void StartFtueBushes()
	{
		Time.timeScale = 0f;
		_ftueFade.enabled = true;
		UICamera.fallThrough = base.gameObject;
		_wuMessage.text = Localise("FTUE.Bushes");
		int num = _wuWidgets.Length;
		for (int i = 0; i < num; i++)
		{
			_wuWidgets[i].enabled = true;
		}
		int num2 = _wuTransitions.Length;
		for (int j = 0; j < num2; j++)
		{
			_wuTransitions[j].TweenIn();
		}
		_ftueWait = 0f;
	}

	private void UpdateFtueBushes()
	{
	}

	public void HandleBushFTUE()
	{
		if (_ftueState == FTUE_STATE.BUSH_MOVE_TO)
		{
			ChangeFTUEState(FTUE_STATE.BUSHES_OUT);
		}
	}

	private void StartFtueBushMoveTo()
	{
		int num = 0;
		LevelNode[] nodesWithObject = GamePlayer.Instance.Level.GetNodesWithObject(LevelNodeObjectType.Bush);
		LevelNode currentPositionNode = GamePlayer.Instance.Player.Character.CurrentPositionNode;
		int num2 = nodesWithObject.Length;
		for (int i = 0; i < num2; i++)
		{
			if (num >= 3)
			{
				break;
			}
			if (Mathf.Abs(nodesWithObject[i].Col - currentPositionNode.Col) <= 5 && Mathf.Abs(nodesWithObject[i].Row - currentPositionNode.Col) <= 5)
			{
				GameObject gameObject = nodesWithObject[i].Objects[0];
				if (gameObject != null)
				{
					_ftueBushArrows.DisplayIcon(gameObject.transform);
					num++;
				}
			}
		}
		_ftueRosterScaleRoot.localScale = Vector3.one;
		int num3 = _wuTransitions.Length;
		for (int j = 0; j < num3; j++)
		{
			_wuTransitions[j].TweenOut();
		}
	}

	private void UpdateFtueBushMoveTo()
	{
		float num = RealTime.time - _ftueStateChangeTime;
		if (num < 0.5f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			_ftueFade.alpha = Mathf.Lerp(23f / 32f, 0f, t);
			return;
		}
		_ftueFade.enabled = false;
		if (GamePlayer.Instance.Player.Character.IsHiding())
		{
			HandleBushFTUE();
		}
		else if (GamePlayer.Instance.Player.Character.CurrentPathTarget != null && GamePlayer.Instance.Player.Character.CurrentPathTarget.levelNode.ContainsObjectType(LevelNodeObjectType.Bush))
		{
			Debug.Log("Going To Bush Already!");
			HandleBushFTUE();
		}
	}

	private void StartFtueBushesOut()
	{
		Time.timeScale = 1f;
		GameController.Instance.PlayerProfile._pSeenBushesTutorial = true;
		_ftueBushArrows.DismissAll();
	}

	private void UpdateFtueBushesOut()
	{
		if (!_wuTransitions[0]._pConsideredTweenedIn && _wuTransitions[0]._pTweenType == ScreenObjectTransitionWidget.TweenType.IDLE)
		{
			int num = _wuWidgets.Length;
			for (int i = 0; i < num; i++)
			{
				_wuWidgets[i].enabled = false;
			}
			ChangeFTUEState(FTUE_STATE.NONE);
		}
	}

	private void HandleFtueProgression()
	{
		if (_ftueState == FTUE_STATE.ROSTER_CHANGE_1)
		{
			ChangeFTUEState(FTUE_STATE.ROSTER_CHANGE_1_TRANSITION);
		}
		else if (_ftueState == FTUE_STATE.LEVELLING_CRU)
		{
			ChangeFTUEState(FTUE_STATE.LEVELLING_CRU_OUT);
		}
		else if (_ftueState == FTUE_STATE.BUSHES)
		{
			ChangeFTUEState(FTUE_STATE.BUSH_MOVE_TO);
		}
	}

	private void OnFailComplete()
	{
		SceneManager.LoadScene("Empty");
	}

	private void OnClick()
	{
		if (_ftueState != FTUE_STATE.NONE)
		{
			HandleFtueProgression();
		}
	}

	private void OnPress(bool isDown)
	{
		_mouseDown = isDown;
		if (isDown)
		{
			_mouseDownTime = Time.timeSinceLevelLoad;
		}
	}
}
