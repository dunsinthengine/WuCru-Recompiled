using UnityEngine;

public class Duelling_enemy : StateMachineBehaviour
{
	private Character character;

	private static int duelCounter;

	private float lastPushTime;

	private float duration;

	private float startTime;

	private float timeGone;

	private bool forceWin;

	private int unlockSurpriseAttackLevel = 3;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!(GamePlayer.Instance.CombatManager.CurrentAttacker == null))
		{
			character = animator.transform.parent.GetComponent<Character>();
			duration = GamePlayer.Instance.CombatManager.CurrentAttacker.duellingInterval;
			duelCounter++;
			startTime = Time.realtimeSinceStartup;
			if (GamePlayer.Instance.CombatManager.CurrentAttacker.performCounter)
			{
				forceWin = true;
				GamePlayer.Instance.CombatManager.CurrentAttacker.Character.ShowCombatEventType(CombatSpecialEventType.CounterAttack);
			}
			else if (CanDoSurpriseAttack() && GamePlayer.Instance.CombatManager.CurrentAttacker.duellingAutoWinChance > Random.value)
			{
				forceWin = true;
				GamePlayer.Instance.CombatManager.CurrentAttacker.Character.ShowCombatEventType(CombatSpecialEventType.SurpriseAttack);
			}
			else
			{
				forceWin = false;
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (GamePlayer.Instance.CombatManager.CurrentAttacker == null)
		{
			return;
		}
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != (character.Controller as AIController).agent && !animator.IsInTransition(layerIndex))
		{
			animator.SetTrigger(TriggerHash.breakDuelling);
		}
		if (character.CurrentStateNameHash() != StateHash.duelling || !GamePlayer.Instance.Player.Character.IsInCombat() || !(GamePlayer.Instance.CombatManager.CurrentAttacker != null) || animator.IsInTransition(layerIndex) || GamePlayer.Instance.Player.Character.CurrentStateNameHash() != StateHash.duelling)
		{
			return;
		}
		if (lastPushTime + duration < Time.time)
		{
			float num = GamePlayer.Instance.CombatManager.CurrentAttacker.duellingStrength + Random.Range((0f - GamePlayer.Instance.CombatManager.CurrentAttacker.duellingStrength) * GamePlayer.Instance.CombatManager.CurrentAttacker.duellingVariance, GamePlayer.Instance.CombatManager.CurrentAttacker.duellingStrength * GamePlayer.Instance.CombatManager.CurrentAttacker.duellingVariance);
			timeGone = Time.realtimeSinceStartup - startTime;
			if ((timeGone > 5f || forceWin) && GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.Duelling))
			{
				GamePlayer.Instance.CombatManager.DuellingStatus--;
			}
			GamePlayer.Instance.CombatManager.DuellingStatus -= num;
			lastPushTime = Time.time;
			duration = GamePlayer.Instance.CombatManager.CurrentAttacker.duellingInterval + Random.Range((0f - GamePlayer.Instance.CombatManager.CurrentAttacker.duellingInterval) * 0.5f, GamePlayer.Instance.CombatManager.CurrentAttacker.duellingInterval * 0.5f);
		}
		animator.SetFloat(VarHash.duellingStatus, 1f - GamePlayer.Instance.CombatManager.DuellingStatus);
	}

	private bool CanDoSurpriseAttack()
	{
		if (GamePlayer.Instance.CombatManager.CurrentAttacker == null)
		{
			return false;
		}
		if (GamePlayer.Instance.CombatManager.CurrentAttacker is ElementalBossEnemy)
		{
			return false;
		}
		if (GameController.Instance.PlayerProfile.Level >= unlockSurpriseAttackLevel)
		{
			return true;
		}
		return false;
	}
}
