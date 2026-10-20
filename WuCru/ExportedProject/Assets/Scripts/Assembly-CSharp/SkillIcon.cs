using UnityEngine;

public class SkillIcon : MonoBehaviour
{
	public enum SKILLTYPE
	{
		WEAPON = 0,
		SPINJITZU = 1,
		STAMINA = 2,
		AGILITY = 3,
		STRENGTH = 4
	}

	public UISprite _icon;

	public UISprite _costIcon;

	public UISprite _costIconBrush;

	public UILabel _costLabel;

	public UISprite _padlock;

	public UISprite _tick;

	public UITexture _sheen;

	public int _buttonLevel;

	public SKILLTYPE _skillType;

	private bool _isPurchasable;

	private bool _hasPurchased;

	private bool _canAfford;

	private bool _purchaseableLevel;

	public bool _pIsPurchasable
	{
		get
		{
			return _isPurchasable;
		}
	}

	public bool _pCanAfford
	{
		get
		{
			return _canAfford;
		}
	}

	public bool _pPurchaseableLevel
	{
		get
		{
			return _purchaseableLevel;
		}
	}

	public bool _pHasPurchased
	{
		get
		{
			return _hasPurchased;
		}
	}

	public void ConfigureButton()
	{
		int num = 0;
		switch (_skillType)
		{
		case SKILLTYPE.WEAPON:
			num = GameController.Instance.PlayerProfile.WeaponLevel;
			break;
		case SKILLTYPE.SPINJITZU:
			num = GameController.Instance.PlayerProfile.SpinjitzuLevel;
			break;
		case SKILLTYPE.STAMINA:
			num = GameController.Instance.PlayerProfile.StaminaLevel;
			break;
		case SKILLTYPE.AGILITY:
			num = GameController.Instance.PlayerProfile.AgilityLevel;
			break;
		case SKILLTYPE.STRENGTH:
			num = GameController.Instance.PlayerProfile.StrengthLevel;
			break;
		}
		if (_buttonLevel <= num)
		{
			_padlock.enabled = false;
			_isPurchasable = false;
			_hasPurchased = true;
			_icon.color = Color.grey;
			_icon.alpha = 1f;
			_tick.enabled = true;
			_costIcon.enabled = false;
			_costIconBrush.enabled = false;
			_costLabel.enabled = false;
			_sheen.enabled = false;
			return;
		}
		_hasPurchased = false;
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int cost = GetCost();
		_canAfford = cost <= playerProfile.AvailableSkillPoints;
		_purchaseableLevel = _buttonLevel == num + 1;
		if (_purchaseableLevel && _canAfford)
		{
			_padlock.enabled = false;
			_tick.enabled = false;
			_isPurchasable = true;
			_icon.color = Color.white;
			_icon.alpha = 1f;
			_costLabel.enabled = true;
			_costIcon.enabled = true;
			_costIconBrush.enabled = true;
			_costLabel.text = cost.ToString();
			_sheen.enabled = true;
		}
		else
		{
			_tick.enabled = false;
			_padlock.enabled = !_purchaseableLevel;
			_icon.color = Color.white;
			_icon.alpha = 0.25f;
			_costIcon.enabled = _purchaseableLevel;
			_costIconBrush.enabled = _purchaseableLevel;
			_costLabel.enabled = _purchaseableLevel;
			_costLabel.text = cost.ToString();
			_isPurchasable = false;
			_sheen.enabled = false;
		}
	}

	public int GetCost()
	{
		return _buttonLevel;
	}
}
