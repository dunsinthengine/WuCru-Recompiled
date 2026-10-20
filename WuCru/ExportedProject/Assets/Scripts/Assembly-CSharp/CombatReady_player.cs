using UnityEngine;

public class CombatReady_player : StateMachineBehaviour
{
	private Character character;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		character.CenterOnPositionTile();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
		{
			character.transform.LookAt(GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform);
		}
		if (GamePlayer.Instance.State != GamePlayerState.Combat)
		{
			animator.SetTrigger(TriggerHash.combatEnd);
		}
	}
}
