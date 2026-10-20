using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NinjaActionElementAttack : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private Image progressImage;

	[SerializeField]
	private Image bgImage;

	private ElementalAttackCharger elementalAttackCharger;

	private bool isDown;

	public void Initialize(float mana)
	{
		progressImage.fillMethod = Image.FillMethod.Vertical;
		progressImage.gameObject.SetActive(false);
		elementalAttackCharger = GamePlayer.Instance.Player.controller.ElementalCharger;
		HandleUpdate(mana, 0f);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (GamePlayer.Instance.Player.Character.IsIdle() || GamePlayer.Instance.Player.Character.IsRunning())
		{
			GamePlayer.Instance.Player.controller.ChargeElemetalAttack();
		}
	}

	public void onDestroy()
	{
	}

	public void HandleUpdate(float manaAmount, float chargeAmount)
	{
		bgImage.fillAmount = manaAmount;
	}
}
