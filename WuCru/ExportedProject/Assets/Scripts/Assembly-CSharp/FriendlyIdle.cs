using UnityEngine;

public class FriendlyIdle : AIBehaviour
{
	private float wakeUpDist = 0.5f;

	public override void Initialize(AIController controller)
	{
		controller.Character.animator.SetTrigger(TriggerHash.sleepToIdle);
		controller.Character.gameObject.transform.Rotate(0f, -90f, 0f);
	}

	public override void Update(AIController controller)
	{
		float num = Vector3.Distance(GamePlayer.Instance.Player.Character.transform.position, controller.agent.Character.transform.position);
		if (num <= wakeUpDist)
		{
			InGameCam pInstance = InGameCam._pInstance;
			pInstance.Zoom(6f, 2f, 1f);
			if (GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
			{
				(GameController.Instance.currentState as GameControllerStatePlaying).CompletionMgr.ReportRescueFriend(controller.agent.Character.NinjaID);
			}
		}
	}
}
