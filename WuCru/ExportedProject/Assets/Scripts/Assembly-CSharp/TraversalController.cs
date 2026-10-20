using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TraversalController : MonoBehaviour
{
	public enum DecorationType
	{
		Near = 0,
		Far = 1,
		VeryFar = 2
	}

	private static TraversalController instance;

	public GameObject car;

	public GameObject carRabbit;

	public GameObject missile;

	public GameObject missileExplosion;

	public GameObject vehicleExplosion;

	public GameObject vehicleTokenPrefab;

	public GameObject bricksCollapse;

	private Vehicle playerVehicle;

	private float missilerate;

	public AnimationCurve inputMovementSpeed;

	public Camera traversalCamera;

	private InGameCam inGameCam;

	public GameObject world;

	public float segmentSize = 10f;

	public int segmentCount = 150;

	public int overflowSegments = 15;

	private float fadeTime = 1.2f;

	public Vector3 carVelocity;

	public float vehicleBaseSpeed = 30f;

	public Vector3 vehicleBaseDirection = new Vector3(-1f, 0f, 0f);

	private List<Vehicle> enemies = new List<Vehicle>();

	private LevelStyle style;

	private Plane inputPlane = new Plane(Vector3.back, Vector3.zero);

	private float lastMouseDownTime;

	private float lastMissileFireTime;

	public float tapThreshold = 0.12f;

	public Image flashImage;

	public Vector3 carSpawnPosition = new Vector3(10f, 0f, 3f);

	public Vector3 planeSpawnPosition = new Vector3(10f, 18f, 3f);

	public float movementThreshold = 2f;

	public float playerMissileSpeed = 50f;

	public float playerMissileAcceleration;

	public float enemyMissileSpeed;

	public float enemyMissileAcceleration = 100f;

	public float enemyAttackDelayMin = 1f;

	public float enemyAttackDelayMax = 4f;

	public float enemyHoverTimeMin = 2.6f;

	public float enemyHoverTimeMax = 3.6f;

	public float enemyHoverSpeed = 10f;

	public float enemyMissileDelay = 0.5f;

	public float enemyMissileMisfireAngle = 10f;

	public float enemyMissileMisfireChance = 0.8f;

	public float missileImpactStrength = 0.4f;

	public float missileImpactDuration = 0.4f;

	public float playerImpactStrength = 0.6f;

	public float playerImpactDuration = 0.6f;

	public float playerImpactFlashAlpha = 0.5f;

	public float playerImpactFlashDuration = 0.2f;

	private int vehicleID = -1;

	private bool carAccelerateSoundPlayed;

	private bool carDeaccelerateSoundPlayed;

	[Header("Audio")]
	[SerializeField]
	private string traversalMusicAudioEvent;

	[SerializeField]
	private string vehicleExplosionAudioEvent;

	[SerializeField]
	private string playerShootAudioEvent;

	[SerializeField]
	private string playerVehicleDrivingAudioEvent;

	[SerializeField]
	private string playerVehicleAccelerateAudioEvent;

	[SerializeField]
	private string playerVehicleDeaccelerateAudioEvent;

	[SerializeField]
	private string playerMissileTrailAudioEvent = "player/traversal/missile/trail";

	private bool screenFadeOutStarted;

	public static TraversalController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<TraversalController>();
			}
			if (instance == null)
			{
				Debug.LogError("There must be an object of typer 'TraversalController' in the scene!");
			}
			return instance;
		}
	}

	private void Start()
	{
		UnityEngine.Random.seed = GameController.Instance.areaMapMetaLayer.GetSeedFromPosition(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol);
		int styleIndex = GameController.Instance.areaMapMetaLayer.GetNodeAt(GameController.Instance.areaMapMetaLayer.currentRow, GameController.Instance.areaMapMetaLayer.currentCol).styleIndex;
		style = GameController.Instance.styleDB._styles[styleIndex];
		if (style.environmentSettings.Count > 0)
		{
			UnityEngine.Random.seed = GameController.Instance.PlayerProfile.TotalLevelsPlayed;
			int index = UnityEngine.Random.Range(0, style.environmentSettings.Count - 1);
			GameObject gameObject = style.environmentSettings[index];
			EnvironmentSetting component = gameObject.GetComponent<EnvironmentSetting>();
			string environmentName = string.Empty;
			component.Setup(world, ref environmentName);
		}
		else
		{
			Debug.LogError("Style does not have any EnvironmentSettings - no lighting or colors set!");
		}
		vehicleID = 0;
		if (vehicleID == -1)
		{
			vehicleID = 0;
		}
		VehicleDefinition vehicleDefinition = VehicleDefinition.vehicleDefinitions[vehicleID];
		car = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Vehicles/" + vehicleDefinition.model));
		if (vehicleDefinition.type == VehicleType.Car)
		{
			car.transform.localPosition = carSpawnPosition;
		}
		if (vehicleDefinition.type == VehicleType.Plane)
		{
			car.transform.localPosition = planeSpawnPosition;
		}
		car.GetComponent<Vehicle>().AnimationSpeed = 1f;
		car.transform.localScale = Vector3.one * vehicleDefinition.traversalScale;
		carRabbit.transform.position = carSpawnPosition;
		AudioPlayer.PlaySound(playerVehicleDrivingAudioEvent, car);
		int num = 0;
		vehicleBaseSpeed = vehicleDefinition.speed[num];
		playerVehicle = car.GetComponent<Vehicle>();
		playerVehicle.id = vehicleID;
		for (int i = 0; i < playerVehicle.smokeEmitters.Length; i++)
		{
			if (!playerVehicle.smokePartileSystem)
			{
				break;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(playerVehicle.smokePartileSystem);
			gameObject2.transform.SetParent(playerVehicle.smokeEmitters[i], false);
		}
		missilerate = vehicleDefinition.missilerate;
		inGameCam = traversalCamera.GetComponent<InGameCam>();
		inGameCam.Target = carRabbit.transform;
		inGameCam.SetState(InGameCamState.StartingTraversal);
		StartCoroutine(ChangeCamState());
		playerVehicle.health = vehicleDefinition.health[num];
		BuildRoadMesh();
		BuildDecoMesh();
		BuildTrees();
		BuildEndGate();
		for (float num2 = -2f * segmentSize; num2 < (float)segmentCount * segmentSize; num2 += UnityEngine.Random.Range(35f, 70f))
		{
			InstantiateDecorationMesh(DecorationType.Near, num2);
		}
		for (float num3 = -2f * segmentSize; num3 < (float)segmentCount * segmentSize; num3 += UnityEngine.Random.Range(8f, 15f))
		{
			InstantiateDecorationMesh(DecorationType.Far, num3);
		}
		for (float num4 = -2f * segmentSize; num4 < (float)segmentCount * segmentSize; num4 += UnityEngine.Random.Range(30f, 60f))
		{
			InstantiateDecorationMesh(DecorationType.VeryFar, num4);
		}
		int num5 = 4;
		for (float num6 = 35f * segmentSize; num6 < (float)segmentCount * segmentSize; num6 += UnityEngine.Random.Range(60f, 100f))
		{
			VehicleDefinition vehicleDefinition2 = VehicleDefinition.vehicleDefinitions[num5];
			GameObject gameObject3 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Vehicles/" + vehicleDefinition2.model), Vector3.right * num6 + Vector3.forward * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(15f, 50f), Quaternion.Euler(0f, 180f, -10f));
			gameObject3.AddComponent<EnemyCopter>();
			Vehicle component2 = gameObject3.GetComponent<Vehicle>();
			component2.isEnemy = true;
			component2.id = num5;
			gameObject3.transform.SetParent(world.transform, true);
			gameObject3.transform.localScale = Vector3.one * vehicleDefinition2.traversalScale * 1.3f;
			enemies.Add(component2);
			component2.health = 1;
		}
		AudioPlayer.MoveFabricAudioListener(FabricManager.Instance.gameObject, Camera.main.gameObject);
		AudioPlayer.PlaySound(traversalMusicAudioEvent);
		DoScreenFadeIn();
	}

	private IEnumerator ChangeCamState()
	{
		yield return new WaitForSeconds(2f);
		inGameCam.SetState(InGameCamState.Traversal);
	}

	private void BuildDecoMesh()
	{
		if (style.decorationAddOnsColored.Count > 0)
		{
			for (float num = -2f * segmentSize; num < (float)segmentCount * segmentSize; num += UnityEngine.Random.Range(50f, 80f))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(style.decorationAddOnsColored[UnityEngine.Random.Range(0, style.decorationAddOnsColored.Count)], Vector3.right * num + Vector3.forward * UnityEngine.Random.Range(-5f, -3f) + Vector3.up * UnityEngine.Random.Range(0f, 0f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
				gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(4f, 6f);
				gameObject.transform.SetParent(world.transform, true);
			}
			for (float num2 = -2f * segmentSize; num2 < (float)segmentCount * segmentSize; num2 += UnityEngine.Random.Range(50f, 80f))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(style.decorationAddOnsColored[UnityEngine.Random.Range(0, style.decorationAddOnsColored.Count)], Vector3.right * num2 + Vector3.forward * UnityEngine.Random.Range(7f, 9f) + Vector3.up * UnityEngine.Random.Range(0f, 0f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
				gameObject2.transform.localScale = gameObject2.transform.localScale * UnityEngine.Random.Range(4f, 6f);
				gameObject2.transform.SetParent(world.transform, true);
			}
		}
	}

	private void BuildTrees()
	{
		if (style.treePrefabs.Count <= 0)
		{
			return;
		}
		for (float num = -2f * segmentSize; num < (float)segmentCount * segmentSize; num += UnityEngine.Random.Range(24f, 60f))
		{
			float num2 = 0.5f;
			GameObject gameObject = UnityEngine.Object.Instantiate(style.treePrefabs[UnityEngine.Random.Range(0, style.treePrefabs.Count)], Vector3.right * num + Vector3.forward * UnityEngine.Random.Range(7f, 9f) + Vector3.up * UnityEngine.Random.Range(-2f, -1f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
			gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(2f, 3f);
			gameObject.transform.SetParent(world.transform, true);
			UnityEngine.Object.Destroy(gameObject.GetComponentInChildren<Collider>());
			if (num2 > UnityEngine.Random.Range(0f, 1f))
			{
				num += UnityEngine.Random.Range(4f, 6f);
				gameObject = UnityEngine.Object.Instantiate(style.treePrefabs[UnityEngine.Random.Range(0, style.treePrefabs.Count)], Vector3.right * num + Vector3.forward * UnityEngine.Random.Range(7f, 9f) + Vector3.up * UnityEngine.Random.Range(-2f, -1f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
				gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(2f, 3f);
				gameObject.transform.SetParent(world.transform, true);
				UnityEngine.Object.Destroy(gameObject.GetComponentInChildren<Collider>());
			}
			num2 = 0.3f;
			if (num2 > UnityEngine.Random.Range(0f, 1f))
			{
				num += UnityEngine.Random.Range(4f, 6f);
				gameObject = UnityEngine.Object.Instantiate(style.treePrefabs[UnityEngine.Random.Range(0, style.treePrefabs.Count)], Vector3.right * num + Vector3.forward * UnityEngine.Random.Range(7f, 9f) + Vector3.up * UnityEngine.Random.Range(-2f, -1f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
				gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(2f, 3f);
				gameObject.transform.SetParent(world.transform, true);
				UnityEngine.Object.Destroy(gameObject.GetComponentInChildren<Collider>());
			}
		}
		for (float num3 = -2f * segmentSize; num3 < (float)segmentCount * segmentSize; num3 += UnityEngine.Random.Range(24f, 60f))
		{
			float num4 = 0.5f;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(style.treePrefabs[UnityEngine.Random.Range(0, style.treePrefabs.Count)], Vector3.right * num3 + Vector3.back * UnityEngine.Random.Range(3f, 4.5f) + Vector3.up * UnityEngine.Random.Range(-2f, -1f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
			gameObject2.transform.localScale = gameObject2.transform.localScale * UnityEngine.Random.Range(2f, 3f);
			gameObject2.transform.SetParent(world.transform, true);
			UnityEngine.Object.Destroy(gameObject2.GetComponentInChildren<Collider>());
			if (num4 > UnityEngine.Random.Range(0f, 1f))
			{
				num3 += UnityEngine.Random.Range(4f, 6f);
				gameObject2 = UnityEngine.Object.Instantiate(style.treePrefabs[UnityEngine.Random.Range(0, style.treePrefabs.Count)], Vector3.right * num3 + Vector3.forward * UnityEngine.Random.Range(7f, 9f) + Vector3.up * UnityEngine.Random.Range(-2f, -1f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
				gameObject2.transform.localScale = gameObject2.transform.localScale * UnityEngine.Random.Range(2f, 3f);
				gameObject2.transform.SetParent(world.transform, true);
				UnityEngine.Object.Destroy(gameObject2.GetComponentInChildren<Collider>());
			}
			num4 = 0.3f;
			if (num4 > UnityEngine.Random.Range(0f, 1f))
			{
				num3 += UnityEngine.Random.Range(4f, 6f);
				gameObject2 = UnityEngine.Object.Instantiate(style.treePrefabs[UnityEngine.Random.Range(0, style.treePrefabs.Count)], Vector3.right * num3 + Vector3.forward * UnityEngine.Random.Range(7f, 9f) + Vector3.up * UnityEngine.Random.Range(-2f, -1f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
				gameObject2.transform.localScale = gameObject2.transform.localScale * UnityEngine.Random.Range(2f, 3f);
				gameObject2.transform.SetParent(world.transform, true);
				UnityEngine.Object.Destroy(gameObject2.GetComponentInChildren<Collider>());
			}
		}
	}

	private void BuildEndGate()
	{
		float num = segmentSize * (float)(segmentCount - overflowSegments);
		if (style.endLevelObjectPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/EndFlag"), Vector3.right * num + Vector3.forward + Vector3.up, Quaternion.Euler(0f, 90f, 0f)) as GameObject;
			gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(6f, 6f);
			gameObject.transform.SetParent(world.transform, true);
			UnityEngine.Object.Destroy(gameObject.GetComponentInChildren<EndFlag>());
			UnityEngine.Object.Destroy(gameObject.GetComponentInChildren<Collider>());
		}
	}

	private void BuildRoadMesh(float size = 15f, float heightVariance = 0.3f, float depthVariance = 0.3f, float colorVariance = 0.1f)
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Color> list3 = new List<Color>();
		List<int> list4 = new List<int>();
		for (int i = 0; i <= segmentCount; i++)
		{
			for (int j = 0; j <= 2; j++)
			{
				list.Add(new Vector3((float)i * size, (j != 2) ? UnityEngine.Random.Range(0f - heightVariance, heightVariance) : (-15f), (j != 2) ? ((0f - (float)j) * size + UnityEngine.Random.Range(0f - depthVariance, depthVariance)) : (0f - ((float)j - 1f) * size)));
				list2.Add((j != 2) ? Vector3.up : Vector3.back);
				if (j == 2)
				{
					list3.Add(style.sideColor * (1f + UnityEngine.Random.Range(0f - colorVariance, colorVariance)));
				}
				else
				{
					list3.Add(style.floorColors[0] * (1f + UnityEngine.Random.Range(0f - colorVariance, colorVariance)));
				}
			}
		}
		int num = 0;
		for (int k = 0; k < list.Count / 3 - 1; k++)
		{
			list4.Add(num);
			list4.Add(num + 3);
			list4.Add(num + 4);
			list4.Add(num + 4);
			list4.Add(num + 1);
			list4.Add(num);
			list4.Add(num + 1);
			list4.Add(num + 4);
			list4.Add(num + 5);
			list4.Add(num + 5);
			list4.Add(num + 2);
			list4.Add(num + 1);
			num += 3;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.normals = list2.ToArray();
		mesh.triangles = list4.ToArray();
		mesh.colors = list3.ToArray();
		mesh.RecalculateBounds();
		GameObject gameObject = new GameObject();
		gameObject.name = "RoadMesh";
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = false;
		meshCollider.isTrigger = false;
		gameObject.transform.position = new Vector3(0f, 0f, 10f);
	}

	private void InstantiateDecorationMesh(DecorationType type, float v)
	{
		GameObject gameObject = null;
		switch (type)
		{
		case DecorationType.Near:
			gameObject = UnityEngine.Object.Instantiate(style.decorationInsertsFree[UnityEngine.Random.Range(0, style.decorationInsertsFree.Count)], Vector3.right * v + Vector3.forward * UnityEngine.Random.Range(-15f, -10f) + Vector3.up * UnityEngine.Random.Range(2f, 4f), Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f));
			gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(10f, 11f);
			gameObject.transform.position += new Vector3(0f, -100f, 0f);
			gameObject.transform.SetParent(world.transform, true);
			break;
		case DecorationType.Far:
			gameObject = UnityEngine.Object.Instantiate(style.decorationInsertsFree[UnityEngine.Random.Range(0, style.decorationInsertsFree.Count)], Vector3.right * v + Vector3.forward * UnityEngine.Random.Range(15f, 35f) + Vector3.up * UnityEngine.Random.Range(20f, 30f), Quaternion.Euler(0f, UnityEngine.Random.Range(-20f, 20f), 0f));
			gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(10f, 11f);
			gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(1.5f, 1f, 1f));
			gameObject.transform.position += new Vector3(0f, -100f, 0f);
			gameObject.transform.SetParent(world.transform, true);
			break;
		case DecorationType.VeryFar:
			gameObject = UnityEngine.Object.Instantiate(style.decorationInsertsFree[UnityEngine.Random.Range(0, style.decorationInsertsFree.Count)], Vector3.right * v + Vector3.forward * UnityEngine.Random.Range(40f, 60f) + Vector3.up * UnityEngine.Random.Range(-3f, -2f), Quaternion.Euler(0f, UnityEngine.Random.Range(-20f, 20f), 0f));
			gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(15f, 18f);
			gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(2.5f, 1f, 1f));
			gameObject.transform.position += new Vector3(0f, -100f, 0f);
			gameObject.transform.SetParent(world.transform, true);
			break;
		}
		MeshFilter componentInChildren = gameObject.GetComponentInChildren<MeshFilter>();
		MeshRenderer componentInChildren2 = gameObject.GetComponentInChildren<MeshRenderer>();
		Color[] array = new Color[componentInChildren.mesh.vertices.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = style.sideColor;
		}
		componentInChildren.mesh.colors = array;
		componentInChildren2.material = new Material(Shader.Find(LevelMeshBuilder.shaderName));
		componentInChildren2.receiveShadows = false;
		componentInChildren2.shadowCastingMode = ShadowCastingMode.Off;
	}

	private void Update()
	{
		Vector3 vector = Vector3.zero;
		float num = 0f;
		if ((bool)car)
		{
			Ray ray = traversalCamera.ScreenPointToRay(Input.mousePosition);
			float enter;
			inputPlane.Raycast(ray, out enter);
			Vector3 point = ray.GetPoint(enter);
			Vector3 forward = point - car.transform.position;
			VehicleDefinition vehicleDefinition = VehicleDefinition.vehicleDefinitions[vehicleID];
			if (Input.GetMouseButtonDown(0))
			{
				lastMouseDownTime = Time.timeSinceLevelLoad;
			}
			else if (Input.GetMouseButton(0) && Time.timeSinceLevelLoad - lastMouseDownTime >= tapThreshold)
			{
				num = ((vehicleDefinition.type != VehicleType.Car) ? inputMovementSpeed.Evaluate(forward.magnitude) : inputMovementSpeed.Evaluate(Mathf.Abs(forward.x)));
				if (vehicleDefinition.type == VehicleType.Car && forward.x < 0f - movementThreshold)
				{
					vector = Vector3.left;
				}
				if (vehicleDefinition.type == VehicleType.Car && forward.x > movementThreshold)
				{
					vector = Vector3.right;
				}
				if (vehicleDefinition.type == VehicleType.Plane && forward.sqrMagnitude > movementThreshold * movementThreshold && (forward.y >= 0f || car.transform.position.y > 0f))
				{
					vector = forward.normalized;
				}
			}
			carVelocity = vehicleBaseDirection * vehicleBaseSpeed + vector * num;
			car.transform.Translate(carVelocity * Time.deltaTime);
			if (vector == Vector3.left && !carDeaccelerateSoundPlayed && carVelocity.magnitude > 15f)
			{
				carDeaccelerateSoundPlayed = true;
				carAccelerateSoundPlayed = false;
				AudioPlayer.PlaySound(playerVehicleDeaccelerateAudioEvent, car);
			}
			if (vector == Vector3.right && !carAccelerateSoundPlayed && carVelocity.magnitude > 40f)
			{
				carDeaccelerateSoundPlayed = false;
				carAccelerateSoundPlayed = true;
				AudioPlayer.PlaySound(playerVehicleAccelerateAudioEvent, car);
			}
			if (Input.GetMouseButtonUp(0) && Time.timeSinceLevelLoad - lastMouseDownTime < tapThreshold && Time.timeSinceLevelLoad - lastMissileFireTime > missilerate)
			{
				int num2 = 0;
				GameObject gameObject = UnityEngine.Object.Instantiate(missile, playerVehicle.missileSpawnPoint, Quaternion.LookRotation(forward, Vector3.up));
				gameObject.SetActive(true);
				TraversalMissile component = gameObject.GetComponent<TraversalMissile>();
				component.baseVelocity = carVelocity;
				component.owner = playerVehicle;
				component.speed = playerMissileSpeed;
				component.acceleration = playerMissileAcceleration;
				component.damage = vehicleDefinition.damage[num2];
				AudioPlayer.PlaySound(playerShootAudioEvent, car.gameObject);
				AudioPlayer.PlaySound(playerMissileTrailAudioEvent, component.gameObject);
				for (int i = 0; i < playerVehicle.muzzleFlashEmitters.Length; i++)
				{
					if ((bool)playerVehicle.muzzleFlashParticleSystem)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(playerVehicle.muzzleFlashParticleSystem);
						gameObject2.transform.SetParent(playerVehicle.muzzleFlashEmitters[i], false);
						gameObject2.transform.localPosition = Vector3.zero;
					}
				}
				lastMissileFireTime = Time.timeSinceLevelLoad;
				Debug.Log("Player Pew!");
			}
			if (traversalCamera.transform.position.x > segmentSize * (float)(segmentCount - overflowSegments))
			{
				StartCoroutine(OnPlayerCompletedLevel());
			}
		}
		carRabbit.transform.Translate(vehicleBaseDirection * vehicleBaseSpeed * Time.deltaTime);
	}

	public void OnMissileHit(TraversalMissile missile, Collider other)
	{
		TraversalMissile component = other.GetComponent<TraversalMissile>();
		Vehicle component2 = other.GetComponent<Vehicle>();
		if (((bool)component && component.owner.isEnemy) || component2 == missile.owner || ((bool)component2 && component2.isEnemy && missile.owner.isEnemy))
		{
			return;
		}
		bool flag = false;
		float num = 0f;
		float num2 = 0f;
		if ((bool)component2 || missile.owner.isEnemy)
		{
			missile.Explode();
			if (missile.IsVisible())
			{
				UnityEngine.Object.Instantiate(missileExplosion, missile.transform.position, Quaternion.identity);
				flag = true;
				num = missileImpactStrength;
				num2 = missileImpactDuration;
			}
		}
		if ((bool)component2)
		{
			component2.health -= missile.damage;
			if (component2.health <= 0)
			{
				UnityEngine.Object.Instantiate(vehicleExplosion, component2.transform.position, Quaternion.identity);
				GameObject gameObject = UnityEngine.Object.Instantiate(bricksCollapse, component2.transform.position, UnityEngine.Random.rotation);
				if (!component2.isEnemy)
				{
					StartCoroutine(OnPlayerDied());
					Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
					foreach (Rigidbody rigidbody in componentsInChildren)
					{
						Vector3 force = 0.5f * vehicleBaseSpeed * vehicleBaseDirection;
						rigidbody.AddForce(force, ForceMode.VelocityChange);
					}
				}
				else
				{
					Vector3 velocity = component2.GetComponent<EnemyCopter>().velocity;
					Rigidbody[] componentsInChildren2 = gameObject.GetComponentsInChildren<Rigidbody>();
					foreach (Rigidbody rigidbody2 in componentsInChildren2)
					{
						rigidbody2.AddForce(0.7f * velocity + Vector3.down * 20f, ForceMode.VelocityChange);
					}
					GameObject gameObject2 = UnityEngine.Object.Instantiate(vehicleTokenPrefab, component2.transform.position, Quaternion.identity);
				}
				AudioPlayer.PlaySound(vehicleExplosionAudioEvent, component2.gameObject);
				UnityEngine.Object.Destroy(component2.gameObject);
			}
			else
			{
				component2.StartCoroutine(VehicleFlash(component2));
			}
			if (!component2.isEnemy)
			{
				StartCoroutine(ScreenFlash(playerImpactFlashAlpha, (playerImpactFlashDuration != 0f) ? (1f / playerImpactFlashDuration) : float.MaxValue));
			}
			flag = true;
			num = playerImpactStrength;
			num2 = playerImpactDuration;
		}
		if (flag)
		{
			inGameCam.Shake(missileImpactStrength, missileImpactDuration);
			Debug.Log("Shake!", missile.gameObject);
		}
	}

	public void OnMissileTimeout(TraversalMissile missile)
	{
		UnityEngine.Object.Destroy(missile.gameObject);
	}

	public IEnumerator OnPlayerCompletedLevel(bool immediate = false)
	{
		AudioPlayer.StopSound(traversalMusicAudioEvent);
		AudioPlayer.StopSound(playerVehicleDrivingAudioEvent, car);
		DoScreenFadeOut();
		if (!immediate)
		{
			yield return new WaitForSeconds(fadeTime + 1f);
		}
		AudioPlayer.MoveFabricAudioListener(Camera.main.gameObject, FabricManager.Instance.gameObject);
		SceneManager.LoadScene("LevelPlayerNew");
	}

	public void DoScreenFadeOut()
	{
		if (!screenFadeOutStarted)
		{
			screenFadeOutStarted = true;
			ScreenFadeFX component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
			component.OnDone = (ScreenFadeFX.OnDoneDelegate)Delegate.Combine(component.OnDone, new ScreenFadeFX.OnDoneDelegate(OnFadeOutDone));
			component.FadeOut(fadeTime);
		}
	}

	public void DoScreenFadeIn()
	{
		ScreenFadeFX component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenFadeFX"))).GetComponent<ScreenFadeFX>();
		component.FadeIn(fadeTime);
	}

	private void OnFadeOutDone()
	{
		UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/ScreenBlockerFX"));
	}

	public IEnumerator OnPlayerDied()
	{
		AudioPlayer.StopSound(traversalMusicAudioEvent);
		AudioPlayer.StopSound(playerVehicleDrivingAudioEvent, car);
		yield return new WaitForSeconds(1.5f);
		DoScreenFadeOut();
		yield return new WaitForSeconds(fadeTime + 1f);
		AudioPlayer.MoveFabricAudioListener(Camera.main.gameObject, FabricManager.Instance.gameObject);
		SceneManager.LoadScene("AreaMap");
	}

	private IEnumerator ScreenFlash(float alpha, float speed)
	{
		flashImage.gameObject.SetActive(true);
		Color color = flashImage.color;
		color.a = alpha;
		flashImage.color = color;
		while (alpha >= 0f)
		{
			color.a = alpha;
			flashImage.color = color;
			alpha -= speed * Time.deltaTime;
			yield return null;
		}
		flashImage.gameObject.SetActive(false);
	}

	private IEnumerator VehicleFlash(Vehicle target)
	{
		SkinnedMeshRenderer[] componentsInChildren = target.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			if (skinnedMeshRenderer != null)
			{
				skinnedMeshRenderer.material.SetFloat("_BlinkSwitch", 1f);
			}
			else
			{
				Debug.LogWarning("SkinnedMeshRenderer is null!");
			}
		}
		MeshRenderer[] componentsInChildren2 = target.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren2)
		{
			if (meshRenderer != null)
			{
				meshRenderer.material.SetFloat("_BlinkSwitch", 1f);
			}
			else
			{
				Debug.LogWarning("MeshRenderer is null!");
			}
		}
		yield return new WaitForSeconds(0.5f);
		SkinnedMeshRenderer[] componentsInChildren3 = target.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren3)
		{
			if (skinnedMeshRenderer2 != null)
			{
				skinnedMeshRenderer2.material.SetFloat("_BlinkSwitch", 0f);
			}
			else
			{
				Debug.LogWarning("SkinnedMeshRenderer is null!");
			}
		}
		MeshRenderer[] componentsInChildren4 = target.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer2 in componentsInChildren4)
		{
			if (meshRenderer2 != null)
			{
				meshRenderer2.material.SetFloat("_BlinkSwitch", 0f);
			}
			else
			{
				Debug.LogWarning("MeshRenderer is null!");
			}
		}
	}
}
