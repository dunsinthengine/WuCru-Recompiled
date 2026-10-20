using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
	public enum DoorType
	{
		EnemyDoor = 0,
		KeyDoor = 1,
		TriggeredDoor = 2,
		ElementalDoor = 3
	}

	public enum DoorState
	{
		Open = 0,
		Closed = 1
	}

	public ElementalPower elementalPower;

	public Transform otherPiece;

	private Coroutine animateDoorCoroutine;

	private Coroutine closeDoorTimerCoroutine;

	public int keyID;

	public DoorType doorType;

	public Vector3 closedPosition;

	public Vector3 openPosition;

	public float doorSpeed = 0.1f;

	private Character openingCharacter;

	public const float CloseDoorTimeout = 8f;

	public DoorState State;

	public string DoorOpenAudioEvent;

	public string DoorCloseAudioEvent;

	public string doorWaitTickAudioEvent;

	public SpriteRenderer lockedIcon;

	public SpriteRenderer lockedIconTriggeredDoor;

	public float iconFadeSpeed = 0.1f;

	public List<GameObject> lockObjects = new List<GameObject>();

	public DoorType Type
	{
		get
		{
			return doorType;
		}
		set
		{
			doorType = value;
			onlyPlayer = value != DoorType.EnemyDoor;
		}
	}

	public override int PickingPriority()
	{
		return 15;
	}

	protected override void OnInitialize()
	{
		onlyPlayer = false;
		Transform transform = base.transform;
		closedPosition = transform.localPosition;
		float num = 0.9f;
		if (otherPiece != null)
		{
			num = 0.45f;
		}
		openPosition = transform.localPosition + num * Vector3.right;
		State = DoorState.Closed;
		isHighlightable = false;
	}

	protected override void OnEnter(Character character)
	{
		if (character == null)
		{
			Debug.LogWarning("Character is null!");
			return;
		}
		if (character.Controller == null)
		{
			Debug.LogWarning("Character controller is null");
			return;
		}
		switch (doorType)
		{
		case DoorType.EnemyDoor:
			if (!character.isPlayer)
			{
				openingCharacter = character;
				OpenDoor();
			}
			break;
		case DoorType.KeyDoor:
			if (keyID > 0 && (!character.isPlayer || (character.isPlayer && !character.HasDoorKey(keyID))) && !character.IsInSpinjitzu())
			{
				Debug.Log("Player has key? " + character.HasDoorKey(keyID));
				ShowLockIcon();
				base.HasTriggered = false;
			}
			else if (State == DoorState.Closed)
			{
				activateBlink = ActivateBlink.NEVER;
				OpenDoor();
			}
			break;
		case DoorType.TriggeredDoor:
			if (animateDoorCoroutine == null && (!character.isPlayer || base.gameObject.transform.position.x < character.transform.position.x || character.IsInSpinjitzu()))
			{
				OpenDoor();
				StartCloseDoorTimer();
			}
			else if (animateDoorCoroutine == null && State == DoorState.Closed)
			{
				ShowLockIcon();
			}
			break;
		case DoorType.ElementalDoor:
			if (character.isPlayer && State == DoorState.Closed)
			{
				Debug.Log("Todo: show HUD stuff for elemental door");
			}
			if (character.isPlayer && (elementalPower == ElementalPower.None || character.elementalPower == elementalPower) && State == DoorState.Closed)
			{
				StartCoroutine(OpenElemtalDoor());
			}
			else if (character.isPlayer && IsOnBackSideOfElementalDoor(character) && State == DoorState.Closed)
			{
				StartCoroutine(OpenElemtalDoor());
			}
			else if (State == DoorState.Closed && character.isPlayer)
			{
				ShowLockIcon();
			}
			break;
		}
	}

	private bool EdgeValueMatchesElementalPower(LevelNodeEdgeValue value)
	{
		switch (elementalPower)
		{
		case ElementalPower.Earth:
			return value == LevelNodeEdgeValue.ElementalDoor_earth;
		case ElementalPower.Electricity:
			return value == LevelNodeEdgeValue.ElementalDoor_electricity;
		case ElementalPower.Energy:
			return value == LevelNodeEdgeValue.ElementalDoor_energy;
		case ElementalPower.Fire:
			return value == LevelNodeEdgeValue.ElementalDoor_fire;
		case ElementalPower.Ice:
			return value == LevelNodeEdgeValue.ElementalDoor_ice;
		case ElementalPower.Water:
			return value == LevelNodeEdgeValue.ElementalDoor_water;
		default:
			return false;
		}
	}

	private bool IsOnBackSideOfElementalDoor(Character character)
	{
		PathFinding pathFinding = new PathFinding(GameController.Instance.CurrentLevel.Nodes);
		LevelNode start = GameController.Instance.CurrentLevel.Nodes[GameController.Instance.CurrentLevel.StartPoint.row, GameController.Instance.CurrentLevel.StartPoint.col];
		LevelNode goal = GameController.Instance.CurrentLevel.Nodes[levelNode.Row + 1, levelNode.Col];
		LevelNode goal2 = levelNode;
		bool flag = pathFinding.GetPathIgnoreDoors(start, goal, null).Length < pathFinding.GetPathIgnoreDoors(start, goal2, null).Length;
		if (flag && character.CurrentPositionNode.Row <= levelNode.Row)
		{
			return true;
		}
		if (!flag && character.CurrentPositionNode.Row > levelNode.Row)
		{
			return true;
		}
		return false;
	}

	private IEnumerator OpenElemtalDoor()
	{
		base.gameObject.GetComponentInChildren<Animator>().SetTrigger("open");
		yield return new WaitForSeconds(1f);
		State = DoorState.Open;
		Collider c = GetComponent<Collider>();
		if (c != null)
		{
			c.enabled = false;
		}
		yield return 0;
	}

	public void ShowLockIcon()
	{
		if (doorType == DoorType.KeyDoor)
		{
			ScreenHud._pInstance._keyIconTracker.DisplayIcon(base.transform);
		}
		if (doorType == DoorType.TriggeredDoor)
		{
			ScreenHud._pInstance._switchIconTracker.DisplayIcon(base.transform);
		}
		if (doorType == DoorType.ElementalDoor)
		{
			ScreenHud._pInstance._hudElementDisplay.DisplayIcon(elementalPower, base.transform);
		}
	}

	public void HideLockIcon()
	{
		if (doorType == DoorType.KeyDoor)
		{
			ScreenHud._pInstance._keyIconTracker.DismissIcon(base.transform);
		}
		if (doorType == DoorType.TriggeredDoor)
		{
			ScreenHud._pInstance._switchIconTracker.DismissIcon(base.transform);
		}
		if (doorType == DoorType.ElementalDoor)
		{
			ScreenHud._pInstance._hudElementDisplay.DismissIcon(base.transform);
		}
	}

	public void OpenDoor()
	{
		if (closeDoorTimerCoroutine != null)
		{
			StopCoroutine(closeDoorTimerCoroutine);
			closeDoorTimerCoroutine = null;
		}
		if (animateDoorCoroutine != null)
		{
			StopCoroutine(animateDoorCoroutine);
			animateDoorCoroutine = null;
		}
		animateDoorCoroutine = StartCoroutine(AnimateDoor(openPosition, DoorState.Open));
	}

	public void StartCloseDoorTimer()
	{
		closeDoorTimerCoroutine = StartCoroutine(CloseDoorTimer(8f));
	}

	protected override void OnExit(Character character)
	{
		if (doorType == DoorType.EnemyDoor && character == openingCharacter)
		{
			StartCloseDoorTimer();
		}
		if (doorType == DoorType.KeyDoor || doorType == DoorType.TriggeredDoor || doorType == DoorType.ElementalDoor)
		{
			HideLockIcon();
		}
	}

	private IEnumerator AnimateDoor(Vector3 position, DoorState nextDoorState)
	{
		Debug.Log("Animating door");
		AudioPlayer.PlaySound((nextDoorState != DoorState.Closed) ? DoorOpenAudioEvent : DoorCloseAudioEvent, base.gameObject);
		foreach (GameObject lockObject in lockObjects)
		{
			lockObject.SetActive(false);
		}
		while (true)
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 value = position - localPosition;
			float num = doorSpeed * Time.deltaTime;
			if (value.sqrMagnitude < num * num)
			{
				break;
			}
			base.transform.localPosition = localPosition + Vector3.Normalize(value) * num;
			if (otherPiece != null)
			{
				Vector3 localPosition2 = otherPiece.transform.position - (base.transform.position - position);
				Vector3 localPosition3 = otherPiece.localPosition;
				Vector3 value2 = position - localPosition3;
				float num2 = doorSpeed * Time.deltaTime;
				if (value2.sqrMagnitude < num2 * num2)
				{
					otherPiece.localPosition = localPosition2;
				}
				else
				{
					otherPiece.localPosition = localPosition3 - Vector3.Normalize(value2) * num2;
				}
			}
			yield return null;
		}
		base.transform.localPosition = position;
		State = nextDoorState;
		animateDoorCoroutine = null;
		Debug.Log("Done Animating door");
	}

	public static IEnumerator Wait(float duration)
	{
		float startTime = Time.time;
		float endTime = startTime + duration;
		while (Time.time < endTime)
		{
			yield return null;
		}
	}

	private IEnumerator CloseDoorTimer(float t)
	{
		ScreenHud._pInstance._doorIconTracker.DisplayIcon(base.transform);
		Debug.Log("Closing door in " + t + " seconds");
		StartCoroutine(PlayDoorWaitAudioTick(1f, (int)t));
		yield return StartCoroutine(Wait(t));
		if (animateDoorCoroutine != null)
		{
			StopCoroutine(animateDoorCoroutine);
			animateDoorCoroutine = null;
		}
		animateDoorCoroutine = StartCoroutine(AnimateDoor(closedPosition, DoorState.Closed));
		closeDoorTimerCoroutine = null;
		ScreenHud._pInstance._doorIconTracker.DismissIcon(base.transform);
	}

	public override void OnPicked(Character character, bool tapDown)
	{
	}

	public static bool IsDoorEdgeValue(LevelNodeEdgeValue value)
	{
		if (value == LevelNodeEdgeValue.KeyDoor || value == LevelNodeEdgeValue.AltKeyDoor || value == LevelNodeEdgeValue.TriggeredDoor || value == LevelNodeEdgeValue.EnemyDoor || value == LevelNodeEdgeValue.ElementalDoor_earth || value == LevelNodeEdgeValue.ElementalDoor_electricity || value == LevelNodeEdgeValue.ElementalDoor_energy || value == LevelNodeEdgeValue.ElementalDoor_fire || value == LevelNodeEdgeValue.ElementalDoor_ice || value == LevelNodeEdgeValue.ElementalDoor_water)
		{
			return true;
		}
		return false;
	}

	private IEnumerator PlayDoorWaitAudioTick(float rate, int times)
	{
		int count = 0;
		while (count < times)
		{
			AudioPlayer.PlaySound(doorWaitTickAudioEvent, base.gameObject);
			count++;
			yield return new WaitForSeconds(rate);
		}
	}
}
