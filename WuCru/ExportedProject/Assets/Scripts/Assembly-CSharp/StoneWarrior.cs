public class StoneWarrior : AIAgent
{
	private void Start()
	{
		CharacterBuilder.ConfigureMinifig(controller.agent.Character, 2000);
	}

	public override void OnStateChange()
	{
		switch (base.State)
		{
		case AIAgentState.Patrolling:
			controller.SetBehaviour(new FollowPath());
			break;
		case AIAgentState.SpottingPlayer:
			controller.SetBehaviour(new SoftSpottingPlayer());
			break;
		case AIAgentState.Suspicious:
			controller.SetBehaviour(new SearchForPlayer());
			break;
		case AIAgentState.Alert:
			controller.SetBehaviour(new HuntPlayer());
			break;
		case AIAgentState.Combat:
			controller.SetBehaviour(new CombatPlayer());
			break;
		case AIAgentState.BeaconCall:
			controller.SetBehaviour(new BeaconCall());
			break;
		case AIAgentState.Scared:
			controller.SetBehaviour(new FleePlayer());
			break;
		case AIAgentState.Dead:
			controller.SetBehaviour(new DeadState());
			break;
		}
	}

	public override AIAgentType GetAgentType()
	{
		return AIAgentType.StoneWarrior;
	}
}
