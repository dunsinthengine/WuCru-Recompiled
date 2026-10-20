using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NinjaScreenController : MonoBehaviour
{
	public MenuCharacter menuCharacter;

	public MenuCharacter[] teamCharacters;

	private Animator menuCharacterAnimator;

	private bool[] showHelmet;

	private GameObject fxGO;

	private int pageIndex;

	public GameObject[] pages;

	public Button nextPageButton;

	public Button previousPageButton;

	public Image alert_PreviousPageButton;

	public Image alert_nextPageButton;

	public Image tapIcon1;

	public Image tapIcon2;

	public Image tapCruIcon1;

	public Image tapCruIcon2;

	public Text availableSkillPointsText;

	public Button[] weaponButtons;

	public Button[] spinjitzuButtons;

	public Button[] staminaButtons;

	public Button[] agilityButtons;

	public Button[] strengthButtons;

	private List<Button> allButtons;

	private int weaponLevel;

	private int spinjitzuLevel;

	private int staminaLevel;

	private int agilityLevel;

	private int strengthLevel;

	private int oldNinjaID = -1;

	private float tappingShowDuration = 0.3f;

	private float tappingPauseDuration = 1f;

	private float tappingCycleTime;

	private bool tappingStarted;

	private bool tappingCruStarted;

	public Color enabledAbilityColor = new Color(254f / 255f, 197f / 255f, 69f / 255f, 1f);

	public Color ownedAbilityColor = new Color(1f, 0f, 0f, 0.2f);

	public Color purchasableAbilityColor = new Color(1f, 1f, 1f, 1f);

	public Color disabledAbilityColor = new Color(1f, 0f, 0f, 0.2f);

	public AnimationCurve abilityAnimation;

	public AnimationCurve abilityFlash;

	public AnimationCurve abilityNextLevelFlash;

	public Button changeNinjaButton;

	public ConfirmDialog confirmDialog;

	private ConfirmRespecNinja confirmRespecNinja;

	private TopBar topBar;

	public static NinjaScreenOpeningPage OpeningPage = NinjaScreenOpeningPage.Default;

	public static NinjaScreenOpeningPage OpeningPagePreReset = NinjaScreenOpeningPage.Default;

	private List<Coroutine> activeCoroutines = new List<Coroutine>();

	public event EventHandler<DataEventArgs<int>> HelmetToggled;

	private void Awake()
	{
		showHelmet = new bool[5];
	}

	private void Start()
	{
		for (int i = 0; i < teamCharacters.Length; i++)
		{
			int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam == 0)
			{
				MinifigDefinition minifigDefinition = GameController.Instance.PlayerProfile.GetMinifigDefinition();
				showHelmet[i] = minifigDefinition.showHelmet;
			}
			else if (ninjaInTeam == 0)
			{
				MinifigDefinition minifigDefinition2 = CharacterBuilder.minifigDefinitions[ninjaInTeam];
				showHelmet[i] = minifigDefinition2.showHelmet;
			}
			else
			{
				showHelmet[i] = false;
			}
		}
		CharacterBuilder.CleanupMenuCharacterTextures();
		PlayerProfile profile = GameController.Instance.PlayerProfile;
		tapIcon1.enabled = false;
		tapIcon2.enabled = false;
		tapCruIcon1.enabled = false;
		tapCruIcon2.enabled = false;
		AnalyticsEvents.LogLoadScene("NinjaScreen");
		ActivateButtons();
		menuCharacter.gameObject.SetActive(true);
		menuCharacterAnimator = menuCharacter.GetComponentInChildren<Animator>();
		BuildCharacters();
		if (OpeningPage != NinjaScreenOpeningPage.Default)
		{
			pageIndex = (int)OpeningPage;
			OpeningPagePreReset = OpeningPage;
			OpeningPage = NinjaScreenOpeningPage.Default;
		}
		for (int j = 0; j < pages.Length; j++)
		{
			pages[j].SetActive(j == pageIndex);
		}
		weaponLevel = profile.WeaponLevel;
		spinjitzuLevel = profile.SpinjitzuLevel;
		staminaLevel = profile.StaminaLevel;
		agilityLevel = profile.AgilityLevel;
		strengthLevel = profile.StrengthLevel;
		allButtons = new List<Button>();
		allButtons.AddRange(weaponButtons);
		allButtons.AddRange(spinjitzuButtons);
		allButtons.AddRange(staminaButtons);
		allButtons.AddRange(agilityButtons);
		allButtons.AddRange(strengthButtons);
		SetLevel(profile.WeaponLevel, ref weaponLevel, weaponButtons);
		SetLevel(profile.SpinjitzuLevel, ref spinjitzuLevel, spinjitzuButtons);
		SetLevel(profile.StaminaLevel, ref staminaLevel, staminaButtons);
		SetLevel(profile.AgilityLevel, ref agilityLevel, agilityButtons);
		SetLevel(profile.StrengthLevel, ref strengthLevel, strengthButtons);
		UpdateNextSkillButtons();
		for (int k = 0; k < allButtons.Count; k++)
		{
			Image component = allButtons[k].GetComponent<Image>();
			component.material = new Material(component.material);
		}
		SetAvailableSkillPoints();
		for (int l = 0; l < weaponButtons.Length; l++)
		{
			int value = l;
			weaponButtons[l].onClick.AddListener(() =>
			{
				if (value <= profile.Level && value == profile.WeaponLevel + 1 && value <= profile.AvailableSkillPoints)
				{
					AudioPlayer.PlaySound("ui/button/click");
					confirmDialog.activeSkillType = SkillType.Weapon;
					confirmDialog.iSkillLevel = value;
					ShowConfirmDialog();
					confirmDialog.acceptButton.onClick.AddListener(() =>
					{
						SetLevel(value, ref weaponLevel, weaponButtons);
						HideConfirmDialog();
						UpdateNextSkillButtons();
						CompleteSpendSkillPoint();
						AudioPlayer.PlaySound("ui/button/sword/shing");
					});
					confirmDialog.cancelButton.onClick.AddListener(() =>
					{
						AudioPlayer.PlaySound("ui/button/back/click");
					});
				}
			});
		}
		for (int num = 0; num < spinjitzuButtons.Length; num++)
		{
			int value2 = num;
			spinjitzuButtons[num].onClick.AddListener(() =>
			{
				if (value2 <= profile.Level && value2 == profile.SpinjitzuLevel + 1 && value2 <= profile.AvailableSkillPoints)
				{
					AudioPlayer.PlaySound("ui/button/click");
					confirmDialog.activeSkillType = SkillType.Spinjitzu;
					confirmDialog.iSkillLevel = value2;
					ShowConfirmDialog();
					confirmDialog.UpdateDialogBox();
					confirmDialog.acceptButton.onClick.AddListener(() =>
					{
						SetLevel(value2, ref spinjitzuLevel, spinjitzuButtons);
						HideConfirmDialog();
						UpdateNextSkillButtons();
						CompleteSpendSkillPoint();
						AudioPlayer.PlaySound("ui/button/sword/shing");
					});
					confirmDialog.cancelButton.onClick.AddListener(() =>
					{
						AudioPlayer.PlaySound("ui/button/back/click");
					});
				}
			});
		}
		for (int num2 = 0; num2 < staminaButtons.Length; num2++)
		{
			int value3 = num2;
			staminaButtons[num2].onClick.AddListener(() =>
			{
				if (value3 <= profile.Level && value3 == profile.StaminaLevel + 1 && value3 <= profile.AvailableSkillPoints)
				{
					AudioPlayer.PlaySound("ui/button/click");
					confirmDialog.activeSkillType = SkillType.Health;
					confirmDialog.iSkillLevel = value3;
					ShowConfirmDialog();
					confirmDialog.acceptButton.onClick.AddListener(() =>
					{
						SetLevel(value3, ref staminaLevel, staminaButtons);
						HideConfirmDialog();
						UpdateNextSkillButtons();
						CompleteSpendSkillPoint();
						AudioPlayer.PlaySound("ui/button/sword/shing");
					});
					confirmDialog.cancelButton.onClick.AddListener(() =>
					{
						AudioPlayer.PlaySound("ui/button/back/click");
					});
				}
			});
		}
		for (int num3 = 0; num3 < agilityButtons.Length; num3++)
		{
			int value4 = num3;
			agilityButtons[num3].onClick.AddListener(() =>
			{
				if (value4 <= profile.Level && value4 == profile.AgilityLevel + 1 && value4 <= profile.AvailableSkillPoints)
				{
					AudioPlayer.PlaySound("ui/button/click");
					confirmDialog.activeSkillType = SkillType.Speed;
					confirmDialog.iSkillLevel = value4;
					ShowConfirmDialog();
					confirmDialog.acceptButton.onClick.AddListener(() =>
					{
						SetLevel(value4, ref agilityLevel, agilityButtons);
						HideConfirmDialog();
						UpdateNextSkillButtons();
						CompleteSpendSkillPoint();
						AudioPlayer.PlaySound("ui/button/sword/shing");
					});
					confirmDialog.cancelButton.onClick.AddListener(() =>
					{
						AudioPlayer.PlaySound("ui/button/back/click");
					});
				}
			});
		}
		for (int num4 = 0; num4 < strengthButtons.Length; num4++)
		{
			int value5 = num4;
			strengthButtons[num4].onClick.AddListener(() =>
			{
				if (value5 <= profile.Level && value5 == profile.StrengthLevel + 1 && value5 <= profile.AvailableSkillPoints)
				{
					AudioPlayer.PlaySound("ui/button/click");
					confirmDialog.activeSkillType = SkillType.Strength;
					confirmDialog.iSkillLevel = value5;
					ShowConfirmDialog();
					confirmDialog.acceptButton.onClick.AddListener(() =>
					{
						SetLevel(value5, ref strengthLevel, strengthButtons);
						HideConfirmDialog();
						UpdateNextSkillButtons();
						CompleteSpendSkillPoint();
						AudioPlayer.PlaySound("ui/button/sword/shing");
					});
					confirmDialog.cancelButton.onClick.AddListener(() =>
					{
						AudioPlayer.PlaySound("ui/button/back/click");
					});
				}
			});
		}
		confirmDialog.cancelButton.onClick.AddListener(HideConfirmDialog);
	}

	private void OnDestroy()
	{
		for (int i = 0; i < teamCharacters.Length; i++)
		{
			int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam > 0)
			{
				MinifigDefinition minifigDefinition = CharacterBuilder.minifigDefinitions[ninjaInTeam];
				minifigDefinition.showHelmet = false;
			}
		}
		OpeningPagePreReset = NinjaScreenOpeningPage.Default;
		CharacterBuilder.CleanupMenuCharacterTextures();
	}

	public void ActivateButtons()
	{
		topBar = UnityEngine.Object.FindObjectOfType<TopBar>();
		if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.UseSkillPoint))
		{
			Debug.Log("Has not seen briefing: UseSkillPoint");
			nextPageButton.onClick.RemoveAllListeners();
			previousPageButton.onClick.AddListener(ShowNinjaSkillPage);
			previousPageButton.onClick.AddListener(() =>
			{
				HideTappingIcon();
				alert_PreviousPageButton.gameObject.SetActive(false);
				Color color4 = previousPageButton.image.color;
				color4.a = 0.5f;
				previousPageButton.image.color = color4;
				previousPageButton.onClick.RemoveAllListeners();
			});
			changeNinjaButton.onClick.RemoveAllListeners();
			Color color = previousPageButton.image.color;
			color.a = 0.5f;
			nextPageButton.image.color = color;
			changeNinjaButton.image.color = color;
			color.a = 1f;
			previousPageButton.image.color = color;
			changeNinjaButton.gameObject.SetActive(false);
			alert_nextPageButton.gameObject.SetActive(false);
			alert_PreviousPageButton.gameObject.SetActive(true);
			Vector3 position = previousPageButton.transform.position;
			ShowTappingIcon(position);
		}
		else if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			Debug.Log("Has not seen briefing: EquipCruMember");
			Vector3 handPosition = previousPageButton.transform.position;
			nextPageButton.onClick.RemoveAllListeners();
			previousPageButton.onClick.AddListener(ShowCruEquipPage);
			previousPageButton.onClick.AddListener(() =>
			{
				HideTappingIcon();
				Color color4 = previousPageButton.image.color;
				color4.a = 0.5f;
				previousPageButton.image.color = color4;
				alert_PreviousPageButton.gameObject.SetActive(false);
				previousPageButton.onClick.RemoveAllListeners();
				ShowCruTappingIcon(handPosition);
			});
			changeNinjaButton.onClick.RemoveAllListeners();
			Color color2 = previousPageButton.image.color;
			color2.a = 0.5f;
			nextPageButton.image.color = color2;
			changeNinjaButton.image.color = color2;
			changeNinjaButton.gameObject.SetActive(false);
			color2.a = 1f;
			previousPageButton.image.color = color2;
			alert_nextPageButton.gameObject.SetActive(false);
			alert_PreviousPageButton.gameObject.SetActive(true);
			ShowTappingIcon(handPosition);
		}
		else
		{
			if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.UseSkillPoint) || !GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
			{
				return;
			}
			topBar.EnableTopbarButtons();
			previousPageButton.onClick.AddListener(PreviousPage);
			nextPageButton.onClick.AddListener(NextPage);
			Color color3 = previousPageButton.image.color;
			color3.a = 1f;
			previousPageButton.image.color = color3;
			nextPageButton.image.color = color3;
			changeNinjaButton.image.color = color3;
			changeNinjaButton.gameObject.SetActive(true);
			changeNinjaButton.onClick.AddListener(() =>
			{
				if (GameController.Instance.PlayerProfile.AvailableSkillPoints > 0 && confirmRespecNinja == null)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ConfirmRespecNinja"));
					confirmRespecNinja = gameObject.GetComponent<ConfirmRespecNinja>();
					confirmRespecNinja.onOKAction = OnRespecNinjaOK;
					confirmRespecNinja.onCancelAction = OnRespecNinjaCancel;
				}
			});
		}
	}

	public void CompleteSpendSkillPoint()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (!playerProfile.HasSeenBriefing(PlayerBriefingType.UseSkillPoint))
		{
			Debug.Log("BriefingComplete UseSkillPoint");
			playerProfile.BriefingComplete(PlayerBriefingType.UseSkillPoint);
			playerProfile.Save();
			ActivateButtons();
		}
	}

	public void CompleteEquipCru()
	{
		HideTappingIcon();
		topBar.EnableTopbarButtons();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (!playerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
		{
			Debug.Log("BriefingComplete EquipCruMember");
			playerProfile.BriefingComplete(PlayerBriefingType.EquipCruMember);
			playerProfile.Save();
			ActivateButtons();
		}
	}

	public void OnRespecNinjaOK()
	{
		GameController.Instance.PlayerProfile.WeaponLevel = 0;
		GameController.Instance.PlayerProfile.SpinjitzuLevel = 0;
		GameController.Instance.PlayerProfile.StaminaLevel = 0;
		GameController.Instance.PlayerProfile.AgilityLevel = 0;
		GameController.Instance.PlayerProfile.StrengthLevel = 0;
		GameController.Instance.PlayerProfile.ReconfigureCount++;
		GameController.Instance.PlayerProfile.CharacterPowerLocked = false;
		AudioPlayer.PlaySound("ui/button/click");
		SceneManager.LoadScene("CharacterCreation");
	}

	public void OnRespecNinjaCancel()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			previousPageButton.onClick.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextPageButton.onClick.Invoke();
		}
		if (tappingStarted && tappingCycleTime < Time.time)
		{
			if (tapIcon1.enabled)
			{
				tapIcon1.enabled = false;
				tapIcon2.enabled = true;
				tappingCycleTime = Time.time + 0.8f;
			}
			else if (tapIcon2.enabled)
			{
				tapIcon1.enabled = false;
				tapIcon2.enabled = false;
				tappingCycleTime = Time.time + 2f;
			}
			else
			{
				tapIcon1.enabled = true;
				tapIcon2.enabled = false;
				tappingCycleTime = Time.time + 0.3f;
			}
		}
		if (tappingCruStarted && tappingCycleTime < Time.time)
		{
			if (tapCruIcon1.enabled)
			{
				tapCruIcon1.enabled = false;
				tapCruIcon2.enabled = true;
				tappingCycleTime = Time.time + 0.8f;
			}
			else if (tapIcon2.enabled)
			{
				tapCruIcon1.enabled = false;
				tapCruIcon2.enabled = false;
				tappingCycleTime = Time.time + 2f;
			}
			else
			{
				tapCruIcon1.enabled = true;
				tapCruIcon2.enabled = false;
				tappingCycleTime = Time.time + 0.3f;
			}
		}
	}

	private void SmokeBomb(Vector3 position, ElementalPower toPower)
	{
		if (fxGO != null)
		{
			UnityEngine.Object.Destroy(fxGO);
		}
		switch (toPower)
		{
		case ElementalPower.Earth:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Brown"));
			break;
		case ElementalPower.Electricity:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Blue"));
			break;
		case ElementalPower.Energy:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Green"));
			break;
		case ElementalPower.Fire:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Red"));
			break;
		case ElementalPower.Ice:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion White"));
			break;
		case ElementalPower.Water:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Dark Blue"));
			break;
		default:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/SwapNinjaFX"));
			break;
		}
		fxGO.transform.position = position;
		AudioPlayer.PlaySound("player/change/smokebomb");
	}

	public void BuildCharacters()
	{
		for (int i = 0; i < teamCharacters.Length; i++)
		{
			BuildCharacter(i);
		}
	}

	public void BuildCharacter(int i)
	{
		MenuCharacter character = teamCharacters[i];
		int ninjaID = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
		Debug.Log("Build character " + i + ": " + ninjaID);
		if (!character.gameObject.activeSelf && ninjaID > -1)
		{
			oldNinjaID = ninjaID;
			MinifigDefinition minifigDefinition = CharacterBuilder.minifigDefinitions[ninjaID];
			Debug.Log("def.weapon: " + minifigDefinition.weapon);
			SmokeBomb(teamCharacters[i].transform.position, minifigDefinition.elementalPower);
		}
		character.gameObject.SetActive(ninjaID > -1);
		if (ninjaID > -1)
		{
			MinifigDefinition def = ((ninjaID != 0) ? CharacterBuilder.minifigDefinitions[ninjaID] : GameController.Instance.PlayerProfile.GetMinifigDefinition());
			Debug.Log("def.weapon: " + def.weapon);
			Texture2D characterTexture = CharacterBuilder.ConfigureTexture(character, def);
			CharacterBuilder.ConfigureCharacter(character, def, characterTexture);
			Animator component = character.GetComponent<Animator>();
			component.SetFloat(TeahouseVariables.weaponType, (float)def.weapon);
			component.SetFloat("randomStart", UnityEngine.Random.value);
			if ((double)UnityEngine.Random.value > 0.5)
			{
				component.SetFloat("idles", 1f);
			}
			else
			{
				component.SetFloat("idles", 0f);
			}
			component.Play(TeahouseTriggers.idle, 0, UnityEngine.Random.Range(0, 1));
			Weapon weapon = character.GetWeapon();
			Debug.Log("Weapon: " + weapon);
			Weapon componentInChildren = character.leftHandTransform.parent.gameObject.GetComponentInChildren<Weapon>();
			if (componentInChildren != null)
			{
				UnityEngine.Object.Destroy(componentInChildren.gameObject);
			}
			if (weapon.type == WeaponType.Shuriken)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(weapon.gameObject);
				gameObject.transform.parent = character.leftHandTransform.parent;
				gameObject.transform.localPosition = new Vector3(-0.1097006f, 0.001299817f, -0.007000302f);
				gameObject.transform.localRotation = Quaternion.Euler(300.4969f, 59.11868f, 206.6632f);
				gameObject.transform.localScale = Vector3.one;
			}
			if (weapon.type == WeaponType.Nunchuck)
			{
				StartCoroutine(MyNinjaGUI.NunchuckHack(weapon));
			}
			DummyPoint[] componentsInChildren = character.leftHandTransform.gameObject.GetComponentsInChildren<DummyPoint>();
			DummyPoint[] array = componentsInChildren;
			foreach (DummyPoint dummyPoint in array)
			{
				if (dummyPoint.type == DummyPointType.LeftHand)
				{
					character.swordTransform.localPosition = dummyPoint.transform.localPosition;
					character.swordTransform.localRotation = dummyPoint.transform.localRotation;
					break;
				}
			}
			GameObject gameObject2 = character.headTransform.gameObject;
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
			int j2 = i;
			menuCharacterMouseInteraction.onClick = new Button.ButtonClickedEvent();
			menuCharacterMouseInteraction.onClick.AddListener(() =>
			{
				showHelmet[j2] = !showHelmet[j2];
				if (j2 == 0)
				{
					GameController.Instance.PlayerProfile.ShowHelmet = showHelmet[j2];
					def = GameController.Instance.PlayerProfile.GetMinifigDefinition();
				}
				else
				{
					def.showHelmet = showHelmet[j2];
					if (HelmetToggled != null)
					{
						HelmetToggled(this, new DataEventArgs<int>
						{
							data = ninjaID
						});
					}
				}
				BuildCharacter(j2);
			});
			MenuCharacterMouseInteraction menuCharacterMouseInteraction2 = character.GetComponent<MenuCharacterMouseInteraction>();
			if (!menuCharacterMouseInteraction2)
			{
				menuCharacterMouseInteraction2 = character.gameObject.AddComponent<MenuCharacterMouseInteraction>();
			}
			menuCharacterMouseInteraction2.onClick = new Button.ButtonClickedEvent();
			menuCharacterMouseInteraction2.onClick.AddListener(() =>
			{
				if (!topBar.isSettingsBoxActive)
				{
					StartCoroutine(PerformAttack(character, def));
				}
			});
		}
		if (pageIndex == 0)
		{
			NinjaStatsGUI component2 = pages[pageIndex].GetComponent<NinjaStatsGUI>();
			component2.UpdateNinjaImage();
		}
	}

	private IEnumerator PerformAttack(MenuCharacter character, MinifigDefinition def)
	{
		character.GetComponent<Animator>().SetTrigger(TeahouseTriggers.attack);
		yield return new WaitForSeconds(0.1f);
		MeleeWeaponTrail trail = character.GetWeapon().GetComponentInChildren<MeleeWeaponTrail>();
		ElementalPower pow = def.elementalPower;
		Color color1 = Color.white;
		Color color2 = Color.white;
		switch (pow)
		{
		case ElementalPower.Earth:
			color1 = new Color32(83, 53, 17, 120);
			color2 = new Color32(231, 176, 109, 60);
			break;
		case ElementalPower.Electricity:
			color1 = new Color32(2, 188, 230, 120);
			color2 = new Color32(202, 250, 252, 60);
			break;
		case ElementalPower.Energy:
			color1 = new Color32(2, byte.MaxValue, 6, 120);
			color2 = new Color32(247, byte.MaxValue, 94, 60);
			break;
		case ElementalPower.Fire:
			color1 = new Color32(byte.MaxValue, 48, 0, 120);
			color2 = new Color32(byte.MaxValue, 234, 3, 60);
			break;
		case ElementalPower.Ice:
			color1 = new Color32(83, 53, 17, 120);
			color2 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 60);
			break;
		case ElementalPower.Water:
			color1 = new Color32(17, 2, 230, 120);
			color2 = new Color32(175, 202, 252, 60);
			break;
		default:
			color1 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 120);
			color2 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 60);
			break;
		}
		if ((bool)trail)
		{
			trail._colors[0] = color1;
			trail._colors[1] = color2;
			trail.Emit = true;
		}
		Weapon weapon = character.GetWeapon();
		if (weapon.type == WeaponType.Nunchuck)
		{
			weapon.GetComponentInChildren<Animation>().Play("attack");
		}
		yield return new WaitForSeconds(0.25f);
		if ((bool)trail)
		{
			trail.Emit = false;
		}
	}

	public void HideTeamCharacters()
	{
		for (int i = 0; i < teamCharacters.Length; i++)
		{
			teamCharacters[i].gameObject.SetActive(false);
		}
	}

	private void SetLevel(int newLevel, ref int currentLevel, Button[] buttons)
	{
		for (int i = 0; i < newLevel + 1; i++)
		{
			Transform transform = buttons[i].transform.Find("SkillCoin");
			GameObject gameObject = transform.gameObject;
			gameObject.SetActive(false);
			Transform transform2 = buttons[i].transform.Find("lvltxt");
			GameObject gameObject2 = transform2.gameObject;
			if (i == newLevel)
			{
				buttons[i].GetComponent<Image>().color = enabledAbilityColor;
				gameObject2.SetActive(true);
			}
			else
			{
				buttons[i].GetComponent<Image>().color = disabledAbilityColor;
				gameObject2.SetActive(false);
			}
		}
		for (int j = newLevel + 1; j < buttons.Length; j++)
		{
			buttons[j].GetComponent<Image>().color = disabledAbilityColor;
			Transform transform3 = buttons[j].transform.Find("SkillCoin");
			GameObject gameObject3 = transform3.gameObject;
			gameObject3.SetActive(false);
			Transform transform4 = buttons[j].transform.Find("lvltxt");
			GameObject gameObject4 = transform4.gameObject;
			gameObject4.SetActive(false);
		}
		if (newLevel != currentLevel)
		{
			currentLevel = newLevel;
			menuCharacterAnimator.SetTrigger(TeahouseTriggers.levelUp);
			PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
			playerProfile.WeaponLevel = weaponLevel;
			playerProfile.SpinjitzuLevel = spinjitzuLevel;
			playerProfile.StaminaLevel = staminaLevel;
			playerProfile.AgilityLevel = agilityLevel;
			playerProfile.StrengthLevel = strengthLevel;
			playerProfile.Save();
			SetAvailableSkillPoints();
			AudioPlayer.PlaySound("music/level/up");
		}
		if (newLevel > 0)
		{
			AnimateSkillButton(buttons[newLevel]);
		}
		if (newLevel + 1 < buttons.Length)
		{
			Transform transform5 = buttons[newLevel + 1].transform.Find("SkillCoin");
			GameObject gameObject5 = transform5.gameObject;
			gameObject5.SetActive(true);
		}
	}

	private void UpdateNextSkillButtons()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (weaponLevel + 1 < weaponButtons.Length)
		{
			if (weaponLevel + 1 <= playerProfile.AvailableSkillPoints)
			{
				FlashNextSkillButton(weaponButtons[weaponLevel + 1]);
			}
			else
			{
				FadeNextSkillButton(weaponButtons[weaponLevel + 1]);
			}
		}
		if (spinjitzuLevel + 1 < spinjitzuButtons.Length)
		{
			if (spinjitzuLevel + 1 <= playerProfile.AvailableSkillPoints)
			{
				FlashNextSkillButton(spinjitzuButtons[spinjitzuLevel + 1]);
			}
			else
			{
				FadeNextSkillButton(spinjitzuButtons[spinjitzuLevel + 1]);
			}
		}
		if (staminaLevel + 1 < staminaButtons.Length)
		{
			if (staminaLevel + 1 <= playerProfile.AvailableSkillPoints)
			{
				FlashNextSkillButton(staminaButtons[staminaLevel + 1]);
			}
			else
			{
				FadeNextSkillButton(staminaButtons[staminaLevel + 1]);
			}
		}
		if (agilityLevel + 1 < agilityButtons.Length)
		{
			if (agilityLevel + 1 <= playerProfile.AvailableSkillPoints)
			{
				FlashNextSkillButton(agilityButtons[agilityLevel + 1]);
			}
			else
			{
				FadeNextSkillButton(agilityButtons[agilityLevel + 1]);
			}
		}
		if (strengthLevel + 1 < strengthButtons.Length)
		{
			if (strengthLevel + 1 <= playerProfile.AvailableSkillPoints)
			{
				FlashNextSkillButton(strengthButtons[strengthLevel + 1]);
			}
			else
			{
				FadeNextSkillButton(strengthButtons[strengthLevel + 1]);
			}
		}
	}

	private void SetAvailableSkillPoints()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		availableSkillPointsText.text = playerProfile.AvailableSkillPoints.ToString();
	}

	public void RefreshLevel()
	{
		UpdateNextSkillButtons();
		SetAvailableSkillPoints();
	}

	private void ShowConfirmDialog()
	{
		confirmDialog.Show();
	}

	private void HideConfirmDialog()
	{
		confirmDialog.Hide();
	}

	private void AnimateSkillButton(Button button)
	{
		foreach (Coroutine activeCoroutine in activeCoroutines)
		{
			StopCoroutine(activeCoroutine);
		}
		ResetButtonFlash();
		activeCoroutines.Add(StartCoroutine(AnimateSkillButtonCoroutine(button)));
	}

	private IEnumerator AnimateSkillButtonCoroutine(Button button)
	{
		Image image = button.GetComponent<Image>();
		float animationAndTime = abilityAnimation[abilityAnimation.length - 1].time;
		float flashEndTime = abilityFlash[abilityFlash.length - 1].time;
		float endTime = Mathf.Max(animationAndTime, flashEndTime);
		for (float t = 0f; t < endTime; t += Time.deltaTime)
		{
			float s = abilityAnimation.Evaluate(t);
			Vector3 scale = new Vector3(s, s, s);
			button.transform.localScale = scale;
			image.material.SetFloat("_FlashAmount", abilityFlash.Evaluate(t));
			yield return null;
		}
	}

	private void FlashNextSkillButton(Button button)
	{
		activeCoroutines.Add(StartCoroutine(FlashNextSkillButtonCoroutine(button)));
	}

	private void FadeNextSkillButton(Button button)
	{
		Image component = button.GetComponent<Image>();
		component.color = disabledAbilityColor;
	}

	private IEnumerator FlashNextSkillButtonCoroutine(Button button)
	{
		Image image = button.GetComponent<Image>();
		image.color = purchasableAbilityColor;
		float t = Time.timeSinceLevelLoad;
		while (true)
		{
			float a = abilityNextLevelFlash.Evaluate(t);
			image.material.SetFloat("_FlashAmount", a);
			t += Time.deltaTime;
			yield return null;
		}
	}

	private void ResetButtonFlash()
	{
		for (int i = 0; i < allButtons.Count; i++)
		{
			allButtons[i].transform.localScale = Vector3.one;
			allButtons[i].GetComponent<Image>().material.SetFloat("_FlashAmount", 0f);
		}
	}

	private void NextPage()
	{
		AudioPlayer.PlaySound("ui/button/click");
		pages[pageIndex].SetActive(false);
		pageIndex++;
		if (pageIndex >= pages.Length)
		{
			pageIndex = 0;
		}
		pages[pageIndex].SetActive(true);
		CruStatsGUI component = pages[pageIndex].GetComponent<CruStatsGUI>();
		if (component != null)
		{
			component.RefreshContainers();
		}
	}

	private void PreviousPage()
	{
		AudioPlayer.PlaySound("ui/button/back/click");
		pages[pageIndex].SetActive(false);
		pageIndex--;
		if (pageIndex < 0)
		{
			pageIndex = pages.Length - 1;
		}
		pages[pageIndex].SetActive(true);
		CruStatsGUI component = pages[pageIndex].GetComponent<CruStatsGUI>();
		if (component != null)
		{
			component.RefreshContainers();
		}
	}

	private void ShowNinjaSkillPage()
	{
		AudioPlayer.PlaySound("ui/button/back/click");
		pages[pageIndex].SetActive(false);
		pageIndex = 2;
		pages[pageIndex].SetActive(true);
	}

	private void ShowCruEquipPage()
	{
		AudioPlayer.PlaySound("ui/button/back/click");
		pages[pageIndex].SetActive(false);
		pageIndex = 1;
		pages[pageIndex].SetActive(true);
	}

	private void OnSkillPointUnlockedGUIClose()
	{
	}

	private void ShowTappingIcon(Vector3 position)
	{
		if (!tappingStarted)
		{
			tapIcon1.gameObject.SetActive(true);
			tapIcon2.gameObject.SetActive(true);
			tapIcon1.enabled = false;
			tapIcon2.enabled = false;
			tapIcon1.transform.position = position;
			tapIcon2.transform.position = position;
			tappingStarted = true;
			tappingCycleTime = Time.time + 3f;
		}
	}

	private void ShowCruTappingIcon(Vector3 position)
	{
		if (!tappingCruStarted)
		{
			tapCruIcon1.gameObject.SetActive(true);
			tapCruIcon2.gameObject.SetActive(false);
			tapCruIcon1.enabled = false;
			tapCruIcon2.enabled = false;
			tappingCruStarted = true;
			tappingCycleTime = Time.time + 2f;
		}
	}

	private void HideTappingIcon()
	{
		tapIcon1.enabled = false;
		tapIcon2.enabled = false;
		tapCruIcon1.enabled = false;
		tapCruIcon2.enabled = false;
		tappingStarted = false;
		tappingCruStarted = false;
	}
}
