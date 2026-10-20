using Fabric;
using UnityEngine;

public class EnemyAttackOption_player : StateMachineBehaviour
{
	private Character character;

	private SwordTappingIcon sti;

	private bool ended;

	private const string AttackSuccessAudioEvent = "player/attack/success";

	private const string AttackFailAudioEvent = "player/attack/fail";

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = GamePlayer.Instance.CombatManager.consecutiveEnemyAttackOptions++;
		character = animator.transform.parent.GetComponent<Character>();
		sti = GamePlayer.Instance.CombatManager.CurrentAttacker.Character.GetSwordTappingIcon();
		GamePlayer.Instance.CombatManager.CurrentAttacker.performCounter = false;
		sti.Show(true, (GameController.Instance.PlayerProfile.WeaponLevel >= 2) ? true : false);
		ScreenHud._pInstance._swordTapTelegraph.Show(sti);
		sti.Apply(character.GetWeapon().GetParryTappingSettings());
		ended = false;
		if (num <= 4)
		{
			if (num > 3)
			{
				GamePlayer.Instance.SetTimeScale(0.8f, 1f);
			}
			else if (num > 2)
			{
				GamePlayer.Instance.SetTimeScale(0.5f, 1f);
			}
			else if (num > 1)
			{
				GamePlayer.Instance.SetTimeScale(0.2f, 1.2f);
			}
			else
			{
				GamePlayer.Instance.SetTimeScale(0.1f, 2f);
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamePlayer.Instance.SetTimeScale(1f, 0.2f);
		sti.Show(false);
		ScreenHud._pInstance._swordTapTelegraph.Hide();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null && !animator.IsInTransition(layerIndex))
		{
			character.transform.LookAt(GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform);
			if (ended)
			{
				return;
			}
			if (Input.GetMouseButtonDown(0) && sti.IsPointInsideIcon(Input.mousePosition))
			{
				if (sti.IsInWindow)
				{
					if (sti.IsInPerfectWindow)
					{
						Debug.Log("***** PERFECT BLOCK *****");
						EventManager.Instance.PostEvent("player/attack/success", character.gameObject);
						animator.SetTrigger(TriggerHash.attackBlockedPerfect);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackBlockedPerfect);
						GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.blue);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.GetSwordTappingIcon().Freeze();
						GamePlayer.Instance.TimeBlink(0.2f);
					}
					else
					{
						EventManager.Instance.PostEvent("player/attack/success", character.gameObject);
						animator.SetTrigger(TriggerHash.attackFailed);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackFailed);
					}
					ended = true;
					sti.Show(false);
					ScreenHud._pInstance._swordTapTelegraph.Hide();
				}
				else if (!sti.IsInGrace)
				{
					EventManager.Instance.PostEvent("player/attack/fail", character.gameObject);
					animator.SetTrigger(TriggerHash.attackSuccess);
					GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackSuccess);
					GamePlayer.Instance.CombatManager.CurrentAttacker.Character.ShowCombatEventType(CombatSpecialEventType.PrimaryAttack);
					ended = true;
					sti.Show(false);
					ScreenHud._pInstance._swordTapTelegraph.Hide();
				}
			}
			else if (sti.IsDone)
			{
				animator.SetTrigger(TriggerHash.attackSuccess);
				GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackSuccess);
				GamePlayer.Instance.CombatManager.CurrentAttacker.Character.ShowCombatEventType(CombatSpecialEventType.PrimaryAttack);
				ended = true;
				sti.Show(false);
				ScreenHud._pInstance._swordTapTelegraph.Hide();
				GamePlayer.Instance.SetTimeScale(1f, 0.2f);
			}
		}
		else if (!animator.IsInTransition(layerIndex))
		{
			Debug.LogWarning("No CurrentAttacker registered!");
		}
	}
}
