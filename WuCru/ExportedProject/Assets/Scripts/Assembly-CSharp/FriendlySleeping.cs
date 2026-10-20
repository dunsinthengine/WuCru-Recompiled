using UnityEngine;

public class FriendlySleeping : AIBehaviour
{
	private float wakeUpDist = 2f;

	public override void Initialize(AIController controller)
	{
	}

	public override void Update(AIController controller)
	{
		float num = Vector3.Distance(GamePlayer.Instance.Player.Character.transform.position, controller.agent.Character.transform.position);
		if (num <= wakeUpDist)
		{
			(GameController.Instance.currentState as GameControllerStatePlaying).OnIngameObjectiveReached();
			controller.agent.SetState(AIAgentState.Alert);
		}
	}
}
