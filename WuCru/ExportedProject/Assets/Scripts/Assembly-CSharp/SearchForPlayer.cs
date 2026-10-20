using System.Collections.Generic;
using UnityEngine;

public class SearchForPlayer : AIBehaviour
{
	public float startTime;

	public float coolDownDuration = 9f;

	public float targetReachedStartTime;

	public float targetReachedDuration = 1.2f;

	public float differentElevationStartTime;

	public float differentElevationTimeout = 4f;

	private bool seekingRandomTarget;

	private LevelNode randomTarget;

	private Vector3 searchDir = Vector3.zero;

	public override void Initialize(AIController controller)
	{
		if (controller.agent.PlayerCharacter != null && !controller.agent.PlayerCharacter.IsDead())
		{
			startTime = Time.time;
			randomTarget = null;
			searchDir = (controller.agent.PlayerCharacter.transform.position - controller.agent.Character.transform.position).normalized;
			controller.agent.Character.transform.LookAt(controller.agent.PlayerCharacter.transform);
		}
		else
		{
			Debug.LogWarning("controller.agent.PlayerCharacter seems to be null - is this causing state-flickering?");
			controller.agent.SetState(AIAgentState.Idle);
		}
		differentElevationStartTime = Time.time - differentElevationTimeout;
	}

	public override void Update(AIController controller)
	{
		if (controller.Character.IsTrapped())
		{
			return;
		}
		if (controller.agent.IsPlayerVisible)
		{
			controller.agent.SetState(AIAgentState.Alert);
		}
		else if (startTime + coolDownDuration < Time.time)
		{
			controller.agent.SetState(AIAgentState.Idle);
		}
		if (controller.agent.LastKnownPlayerLocation != null && controller.agent.PlayerCharacter != null && controller.Character.CurrentPathNode != null)
		{
			if (controller.Character.CurrentPathNode != controller.agent.LastKnownPlayerLocation && !seekingRandomTarget)
			{
				if (controller.Character.IsClimbing())
				{
					return;
				}
				LevelNode levelNode = null;
				if (controller.agent.PlayerCharacter.CurrentPositionNode == controller.agent.LastKnownPlayerLocation && controller.agent.PlayerCharacter.IsHiding())
				{
					PathFinding.Node[] array = null;
					if (controller.Character.CurrentPositionNode != null && controller.Character.CurrentPositionNode.Elevation >= 1)
					{
						array = controller.Character.PathFinding.GetPath(controller.Character.CurrentPositionNode, AIBeacon.Instance.GetLastBeaconCallLocation(controller.agent.AgentId), (PathFinding.Node n1, PathFinding.Node n2) => controller.agent.Character.PathFinding.DoesNeighbourHaveAccess(n1.levelNode, n2.levelNode, false));
					}
					levelNode = ((array == null || array.Length < 2) ? controller.agent.LastKnownPlayerLocation : array[array.Length - 2].levelNode);
				}
				else
				{
					levelNode = controller.agent.LastKnownPlayerLocation;
				}
				controller.Character.Controller.WalkTo(levelNode);
				searchDir = (controller.agent.PlayerCharacter.transform.position - controller.agent.Character.transform.position).normalized;
			}
			else if (controller.Character.IsIdle() && randomTarget == null)
			{
				targetReachedStartTime = Time.time;
				randomTarget = GetRandomTarget(controller);
				if (randomTarget != null)
				{
					seekingRandomTarget = true;
				}
			}
			else if (controller.Character.IsIdle() && randomTarget != null)
			{
				if (targetReachedStartTime + targetReachedDuration < Time.time)
				{
					controller.Character.Controller.WalkTo(randomTarget);
				}
				if (controller.Character.CurrentPositionNode == randomTarget)
				{
					seekingRandomTarget = false;
					randomTarget = null;
				}
			}
		}
		else
		{
			controller.agent.SetState(AIAgentState.Idle);
		}
	}

	private LevelNode GetRandomTarget(AIController controller)
	{
		LevelNode levelNode = null;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (searchDir.y > 0f)
		{
			flag = true;
		}
		if (searchDir.y < 0f)
		{
			flag2 = true;
		}
		if (searchDir.x > 0f)
		{
			flag3 = true;
		}
		if (searchDir.x < 0f)
		{
			flag4 = true;
		}
		LevelNode levelNode2 = ((levelNode == null) ? controller.agent.LastKnownPlayerLocation : levelNode);
		int num = levelNode2.Row - (flag2 ? 4 : 0);
		int num2 = levelNode2.Row + (flag ? 4 : 0);
		int num3 = levelNode2.Col - (flag4 ? 4 : 0);
		int num4 = levelNode2.Col + (flag3 ? 4 : 0);
		LevelNode[,] nodes = GamePlayer.Instance.Level.Nodes;
		List<LevelNode> list = new List<LevelNode>();
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(i, j), nodes) && nodes[i, j] != controller.Character.CurrentPositionNode && nodes[i, j] != levelNode && controller.agent.Character.DoesPathExist(nodes[i, j]) && nodes[i, j] != GamePlayer.Instance.Player.Character.CurrentPositionNode)
				{
					LevelNode levelNode3 = nodes[i, j];
					if (levelNode3 != controller.agent.PlayerCharacter.CurrentPositionNode && (levelNode3.Elevation == controller.Character.CurrentPositionNode.Elevation || (levelNode3.Elevation != controller.Character.CurrentPositionNode.Elevation && Time.time - differentElevationStartTime >= differentElevationTimeout)) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard_earth) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard_electricity) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard_energy) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard_fire) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard_ice) && !levelNode3.ContainsObjectType(LevelNodeObjectType.CrystalShard_water) && !levelNode3.ContainsObjectType(LevelNodeObjectType.EndFlag) && !levelNode3.ContainsObjectType(LevelNodeObjectType.GoldChest) && !levelNode3.ContainsObjectType(LevelNodeObjectType.MetalChest) && !levelNode3.ContainsObjectType(LevelNodeObjectType.WoodChest) && !levelNode3.ContainsObjectType(LevelNodeObjectType.BellTower))
					{
						list.Add(nodes[i, j]);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			levelNode = list[Random.Range(0, list.Count)];
		}
		if (levelNode != null && levelNode.Elevation != controller.Character.CurrentPositionNode.Elevation)
		{
			differentElevationStartTime = Time.time;
		}
		return levelNode;
	}
}
