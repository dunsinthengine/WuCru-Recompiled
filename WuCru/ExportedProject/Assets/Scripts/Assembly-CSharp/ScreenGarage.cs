using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenGarage : ScreenBase
{
	[Serializable]
	public class VehicleUIDefinition
	{
		public GarageSlotButton _button;

		public float _modelScale;

		public Vector3 _modelOffset;

		public string _prefabLocation;

		[NonSerialized]
		public GameObject _modelPrefab;

		public UnlockableVehicleParts.VehicleDefinition.VEHICLE_TYPE _type;

		public int _season;

		[NonSerialized]
		public int _vehicleIndex;

		public void LoadPrefab()
		{
			_modelPrefab = Resources.Load<GameObject>(_prefabLocation);
		}

		public void ClearPrefab()
		{
			_modelPrefab = null;
		}
	}

	private const string BACKGROUND_IMAGE_VANILLA = "City_Background";

	private const string BACKGROUND_IMAGE_HOT = "City_Background";

	private const string BACKGROUND_IMAGE_CITY = "City_Background";

	private const string PARTS_LOC_KEY = "Garage.Parts";

	private const string HANDS_OF_TIME_MAP_NAME = "S2A1";

	private const string CITY_MAP_NAME = "S3A1";

	private const int VANILLA_SEASON_INDEX = 1;

	private const int HANDS_OF_TIME_SEASON_INDEX = 2;

	private const int CITY_SEASON_INDEX = 3;

	private const int NUM_VEHICLE_PARTS = 3;

	[SerializeField]
	private UIScrollView _scrollViewRef;

	[SerializeField]
	private UITable _tableRef;

	[SerializeField]
	private UILabel _partsLabel;

	[SerializeField]
	private Texture2D _whiteTexture;

	[SerializeField]
	private Material _backgroundMaterial;

	[SerializeField]
	private Transform _vehicleSpawnPoint;

	[SerializeField]
	private VehicleUIDefinition[] _vehicleList;

	[SerializeField]
	private UIWidget[] _partIndicatorBGs;

	[SerializeField]
	private UIWidget[] _partIndicatorIcons;

	[SerializeField]
	private Material _silhouetteMaterial;

	private Dictionary<int, Texture2D> _backgrounds = new Dictionary<int, Texture2D>();

	private GameObject _instantiatedVehicle;

	private bool _dragging;

	private Vector3 _oldMP;

	private VehiclePartsHandler _vph;

	private int _selectedVehicle;

	private int _vehicleRenderLayer;

	private float _rotSpeed;

	protected override void Awake()
	{
		base.Awake();
		_vehicleRenderLayer = LayerMask.NameToLayer("MenuCharacter");
		List<int> usedVehicleParts = GameController.Instance.PlayerProfile.GetUsedVehicleParts();
		UnlockableVehicleParts._pInstance.updateCollected(usedVehicleParts);
		List<int> viewedVehicleParts = GameController.Instance.PlayerProfile.GetViewedVehicleParts();
		UnlockableVehicleParts._pInstance.updateViewed(viewedVehicleParts);
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		for (int i = 0; i < _vehicleList.Length; i++)
		{
			_vehicleList[i].LoadPrefab();
		}
		bool showMinifigGoldenTrail = GameController.Instance.PlayerProfile.HasAnyUnseenMinfigParts();
		int[] unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
		bool showDojoGoldenTrail = unseenCards != null && unseenCards.Length > 0;
		ScreenHeaderNavigation._pInstance.OpenHeader();
		ScreenHeaderNavigation._pInstance.ConfigureHeader(onSkillsPressed, onMapPressed, null, OnDojo, onSettingsPressed, showMinifigGoldenTrail, false, showDojoGoldenTrail);
		List<int> usedVehicleParts = GameController.Instance.PlayerProfile.GetUsedVehicleParts();
		UnlockableVehicleParts._pInstance.updateCollected(usedVehicleParts);
		List<int> viewedVehicleParts = GameController.Instance.PlayerProfile.GetViewedVehicleParts();
		UnlockableVehicleParts._pInstance.updateViewed(viewedVehicleParts);
		_selectedVehicle = -1;
		_scrollViewRef.ResetPosition();
		for (int j = 0; j < _vehicleList.Length; j++)
		{
			for (int k = 0; k < UnlockableVehicleParts._pInstance._vehicleDefinitions.Length; k++)
			{
				if (_vehicleList[j]._season == UnlockableVehicleParts._pInstance._vehicleDefinitions[k]._season && _vehicleList[j]._type == UnlockableVehicleParts._pInstance._vehicleDefinitions[k]._vehicleNumber)
				{
					_vehicleList[j]._vehicleIndex = k;
					break;
				}
			}
		}
		bool flag = GameController.Instance.areaMapDB.IsMapAvalible("S2A1");
		bool flag2 = GameController.Instance.areaMapDB.IsMapAvalible("S3A1");
		for (int l = 0; l < _vehicleList.Length; l++)
		{
			if (_vehicleList[l]._button._goldenFrame != null)
			{
			}
			if ((_vehicleList[l]._season == 2 && !flag) || (_vehicleList[l]._season == 3 && !flag2))
			{
				_vehicleList[l]._button.gameObject.SetActive(false);
				continue;
			}
			_vehicleList[l]._button.gameObject.SetActive(true);
			int num = UnlockableVehicleParts._pInstance.numPartUnlocked(_vehicleList[l]._vehicleIndex);
			if (UnlockableVehicleParts._pInstance.isPartUnlocked(_vehicleList[l]._vehicleIndex, 0))
			{
				if (!UnlockableVehicleParts._pInstance.hasPartBeenViewed(_vehicleList[l]._vehicleIndex, 0) && _vehicleList[l]._button._goldenFrame != null)
				{
					_vehicleList[l]._button._goldenFrame.SetVisible(true);
				}
			}
			else if (UnlockableVehicleParts._pInstance.isPartUnlocked(_vehicleList[l]._vehicleIndex, 1))
			{
				if (!UnlockableVehicleParts._pInstance.hasPartBeenViewed(_vehicleList[l]._vehicleIndex, 1) && _vehicleList[l]._button._goldenFrame != null)
				{
					_vehicleList[l]._button._goldenFrame.SetVisible(true);
				}
			}
			else if (UnlockableVehicleParts._pInstance.isPartUnlocked(_vehicleList[l]._vehicleIndex, 2) && !UnlockableVehicleParts._pInstance.hasPartBeenViewed(_vehicleList[l]._vehicleIndex, 2) && _vehicleList[l]._button._goldenFrame != null)
			{
				_vehicleList[l]._button._goldenFrame.SetVisible(true);
			}
			_vehicleList[l]._button.SetCompletedState(num >= 3);
		}
		_backgrounds.Add(1, Resources.Load<Texture2D>("City_Background"));
		_backgrounds.Add(2, Resources.Load<Texture2D>("City_Background"));
		_backgrounds.Add(3, Resources.Load<Texture2D>("City_Background"));
		for (int m = 0; m < _vehicleList.Length; m++)
		{
			if ((_vehicleList[m]._season != 2 || flag) && (_vehicleList[m]._season != 3 || flag2))
			{
				SlotPressed(m);
				break;
			}
		}
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
		for (int i = 0; i < _vehicleList.Length; i++)
		{
			_vehicleList[i].ClearPrefab();
		}
		_backgrounds.Clear();
		_backgroundMaterial.mainTexture = _whiteTexture;
		UnityEngine.Object.Destroy(_instantiatedVehicle);
	}

	private void createVehicle()
	{
		if (_vehicleList.Length <= _selectedVehicle || _vehicleList[_selectedVehicle]._modelPrefab == null)
		{
			return;
		}
		_instantiatedVehicle = UnityEngine.Object.Instantiate(_vehicleList[_selectedVehicle]._modelPrefab);
		_vph = _instantiatedVehicle.GetComponent<VehiclePartsHandler>();
		if (_vph != null)
		{
			for (int i = 0; i < 3; i++)
			{
				if (!UnlockableVehicleParts._pInstance.isPartUnlocked(_vehicleList[_selectedVehicle]._vehicleIndex, i))
				{
					_vph.OverrideMaterial(i, _silhouetteMaterial);
				}
				else if (!UnlockableVehicleParts._pInstance.hasPartBeenViewed(_vehicleList[_selectedVehicle]._vehicleIndex, i))
				{
					_vph.Flash(i, _silhouetteMaterial);
					UnlockableVehicleParts._pInstance.ViewVehiclePartSpecial(_vehicleList[_selectedVehicle]._vehicleIndex, i);
				}
			}
		}
		float modelScale = _vehicleList[_selectedVehicle]._modelScale;
		_instantiatedVehicle.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
		_instantiatedVehicle.transform.parent = _vehicleSpawnPoint;
		_instantiatedVehicle.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
		_instantiatedVehicle.transform.localPosition = _vehicleList[_selectedVehicle]._modelOffset;
		NGUITools.SetLayer(_instantiatedVehicle, _vehicleRenderLayer);
	}

	protected override void Update()
	{
		base.Update();
		UpdateCheats();
		UpdateDrag();
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
			if (!screenInstance.gameObject.activeInHierarchy)
			{
				ScreenHeaderNavigation._pInstance.OnMap();
			}
		}
	}

	public void onPress()
	{
		_dragging = true;
		_oldMP = Input.mousePosition;
	}

	public void onRelease()
	{
		_dragging = false;
		Vector3 mousePosition = Input.mousePosition;
		float num = _oldMP.x - mousePosition.x;
		num *= 0.1f;
		_rotSpeed = Mathf.Clamp(num, -3f, 3f);
	}

	public void onSkillsPressed()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		SceneManager.LoadScene("Empty");
		ScreenSkills screenInstance = instance.GetScreenInstance<ScreenSkills>();
		Navigate(screenInstance);
	}

	public void onMapPressed()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		SceneManager.LoadScene("Empty");
		if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap())
		{
			ScreenWorldMap screenInstance = instance.GetScreenInstance<ScreenWorldMap>();
			Navigate(screenInstance);
		}
		else
		{
			ScreenMap screenInstance2 = instance.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance2);
		}
	}

	public void onSettingsPressed()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenSettings screenInstance = instance.GetScreenInstance<ScreenSettings>();
		if (Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenSettings>().gameObject.activeInHierarchy)
		{
			screenInstance.EndFlow();
			return;
		}
		screenInstance._returnLoc = ScreenSettings.RETURN_SCREEN_TYPE.SCREEN_GARAGE;
		instance.ShowPopupOverlay(screenInstance);
	}

	public void OnDojo()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		SceneManager.LoadScene("Dojo");
		Navigate(screenInstance);
	}

	public void onSlot1Pressed()
	{
		SlotPressed(0);
	}

	public void onSlot2Pressed()
	{
		SlotPressed(1);
	}

	public void onSlot3Pressed()
	{
		SlotPressed(2);
	}

	public void onSlot4Pressed()
	{
		SlotPressed(3);
	}

	public void onSlot5Pressed()
	{
		SlotPressed(4);
	}

	public void onSlot6Pressed()
	{
		SlotPressed(5);
	}

	public void onSlot7Pressed()
	{
		SlotPressed(6);
	}

	public void onSlot8Pressed()
	{
		SlotPressed(7);
	}

	public void onSlot9Pressed()
	{
		SlotPressed(8);
	}

	public void onSlot10Pressed()
	{
		SlotPressed(9);
	}

	public void onSlot11Pressed()
	{
		SlotPressed(10);
	}

	public void onSlot12Pressed()
	{
		SlotPressed(11);
	}

	private void UpdateCheats()
	{
	}

	private void UpdateDrag()
	{
		if (_instantiatedVehicle == null)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (_dragging)
		{
			Vector3 mousePosition = Input.mousePosition;
			float num = _oldMP.x - mousePosition.x;
			num *= 0.25f;
			_instantiatedVehicle.transform.Rotate(0f, num, 0f);
			_oldMP = mousePosition;
			return;
		}
		if (_rotSpeed < 0f)
		{
			_rotSpeed += deltaTime;
			if (_rotSpeed > 0f)
			{
				_rotSpeed = 0f;
			}
		}
		else if (_rotSpeed > 0f)
		{
			_rotSpeed -= deltaTime;
			if (_rotSpeed < 0f)
			{
				_rotSpeed = 0f;
			}
		}
		_instantiatedVehicle.transform.Rotate(0f, _rotSpeed, 0f);
	}

	private void SlotPressed(int slotIdx, bool doForce = false)
	{
		if (_selectedVehicle != slotIdx || doForce)
		{
			if (_instantiatedVehicle != null)
			{
				UnityEngine.Object.Destroy(_instantiatedVehicle);
			}
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_selectedVehicle = slotIdx;
			int num = UnlockableVehicleParts._pInstance.numPartUnlocked(_vehicleList[_selectedVehicle]._vehicleIndex);
			if (_vehicleList[_selectedVehicle]._button._goldenFrame != null)
			{
				_vehicleList[_selectedVehicle]._button._goldenFrame.SetVisible(false);
			}
			_partsLabel.text = LocalisationFacade.Instance.GetString("Garage.Parts") + " " + num + "/" + 3;
			_backgroundMaterial.mainTexture = _backgrounds[_vehicleList[_selectedVehicle]._season];
			Debug.Log("Set Mat");
			for (int i = 0; i < _partIndicatorBGs.Length; i++)
			{
				_partIndicatorBGs[i].color = ((!UnlockableVehicleParts._pInstance.isPartUnlocked(_vehicleList[_selectedVehicle]._vehicleIndex, i)) ? Color.Lerp(Color.white, Color.black, 0.66f) : Color.white);
				_partIndicatorIcons[i].color = _partIndicatorBGs[i].color;
			}
			for (int j = 0; j < _vehicleList.Length; j++)
			{
				_vehicleList[j]._button.SetSelectedState(j == _selectedVehicle);
			}
			_tableRef.Reposition();
			createVehicle();
		}
	}
}
