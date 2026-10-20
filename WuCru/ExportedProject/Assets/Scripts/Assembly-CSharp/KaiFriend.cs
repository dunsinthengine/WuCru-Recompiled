public class KaiFriend : AIAgent
{
	private void Start()
	{
		CharacterBuilder.ConfigureMinifig(controller.agent.Character, Team.GetID(MinifigType.Kai, 0));
	}

	public override void OnStateChange()
	{
		switch (base.State)
		{
		case AIAgentState.Idle:
			controller.SetBehaviour(new FriendlySleeping());
			break;
		case AIAgentState.Patrolling:
			controller.SetBehaviour(new FriendlySleeping());
			break;
		case AIAgentState.SpottingPlayer:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		case AIAgentState.Suspicious:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		case AIAgentState.Alert:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		case AIAgentState.Combat:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		case AIAgentState.BeaconCall:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		case AIAgentState.Scared:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		case AIAgentState.Dead:
			controller.SetBehaviour(new FriendlyFollowPlayer());
			break;
		}
	}

	public override AIAgentType GetAgentType()
	{
		return AIAgentType.KaiFriend;
	}

	public override void OnAIBeaconCall()
	{
	}

	public override void OnPlayerSpotted(LevelNode location, bool visible)
	{
		base.OnPlayerSpotted(location, visible);
	}
}
