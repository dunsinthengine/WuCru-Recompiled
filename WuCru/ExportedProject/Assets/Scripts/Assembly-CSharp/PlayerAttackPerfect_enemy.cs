using UnityEngine;

public class PlayerAttackPerfect_enemy : StateMachineBehaviour
{
	private bool transition;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		transition = false;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime > stateInfo.length / 2f && !transition)
		{
			GamePlayer.Instance.CombatManager.CurrentAttacker.Character.TakeDamage(GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.PerfectAttack), GamePlayer.Instance.Player.Character.transform.position, DamageType.PerfectAttack);
			transition = true;
		}
	}
}
