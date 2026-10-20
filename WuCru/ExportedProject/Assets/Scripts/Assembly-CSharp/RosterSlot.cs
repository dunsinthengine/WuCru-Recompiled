using System.Text;
using UnityEngine;

public class RosterSlot : MonoBehaviour
{
	public UISprite _weaponIcon;

	public UISprite _elementIcon;

	public UISprite _crystalIcon;

	public UISprite[] _lockedSprites;

	public UISprite _addIcon;

	public UILabel _addMessage;

	public UILabel _addMessageShadow;

	public UISprite _statsPanel;

	public UILabel _lblCrystalCount;

	public UILabel _lblName;

	public UILabel _lblNameShadow;

	public UILabel _lblLevel;

	public UILabel _lblLockedTitle;

	public UILabel _lblLockedMessage;

	public UITexture _blobShadowName;

	private bool _locked;

	public bool _pLocked
	{
		get
		{
			return _locked;
		}
	}

	public void ConfigureForAdd()
	{
		_addIcon.enabled = true;
		_addMessage.enabled = true;
		_addMessageShadow.enabled = true;
		_weaponIcon.enabled = false;
		_elementIcon.enabled = false;
		_lblCrystalCount.enabled = false;
		_lblName.enabled = false;
		_lblNameShadow.enabled = false;
		_blobShadowName.enabled = false;
		_lblLockedTitle.enabled = false;
		_lblLockedMessage.enabled = false;
		_lblLevel.enabled = false;
		_crystalIcon.enabled = false;
		_statsPanel.enabled = false;
		int num = _lockedSprites.Length;
		for (int i = 0; i < num; i++)
		{
			_lockedSprites[i].enabled = false;
		}
		_locked = false;
	}

	public void ConfigureForLocked(string reason)
	{
		_addIcon.enabled = false;
		_addMessage.enabled = false;
		_addMessageShadow.enabled = false;
		_weaponIcon.enabled = false;
		_elementIcon.enabled = false;
		_lblCrystalCount.enabled = false;
		_lblName.enabled = false;
		_lblNameShadow.enabled = false;
		_blobShadowName.enabled = false;
		_lblLockedTitle.enabled = true;
		_lblLockedMessage.enabled = true;
		_lblLevel.enabled = false;
		_crystalIcon.enabled = false;
		_statsPanel.enabled = false;
		int num = _lockedSprites.Length;
		for (int i = 0; i < num; i++)
		{
			_lockedSprites[i].enabled = true;
		}
		_locked = true;
		_lblLockedMessage.text = Localise(reason);
	}

	public void ConfigureForCharacter(MinifigType minifigType)
	{
		_addIcon.enabled = false;
		_addMessage.enabled = false;
		_addMessageShadow.enabled = false;
		_weaponIcon.enabled = true;
		_elementIcon.enabled = true;
		_lblCrystalCount.enabled = true;
		_lblName.enabled = true;
		_lblNameShadow.enabled = false;
		_blobShadowName.enabled = true;
		_lblLockedTitle.enabled = false;
		_lblLockedMessage.enabled = false;
		_lblLevel.enabled = true;
		_crystalIcon.enabled = true;
		_statsPanel.enabled = true;
		_lblName.color = GlobalStyle._pInstance.GetNameTextMainColor(minifigType);
		int num = _lockedSprites.Length;
		for (int i = 0; i < num; i++)
		{
			_lockedSprites[i].enabled = false;
		}
		switch (minifigType)
		{
		case MinifigType.Cole:
			_lblName.text = Localise("CharacterName.Cole");
			_crystalIcon.spriteName = "Crystal_Cole";
			_elementIcon.spriteName = "Element_Cole";
			_weaponIcon.spriteName = "Weapon_Cole";
			break;
		case MinifigType.Zane:
			_lblName.text = Localise("CharacterName.Zane");
			_crystalIcon.spriteName = "Crystal_Zane";
			_elementIcon.spriteName = "Element_Zane";
			_weaponIcon.spriteName = "Weapon_Zane";
			break;
		case MinifigType.Jay:
			_lblName.text = Localise("CharacterName.Jay");
			_crystalIcon.spriteName = "Crystal_Jay";
			_elementIcon.spriteName = "Element_Jay";
			_weaponIcon.spriteName = "Weapon_Zane";
			break;
		case MinifigType.Kai:
			_lblName.text = Localise("CharacterName.Kai");
			_crystalIcon.spriteName = "Crystal_Kai";
			_elementIcon.spriteName = "Element_Kai";
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		case MinifigType.Lloyd:
			_lblName.text = Localise("CharacterName.Lloyd");
			_crystalIcon.spriteName = "Crystal_Lloyd";
			_elementIcon.spriteName = "Element_Lloyd";
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		case MinifigType.Nya:
			_lblName.text = Localise("CharacterName.Nya");
			_crystalIcon.spriteName = "Crystal_Nya";
			_elementIcon.spriteName = "Element_Nya";
			_weaponIcon.spriteName = "Weapon_Lloyd";
			break;
		}
		_lblNameShadow.text = _lblName.text;
		StringBuilder stringBuilder = new StringBuilder();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int num2 = Team.LevelForShards(99999);
		int num3 = Mathf.Clamp(playerProfile.GetNinjaLevel(minifigType) + 1, 0, num2);
		stringBuilder.Append(Localise("General.Lvl"));
		stringBuilder.Append(num3);
		_lblLevel.text = stringBuilder.ToString();
		int num4 = Team.ShardsForLevel(num3);
		if (num4 != -1 && num3 != num2)
		{
			int ninjaTokens = playerProfile.GetNinjaTokens(minifigType);
			stringBuilder.Length = 0;
			stringBuilder.Append(ninjaTokens);
			stringBuilder.Append("/");
			stringBuilder.Append(num4);
			_lblCrystalCount.text = stringBuilder.ToString();
		}
		else
		{
			_lblCrystalCount.text = Localise("General.Max");
		}
		_locked = false;
	}

	public string GetPreviousCrystalCountString(MinifigType minifigType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int ninjaLevel = playerProfile.GetNinjaLevel(minifigType);
		int num = Team.ShardsForLevel(ninjaLevel);
		if (num != -1)
		{
			int value = playerProfile.GetNinjaTokens(minifigType) - 1;
			stringBuilder.Length = 0;
			stringBuilder.Append(value);
			stringBuilder.Append("/");
			stringBuilder.Append(num);
			return stringBuilder.ToString();
		}
		return Localise("General.Max");
	}

	public void MarkRankUpAsSeen(MinifigType minifigType)
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int max = Team.LevelForShards(99999);
		int num = Mathf.Clamp(playerProfile.GetNinjaLevel(minifigType) + 1, 0, max);
		switch (minifigType)
		{
		case MinifigType.Zane:
			playerProfile._pLastSeenZaneLevel = num;
			break;
		case MinifigType.Cole:
			playerProfile._pLastSeenColeLevel = num;
			break;
		case MinifigType.Jay:
			playerProfile._pLastSeenJayLevel = num;
			break;
		case MinifigType.Kai:
			playerProfile._pLastSeenKaiLevel = num;
			break;
		case MinifigType.Lloyd:
			playerProfile._pLastSeenLloydLevel = num;
			break;
		case MinifigType.Nya:
			playerProfile._pLastSeenNyaLevel = num;
			break;
		}
	}

	public bool ShouldShowRankUpEffect(MinifigType minifigType)
	{
		bool flag = false;
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int max = Team.LevelForShards(99999);
		int num = Mathf.Clamp(playerProfile.GetNinjaLevel(minifigType) + 1, 0, max);
		switch (minifigType)
		{
		case MinifigType.Zane:
			flag = num == playerProfile._pLastSeenZaneLevel;
			break;
		case MinifigType.Cole:
			flag = num == playerProfile._pLastSeenColeLevel;
			break;
		case MinifigType.Jay:
			flag = num == playerProfile._pLastSeenJayLevel;
			break;
		case MinifigType.Kai:
			flag = num == playerProfile._pLastSeenKaiLevel;
			break;
		case MinifigType.Lloyd:
			flag = num == playerProfile._pLastSeenLloydLevel;
			break;
		case MinifigType.Nya:
			flag = num == playerProfile._pLastSeenNyaLevel;
			break;
		}
		bool flag2 = num == 1;
		return !flag && !flag2;
	}

	public string GetPreviousLevelString(MinifigType minifigType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int ninjaLevel = playerProfile.GetNinjaLevel(minifigType);
		stringBuilder.Append(Localise("General.Lvl"));
		stringBuilder.Append(ninjaLevel);
		return stringBuilder.ToString();
	}

	private string Localise(string key)
	{
		if (LocalisationFacade.Instance == null)
		{
			return key;
		}
		string text = LocalisationFacade.Instance.GetString(key);
		if (text == string.Empty || text == null)
		{
			return key;
		}
		return text;
	}
}
