using UnityEngine;

public class Running : StateMachineBehaviour
{
	private Character character;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = (animator.transform.parent != null) ? animator.transform.parent.GetComponent<Character>() : animator.GetComponentInParent<Character>();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (character != null && character.isPlayer)
		{
			GamePlayer.Instance.MovementPointer.Hide();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (character == null && animator != null)
		{
			character = (animator.transform.parent != null) ? animator.transform.parent.GetComponent<Character>() : animator.GetComponentInParent<Character>();
		}
		if (character != null)
		{
			animator.SetFloat(VarHash.movementSpeed, character.Speed);
		}
		if (character.isPlayer && character.IsRunning())
		{
			(character.Controller as PlayerController).HandlePicking();
		}
		if (character == null)
		{
			if (!animator.IsInTransition(0))
			{
				animator.SetBool(VarHash.isMoving, false);
			}
			animator.SetFloat(VarHash.movementSpeed, 0f);
		}
		else
		{
			if (character.IsClimbing() || animator.IsInTransition(layerIndex))
			{
				return;
			}
			if (character.isPlayer)
			{
				if (character.HasPath() && !ScreenResults._pInResults)
				{
					character.MoveStep();
				}
				else if (!Input.GetMouseButton(0) || ScreenResults._pInResults)
				{
					if (character.isPlayer)
					{
						GamePlayer.Instance.MovementPointer.Hide();
					}
					character.Animator.SetBool(VarHash.isMoving, false);
					animator.SetFloat(VarHash.movementSpeed, 0f);
					character.ResetPath();
					GamePlayer.Instance.SelectedObject = null;
				}
			}
			else if (character.HasPath() && !ScreenResults._pInResults)
			{
				character.MoveStep();
			}
			else
			{
				character.Animator.SetBool(VarHash.isMoving, false);
				animator.SetFloat(VarHash.movementSpeed, 0f);
				character.ResetPath();
			}
		}
	}
}
