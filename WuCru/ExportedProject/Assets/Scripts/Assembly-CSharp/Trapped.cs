using UnityEngine;

public class Trapped : StateMachineBehaviour
{
	private Character character;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		if (Time.timeScale < 1f && !ScreenResults._pInResults)
		{
			GamePlayer.Instance.SetTimeScale(1f, 0.2f);
		}
		GamePlayer.Instance.SelectedObject = null;
		if (animator.GetBool(VarHash.isMoving) && character.Velocity.magnitude == 0f)
		{
			animator.SetBool(VarHash.isMoving, false);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!animator.IsInTransition(layerIndex))
		{
			animator.SetTrigger(TriggerHash.reset);
		}
	}
}
