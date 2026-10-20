public class CombatPlayerElementalBoss : AIBehaviour
{
	private bool isLocated;

	public override void Initialize(AIController controller)
	{
		isLocated = false;
		if (GamePlayer.Instance.State != GamePlayerState.Combat)
		{
			GamePlayer.Instance.SetState(GamePlayerState.Combat);
		}
		controller.Character.Animator.SetTrigger(TriggerHash.combatPrepare);
	}

	public override void Update(AIController controller)
	{
		if (controller.agent.PlayerCharacter.IsInSpinjitzu())
		{
			ElementalBossEnemy elementalBossEnemy = (ElementalBossEnemy)controller.agent;
			if (elementalBossEnemy.Character.elementalPower == controller.agent.PlayerCharacter.elementalPower)
			{
				controller.agent.SetState(AIAgentState.Scared);
			}
		}
		else if (controller.agent.PlayerCharacter.IsDead())
		{
			controller.agent.SetState(AIAgentState.Idle);
		}
		else if (controller.Character.IsDead())
		{
			controller.agent.SetState(AIAgentState.Idle);
		}
		else if (!controller.Character.IsTrapped())
		{
			PathFinding.Node[] pathIncludeDoors = controller.Character.PathFinding.GetPathIncludeDoors(controller.Character.CurrentPositionNode, GamePlayer.Instance.Player.Character.CurrentPositionNode, controller.Character);
			if (pathIncludeDoors == null && controller.Character.CurrentPositionNode != GamePlayer.Instance.Player.Character.CurrentPositionNode)
			{
				controller.agent.SetState(AIAgentState.Idle);
				controller.Character.Animator.SetTrigger(TriggerHash.combatEnd);
			}
			else if (!controller.Character.Animator.GetBool(VarHash.isMoving) && !isLocated)
			{
				isLocated = true;
				controller.Character.transform.LookAt(controller.agent.PlayerCharacter.transform);
			}
		}
	}
}
