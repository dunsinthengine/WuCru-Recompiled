using Fabric;
using UnityEngine;

public class SoftSpottingPlayer : AIBehaviour
{
	private const string enemyAlertedAudioEvent = "npc/enemy/alerted";

	private static float lastAlertedSound;

	private const float alertSoundCooldown = 8f;

	public override void Initialize(AIController controller)
	{
		HUDItemTracker hUDItemTracker = ScreenHud._pInstance._enemyIconTracker;
		if (GameController.Instance.Seed >= 5000)
		{
			hUDItemTracker = ScreenHud._pInstance._enemyIconTrackerCity;
		}
		hUDItemTracker.DisplayIcon(controller.Character.gameObject.transform, false, 0.1f, 0.1f);
		ScreenHud._pInstance._enemyIconSeenTracker.DisplayIcon(controller.Character.gameObject.transform, false, 0.1f, 0.1f);
		ScreenHud._pInstance._enemyIconSuspiciousTracker.DismissIcon(controller.Character.gameObject.transform);
	}

	public override void Exit(AIController controller)
	{
		if (controller.agent.SpottetPlayerMeter >= 1f && Time.time > lastAlertedSound + 8f)
		{
			AudioPlayer.PlaySound("npc/enemy/alerted");
			lastAlertedSound = Time.time;
		}
		EventManager.Instance.PostEvent("fx/slow/spotting", EventAction.StopSound, controller.agent.Character.gameObject);
	}

	public override void Update(AIController controller)
	{
		if (controller.agent.SpottetPlayerMeter <= 0f)
		{
			controller.agent.SetState(AIAgentState.Patrolling);
		}
	}
}
