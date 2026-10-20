using UnityEngine;

public class HuntPlayer : AIBehaviour
{
	public static float combatDist = 1.7f;

	private float lastWalkToTime;

	private float walkToInterval = 0.5f;

	public override void Initialize(AIController controller)
	{
		AIBeacon.Instance.ReportEvent(controller.agent, controller.agent.PlayerCharacter.CurrentPositionNode, controller.agent.Character.transform.position, 5f, 0.5f);
	}

	public override void Update(AIController controller)
	{
		if (controller.agent.PlayerCharacter.IsDead())
		{
			controller.agent.SetState(AIAgentState.Idle);
		}
		else
		{
			if (controller.Character.IsDead())
			{
				return;
			}
			if (controller.agent.PlayerCharacter.IsInSpinjitzu())
			{
				controller.agent.SetState(AIAgentState.Scared);
			}
			else
			{
				if (controller.Character.Animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy alerted") || controller.Character.IsTrapped())
				{
					return;
				}
				if (controller.agent.LastKnownPlayerLocation != null && !controller.Character.IsClimbing() && lastWalkToTime + walkToInterval < Time.time)
				{
					lastWalkToTime = Time.time;
					if (GamePlayer.Instance.Level.Nodes[controller.agent.LastKnownPlayerLocation.Row, controller.agent.LastKnownPlayerLocation.Col].ObjectTypes.Count > 0)
					{
						PathFinding.Node[] path = controller.agent.Character.PathFinding.GetPath(controller.agent.Character.CurrentPositionNode, controller.agent.LastKnownPlayerLocation, (PathFinding.Node n1, PathFinding.Node n2) => controller.agent.Character.PathFinding.DoesNeighbourHaveAccess(n1.levelNode, n2.levelNode, false));
						if (path != null && path.Length >= 2)
						{
							controller.agent.Perception.SetLastKnownPlayerLocation(path[path.Length - 2].levelNode);
						}
					}
					controller.Character.Controller.WalkTo(controller.agent.LastKnownPlayerLocation);
				}
				if (!controller.agent.IsPlayerVisible && GamePlayer.Instance.State != GamePlayerState.Combat)
				{
					controller.agent.SetState(AIAgentState.Suspicious);
				}
				else
				{
					if (!(controller.agent.PlayerCharacter != null) || controller.agent.Character.Animator.IsInTransition(0) || controller.agent.PlayerCharacter.animator.IsInTransition(0) || controller.agent.Character.CurrentPositionNode.Elevation != GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation || (!(Vector3.Distance(controller.agent.Character.transform.position, controller.agent.PlayerCharacter.transform.position) < combatDist) && GamePlayer.Instance.State != GamePlayerState.Combat) || controller.agent.PlayerCharacter.IsClimbing() || controller.agent.Character.CurrentPositionNode.Elevation != controller.agent.PlayerCharacter.CurrentPositionNode.Elevation || controller.agent.PlayerCharacter.IsSneakAttacking())
					{
						return;
					}
					PathFinding.Node[] pathIncludeDoors = controller.Character.PathFinding.GetPathIncludeDoors((controller.Character.CurrentPathNode != null) ? controller.Character.CurrentPathNode : controller.Character.CurrentPositionNode, GamePlayer.Instance.Player.Character.CurrentPositionNode, controller.Character);
					bool flag = true;
					if (pathIncludeDoors != null)
					{
						PathFinding.Node[] array = pathIncludeDoors;
						foreach (PathFinding.Node node in array)
						{
							if (node.levelNode.Elevation != GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation)
							{
								flag = false;
								break;
							}
						}
					}
					if (pathIncludeDoors != null && flag && !controller.Character.IsClimbing() && GamePlayer.Instance.CombatManager.CanGetCombatSlots())
					{
						controller.agent.SetState(AIAgentState.Combat);
					}
				}
			}
		}
	}
}
