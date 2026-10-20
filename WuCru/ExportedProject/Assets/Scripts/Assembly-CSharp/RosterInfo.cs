using System.Text;
using UnityEngine;

public class RosterInfo : MonoBehaviour
{
	public MinifigType _minifigType;

	public UILabel _lblLocked;

	public UILabel _lblCrystalCount;

	public UILabel _lblCrystalCountShadow;

	public UILabel _lblLevel;

	public UILabel _lblName;

	public UITexture _arrow;

	public UIButton _buttonRef;

	public UISprite _weaponIcon;

	public UISprite _elementIcon;

	public UISprite _crystalIcon;

	public UISprite _padlock;

	public UITexture _faceIcon;

	public UITexture _faceFrame;

	public UISprite _ninjaAlreadySelectedIcon;

	public Transform _scaleWrapper;

	public ScreenObjectTransitionWidget[] _transitionWidgets;

	private bool _isInRosterAlready;

	private bool _isUnlocked;

	private int _level;

	private ScreenRoster _screenRef;

	private bool _isHovered;

	private Color _baseFrameColor;

	private ScreenRoster _parentScreen;

	public ScreenRoster _pParentScreen
	{
		get
		{
			return _parentScreen;
		}
		set
		{
			_parentScreen = value;
		}
	}

	public bool _pIsUnlocked
	{
		get
		{
			return _isUnlocked;
		}
	}

	public int _pLevel
	{
		get
		{
			return _level;
		}
	}

	public ScreenRoster _pScreenRef
	{
		get
		{
			return _screenRef;
		}
		set
		{
			_screenRef = value;
		}
	}

	public bool _pIsHovered
	{
		get
		{
			return _isHovered;
		}
	}

	public bool _pAlreadyRostered
	{
		get
		{
			return _isInRosterAlready;
		}
	}

	public bool _pUnselectable
	{
		get
		{
			PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
			int iD = Team.GetID(_minifigType, _level);
			int ninjaInTeam = playerProfile.GetNinjaInTeam(_pScreenRef._pSelectedSlot);
			return _isInRosterAlready && ninjaInTeam != iD;
		}
	}

	private void Awake()
	{
		if (_faceFrame != null)
		{
			_baseFrameColor = _faceFrame.color;
		}
	}

	public void SetTransitionWidgetsToDefaultState()
	{
		base.gameObject.SetActive(false);
	}

	public void SetWrapperScale(Vector3 newScale)
	{
		_scaleWrapper.localScale = newScale;
	}

	public void TweenIn()
	{
		base.gameObject.SetActive(true);
		int num = _transitionWidgets.Length;
		for (int i = 0; i < num; i++)
		{
			_transitionWidgets[i].TweenIn();
		}
	}

	public void TweenOut()
	{
		int num = _transitionWidgets.Length;
		for (int i = 0; i < num; i++)
		{
			_transitionWidgets[i].TweenOut();
		}
	}

	public void RefreshIconStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int num = 0;
		switch (_minifigType)
		{
		case MinifigType.Zane:
			num = playerProfile.ZaneLevel;
			break;
		case MinifigType.Cole:
			num = playerProfile.ColeLevel;
			break;
		case MinifigType.Jay:
			num = playerProfile.JayLevel;
			break;
		case MinifigType.Kai:
			num = playerProfile.KaiLevel;
			break;
		case MinifigType.Nya:
			num = playerProfile.NyaLevel;
			break;
		case MinifigType.Lloyd:
			num = playerProfile.LloydLevel;
			break;
		}
		_level = num;
		int num2 = Team.LevelForShards(99999);
		int num3 = Mathf.Clamp(num + 1, 0, num2);
		stringBuilder.Append(Localise("General.Lvl"));
		stringBuilder.Append(num3);
		_lblLevel.text = stringBuilder.ToString();
		int num4 = Team.ShardsForLevel(playerProfile.GetNinjaLevel(_minifigType) + 1);
		if (num4 != -1 && num3 != num2)
		{
			int ninjaTokens = playerProfile.GetNinjaTokens(_minifigType);
			stringBuilder.Length = 0;
			stringBuilder.Append(ninjaTokens);
			stringBuilder.Append("/");
			stringBuilder.Append(num4);
			_lblCrystalCount.text = stringBuilder.ToString();
			_lblCrystalCountShadow.text = _lblCrystalCount.text;
		}
		else
		{
			_lblCrystalCount.text = Localise("General.Max");
			_lblCrystalCountShadow.text = _lblCrystalCount.text;
		}
		int iD = Team.GetID(_minifigType, num);
		_isUnlocked = playerProfile.UnlockedNinjaSKUs.Contains(iD);
		_lblLevel.enabled = _isUnlocked;
		_padlock.enabled = !_isUnlocked;
		_lblLocked.enabled = !_isUnlocked;
		_arrow.enabled = _isUnlocked;
		_lblName.color = ((!_isUnlocked) ? Color.grey : GlobalStyle._pInstance.GetNameTextMainColor(_minifigType));
		UpdateLockedStatus();
	}

	public void UpdateLockedStatus()
	{
		UpdateRosteredStatus();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int iD = Team.GetID(_minifigType, _level);
		int ninjaInTeam = playerProfile.GetNinjaInTeam(_pScreenRef._pSelectedSlot);
		bool flag = _isInRosterAlready && ninjaInTeam != iD;
		bool isEnabled = _isUnlocked && !flag;
		_buttonRef.isEnabled = isEnabled;
		_faceIcon.alpha = ((!_isUnlocked) ? 0.5f : ((!flag) ? 1f : 0.6f));
		_faceFrame.color = ((!_isUnlocked) ? new Color(0.15f, 0.15f, 0.15f) : ((!flag) ? _baseFrameColor : (_baseFrameColor * 0.5f)));
		_weaponIcon.alpha = _faceIcon.alpha;
		_elementIcon.alpha = _faceIcon.alpha;
		_crystalIcon.alpha = _faceIcon.alpha;
		_ninjaAlreadySelectedIcon.enabled = flag;
		_lblCrystalCount.color = ((_isUnlocked && !flag) ? Color.white : new Color(0.2f, 0.2f, 0.2f));
	}

	public void UpdateRosteredStatus()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int level = 0;
		switch (_minifigType)
		{
		case MinifigType.Zane:
			level = playerProfile.ZaneLevel;
			break;
		case MinifigType.Cole:
			level = playerProfile.ColeLevel;
			break;
		case MinifigType.Jay:
			level = playerProfile.JayLevel;
			break;
		case MinifigType.Kai:
			level = playerProfile.KaiLevel;
			break;
		case MinifigType.Nya:
			level = playerProfile.NyaLevel;
			break;
		case MinifigType.Lloyd:
			level = playerProfile.LloydLevel;
			break;
		}
		int iD = Team.GetID(_minifigType, level);
		_isInRosterAlready = playerProfile.IsNinjaInTeam(iD);
	}

	public void OnDragOver(GameObject draggedObject)
	{
		_isHovered = true;
	}

	public void OnDragOut(GameObject draggedObject)
	{
		_isHovered = false;
		if (UICamera.hoveredObject == base.gameObject)
		{
			_pParentScreen.OnHeroTap(this);
		}
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

	public string GetPreviousCrystalCountString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int ninjaLevel = playerProfile.GetNinjaLevel(_minifigType);
		int num = Team.ShardsForLevel(ninjaLevel);
		if (num != -1)
		{
			int value = playerProfile.GetNinjaTokens(_minifigType) - 1;
			stringBuilder.Length = 0;
			stringBuilder.Append(value);
			stringBuilder.Append("/");
			stringBuilder.Append(num);
			return stringBuilder.ToString();
		}
		return Localise("General.Max");
	}

	public string GetPreviousLevelString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int ninjaLevel = playerProfile.GetNinjaLevel(_minifigType);
		stringBuilder.Append(Localise("General.Lvl"));
		stringBuilder.Append(ninjaLevel);
		return stringBuilder.ToString();
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
}
