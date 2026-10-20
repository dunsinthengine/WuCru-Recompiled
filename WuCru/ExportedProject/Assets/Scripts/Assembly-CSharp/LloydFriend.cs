using UnityEngine;

public class LloydFriend : AIAgent
{
	private void Start()
	{
		CharacterBuilder.ConfigureMinifig(controller.agent.Character, Team.GetID(MinifigType.Lloyd, 0));
	}

	public override void OnStateChange()
	{
		Debug.Log("Lloyd State change: " + base.State);
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
			TriggerDOTDUnlock();
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
		return AIAgentType.LloydFriend;
	}

	public override void OnAIBeaconCall()
	{
	}

	public override void OnPlayerSpotted(LevelNode location, bool visible)
	{
		base.OnPlayerSpotted(location, visible);
	}

	private void TriggerDOTDUnlock()
	{
		LevelExtensionController.LevelExtensionSettings levelExtensionSettings = GameController.Instance.LevelExtensionController.GetLevelExtensionSettings("DayOfTheDeparted");
		levelExtensionSettings.collected = levelExtensionSettings.total - 1;
		Debug.Log("OnPlayerSpotted");
		Debug.Log("<color=yellow>Setting Collected: " + levelExtensionSettings.collected + "</color>");
		LevelExtensionController.LevelExtensionSettings levelExtensionSettings2 = GameController.Instance.LevelExtensionController.GetLevelExtensionSettings("DayOfTheDeparted");
		GameController.Instance.LevelExtensionController.HandleEnemyDropPickedUp(GameController.Instance.Seed);
	}
}
