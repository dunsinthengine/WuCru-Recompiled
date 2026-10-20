using UnityEngine;

public class MapButtonInfo : MonoBehaviour
{
	private int _coordX;

	private int _coordY;

	public bool _wantDebug;

	public UIWidget[] _stars;

	public UIWidget _unexplored;

	public UIWidget _currentObjective;

	public UIWidget _dotdMarker;

	public UIWidget _wardrobeMarker;

	public UIWidget _vehicleMarker;

	public UISprite _unexploredFTUE;

	public UISprite _objectiveFTUE;

	public UISprite _vehiclePartFTUE;

	public UITexture _playerImage;

	public UISprite _dropShadow;

	public GameObject _currentPosition;

	private bool _isObjective;

	private bool _isLocked;

	public UILabel _label;

	public UIButton _button;

	public int _pCoordX
	{
		get
		{
			return _coordX;
		}
		set
		{
			_coordX = value;
		}
	}

	public int _pCoordY
	{
		get
		{
			return _coordY;
		}
		set
		{
			_coordY = value;
		}
	}

	public int _pSeed
	{
		get
		{
			return GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(_pCoordY, _pCoordX);
		}
	}

	public bool _pIsLocked
	{
		get
		{
			return _isLocked;
		}
	}

	public bool _pIsObjective
	{
		get
		{
			return _isObjective;
		}
	}

	public AreaMapNode _pNode
	{
		get
		{
			return GameController.Instance.areaMapMetaLayer.GetNodeAt(_pCoordY, _pCoordX);
		}
	}

	public void SetStars(int numStars)
	{
		int num = _stars.Length;
		for (int i = 0; i < num; i++)
		{
			UISprite uISprite = (UISprite)_stars[i];
			uISprite.spriteName = ((i >= numStars) ? "MiniStar_Empty" : "MiniStar_Awarded");
		}
	}

	public void SetLocked(bool isLocked)
	{
		_isLocked = isLocked;
	}

	public void SetCurrentObjective(bool isObjective)
	{
		_currentObjective.enabled = isObjective;
		_isObjective = isObjective;
	}

	public void SetCurrentPositionVisible(bool visible)
	{
		_currentPosition.SetActive(visible);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenMap screenInstance = instance.GetScreenInstance<ScreenMap>();
		if (visible)
		{
			_playerImage.mainTexture = screenInstance._pPlayerMugshot;
		}
		else
		{
			_playerImage.mainTexture = null;
		}
	}

	public void SetUnexploredVisible(bool visible)
	{
		_unexplored.enabled = visible;
	}

	public void SetStarsVisible(bool visible)
	{
		int num = _stars.Length;
		for (int i = 0; i < num; i++)
		{
			_stars[i].enabled = visible;
		}
	}

	public void SetDoTDActive(bool visible)
	{
		_dotdMarker.enabled = visible;
	}

	public void SetWardrobeMarkerActive(bool visible)
	{
		_wardrobeMarker.enabled = visible;
	}

	public void SetVehicleMarkerActive(bool visible)
	{
		_vehicleMarker.enabled = visible;
	}

	public void ShowUnexploredFTUE(bool visible)
	{
		_unexploredFTUE.enabled = visible && _unexplored.enabled;
	}

	public void ShowObjectiveFTUE(bool visible, GameObject arrow)
	{
		_objectiveFTUE.enabled = visible && _currentObjective.enabled;
		if (arrow != null && _objectiveFTUE.enabled)
		{
			arrow.transform.parent = _objectiveFTUE.transform;
			arrow.transform.localPosition = Vector3.zero;
		}
	}

	public void ShowVehiclePartFTUE(bool visible)
	{
		_vehiclePartFTUE.enabled = visible && _vehicleMarker.enabled;
	}
}
