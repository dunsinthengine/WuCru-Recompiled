using UnityEngine;

public class DeadStateElementalBoss : AIBehaviour
{
	public override void Initialize(AIController controller)
	{
		controller.Character.Visible(false);
		controller.Die();
		HUDItemTracker hUDItemTracker = ScreenHud._pInstance._enemyIconTracker;
		if (GameController.Instance.Seed >= 5000)
		{
			hUDItemTracker = ScreenHud._pInstance._enemyIconTrackerCity;
		}
		hUDItemTracker.DismissIcon(controller.Character.gameObject.transform);
		ScreenHud._pInstance._enemyIconSeenTracker.DismissIcon(controller.Character.gameObject.transform);
		ScreenHud._pInstance._enemyIconSuspiciousTracker.DismissIcon(controller.Character.gameObject.transform);
		ECOLLECTABLE_CARD eCOLLECTABLE_CARD = ECOLLECTABLE_CARD.INVALID;
		eCOLLECTABLE_CARD = (ECOLLECTABLE_CARD)Random.Range(1, 13);
		ScreenResultsCard._acquiredCard = eCOLLECTABLE_CARD;
	}
}
