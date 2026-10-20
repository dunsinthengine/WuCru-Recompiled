using UnityEngine;

public class HUDRosterSlot : MonoBehaviour
{
	public Texture[] _ninjaTextures;

	public UITexture _ninjaIcon;

	public UITexture _playerIcon;

	public UISprite _borderCircle;

	public UISprite _abilityIcon;

	public UISprite _deadCross;

	public UIWidget _mainInfoWrapper;

	public UISprite _selectedCircle;

	public UISprite _selectedArrow;

	public UISprite _blob;

	public UIButton _button;

	private readonly Color32 _zaneColor = Color.white;

	private readonly Color32 _jayColor = new Color32(60, 166, byte.MaxValue, byte.MaxValue);

	private readonly Color32 _kaiColor = new Color32(243, 30, 32, byte.MaxValue);

	private readonly Color32 _lloydColor = new Color32(7, 182, 24, byte.MaxValue);

	private readonly Color32 _coleColor = new Color32(176, 123, 39, byte.MaxValue);

	private readonly Color32 _nyaColor = new Color32(byte.MaxValue, 148, 171, byte.MaxValue);

	private readonly Color32 _playerColor = new Color32(235, 155, 17, byte.MaxValue);

	private float _abilityScale = 1f;

	private float _maxScale = 1.2f;

	private float _minScale = 0.8f;

	private bool _scaleDir;

	private ElementalPower _elementPower;

	private MinifigType _minifigType;

	private int _characterID;

	public MinifigType _pMinifigType
	{
		get
		{
			return _minifigType;
		}
	}

	public int _pCharacterID
	{
		get
		{
			return _characterID;
		}
	}

	public void SetSelected(bool isSelected)
	{
		_blob.enabled = !isSelected;
		_selectedCircle.enabled = isSelected;
		_selectedArrow.enabled = isSelected;
	}

	public void SetEnabled(bool isEnabled)
	{
		_mainInfoWrapper.gameObject.SetActive(isEnabled);
		if (!isEnabled)
		{
			_deadCross.enabled = false;
		}
		_button.isEnabled = isEnabled;
	}

	public void SetDead()
	{
		_deadCross.enabled = true;
		_mainInfoWrapper.alpha = 0.5f;
		_button.isEnabled = false;
	}

	private void Update()
	{
		Vector3 localScale = _abilityIcon.transform.localScale;
		if (_elementPower != GameController.Instance._hudElemental)
		{
			_abilityScale = 1f;
			_scaleDir = false;
			localScale.x = 1f;
			localScale.y = 1f;
			_abilityIcon.transform.localScale = localScale;
			return;
		}
		float deltaTime = Time.deltaTime;
		localScale = _abilityIcon.transform.localScale;
		localScale.x = _abilityScale;
		localScale.y = _abilityScale;
		_abilityIcon.transform.localScale = localScale;
		if (!_scaleDir)
		{
			_abilityScale += deltaTime;
			if (_abilityScale > _maxScale)
			{
				_abilityScale = _maxScale;
				_scaleDir = true;
			}
		}
		else
		{
			_abilityScale -= deltaTime;
			if (_abilityScale < _minScale)
			{
				_abilityScale = _minScale;
				_scaleDir = false;
			}
		}
	}

	public void Configure(MinifigType minifigType, int charID)
	{
		_characterID = charID;
		_minifigType = minifigType;
		_button.isEnabled = true;
		_deadCross.enabled = false;
		_mainInfoWrapper.gameObject.SetActive(true);
		_mainInfoWrapper.alpha = 1f;
		_selectedArrow.enabled = false;
		_selectedCircle.enabled = false;
		_blob.enabled = true;
		if (minifigType == MinifigType.None)
		{
			_ninjaIcon.enabled = false;
			_playerIcon.enabled = true;
			_borderCircle.color = _playerColor;
			PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
			_elementPower = playerProfile.ElementalPower;
			switch (playerProfile.ElementalPower)
			{
			case ElementalPower.Earth:
				_abilityIcon.spriteName = "Element_Cole";
				break;
			case ElementalPower.Electricity:
				_abilityIcon.spriteName = "Element_Jay";
				break;
			case ElementalPower.Energy:
				_abilityIcon.spriteName = "Element_Lloyd";
				break;
			case ElementalPower.Fire:
				_abilityIcon.spriteName = "Element_Kai";
				break;
			case ElementalPower.Ice:
				_abilityIcon.spriteName = "Element_Zane";
				break;
			case ElementalPower.Water:
				_abilityIcon.spriteName = "Element_Nya";
				break;
			}
			Texture playerTex = (ScreenHud._pInstance != null) ? ScreenHud._pInstance.GetPlayerTexture() : null;
			_playerIcon.mainTexture = playerTex;
			return;
		}
		_ninjaIcon.enabled = true;
		_playerIcon.enabled = false;
		switch (minifigType)
		{
		case MinifigType.Cole:
			_ninjaIcon.mainTexture = _ninjaTextures[0];
			_abilityIcon.spriteName = "Element_Cole";
			_borderCircle.color = _coleColor;
			_elementPower = ElementalPower.Earth;
			break;
		case MinifigType.Jay:
			_ninjaIcon.mainTexture = _ninjaTextures[1];
			_abilityIcon.spriteName = "Element_Jay";
			_borderCircle.color = _jayColor;
			_elementPower = ElementalPower.Electricity;
			break;
		case MinifigType.Kai:
			_ninjaIcon.mainTexture = _ninjaTextures[2];
			_abilityIcon.spriteName = "Element_Kai";
			_borderCircle.color = _kaiColor;
			_elementPower = ElementalPower.Fire;
			break;
		case MinifigType.Lloyd:
			_ninjaIcon.mainTexture = _ninjaTextures[3];
			_abilityIcon.spriteName = "Element_Lloyd";
			_borderCircle.color = _lloydColor;
			_elementPower = ElementalPower.Energy;
			break;
		case MinifigType.Nya:
			_ninjaIcon.mainTexture = _ninjaTextures[4];
			_abilityIcon.spriteName = "Element_Nya";
			_borderCircle.color = _nyaColor;
			_elementPower = ElementalPower.Water;
			break;
		case MinifigType.Zane:
			_ninjaIcon.mainTexture = _ninjaTextures[5];
			_abilityIcon.spriteName = "Element_Zane";
			_borderCircle.color = _zaneColor;
			_elementPower = ElementalPower.Ice;
			break;
		}
	}
}
