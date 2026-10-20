using UnityEngine;

public class ElementalAttackPerform : StateMachineBehaviour
{
	private Character character;

	private Character enemy;

	private PlayerController playerController;

	private bool hasFired;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		enemy = character.Controller.CurrentEnemy;
		playerController = character.Controller as PlayerController;
		hasFired = false;
		character.LookAt(playerController.ElementalAttackTarget);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!hasFired && animator.GetAnimatorTransitionInfo(0).normalizedTime > 0.6f)
		{
			hasFired = true;
			playerController.ElementalAttackFx.SetTarget(playerController.ElementalAttackTarget);
			playerController.ElementalAttackFx.SetState(ElementalAttack.ElementalAttackState.FLYING);
		}
	}
}
