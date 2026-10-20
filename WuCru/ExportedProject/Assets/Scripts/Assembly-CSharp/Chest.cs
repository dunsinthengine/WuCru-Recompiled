using System;
using System.Collections;
using Fabric;
using UnityEngine;

public class Chest : InteractableObject
{
	public enum ChestState
	{
		Closed = 0,
		Unlocking = 1,
		Opened = 2
	}

	private static Chest _walkingToChest;

	private static Bush _returnToBush;

	public string ChestOpenAudioEvent;

	public ChestType chestType;

	public ParticleSystem spawnFx;

	private bool _canDropSkillPoint;

	private bool _canDropWardrobeItem;

	private bool _canDropVehiclePart;

	private Animator animator;

	public int minStuds = 12;

	public int maxStuds = 15;

	public float bluePrintPayoutSilver = 0.15f;

	public float bluePrintPayoutGold = 0.35f;

	private int numberOfStuds = 15;

	public ChestState State;

	public static Chest _pWalkingToChest
	{
		get
		{
			return _walkingToChest;
		}
		set
		{
			_walkingToChest = value;
		}
	}

	public static Bush _pReturnToBush
	{
		get
		{
			return _returnToBush;
		}
		set
		{
			_returnToBush = value;
		}
	}

	public bool _pCanDropSkillPoint
	{
		get
		{
			return _canDropSkillPoint;
		}
		set
		{
			_canDropSkillPoint = value;
		}
	}

	public bool _pCanDropWardrobeItem
	{
		get
		{
			return _canDropWardrobeItem;
		}
		set
		{
			_canDropWardrobeItem = value;
		}
	}

	public bool _pCanDropVehiclePart
	{
		get
		{
			return _canDropVehiclePart;
		}
		set
		{
			_canDropVehiclePart = value;
		}
	}

	public override int PickingPriority()
	{
		return 11;
	}

	protected override void OnInitialize()
	{
		State = ChestState.Closed;
		isHighlightable = true;
		animator = GetComponentInChildren<Animator>();
		animator.SetBool("Triggered", false);
		numberOfStuds = UnityEngine.Random.Range(minStuds, maxStuds + 1);
		activateBlink = ActivateBlink.ON_PICK;
	}

	private void EvaluateSuccess()
	{
		HUDChestMinigame hudChestMinigame = ScreenHud._pInstance._hudChestMinigame;
		if (hudChestMinigame._pIsInSafeZone)
		{
			AudioPlayer.PlaySound("props/trap/deactivate/success", base.gameObject);
			hudChestMinigame.CancelMinigame();
			animator.SetBool("Triggered", true);
			State = ChestState.Opened;
			isHighlightable = false;
			GamePlayer.Instance.SelectedObject = null;
			AudioPlayer.PlaySound(ChestOpenAudioEvent, base.gameObject);
			if (OnInteract != null)
			{
				OnInteract(GameController.Instance.CurrentLevel, this);
			}
			StartCoroutine(SpawnStuds());
			GamePlayer.Instance.Level.ChunkDB.PickupLoot(levelNode.ObjectTypes[0].objectType);
			AnalyticsEvents.LogChestOpen(true);
			if (UnityEngine.Random.Range(0, 100) < 10)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("RandomChicken"));
				gameObject.transform.position = base.transform.position + Vector3.up;
			}
		}
		else
		{
			hudChestMinigame.CancelMinigame();
			AudioPlayer.PlaySound("props/generic/fail", base.gameObject);
			State = ChestState.Closed;
			animator.SetBool("Triggered", false);
			GamePlayer.Instance.SelectedObject = null;
			AnalyticsEvents.LogChestOpen(false);
		}
	}

	protected override void OnUpdate()
	{
		if (State != ChestState.Unlocking)
		{
			return;
		}
		HUDChestMinigame hudChestMinigame = ScreenHud._pInstance._hudChestMinigame;
		if (hudChestMinigame._pInProgress)
		{
			if (Input.GetMouseButtonDown(0))
			{
				EvaluateSuccess();
			}
		}
		else
		{
			State = ChestState.Closed;
			animator.SetBool("Triggered", false);
			GamePlayer.Instance.SelectedObject = null;
			AnalyticsEvents.LogChestOpen(false);
		}
	}

	private IEnumerator SpawnStuds()
	{
		int numObjects = GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WoodChest);
		numObjects += GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.MetalChest);
		numObjects += GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.GoldChest);
		int numPickedObjects = GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.WoodChest);
		numPickedObjects += GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.MetalChest);
		numPickedObjects += GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.GoldChest);
		ScreenHud._pInstance._poolMessagesGreenNumbers.TriggerMessage(base.gameObject.transform.position, numPickedObjects + 1 + "/" + numObjects);
		GameObject studPrefab = Resources.Load<GameObject>("Prefabs/Knob_Pickup");
		Transform parent = GameController.Instance.LevelVisualizer.levelRoot.transform;
		spawnFx.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.85f);
		if (chestType == ChestType.Gold)
		{
			GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 0.5f, Color.yellow);
		}
		for (int i = 0; i < numberOfStuds; i++)
		{
			EventManager.Instance.PostEvent("fx/stud/eject", EventAction.SetPitchProperty, 1f + 1f / ((float)(numberOfStuds - i) + 1f));
			AudioPlayer.PlaySound("fx/stud/eject");
			float angle = (float)Math.PI + ((float)i + 0.5f) * (float)Math.PI / (float)numberOfStuds;
			StartCoroutine(SpawnStudAtAngle(angle, studPrefab, parent));
			yield return new WaitForSeconds(0.1f);
		}
		if (!GameController.Instance.PlayerProfile.isSkillCoinDenied() && _pCanDropSkillPoint && GameController.Instance.PlayerProfile.TotalSkillPointsAcquired < 50)
		{
			if (!GameController.Instance.PlayerProfile.HasCurrentLevelCollectedSkillCoin())
			{
				float num = UnityEngine.Random.Range(1f, 100f);
				bool flag = GameController.Instance.PlayerProfile.isSkillCoinAutoDrop();
				if (num < 15f || flag)
				{
					GameController.Instance.PlayerProfile.enableDenySkillCoin();
					StartCoroutine(SpawnSkillCoin(parent));
				}
				else
				{
					GameController.Instance.PlayerProfile.increaseSkillCoinRate();
				}
			}
			else
			{
				Debug.Log("Cannot drop a skill point as one was already collected from this level");
			}
		}
		if (_pCanDropWardrobeItem)
		{
			StartCoroutine(SpawnWardrobeItem(parent));
		}
		PlayerProfile profile = GameController.Instance.PlayerProfile;
		if (_pCanDropVehiclePart)
		{
			StartCoroutine(SpawnBlueprint(parent));
		}
		int stamLevel = profile.StaminaLevel;
		if (stamLevel > 0 && UnityEngine.Random.value < (float)stamLevel / 20f && GamePlayer.Instance.Player.Character.healthBar.imageFill.fillAmount < 1f)
		{
			GamePlayer.Instance.Player.Character.Health += 25;
			GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 0.5f, Color.red);
		}
	}

	private IEnumerator SpawnSkillCoin(Transform parent)
	{
		GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/SkillCoinCollectable"), base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
		go.transform.parent = parent;
		SkillCoinCollectable sc = go.GetComponentInChildren<SkillCoinCollectable>();
		sc.levelNode = levelNode;
		sc.SetAttracted();
		Rigidbody rigidbody = go.AddComponent<Rigidbody>();
		Vector3 forceVector = Vector3.up * 6.5f;
		rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
		BoxCollider bc = go.GetComponent<BoxCollider>();
		bc.enabled = false;
		while (go.transform.position.y >= base.transform.position.y + 0.5f)
		{
			yield return null;
		}
		bc.enabled = true;
		UnityEngine.Object.Destroy(rigidbody);
		sc.StartAttract();
	}

	private IEnumerator SpawnWardrobeItem(Transform parent)
	{
		GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/WardrobeCollectable"), base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
		go.transform.parent = parent;
		WardrobeCollectable sc = go.GetComponentInChildren<WardrobeCollectable>();
		sc.levelNode = levelNode;
		sc.SetAttracted();
		Rigidbody rigidbody = go.AddComponent<Rigidbody>();
		Vector3 forceVector = Vector3.up * 6.5f;
		rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
		BoxCollider bc = go.GetComponent<BoxCollider>();
		bc.enabled = false;
		while (go.transform.position.y >= base.transform.position.y + 0.5f)
		{
			yield return null;
		}
		bc.enabled = true;
		UnityEngine.Object.Destroy(rigidbody);
		sc.StartAttract();
	}

	private IEnumerator SpawnBlueprint(Transform parent)
	{
		GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Blueprint"), base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
		go.transform.parent = parent;
		go.GetComponentInChildren<Blueprint>().levelNode = levelNode;
		Blueprint blueprint = go.GetComponent<Blueprint>();
		blueprint.SetAttracted();
		Rigidbody rigidbody = go.AddComponent<Rigidbody>();
		Vector3 forceVector = Vector3.up * 6.5f;
		rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
		go.GetComponent<BoxCollider>().enabled = false;
		while (go.transform.position.y >= base.transform.position.y + 0.5f)
		{
			yield return null;
		}
		go.GetComponent<BoxCollider>().enabled = true;
		UnityEngine.Object.Destroy(go.GetComponent<Rigidbody>());
		blueprint.StartAttract();
	}

	private IEnumerator SpawnStudAtAngle(float angle, GameObject prefab, Transform parent)
	{
		GameObject go = UnityEngine.Object.Instantiate(prefab, base.transform.position + new Vector3(0f, 1f, 0f) + LevelHelper.VectorForAngle(angle) * 0.5f, Quaternion.identity);
		go.transform.parent = parent;
		go.GetComponentInChildren<Studs>().levelNode = levelNode;
		Studs stud = go.GetComponent<Studs>();
		stud.SetAttracted();
		Rigidbody rigidbody = go.AddComponent<Rigidbody>();
		Vector3 forceVector = (LevelHelper.VectorForAngle(angle) + Vector3.up * 4f).normalized * 4.5f;
		rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
		go.GetComponent<BoxCollider>().enabled = false;
		while (go.transform.position.y >= base.transform.position.y + 0.5f)
		{
			yield return null;
		}
		go.GetComponent<BoxCollider>().enabled = true;
		UnityEngine.Object.Destroy(go.GetComponent<Rigidbody>());
		stud.StartAttract();
	}

	protected override void OnEnter(Character character)
	{
		if (character.isPlayer && character.Animator.GetBool(VarHash.isMoving))
		{
			character.Animator.SetBool(VarHash.isMoving, false);
			character.ResetPath();
		}
		if (State != ChestState.Closed)
		{
			return;
		}
		if (!character.IsInSpinjitzu())
		{
			GamePlayer.Instance.SelectedObject = base.gameObject;
			if ((!Input.GetMouseButtonDown(0) && !(_pWalkingToChest == this)) || !IsMouseOverObject())
			{
				return;
			}
			if (ScreenHud._pInstance._hudChestMinigame._pInProgress)
			{
				EvaluateSuccess();
				return;
			}
			ScreenHud._pInstance._hudChestMinigame.BeginGame(base.transform);
			State = ChestState.Unlocking;
			if (character.Animator.GetBool(VarHash.isMoving))
			{
				character.Animator.SetBool(VarHash.isMoving, false);
			}
			AudioPlayer.PlaySound("props/trap/hold", base.gameObject);
		}
		else
		{
			animator.SetBool("Triggered", true);
			State = ChestState.Opened;
			isHighlightable = false;
			GamePlayer.Instance.SelectedObject = null;
			AudioPlayer.PlaySound(ChestOpenAudioEvent, base.gameObject);
			if (OnInteract != null)
			{
				OnInteract(GameController.Instance.CurrentLevel, this);
			}
			StartCoroutine(SpawnStuds());
			GamePlayer.Instance.Level.ChunkDB.PickupLoot(levelNode.ObjectTypes[0].objectType);
		}
	}

	protected override void OnExit(Character character)
	{
		_walkingToChest = null;
		if (State == ChestState.Unlocking)
		{
			ScreenHud._pInstance._hudChestMinigame.CancelMinigame();
			State = ChestState.Closed;
			animator.SetBool("Triggered", false);
			if (_returnToBush != null)
			{
				_returnToBush = null;
			}
		}
		AudioPlayer.StopSound("props/trap/hold", base.gameObject);
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		_walkingToChest = null;
		if (State == ChestState.Opened)
		{
			GamePlayer.Instance.SelectedObject = null;
		}
		else if (!playerIsInTriggerZone)
		{
			_walkingToChest = this;
			character.Controller.WalkTo(levelNode, true);
		}
		else
		{
			OnEnter(character);
		}
	}

	public override bool ShouldCaptureInputWhileMouseOverObject()
	{
		return true;
	}
}
