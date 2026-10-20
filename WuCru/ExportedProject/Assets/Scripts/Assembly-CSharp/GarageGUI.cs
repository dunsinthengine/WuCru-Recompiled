using System;
using System.Collections;
using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class GarageGUI : MonoBehaviour
{
	public Text vehicleLabel;

	public GameObject vehicleContainer;

	public Camera vehicleCamera;

	public Button nextVehicleButton;

	public Button previousVehicleButton;

	public Button unlockVehicleButton;

	public Button upgradeVehicleButton;

	public Button selectVehicleButton;

	public Image lockedImage;

	public GameObject blueprints;

	public GameObject progress;

	public Image[] blueprintImages;

	public Vector3[] bluePrintPositions;

	public Image speedSprite;

	public Image healthSprite;

	public Image damageSprite;

	public Text blueprintLabel;

	public Image driveVehicleImage;

	public Button driveVehicleButton;

	public Image levelProgressSprite;

	public Text vehicleLevelLabel;

	public GameObject levelUpFX;

	public Material lockedVehicleMaterial;

	private float f_lastX;

	private float f_difX;

	private int i_direction = 1;

	private float f_damp = 0.96f;

	private float f_min = 0.03f;

	private float f_spinfactor = 5f;

	private bool upgradeInProgress;

	private GameObject vehicle;

	private int currentVehicleID;

	private bool mouseDownOnVehicle;

	private Vector3 mouseDownPosition = Vector3.zero;

	private Vector3 vehicleEuler = Vector3.zero;

	private Vector3 defaultVehicleRotation = new Vector3(0f, 40f, 0f);

	private float mouseDownTime;

	private float mouseClickTimeThreshold = 0.2f;

	private TopBar topBar;

	private TraversalTeaserGUI traversalTeaserGUI;

	private int oldQualityLevel;

	private void Awake()
	{
		AnalyticsEvents.LogLoadScene("Garage");
		topBar = UnityEngine.Object.FindObjectOfType<TopBar>();
		int num = 0;
		bluePrintPositions = new Vector3[blueprintImages.Length];
		Image[] array = blueprintImages;
		foreach (Image image in array)
		{
			Vector3 position = image.gameObject.transform.position;
			bluePrintPositions[num] = position;
			num++;
		}
		currentVehicleID = 0;
		if (currentVehicleID < 0)
		{
			currentVehicleID = 0;
		}
		if (PlayerProfile.newlyUnlockedVehicles.Count > 0)
		{
			currentVehicleID = PlayerProfile.newlyUnlockedVehicles[0];
			PlayerProfile.newlyUnlockedVehicles.Remove(PlayerProfile.newlyUnlockedVehicles[0]);
		}
		oldQualityLevel = QualitySettings.GetQualityLevel();
		int index = Array.IndexOf(QualitySettings.names, "Beautiful");
		QualitySettings.SetQualityLevel(index, true);
		SelectVehicle();
		previousVehicleButton.onClick.AddListener(() =>
		{
			if (!upgradeInProgress)
			{
				MyNinjaGUI.Decrement(ref currentVehicleID, VehicleDefinition.vehicleDefinitions.Count);
				SelectVehicle();
			}
		});
		nextVehicleButton.onClick.AddListener(() =>
		{
			if (!upgradeInProgress)
			{
				MyNinjaGUI.Increment(ref currentVehicleID, VehicleDefinition.vehicleDefinitions.Count);
				SelectVehicle();
			}
		});
		unlockVehicleButton.onClick.AddListener(() =>
		{
			StartCoroutine(Unlock());
		});
		upgradeVehicleButton.onClick.AddListener(() =>
		{
			StartCoroutine(LevelUp());
		});
		selectVehicleButton.onClick.AddListener(() =>
		{
		});
	}

	public void OnDestroy()
	{
		QualitySettings.SetQualityLevel(oldQualityLevel, true);
	}

	private IEnumerator Unlock()
	{
		upgradeInProgress = true;
		PlayerProfile profile = GameController.Instance.PlayerProfile;
		VehicleDefinition def = VehicleDefinition.vehicleDefinitions[currentVehicleID];
		Image[] array = blueprintImages;
		foreach (Image b in array)
		{
			if (b.enabled)
			{
				Vector3 targetPos = lockedImage.gameObject.transform.position;
				Vector3 fromPos = b.gameObject.transform.position;
				FlyBluePrint(fromPos, targetPos, 1.2f, b.gameObject);
				yield return new WaitForSeconds(0.1f);
			}
		}
		yield return new WaitForSeconds(1.1f);
		i_direction = 1;
		f_difX = 3.6f;
		vehicle.GetComponent<Vehicle>().Animate(5f);
		AudioPlayer.PlaySound("ui/button/shing");
		AudioPlayer.PlaySound("music/level/up");
		GameObject levelupFXGO = UnityEngine.Object.Instantiate(levelUpFX, vehicle.transform.position, vehicle.transform.rotation);
		ParticleSystem[] levelupFXParticles = levelupFXGO.GetComponentsInChildren<ParticleSystem>();
		upgradeInProgress = false;
		bool particlesAlive = true;
		while (particlesAlive)
		{
			particlesAlive = false;
			for (int j = 0; j < levelupFXParticles.Length; j++)
			{
				particlesAlive |= levelupFXParticles[j].IsAlive();
			}
			yield return null;
		}
		UnityEngine.Object.Destroy(levelupFXGO);
	}

	public void FlyBluePrint(Vector3 from, Vector3 to, float duration, GameObject go)
	{
		StartCoroutine(DoFly(from, to, duration, go));
	}

	private IEnumerator DoFly(Vector3 from, Vector3 to, float duration, GameObject go)
	{
		AudioPlayer.PlaySound("fx/stud/eject");
		float startTime = Time.time;
		go.transform.position = from;
		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			go.transform.position = Vector3.Lerp(from, to, t);
			yield return 0;
		}
		AudioPlayer.PlaySound("props/stud/pickup");
		go.SetActive(false);
	}

	private IEnumerator LevelUp()
	{
		upgradeInProgress = true;
		PlayerProfile profile = GameController.Instance.PlayerProfile;
		GameObject levelupFXGO = UnityEngine.Object.Instantiate(levelUpFX, vehicle.transform.position, vehicle.transform.rotation);
		ParticleSystem[] levelupFXParticles = levelupFXGO.GetComponentsInChildren<ParticleSystem>();
		SelectVehicle();
		upgradeInProgress = false;
		bool particlesAlive = true;
		while (particlesAlive)
		{
			particlesAlive = false;
			for (int i = 0; i < levelupFXParticles.Length; i++)
			{
				particlesAlive |= levelupFXParticles[i].IsAlive();
			}
			yield return null;
		}
		UnityEngine.Object.Destroy(levelupFXGO);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			previousVehicleButton.onClick.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextVehicleButton.onClick.Invoke();
		}
	}

	private void SelectVehicle()
	{
		if ((bool)vehicle)
		{
			UnityEngine.Object.Destroy(vehicle);
			vehicle = null;
		}
		unlockVehicleButton.gameObject.SetActive(false);
		upgradeVehicleButton.gameObject.SetActive(false);
		int key = currentVehicleID;
		VehicleDefinition vehicleDefinition = VehicleDefinition.vehicleDefinitions[key];
		vehicle = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Vehicles/" + vehicleDefinition.model));
		vehicle.transform.SetParent(vehicleContainer.transform, false);
		vehicle.transform.localScale = Vector3.one;
		vehicleContainer.transform.localScale = Vector3.one * vehicleDefinition.garageScale;
		Localize component = vehicleLabel.GetComponent<Localize>();
		string key2 = "VEHICLE_" + (vehicleDefinition.id + 1);
		component.SetKey(key2);
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int num = 0;
		int num2 = vehicleDefinition.blueprints;
	}
}
