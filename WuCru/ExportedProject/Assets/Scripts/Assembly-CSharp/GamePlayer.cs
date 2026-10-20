using System;
using System.Collections;
using Fabric;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
	private bool useNewCamera = true;

	private InGameCam ingameCam;

	private PlayCam playCam;

	private Player player;

	private Level level;

	private ScreenFlash screenFlash;

	private MovementPointer movementPointer;

	private bool showAIPaths;

	private GamePlayerState state;

	private CombatManager combatManager;

	private float aiBeaconBlinkTime;

	private float aiBeaconBlinkInterval = 0.1f;

	private static GamePlayer instance;

	private GameObject selectedObject;

	private float timeScaleTarget = 1f;

	private float timeScaleChangeDuration = 1.5f;

	private float timeBlinkDuration = 0.5f;

	private float timeLastPickup;

	private float studPitch;

	public Vector3 SelectedObjectPosition;

	public static GamePlayer Instance
	{
		get
		{
			return instance;
		}
	}

	public Player Player
	{
		get
		{
			return player;
		}
	}

	public Level Level
	{
		get
		{
			return level;
		}
	}

	public GamePlayerState State
	{
		get
		{
			return state;
		}
	}

	public CombatManager CombatManager
	{
		get
		{
			return combatManager;
		}
	}

	public ScreenFlash ScreenFlash
	{
		get
		{
			return screenFlash;
		}
	}

	public MovementPointer MovementPointer
	{
		get
		{
			return movementPointer;
		}
	}

	public bool ShowAIPaths
	{
		get
		{
			return showAIPaths;
		}
		set
		{
			showAIPaths = value;
		}
	}

	public GameObject SelectedObject
	{
		get
		{
			return selectedObject;
		}
		set
		{
			if (value != selectedObject)
			{
				selectedObject = value;
				if (value != null)
				{
					SelectedObjectPosition = Camera.main.WorldToScreenPoint(value.transform.position);
				}
				else
				{
					SelectedObjectPosition = Vector3.zero;
				}
			}
		}
	}

	private void Awake()
	{
		instance = this;
		TriggerHash.Initialize();
		StateHash.Initialize();
		VarHash.Initialize();
		combatManager = base.gameObject.AddComponent<CombatManager>();
		GameController gameController = GameController.Instance;
		gameController.OnAlertStateBegin = (GameController.OnAlertStateBeginDelegate)Delegate.Combine(gameController.OnAlertStateBegin, new GameController.OnAlertStateBeginDelegate(AlertMusicBegin));
		GameController gameController2 = GameController.Instance;
		gameController2.OnAlertStateEnd = (GameController.OnAlertStateEndDelegate)Delegate.Combine(gameController2.OnAlertStateEnd, new GameController.OnAlertStateEndDelegate(AlertMusicEnd));
		movementPointer = MovementPointer.CreateNew();
	}

	public void Reset()
	{
		timeScaleTarget = 1f;
		Time.timeScale = 1f;
		combatManager.Reset();
		SetState(GamePlayerState.Exploring);
		Level.ResetAI();
		movementPointer.Hide();
	}

	public void SetTimeScale(float scale, float duration)
	{
		GameControllerStatePlaying gameControllerStatePlaying = GameController.Instance.currentState as GameControllerStatePlaying;
		if (gameControllerStatePlaying != null && !gameControllerStatePlaying.IsConfirmingQuit)
		{
			timeScaleTarget = scale;
			timeScaleChangeDuration = duration;
			StartCoroutine(ChangeTimeScale());
		}
	}

	private IEnumerator ChangeTimeScale()
	{
		if (Time.timeScale != timeScaleTarget && !ScreenResults._pInResults)
		{
			if (timeScaleTarget > Time.timeScale)
			{
				EventManager.Instance.PostEvent("player/combat/speedup");
			}
			else if (timeScaleTarget < Time.timeScale)
			{
				EventManager.Instance.PostEvent("player/combat/slowdown");
			}
			float localTarget = timeScaleTarget;
			float startTime = Time.realtimeSinceStartup;
			float crntTimeScale = Time.timeScale;
			while (startTime + timeScaleChangeDuration > Time.realtimeSinceStartup)
			{
				yield return new WaitForEndOfFrame();
				if (timeScaleTarget != localTarget)
				{
					break;
				}
				float t = (Time.realtimeSinceStartup - startTime) / timeScaleChangeDuration;
				GameControllerStatePlaying controller = GameController.Instance.currentState as GameControllerStatePlaying;
				if (controller != null && !controller.IsConfirmingQuit)
				{
					Time.timeScale = Mathf.Lerp(crntTimeScale, timeScaleTarget, t);
				}
			}
		}
		else
		{
			yield return 0;
		}
	}

	public void TimeBlink(float duration)
	{
		timeBlinkDuration = duration;
		StartCoroutine(PerformTimeBlink());
	}

	private IEnumerator PerformTimeBlink()
	{
		float startTime = Time.realtimeSinceStartup;
		Time.timeScale = 0f;
		while (startTime + timeBlinkDuration > Time.realtimeSinceStartup)
		{
			yield return null;
		}
		GameControllerStatePlaying controller = GameController.Instance.currentState as GameControllerStatePlaying;
		if (controller != null && !controller.IsConfirmingQuit)
		{
			Time.timeScale = 1f;
		}
		else if (EditorStateMachine.Instance != null)
		{
			Time.timeScale = 1f;
		}
	}

	public void SetPickupTime()
	{
		timeLastPickup = Time.realtimeSinceStartup;
	}

	public float GetTimeSinceLastPickup(float currentTime)
	{
		return currentTime - timeLastPickup;
	}

	public float StudPitchIncrease()
	{
		studPitch++;
		if (studPitch > 10f)
		{
			studPitch = 10f;
		}
		return studPitch;
	}

	public float StudPitchReset()
	{
		studPitch = 0f;
		return 0f;
	}

	public void Initialize(Level level)
	{
		this.level = level;
	}

	public void SetState(GamePlayerState newState)
	{
		if (newState != state)
		{
			InitializeState(newState);
			state = newState;
		}
	}

	private void InitializeState(GamePlayerState newState)
	{
		if (newState == state)
		{
			Debug.LogError(string.Concat("Cannot change to state '", newState, "' as this is current state"));
			return;
		}
		switch (newState)
		{
		case GamePlayerState.Idle:
			if (player != null)
			{
				UnityEngine.Object.Destroy(player);
			}
			if (playCam != null)
			{
				UnityEngine.Object.Destroy(playCam);
			}
			AudioPlayer.StopSound(level.LevelStyle.AmbienceAudioEventName);
			AudioPlayer.StopSound(level.LevelStyle.MusicAudioEventName);
			combatManager.CurrentAttacker = null;
			break;
		case GamePlayerState.Exploring:
			if (state == GamePlayerState.Idle)
			{
				combatManager.Reset();
				player = base.gameObject.AddComponent<Player>();
				if (useNewCamera)
				{
					if (ingameCam != null)
					{
						UnityEngine.Object.Destroy(ingameCam);
					}
					Debug.Log("---------------ADDING IN GAME CAM---------------");
					ingameCam = Camera.main.gameObject.AddComponent<InGameCam>();
				}
				else
				{
					Debug.Log("---------------ADDING PLAYCAM---------------");
					playCam = Camera.main.gameObject.AddComponent<PlayCam>();
				}
				AudioPlayer.PlaySound(level.LevelStyle.AmbienceAudioEventName);
				AudioPlayer.PlaySound(level.LevelStyle.MusicAudioEventName);
				AudioPlayer.SetSwitch(level.LevelStyle.MusicAudioEventName, level.LevelStyle.ExplorationMusicComponentName);
			}
			else if (state == GamePlayerState.Combat)
			{
				AudioPlayer.SetSwitch(level.LevelStyle.MusicAudioEventName, level.LevelStyle.ExplorationMusicComponentName);
				if (player.Character.IsInCombat() || player.Character.IsPreparingCombat())
				{
					player.Character.Animator.SetTrigger(TriggerHash.combatEnd);
				}
				combatManager.Reset();
				SetTimeScale(1f, 0.1f);
				AnalyticsEvents.LogEndCombat();
			}
			break;
		case GamePlayerState.Combat:
			AudioPlayer.SetSwitch(level.LevelStyle.MusicAudioEventName, level.LevelStyle.CombatMusicComponentName);
			combatManager.InitializeCombat();
			AnalyticsEvents.LogStartCombat();
			break;
		}
	}

	public void Update()
	{
		if (screenFlash == null)
		{
			screenFlash = UnityEngine.Object.FindObjectOfType<ScreenFlash>();
		}
		switch (state)
		{
		case GamePlayerState.Exploring:
			UpdateAIBeacon();
			break;
		case GamePlayerState.Combat:
			UpdateAIBeacon();
			combatManager.UpdateCombat();
			break;
		}
		if (state != GamePlayerState.Idle && state != GamePlayerState.Combat && CombatManager.NumEnemies > 0)
		{
			SetState(GamePlayerState.Combat);
		}
	}

	private void UpdateAIBeacon()
	{
		if (aiBeaconBlinkTime + aiBeaconBlinkInterval < Time.time)
		{
			aiBeaconBlinkTime = Time.time;
			if (level != null)
			{
				AIBeacon.Instance.Blink(level.Agents);
			}
		}
	}

	public void EditorPlayFromHere(int row, int col)
	{
		player.Character.ForceMove(row, col);
	}

	private void OnDestroy()
	{
		if (combatManager != null)
		{
			UnityEngine.Object.Destroy(combatManager);
		}
		if (GameController._pGameControllerExists)
		{
			if (GameController.Instance.OnAlertStateBegin != null)
			{
				GameController gameController = GameController.Instance;
				gameController.OnAlertStateBegin = (GameController.OnAlertStateBeginDelegate)Delegate.Remove(gameController.OnAlertStateBegin, new GameController.OnAlertStateBeginDelegate(AlertMusicBegin));
			}
			if (GameController.Instance.OnAlertStateEnd != null)
			{
				GameController gameController2 = GameController.Instance;
				gameController2.OnAlertStateEnd = (GameController.OnAlertStateEndDelegate)Delegate.Remove(gameController2.OnAlertStateEnd, new GameController.OnAlertStateEndDelegate(AlertMusicEnd));
			}
		}
		if (player != null)
		{
			UnityEngine.Object.Destroy(player);
		}
		if (playCam != null)
		{
			UnityEngine.Object.Destroy(playCam);
		}
	}

	private void AlertMusicBegin()
	{
		if (GameController.Instance.AlertCount > 0 && state != GamePlayerState.Combat && !AudioPlayer.IsPlaying(level.LevelStyle.AudioHierarchyPrefix + level.LevelStyle.AlertedMusicComponentName))
		{
			AudioPlayer.SetSwitch(level.LevelStyle.MusicAudioEventName, level.LevelStyle.AlertedMusicComponentName);
		}
	}

	private void AlertMusicEnd()
	{
		if (state != GamePlayerState.Combat && !AudioPlayer.IsPlaying(level.LevelStyle.AudioHierarchyPrefix + level.LevelStyle.ExplorationMusicComponentName))
		{
			AudioPlayer.SetSwitch(level.LevelStyle.MusicAudioEventName, level.LevelStyle.ExplorationMusicComponentName);
		}
	}
}
