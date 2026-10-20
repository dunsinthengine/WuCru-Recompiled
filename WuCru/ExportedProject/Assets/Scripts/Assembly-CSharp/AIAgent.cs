using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public abstract class AIAgent : MonoBehaviour
{
	public delegate void OnAIAgentDeadDelegate(Level level, AIAgent agent);

	private int agentId;

	public TextMesh textMesh;

	public GameObject characterPrefab;

	public bool isBoss;

	public bool isFriend;

	public bool canHear;

	public float PatrollingSpeed = 1f;

	public float SpottingPlayerSpeed = 0.5f;

	public float SuspiciousSpeed = 1.8f;

	public float AlertSpeed = 3.1f;

	public float CombatSpeed = 2.8f;

	public float BeaconSpeed = 2.4f;

	public float FleeSpeed = 3f;

	public float PatrollingVisionRadius = 7f;

	public float SpottingPlayerVisionRadius = 7.5f;

	public float SuspiciousVisionRadius = 8.5f;

	public float AlertVisionRadius = 10f;

	public float CombatVisionRadius = 10f;

	public float BeaconVisionRadius = 8.5f;

	public float PatrollingVisionAngle = 140f;

	public float SpottingPlayerVisionAngle = 160f;

	public float SuspiciousVisionAngle = 220f;

	public float AlertVisionAngle = 220f;

	public float CombatVisionAngle = 180f;

	public float BeaconVisionAngle = 200f;

	public float PatrollingHearingRadius = 2.3f;

	public float SpottingPlayerHearingRadius = 2.3f;

	public float SuspiciousHearingRadius = 2.3f;

	public float AlertHearingRadius = 2.3f;

	public float CombatHearingRadius = 2.3f;

	public float BeaconHearingRadius = 2.3f;

	public float PatrollingHearingSpeedThreshold = 3f;

	public float SpottingPlayerHearingSpeedThreshold = 3f;

	public float SuspiciousHearingSpeedThreshold = 3f;

	public float AlertHearingSpeedThreshold = 3f;

	public float CombatHearingSpeedThreshold = 3f;

	public float BeaconHearingSpeedThreshold = 3f;

	public float duellingStrength = 0.5f;

	public float duellingVariance = 0.2f;

	public float duellingInterval = 0.2f;

	public float duellingAutoWinChance;

	public float combatCounterChance;

	public float combatDeflectChance;

	public bool performCounter;

	public int health = 100;

	[Header("Audio")]
	[SerializeField]
	private string deathAudioEvent;

	[SerializeField]
	private string spottingMeterAudioEvent = "fx/slow/spotting";

	private bool spottingSoundStarted;

	private float spottingMeterLowPassMin = 100f;

	private float spottingMeterLowPassMax = 12000f;

	[SerializeField]
	private string spottingMeterStartAudioEvent = "fx/soft/spot/start";

	private float lastSpottedSound;

	private float spottedSoundCooldown = 5f;

	private List<string> objectTags = new List<string>();

	protected AIController controller;

	protected AIPerception perception;

	private AIAgentState state;

	protected Character playerCharacter;

	protected Player player;

	protected AIPathMode pathMode;

	private float aiBeaconResponseThreshold = 0.1f;

	private Vector3 startPosition;

	private float spottetPlayerStartTime;

	private float spottetPlayerMeter;

	private float spottetPlayerMeterAccelerationMin = 0.1f;

	private float spottetPlayerMeterAccelerationMax = 1.6f;

	private float spottetPlayerMeterDecay = 1f;

	private float spottetStateShiftThreshold = 0.2f;

	private bool isSpottingPlayer;

	public OnAIAgentDeadDelegate OnAIAgentDead;

	public int AgentId
	{
		get
		{
			return agentId;
		}
		set
		{
			agentId = value;
		}
	}

	public Character Character
	{
		get
		{
			return controller.Character;
		}
	}

	public AIAgentState State
	{
		get
		{
			return state;
		}
	}

	public LevelNode LastKnownPlayerLocation
	{
		get
		{
			return perception.LastKnownPlayerLocation;
		}
	}

	public bool IsPlayerVisible
	{
		get
		{
			return perception.IsPlayerVisible && spottetPlayerMeter >= 1f;
		}
	}

	public AIPerception Perception
	{
		get
		{
			return perception;
		}
	}

	public Character PlayerCharacter
	{
		get
		{
			return playerCharacter;
		}
	}

	public Player Player
	{
		get
		{
			return player;
		}
	}

	public AIPathMode PathMode
	{
		get
		{
			return pathMode;
		}
		set
		{
			pathMode = value;
		}
	}

	public float AIBeaconResponseThreshold
	{
		get
		{
			return aiBeaconResponseThreshold;
		}
	}

	public bool IsAlerted
	{
		get
		{
			return state == AIAgentState.Alert && GamePlayer.Instance.State != GamePlayerState.Combat;
		}
	}

	public bool IsSuspisious
	{
		get
		{
			return (state == AIAgentState.Suspicious || state == AIAgentState.BeaconCall) && GamePlayer.Instance.State != GamePlayerState.Combat;
		}
	}

	public float SpottetPlayerMeter
	{
		get
		{
			return spottetPlayerMeter;
		}
	}

	public void AddObjectTag(string tag)
	{
		objectTags.Add(tag);
	}

	public bool HasObjectTag(string tag)
	{
		foreach (string objectTag in objectTags)
		{
			if (objectTag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void OnUpdate()
	{
		if (player != null && player.Character != null && !GamePlayer.Instance.Player.Character.IsInSpinjitzu())
		{
			if (isSpottingPlayer)
			{
				float t = Mathf.Clamp01(1f - Vector3.Distance(player.Character.transform.position, controller.Character.transform.position) / perception.visionRadius);
				spottetPlayerMeter += Mathf.Clamp01(Mathf.Lerp(spottetPlayerMeterAccelerationMin, spottetPlayerMeterAccelerationMax, t) * Time.deltaTime);
			}
			else
			{
				spottetPlayerMeter -= Mathf.Clamp01(spottetPlayerMeterDecay * Time.deltaTime);
			}
			spottetPlayerMeter = Mathf.Clamp01(spottetPlayerMeter);
			if (spottetPlayerMeter >= 1f && state < AIAgentState.Alert)
			{
				SetState(AIAgentState.Alert);
				SetSpottedMeterSoundState();
				GameController.Instance.CurrentLevel.Challenge.OnPlayerSpotted(Character.CurrentPositionNode, true);
			}
			if (spottetPlayerMeter >= spottetStateShiftThreshold && state < AIAgentState.SpottingPlayer)
			{
				SetState(AIAgentState.SpottingPlayer);
			}
		}
	}

	public virtual void OnPlayerSpotted(LevelNode location, bool visible)
	{
		isSpottingPlayer = visible;
		if (!visible || !playerCharacter.IsInSpinjitzu() || state == AIAgentState.Scared)
		{
			return;
		}
		if (Character.IsElementalBoss())
		{
			if (Character.elementalPower == GamePlayer.Instance.Player.Character.elementalPower)
			{
				SetState(AIAgentState.Scared);
			}
		}
		else
		{
			SetState(AIAgentState.Scared);
		}
	}

	public virtual void OnPlayerHeard(LevelNode location, bool heard)
	{
		if (State < AIAgentState.Suspicious && !Character.IsTrapped() && !Character.IsDead() && !playerCharacter.IsPreparingSneakAttack() && canHear)
		{
			SetState(AIAgentState.Suspicious);
		}
	}

	public virtual void OnStateChange()
	{
	}

	public virtual void OnAIBeaconCall()
	{
		if (!Character.IsTrapped() && !Character.IsInCombat() && !Character.IsPreparingCombat() && state < AIAgentState.BeaconCall && state != AIAgentState.Scared)
		{
			SetState(AIAgentState.BeaconCall);
		}
	}

	public abstract AIAgentType GetAgentType();

	protected virtual void Awake()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "AIController";
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		controller = gameObject.AddComponent<AIController>();
		controller.agent = this;
		controller.InitializeFromAgent();
		GameObject gameObject2 = new GameObject();
		gameObject2.name = "Perception";
		perception = gameObject2.AddComponent<AIPerception>();
		perception.agent = this;
		perception.transform.parent = Character.transform;
		perception.transform.localPosition = Character.GetEyePosition();
		AIPerception aIPerception = perception;
		aIPerception.OnPlayerSpotted = (AIPerception.OnPlayerSpottedDelegate)Delegate.Combine(aIPerception.OnPlayerSpotted, new AIPerception.OnPlayerSpottedDelegate(OnPlayerSpotted));
		AIPerception aIPerception2 = perception;
		aIPerception2.OnPlayerHeard = (AIPerception.OnPlayerHeardDelegate)Delegate.Combine(aIPerception2.OnPlayerHeard, new AIPerception.OnPlayerHeardDelegate(OnPlayerHeard));
		startPosition = base.transform.position;
		StartCoroutine(InitEmoticon());
		if (textMesh != null)
		{
			textMesh.transform.parent = GetComponentInChildren<Character>().transform;
			if ((EditorStateMachine.Instance != null && !EditorStateMachine.Instance.debugTextOnCharacters) || (GameController.Instance != null && !GameController.Instance.debugTextOnCharacters))
			{
				textMesh.gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator InitEmoticon()
	{
		yield return 0;
	}

	private void Update()
	{
		if (controller.Character.IsDead() & spottingSoundStarted)
		{
			SetSpottedMeterSoundState();
		}
		if (Character.IsDead() || (!(GamePlayer.Instance.Player == null) && GamePlayer.Instance.Player.Character.IsInSpinjitzu()))
		{
			return;
		}
		if (!isFriend && spottetPlayerMeter > 0f)
		{
			EventManager.Instance.SetDSPParameter(spottingMeterAudioEvent, DSPType.LowPass, "CutoffFrequency", spottingMeterLowPassMin + spottetPlayerMeter * (spottingMeterLowPassMax - spottingMeterLowPassMin), 0f, 0.5f, Character.gameObject);
			EventManager.Instance.PostEvent(spottingMeterAudioEvent, EventAction.SetPitchProperty, 1f + spottetPlayerMeter * 0.5f, Character.gameObject);
			if (!spottingSoundStarted && lastSpottedSound + spottedSoundCooldown < Time.time && (state == AIAgentState.Idle || state == AIAgentState.Patrolling || state == AIAgentState.Suspicious || state == AIAgentState.BeaconCall))
			{
				AudioPlayer.PlaySound(spottingMeterStartAudioEvent, Character.gameObject);
				SetSpottedMeterSoundState(true);
				lastSpottedSound = Time.time;
			}
		}
		if (!isFriend)
		{
			if (IsAlerted || State == AIAgentState.SpottingPlayer)
			{
				HUDItemTracker hUDItemTracker = ScreenHud._pInstance._enemyIconTracker;
				if (GameController.Instance.Seed >= 5000)
				{
					hUDItemTracker = ScreenHud._pInstance._enemyIconTrackerCity;
				}
				hUDItemTracker.DisplayIcon(controller.Character.gameObject.transform, false, 0.1f);
				ScreenHud._pInstance._enemyIconSeenTracker.DisplayIcon(controller.Character.gameObject.transform, false, 0.1f);
				ScreenHud._pInstance._enemyIconSuspiciousTracker.DismissIcon(controller.Character.gameObject.transform);
				ScreenHud._pInstance._enemyIconSeenTracker.SetScale(new Vector3(SpottetPlayerMeter, SpottetPlayerMeter, SpottetPlayerMeter), controller.Character.transform);
			}
			else if (IsSuspisious)
			{
				HUDItemTracker hUDItemTracker2 = ScreenHud._pInstance._enemyIconTracker;
				if (GameController.Instance.Seed >= 5000)
				{
					hUDItemTracker2 = ScreenHud._pInstance._enemyIconTrackerCity;
				}
				hUDItemTracker2.DisplayIcon(controller.Character.gameObject.transform, false, 0.1f);
				ScreenHud._pInstance._enemyIconSeenTracker.DismissIcon(controller.Character.gameObject.transform);
				ScreenHud._pInstance._enemyIconSuspiciousTracker.DisplayIcon(controller.Character.gameObject.transform, false, 0.1f, 0.1f);
			}
			else if (!IsAlerted && !IsSuspisious)
			{
				ScreenHud._pInstance._enemyIconTracker.DismissIcon(controller.Character.gameObject.transform);
				ScreenHud._pInstance._enemyIconSeenTracker.DismissIcon(controller.Character.gameObject.transform);
				ScreenHud._pInstance._enemyIconSuspiciousTracker.DismissIcon(controller.Character.gameObject.transform);
				SetSpottedMeterSoundState();
			}
		}
		if ((playerCharacter == null || player == null) && GamePlayer.Instance != null && GamePlayer.Instance.Player != null)
		{
			player = GamePlayer.Instance.Player;
			playerCharacter = player.Character;
		}
		if (state == AIAgentState.Idle)
		{
			SetState(AIAgentState.Patrolling);
		}
		OnUpdate();
		if ((EditorStateMachine.Instance != null && EditorStateMachine.Instance.debugTextOnCharacters) || (GameController.Instance != null && GameController.Instance.debugTextOnCharacters))
		{
			if (!textMesh.gameObject.activeSelf)
			{
				textMesh.gameObject.SetActive(true);
			}
			if (textMesh != null)
			{
				textMesh.transform.rotation = Quaternion.identity;
			}
			if (textMesh != null)
			{
				if (Character.IsDead())
				{
					textMesh.text = string.Empty;
					return;
				}
				textMesh.text = string.Concat(agentId, ":", controller.CurrentBehaviour, "/", controller.Character.CurrentStateName());
			}
		}
		else if (textMesh != null)
		{
			textMesh.text = string.Empty;
		}
	}

	public void Reset()
	{
		Character.ForceMove(startPosition);
		if (Character.GetSwordTappingIcon() != null)
		{
			Character.GetSwordTappingIcon().Show(false);
			ScreenHud._pInstance._swordTapTelegraph.Hide();
		}
		perception.SetLastKnownPlayerLocation(null);
		controller.SetBehaviour(null);
		SetState(AIAgentState.Idle);
		Character.Animator.SetTrigger("Reset");
		spottetPlayerMeter = 0f;
		OnStateChange();
	}

	public void SetMaximumAlert()
	{
		spottetPlayerMeter = 1f;
		perception.SetLastKnownPlayerLocation(GamePlayer.Instance.Player.Character.CurrentPositionNode);
	}

	public void SetState(AIAgentState state)
	{
		if ((this.state == AIAgentState.Dead && state != AIAgentState.Idle) || (this.state == state && state != AIAgentState.Idle))
		{
			return;
		}
		this.state = state;
		switch (state)
		{
		case AIAgentState.Patrolling:
			controller.Character.Speed = PatrollingSpeed;
			perception.SetVisionRadius(PatrollingVisionRadius);
			perception.SetVisionAngle(PatrollingVisionAngle);
			perception.SetHearingRadius(PatrollingHearingRadius);
			perception.SetHearingSpeedThreashold(PatrollingHearingSpeedThreshold);
			break;
		case AIAgentState.SpottingPlayer:
			controller.Character.Speed = SpottingPlayerSpeed;
			perception.SetVisionRadius(SpottingPlayerVisionRadius);
			perception.SetVisionAngle(SpottingPlayerVisionAngle);
			perception.SetHearingRadius(SpottingPlayerHearingRadius);
			perception.SetHearingSpeedThreashold(SpottingPlayerHearingSpeedThreshold);
			break;
		case AIAgentState.Suspicious:
			controller.Character.Speed = SuspiciousSpeed;
			perception.SetVisionRadius(SuspiciousVisionRadius);
			perception.SetVisionAngle(SuspiciousVisionAngle);
			perception.SetHearingRadius(SuspiciousHearingRadius);
			perception.SetHearingSpeedThreashold(SuspiciousHearingSpeedThreshold);
			break;
		case AIAgentState.Alert:
			controller.Character.Speed = AlertSpeed;
			perception.SetVisionRadius(AlertVisionRadius);
			perception.SetVisionAngle(AlertVisionAngle);
			perception.SetHearingRadius(AlertHearingRadius);
			perception.SetHearingSpeedThreashold(AlertHearingSpeedThreshold);
			break;
		case AIAgentState.Combat:
			controller.Character.Speed = CombatSpeed;
			perception.SetVisionRadius(CombatVisionRadius);
			perception.SetVisionAngle(CombatVisionAngle);
			perception.SetHearingRadius(CombatHearingRadius);
			perception.SetHearingSpeedThreashold(CombatHearingSpeedThreshold);
			break;
		case AIAgentState.BeaconCall:
			controller.Character.Speed = BeaconSpeed;
			perception.SetVisionRadius(BeaconVisionRadius);
			perception.SetVisionAngle(BeaconVisionAngle);
			perception.SetHearingRadius(BeaconHearingRadius);
			perception.SetHearingSpeedThreashold(BeaconHearingSpeedThreshold);
			break;
		case AIAgentState.Dead:
			AudioPlayer.PlaySound(deathAudioEvent, base.gameObject);
			SetSpottedMeterSoundState();
			if (OnAIAgentDead != null)
			{
				OnAIAgentDead(GameController.Instance.CurrentLevel, this);
			}
			break;
		}
		OnStateChange();
	}

	private void OnDestroy()
	{
		if (controller != null && controller.gameObject != null)
		{
			UnityEngine.Object.Destroy(controller.gameObject);
		}
		if (perception != null && perception.gameObject != null)
		{
			UnityEngine.Object.Destroy(perception.gameObject);
		}
	}

	private void SetSpottedMeterSoundState(bool on = false)
	{
		if (on)
		{
			AudioPlayer.PlaySound(spottingMeterAudioEvent, Character.gameObject);
		}
		else
		{
			AudioPlayer.StopSound(spottingMeterAudioEvent, Character.gameObject);
		}
		spottingSoundStarted = on;
	}
}
