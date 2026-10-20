using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class Character : MonoBehaviour
{
	public delegate void OnCharacterDeadDelegate(Level level, Character character, DamageType damageType);

	public delegate void OnMissionAccomplishedDelegate();

	public delegate void OnPlayerDeadDelegate();

	public delegate void OnPlayerStart();

	public const float DEFAULT_CHARACTER_SPEED = 4f;

	public const string SwordDrawAudioEvent = "player/combat/sword/draw";

	public bool isPlayer;

	public Transform headTransform;

	public Transform handTransform;

	public Transform leftHandTransform;

	public Transform backTransform;

	public Transform swordTransform;

	public Transform rootTransform;

	public Transform shoulderTransform;

	public GameObject[] bodyparts;

	public GameObject hairGameObject;

	public GameObject helmetGameObject;

	public GameObject weaponGameObject;

	public ElementalPower elementalPower = ElementalPower.Earth;

	public Color[] elementalPowerPrimaryColors = new Color[7];

	public Color[] elementalPowerSecondaryColors = new Color[7];

	public float interactDistance = 5f;

	private bool isInteractable;

	[SerializeField]
	public Animator animator;

	private CharacterCollider characterCollider;

	private Rigidbody characterRididbody;

	private Transform characterTransform;

	public Vector3 positionOffset = Vector3.zero;

	private Vector3 prevPositionOffset = Vector3.zero;

	private ICharacterController controller;

	private PathFinding pathFinding;

	private float speed = 4f;

	private float ladderSpeedRatio = 0.85f;

	public HealthBar healthBar;

	private int maxHealth = 100;

	private int health = 100;

	private float mana;

	private Vector3 delta = Vector3.zero;

	private LevelNode currentDestination;

	private Queue<PathFinding.Node> plannedPath = new Queue<PathFinding.Node>();

	private PathFinding.Node[] path;

	private PathFinding.Node currentPathNode;

	private PathFinding.Node currentPathTarget;

	private SkinnedMeshRenderer[] skinnedMeshRenderers;

	private MeshRenderer[] meshRenderers;

	private List<int> doorKeys = new List<int>();

	private float spinjitzuDamageInterval = 0.3f;

	private float lastSpinjitzuDamageTime;

	private float lastDamageTime;

	private float damageBlinkValue;

	public string spinjitzuPrefabNameRoot = "Prefabs/Characters/Spinjitzu/";

	private float spinjitzuDuration = 8f;

	private float spinjitzuProgression;

	private bool spinjitzuInProgress;

	private int ninjaID;

	private Projector blobShadow;

	private Coroutine centerOnTileCoroutine;

	private float outlineTimer;

	protected bool activateBlink;

	private float activateBlinkTimer;

	public OnCharacterDeadDelegate OnCharacterDead;

	public OnMissionAccomplishedDelegate onMissionAccomplished;

	public OnPlayerDeadDelegate onPlayerDead;

	public OnPlayerStart onPlayerStart;

	public Mesh mesh;

	public Texture2D texture;

	public Vector3 blahPoint = new Vector3(0.5f, 0f, 0.5f);

	public Vector3 blahPoint2 = new Vector3(2.5f, 0f, 1f);

	private const bool useSimpleVisionCone = true;

	public bool visionConeDebug;

	public PathFinding PathFinding
	{
		get
		{
			return pathFinding;
		}
	}

	public Vector3 inputVector { get; set; }

	public int Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			if (isPlayer)
			{
				ScreenHud._pInstance.SetHealth(health, maxHealth);
			}
		}
	}

	public int MissilesLeft
	{
		get
		{
			return (GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaMissilesLeft(ninjaID);
		}
		set
		{
			if (GameController.Instance.currentState is GameControllerStatePlaying)
			{
				(GameController.Instance.currentState as GameControllerStatePlaying).SetNinjaMissilesLeft(ninjaID, value);
			}
		}
	}

	public ICharacterController Controller
	{
		get
		{
			return controller;
		}
	}

	public LevelNode CurrentPathNode
	{
		get
		{
			if (currentPathNode == null)
			{
				return null;
			}
			return currentPathNode.levelNode;
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

	public float LadderSpeedRatio
	{
		get
		{
			return ladderSpeedRatio;
		}
	}

	public Animator Animator
	{
		get
		{
			return animator;
		}
	}

	public LevelNode CurrentDestination
	{
		get
		{
			return currentDestination;
		}
	}

	public PathFinding.Node CurrentPathTarget
	{
		get
		{
			return currentPathTarget;
		}
	}

	public int NumPlannedPathNodes
	{
		get
		{
			return plannedPath.Count;
		}
	}

	public Vector3 Velocity { get; private set; }

	public Transform SwordTransform
	{
		get
		{
			return swordTransform;
		}
	}

	public Queue<PathFinding.Node> PlannedPath
	{
		get
		{
			return plannedPath;
		}
	}

	public bool IsInteractable
	{
		get
		{
			return isInteractable;
		}
	}

	public int NinjaID
	{
		get
		{
			return ninjaID;
		}
		set
		{
			ninjaID = value;
		}
	}

	public float SpinjitzuProgression
	{
		get
		{
			if (!isPlayer || !IsInSpinjitzu())
			{
				return 0f;
			}
			return spinjitzuProgression;
		}
		set
		{
			spinjitzuProgression = value;
			ScreenHud._pInstance.SetSpinjitsuCountdown(spinjitzuProgression);
		}
	}

	public bool SpinjitzuInProgress
	{
		get
		{
			return spinjitzuInProgress;
		}
		set
		{
			spinjitzuInProgress = value;
			NinjaActionGUI ninjaActionGUI = UnityEngine.Object.FindObjectOfType<NinjaActionGUI>();
			if ((bool)ninjaActionGUI)
			{
				ninjaActionGUI.spinjitzuProgress.enabled = spinjitzuInProgress;
			}
		}
	}

	public int MaxHealth
	{
		get
		{
			return maxHealth;
		}
		set
		{
			maxHealth = value;
		}
	}

	public float SpinjitzuDuration
	{
		get
		{
			return spinjitzuDuration;
		}
		set
		{
			spinjitzuDuration = value;
		}
	}

	public LevelNode CurrentPositionNode
	{
		get
		{
			if (GamePlayer.Instance.Level == null)
			{
				return null;
			}
			int num = Mathf.Clamp(Mathf.RoundToInt(base.transform.position.z), 0, GamePlayer.Instance.Level.Nodes.GetLength(0) - 1);
			int num2 = Mathf.Clamp(Mathf.RoundToInt(base.transform.position.x), 0, GamePlayer.Instance.Level.Nodes.GetLength(1) - 1);
			return GamePlayer.Instance.Level.Nodes[num, num2];
		}
	}

	public void AddDoorKey(int key)
	{
		if (!HasDoorKey(key))
		{
			doorKeys.Add(key);
		}
	}

	public bool HasDoorKey(int key)
	{
		return doorKeys.Contains(key);
	}

	public Weapon GetWeapon()
	{
		if (swordTransform == null)
		{
			return null;
		}
		return swordTransform.GetComponentInChildren<Weapon>();
	}

	public CharacterCollider GetCharacterCollider()
	{
		return characterCollider;
	}

	public void EquipWeapon(bool inHand)
	{
		if (swordTransform == null)
		{
			Debug.LogWarning("Character: swordTransform is null during EquipWeapon. isPlayer = " + isPlayer);
			return;
		}
		if (GetWeapon() == null)
		{
			Debug.LogWarning("Character: GetWeapon() return null during EquipWeapon. isPlayer = " + isPlayer);
			return;
		}
		if (leftHandTransform == null)
		{
			Debug.LogWarning("Character: leftHandTransform is null during EquipWeapon. isPlayer = " + isPlayer);
			return;
		}
		if (inHand)
		{
			swordTransform.parent = handTransform;
			swordTransform.localPosition = Vector3.zero;
			swordTransform.localRotation = Quaternion.identity;
			EventManager.Instance.PostEvent("player/combat/sword/draw", base.gameObject);
			Weapon weapon = GetWeapon();
			Animator.SetFloat(VarHash.weaponType, (float)weapon.type);
			DummyPoint[] componentsInChildren = swordTransform.gameObject.GetComponentsInChildren<DummyPoint>();
			DummyPoint[] array = componentsInChildren;
			foreach (DummyPoint dummyPoint in array)
			{
				if (dummyPoint.type == DummyPointType.Hand)
				{
					swordTransform.localPosition = dummyPoint.transform.localPosition;
					swordTransform.localRotation = dummyPoint.transform.localRotation;
					break;
				}
			}
			if (weapon.type == WeaponType.Shuriken && MissilesLeft > 1)
			{
				Weapon componentInChildren = leftHandTransform.gameObject.GetComponentInChildren<Weapon>();
				if (componentInChildren != null)
				{
					UnityEngine.Object.Destroy(componentInChildren.gameObject);
				}
				Vector3 localPosition = weapon.transform.localPosition;
				Quaternion localRotation = weapon.transform.localRotation;
				Vector3 localScale = weapon.transform.localScale;
				GameObject gameObject = UnityEngine.Object.Instantiate(weapon.gameObject);
				gameObject.transform.parent = leftHandTransform;
				gameObject.transform.localPosition = localPosition;
				gameObject.transform.localRotation = localRotation;
				gameObject.transform.localScale = localScale;
			}
			componentsInChildren = leftHandTransform.gameObject.GetComponentsInChildren<DummyPoint>();
			DummyPoint[] array2 = componentsInChildren;
			foreach (DummyPoint dummyPoint2 in array2)
			{
				if (dummyPoint2.type == DummyPointType.LeftHand)
				{
					swordTransform.localPosition = dummyPoint2.transform.localPosition;
					swordTransform.localRotation = dummyPoint2.transform.localRotation;
					break;
				}
			}
			if (weapon.type == WeaponType.Nunchuck)
			{
				Animation componentInChildren2 = weapon.GetComponentInChildren<Animation>();
				componentInChildren2.Play("start_pos");
			}
			return;
		}
		Weapon componentInChildren3 = leftHandTransform.gameObject.GetComponentInChildren<Weapon>();
		if (componentInChildren3 != null)
		{
			UnityEngine.Object.Destroy(componentInChildren3.gameObject);
		}
		swordTransform.parent = backTransform;
		swordTransform.localPosition = Vector3.zero;
		swordTransform.localRotation = Quaternion.identity;
		if (isPlayer)
		{
			Animator.SetFloat(VarHash.weaponType, 0f);
		}
		DummyPoint[] componentsInChildren2 = swordTransform.gameObject.GetComponentsInChildren<DummyPoint>();
		DummyPoint[] array3 = componentsInChildren2;
		foreach (DummyPoint dummyPoint3 in array3)
		{
			if (dummyPoint3.type == DummyPointType.Back)
			{
				swordTransform.localPosition = dummyPoint3.transform.localPosition;
				swordTransform.localRotation = dummyPoint3.transform.localRotation;
				break;
			}
		}
		Weapon componentInChildren4 = swordTransform.GetComponentInChildren<Weapon>();
		if (componentInChildren4.type == WeaponType.Nunchuck)
		{
			Animation componentInChildren5 = componentInChildren4.GetComponentInChildren<Animation>();
			componentInChildren5.Play("folded");
		}
	}

	private void Awake()
	{
		characterTransform = base.transform;
		if (animator == null)
		{
			animator = GetComponentInChildren<Animator>();
		}
		if (animator != null)
		{
			animator.applyRootMotion = false;
		}
		RefreshMeshRenderers();
		blobShadow = GetComponentInChildren<Projector>();
	}

	public void RefreshMeshRenderers()
	{
		skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
		if (skinnedMeshRenderers != null)
		{
			for (int i = 0; i < skinnedMeshRenderers.Length; i++)
			{
				if (skinnedMeshRenderers[i] != null)
				{
					skinnedMeshRenderers[i].updateWhenOffscreen = true;
				}
			}
		}
	}

	private void Start()
	{
		characterTransform = base.transform;
		if (animator != null)
		{
			animator.applyRootMotion = false;
		}
		controller = base.transform.parent.gameObject.GetComponent<ICharacterController>();
		elementalPowerPrimaryColors[0] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		elementalPowerSecondaryColors[0] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		elementalPowerPrimaryColors[1] = new Color32(83, 53, 17, byte.MaxValue);
		elementalPowerSecondaryColors[1] = new Color32(231, 176, 109, byte.MaxValue);
		elementalPowerPrimaryColors[2] = new Color32(byte.MaxValue, 48, 0, byte.MaxValue);
		elementalPowerSecondaryColors[2] = new Color32(byte.MaxValue, 234, 3, byte.MaxValue);
		elementalPowerPrimaryColors[3] = new Color32(17, 2, 230, byte.MaxValue);
		elementalPowerSecondaryColors[3] = new Color32(175, 202, 252, byte.MaxValue);
		elementalPowerPrimaryColors[4] = new Color32(2, 188, 230, byte.MaxValue);
		elementalPowerSecondaryColors[4] = new Color32(202, 250, 252, byte.MaxValue);
		elementalPowerPrimaryColors[5] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		elementalPowerSecondaryColors[5] = new Color32(175, 234, byte.MaxValue, byte.MaxValue);
		elementalPowerPrimaryColors[6] = new Color32(2, byte.MaxValue, 6, byte.MaxValue);
		elementalPowerSecondaryColors[6] = new Color32(247, byte.MaxValue, 94, byte.MaxValue);
		if (isPlayer)
		{
			PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
			int num = 0;
			if (ninjaID != 0 && !GameController.Instance.MissionStats.deadNinjaIDs.Contains(ninjaID))
			{
				num = ninjaID;
			}
			else
			{
				int[] ninjaTeam = playerProfile.NinjaTeam;
				foreach (int num2 in ninjaTeam)
				{
					if (!GameController.Instance.MissionStats.deadNinjaIDs.Contains(num2))
					{
						num = num2;
						break;
					}
				}
			}
			CharacterBuilder.ConfigureMinifig(this, num);
			Weapon weapon = GetWeapon();
			if (weapon != null && weapon.type == WeaponType.Shuriken && MissilesLeft <= 0)
			{
				(Controller as PlayerController).ChangeWeapon(WeaponType.Katana);
			}
			MaxHealth = playerProfile.MaxHealth;
			int ninjaLevel = playerProfile.GetNinjaLevel(Team.GetTypeForID(num));
			Team.Configure(this, num, ninjaLevel);
			RefreshNinjaPortrait(num);
			if (GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
			{
				ScreenHud._pInstance.SetSpinjitsuCount((GameController.Instance.currentState as GameControllerStatePlaying).SpinjitzuCount);
			}
			else
			{
				ScreenHud._pInstance.SetSpinjitsuCount(GameController.Instance.PlayerProfile.SpinjitzuCount);
			}
			if (GameController.Instance.currentState is GameControllerStatePlaying)
			{
				health = Mathf.RoundToInt((GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaHealthFactor(ninjaID) * (float)GamePlayer.Instance.Player.Character.MaxHealth);
				MissilesLeft = (GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaMissilesLeft(ninjaID);
			}
			else
			{
				health = GamePlayer.Instance.Player.Character.MaxHealth;
				MissilesLeft = 0;
			}
			if (isPlayer)
			{
				ScreenHud._pInstance.SetHealth(health, maxHealth);
			}
			if (onPlayerStart != null)
			{
				onPlayerStart();
			}
			if (GameController.Instance != null)
			{
				GameController.Instance.MissionStats.ReportPlayerStart();
			}
			ScreenHud._pInstance.SetSpinjitsuType(elementalPower);
			base.transform.Rotate(new Vector3(0f, 90f, 0f));
		}
		else
		{
			MaxHealth = (Controller as AIController).agent.health;
			health = maxHealth;
			EquipWeapon(false);
		}
		ToggleInteractable();
		lastDamageTime = float.MinValue;
		damageBlinkValue = 0f;
		if (!isPlayer)
		{
			OnCharacterDead = (OnCharacterDeadDelegate)Delegate.Combine(OnCharacterDead, new OnCharacterDeadDelegate(GameController.Instance.MissionController.OnCharacterDead));
		}
		if (GameController.Instance.CurrentLevel != null && GameController.Instance.CurrentLevel.Challenge != null)
		{
			OnCharacterDead = (OnCharacterDeadDelegate)Delegate.Combine(OnCharacterDead, new OnCharacterDeadDelegate(GameController.Instance.CurrentLevel.Challenge.OnCharacterDead));
		}
		characterCollider = base.gameObject.GetComponent<CharacterCollider>();
		characterRididbody = base.gameObject.GetComponent<Rigidbody>();
		characterTransform = base.transform;
	}

	public void Visible(bool visible)
	{
		SkinnedMeshRenderer[] array = skinnedMeshRenderers;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer != null)
			{
				skinnedMeshRenderer.enabled = visible;
			}
		}
		MeshRenderer[] array2 = meshRenderers;
		foreach (MeshRenderer meshRenderer in array2)
		{
			if (meshRenderer != null)
			{
				meshRenderer.enabled = visible;
			}
		}
		if (!isPlayer)
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.GetComponent<Renderer>().enabled = visible;
			}
		}
	}

	public int CurrentStateNameHash()
	{
		if (Animator != null)
		{
			return Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
		}
		return StateHash.idle;
	}

	public string CurrentStateName()
	{
		if (StateHash.hashToName.ContainsKey(CurrentStateNameHash()))
		{
			return StateHash.hashToName[CurrentStateNameHash()];
		}
		Debug.LogError("State name hash value not in dictionary");
		return string.Empty;
	}

	public bool IsClimbing()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.climbing)
		{
			return true;
		}
		return false;
	}

	public bool IsRunning()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.moving)
		{
			return true;
		}
		return false;
	}

	public bool IsIdle()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.idle)
		{
			return true;
		}
		return false;
	}

	public bool IsPreparingCombat()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.preparingCombat)
		{
			return true;
		}
		if (num == StateHash.spectatingCombat)
		{
			return true;
		}
		return false;
	}

	public bool IsInCombat()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.combatReady)
		{
			return true;
		}
		if (num == StateHash.duelling)
		{
			return true;
		}
		if (num == StateHash.playerAttackOption)
		{
			return true;
		}
		if (num == StateHash.enemyAttackOption)
		{
			return true;
		}
		if (num == StateHash.playerAttackSuccess)
		{
			return true;
		}
		if (num == StateHash.playerAttackBlocked)
		{
			return true;
		}
		if (num == StateHash.enemyAttackSuccess)
		{
			return true;
		}
		if (num == StateHash.enemyAttackBlocked)
		{
			return true;
		}
		if (num == StateHash.secondaryAttack)
		{
			return true;
		}
		if (num == StateHash.secondaryAttackHit)
		{
			return true;
		}
		return false;
	}

	public bool IsHiding()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.hiding)
		{
			return true;
		}
		return false;
	}

	public bool IsDead()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.dead)
		{
			return true;
		}
		if (!isPlayer && Controller != null && (Controller as AIController).agent != null && (Controller as AIController).agent.State == AIAgentState.Dead)
		{
			return true;
		}
		if (isPlayer && num == StateHash.missionEnding)
		{
			return true;
		}
		return false;
	}

	public bool IsTrapped()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.trapped)
		{
			return true;
		}
		return false;
	}

	public bool IsSneakAttacking()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.performSneakAttack)
		{
			return true;
		}
		if (num == StateHash.rangedSneakAttack)
		{
			return true;
		}
		if (num == StateHash.rangedSneakAttackAnticipation)
		{
			return true;
		}
		return false;
	}

	public bool IsPreparingSneakAttack()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.preparingSneakAttack)
		{
			return true;
		}
		return false;
	}

	public bool IsInSpinjitzu()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.prepareSpinjitzu)
		{
			return true;
		}
		if (num == StateHash.spintitzu)
		{
			return true;
		}
		if (num == StateHash.endSpinjitzu)
		{
			return true;
		}
		if (spinjitzuInProgress)
		{
			return true;
		}
		return false;
	}

	public bool IsElemetalAttackAnticipating()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.elementalAttackAnticipation)
		{
			return true;
		}
		return false;
	}

	public bool IsElemetalAttacking()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.elementalAttackPerform)
		{
			return true;
		}
		return false;
	}

	public bool IsSwingHitting()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.preparingSwingHit)
		{
			return true;
		}
		if (num == StateHash.swingHitSuccess)
		{
			return true;
		}
		if (num == StateHash.swingHitFailed)
		{
			return true;
		}
		return false;
	}

	public bool IsKnockedDown()
	{
		int num = CurrentStateNameHash();
		if (num == StateHash.knockedDown)
		{
			return true;
		}
		return false;
	}

	public bool IsElementalBoss()
	{
		if (isPlayer)
		{
			return false;
		}
		if (Controller == null)
		{
			return false;
		}
		AIAgentType agentType = (Controller as AIController).agent.GetAgentType();
		return agentType == AIAgentType.ElementalBoss;
	}

	public bool IsSnakeBoss()
	{
		if (isPlayer)
		{
			return false;
		}
		if (Controller == null)
		{
			return false;
		}
		switch ((Controller as AIController).agent.GetAgentType())
		{
		case AIAgentType.SnakeBossEnemy:
		case AIAgentType.ConstrictaiBossEnemy:
		case AIAgentType.FangpyreBossEnemy:
		case AIAgentType.HypnobraiBossEnemy:
		case AIAgentType.VenomariBossEnemy:
			return true;
		default:
			return false;
		}
	}

	public SwordTappingIcon GetSwordTappingIcon()
	{
		return swordTransform.gameObject.GetComponentInChildren<SwordTappingIcon>();
	}

	public void TakeDamage(int amount, Vector3 fromPosition, DamageType damageType, AIAgentType agentType = AIAgentType.None)
	{
		if (damageType == DamageType.Spinjitzu)
		{
			bool flag = true;
			if (IsElementalBoss() && elementalPower != GamePlayer.Instance.Player.Character.elementalPower)
			{
				flag = false;
			}
			if (!flag)
			{
				Spinjitzu componentInChildren = GamePlayer.Instance.Player.Character.GetComponentInChildren<Spinjitzu>();
				if (componentInChildren != null)
				{
					componentInChildren._pWantCancel = true;
				}
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (!(realtimeSinceStartup > lastSpinjitzuDamageTime + spinjitzuDamageInterval) || IsDead())
			{
				return;
			}
			string empty = string.Empty;
			if ((double)UnityEngine.Random.value > 0.7)
			{
				AudioPlayer.PlaySound("ui/button/sword/shing", base.gameObject);
			}
			else
			{
				AudioPlayer.PlaySound("player/combat/scythe/attack/success", base.gameObject);
			}
			lastSpinjitzuDamageTime = Time.realtimeSinceStartup;
		}
		if (IsDead())
		{
			return;
		}
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (isPlayer && playerProfile.AgilityLevel > 0 && UnityEngine.Random.value < (float)playerProfile.AgilityLevel / 20f)
		{
			ShowCombatEventType(CombatSpecialEventType.Dodge);
			GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.white);
			return;
		}
		health -= amount;
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.red);
		if (health <= 0)
		{
			health = 0;
			OnCharacterDie(fromPosition, damageType, agentType);
		}
		else
		{
			lastDamageTime = Time.time;
		}
		if (isPlayer)
		{
			ScreenHud._pInstance.SetHealth(health, maxHealth);
		}
		else if (isPlayer)
		{
			Debug.LogWarning("HealthBar is null");
		}
		if (isPlayer && GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			(GameController.Instance.currentState as GameControllerStatePlaying).SetNinjaHealth(ninjaID, (float)health / (float)maxHealth);
			(GameController.Instance.currentState as GameControllerStatePlaying).SetNinjaMissilesLeft(ninjaID, MissilesLeft);
		}
		else if (!isPlayer && GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() != typeof(GameControllerStatePlaying))
		{
		}
		ShowDamageNumber(amount, damageType);
	}

	private void ShowDamageNumber(int amount, DamageType damageType)
	{
		switch (damageType)
		{
		case DamageType.Spinjitzu:
			ScreenHud._pInstance._poolMessageSpinjitsu.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case DamageType.SneakAttack:
			ScreenHud._pInstance._poolMessageSneakAttack.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case DamageType.SecondaryAttack:
			ScreenHud._pInstance._poolMessageSecondaryAttack.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case DamageType.PerfectAttack:
			ScreenHud._pInstance._poolMessagePerfectAttack.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case DamageType.Trap:
			ScreenHud._pInstance._poolMessageTrap.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case DamageType.Explosion:
			ScreenHud._pInstance._poolMessageExplosion.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		}
		if (amount > 0)
		{
			ScreenHud._pInstance._poolMessageNumbers.TriggerMessage(base.transform.position + Vector3.up, amount.ToString());
		}
		int consecutiveHits = GamePlayer.Instance.CombatManager.consecutiveHits;
		if (isPlayer)
		{
			InGameCam._pInstance.Shake(0.7f, 0.5f);
		}
		else if (consecutiveHits > 3)
		{
			InGameCam._pInstance.Shake(1.2f, 0.5f);
		}
		else if (consecutiveHits > 2)
		{
			InGameCam._pInstance.Shake(0.8f, 0.5f);
		}
		else if (consecutiveHits > 1)
		{
			InGameCam._pInstance.Shake(0.6f, 0.5f);
		}
		else
		{
			InGameCam._pInstance.Shake(0.2f, 0.5f);
		}
	}

	public void ShowCombatEventType(CombatSpecialEventType eventType)
	{
		switch (eventType)
		{
		case CombatSpecialEventType.PrimaryAttack:
		case CombatSpecialEventType.SecondaryAttack:
		case CombatSpecialEventType.PerfectAttack:
			return;
		case CombatSpecialEventType.CombatWon:
		{
			if (GameController.Instance.AlertCount > 0)
			{
				return;
			}
			AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
			foreach (AIAgent aIAgent in agents)
			{
				if (!aIAgent.Character.IsDead() && Vector3.Distance(aIAgent.Character.transform.position, base.transform.position) <= 5f)
				{
					return;
				}
			}
			break;
		}
		}
		switch (eventType)
		{
		case CombatSpecialEventType.CombatLost:
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenHud screenInstance = instance.GetScreenInstance<ScreenHud>();
			screenInstance.OnCharacterDefeated();
			break;
		}
		case CombatSpecialEventType.CounterAttack:
			ScreenHud._pInstance._poolMessageCounterAttack.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case CombatSpecialEventType.Dodge:
			ScreenHud._pInstance._poolMessageDodge.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case CombatSpecialEventType.Deflection:
			ScreenHud._pInstance._poolMessageDeflect.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		case CombatSpecialEventType.SurpriseAttack:
			ScreenHud._pInstance._poolMessageSurprise.TriggerMessage(base.transform.position + Vector3.up + UnityEngine.Random.onUnitSphere);
			break;
		}
	}

	private void OnCharacterDie(Vector3 fromPosition, DamageType damageType, AIAgentType agentType)
	{
		Animator.SetTrigger(TriggerHash.die);
		if ((bool)blobShadow)
		{
			blobShadow.enabled = false;
		}
		if (isPlayer)
		{
			AnalyticsEvents.LogPlayerDied(agentType, damageType);
			if (EditorStateMachine.Instance != null)
			{
				StartCoroutine(DelayedPlayerDeadMsg());
			}
			else if (onPlayerDead != null)
			{
				onPlayerDead();
			}
		}
		else if (!isPlayer)
		{
			AnalyticsEvents.LogEnemyDied(damageType);
			isInteractable = false;
			(controller as AIController).agent.SetState(AIAgentState.Dead);
			if (GameController.Instance != null)
			{
				GameController.Instance.MissionStats.ReportEnemyDead();
			}
			if (OnCharacterDead != null)
			{
				OnCharacterDead(GameController.Instance.CurrentLevel, this, damageType);
			}
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			component.enabled = false;
		}
	}

	private IEnumerator DelayedPlayerDeadMsg()
	{
		yield return new WaitForSeconds(2f);
		Message msg = EditorStateMachine.Instance.GetComponent<Message>();
		msg.msg = "Player has died...";
		msg.OnCloseAction = Respawn;
		EditorStateMachine.Instance.PushState(msg);
	}

	public void OnCharacterReachEndFlag()
	{
		if (!isPlayer || IsDead())
		{
			return;
		}
		if (GameController.Instance != null)
		{
			GameController.Instance.MissionStats.ReportLevelCompleted();
		}
		(GameController.Instance.currentState as GameControllerStatePlaying).OnIngameObjectiveReached();
		int index = GamePlayer.Instance.Level.LevelTheme.index;
		int count = GamePlayer.Instance.Level.LevelThemeCollection.themes.Count;
		if (onMissionAccomplished == null)
		{
			if (GamePlayer.Instance.Level.LevelTheme.index == GamePlayer.Instance.Level.LevelThemeCollection.themes.Count - 1)
			{
				Message component = EditorStateMachine.Instance.GetComponent<Message>();
				component.msg = "Mission " + (index + 1) + " of " + count + " Accomplished!\nReturning to Editor";
				component.OnCloseAction = StopPlaying;
				EditorStateMachine.Instance.PushState(component);
			}
			else
			{
				MessageYesNo component2 = EditorStateMachine.Instance.GetComponent<MessageYesNo>();
				component2.msg = "Mission " + (index + 1) + " of " + count + " Accomplished!\nLoading next mission...";
				component2.OnOkAction = LoadNextSequence;
				component2.OnCancelAction = StopPlaying;
				EditorStateMachine.Instance.PushState(component2);
			}
		}
		else
		{
			onMissionAccomplished();
		}
	}

	private void LoadNextSequence()
	{
		Debug.Log("LoadNextSequence()");
		if (EditorStateMachine.Instance != null)
		{
			LevelEngine levelEngine = new LevelEngine();
			LevelThemeCollection levelThemeCollection = GamePlayer.Instance.Level.LevelThemeCollection;
			LevelTheme levelTheme = levelThemeCollection.themes.values[GamePlayer.Instance.Level.LevelTheme.index + 1];
			LevelStyle levelStyle = GamePlayer.Instance.Level.LevelStyle;
			EditorStateMachine.Instance.level = levelEngine.GenerateLevel(levelThemeCollection, levelTheme, levelStyle, GamePlayer.Instance.Level.Seed, ElementalPower.None, ElementalPower.None);
			GameObject gameObject = UnityEngine.Object.FindObjectOfType<GamePlayer>().gameObject;
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			gameObject = new GameObject();
			gameObject.name = "GamePlayer";
			GamePlayer gamePlayer = gameObject.AddComponent<GamePlayer>();
			gamePlayer.Initialize(EditorStateMachine.Instance.level);
			EditorStateMachine.Instance.levelVisualizer.BuildLevel(EditorStateMachine.Instance.level, EditorStateMachine.Instance.level.LevelStyle);
			GamePlayer.Instance.SetState(GamePlayerState.Exploring);
			Character character = GamePlayer.Instance.Player.Character;
			character.onMissionAccomplished = (OnMissionAccomplishedDelegate)Delegate.Combine(character.onMissionAccomplished, new OnMissionAccomplishedDelegate(OnCharacterReachEndFlag));
			string styleName = levelStyle.styleName;
			EditorStateMachine.Instance.debugGUI.SetLevelInfo(GamePlayer.Instance.Level.Seed, styleName, levelThemeCollection.collectionName, levelTheme.themeName);
		}
	}

	public void Respawn()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		health = MaxHealth;
		UpdateLevel(GamePlayer.Instance.Level.Nodes);
		ResetPath();
		ScreenHud._pInstance.SetHealth(health, MaxHealth);
		GamePlayer.Instance.Level.ResetAI();
		TriggerHash.ResetTriggers(Animator);
		EquipWeapon(false);
		Animator.SetBool(VarHash.isMoving, false);
		GamePlayer.Instance.SetState(GamePlayerState.Exploring);
		Animator.SetTrigger(TriggerHash.reset);
		Visible(true);
		lastDamageTime = float.MinValue;
		int num = 0;
		int[] ninjaTeam = playerProfile.NinjaTeam;
		foreach (int num2 in ninjaTeam)
		{
			if (!GameController.Instance.MissionStats.deadNinjaIDs.Contains(num2))
			{
				num = num2;
				break;
			}
		}
		CharacterBuilder.ConfigureMinifig(this, num);
		int ninjaLevel = playerProfile.GetNinjaLevel(Team.GetTypeForID(num));
		Team.Configure(this, num, ninjaLevel);
		ScreenHud._pInstance.RespawnCharacter(num);
		ScreenHud._pInstance.SetSpinjitsuType(GamePlayer.Instance.Player.Character.elementalPower);
		GetSwordTappingIcon().Show(false);
		ScreenHud._pInstance._swordTapTelegraph.Hide();
		ScreenHud._pInstance._hudBattleMinigame.CloseMinigame();
		Spinjitzu componentInChildren = GetComponentInChildren<Spinjitzu>();
		if (componentInChildren != null)
		{
			UnityEngine.Object.Destroy(componentInChildren.gameObject);
		}
	}

	public void RefreshNinjaPortrait(int chNinjaID = -1)
	{
		if (chNinjaID < 0)
		{
			chNinjaID = ninjaID;
		}
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		NinjaTeamGUI ninjaTeamGUI = UnityEngine.Object.FindObjectOfType<NinjaTeamGUI>();
		if (!ninjaTeamGUI)
		{
			return;
		}
		if (ninjaTeamGUI.characterTextureDict.Count > 0)
		{
			ninjaTeamGUI.portraits[0].Configure(chNinjaID, ninjaTeamGUI.characterTextureDict[chNinjaID], NinjaPortraitState.Selected);
		}
		for (int i = 0; i < playerProfile.NinjaTeam.Length; i++)
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam == chNinjaID)
			{
				ninjaTeamGUI.portraits[i + 1].Configure(ninjaInTeam, ninjaTeamGUI.characterTextureDict[ninjaInTeam], NinjaPortraitState.Selected);
			}
			else if (i > 0)
			{
				if (GameController.Instance.MissionStats.deadNinjaIDs.Contains(ninjaInTeam))
				{
					ninjaTeamGUI.portraits[i + 1].Configure(ninjaInTeam, ninjaTeamGUI.characterTextureDict[ninjaInTeam], NinjaPortraitState.Disabled);
				}
				else
				{
					ninjaTeamGUI.portraits[i + 1].Configure(ninjaInTeam, ninjaTeamGUI.characterTextureDict[ninjaInTeam]);
				}
			}
		}
	}

	private void StopPlaying()
	{
		GamePlayer.Instance.SetState(GamePlayerState.Idle);
		EditorStateMachine.Instance.SetState(EditorStateMachine.Instance.gameObject.GetComponent<Editing>());
	}

	public bool DoesPathExist(LevelNode toNode)
	{
		if (currentPathNode == null || toNode == null)
		{
			return false;
		}
		return pathFinding.GetPathIgnoreDoors(CurrentPathNode, toNode, null) != null;
	}

	public Vector3 GetEyePosition()
	{
		return new Vector3(0f, 0.8f, 0f);
	}

	public void UpdateLevel(LevelNode[,] nodes)
	{
		pathFinding = new PathFinding(nodes);
		currentPathNode = pathFinding.StartNode;
		base.transform.position = currentPathNode.Position;
	}

	public void ResetPath()
	{
		currentPathTarget = null;
		plannedPath.Clear();
		currentDestination = null;
	}

	public bool HasPath()
	{
		return CurrentPathTarget != null || plannedPath.Count > 0;
	}

	public void ResetPathAfterClimbing()
	{
		if (isPlayer)
		{
			if (plannedPath.Count > 0 && plannedPath.Peek() == currentPathNode)
			{
				plannedPath.Dequeue();
			}
			if (plannedPath.Count > 0 && plannedPath.Peek() == CurrentPathTarget)
			{
				plannedPath.Dequeue();
			}
		}
		if (currentPathTarget != null)
		{
			currentPathNode = currentPathTarget;
		}
		else if (CurrentPositionNode != null && pathFinding != null && pathFinding.Nodes != null)
		{
			currentPathNode = pathFinding.Nodes[CurrentPositionNode.Row, CurrentPositionNode.Col];
		}
		currentPathTarget = null;
	}

	public void SetCurrentPathNode(int row, int col)
	{
		if (pathFinding != null && pathFinding.Nodes != null && row >= 0 && row < pathFinding.Nodes.GetLength(0) && col >= 0 && col < pathFinding.Nodes.GetLength(1))
		{
			currentPathNode = pathFinding.Nodes[row, col];
		}
	}

	public void ForceMove(Vector3 position)
	{
		ForceMove(Mathf.FloorToInt(position.z), Mathf.FloorToInt(position.x));
	}

	public void ForceMove(int row, int col)
	{
		currentPathNode = pathFinding.Nodes[row, col];
		base.transform.position = new Vector3(col, currentPathNode.levelNode.Elevation, row);
		ResetPath();
	}

	private void DebugDrawCircle(Vector3 position, float radius, Color color, int segments = 32, float duration = 0f)
	{
		for (int i = 0; i < segments; i++)
		{
			Debug.DrawLine(position + radius * LevelHelper.VectorForAngle((float)Math.PI * 2f * (float)i / (float)segments), position + radius * LevelHelper.VectorForAngle((float)Math.PI * 2f * (float)(i + 1) / (float)segments), color, duration);
		}
	}

	private void DebugDrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
	{
		Debug.DrawLine(start, end, color, duration);
	}

	private void CheckWallIntersection(Vector3 transformPosition, float deltaTime, ref Vector3 localDelta, out LevelNode currentNode, out LevelNode horizontalNeighbor, out LevelNode verticalNeighbor, out LevelNode diagonalNeighbor, CharacterCollider thisCharacterCollider)
	{
		LevelNode[,] nodes = GamePlayer.Instance.Level.Nodes;
		currentNode = CurrentPositionNode;
		Vector3 vector = LevelHelper.NodeToVector3(currentNode);
		Vector3 vector2 = transformPosition - vector;
		Vector3 vector3 = transformPosition;
		float num = ((!(vector2.x < 0f)) ? 0.5f : (-0.5f));
		float num2 = ((!(vector2.z < 0f)) ? 0.5f : (-0.5f));
		float num3 = vector3.x - (vector.x + num);
		float num4 = vector3.z - (vector.z + num2);
		float num5 = Mathf.Sign(vector2.x);
		float num6 = num5 * localDelta.x + thisCharacterCollider.radius - num5 * (vector.x + num - vector3.x);
		float num7 = Mathf.Sign(vector2.z);
		float num8 = num7 * localDelta.z + thisCharacterCollider.radius - num7 * (vector.z + num2 - vector3.z);
		LevelDirection direction = ((!(vector2.x > 0f)) ? LevelDirection.West : LevelDirection.East);
		LevelDirection direction2 = ((!(vector2.z > 0f)) ? LevelDirection.South : LevelDirection.North);
		horizontalNeighbor = currentNode.GetNeighbour(nodes, direction);
		verticalNeighbor = currentNode.GetNeighbour(nodes, direction2);
		diagonalNeighbor = null;
		if (horizontalNeighbor != null)
		{
			diagonalNeighbor = horizontalNeighbor.GetNeighbour(nodes, direction2);
		}
		else if (verticalNeighbor != null)
		{
			diagonalNeighbor = verticalNeighbor.GetNeighbour(nodes, direction);
		}
		if (!IsInSpinjitzu())
		{
			if (LevelHelper.NodeHasBoundary(currentNode, nodes, direction) && !LevelHelper.NodeHasOpenDoor(currentNode, nodes, direction) && num6 > 0f)
			{
				localDelta.x -= num6 * num5;
			}
			if (LevelHelper.NodeHasBoundary(currentNode, nodes, direction2) && !LevelHelper.NodeHasOpenDoor(currentNode, nodes, direction2) && num8 > 0f)
			{
				localDelta.z -= num8 * num7;
			}
		}
		else
		{
			if (LevelHelper.NodeHasSpinjitzuBoundry(currentNode, nodes, direction) && !LevelHelper.NodeHasOpenDoor(currentNode, nodes, direction) && num6 > 0f)
			{
				localDelta.x -= num6 * num5;
			}
			if (LevelHelper.NodeHasSpinjitzuBoundry(currentNode, nodes, direction2) && !LevelHelper.NodeHasOpenDoor(currentNode, nodes, direction2) && num8 > 0f)
			{
				localDelta.z -= num8 * num7;
			}
			float num9 = vector3.x + num5 * thisCharacterCollider.radius - (vector.x + num);
			float num10 = vector3.z + num7 * thisCharacterCollider.radius - (vector.z + num2);
			LevelNode levelNode = LevelHelper.Vector3ToNode(transformPosition + new Vector3(num9 * num5, 0f, num10 * num7), nodes);
			if (levelNode != null && levelNode.Elevation > 0)
			{
				float num11 = (float)levelNode.Elevation - transformPosition.y;
				if (num11 < -0.1f)
				{
					localDelta.y = Mathf.Min(num11, -1f) * 4f * deltaTime;
					localDelta.x *= 0.4f;
					localDelta.z *= 0.4f;
				}
				else if (num11 > 0.1f)
				{
					localDelta.y = Mathf.Min(num11, 1f) * 4f * deltaTime;
					localDelta.x *= 0.2f;
					localDelta.z *= 0.2f;
				}
				else
				{
					base.transform.position = new Vector3(transformPosition.x, levelNode.Elevation, transformPosition.z);
				}
			}
		}
		if (horizontalNeighbor == null || LevelHelper.NodeHasBoundary(currentNode, nodes, direction) || !LevelHelper.NodeHasBoundary(horizontalNeighbor, nodes, direction2) || LevelHelper.NodeHasOpenDoor(horizontalNeighbor, nodes, direction2) || verticalNeighbor == null || LevelHelper.NodeHasBoundary(currentNode, nodes, direction2) || !LevelHelper.NodeHasBoundary(verticalNeighbor, nodes, direction) || LevelHelper.NodeHasOpenDoor(verticalNeighbor, nodes, direction))
		{
			return;
		}
		if (num6 > 0f && num8 > 0f)
		{
			if (num6 > num8)
			{
				localDelta.z -= num8 * num7;
			}
			else
			{
				localDelta.x -= num6 * num5;
			}
		}
		Vector3 vector4 = vector + new Vector3(num, 0f, num2);
		Vector3 vector5 = vector4 - (vector3 + localDelta);
		vector5.y = 0f;
		float magnitude = vector5.magnitude;
		float num12 = thisCharacterCollider.radius - magnitude;
		if (num12 > 0f)
		{
			localDelta -= num12 * vector5.normalized;
		}
	}

	private void CheckObjectCollisions(Vector3 transformPosition, float deltaTime, ref Vector3 localDelta, LevelNode currentNode, LevelNode horizontalNeighbor, LevelNode verticalNeighbor, LevelNode diagonalNeighbor, CharacterCollider thisCharacterCollider)
	{
		List<CharacterCollider> list = new List<CharacterCollider>(currentNode.CharacterColliders);
		if (horizontalNeighbor != null)
		{
			list.AddRange(horizontalNeighbor.CharacterColliders);
		}
		if (verticalNeighbor != null)
		{
			list.AddRange(verticalNeighbor.CharacterColliders);
		}
		if (diagonalNeighbor != null)
		{
			list.AddRange(diagonalNeighbor.CharacterColliders);
		}
		AIAgent[] agents = GamePlayer.Instance.Level.Agents;
		foreach (AIAgent aIAgent in agents)
		{
			if (aIAgent.Character != this && !aIAgent.Character.IsDead())
			{
				CharacterCollider characterCollider = aIAgent.Character.GetCharacterCollider();
				if (characterCollider != null)
				{
					list.Add(characterCollider);
				}
			}
		}
		if (!isPlayer && GamePlayer.Instance.Player != null)
		{
			list.Add(GamePlayer.Instance.Player.Character.GetCharacterCollider());
		}
		foreach (CharacterCollider item in list)
		{
			if (item != null)
			{
				Vector3 vector = item.transform.position + item.transform.right * item.offset - transformPosition;
				vector.y = 0f;
				float num = thisCharacterCollider.radius + localDelta.magnitude + item.radius - vector.magnitude;
				if (num > 0f)
				{
					localDelta -= num * vector.normalized;
				}
			}
		}
	}

	public void MoveTowards(Vector3 direction, float speed, float maxStep)
	{
		float num = CurrentPositionNode.Elevation;
		if (centerOnTileCoroutine != null)
		{
			StopCoroutine(centerOnTileCoroutine);
			centerOnTileCoroutine = null;
		}
		Velocity = speed * direction;
		if (IsPreparingCombat() || IsInCombat() || (!isPlayer && (controller as AIController).CurrentBehaviour.GetType() == typeof(CombatPlayer)))
		{
			float num2 = speed * Time.deltaTime;
			if (num2 > maxStep)
			{
				num2 = maxStep;
			}
			delta = direction * num2;
			characterTransform.position += delta;
		}
		else
		{
			if (IsDead() || IsTrapped() || (isPlayer && IsClimbing()))
			{
				return;
			}
			if (CurrentStateNameHash() == StateHash.hiding)
			{
				Animator.SetTrigger(TriggerHash.move);
			}
			float num3 = Time.deltaTime;
			delta = Vector3.zero;
			Vector3 position = characterTransform.position;
			while (num3 > 0f && maxStep > 0f && GameController.Instance.CurrentLevel != null)
			{
				float num4 = Mathf.Min(num3, 1f / 30f);
				num3 -= num4;
				float num5 = speed * num4;
				if (num5 > maxStep)
				{
					num5 = maxStep;
				}
				maxStep -= num5;
				Vector3 localDelta = direction * num5;
				LevelNode currentNode;
				LevelNode horizontalNeighbor;
				LevelNode verticalNeighbor;
				LevelNode diagonalNeighbor;
				CheckWallIntersection(position, num4, ref localDelta, out currentNode, out horizontalNeighbor, out verticalNeighbor, out diagonalNeighbor, characterCollider);
				if (!IsInCombat() && !IsPreparingCombat())
				{
					CheckObjectCollisions(position, num4, ref localDelta, currentNode, horizontalNeighbor, verticalNeighbor, diagonalNeighbor, characterCollider);
				}
				CheckWallIntersection(position, num4, ref localDelta, out currentNode, out horizontalNeighbor, out verticalNeighbor, out diagonalNeighbor, characterCollider);
				LevelNode levelNode = LevelHelper.Vector3ToNode(position + localDelta, GameController.Instance.CurrentLevel.Nodes);
				LevelDirection levelDir = LevelHelper.GetLevelDirectionForVector(direction);
				bool hasLadderConnection = (currentNode != null && LevelHelper.NodeHasLadder(currentNode, GameController.Instance.CurrentLevel.Nodes, levelDir));
				if (levelNode != null && ((float)levelNode.Elevation == num || hasLadderConnection))
				{
					position += localDelta;
					delta += localDelta;
				}
			}
			characterTransform.position = position;
			direction.y = 0f;
			characterTransform.LookAt(characterTransform.position + direction);
			if (!IsInCombat() && !IsClimbing() && !Animator.GetBool(VarHash.isMoving))
			{
				Animator.SetBool(VarHash.isMoving, true);
			}
		}
	}

	public void StartSpinjitzu(bool useCharge = true)
	{
		Debug.Log("StartSpinjitzu()");
		if (!IsInSpinjitzu() && isPlayer && !IsDead() && (IsIdle() || IsRunning() || IsPreparingCombat() || IsInCombat()) && (GameController.Instance.currentState as GameControllerStatePlaying).SpinjitzuCount > 0)
		{
			if (useCharge)
			{
				(GameController.Instance.currentState as GameControllerStatePlaying).SpinjitzuCount--;
			}
			ScreenHud._pInstance.SetSpinjitsuCount((GameController.Instance.currentState as GameControllerStatePlaying).SpinjitzuCount);
			spinjitzuInProgress = true;
			spinjitzuProgression = 0f;
			animator.SetTrigger(TriggerHash.startSpinjitzu);
			Spinjitzu spinjitzu = base.gameObject.AddComponent<Spinjitzu>();
		}
	}

	public void WalkTo(LevelNode destination, bool force = false)
	{
		if (isPlayer && (IsInCombat() || IsPreparingCombat()))
		{
			GamePlayer.Instance.MovementPointer.Hide();
		}
		if (centerOnTileCoroutine != null)
		{
			StopCoroutine(centerOnTileCoroutine);
			centerOnTileCoroutine = null;
		}
		if (pathFinding == null || IsDead() || IsTrapped())
		{
			return;
		}
		if (isPlayer && currentDestination == destination && !force && currentPathTarget != null && !IsClimbing())
		{
			Debug.Log("B1 [" + currentDestination.Row + " / " + CurrentDestination.Col + "]");
		}
		else if (CurrentPositionNode == destination)
		{
			if (!isPlayer)
			{
			}
		}
		else
		{
			if (isPlayer && IsClimbing())
			{
				return;
			}
			if (CurrentStateNameHash() == StateHash.hiding)
			{
				Animator.SetTrigger(TriggerHash.move);
			}
			LevelNode currentPositionNode = CurrentPositionNode;
			if (currentPathTarget != null)
			{
				currentPositionNode = currentPathTarget.levelNode;
			}
			PathFinding.Node[] array = pathFinding.GetPathIgnoreDoors(CurrentPositionNode, destination, this);
			LevelNode[,] nodes = GamePlayer.Instance.Level.Nodes;
			if (array != null && array.Length > 1)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					LevelDirection value = LevelHelper.GetDirectionForNeighbor(array[i].levelNode, array[i + 1].levelNode).Value;
					if (LevelHelper.NodeHasClosedDoor(array[i].levelNode, nodes, value) || (IsSnakeBoss() && LevelHelper.NodeHasLadder(array[i].levelNode, nodes, value)))
					{
						PathFinding.Node[] array2 = new PathFinding.Node[i + 1];
						Array.Copy(array, array2, i + 1);
						array = array2;
						break;
					}
				}
			}
			path = pathFinding.GetSmoothPath(array);
			if (path == null)
			{
				return;
			}
			currentDestination = destination;
			if (path.Length > 1)
			{
				plannedPath.Clear();
				for (int j = 1; j < path.Length; j++)
				{
					plannedPath.Enqueue(path[j]);
				}
			}
			float num = destination.Elevation - CurrentPositionNode.Elevation;
			if (num == 0f && IsRunning())
			{
				currentPathTarget = null;
			}
			LevelNode levelNode = plannedPath.Peek().levelNode;
			if (!IsInCombat() && !IsClimbing() && !Animator.GetBool(VarHash.isMoving))
			{
				Animator.SetBool(VarHash.isMoving, true);
			}
		}
	}

	public void MoveStep()
	{
		if (ScreenResults._pInResults)
		{
			return;
		}
		if (centerOnTileCoroutine != null)
		{
			StopCoroutine(centerOnTileCoroutine);
			centerOnTileCoroutine = null;
		}
		if (IsClimbing())
		{
			return;
		}
		if (currentPathTarget == null)
		{
			if (plannedPath.Count > 0)
			{
				currentPathTarget = plannedPath.Dequeue();
			}
			else if (IsRunning() || IsPreparingCombat())
			{
				if (!isPlayer || inputVector.magnitude < 0.01f)
				{
					Animator.SetBool(VarHash.isMoving, false);
					currentDestination = null;
				}
			}
		}
		if (currentPathTarget == null)
		{
			return;
		}
		if (currentPathNode == null)
		{
			Debug.LogWarning("CurrentPathNode was null!");
			currentPathNode = pathFinding.Nodes[CurrentPositionNode.Row, CurrentPositionNode.Col];
		}
		Vector3 vector = currentPathTarget.Position - base.transform.position;
		float magnitude = vector.magnitude;
		if (magnitude < 0.1f)
		{
			currentPathNode = currentPathTarget;
			currentPathTarget = null;
			if (plannedPath.Count == 0)
			{
				Animator.SetBool(VarHash.isMoving, false);
				currentDestination = null;
				if (isPlayer)
				{
					GamePlayer.Instance.MovementPointer.Hide();
				}
			}
			else
			{
				currentPathTarget = plannedPath.Dequeue();
				if (IsClimbing())
				{
					Animator.SetTrigger(TriggerHash.stopClimbing);
				}
			}
		}
		if (currentPathTarget == null)
		{
			return;
		}
		float num = 1f;
		if (Mathf.Abs(currentPathTarget.Position.y - currentPathNode.Position.y) > 0.5f)
		{
			num = ladderSpeedRatio;
			if (!IsClimbing() && !IsInSpinjitzu())
			{
				LookAt(currentPathTarget.Position);
				Animator.SetTrigger(TriggerHash.climb);
			}
			else
			{
				if (!IsInSpinjitzu())
				{
					return;
				}
				vector = LevelHelper.NodeToVector3(currentPathTarget.levelNode) - base.transform.position;
				float magnitude2 = vector.magnitude;
				if (Mathf.Abs(magnitude2) <= 0.1f)
				{
					base.transform.position = LevelHelper.NodeToVector3(currentPathTarget.levelNode);
					return;
				}
				float num2 = Speed * Time.deltaTime;
				if (num2 > magnitude2)
				{
					num2 = magnitude2;
				}
				base.transform.position = base.transform.position + vector.normalized * num2;
			}
		}
		else
		{
			float num3 = speed * num;
			Vector3 position = base.transform.position;
			MoveTowards(vector.normalized, num3, magnitude);
			if (isPlayer && Vector3.Distance(base.transform.position, GamePlayer.Instance.MovementPointer.transform.position) < 1.2f)
			{
				GamePlayer.Instance.MovementPointer.Hide();
			}
		}
	}

	private void OnDestroy()
	{
		CharacterBuilder.CleanupInGameCharacterTexture(ninjaID);
	}

	private void Update()
	{
		base.transform.position = base.transform.position + prevPositionOffset - positionOffset;
		prevPositionOffset = positionOffset;
		float num = Time.time - lastDamageTime;
		float num2 = ((num > 0f && num < 0.3f) ? 1 : 0);
		if (num2 != damageBlinkValue)
		{
			damageBlinkValue = num2;
			SkinnedMeshRenderer[] array = skinnedMeshRenderers;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.material.SetFloat("_BlinkSwitch", num2);
				}
				else
				{
					Debug.LogWarning("SkinnedMeshRenderer is null!");
				}
			}
			MeshRenderer[] array2 = meshRenderers;
			foreach (MeshRenderer meshRenderer in array2)
			{
				if (meshRenderer != null)
				{
					meshRenderer.material.SetFloat("_BlinkSwitch", num2);
				}
				else
				{
					Debug.LogWarning("MeshRenderer is null!");
				}
			}
		}
		if (GamePlayer.Instance.Player != null && !isPlayer && !(controller as AIController).agent.isFriend)
		{
			float num3 = Vector3.Distance(base.transform.position, GamePlayer.Instance.Player.Character.transform.position);
			if (num3 <= interactDistance && !isInteractable && CurrentPositionNode.Elevation == GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation)
			{
				isInteractable = true;
				ToggleInteractable();
			}
			if ((num3 > interactDistance || CurrentPositionNode.Elevation != GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation) && isInteractable)
			{
				isInteractable = false;
				ToggleInteractable();
			}
			if (GamePlayer.Instance.Player.Character.IsInSpinjitzu() && num3 < 1f)
			{
				int num4 = 40 + UnityEngine.Random.Range(0, 10);
				int spinjitzuLevel = GameController.Instance.PlayerProfile.SpinjitzuLevel;
				num4 += spinjitzuLevel * 10;
				if (IsSnakeBoss())
				{
					num4 /= 4;
				}
				Vector3 fromPosition = LevelHelper.NodeToVector3(GamePlayer.Instance.Player.Character.CurrentPositionNode);
				TakeDamage(num4, fromPosition, DamageType.Spinjitzu);
			}
		}
		if (activateBlink)
		{
			float num5 = Time.time - activateBlinkTimer;
			AnimationCurve interactableFlash = GamePlayer.Instance.Level.LevelStyle.interactableFlash;
			if (num5 < 1f)
			{
				SetOutlineSwitchValue(interactableFlash.Evaluate(num5) * 4f);
			}
			else
			{
				SetOutlineSwitchValue(0f);
				activateBlink = false;
			}
		}
		else if (IsInteractable)
		{
			float num6 = Time.time - outlineTimer;
			AnimationCurve interactableFlash2 = GamePlayer.Instance.Level.LevelStyle.interactableFlash;
			AnimationCurve interactableLoop = GamePlayer.Instance.Level.LevelStyle.interactableLoop;
			float time = interactableFlash2[interactableFlash2.length - 1].time;
			float outlineSwitchValue = ((!(num6 < time)) ? interactableLoop.Evaluate(num6 - time) : interactableFlash2.Evaluate(num6));
			SetOutlineSwitchValue(outlineSwitchValue);
		}
		if (!IsDead() && !IsPreparingCombat() && !isPlayer)
		{
			CreateViewFrustumMesh(base.gameObject, 60f, 10f);
		}
		else if (mesh != null)
		{
			UnityEngine.Object.Destroy(mesh);
		}
	}

	public void LookAt(Vector3 pos)
	{
		pos.y = base.transform.position.y;
		base.transform.LookAt(pos);
	}

	public void DoActivateBlink()
	{
		activateBlink = true;
		activateBlinkTimer = Time.time;
	}

	protected void SetOutlineSwitchValue(float value)
	{
		if (meshRenderers != null)
		{
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer != null && meshRenderer.enabled)
				{
					meshRenderer.material.SetFloat("_OutlineSwitch", value);
				}
			}
		}
		if (skinnedMeshRenderers == null)
		{
			return;
		}
		SkinnedMeshRenderer[] array2 = skinnedMeshRenderers;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
		{
			if (skinnedMeshRenderer != null && skinnedMeshRenderer.enabled)
			{
				skinnedMeshRenderer.material.SetFloat("_OutlineSwitch", value);
			}
		}
	}

	private Vector3 lineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	{
		float num = ((p4.x - p3.x) * (p1.z - p3.z) - (p4.z - p3.z) * (p1.x - p3.x)) / ((p4.z - p3.z) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.z - p1.z));
		return new Vector3(p1.x + num * (p2.x - p1.x), 0f, p1.z + num * (p2.z - p1.z));
	}

	private void CreateViewFrustumMesh(GameObject character, float visionAngle, float visionRadius)
	{
	}

	private int EndPointCompare(EndPoint a, EndPoint b)
	{
		int num = b.angle.CompareTo(a.angle);
		if (num == 0)
		{
			if (!a.begin && b.begin)
			{
				return 1;
			}
			if (a.begin && !b.begin)
			{
				return -1;
			}
		}
		return num;
	}

	protected virtual void ToggleInteractable()
	{
		if (isInteractable)
		{
			outlineTimer = Time.time;
			return;
		}
		outlineTimer = 0f;
		if (skinnedMeshRenderers != null)
		{
			SkinnedMeshRenderer[] array = skinnedMeshRenderers;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (skinnedMeshRenderer != null && skinnedMeshRenderer.enabled)
				{
					skinnedMeshRenderer.material.SetFloat("_OutlineSwitch", 0f);
				}
				else if (skinnedMeshRenderer == null)
				{
					Debug.LogError("SkinnedMeshRenderer is null!");
				}
			}
		}
		if (meshRenderers == null)
		{
			return;
		}
		MeshRenderer[] array2 = meshRenderers;
		foreach (MeshRenderer meshRenderer in array2)
		{
			if (meshRenderer != null && meshRenderer.enabled)
			{
				meshRenderer.material.SetFloat("_OutlineSwitch", 0f);
			}
			else if (meshRenderer == null)
			{
				Debug.LogError("MeshRenderer is null!");
			}
		}
	}

	public void SpawnObject(string spawnTypePrefabName)
	{
		if (isPlayer)
		{
			Debug.LogWarning("Attampt to spawn an object from player. Only allowed for enemies!");
		}
		else
		{
			SpawnObject(Resources.Load<GameObject>(spawnTypePrefabName));
		}
	}

	public void SpawnObject(GameObject prefabObject)
	{
		if (isPlayer)
		{
			Debug.LogWarning("Attampt to spawn an object from player. Only allowed for enemies!");
		}
		StartCoroutine(DoSpawnObject(UnityEngine.Object.Instantiate(prefabObject)));
	}

	private IEnumerator DoSpawnObject(GameObject spawnedObject)
	{
		if (spawnedObject != null)
		{
			if (GameController.Instance.LevelVisualizer != null)
			{
				if (GameController.Instance.LevelVisualizer.levelRoot != null)
				{
					spawnedObject.transform.parent = GameController.Instance.LevelVisualizer.levelRoot.transform;
				}
				else
				{
					spawnedObject.transform.parent = EditorStateMachine.Instance.levelVisualizer.levelRoot.transform;
				}
				spawnedObject.transform.position = base.gameObject.transform.position + new Vector3(0f, 1f, 0f);
				bool hasRigidBody = false;
				Rigidbody rigidbody = spawnedObject.GetComponent<Rigidbody>();
				if (rigidbody != null)
				{
					hasRigidBody = true;
				}
				if (!hasRigidBody)
				{
					rigidbody = spawnedObject.AddComponent<Rigidbody>();
				}
				Vector3 forceVector = Vector3.up * 5.5f;
				rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
				rigidbody.AddTorque(new Vector3(0f, 50f, 0f));
				spawnedObject.GetComponent<BoxCollider>().enabled = false;
				while (spawnedObject.transform.position.y > base.transform.position.y + 0.5f)
				{
					yield return null;
				}
				spawnedObject.GetComponent<BoxCollider>().enabled = true;
				if (!hasRigidBody)
				{
					UnityEngine.Object.Destroy(spawnedObject.GetComponent<Rigidbody>());
				}
				spawnedObject.GetComponent<Collectible>().StartAttract();
			}
			else
			{
				Debug.LogError("GameController.Instance.LevelVisualizer is null");
			}
		}
		else
		{
			Debug.LogError("Character could not instantiate object");
		}
		yield return 0;
	}

	public void CenterOnPositionTile(float duration = 0.2f)
	{
		centerOnTileCoroutine = StartCoroutine(DoCenterOnPositionTile(duration));
	}

	private IEnumerator DoCenterOnPositionTile(float duration)
	{
		Vector3 pos = base.transform.position;
		Vector3 target = LevelHelper.NodeToVector3(CurrentPositionNode);
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			if (t > 1f)
			{
				t = 1f;
			}
			base.transform.position = Vector3.Lerp(pos, target, t);
			yield return null;
		}
		base.transform.position = target;
	}
}
