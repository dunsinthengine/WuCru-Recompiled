using System.Collections;
using UnityEngine;

public class BearTrap : InteractableObject
{
	private static BearTrap _lastPickedTrap;

	public MeshRenderer pulseRenderer;

	private Animator animator;

	private Character triggerCharacter;

	private bool isTriggered;

	private float triggerTime;

	private float triggerDuration = 1f;

	private bool pressAndHold;

	private bool isDismantled;

	public override int PickingPriority()
	{
		return 12;
	}

	protected override void OnInitialize()
	{
		animator = GetComponent<Animator>();
		onlyPlayer = true;
		Animator component = pulseRenderer.gameObject.GetComponent<Animator>();
		component.SetFloat("BlinkSpeed", Random.Range(0f, 1f));
	}

	protected override void OnUpdate()
	{
		if (ScreenHud._pInstance._hudChestMinigame._pInProgress && isTriggered)
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			if (ScreenHud._pInstance._hudChestMinigame._pIsInSafeZone)
			{
				isTriggered = false;
				if (triggerCharacter != null)
				{
					triggerCharacter.Animator.SetTrigger(TriggerHash.dismantled);
				}
				isDismantled = true;
				onlyPlayer = false;
				pulseRenderer.material.SetColor("_EmisColor", Color.green);
				triggerCharacter.Animator.SetTrigger(TriggerHash.dismantled);
				AudioPlayer.PlaySound("props/trap/deactivate/success", base.gameObject);
				GamePlayer.Instance.SelectedObject = null;
				if (OnInteract != null)
				{
					OnInteract(GameController.Instance.CurrentLevel, this);
				}
				AnalyticsEvents.LogTrapDismantling(true);
			}
			else
			{
				if (triggerCharacter != null)
				{
					triggerCharacter.Animator.SetTrigger(TriggerHash.dismantled);
				}
				isTriggered = false;
				isDismantled = false;
				AudioPlayer.PlaySound("props/trap/deactivate/fail", base.gameObject);
				AnalyticsEvents.LogTrapDismantling(false);
				StartCoroutine(DoDelayedTrap());
			}
			if (triggerCharacter != null)
			{
				triggerCharacter.ResetPath();
			}
			ScreenHud._pInstance._hudChestMinigame.CancelMinigame();
		}
		else if (!ScreenHud._pInstance._hudChestMinigame._pInProgress && isTriggered)
		{
			if (triggerCharacter != null)
			{
				triggerCharacter.Animator.SetTrigger(TriggerHash.dismantled);
			}
			isTriggered = false;
			isDismantled = false;
			AudioPlayer.PlaySound("props/trap/deactivate/fail", base.gameObject);
			AnalyticsEvents.LogTrapDismantling(false);
			StartCoroutine(DoDelayedTrap());
		}
	}

	protected override void OnEnter(Character character)
	{
		if ((isTriggered && (!character.isPlayer || animator.GetBool("Trap") || ScreenHud._pInstance._hudChestMinigame._pInProgress)) || (character.isPlayer && isDismantled))
		{
			return;
		}
		if (!character.isPlayer)
		{
			AIController aIController = character.Controller as AIController;
			if (aIController.agent is ElementalBossEnemy)
			{
				Debug.Log("Elemental Boss - Ignoring Trap");
				return;
			}
		}
		isTriggered = true;
		triggerCharacter = character;
		if (triggerCharacter.isPlayer && isDismantled)
		{
			return;
		}
		isDismantled = true;
		GamePlayer.Instance.SelectedObject = base.gameObject;
		if (!character.IsInSpinjitzu())
		{
			Debug.Log("OnEnter() " + triggerCharacter.isPlayer + " " + Input.GetMouseButton(0) + " " + (GamePlayer.Instance.SelectedObject == base.gameObject), GamePlayer.Instance.SelectedObject);
			if (triggerCharacter.isPlayer && _lastPickedTrap == this)
			{
				AudioPlayer.PlaySound("props/trap/hold", base.gameObject);
				ScreenHud._pInstance._hudChestMinigame.BeginGame(base.transform);
			}
			else
			{
				StartCoroutine(DoDelayedTrap());
			}
		}
		else
		{
			DoTrap();
		}
	}

	private IEnumerator DoDelayedTrap()
	{
		isDismantled = true;
		yield return new WaitForSeconds(0.2f);
		AudioPlayer.PlaySound("props/trap/spring", base.gameObject);
		DoTrap();
	}

	private void DoTrap()
	{
		isDismantled = true;
		animator.SetBool("Trap", true);
		triggerTime = Time.time;
		if (!triggerCharacter.IsInSpinjitzu())
		{
			triggerCharacter.Controller.ToggleTrapped(true);
			AIBeacon.Instance.ReportEvent(null, triggerCharacter.CurrentPositionNode, triggerCharacter.transform.position, 8f, 0.75f);
			if (triggerCharacter.isPlayer)
			{
				triggerCharacter.TakeDamage(GetDamagePlayer(), base.transform.position, DamageType.Trap);
			}
			else
			{
				triggerCharacter.TakeDamage(GetDamageEnemy(), base.transform.position, DamageType.Trap);
			}
		}
		else
		{
			isDismantled = true;
			GamePlayer.Instance.SelectedObject = null;
			if (OnInteract != null)
			{
				OnInteract(GameController.Instance.CurrentLevel, this);
			}
		}
		StartCoroutine(DelayedRemoveTrap());
	}

	private int GetDamagePlayer()
	{
		return 25 + Random.Range(-3, 4);
	}

	private int GetDamageEnemy()
	{
		return 150 + Random.Range(-50, 50);
	}

	protected override void OnExit(Character character)
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		if (isDismantled || isTriggered)
		{
			GamePlayer.Instance.SelectedObject = null;
			character.Controller.WalkTo(levelNode);
			return;
		}
		_lastPickedTrap = this;
		float num = Vector3.Distance(character.transform.position, base.transform.position);
		if (num < 0.1f)
		{
			OnEnter(character);
		}
		else
		{
			character.Controller.WalkTo(levelNode);
		}
	}

	public override bool ShouldCaptureInputWhileMouseOverObject()
	{
		return true;
	}

	private IEnumerator DelayedRemoveTrap()
	{
		yield return new WaitForSeconds(triggerDuration);
		base.transform.parent.gameObject.SetActive(false);
	}
}
