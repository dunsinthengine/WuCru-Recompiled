using UnityEngine;

public class EnemyAttackBlocked_player : StateMachineBehaviour
{
	private Character character;

	private SwordFXs swordFX;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		swordFX = character.GetComponentInChildren<SwordFXs>();
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.3f, 2.5f, Color.blue);
		swordFX.parryFX.Play();
		GamePlayer.Instance.CombatManager.ReportResult(CombatResult.EnemyAttackBlocked);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
