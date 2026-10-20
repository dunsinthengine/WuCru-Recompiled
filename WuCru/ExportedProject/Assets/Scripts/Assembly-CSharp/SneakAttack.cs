using UnityEngine;

public class SneakAttack : StateMachineBehaviour
{
	private Character character;

	private Vector3 target;

	private Character enemy;

	private bool hasReachedDestination;

	private bool hasFired;

	private bool hasUnsheathed;

	private LevelNode currentTargetNode;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Sneak Attack ENTER");
		character = animator.transform.parent.GetComponent<Character>();
		(character.Controller as PlayerController).HasEnemySneakAttackTarget = true;
		enemy = character.Controller.CurrentEnemy;
		target = character.Controller.CurrentEnemy.transform.position;
		hasReachedDestination = false;
		hasFired = false;
		hasUnsheathed = false;
		character.ResetPath();
		character.WalkTo(enemy.CurrentPositionNode);
		currentTargetNode = enemy.CurrentPositionNode;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		(character.Controller as PlayerController).HasEnemySneakAttackTarget = false;
		animator.ResetTrigger(TriggerHash.sneakAttack);
		GamePlayer.Instance.SetTimeScale(1f, 0.3f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!hasReachedDestination)
		{
			if (character.Controller.CurrentEnemy.CurrentPositionNode.Elevation != character.CurrentPositionNode.Elevation)
			{
				animator.SetTrigger(TriggerHash.reset);
				hasReachedDestination = true;
				return;
			}
			target = character.Controller.CurrentEnemy.transform.position;
			Vector3 vector = target - character.transform.position;
			if (vector.magnitude < 0.7f)
			{
				hasReachedDestination = true;
				animator.SetBool(VarHash.isMoving, false);
			}
			else
			{
				if (currentTargetNode != enemy.CurrentPositionNode)
				{
					character.ResetPath();
					character.WalkTo(enemy.CurrentPositionNode);
					currentTargetNode = enemy.CurrentPositionNode;
				}
				character.MoveStep();
			}
			if (vector.magnitude < 1f && vector.magnitude > 0.5f && !hasUnsheathed)
			{
				character.EquipWeapon(true);
				if (character.GetWeapon().type == WeaponType.Nunchuck)
				{
					character.GetWeapon().GetComponentInChildren<Animation>().Play("anticipation");
				}
				GamePlayer.Instance.SetTimeScale(0.2f, 0.6f);
				hasUnsheathed = true;
			}
		}
		else if (!hasFired)
		{
			hasFired = true;
			animator.SetTrigger(TriggerHash.sneakAttackPerform);
		}
	}
}
