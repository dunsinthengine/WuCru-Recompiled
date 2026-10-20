using System.Collections.Generic;
using UnityEngine;

public class FleePlayer : AIBehaviour
{
	private LevelNode target;

	public override void Initialize(AIController controller)
	{
		controller.Character.Speed = controller.agent.FleeSpeed;
		controller.Character.Animator.SetTrigger(TriggerHash.enemyScare);
		target = GetFleeTarget(controller);
		if (target != null)
		{
			controller.Character.WalkTo(target);
		}
	}

	public override void Update(AIController controller)
	{
		if (!controller.agent.PlayerCharacter.IsInSpinjitzu())
		{
			controller.agent.SetState(AIAgentState.Suspicious);
			return;
		}
		target = GetFleeTarget(controller);
		if (target != null)
		{
			controller.Character.WalkTo(target);
		}
	}

	public override void Exit(AIController controller)
	{
	}

	private LevelNode GetFleeTarget(AIController controller)
	{
		LevelNode levelNode = FindNearestLadderAwayFromPlayer(controller);
		if (levelNode != null)
		{
			return levelNode;
		}
		levelNode = FindPlateauNodeAwayFromPlayer(controller);
		if (levelNode != null)
		{
			return levelNode;
		}
		return null;
	}

	private LevelNode FindPlateauNodeAwayFromPlayer(AIController controller)
	{
		LevelNode result = null;
		float num = 0f;
		Vector3 normalized = (LevelHelper.NodeToVector3(controller.agent.Character.CurrentPositionNode) - LevelHelper.NodeToVector3(controller.agent.PlayerCharacter.CurrentPositionNode)).normalized;
		LevelPlateau levelPlateau = controller.agent.Character.CurrentPositionNode.levelPlateau;
		if (levelPlateau == null)
		{
			return null;
		}
		foreach (LevelNode node in levelPlateau.nodes)
		{
			Vector3 normalized2 = (LevelHelper.NodeToVector3(node) - LevelHelper.NodeToVector3(controller.agent.Character.CurrentPositionNode)).normalized;
			float num2 = Vector3.Dot(normalized, normalized2);
			if (num2 >= 0f)
			{
				float num3 = LevelHelper.Distance(controller.agent.PlayerCharacter.CurrentPositionNode, node);
				if (num3 > num)
				{
					num = num3;
					result = node;
				}
			}
		}
		return result;
	}

	private LevelNode FindNearestLadderAwayFromPlayer(AIController controller)
	{
		List<LevelNode> list = new List<LevelNode>();
		LevelPlateau levelPlateau = controller.agent.Character.CurrentPositionNode.levelPlateau;
		if (levelPlateau != null)
		{
			foreach (LevelNode node in levelPlateau.nodes)
			{
				if (node.HasLadderAnySizeAnyDirection())
				{
					if (!list.Contains(node))
					{
						list.Add(node);
					}
				}
				else if (node.HasNeighbourLadderAnySize(GamePlayer.Instance.Level.Nodes))
				{
					LevelNode neighbourLadderNode = node.GetNeighbourLadderNode(GamePlayer.Instance.Level.Nodes, LevelDirection.North);
					if (neighbourLadderNode != null && !list.Contains(neighbourLadderNode))
					{
						list.Add(node);
					}
					neighbourLadderNode = node.GetNeighbourLadderNode(GamePlayer.Instance.Level.Nodes, LevelDirection.South);
					if (neighbourLadderNode != null && !list.Contains(neighbourLadderNode))
					{
						list.Add(node);
					}
					neighbourLadderNode = node.GetNeighbourLadderNode(GamePlayer.Instance.Level.Nodes, LevelDirection.East);
					if (neighbourLadderNode != null && !list.Contains(neighbourLadderNode))
					{
						list.Add(node);
					}
					neighbourLadderNode = node.GetNeighbourLadderNode(GamePlayer.Instance.Level.Nodes, LevelDirection.West);
					if (neighbourLadderNode != null && !list.Contains(neighbourLadderNode))
					{
						list.Add(node);
					}
				}
			}
		}
		Vector3 normalized = (LevelHelper.NodeToVector3(controller.agent.Character.CurrentPositionNode) - LevelHelper.NodeToVector3(controller.agent.PlayerCharacter.CurrentPositionNode)).normalized;
		foreach (LevelNode item in list)
		{
			Vector3 normalized2 = (LevelHelper.NodeToVector3(item) - LevelHelper.NodeToVector3(controller.agent.Character.CurrentPositionNode)).normalized;
			float num = Vector3.Dot(normalized, normalized2);
			if (num >= 0f)
			{
				if (item.levelPlateau != levelPlateau)
				{
					return item.levelPlateau.nodes[Random.Range(0, item.levelPlateau.nodes.Count)];
				}
				LevelNode levelNode = null;
				if (item.HasLadderAnySize(LevelDirection.North))
				{
					levelNode = GamePlayer.Instance.Level.Nodes[item.Row + 1, item.Col];
				}
				else if (item.HasLadderAnySize(LevelDirection.South))
				{
					levelNode = GamePlayer.Instance.Level.Nodes[item.Row - 1, item.Col];
				}
				else if (item.HasLadderAnySize(LevelDirection.East))
				{
					levelNode = GamePlayer.Instance.Level.Nodes[item.Row, item.Col + 1];
				}
				else if (item.HasLadderAnySize(LevelDirection.West))
				{
					levelNode = GamePlayer.Instance.Level.Nodes[item.Row, item.Col - 1];
				}
				if (levelNode != null)
				{
					return levelNode.levelPlateau.nodes[Random.Range(0, levelNode.levelPlateau.nodes.Count)];
				}
			}
		}
		return null;
	}
}
