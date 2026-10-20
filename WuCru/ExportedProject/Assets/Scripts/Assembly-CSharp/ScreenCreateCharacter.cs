using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenCreateCharacter : ScreenBase
{
	public class DecalDefinition
	{
		public int _requiredLevel;

		public string _torso;

		public string _pelvis;

		public string _legs;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			DecalDefinition decalDefinition = (DecalDefinition)obj;
			return _requiredLevel == decalDefinition._requiredLevel && _torso == decalDefinition._torso && _pelvis == decalDefinition._pelvis && _legs == decalDefinition._legs;
		}

		public override int GetHashCode()
		{
			return _requiredLevel.GetHashCode() ^ _torso.GetHashCode() ^ _pelvis.GetHashCode() ^ _legs.GetHashCode();
		}
	}

	public GameObject _menuCharacterPrefab;

	private const int NON_PLAYER_WEAPONS_COUNT = 11;

	public UIButton _goButton;

	public UISprite _hairColor;

	public UISprite _maskColor;

	public UISprite _topColor;

	public GoldenTrailFX _hairGoldenTrailFX;

	public GoldenTrailFX _faceGoldenTrailFX;

	public GoldenTrailFX _torsoGoldenTrailFX;

	public GoldenTrailFX _helmetGoldenTrailFX;

	public UISprite _weaponIcon;

	public UISprite _abilityIcon;

	public UISprite _headIcon;

	public UISprite _hairIcon;

	public UISprite _topIcon;

	public UISprite _maskIcon;

	public UISprite _headPadlock;

	public UISprite _hairPadlock;

	public UISprite _topPadlock;

	public UISprite _helmetPadlock;

	public Color32 _headColor;

	public Color32 _handColor;

	public Color32 _quiverColor;

	public Color32[] _suitColors;

	public Color32[] _hairColors;

	public GameObject[] _elementalPowerEffects;

	public Transform _panelAnchor;

	private int _genderIndex;

	private int _faceIndex;

	private int _hairIndex;

	private int _hairColorIndex;

	private int _helmetIndex;

	private int _helmetColorIndex;

	private int _colorIndex;

	private int _suitIndex;

	private int _weaponIndex;

	private int _powerIndex;

	private int _maxSuitCount;

	private bool _showHelmet;

	private List<DecalDefinition> _decals = new List<DecalDefinition>();

	private List<string> _helmets = new List<string>();

	private List<string> _faces = new List<string>();

	private List<string> _hair = new List<string>();

	private List<bool> _isVanillaFace = new List<bool>();

	private List<bool> _isVanillaHair = new List<bool>();

	private List<bool> _isVanillaTorso = new List<bool>();

	private List<bool> _isVanillaHelmet = new List<bool>();

	private List<bool> _isFaceUnlocked = new List<bool>();

	private List<bool> _isTorsoUnlocked = new List<bool>();

	private List<bool> _isHairUnlocked = new List<bool>();

	private List<bool> _isHelmetUnlocked = new List<bool>();

	private List<int> _unlockableFaceIndex = new List<int>();

	private List<int> _unlockableHairIndex = new List<int>();

	private List<int> _unlockableTorsoIndex = new List<int>();

	private List<int> _unlockableHelmetIndex = new List<int>();

	private int _numVanillaFaces;

	private int _numVanillaTorsos;

	private int _numVanillaHair;

	private int _numVanillaHelmet;

	private int _numUnseenFaces;

	private int _numUnseenTorsos;

	private int _numUnseenHairs;

	private int _numUnseenHelmet;

	private int _powerAnimHash;

	private int _idleAnimHash;

	private int _resetAnimHash;

	private int _attackAnimHash;

	private WeaponType[] _weapons;

	private List<string> _helmetReplacesHead = new List<string>();

	private List<string> _helmetHidesHair = new List<string>();

	private ElementalPower[] _powers;

	private GameObject _instantiatedCharacter;

	private GameObject _createdCamera;

	private GameObject _currentElementalPowerEffect;

	private MenuCharacter _menuCharacter;

	private Animator _animRef;

	private int _targetAnimHash;

	private bool _playSFX = true;

	private int _fakeRandomIndex;

	private float _fakeRandomTimer;

	public Shader _halfLambertNoFogVertexColor;

	public Shader _halfLambertNoFogTextured;

	public Shader _halfLambertVertexColor;

	public Shader _halfLambertTextured;

	private string _pSelectedFaceTexture
	{
		get
		{
			return _faces[_faceIndex];
		}
	}

	private string _pSelectedTorsoTexture
	{
		get
		{
			return _decals[_suitIndex]._torso;
		}
	}

	private string _pSelectedPelvisTexture
	{
		get
		{
			return _decals[_suitIndex]._pelvis;
		}
	}

	private string _pSelectedLegsTexture
	{
		get
		{
			return _decals[_suitIndex]._legs;
		}
	}

	private Color32 _pSelectedSuitColor
	{
		get
		{
			return _suitColors[_colorIndex];
		}
	}

	private Color32 _pSelectedHairColor
	{
		get
		{
			return _hairColors[_hairColorIndex];
		}
	}

	private Color32 _pSelectedHelmetColor
	{
		get
		{
			return _suitColors[_helmetColorIndex];
		}
	}

	private string _pSelectedHairModel
	{
		get
		{
			return _hair[_hairIndex];
		}
	}

	private string _pSelectedHelmetModel
	{
		get
		{
			return (_helmetIndex <= -1) ? string.Empty : _helmets[_helmetIndex];
		}
	}

	private WeaponType _pSelectedWeapon
	{
		get
		{
			return _weapons[_weaponIndex];
		}
	}

	private ElementalPower _pSelectedPower
	{
		get
		{
			return _powers[_powerIndex];
		}
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		Time.timeScale = 1f;
		_targetAnimHash = -1;
		_powerAnimHash = Animator.StringToHash("power");
		_idleAnimHash = Animator.StringToHash("Idle");
		_resetAnimHash = Animator.StringToHash("reset");
		_attackAnimHash = Animator.StringToHash("attack");
		_decals.Clear();
		_helmets.Clear();
		_faces.Clear();
		_hair.Clear();
		_isVanillaFace.Clear();
		_isVanillaHair.Clear();
		_isVanillaTorso.Clear();
		_isVanillaHelmet.Clear();
		_isFaceUnlocked.Clear();
		_isTorsoUnlocked.Clear();
		_isHairUnlocked.Clear();
		_isHelmetUnlocked.Clear();
		_unlockableFaceIndex.Clear();
		_unlockableHairIndex.Clear();
		_unlockableTorsoIndex.Clear();
		_unlockableHelmetIndex.Clear();
		_hairGoldenTrailFX.SetVisible(false);
		_faceGoldenTrailFX.SetVisible(false);
		_torsoGoldenTrailFX.SetVisible(false);
		_helmetGoldenTrailFX.SetVisible(false);
		_instantiatedCharacter = UnityEngine.Object.Instantiate(_menuCharacterPrefab);
		_menuCharacter = _instantiatedCharacter.GetComponent<MenuCharacter>();
		_animRef = _menuCharacter.GetComponentInChildren<Animator>();
		float num = (float)Screen.width / (float)Screen.height;
		_createdCamera = new GameObject("MenuCameraCam");
		Camera camera = _createdCamera.AddComponent<Camera>();
		_createdCamera.transform.position = _instantiatedCharacter.transform.position + Vector3.back * 1.5f + Vector3.up * 0.75f;
		camera.rect = new Rect(0f, 0f, (!(num > 1.555f)) ? 0.6f : 0.7f, 1f);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		if (instance != null)
		{
			Camera component = instance._mainUICamera.GetComponent<Camera>();
			if (component != null)
			{
				camera.rect = new Rect(0f, 0f, component.WorldToViewportPoint(_panelAnchor.transform.position).x, 1f);
			}
		}
		camera.clearFlags = CameraClearFlags.Depth;
		camera.depth = 30f;
		camera.cullingMask = (1 << LayerMask.NameToLayer("MenuCharacter")) | (1 << LayerMask.NameToLayer("TransparentFX"));
		bool flag = GameController.Instance.PlayerProfile.TotalLevelsPlayed < 1;
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		_showHelmet = playerProfile.ShowHelmet;
		TextAsset textAsset = Resources.Load("player") as TextAsset;
		using (StringReader stringReader = new StringReader(textAsset.text))
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			stringReader.ReadLine();
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split(',');
				if (!string.IsNullOrEmpty(array[0]))
				{
					list.Add(array[0]);
				}
				if (!string.IsNullOrEmpty(array[1]))
				{
					list2.Add(array[1]);
				}
				if (!string.IsNullOrEmpty(array[2]))
				{
					list3.Add(array[2]);
				}
				if (!string.IsNullOrEmpty(array[3]))
				{
					list4.Add(array[3]);
				}
				if (!string.IsNullOrEmpty(array[4]))
				{
					_helmets.Add(array[4]);
				}
				if (!string.IsNullOrEmpty(array[4]) && array[5] == "TRUE")
				{
					_helmetHidesHair.Add(array[4]);
				}
				int result;
				if (int.TryParse(array[6], out result))
				{
					_decals.Add(new DecalDefinition
					{
						_requiredLevel = result,
						_torso = array[9],
						_pelvis = array[8],
						_legs = array[7]
					});
					if (result <= GameController.Instance.PlayerProfile.Level)
					{
						_maxSuitCount = _decals.Count;
					}
				}
			}
			_faces.AddRange(list);
			_faces.AddRange(list2);
			_hair.AddRange(list3);
			_hair.AddRange(list4);
		}
		WeaponType[] array2 = Enum.GetValues(typeof(WeaponType)) as WeaponType[];
		_weapons = new WeaponType[array2.Length - 1];
		Array.Copy(array2, 1, _weapons, 0, _weapons.Length - 11);
		ElementalPower[] array3 = Enum.GetValues(typeof(ElementalPower)) as ElementalPower[];
		_powers = new ElementalPower[array3.Length - 1];
		Array.Copy(array3, 1, _powers, 0, _powers.Length);
		int count = _faces.Count;
		for (int i = 0; i < count; i++)
		{
			_isVanillaFace.Add(true);
			_isFaceUnlocked.Add(true);
			_unlockableFaceIndex.Add(-1);
		}
		int count2 = _decals.Count;
		for (int j = 0; j < count2; j++)
		{
			_isTorsoUnlocked.Add(true);
			_isVanillaTorso.Add(true);
			_unlockableTorsoIndex.Add(-1);
		}
		int count3 = _hair.Count;
		for (int k = 0; k < count3; k++)
		{
			_isHairUnlocked.Add(true);
			_isVanillaHair.Add(true);
			_unlockableHairIndex.Add(-1);
		}
		int count4 = _helmets.Count;
		for (int l = 0; l < count4; l++)
		{
			_isHelmetUnlocked.Add(true);
			_isVanillaHelmet.Add(true);
			_unlockableHelmetIndex.Add(-1);
		}
		_numVanillaHair = _isVanillaHair.Count;
		_numVanillaFaces = _isVanillaFace.Count;
		_numVanillaTorsos = _isVanillaTorso.Count;
		_numVanillaHelmet = _isVanillaHelmet.Count;
		_numUnseenFaces = 0;
		_numUnseenHairs = 0;
		_numUnseenTorsos = 0;
		_numUnseenHelmet = 0;
		UnlockableMinifigParts.PartDefinition[] partDefinitions = UnlockableMinifigParts._pInstance._partDefinitions;
		int num2 = partDefinitions.Length;
		for (int m = 0; m < num2; m++)
		{
			if (partDefinitions[m]._partType == UnlockableMinifigParts.PartDefinition.PART_TYPE.FACE)
			{
				_faces.Add(partDefinitions[m]._associatedPaths[0]);
				_isVanillaFace.Add(false);
				bool flag2 = GameController.Instance.PlayerProfile.HasCollectedMinifigPartByIndex(m);
				_isFaceUnlocked.Add(flag2);
				if (!GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(m) && flag2)
				{
					_numUnseenFaces++;
				}
				_unlockableFaceIndex.Add(m);
			}
			if (partDefinitions[m]._partType == UnlockableMinifigParts.PartDefinition.PART_TYPE.HAIR)
			{
				_hair.Add(partDefinitions[m]._associatedPaths[0]);
				_isVanillaHair.Add(false);
				bool flag3 = GameController.Instance.PlayerProfile.HasCollectedMinifigPartByIndex(m);
				_isHairUnlocked.Add(flag3);
				if (!GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(m) && flag3)
				{
					_numUnseenHairs++;
				}
				_unlockableHairIndex.Add(m);
			}
			if (partDefinitions[m]._partType == UnlockableMinifigParts.PartDefinition.PART_TYPE.HELMET)
			{
				_helmets.Add(partDefinitions[m]._associatedPaths[0]);
				_isVanillaHelmet.Add(false);
				bool flag4 = GameController.Instance.PlayerProfile.HasCollectedMinifigPartByIndex(m);
				_isHelmetUnlocked.Add(flag4);
				if (!GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(m) && flag4)
				{
					_numUnseenHelmet++;
				}
				if (partDefinitions[m]._helmetHidesHair)
				{
					_helmetHidesHair.Add(partDefinitions[m]._associatedPaths[0]);
				}
				_unlockableHelmetIndex.Add(m);
			}
			if (partDefinitions[m]._partType == UnlockableMinifigParts.PartDefinition.PART_TYPE.TORSO)
			{
				_isVanillaTorso.Add(false);
				bool flag5 = GameController.Instance.PlayerProfile.HasCollectedMinifigPartByIndex(m);
				_isTorsoUnlocked.Add(flag5);
				if (!GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(m) && flag5)
				{
					_numUnseenTorsos++;
				}
				_unlockableTorsoIndex.Add(m);
				_decals.Add(new DecalDefinition
				{
					_requiredLevel = 0,
					_torso = partDefinitions[m]._associatedPaths[0],
					_pelvis = partDefinitions[m]._associatedPaths[1],
					_legs = partDefinitions[m]._associatedPaths[2]
				});
			}
		}
		_maxSuitCount = _decals.Count;
		_faceGoldenTrailFX.SetVisible(_numUnseenFaces > 0);
		_hairGoldenTrailFX.SetVisible(_numUnseenHairs > 0);
		_torsoGoldenTrailFX.SetVisible(_numUnseenTorsos > 0);
		_helmetGoldenTrailFX.SetVisible(_numUnseenHelmet > 0);
		if (flag)
		{
			GameController.Instance.PlayerProfile.MarkAmuzoObjectives();
			_playSFX = false;
			_fakeRandomIndex = 10;
			_fakeRandomTimer = 0.25f;
			OnRandomise();
			_playSFX = true;
			return;
		}
		_faceIndex = _faces.IndexOf(playerProfile.FaceTexture);
		if (_faceIndex == -1)
		{
			_faceIndex = 0;
		}
		_hairIndex = _hair.IndexOf(playerProfile.HairModel);
		if (_hairIndex == -1)
		{
			_hairIndex = 0;
		}
		_helmetIndex = _helmets.IndexOf(playerProfile.HelmetModel);
		if (_helmetIndex == -1)
		{
			_helmetIndex = 0;
		}
		_suitIndex = _decals.IndexOf(new DecalDefinition
		{
			_torso = playerProfile.TorsoTexture,
			_pelvis = playerProfile.PelvisTexture,
			_legs = playerProfile.LegsTexture
		});
		if (_suitIndex == -1)
		{
			_suitIndex = 0;
		}
		_colorIndex = Array.IndexOf(_suitColors, playerProfile.TorsoColor);
		if (_colorIndex == -1)
		{
			_colorIndex = 0;
		}
		_hairColorIndex = Array.IndexOf(_hairColors, playerProfile.HairColor);
		if (_hairColorIndex == -1)
		{
			_hairColorIndex = 0;
		}
		_helmetColorIndex = Array.IndexOf(_suitColors, playerProfile.HelmetColor);
		if (_helmetColorIndex == -1)
		{
			_helmetColorIndex = 0;
		}
		_weaponIndex = (int)(playerProfile.Weapon - 1);
		if (_weaponIndex == -1)
		{
			_weaponIndex = 0;
		}
		_powerIndex = (int)(playerProfile.ElementalPower - 1);
		if (_powerIndex == -1)
		{
			_powerIndex = 0;
		}
		BuildCharacter();
	}

	protected override void Update()
	{
		base.Update();
		if (_fakeRandomIndex > 0)
		{
			_fakeRandomTimer -= Time.deltaTime;
			if (_fakeRandomTimer <= 0f)
			{
				_fakeRandomIndex--;
				_fakeRandomTimer = 0.2f;
				int forceHelmetIndex = -1;
				if (_fakeRandomIndex == 6 || _fakeRandomIndex == 2)
				{
					_showHelmet = true;
					forceHelmetIndex = 7;
				}
				else if (_fakeRandomIndex == 3 || _fakeRandomIndex == 8)
				{
					_showHelmet = true;
					forceHelmetIndex = 5;
				}
				else
				{
					_showHelmet = false;
				}
				OnRandomise(_fakeRandomIndex == 0, forceHelmetIndex);
			}
		}
		if (_targetAnimHash != -1 && _animRef.GetCurrentAnimatorStateInfo(0).shortNameHash == _idleAnimHash)
		{
			SetAnimTrigger(_targetAnimHash);
		}
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
			int totalLevelsPlayed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
			if (chapterCursor <= 1 && totalLevelsPlayed == 0)
			{
				ScreenTitle screenInstance = instance.GetScreenInstance<ScreenTitle>();
				Navigate(screenInstance);
				return;
			}
			ScreenMap screenInstance2 = instance.GetScreenInstance<ScreenMap>();
			screenInstance2._pPlayerMugshot = null;
			SceneManager.LoadScene("Empty");
			ScreenStats screenInstance3 = instance.GetScreenInstance<ScreenStats>();
			Navigate(screenInstance3);
		}
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
		UnityEngine.Object.Destroy(_instantiatedCharacter);
		UnityEngine.Object.Destroy(_createdCamera);
	}

	public void MakeCharacterAnimate()
	{
		SetAnimTrigger(_attackAnimHash);
	}

	public void OnRandomise(bool wantSound = true, int forceHelmetIndex = -1)
	{
		_hairIndex = UnityEngine.Random.Range(0, _numVanillaHair);
		_faceIndex = UnityEngine.Random.Range(0, _numVanillaFaces);
		_suitIndex = UnityEngine.Random.Range(0, _numVanillaTorsos);
		_helmetIndex = UnityEngine.Random.Range(0, _numVanillaHelmet);
		_powerIndex = UnityEngine.Random.Range(0, _powers.Length);
		_weaponIndex = UnityEngine.Random.Range(0, _weapons.Length - 11);
		_helmetColorIndex = UnityEngine.Random.Range(0, _suitColors.Length);
		_colorIndex = UnityEngine.Random.Range(0, _suitColors.Length);
		_hairColorIndex = UnityEngine.Random.Range(0, _hairColors.Length);
		if (forceHelmetIndex > -1)
		{
			_helmetIndex = forceHelmetIndex;
		}
		if (_playSFX && wantSound)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
		}
		BuildCharacter();
	}

	public void OnPrevHead()
	{
		if (_fakeRandomIndex <= 0)
		{
			Decrement(ref _faceIndex, _faces.Count);
			_showHelmet = false;
			BuildCharacter();
		}
	}

	public void OnNextHead()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _faceIndex, _faces.Count);
			_showHelmet = false;
			BuildCharacter();
		}
	}

	public void OnPrevHair()
	{
		if (_fakeRandomIndex <= 0)
		{
			Decrement(ref _hairIndex, _hair.Count);
			_showHelmet = false;
			BuildCharacter();
		}
	}

	public void OnNextHair()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _hairIndex, _hair.Count);
			_showHelmet = false;
			BuildCharacter();
		}
	}

	public void OnNextHairColor()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _hairColorIndex, _hairColors.Length);
			_showHelmet = false;
			BuildCharacter();
			UpdateButtonColors();
		}
	}

	public void OnPrevMask()
	{
		if (_fakeRandomIndex <= 0)
		{
			Decrement(ref _helmetIndex, _helmets.Count);
			_showHelmet = true;
			BuildCharacter();
		}
	}

	public void OnNextMask()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _helmetIndex, _helmets.Count);
			_showHelmet = true;
			BuildCharacter();
		}
	}

	public void OnNextMaskColor()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _helmetColorIndex, _suitColors.Length);
			_showHelmet = true;
			BuildCharacter();
			UpdateButtonColors();
		}
	}

	public void OnPrevTop()
	{
		if (_fakeRandomIndex <= 0)
		{
			Decrement(ref _suitIndex, _maxSuitCount);
			BuildCharacter();
		}
	}

	public void OnNextTop()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _suitIndex, _maxSuitCount);
			BuildCharacter();
		}
	}

	public void OnNextTopColor()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _colorIndex, _suitColors.Length);
			BuildCharacter();
			UpdateButtonColors();
		}
	}

	public void OnPrevWeapon()
	{
		if (_fakeRandomIndex <= 0)
		{
			Decrement(ref _weaponIndex, _weapons.Length - 11);
			BuildCharacter();
		}
	}

	public void OnNextWeapon()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _weaponIndex, _weapons.Length - 11);
			BuildCharacter();
		}
	}

	public void OnPrevElement()
	{
		if (_fakeRandomIndex <= 0)
		{
			Decrement(ref _powerIndex, _powers.Length);
			BuildCharacter();
			OnChangeElement();
		}
	}

	public void OnNextElement()
	{
		if (_fakeRandomIndex <= 0)
		{
			Increment(ref _powerIndex, _powers.Length);
			BuildCharacter();
			OnChangeElement();
		}
	}

	private void OnChangeElement()
	{
		if (_fakeRandomIndex <= 0)
		{
			if (_currentElementalPowerEffect != null)
			{
				UnityEngine.Object.Destroy(_currentElementalPowerEffect);
			}
			_currentElementalPowerEffect = UnityEngine.Object.Instantiate(_elementalPowerEffects[_powerIndex]);
			_currentElementalPowerEffect.transform.SetParent(_menuCharacter.leftHandTransform, false);
			_currentElementalPowerEffect.transform.localPosition = Vector3.zero;
			SetAnimTrigger(_powerAnimHash);
		}
	}

	public void OnCreateCharacter()
	{
		if (_fakeRandomIndex <= 0)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
			playerProfile.FaceTexture = _pSelectedFaceTexture;
			playerProfile.HairModel = _pSelectedHairModel;
			playerProfile.HelmetModel = _pSelectedHelmetModel;
			playerProfile.HelmetHidesHair = _helmetHidesHair.Contains(_pSelectedHelmetModel);
			playerProfile.TorsoTexture = _pSelectedTorsoTexture;
			playerProfile.PelvisTexture = _pSelectedPelvisTexture;
			playerProfile.LegsTexture = _pSelectedLegsTexture;
			playerProfile.HeadColor = _headColor;
			playerProfile.HandColor = _handColor;
			playerProfile.ArmColor = _pSelectedSuitColor;
			playerProfile.LegColor = _pSelectedSuitColor;
			playerProfile.TorsoColor = _pSelectedSuitColor;
			playerProfile.PelvisColor = _pSelectedSuitColor;
			playerProfile.HelmetColor = _pSelectedHelmetColor;
			playerProfile.QuiverColor = _quiverColor;
			playerProfile.HairColor = _pSelectedHairColor;
			playerProfile.Weapon = _pSelectedWeapon;
			playerProfile.ElementalPower = _pSelectedPower;
			playerProfile.CharacterPowerLocked = true;
			playerProfile.ShowHelmet = _showHelmet;
			playerProfile.Save();
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(1);
			int totalLevelsPlayed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
			ScreenMap screenInstance = instance.GetScreenInstance<ScreenMap>();
			screenInstance._pPlayerMugshot = null;
			if (chapterCursor <= 1 && totalLevelsPlayed == 0)
			{
				Navigate(instance.GetScreenInstance<ScreenCutsceneIntro>());
				return;
			}
			SceneManager.LoadScene("Empty");
			ScreenStats screenInstance2 = instance.GetScreenInstance<ScreenStats>();
			Navigate(screenInstance2);
		}
	}

	private void Increment(ref int index, int count, bool allowNone = false)
	{
		AudioPlayer.PlaySound("ui/button/spinner");
		index++;
		if (index >= count)
		{
			index = (allowNone ? (-1) : 0);
		}
	}

	private void Decrement(ref int index, int count, bool allowNone = false)
	{
		AudioPlayer.PlaySound("ui/button/spinner");
		index--;
		if (index < (allowNone ? (-1) : 0))
		{
			index = count - 1;
		}
	}

	private void BuildCharacter()
	{
		MinifigDefinition minifigDefinition = new MinifigDefinition();
		minifigDefinition.hairModel = _pSelectedHairModel;
		minifigDefinition.showHelmet = _showHelmet;
		minifigDefinition.helmetModel = _pSelectedHelmetModel;
		minifigDefinition.helmetReplacesHead = _helmetReplacesHead.Contains(_pSelectedHelmetModel);
		minifigDefinition.helmetHidesHair = _helmetHidesHair.Contains(_pSelectedHelmetModel);
		minifigDefinition.weapon = _pSelectedWeapon;
		minifigDefinition.faceDecal = _pSelectedFaceTexture;
		minifigDefinition.torsoDecal = _pSelectedTorsoTexture;
		minifigDefinition.pelvisDecal = _pSelectedPelvisTexture;
		minifigDefinition.legDecal = _pSelectedLegsTexture;
		minifigDefinition.colorSlots = null;
		minifigDefinition.colors = new Color32[16]
		{
			_headColor,
			_handColor,
			_pSelectedSuitColor,
			_pSelectedSuitColor,
			_pSelectedSuitColor,
			_pSelectedSuitColor,
			_quiverColor,
			_pSelectedHelmetColor,
			_pSelectedHairColor,
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.black
		};
		Texture2D characterTexture = CharacterBuilder.ConfigureTexture(_menuCharacter, minifigDefinition);
		CharacterBuilder.ConfigureCharacter(_menuCharacter, minifigDefinition, characterTexture);
		SkinnedMeshRenderer[] componentsInChildren = _menuCharacter.GetComponentsInChildren<SkinnedMeshRenderer>();
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			int num2 = componentsInChildren[i].materials.Length;
			for (int j = 0; j < num2; j++)
			{
				if (componentsInChildren[i].materials[j].shader.name == _halfLambertTextured.name)
				{
					componentsInChildren[i].materials[j].shader = _halfLambertNoFogTextured;
				}
				else if (componentsInChildren[i].materials[j].shader.name == _halfLambertVertexColor.name)
				{
					componentsInChildren[i].materials[j].shader = _halfLambertNoFogVertexColor;
				}
			}
		}
		MeshRenderer[] componentsInChildren2 = _menuCharacter.GetComponentsInChildren<MeshRenderer>();
		int num3 = componentsInChildren2.Length;
		for (int k = 0; k < num3; k++)
		{
			int num4 = componentsInChildren2[k].materials.Length;
			for (int l = 0; l < num4; l++)
			{
				if (componentsInChildren2[k].materials[l].shader.name == _halfLambertTextured.name)
				{
					componentsInChildren2[k].materials[l].shader = _halfLambertNoFogTextured;
				}
				else if (componentsInChildren2[k].materials[l].shader.name == _halfLambertVertexColor.name)
				{
					componentsInChildren2[k].materials[l].shader = _halfLambertNoFogVertexColor;
				}
			}
		}
		UpdateButtonColors();
		switch (_pSelectedPower)
		{
		case ElementalPower.Earth:
			_abilityIcon.spriteName = "Earth_Ability";
			break;
		case ElementalPower.Electricity:
			_abilityIcon.spriteName = "Electric_Ability";
			break;
		case ElementalPower.Energy:
			_abilityIcon.spriteName = "Energy_Ability";
			break;
		case ElementalPower.Fire:
			_abilityIcon.spriteName = "Fire_Ability";
			break;
		case ElementalPower.Ice:
			_abilityIcon.spriteName = "Ice_Ability";
			break;
		case ElementalPower.Water:
			_abilityIcon.spriteName = "Water_Ability";
			break;
		}
		switch (_pSelectedWeapon)
		{
		case WeaponType.Katana:
			_weaponIcon.spriteName = "Sword_Weapon";
			break;
		case WeaponType.Nunchuck:
			_weaponIcon.spriteName = "NunChuck_Weapon";
			break;
		case WeaponType.Scythe:
			_weaponIcon.spriteName = "Scythe_Weapon";
			break;
		case WeaponType.Shuriken:
			_weaponIcon.spriteName = "Shuriken_Weapon";
			break;
		}
		_hairPadlock.enabled = !_isHairUnlocked[_hairIndex];
		_headPadlock.enabled = !_isFaceUnlocked[_faceIndex];
		_topPadlock.enabled = !_isTorsoUnlocked[_suitIndex];
		_helmetPadlock.enabled = !_isHelmetUnlocked[_helmetIndex];
		_topIcon.alpha = ((!_isTorsoUnlocked[_suitIndex]) ? 0.5f : 1f);
		_hairIcon.alpha = ((!_isHairUnlocked[_hairIndex]) ? 0.5f : 1f);
		_headIcon.alpha = ((!_isFaceUnlocked[_faceIndex]) ? 0.5f : 1f);
		_maskIcon.alpha = ((!_isHelmetUnlocked[_helmetIndex]) ? 0.5f : 1f);
		if (_isFaceUnlocked[_faceIndex] && !_isVanillaFace[_faceIndex] && _numUnseenFaces > 0 && !GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(_unlockableFaceIndex[_faceIndex]))
		{
			GameController.Instance.PlayerProfile.MarkMinifigPartSeen(_unlockableFaceIndex[_faceIndex]);
			_numUnseenFaces--;
		}
		if (_isTorsoUnlocked[_suitIndex] && !_isVanillaTorso[_suitIndex] && _numUnseenTorsos > 0 && !GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(_unlockableTorsoIndex[_suitIndex]))
		{
			GameController.Instance.PlayerProfile.MarkMinifigPartSeen(_unlockableTorsoIndex[_suitIndex]);
			_numUnseenTorsos--;
		}
		if (_isHairUnlocked[_hairIndex] && !_isVanillaHair[_hairIndex] && _numUnseenHairs > 0 && !GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(_unlockableHairIndex[_hairIndex]))
		{
			GameController.Instance.PlayerProfile.MarkMinifigPartSeen(_unlockableHairIndex[_hairIndex]);
			_numUnseenHairs--;
		}
		if (_isHelmetUnlocked[_helmetIndex] && !_isVanillaHelmet[_helmetIndex] && _numUnseenHelmet > 0 && !GameController.Instance.PlayerProfile.HasSeenMinifigPartByIndex(_unlockableHelmetIndex[_helmetIndex]))
		{
			GameController.Instance.PlayerProfile.MarkMinifigPartSeen(_unlockableHelmetIndex[_helmetIndex]);
			_numUnseenHelmet--;
		}
		_faceGoldenTrailFX.SetVisible(_numUnseenFaces > 0);
		_hairGoldenTrailFX.SetVisible(_numUnseenHairs > 0);
		_torsoGoldenTrailFX.SetVisible(_numUnseenTorsos > 0);
		_helmetGoldenTrailFX.SetVisible(_numUnseenHelmet > 0);
		_goButton.isEnabled = _fakeRandomIndex <= 0 && _isFaceUnlocked[_faceIndex] && _isTorsoUnlocked[_suitIndex] && _isHairUnlocked[_hairIndex] && _isHelmetUnlocked[_helmetIndex];
	}

	public void UpdateButtonColors()
	{
		_hairColor.color = _pSelectedHairColor;
		_topColor.color = _pSelectedSuitColor;
		_maskColor.color = _pSelectedHelmetColor;
	}

	private void SetAnimTrigger(int trigger)
	{
		_targetAnimHash = trigger;
		if (_animRef.GetCurrentAnimatorStateInfo(0).shortNameHash != _idleAnimHash)
		{
			_animRef.SetTrigger(_resetAnimHash);
			return;
		}
		_animRef.SetTrigger(_targetAnimHash);
		_targetAnimHash = -1;
	}
}
