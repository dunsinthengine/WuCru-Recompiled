using System.Collections;
using UnityEngine;

public class InGameCam : MonoBehaviour
{
	private static InGameCam _instance;

	private static Color _clearColor;

	private InGameCamState state = InGameCamState.Starting;

	private InGameCamState prevState;

	private Player player;

	private Transform target;

	private CamSettingsManager settingsManager;

	private Vector3 lookAt;

	private bool isInInterestZone;

	private bool isAtInterestPoint;

	private InterestPoint currentInterestPoint;

	private Vector3 shakeVector = Vector3.zero;

	private float zoomAmount;

	public static InGameCam _pInstance
	{
		get
		{
			return _instance;
		}
	}

	public static Color _pClearColor
	{
		get
		{
			return _clearColor;
		}
		set
		{
			_clearColor = value;
			_clearColor.a = 1f;
		}
	}

	public Transform Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	private void Awake()
	{
		Debug.Log("---------------------IN GAME CAM!---------------", base.gameObject);
		_instance = this;
		Camera component = GetComponent<Camera>();
		component.clearFlags = CameraClearFlags.Color;
		component.backgroundColor = _pClearColor;
		Init();
		settingsManager = base.gameObject.AddComponent<CamSettingsManager>();
		if (target != null)
		{
			lookAt = GetStateLookAt();
			base.transform.position = GetStatePos();
			base.transform.LookAt(lookAt);
		}
		if (player != null)
		{
			StartCoroutine(DelayedExplorationState());
		}
	}

	private void Init()
	{
		if (player == null)
		{
			player = Object.FindObjectOfType<Player>();
		}
		if (!(target == null))
		{
			return;
		}
		if (player != null)
		{
			target = player.Character.transform;
			return;
		}
		TraversalController traversalController = Object.FindObjectOfType<TraversalController>();
		if (traversalController != null && traversalController.car != null)
		{
			target = traversalController.car.transform;
		}
	}

	private IEnumerator DelayedExplorationState()
	{
		yield return new WaitForSeconds(1f);
		if (state == InGameCamState.Starting)
		{
			SetState(InGameCamState.Exploring);
		}
	}

	private void Update()
	{
		if (player == null || target == null)
		{
			Init();
		}
		if (player != null)
		{
			if (GamePlayer.Instance.State == GamePlayerState.Combat && state != InGameCamState.Combat && GamePlayer.Instance.CombatManager.CurrentAttacker != null)
			{
				SetState(InGameCamState.Combat);
			}
			if (GamePlayer.Instance.State != GamePlayerState.Combat && state == InGameCamState.Combat)
			{
				if (isInInterestZone)
				{
					SetState(InGameCamState.InterestZone);
				}
				else if (isAtInterestPoint)
				{
					SetState(InGameCamState.InterestPoint);
				}
				else
				{
					SetState(InGameCamState.Exploring);
				}
			}
			if (player.Character.IsHiding() && state != InGameCamState.Stealth && state != InGameCamState.InterestZone)
			{
				SetState(InGameCamState.Stealth);
			}
			if (!player.Character.IsHiding() && state == InGameCamState.Stealth)
			{
				SetState(InGameCamState.Exploring);
			}
		}
		if (target != null)
		{
			base.transform.position = shakeVector + Vector3.Lerp(base.transform.position, GetStatePos(), GetStateLerpValue() * Time.deltaTime);
			lookAt = Vector3.Lerp(lookAt, GetStateLookAt(), GetStateLerpValue() * Time.deltaTime);
			base.transform.LookAt(lookAt);
		}
		if (state == InGameCamState.Exploring)
		{
			settingsManager.EvaluateExplorationCam();
		}
		prevState = state;
	}

	public void Shake(float strength, float duration)
	{
		StartCoroutine(DoShake(strength, duration));
	}

	public void Zoom(float amount, float duration, float transitionDuration)
	{
		StartCoroutine(DoZoom(amount, duration, transitionDuration));
	}

	private IEnumerator DoShake(float strength, float duration)
	{
		GameControllerStatePlaying controller = GameController.Instance.currentState as GameControllerStatePlaying;
		if (!(controller != null) || !controller.IsConfirmingQuit)
		{
			float startTime = Time.time;
			while (Time.time - startTime <= duration && (!(controller != null) || !controller.IsConfirmingQuit))
			{
				float t = 1f - (Time.time - startTime) / duration;
				shakeVector = new Vector3(Random.Range((0f - strength) * t, strength * t), Random.Range((0f - strength) * t, strength * t), Random.Range((0f - strength) * t, strength * t));
				yield return 0;
			}
			shakeVector = Vector3.zero;
		}
	}

	private IEnumerator DoZoom(float amount, float duration, float transitionDuration)
	{
		float startTime = Time.time;
		zoomAmount = amount;
		while (Time.time - startTime <= duration)
		{
			yield return 0;
		}
		startTime = Time.time;
		while (Time.time - startTime <= transitionDuration)
		{
			float t = 1f - (Time.time - startTime) / duration;
			zoomAmount = Mathf.Lerp(0f, amount, t);
			yield return 0;
		}
		zoomAmount = 0f;
	}

	public void OnInterestZoneEnter(InterestZone zone)
	{
		isInInterestZone = true;
		if (state != InGameCamState.Combat && state != InGameCamState.InterestPoint)
		{
			SetState(InGameCamState.InterestZone);
		}
	}

	public void OnInterestZoneExit(InterestZone zone)
	{
		isInInterestZone = false;
		if (isAtInterestPoint)
		{
			SetState(InGameCamState.InterestPoint);
		}
		else if (state != InGameCamState.Combat)
		{
			SetState(InGameCamState.Exploring);
		}
	}

	public void OnInterestPointEnter(InterestPoint point)
	{
		isAtInterestPoint = true;
		currentInterestPoint = point;
		if (state != InGameCamState.Combat)
		{
			SetState(InGameCamState.InterestPoint);
		}
	}

	public void OnInterestPointExit(InterestPoint point)
	{
		isAtInterestPoint = false;
		currentInterestPoint = null;
		if (isInInterestZone)
		{
			SetState(InGameCamState.InterestZone);
		}
		else if (state != InGameCamState.Combat)
		{
			SetState(InGameCamState.Exploring);
		}
	}

	public void SetState(InGameCamState newState)
	{
		if (state != newState)
		{
			state = newState;
			settingsManager.Transition(newState, settingsManager.TransitionOutTime);
		}
	}

	private Vector3 ZoomOffset(Transform zoomTarget, float zoom)
	{
		Vector3 normalized = (zoomTarget.position - base.transform.position).normalized;
		normalized *= zoom;
		return settingsManager.PositionOffset + normalized;
	}

	private Vector3 GetStatePos()
	{
		if (state != InGameCamState.Combat)
		{
			return target.position + settingsManager.PositionOffset + new Vector3(0f, 0f, 0f - settingsManager.Dist + zoomAmount);
		}
		if (GamePlayer.Instance.CombatManager.SecondaryAttacker != null)
		{
			return GamePlayer.Instance.CombatManager.SecondaryAttacker.Character.transform.position + ZoomOffset(GamePlayer.Instance.CombatManager.SecondaryAttacker.Character.transform, 2f) + new Vector3(0f, 0f, 0f - settingsManager.Dist);
		}
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
		{
			if (player.Character.CurrentStateNameHash() == StateHash.playerAttackOption)
			{
				return player.Character.transform.position + ZoomOffset(player.Character.transform, 2.4f) + new Vector3(0f, 0f, 0f - settingsManager.Dist);
			}
			if (player.Character.CurrentStateNameHash() == StateHash.enemyAttackOption)
			{
				return player.Character.transform.position + ZoomOffset(GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform, 3f) + new Vector3(0f, 0f, 0f - settingsManager.Dist);
			}
			return player.Character.transform.position + ZoomOffset(player.Character.transform, 0.5f) + new Vector3(0f, 0f, 0f - settingsManager.Dist);
		}
		return player.Character.transform.position + settingsManager.PositionOffset + new Vector3(0f, 0f, 0f - settingsManager.Dist);
	}

	private Vector3 GetStateLookAt()
	{
		if (state != InGameCamState.Combat)
		{
			return target.position + settingsManager.LookAtOffset;
		}
		if (GamePlayer.Instance.CombatManager.SecondaryAttacker != null)
		{
			return GamePlayer.Instance.CombatManager.SecondaryAttacker.Character.transform.position + settingsManager.LookAtOffset;
		}
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
		{
			if (player.Character.CurrentStateNameHash() == StateHash.duelling)
			{
				return player.Character.swordTransform.position;
			}
			if (player.Character.CurrentStateNameHash() == StateHash.playerAttackOption)
			{
				return player.Character.swordTransform.position;
			}
			if (player.Character.CurrentStateNameHash() == StateHash.enemyAttackOption)
			{
				return GamePlayer.Instance.CombatManager.CurrentAttacker.Character.swordTransform.position;
			}
			return player.Character.swordTransform.position;
		}
		return player.Character.transform.position + settingsManager.LookAtOffset;
	}

	private float GetStateLerpValue()
	{
		return settingsManager.InterpolationSpeed;
	}
}
