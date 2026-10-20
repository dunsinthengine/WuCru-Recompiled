using UnityEngine;

public class PlayerAttackSuccess_enemy : StateMachineBehaviour
{
	private bool transition;

	private int unlockCounterAttackLevel = 3;

	private const string hitGruntAudioEvent = "npc/hit/grunt";

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		transition = false;
		AudioPlayer.PlaySound("npc/hit/grunt", animator.transform.parent.gameObject);
		if (CanDoCounterAttack() && Random.value < GamePlayer.Instance.CombatManager.CurrentAttacker.combatCounterChance)
		{
			GamePlayer.Instance.CombatManager.CurrentAttacker.performCounter = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime > stateInfo.length / 2f && !transition)
		{
			GamePlayer.Instance.CombatManager.CurrentAttacker.Character.TakeDamage(GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.PrimaryAttack), GamePlayer.Instance.Player.Character.transform.position, DamageType.PrimaryAttack);
			transition = true;
		}
	}

	private bool CanDoCounterAttack()
	{
		if (GameController.Instance.PlayerProfile.Level >= unlockCounterAttackLevel)
		{
			return true;
		}
		return false;
	}
}
