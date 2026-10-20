using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNinjaGUI : MonoBehaviour
{
	public class DecalDefinition
	{
		public int RequiredLevel;

		public string Torso;

		public string Pelvis;

		public string Legs;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			DecalDefinition decalDefinition = (DecalDefinition)obj;
			return RequiredLevel == decalDefinition.RequiredLevel && Torso == decalDefinition.Torso && Pelvis == decalDefinition.Pelvis && Legs == decalDefinition.Legs;
		}

		public override int GetHashCode()
		{
			return RequiredLevel.GetHashCode() ^ Torso.GetHashCode() ^ Pelvis.GetHashCode() ^ Legs.GetHashCode();
		}
	}

	private Coroutine animCoroutine;

	private const int TORSO = 0;

	private const int PELVIS = 1;

	private const int LEGS = 2;

	public Button buttonFaceNext;

	public Button buttonFacePrevious;

	public Button buttonHairNext;

	public Button buttonHairPrevious;

	public Button buttonHairColorNext;

	public Button buttonHelmetNext;

	public Button buttonHelmetPrevious;

	public Button buttonHelmetColorNext;

	public Button buttonSuitNext;

	public Button buttonSuitPrevious;

	public Button buttonColorNext;

	public Button buttonWeaponNext;

	public Button buttonWeaponPrevious;

	public Button buttonPowerNext;

	public Button buttonPowerPrevious;

	public Image imageElementIcon;

	public Image imageWeaponIcon;

	public Button buttonAccept;

	public Button buttonShowHelmet;

	public Button buttonHideHelmet;

	public Color32 headColor;

	public Color32 handColor;

	public Color32 quiverColor;

	public Color32[] suitColors;

	public Color32[] hairColors;

	public GameObject[] elementalPowerEffects;

	public GameObject[] lockedAttributeGroups;

	public GameObject currentElementalPowerEffect;

	private GameObject oldPower;

	public MenuCharacter menuCharacter;

	private Animator menuCharacterAnimator;

	private List<string> helmetReplacesHead = new List<string>();

	private List<string> helmetHidesHair = new List<string>();

	private bool showHelmet;

	private List<DecalDefinition> decals = new List<DecalDefinition>();

	private List<string> helmets = new List<string>();

	private List<string> faces = new List<string>();

	private List<string> hair = new List<string>();

	private WeaponType[] weapons;

	private const int nonPlayerWeaponCount = 4;

	private ElementalPower[] powers;

	private int genderIndex;

	private int faceIndex;

	private int hairIndex;

	private int hairColorIndex;

	private int helmetIndex;

	private int helmetColorIndex;

	private int colorIndex;

	private int suitIndex;

	private int weaponIndex;

	private int powerIndex;

	private int maxSuitCount;

	private string SelectedFaceTexture
	{
		get
		{
			return faces[faceIndex];
		}
	}

	private string SelectedTorsoTexture
	{
		get
		{
			return decals[suitIndex].Torso;
		}
	}

	private string SelectedPelvisTexture
	{
		get
		{
			return decals[suitIndex].Pelvis;
		}
	}

	private string SelectedLegsTexture
	{
		get
		{
			return decals[suitIndex].Legs;
		}
	}

	private Color32 SelectedSuitColor
	{
		get
		{
			return suitColors[colorIndex];
		}
	}

	private Color32 SelectedHairColor
	{
		get
		{
			return hairColors[hairColorIndex];
		}
	}

	private Color32 SelectedHelmetColor
	{
		get
		{
			return suitColors[helmetColorIndex];
		}
	}

	private string SelectedHairModel
	{
		get
		{
			return hair[hairIndex];
		}
	}

	private string SelectedHelmetModel
	{
		get
		{
			return (helmetIndex <= -1) ? string.Empty : helmets[helmetIndex];
		}
	}

	private WeaponType SelectedWeapon
	{
		get
		{
			return weapons[weaponIndex];
		}
	}

	private ElementalPower SelectedPower
	{
		get
		{
			return powers[powerIndex];
		}
	}

	private void Awake()
	{
		bool firstNinja = GameController.Instance.PlayerProfile.TotalLevelsPlayed < 1;
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
					helmets.Add(array[4]);
				}
				if (!string.IsNullOrEmpty(array[4]) && array[5] == "TRUE")
				{
					helmetHidesHair.Add(array[4]);
				}
				int result;
				if (int.TryParse(array[6], out result))
				{
					decals.Add(new DecalDefinition
					{
						RequiredLevel = result,
						Torso = array[9],
						Pelvis = array[8],
						Legs = array[7]
					});
					if (result <= GameController.Instance.PlayerProfile.Level)
					{
						maxSuitCount = decals.Count;
					}
				}
			}
			faces.AddRange(list);
			faces.AddRange(list2);
			hair.AddRange(list3);
			hair.AddRange(list4);
		}
		WeaponType[] array2 = Enum.GetValues(typeof(WeaponType)) as WeaponType[];
		weapons = new WeaponType[array2.Length - 1];
		Array.Copy(array2, 1, weapons, 0, weapons.Length - 4);
		ElementalPower[] array3 = Enum.GetValues(typeof(ElementalPower)) as ElementalPower[];
		powers = new ElementalPower[array3.Length - 1];
		Array.Copy(array3, 1, powers, 0, powers.Length);
		showHelmet = GameController.Instance.PlayerProfile.ShowHelmet;
		buttonFacePrevious.onClick.AddListener(() =>
		{
			Decrement(ref faceIndex, faces.Count);
			showHelmet = false;
			BuildCharacter();
		});
		buttonFaceNext.onClick.AddListener(() =>
		{
			Increment(ref faceIndex, faces.Count);
			showHelmet = false;
			BuildCharacter();
		});
		buttonHairPrevious.onClick.AddListener(() =>
		{
			Decrement(ref hairIndex, hair.Count);
			showHelmet = false;
			BuildCharacter();
		});
		buttonHairNext.onClick.AddListener(() =>
		{
			Increment(ref hairIndex, hair.Count);
			showHelmet = false;
			BuildCharacter();
		});
		buttonHelmetPrevious.onClick.AddListener(() =>
		{
			Decrement(ref helmetIndex, helmets.Count);
			showHelmet = true;
			BuildCharacter();
		});
		buttonHelmetNext.onClick.AddListener(() =>
		{
			Increment(ref helmetIndex, helmets.Count);
			showHelmet = true;
			BuildCharacter();
		});
		buttonHelmetColorNext.onClick.AddListener(() =>
		{
			Increment(ref helmetColorIndex, suitColors.Length);
			showHelmet = true;
			BuildCharacter();
			UpdateButtonColors();
		});
		buttonColorNext.onClick.AddListener(() =>
		{
			Increment(ref colorIndex, suitColors.Length);
			BuildCharacter();
			UpdateButtonColors();
		});
		buttonHairColorNext.onClick.AddListener(() =>
		{
			Increment(ref hairColorIndex, hairColors.Length);
			showHelmet = false;
			BuildCharacter();
			UpdateButtonColors();
		});
		buttonSuitPrevious.onClick.AddListener(() =>
		{
			Decrement(ref suitIndex, maxSuitCount);
			BuildCharacter();
		});
		buttonSuitNext.onClick.AddListener(() =>
		{
			Increment(ref suitIndex, maxSuitCount);
			BuildCharacter();
		});
		buttonWeaponPrevious.onClick.AddListener(() =>
		{
			Decrement(ref weaponIndex, weapons.Length - 4);
			BuildCharacter();
			UpdateWeaponIcon();
			SetTrigger(TeahouseTriggers.levelUp);
			if ((bool)currentElementalPowerEffect)
			{
				UnityEngine.Object.Destroy(currentElementalPowerEffect);
			}
		});
		buttonWeaponNext.onClick.AddListener(() =>
		{
			Increment(ref weaponIndex, weapons.Length - 4);
			BuildCharacter();
			UpdateWeaponIcon();
			SetTrigger(TeahouseTriggers.levelUp);
			if ((bool)currentElementalPowerEffect)
			{
				UnityEngine.Object.Destroy(currentElementalPowerEffect);
			}
		});
		buttonPowerPrevious.onClick.AddListener(() =>
		{
			Decrement(ref powerIndex, powers.Length);
			BuildCharacter();
			UpdateElementIcon();
			SetTrigger(TeahouseTriggers.power);
		});
		buttonPowerNext.onClick.AddListener(() =>
		{
			Increment(ref powerIndex, powers.Length);
			BuildCharacter();
			UpdateElementIcon();
			SetTrigger(TeahouseTriggers.power);
		});
		buttonShowHelmet.onClick.AddListener(() =>
		{
			showHelmet = true;
			BuildCharacter();
		});
		buttonHideHelmet.onClick.AddListener(() =>
		{
			showHelmet = false;
			BuildCharacter();
		});
		buttonAccept.onClick.AddListener(() =>
		{
			PlayerProfile playerProfile2 = GameController.Instance.PlayerProfile;
			playerProfile2.FaceTexture = SelectedFaceTexture;
			playerProfile2.HairModel = SelectedHairModel;
			playerProfile2.HelmetModel = SelectedHelmetModel;
			playerProfile2.HelmetHidesHair = helmetHidesHair.Contains(SelectedHelmetModel);
			playerProfile2.TorsoTexture = SelectedTorsoTexture;
			playerProfile2.PelvisTexture = SelectedPelvisTexture;
			playerProfile2.LegsTexture = SelectedLegsTexture;
			playerProfile2.HeadColor = headColor;
			playerProfile2.HandColor = handColor;
			playerProfile2.ArmColor = SelectedSuitColor;
			playerProfile2.LegColor = SelectedSuitColor;
			playerProfile2.TorsoColor = SelectedSuitColor;
			playerProfile2.PelvisColor = SelectedSuitColor;
			playerProfile2.HelmetColor = SelectedHelmetColor;
			playerProfile2.QuiverColor = quiverColor;
			playerProfile2.HairColor = SelectedHairColor;
			playerProfile2.Weapon = SelectedWeapon;
			playerProfile2.ElementalPower = SelectedPower;
			playerProfile2.CharacterPowerLocked = true;
			playerProfile2.ShowHelmet = showHelmet;
			playerProfile2.Save();
			AudioPlayer.PlaySound("ui/button/sword/shing");
			AnalyticsEvents.LogCharacterCreation();
			if (firstNinja)
			{
				Debug.Log("FirstNinja or no levels played - so launch a gameplay level");
				StartCoroutine(DoLoadFistLevel());
			}
			else
			{
				SceneManager.LoadScene("NinjaScreen");
			}
		});
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (firstNinja)
		{
			hairIndex = UnityEngine.Random.Range(0, hair.Count);
			faceIndex = UnityEngine.Random.Range(0, faces.Count);
			suitIndex = UnityEngine.Random.Range(0, decals.Count);
			helmetIndex = UnityEngine.Random.Range(0, helmets.Count);
			powerIndex = UnityEngine.Random.Range(0, powers.Length);
			weaponIndex = UnityEngine.Random.Range(0, weapons.Length - 4);
			helmetColorIndex = UnityEngine.Random.Range(0, suitColors.Length);
			colorIndex = UnityEngine.Random.Range(0, suitColors.Length);
		}
		else
		{
			faceIndex = faces.IndexOf(playerProfile.FaceTexture);
			if (faceIndex == -1)
			{
				faceIndex = 0;
			}
			hairIndex = hair.IndexOf(playerProfile.HairModel);
			if (hairIndex == -1)
			{
				hairIndex = 0;
			}
			helmetIndex = helmets.IndexOf(playerProfile.HelmetModel);
			if (helmetIndex == -1)
			{
				helmetIndex = 0;
			}
			suitIndex = decals.IndexOf(new DecalDefinition
			{
				Torso = playerProfile.TorsoTexture,
				Pelvis = playerProfile.PelvisTexture,
				Legs = playerProfile.LegsTexture
			});
			if (suitIndex == -1)
			{
				suitIndex = 0;
			}
			colorIndex = Array.IndexOf(suitColors, playerProfile.TorsoColor);
			if (colorIndex == -1)
			{
				colorIndex = 0;
			}
			hairColorIndex = Array.IndexOf(hairColors, playerProfile.HairColor);
			if (hairColorIndex == -1)
			{
				hairColorIndex = 0;
			}
			helmetColorIndex = Array.IndexOf(suitColors, playerProfile.HelmetColor);
			if (helmetColorIndex == -1)
			{
				helmetColorIndex = 0;
			}
			weaponIndex = (int)(playerProfile.Weapon - 1);
			if (weaponIndex == -1)
			{
				weaponIndex = 0;
			}
			powerIndex = (int)(playerProfile.ElementalPower - 1);
			if (powerIndex == -1)
			{
				powerIndex = 0;
			}
		}
		if (playerProfile.CharacterPowerLocked)
		{
			for (int num = 0; num < lockedAttributeGroups.Length; num++)
			{
				lockedAttributeGroups[num].SetActive(false);
			}
		}
		menuCharacterAnimator = menuCharacter.GetComponentInChildren<Animator>();
		menuCharacterAnimator.SetTrigger(TeahouseTriggers.power);
		BuildCharacter();
		UpdateButtonColors();
		UpdateWeaponIcon();
		UpdateElementIcon();
	}

	private IEnumerator DoLoadFistLevel()
	{
		GameController.Instance.PlayerProfile.LastPlayedSeason = GameController.Instance.areaMapMetaLayer.season;
		int seed = GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(GameController.Instance.areaMapMetaLayer.startRow, GameController.Instance.areaMapMetaLayer.startCol);
		if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(seed))
		{
			LevelUnlockData levelUnlockData = new LevelUnlockData();
			levelUnlockData.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
			levelUnlockData.seed = seed;
			GameController.Instance.PlayerProfile.LevelUnlocked.Add(seed, levelUnlockData);
			Debug.Log("<color=yellow>Level Unlock" + seed + "</color>");
		}
		yield return null;
		ScreenFadeFX fadeFX = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
		fadeFX.OnDone = (ScreenFadeFX.OnDoneDelegate)Delegate.Combine(fadeFX.OnDone, new ScreenFadeFX.OnDoneDelegate(FirstLevelLoad));
		fadeFX.FadeOut(1.2f);
		yield return 0;
	}

	private void FirstLevelLoad()
	{
		GameObject target = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenBlockerFX"));
		UnityEngine.Object.DontDestroyOnLoad(target);
		UnityEngine.Object target2 = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/InGameLoadScreen"));
		UnityEngine.Object.DontDestroyOnLoad(target2);
		GameController.Instance.seed = 1049;
		GameController.Instance.styleIndex = 5;
		GameController.Instance.themeCollectionIndex = 4;
		GameAudioManager.Instance.StopAllMusic(false);
		GameAudioManager.Instance.StopAmbience(false);
		SceneManager.LoadScene("LevelPlayerNew");
	}

	private void SetTrigger(int trigger)
	{
		if (animCoroutine != null)
		{
			StopCoroutine(animCoroutine);
		}
		animCoroutine = StartCoroutine(DoSetTrigger(trigger));
	}

	private IEnumerator DoSetTrigger(int trigger)
	{
		if (menuCharacterAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Idle"))
		{
			menuCharacterAnimator.SetTrigger(TeahouseTriggers.reset);
		}
		while (menuCharacterAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Idle"))
		{
			yield return 0;
		}
		menuCharacterAnimator.SetTrigger(trigger);
		Weapon weapon = menuCharacter.GetWeapon();
		if (weapon.type == WeaponType.Nunchuck)
		{
			StartCoroutine(PlayNunchuckAnimDuringWeaponSelection());
		}
	}

	private IEnumerator PlayNunchuckAnimDuringWeaponSelection()
	{
		yield return new WaitForSeconds(0.65f);
		Weapon weapon = menuCharacter.GetWeapon();
		if (weapon.type == WeaponType.Nunchuck)
		{
			weapon.GetComponentInChildren<Animation>().Play("attack");
		}
	}

	public static void Increment(ref int index, int count, bool allowNone = false)
	{
		AudioPlayer.PlaySound("ui/button/spinner");
		index++;
		if (index >= count)
		{
			index = (allowNone ? (-1) : 0);
		}
	}

	public static void Decrement(ref int index, int count, bool allowNone = false)
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
		minifigDefinition.hairModel = SelectedHairModel;
		minifigDefinition.showHelmet = showHelmet;
		minifigDefinition.helmetModel = SelectedHelmetModel;
		minifigDefinition.helmetReplacesHead = helmetReplacesHead.Contains(SelectedHelmetModel);
		minifigDefinition.helmetHidesHair = helmetHidesHair.Contains(SelectedHelmetModel);
		minifigDefinition.weapon = SelectedWeapon;
		minifigDefinition.faceDecal = SelectedFaceTexture;
		minifigDefinition.torsoDecal = SelectedTorsoTexture;
		minifigDefinition.pelvisDecal = SelectedPelvisTexture;
		minifigDefinition.legDecal = SelectedLegsTexture;
		minifigDefinition.colorSlots = null;
		minifigDefinition.colors = new Color32[16]
		{
			headColor,
			handColor,
			SelectedSuitColor,
			SelectedSuitColor,
			SelectedSuitColor,
			SelectedSuitColor,
			quiverColor,
			SelectedHelmetColor,
			SelectedHairColor,
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.black
		};
		Texture2D characterTexture = CharacterBuilder.ConfigureTexture(menuCharacter, minifigDefinition);
		CharacterBuilder.ConfigureCharacter(menuCharacter, minifigDefinition, characterTexture);
		if (SelectedWeapon == WeaponType.Nunchuck)
		{
			StartCoroutine(NunchuckHack(menuCharacter.GetWeapon()));
		}
		if (powerIndex < elementalPowerEffects.Length)
		{
			GameObject gameObject = elementalPowerEffects[powerIndex];
			if (gameObject != oldPower)
			{
				if ((bool)currentElementalPowerEffect)
				{
					UnityEngine.Object.Destroy(currentElementalPowerEffect);
				}
				currentElementalPowerEffect = UnityEngine.Object.Instantiate(elementalPowerEffects[powerIndex]);
				currentElementalPowerEffect.transform.SetParent(menuCharacter.leftHandTransform, false);
				oldPower = gameObject;
			}
		}
		GameObject gameObject2 = menuCharacter.headTransform.gameObject;
		BoxCollider boxCollider = gameObject2.GetComponent<BoxCollider>();
		if (!boxCollider)
		{
			boxCollider = gameObject2.AddComponent<BoxCollider>();
		}
		boxCollider.center = new Vector3(-0.15f, 0f, 0f);
		boxCollider.size = new Vector3(0.4f, 0.3f, 0.3f);
		boxCollider.isTrigger = true;
		MenuCharacterMouseInteraction menuCharacterMouseInteraction = gameObject2.GetComponent<MenuCharacterMouseInteraction>();
		if (!menuCharacterMouseInteraction)
		{
			menuCharacterMouseInteraction = gameObject2.AddComponent<MenuCharacterMouseInteraction>();
		}
		menuCharacterMouseInteraction.onClick = new Button.ButtonClickedEvent();
		menuCharacterMouseInteraction.onClick.AddListener(() =>
		{
			showHelmet = !showHelmet;
			BuildCharacter();
		});
		MenuCharacterMouseInteraction menuCharacterMouseInteraction2 = menuCharacter.GetComponent<MenuCharacterMouseInteraction>();
		if (!menuCharacterMouseInteraction2)
		{
			menuCharacterMouseInteraction2 = menuCharacter.gameObject.AddComponent<MenuCharacterMouseInteraction>();
		}
		menuCharacterMouseInteraction2.onClick = new Button.ButtonClickedEvent();
		menuCharacterMouseInteraction2.onClick.AddListener(() =>
		{
			menuCharacter.GetComponent<Animator>().SetTrigger(TeahouseTriggers.attack);
			Weapon weapon = menuCharacter.GetWeapon();
			if (weapon.type == WeaponType.Nunchuck)
			{
				if (menuCharacterAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Idle"))
				{
					menuCharacterAnimator.SetTrigger(TeahouseTriggers.reset);
				}
				weapon.GetComponentInChildren<Animation>().Play("attack");
			}
		});
	}

	public void UpdateButtonColors()
	{
		buttonHairColorNext.image.color = SelectedHairColor;
		buttonColorNext.image.color = SelectedSuitColor;
		buttonHelmetColorNext.image.color = SelectedHelmetColor;
	}

	public void UpdateWeaponIcon()
	{
		switch (SelectedWeapon)
		{
		case WeaponType.Katana:
			imageWeaponIcon.sprite = Resources.Load<Sprite>(Paths.weaponSpriteKatana);
			break;
		case WeaponType.Shuriken:
			imageWeaponIcon.sprite = Resources.Load<Sprite>(Paths.weaponSpriteShuriken);
			break;
		case WeaponType.Scythe:
			imageWeaponIcon.sprite = Resources.Load<Sprite>(Paths.weaponSpriteScythe);
			break;
		case WeaponType.Nunchuck:
			imageWeaponIcon.sprite = Resources.Load<Sprite>(Paths.weaponSpriteNunchuk);
			break;
		}
	}

	public void UpdateElementIcon()
	{
		switch (SelectedPower)
		{
		case ElementalPower.Earth:
			imageElementIcon.sprite = Resources.Load<Sprite>(Paths.elementSpriteEarth);
			break;
		case ElementalPower.Electricity:
			imageElementIcon.sprite = Resources.Load<Sprite>(Paths.elementSpriteElectricicy);
			break;
		case ElementalPower.Energy:
			imageElementIcon.sprite = Resources.Load<Sprite>(Paths.elementSpriteEnergy);
			break;
		case ElementalPower.Fire:
			imageElementIcon.sprite = Resources.Load<Sprite>(Paths.elementSpriteFire);
			break;
		case ElementalPower.Ice:
			imageElementIcon.sprite = Resources.Load<Sprite>(Paths.elementSpriteIce);
			break;
		case ElementalPower.Water:
			imageElementIcon.sprite = Resources.Load<Sprite>(Paths.elementSpriteWater);
			break;
		}
	}

	public static IEnumerator NunchuckHack(Weapon weapon)
	{
		Animation a = weapon.GetComponentInChildren<Animation>();
		yield return null;
		if (a != null)
		{
			a.Play("folded", PlayMode.StopAll);
		}
	}
}
