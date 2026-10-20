using UnityEngine;

public class FriendlyFollowPlayer : AIBehaviour
{
	public static float stopDistToPlayer = 2.4f;

	public static float minDistToPlayer = 2.1f;

	private LevelNode fleeTargetNode;

	public override void Initialize(AIController controller)
	{
		LevelNode[,] nodes = GamePlayer.Instance.Level.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				LevelNode levelNode = nodes[i, j];
				foreach (LevelNodeObjectTypeAndTag objectType in levelNode.ObjectTypes)
				{
					if (objectType.objectType == LevelNodeObjectType.EndFlag)
					{
						fleeTargetNode = GamePlayer.Instance.Level.Nodes[levelNode.Row, levelNode.Col - 3];
						break;
					}
				}
			}
		}
		controller.agent.Character.Animator.SetTrigger(TriggerHash.wakeUp);
		if (GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			(GameController.Instance.currentState as GameControllerStatePlaying).CompletionMgr.ReportRescueFriend(controller.agent.Character.NinjaID);
		}
	}

	public override void Update(AIController controller)
	{
		if (controller.agent.Character.CurrentStateNameHash() == StateHash.sleep || controller.agent.Character.CurrentStateNameHash() == StateHash.wakingUp)
		{
			return;
		}
		if (GamePlayer.Instance.State != GamePlayerState.Combat)
		{
			float num = Vector3.Distance(controller.agent.Character.transform.position, GamePlayer.Instance.Player.Character.transform.position);
			LevelNode currentPositionNode = GamePlayer.Instance.Player.Character.CurrentPositionNode;
			if (num <= minDistToPlayer)
			{
				Vector3 normalized = (controller.Character.transform.position - GamePlayer.Instance.Player.Character.transform.position).normalized;
				if (Vector3.Dot(normalized, GamePlayer.Instance.Player.Character.transform.forward) > 0.25f)
				{
					LevelDirection direction = LevelDirection.North;
					float num2 = Vector3.Dot(GamePlayer.Instance.Player.Character.transform.forward, Vector3.forward);
					if (num2 <= -0.5f)
					{
						direction = LevelDirection.South;
					}
					else if (num2 < 0.5f)
					{
						Vector3 lhs = Vector3.Cross(GamePlayer.Instance.Player.Character.transform.forward, Vector3.forward);
						direction = ((!(Vector3.Dot(lhs, Vector3.up) > 0f)) ? LevelDirection.East : LevelDirection.West);
					}
					LevelNode neighbour = controller.Character.CurrentPositionNode.GetNeighbour(GameController.Instance.CurrentLevel.Nodes, direction);
					if (neighbour != null)
					{
						controller.Character.ResetPath();
						controller.Character.WalkTo(neighbour);
					}
				}
			}
			else if (num <= stopDistToPlayer)
			{
				if (controller.agent.Character.Animator.GetBool(VarHash.isMoving))
				{
					controller.agent.Character.Animator.SetBool(VarHash.isMoving, false);
					controller.Character.ResetPath();
				}
			}
			else if (currentPositionNode != null && (controller.agent.Character.CurrentDestination == null || controller.agent.Character.CurrentDestination != currentPositionNode))
			{
				controller.agent.Character.WalkTo(currentPositionNode);
			}
		}
		else if (controller.agent.Character.CurrentDestination == null || controller.agent.Character.CurrentDestination != fleeTargetNode)
		{
			controller.agent.Character.WalkTo(fleeTargetNode);
		}
	}
}
