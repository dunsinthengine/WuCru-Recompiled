using UnityEngine;

public class SpinjitzuAnimState : StateMachineBehaviour
{
	private Character character;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character.ResetPathAfterClimbing();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
