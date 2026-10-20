using UnityEngine;

public class BeaconCall : AIBehaviour
{
	public LevelNode target;

	private float startTime;

	private float reactionTime = 1.2f;

	private bool hasReacted;

	public override void Initialize(AIController controller)
	{
		startTime = Time.time;
		PathFinding.Node[] array = null;
		if (controller.Character.CurrentPositionNode != null && controller.Character.CurrentPositionNode.Elevation >= 1)
		{
			array = controller.Character.PathFinding.GetPath(controller.Character.CurrentPositionNode, AIBeacon.Instance.GetLastBeaconCallLocation(controller.agent.AgentId), (PathFinding.Node n1, PathFinding.Node n2) => controller.agent.Character.PathFinding.DoesNeighbourHaveAccess(n1.levelNode, n2.levelNode, false));
		}
		if (array != null && array.Length >= 2)
		{
			target = array[array.Length - 2].levelNode;
		}
		else
		{
			target = AIBeacon.Instance.GetLastBeaconCallLocation(controller.agent.AgentId);
		}
		controller.Character.ResetPath();
		controller.agent.Perception.SetLastKnownPlayerLocation(target);
		controller.Character.Animator.SetTrigger(TriggerHash.enemyAlert);
	}

	public override void Update(AIController controller)
	{
		if (controller.Character.IsTrapped())
		{
			return;
		}
		if (controller.agent.IsPlayerVisible && !controller.agent.PlayerCharacter.IsInSpinjitzu())
		{
			if (GamePlayer.Instance.State == GamePlayerState.Combat)
			{
				controller.agent.SetState(AIAgentState.Combat);
			}
			else
			{
				controller.agent.SetState(AIAgentState.Alert);
			}
		}
		else if (controller.agent.IsPlayerVisible && controller.agent.PlayerCharacter.IsInSpinjitzu())
		{
			controller.agent.SetState(AIAgentState.Scared);
		}
		else if (controller.agent.PlayerCharacter.IsDead())
		{
			controller.agent.SetState(AIAgentState.Idle);
		}
		else if (controller.agent.Character.IsIdle() && Time.time - startTime > reactionTime + 1f)
		{
			controller.agent.SetState(AIAgentState.Suspicious);
		}
		else if (!hasReacted && startTime + reactionTime <= Time.time)
		{
			hasReacted = true;
			controller.Character.Controller.WalkTo(target);
		}
	}
}
