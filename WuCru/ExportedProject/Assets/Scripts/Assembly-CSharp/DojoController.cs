using System;
using System.Collections.Generic;
using AmuzoEngine;
using Cape.LegoSdk.Core;
using UnityEngine;

public class DojoController : MonoBehaviour
{
	[Serializable]
	public class DojoCharacterDef : MinifigDefinition
	{
		public RuntimeAnimatorController animController;

		public string startState = "SitIdle_2";

		public int initialWalkDelay;

		public bool exitSofaFromLeft;
	}

	[Serializable]
	public class Highlights
	{
		public MeshRenderer _rends;

		public Material _highlight;

		public Material _regular;
	}

	private enum ESTATE
	{
		ROOT = 0,
		TRAVEL_TO_ARCADE = 1,
		TRAVEL_FROM_ARCADE = 2,
		ARCADE = 3,
		TRAVEL_TO_CINEMA = 4,
		TRAVEL_FROM_CINEMA = 5,
		CINEMA = 6,
		TRAVEL_TO_STICKERBOOK = 7,
		TRAVEL_FROM_STICKERBOOK = 8,
		STICKERBOOK = 9
	}

	public const string HTML5GAME = "http://dev.division13.com/ninjago/build/";

	private const int ARCADE_INDEX = 0;

	private static DojoController _instance;

	private const float TRAVEL_DUR = 1f;

	public Camera _cameraRef;

	public ArcadeMachineFX _arcadeFX;

	public Transform _rootLocation;

	public Transform _arcadeLocation;

	public Transform _cinemaLocation;

	public Transform _stickerbookLocation;

	public Transform _stickerbookTravelLookAt;

	public GameObject _menuCharacterPrefab;

	public GameObject _stickerbookGoldenTrail;

	public DojoCharacterDef[] _characterDefinitions;

	public DojoPath[] _paths;

	public Highlights[] _highlights;

	public GameObject _arcadeDisabledFX;

	public UniWebView _webView;

	private UniWebView _instancedWebView;

	private ESTATE _currentState;

	private Vector3 _from;

	private Vector3 _to;

	private Quaternion _fromRot;

	private Quaternion _toRot;

	private float _stateTime;

	private bool _touchDown;

	private int[] _unseenCards;

	private List<GameObject> _objects = new List<GameObject>();

	private List<Animator> _animators = new List<Animator>();

	private List<DojoCharacterBehaviour> _behaviours = new List<DojoCharacterBehaviour>();

	private bool _hasInternetCinema;

	private bool _hasInternetArcade;

	private float _visitedAreaStartTime;

	private float _arcadeCheckTime;

	public static DojoController _pInstance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		_stickerbookGoldenTrail.SetActive(false);
		_unseenCards = GameController.Instance.PlayerProfile.GetUnseenCards();
		if (_unseenCards != null && _unseenCards.Length > 0)
		{
			_stickerbookGoldenTrail.SetActive(true);
		}
		else
		{
			_unseenCards = null;
		}
		int num = 0;
		int num2 = _characterDefinitions.Length;
		bool flag = false;
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_menuCharacterPrefab);
			gameObject.transform.localScale = Vector3.one * 1.5f;
			gameObject.transform.position = ((!flag) ? _paths[num]._spawnPos.position : _paths[num]._offScreenPos.position);
			gameObject.transform.rotation = ((!flag) ? _paths[num]._spawnPos.rotation : _paths[num]._offScreenPos.rotation);
			DojoCharacterBehaviour dojoCharacterBehaviour = gameObject.AddComponent<DojoCharacterBehaviour>();
			dojoCharacterBehaviour._pPath = _paths[num];
			dojoCharacterBehaviour._pStartState = _characterDefinitions[i].startState;
			dojoCharacterBehaviour._pInitialWalkDelay = _characterDefinitions[i].initialWalkDelay;
			dojoCharacterBehaviour._pExitSofaFromLeft = _characterDefinitions[i].exitSofaFromLeft;
			if (flag)
			{
				dojoCharacterBehaviour.SetWaiting();
			}
			_behaviours.Add(dojoCharacterBehaviour);
			BuildCharacter(gameObject, _characterDefinitions[i]);
			_objects.Add(gameObject);
			Animator component = gameObject.GetComponent<Animator>();
			component.runtimeAnimatorController = _characterDefinitions[i].animController;
			_animators.Add(component);
			num++;
			if (num >= _paths.Length)
			{
				num = 0;
				flag = true;
			}
		}
		_arcadeFX.ChangeEnabledState(GlobalInGameData.DO_ENABLE_ARCADE);
		_arcadeDisabledFX.SetActive(!GlobalInGameData.DO_ENABLE_ARCADE);
		if (GlobalInGameData.DO_ENABLE_ARCADE)
		{
			string pArcadeMachineUrl = LegoCoreSdk.Instance._pArcadeMachineUrl;
			Debug.Log("ARCADE URL = " + pArcadeMachineUrl);
			if (pArcadeMachineUrl == null || string.IsNullOrEmpty(pArcadeMachineUrl.Trim()) || pArcadeMachineUrl == "NONE")
			{
				_arcadeDisabledFX.SetActive(true);
			}
			else
			{
				_arcadeDisabledFX.SetActive(false);
			}
		}
		SetHighlights(true);
		CheckInternetCinema();
		CheckInternetArcade();
		GameAudioManager.Instance.StopAllMusic(false);
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void Update()
	{
		UpdateBehaviours();
		Vector3 position = Input.mousePosition;
		if (Input.touchCount > 0)
		{
			position = Input.touches[0].position;
		}
		if (Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			if (!_touchDown)
			{
				Ray ray = _cameraRef.ScreenPointToRay(position);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 200f))
				{
					switch (hitInfo.collider.name)
					{
					case "ArcadeHitbox":
						OnClickedArcade();
						break;
					case "CinemaHitbox":
						OnClickedCinema();
						break;
					case "StickerbookHitbox":
						OnClickedStickerbook();
						break;
					case "CharacterHitBox1":
						RequestSpotAnim(0);
						break;
					case "CharacterHitBox2":
						RequestSpotAnim(1);
						break;
					case "CharacterHitBox3":
						RequestSpotAnim(2);
						break;
					}
				}
			}
			_touchDown = true;
		}
		else
		{
			_touchDown = false;
		}
		switch (_currentState)
		{
		case ESTATE.ROOT:
			UpdateRoot();
			break;
		case ESTATE.TRAVEL_TO_ARCADE:
			UpdateTravelToArcade();
			break;
		case ESTATE.TRAVEL_FROM_ARCADE:
			UpdateTravelFromArcade();
			break;
		case ESTATE.ARCADE:
			UpdateArcade();
			break;
		case ESTATE.TRAVEL_TO_CINEMA:
			UpdateTravelToCinema();
			break;
		case ESTATE.TRAVEL_FROM_CINEMA:
			UpdateTravelFromCinema();
			break;
		case ESTATE.CINEMA:
			UpdateCinema();
			break;
		case ESTATE.TRAVEL_TO_STICKERBOOK:
			UpdateTravelToStickerbook();
			break;
		case ESTATE.TRAVEL_FROM_STICKERBOOK:
			UpdateTravelFromStickerbook();
			break;
		case ESTATE.STICKERBOOK:
			UpdateStickerbook();
			break;
		}
	}

	private void SetHighlights(bool highlightsEnabled)
	{
		int num = _highlights.Length;
		for (int i = 0; i < num; i++)
		{
			if (i == 0 && !GlobalInGameData.DO_ENABLE_ARCADE)
			{
				_highlights[i]._rends.material = _highlights[i]._regular;
			}
			else
			{
				_highlights[i]._rends.material = ((!highlightsEnabled) ? _highlights[i]._regular : _highlights[i]._highlight);
			}
		}
	}

	private bool CheckInternetCinema()
	{
		_hasInternetCinema = InternetCheckFacade._pIsOnline;
		return _hasInternetCinema;
	}

	private bool CheckInternetArcade()
	{
		_hasInternetArcade = InternetCheckFacade._pIsOnline;
		return _hasInternetArcade;
	}

	private void RequestSpotAnim(int pathIndex)
	{
		int count = _behaviours.Count;
		for (int i = 0; i < count; i++)
		{
			if (_behaviours[i]._pState == DojoCharacterBehaviour.STATE.SITTING && _behaviours[i]._pPath == _paths[pathIndex])
			{
				_behaviours[i].RequestSpotAnim();
			}
		}
	}

	public void RequestCharacterSwap(DojoCharacterBehaviour original)
	{
		int count = _behaviours.Count;
		List<DojoCharacterBehaviour> list = new List<DojoCharacterBehaviour>();
		for (int i = 0; i < count; i++)
		{
			if (_behaviours[i] != original && _behaviours[i]._pState == DojoCharacterBehaviour.STATE.WAITING)
			{
				list.Add(_behaviours[i]);
			}
		}
		int count2 = list.Count;
		if (count2 > 0)
		{
			int index = UnityEngine.Random.Range(0, count2);
			list[index].SwapWith(original);
		}
		else
		{
			original.SwapWith(original);
		}
	}

	private void UpdateBehaviours()
	{
		int num = 0;
		int count = _behaviours.Count;
		for (int i = 0; i < count; i++)
		{
			if (_behaviours[i]._pState == DojoCharacterBehaviour.STATE.SITTING)
			{
				num++;
			}
		}
		for (int j = 0; j < count; j++)
		{
			_behaviours[j]._pNumOnSofa = num;
		}
	}

	public void OnClickedStickerbook()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenNoConnection screenInstance = instance.GetScreenInstance<ScreenNoConnection>();
		if (!screenInstance.gameObject.activeInHierarchy && _currentState == ESTATE.ROOT)
		{
			_from = _cameraRef.transform.position;
			_to = _stickerbookLocation.position;
			_fromRot = _cameraRef.transform.rotation;
			_toRot = _stickerbookLocation.rotation;
			_stateTime = Time.time;
			_currentState = ESTATE.TRAVEL_TO_STICKERBOOK;
			ScreenHeaderNavigation._pInstance.DisableButtons();
			_stickerbookGoldenTrail.SetActive(false);
			SetHighlights(false);
			_visitedAreaStartTime = Time.time;
		}
	}

	public void OnClickedArcade()
	{
		if (!GlobalInGameData.DO_ENABLE_ARCADE || _arcadeDisabledFX.activeInHierarchy)
		{
			return;
		}
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenNoConnection screenInstance = instance.GetScreenInstance<ScreenNoConnection>();
		if (screenInstance.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!CheckInternetArcade())
		{
			ScreenDojo screenInstance2 = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenDojo>();
			screenInstance2.ShowNoInternet();
		}
		else if (_currentState == ESTATE.ROOT)
		{
			_from = _cameraRef.transform.position;
			_to = _arcadeLocation.position;
			_fromRot = _cameraRef.transform.rotation;
			_toRot = _arcadeLocation.rotation;
			_stateTime = Time.time;
			_currentState = ESTATE.TRAVEL_TO_ARCADE;
			ScreenHeaderNavigation._pInstance.DisableButtons();
			_arcadeFX.FadeOff();
			if (ControllerVideoPlayer._pInstance != null)
			{
				ControllerVideoPlayer._pInstance.Pause(true);
			}
			SetHighlights(false);
			_visitedAreaStartTime = Time.time;
		}
	}

	public void OnClickedCinema()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenNoConnection screenInstance = instance.GetScreenInstance<ScreenNoConnection>();
		if (ControllerVideoPlayer._pInstance != null && ControllerVideoPlayer._pInstance._pIsCtaVisible)
		{
			ManagerCrossPromo._pInstance.OpenCtaUrl();
		}
		else if (!screenInstance.gameObject.activeInHierarchy && _currentState == ESTATE.ROOT)
		{
			if (!CheckInternetCinema())
			{
				ScreenDojo screenInstance2 = Facades<ScreenFacade>.Instance.GetScreenInstance<ScreenDojo>();
				screenInstance2.ShowNoInternet();
				return;
			}
			_from = _cameraRef.transform.position;
			_to = _cinemaLocation.position;
			_fromRot = _cameraRef.transform.rotation;
			_toRot = _cinemaLocation.rotation;
			_stateTime = Time.time;
			_currentState = ESTATE.TRAVEL_TO_CINEMA;
			ScreenHeaderNavigation._pInstance.DisableButtons();
			SetHighlights(false);
			_visitedAreaStartTime = Time.time;
		}
	}

	public void OnReturnToRoot()
	{
		if (_currentState == ESTATE.ROOT || _currentState == ESTATE.TRAVEL_FROM_ARCADE || _currentState == ESTATE.TRAVEL_FROM_CINEMA || _currentState == ESTATE.TRAVEL_FROM_STICKERBOOK)
		{
			return;
		}
		_from = _cameraRef.transform.position;
		_to = _rootLocation.transform.position;
		_fromRot = _cameraRef.transform.rotation;
		_toRot = _rootLocation.transform.rotation;
		int num = Mathf.FloorToInt(Time.time - _visitedAreaStartTime);
		Debug.Log("Time In Area:" + num);
		_stateTime = Time.time;
		switch (_currentState)
		{
		case ESTATE.ARCADE:
		{
			_currentState = ESTATE.TRAVEL_FROM_ARCADE;
			if (ControllerVideoPlayer._pInstance != null)
			{
				ControllerVideoPlayer._pInstance.Pause(false);
			}
			if (_instancedWebView != null)
			{
				_instancedWebView.Stop();
				_instancedWebView.Hide();
				UnityEngine.Object.Destroy(_instancedWebView.gameObject);
			}
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
			screenInstance._debug.text = string.Empty;
			break;
		}
		case ESTATE.CINEMA:
			_currentState = ESTATE.TRAVEL_FROM_CINEMA;
			break;
		case ESTATE.STICKERBOOK:
			_currentState = ESTATE.TRAVEL_FROM_STICKERBOOK;
			break;
		}
	}

	private void UpdateRoot()
	{
	}

	private void UpdateTravelToArcade()
	{
		float num = Time.time - _stateTime;
		if (num < 1f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 1f, 0f, 1f);
			_cameraRef.transform.position = Vector3.Lerp(_from, _to, t);
			_cameraRef.transform.rotation = Quaternion.Lerp(_fromRot, _toRot, t);
			return;
		}
		_cameraRef.transform.position = _to;
		_cameraRef.transform.rotation = _toRot;
		_currentState = ESTATE.ARCADE;
		ScreenHeaderNavigation._pInstance.ManualTweenBar(false);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		screenInstance.ShowBack(true);
		screenInstance.OnCinema(false);
		string pArcadeMachineUrl = LegoCoreSdk.Instance._pArcadeMachineUrl;
		Debug.Log("URL = " + pArcadeMachineUrl);
		GameObject gameObject = UnityEngine.Object.Instantiate(_webView.gameObject);
		_instancedWebView = gameObject.GetComponent<UniWebView>();
		_instancedWebView.Load(pArcadeMachineUrl);
		_instancedWebView.OnLoadComplete += OnLoadComplete;
		_instancedWebView.OnWebViewShouldClose += OnWebViewShouldClose;
		_instancedWebView.OnReceivedMessage += OnReceivedMessage;
	}

	private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (success)
		{
			_instancedWebView.Show();
			return;
		}
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		screenInstance._debug.text = errorMessage;
	}

	private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		if (message.path == "OnBack" && _currentState == ESTATE.ARCADE)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
			screenInstance.OnBack();
		}
	}

	private bool OnWebViewShouldClose(UniWebView webView)
	{
		if (_currentState == ESTATE.ARCADE)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
			screenInstance.OnBack();
		}
		return true;
	}

	private void UpdateTravelFromArcade()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		screenInstance._debug.text = string.Empty;
		float num = Time.time - _stateTime;
		if (num < 1f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 1f, 0f, 1f);
			_cameraRef.transform.position = Vector3.Lerp(_from, _to, t);
			_cameraRef.transform.rotation = Quaternion.Lerp(_fromRot, _toRot, t);
			return;
		}
		_cameraRef.transform.position = _to;
		_cameraRef.transform.rotation = _toRot;
		_currentState = ESTATE.ROOT;
		ScreenHeaderNavigation._pInstance.ManualTweenBar(true);
		ScreenHeaderNavigation._pInstance.EnableButtons();
		screenInstance.ShowBack(false);
		_arcadeFX.FadeIn();
		SetHighlights(true);
	}

	private void UpdateArcade()
	{
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			screenInstance.OnBack();
		}
		if (_arcadeCheckTime <= 0f && _instancedWebView != null)
		{
			CheckInternetArcade();
			if (!_hasInternetArcade)
			{
				if (_instancedWebView != null)
				{
					_instancedWebView.Stop();
					_instancedWebView.Hide();
					UnityEngine.Object.Destroy(_instancedWebView.gameObject);
				}
				screenInstance._debug.text = string.Empty;
				screenInstance.OnBack();
			}
			_arcadeCheckTime = 4f;
		}
		_arcadeCheckTime -= Time.deltaTime;
	}

	private void UpdateTravelToCinema()
	{
		float num = Time.time - _stateTime;
		if (num < 1f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 1f, 0f, 1f);
			_cameraRef.transform.position = Vector3.Lerp(_from, _to, t);
			_cameraRef.transform.rotation = Quaternion.Lerp(_fromRot, _toRot, t);
			return;
		}
		_cameraRef.transform.position = _to;
		_cameraRef.transform.rotation = _toRot;
		_currentState = ESTATE.CINEMA;
		ScreenHeaderNavigation._pInstance.ManualTweenBar(false);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		screenInstance.ShowBack(true);
		screenInstance.OnCinema(true);
	}

	private void UpdateTravelFromCinema()
	{
		float num = Time.time - _stateTime;
		if (num < 1f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 1f, 0f, 1f);
			_cameraRef.transform.position = Vector3.Lerp(_from, _to, t);
			_cameraRef.transform.rotation = Quaternion.Lerp(_fromRot, _toRot, t);
			return;
		}
		_cameraRef.transform.position = _to;
		_cameraRef.transform.rotation = _toRot;
		_currentState = ESTATE.ROOT;
		ScreenHeaderNavigation._pInstance.ManualTweenBar(true);
		ScreenHeaderNavigation._pInstance.EnableButtons();
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
		screenInstance.ShowBack(false);
		screenInstance.OnCinema(false);
		SetHighlights(true);
	}

	private void UpdateCinema()
	{
	}

	private void UpdateTravelToStickerbook()
	{
		float num = Time.time - _stateTime;
		if (num < 1f)
		{
			Vector3 forward = _stickerbookTravelLookAt.position - _cameraRef.transform.position;
			Quaternion quaternion = Quaternion.LookRotation(forward);
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.Linear, num, 1f, 0f, 1f);
			_cameraRef.transform.position = Vector3.Lerp(_from, _to, t);
			_cameraRef.transform.rotation = Quaternion.Lerp(Quaternion.Lerp(_fromRot, quaternion, t), Quaternion.Lerp(quaternion, _toRot, t), t);
		}
		else
		{
			_cameraRef.transform.position = _to;
			_cameraRef.transform.rotation = _toRot;
			_currentState = ESTATE.STICKERBOOK;
			ScreenHeaderNavigation._pInstance.ManualTweenBar(false);
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
			screenInstance.ShowBack(true);
			screenInstance.OnStickerbook();
			screenInstance.OnCinema(false);
		}
	}

	private void UpdateTravelFromStickerbook()
	{
		float num = Time.time - _stateTime;
		if (!(num < 0.5f))
		{
			num -= 0.5f;
			if (num < 1f)
			{
				Vector3 forward = _stickerbookTravelLookAt.position - _cameraRef.transform.position;
				Quaternion quaternion = Quaternion.LookRotation(forward);
				float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.Linear, num, 1f, 0f, 1f);
				_cameraRef.transform.position = Vector3.Lerp(_from, _to, t);
				_cameraRef.transform.rotation = Quaternion.Lerp(Quaternion.Lerp(_fromRot, quaternion, t), Quaternion.Lerp(quaternion, _toRot, t), t);
			}
			else
			{
				_cameraRef.transform.position = _to;
				_cameraRef.transform.rotation = _toRot;
				_currentState = ESTATE.ROOT;
				ScreenHeaderNavigation._pInstance.ManualTweenBar(true);
				ScreenHeaderNavigation._pInstance.EnableButtons();
				ScreenFacade instance = Facades<ScreenFacade>.Instance;
				ScreenDojo screenInstance = instance.GetScreenInstance<ScreenDojo>();
				screenInstance.ShowBack(false);
				SetHighlights(true);
			}
		}
	}

	private void UpdateStickerbook()
	{
	}

	private void BuildCharacter(GameObject obj, DojoCharacterDef definition)
	{
		MenuCharacter component = obj.GetComponent<MenuCharacter>();
		Texture2D characterTexture = CharacterBuilder.ConfigureTexture(component, definition);
		CharacterBuilder.ConfigureCharacter(component, definition, characterTexture);
	}
}
