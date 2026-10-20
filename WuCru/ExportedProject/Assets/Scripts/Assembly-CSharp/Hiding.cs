using UnityEngine;

public class Hiding : StateMachineBehaviour
{
	private Character character;

	private bool allowInputHandling;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		allowInputHandling = false;
		animator.ResetTrigger(TriggerHash.move);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (character.isPlayer)
		{
			GamePlayer.Instance.MovementPointer.Hide();
			PlayerController playerController = character.Controller as PlayerController;
			if (!allowInputHandling && Input.GetMouseButtonDown(0))
			{
				allowInputHandling = true;
			}
			if (allowInputHandling)
			{
				playerController.HandlePicking();
			}
		}
	}
}
