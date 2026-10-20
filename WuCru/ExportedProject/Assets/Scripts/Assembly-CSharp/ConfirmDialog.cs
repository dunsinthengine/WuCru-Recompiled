using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : MonoBehaviour
{
	public Button acceptButton;

	public Button cancelButton;

	public Image skillImage;

	public Image icon1;

	public Image icon2;

	public Image icon3;

	public Text lvlText;

	public Image coinsIcon;

	public SkillType activeSkillType;

	public int iSkillLevel;

	public Localize skillText;

	public Localize icon1text;

	public Localize icon2text;

	public Localize icon3text;

	public Localize header1text;

	public Localize header2text;

	public Localize header3text;

	private void Awake()
	{
	}

	public void Show()
	{
		acceptButton.onClick.RemoveAllListeners();
		base.gameObject.SetActive(true);
		UpdateDialogBox();
	}

	public void Hide()
	{
		AudioPlayer.PlaySound("ui/button/back/click");
		base.gameObject.SetActive(false);
	}

	public void UpdateDialogBox()
	{
		int num = 22;
		icon1.enabled = true;
		icon2.enabled = true;
		icon3.enabled = true;
		skillText.SetValue(" ");
		header1text.SetValue(" ");
		header2text.SetValue(" ");
		header3text.SetValue(" ");
		icon1text.SetValue(" ");
		icon2text.SetValue(" ");
		icon3text.SetValue(" ");
		if (activeSkillType == SkillType.Spinjitzu)
		{
			skillText.SetKey("ABILITY_SPINJITZU");
			skillImage.sprite = Resources.Load<Sprite>(Paths.skillImageSpinjitzu);
			icon1.sprite = Resources.Load<Sprite>(Paths.skillSpriteSpinjitzu);
			icon3.sprite = Resources.Load<Sprite>(Paths.skillSpriteTime);
			icon2.enabled = false;
			header1text.SetValue(string.Empty);
			header3text.SetKey("ABILITY_DURATION");
			header2text.SetValue(string.Empty);
			if (iSkillLevel == 1)
			{
				icon1text.SetValue("1");
				icon3text.SetValue("8s");
			}
			else if (iSkillLevel == 2)
			{
				icon1text.SetValue("2");
				icon3text.SetValue("10s");
			}
			else if (iSkillLevel == 3)
			{
				icon1text.SetValue("3");
				icon3text.SetValue("12s");
			}
			else if (iSkillLevel == 4)
			{
				icon1text.SetValue("3");
				icon3text.SetValue("14S");
			}
		}
		else if (activeSkillType == SkillType.Weapon)
		{
			skillImage.sprite = Resources.Load<Sprite>(Paths.skillImageWeapon);
			skillText.SetKey("ABILITY_WEAPON");
			header1text.SetKey("ABILITY_DAMAGE");
			header3text.SetKey("COMBAT_PERFECT");
			header2text.SetValue(string.Empty);
			icon2.enabled = false;
			icon1.sprite = Resources.Load<Sprite>(Paths.skillSpriteDamage);
			icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectAttack);
			if (iSkillLevel == 1)
			{
				icon1text.SetValue("+25%");
				icon3text.SetKey("ABILITY_ATTACK");
				icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectAttack);
			}
			else if (iSkillLevel == 2)
			{
				icon1text.SetValue("+50%");
				icon3text.SetKey("ABILITY_BLOCK");
				icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectBlock);
			}
			else if (iSkillLevel == 3)
			{
				icon1text.SetValue("+75%");
				icon3text.SetKey("DAMTYPE_BACKSTAB");
				icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectSneak);
			}
			else if (iSkillLevel == 4)
			{
				icon1text.SetValue("+100%");
				header3text.SetKey("ABILITY_AUTOMATIC");
				icon3text.SetKey("ABILITY_SPINJITZU");
				icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectSpin);
			}
		}
		else if (activeSkillType == SkillType.Strength)
		{
			skillText.SetKey("ABILITY_STRENGTH");
			skillImage.sprite = Resources.Load<Sprite>(Paths.skillImageStrength);
			header1text.SetKey("ABILITY_DAMAGE");
			header2text.SetValue(string.Empty);
			header3text.SetKey("ABILITY_CRITICAL");
			icon2.enabled = false;
			icon1text.SetValue("+20");
			icon3text.SetValue("10%");
			icon2text.SetValue(string.Empty);
			icon1.sprite = Resources.Load<Sprite>(Paths.skillSpriteDamage);
			icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectAttack);
			if (iSkillLevel == 1)
			{
				icon1text.SetValue("+25%");
				icon3text.SetValue("5%");
			}
			else if (iSkillLevel == 2)
			{
				icon1text.SetValue("+50%");
				icon3text.SetValue("10%");
			}
			else if (iSkillLevel == 3)
			{
				icon1text.SetValue("+75%");
				icon3text.SetValue("15%");
			}
			else if (iSkillLevel == 4)
			{
				icon1text.SetValue("+100%");
				icon3text.SetValue("20%");
			}
		}
		else if (activeSkillType == SkillType.Speed)
		{
			skillText.SetKey("ABILITY_SPEED");
			skillImage.sprite = Resources.Load<Sprite>(Paths.skillImageSpeed);
			header1text.SetKey("ABILITY_SPEED");
			header2text.SetValue(string.Empty);
			header3text.SetKey("ABILITY_DODGE");
			icon2.enabled = false;
			icon2text.SetValue(string.Empty);
			icon1text.SetValue("+10");
			icon3text.SetValue("5%");
			icon1.sprite = Resources.Load<Sprite>(Paths.skillSpriteSpeed);
			icon3.sprite = Resources.Load<Sprite>(Paths.skillSpritePerfectAttack);
			if (iSkillLevel == 1)
			{
				icon1text.SetValue("+10%");
				icon3text.SetValue("5%");
			}
			else if (iSkillLevel == 2)
			{
				icon1text.SetValue("+20%");
				icon3text.SetValue("10%");
			}
			else if (iSkillLevel == 3)
			{
				icon1text.SetValue("+30%");
				icon3text.SetValue("15%");
			}
			else if (iSkillLevel == 4)
			{
				icon1text.SetValue("+40%");
				icon3text.SetValue("20%");
			}
		}
		else if (activeSkillType == SkillType.Health)
		{
			skillText.SetKey("ABILITY_HEALTH");
			skillImage.sprite = Resources.Load<Sprite>(Paths.skillImageHealth);
			header1text.SetKey("ABILITY_HEALTH");
			header2text.SetValue(string.Empty);
			header3text.SetKey("ABILITY_CHANCE");
			icon2.enabled = false;
			icon2text.SetValue(string.Empty);
			icon1text.SetValue("+20");
			icon3text.SetValue("10%");
			icon1.sprite = Resources.Load<Sprite>(Paths.skillSpriteHeart);
			icon3.sprite = Resources.Load<Sprite>(Paths.skillSpriteChestHeart);
			if (iSkillLevel == 1)
			{
				icon1text.SetValue("+10");
				icon3text.SetValue("5%");
			}
			else if (iSkillLevel == 2)
			{
				icon1text.SetValue("+20");
				icon3text.SetValue("10%");
			}
			else if (iSkillLevel == 3)
			{
				icon1text.SetValue("+30");
				icon3text.SetValue("15%");
			}
			else if (iSkillLevel == 4)
			{
				icon1text.SetValue("+40");
				icon3text.SetValue("20%");
			}
		}
		lvlText.text = iSkillLevel.ToString();
		coinsIcon.sprite = Resources.Load<Sprite>(Paths.skillSpriteCoinsBase + iSkillLevel);
	}
}
