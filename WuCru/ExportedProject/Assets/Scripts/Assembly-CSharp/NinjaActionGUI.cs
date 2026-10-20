using UnityEngine;
using UnityEngine.UI;

public class NinjaActionGUI : MonoBehaviour
{
	[SerializeField]
	private Image imgSpinjitzu;

	[SerializeField]
	private Image imgSpinjitzuBg;

	[SerializeField]
	private Button btnSpinjitzu;

	[SerializeField]
	public Image spinjitzuProgress;

	[SerializeField]
	public Sprite[] spinjitzuElementalPowerIcons;

	[SerializeField]
	private NinjaActionElementAttack ninjaActionElementalAttack;

	[SerializeField]
	private Image imgElemental;

	[SerializeField]
	private Image imgElementalBg;

	[SerializeField]
	private Button btnElemental;

	[SerializeField]
	public Image elementalProgress;

	[SerializeField]
	public Sprite[] elementalPowerIcons;

	private void Awake()
	{
		btnSpinjitzu.onClick.AddListener(() =>
		{
			GamePlayer.Instance.Player.Character.StartSpinjitzu();
		});
		ninjaActionElementalAttack.gameObject.SetActive(false);
	}

	public void ApplyElementImages(ElementalPower power)
	{
		imgSpinjitzu.sprite = spinjitzuElementalPowerIcons[(int)(power - 1)];
		NinjaPortraitData component = GameController.Instance.GetComponent<NinjaPortraitData>();
		imgSpinjitzuBg.color = component.powerColors[(int)power];
		spinjitzuProgress.sprite = spinjitzuElementalPowerIcons[(int)(power - 1)];
		imgElemental.sprite = elementalPowerIcons[(int)(power - 1)];
		imgElementalBg.color = component.powerColors[(int)power];
		elementalProgress.sprite = elementalPowerIcons[(int)(power - 1)];
	}

	public void SetCurrentMana(float mana)
	{
		ninjaActionElementalAttack.Initialize(mana);
	}
}
