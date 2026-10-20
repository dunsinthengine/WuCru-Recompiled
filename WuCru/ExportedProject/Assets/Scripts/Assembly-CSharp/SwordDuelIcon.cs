using UnityEngine;
using UnityEngine.UI;

public class SwordDuelIcon : MonoBehaviour
{
	public float status = 0.5f;

	public float hitRadius = 128f;

	public Image imageBG;

	public Image imagePointer;

	public Image imageGreenFill;

	public Image imageRedFill;

	private float extent = 260f;

	private Vector3 screenPos;

	private Vector3 offset = new Vector3(0f, -110f, 0f);

	private void Awake()
	{
		if (imageRedFill != null)
		{
			Vector3[] array = new Vector3[4];
			imageRedFill.rectTransform.GetWorldCorners(array);
			extent = (array[3].x - array[0].x) * 0.5f;
		}
		Show(false);
	}

	public void Show(bool visible)
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		bool flag = playerProfile.HasSeenBriefing(PlayerBriefingType.Duelling) && playerProfile.HasSeenBriefing(PlayerBriefingType.PlayerAttack);
		if (visible)
		{
			imageBG.gameObject.SetActive(true);
			imagePointer.gameObject.SetActive(true);
			imageGreenFill.gameObject.SetActive(true);
			imageRedFill.gameObject.SetActive(true);
			base.enabled = true;
		}
		else
		{
			imageBG.gameObject.SetActive(false);
			imagePointer.gameObject.SetActive(false);
			imageGreenFill.gameObject.SetActive(false);
			imageRedFill.gameObject.SetActive(false);
			base.enabled = false;
		}
	}

	private void PositionObjects()
	{
		screenPos = Camera.main.WorldToScreenPoint(GamePlayer.Instance.Player.Character.transform.position) + offset;
		imageBG.transform.position = screenPos;
		imagePointer.transform.position = screenPos;
		imageGreenFill.transform.position = screenPos;
		imageRedFill.transform.position = screenPos;
	}

	public bool IsPointInsideIcon(Vector2 point)
	{
		Vector2 b = Camera.main.WorldToScreenPoint(base.transform.position);
		float num = Vector2.Distance(point, b);
		if (num <= hitRadius)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		PositionObjects();
		imagePointer.transform.position = screenPos + new Vector3(0f - extent + GamePlayer.Instance.CombatManager.DuellingStatus * (extent * 2f), 0f, 0f);
		imageGreenFill.fillAmount = GamePlayer.Instance.CombatManager.DuellingStatus;
	}
}
