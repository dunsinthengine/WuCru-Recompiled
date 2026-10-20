using UnityEngine;

public class EnemyAttackOption_enemy : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animator.GetFloat(VarHash.attackType) == 0f)
		{
			animator.SetFloat(VarHash.attackType, 1f);
		}
		else
		{
			animator.SetFloat(VarHash.attackType, 0f);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
