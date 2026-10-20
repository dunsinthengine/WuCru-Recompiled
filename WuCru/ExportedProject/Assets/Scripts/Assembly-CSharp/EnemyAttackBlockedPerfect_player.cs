using UnityEngine;

public class EnemyAttackBlockedPerfect_player : StateMachineBehaviour
{
	private Character character;

	private SwordFXs swordFX;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		swordFX = character.GetComponentInChildren<SwordFXs>();
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.3f, 2.5f, Color.blue);
		swordFX.parryFX.Play();
		GamePlayer.Instance.TimeBlink(0.2f);
		GamePlayer.Instance.CombatManager.ReportResult(CombatResult.EnemyAttackBlocked);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamePlayer.Instance.CombatManager.CurrentAttacker.Character.GetSwordTappingIcon().Unfreeze();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
