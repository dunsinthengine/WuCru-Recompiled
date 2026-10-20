public class MasterWuFriend : AIAgent
{
	private void Start()
	{
		CharacterBuilder.ConfigureMinifig(controller.agent.Character, 50);
	}

	public override void OnStateChange()
	{
		switch (base.State)
		{
		case AIAgentState.Idle:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.Patrolling:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.SpottingPlayer:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.Suspicious:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.Alert:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.Combat:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.BeaconCall:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.Scared:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		case AIAgentState.Dead:
			controller.SetBehaviour(new FriendlyIdle());
			break;
		}
	}

	public override AIAgentType GetAgentType()
	{
		return AIAgentType.MasterWuFriend;
	}

	public override void OnAIBeaconCall()
	{
	}

	public override void OnPlayerSpotted(LevelNode location, bool visible)
	{
		base.OnPlayerSpotted(location, visible);
	}
}
