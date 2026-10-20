using UnityEngine;

public class Climbing : StateMachineBehaviour
{
	private Character character;

	private bool hasReachedDestination;

	private bool hasReachedPoint;

	private bool goingUp;

	private float playerClimbSpeed = 4f;

	private Vector3 targetPosition;

	private Vector3 lookAtPosition;

	private LevelNode targetNode;

	private Ladder ladder;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		character.inputVector = Vector3.zero;
		if (character.isPlayer)
		{
			Debug.Log("Climbing enter");
		}
		goingUp = false;
		hasReachedDestination = false;
		hasReachedPoint = false;
		if (character.CurrentPathTarget == null)
		{
			LevelDirection levelDirectionForVector = LevelHelper.GetLevelDirectionForVector(character.transform.forward);
			if (character.CurrentPositionNode != null && GamePlayer.Instance != null && GamePlayer.Instance.Level != null)
			{
				targetNode = character.CurrentPositionNode.GetNeighbour(GamePlayer.Instance.Level.Nodes, levelDirectionForVector);
			}
		}
		else
		{
			targetNode = character.CurrentPathTarget.levelNode;
		}
		if (targetNode == null || character.CurrentPositionNode == null)
		{
			animator.ResetTrigger(TriggerHash.climb);
			animator.SetTrigger(TriggerHash.stopClimbing);
			animator.SetBool(VarHash.isMoving, true);
			return;
		}
		goingUp = character.CurrentPositionNode.Elevation <= targetNode.Elevation;
		ladder = ((!goingUp) ? targetNode.GetLadder() : character.CurrentPositionNode.GetLadder());
		if (ladder == null)
		{
			Debug.LogError("Did not find a ladder!!");
			animator.SetTrigger(TriggerHash.stopClimbing);
			animator.SetBool(VarHash.isMoving, true);
		}
		else
		{
			targetPosition = LevelHelper.NodeToVector3(targetNode);
			lookAtPosition = ((!goingUp) ? LevelHelper.NodeToVector3(character.CurrentPositionNode) : targetPosition);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		ladder = null;
		animator.ResetTrigger(TriggerHash.climb);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Input.GetMouseButtonUp(0))
		{
			GamePlayer.Instance.MovementPointer.Hide();
		}
		if (hasReachedDestination || animator.IsInTransition(0))
		{
			return;
		}
		Vector3 vector = targetPosition;
		if (!hasReachedPoint)
		{
			vector = ((!goingUp) ? new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z) : new Vector3(character.transform.position.x, targetPosition.y, character.transform.position.z));
		}
		Vector3 vector2 = vector - character.transform.position;
		float magnitude = vector2.magnitude;
		if (magnitude <= 0.1f)
		{
			if (!hasReachedPoint)
			{
				hasReachedPoint = true;
			}
			else
			{
				if (character.isPlayer)
				{
					GamePlayer.Instance.SelectedObject = null;
				}
				hasReachedDestination = true;
				character.ResetPathAfterClimbing();
				if (targetNode != null)
				{
					character.SetCurrentPathNode(targetNode.Row, targetNode.Col);
				}
				animator.ResetTrigger(TriggerHash.climb);
				animator.SetTrigger(TriggerHash.stopClimbing);
				animator.SetBool(VarHash.isMoving, true);
			}
			character.transform.position = vector;
		}
		if (!hasReachedDestination || !hasReachedPoint)
		{
			float num = character.Speed;
			if (!character.isPlayer)
			{
				num *= character.LadderSpeedRatio;
			}
			float num2 = num * Time.deltaTime;
			if (num2 > magnitude)
			{
				num2 = magnitude;
			}
			Vector3 normalized = vector2.normalized;
			character.transform.position = character.transform.position + normalized * num2;
			character.LookAt(lookAtPosition);
		}
	}
}
