using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	public enum ActivateBlink
	{
		NEVER = 0,
		ON_PICK = 1,
		ON_GOTO_NOTE = 2
	}

	public delegate void OnInteractDelegate(Level level, InteractableObject instance);

	public LevelNode levelNode;

	public bool onlyPlayer = true;

	public float interactDistance = 5f;

	public int interactMaxElevationDif;

	public int interactMinElevationDif;

	private List<string> objectTags = new List<string>();

	protected float distToPlayer = float.MaxValue;

	protected float eDifToPlayer;

	private bool isInteractable;

	private bool hasEmptyParent;

	private bool hasTriggered;

	protected MeshRenderer[] renderers;

	protected string spawnTypePrefabName = string.Empty;

	public OnInteractDelegate OnInteract;

	protected bool isHighlightable = true;

	public ActivateBlink activateBlink;

	protected bool doActivateBlink;

	private float activateBlinkTimer;

	private float outlineTimer;

	protected bool playerIsInTriggerZone;

	public bool HasTriggered
	{
		get
		{
			return hasTriggered;
		}
		set
		{
			hasTriggered = value;
		}
	}

	public bool IsInteractable
	{
		get
		{
			return isInteractable;
		}
	}

	protected abstract void OnEnter(Character character);

	protected abstract void OnExit(Character character);

	public abstract void OnPicked(Character character, bool tapDown);

	protected virtual void OnUpdate()
	{
	}

	protected virtual void OnInitialize()
	{
	}

	public virtual bool ShouldCaptureInputWhileMouseOverObject()
	{
		return false;
	}

	public virtual int PickingPriority()
	{
		return 0;
	}

	private void Start()
	{
		interactDistance = 20f;
		hasEmptyParent = HasEmptyParent();
		if (hasEmptyParent)
		{
			renderers = base.transform.parent.GetComponentsInChildren<MeshRenderer>();
		}
		else
		{
			renderers = base.transform.GetComponentsInChildren<MeshRenderer>();
		}
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			isInteractable = false;
			ToggleInteractable();
		}
		OnInteract = (OnInteractDelegate)Delegate.Combine(OnInteract, new OnInteractDelegate(GameController.Instance.ObjectiveController.OnInteractableInteract));
		OnInteract = (OnInteractDelegate)Delegate.Combine(OnInteract, new OnInteractDelegate(GameController.Instance.MissionController.OnInteractableInteract));
		if (GameController.Instance.CurrentLevel != null && GameController.Instance.CurrentLevel.Challenge != null)
		{
			OnInteract = (OnInteractDelegate)Delegate.Combine(OnInteract, new OnInteractDelegate(GameController.Instance.CurrentLevel.Challenge.OnInteractableInteract));
		}
		if (GameController.Instance.ObjectiveController.ShouldSpawnObject(GetType().ToString()))
		{
			spawnTypePrefabName = GameController.Instance.ObjectiveController.ObjectToSpawn();
		}
		OnInitialize();
		if (levelNode == null)
		{
			levelNode = LevelHelper.Vector3ToNode(base.transform.position, GameController.Instance.CurrentLevel.Nodes);
		}
	}

	public void AddObjectTag(string tag)
	{
		objectTags.Add(tag);
	}

	public bool HasObjectTag(string tag)
	{
		foreach (string objectTag in objectTags)
		{
			if (objectTag == tag)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasEmptyParent()
	{
		GameObject gameObject = null;
		if (EditorStateMachine.Instance != null)
		{
			gameObject = EditorStateMachine.Instance.levelVisualizer.levelRoot;
		}
		else if (GameController.Instance != null)
		{
			gameObject = GameController.Instance.LevelVisualizer.levelRoot;
		}
		if (gameObject != null)
		{
			if (base.transform.parent.gameObject == gameObject)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private void OnTriggerEnter(Collider other)
	{
		Character component = other.gameObject.GetComponent<Character>();
		if (component != null)
		{
			if (component.isPlayer)
			{
				playerIsInTriggerZone = true;
			}
			if ((component.CurrentPositionNode.Elevation == levelNode.Elevation || GetType() == typeof(Ladder)) && (component.isPlayer || !onlyPlayer) && (!component.IsInSpinjitzu() || GetType() == typeof(DoorTrigger) || GetType() == typeof(CrystalShard) || GetType() == typeof(BearTrap) || GetType() == typeof(Chest) || GetType() == typeof(BellTower)))
			{
				OnBaseEnter(component);
				hasTriggered = true;
			}
			else if (component.isPlayer && component.IsInSpinjitzu() && GetType() == typeof(Door))
			{
				OnBaseEnter(component);
				hasTriggered = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Character component = other.gameObject.GetComponent<Character>();
		if (component != null && component.isPlayer)
		{
			playerIsInTriggerZone = false;
		}
		if (component != null && (component.isPlayer || !onlyPlayer))
		{
			OnExit(component);
		}
	}

	private void OnBaseEnter(Character character)
	{
		OnEnter(character);
	}

	private void Update()
	{
		bool flag = false;
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i] != null && renderers[i].isVisible)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		if (GamePlayer.Instance.Player != null && levelNode != null)
		{
			if (GetType() != typeof(Ladder))
			{
				Vector3 b = Vector3.zero;
				b = (hasEmptyParent ? base.transform.parent.position : new Vector3(base.transform.position.x, GamePlayer.Instance.Player.Character.transform.position.y, base.transform.position.z));
				distToPlayer = Vector3.Distance(GamePlayer.Instance.Player.Character.transform.position, b);
				eDifToPlayer = levelNode.Elevation - GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation;
				if (distToPlayer <= interactDistance && eDifToPlayer >= (float)interactMinElevationDif && eDifToPlayer <= (float)interactMaxElevationDif && !isInteractable)
				{
					isInteractable = true;
					ToggleInteractable();
				}
				else if ((distToPlayer > interactDistance || eDifToPlayer < (float)interactMinElevationDif || eDifToPlayer > (float)interactMaxElevationDif) && isInteractable)
				{
					isInteractable = false;
					ToggleInteractable();
				}
			}
			else
			{
				Vector3 b2 = LevelHelper.NodeToVector3(levelNode);
				LevelDirection direction = LevelDirection.North;
				for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
				{
					if (levelNode.HasLadderAnySize(levelDirection))
					{
						direction = levelDirection;
						break;
					}
				}
				Vector3 b3 = LevelHelper.NodeToVector3(levelNode.GetNeighbour(GamePlayer.Instance.Level.Nodes, direction));
				float a = Vector3.Distance(GamePlayer.Instance.Player.Character.transform.position, b2);
				float b4 = Vector3.Distance(GamePlayer.Instance.Player.Character.transform.position, b3);
				float num = Mathf.Min(a, b4);
				if (num <= interactDistance && !isInteractable)
				{
					isInteractable = true;
					ToggleInteractable();
				}
				else if (num > interactDistance && isInteractable)
				{
					isInteractable = false;
					ToggleInteractable();
				}
			}
		}
		if (doActivateBlink)
		{
			float num2 = Time.time - activateBlinkTimer;
			AnimationCurve interactableFlash = GamePlayer.Instance.Level.LevelStyle.interactableFlash;
			if (num2 < 1f)
			{
				SetOutlineSwitchValue(interactableFlash.Evaluate(num2) * 4f);
			}
			else
			{
				SetOutlineSwitchValue(0f);
				doActivateBlink = false;
			}
		}
		else if (IsInteractable && isHighlightable)
		{
			float num3 = Time.time - outlineTimer;
			AnimationCurve interactableFlash2 = GamePlayer.Instance.Level.LevelStyle.interactableFlash;
			AnimationCurve interactableLoop = GamePlayer.Instance.Level.LevelStyle.interactableLoop;
			float time = interactableFlash2[interactableFlash2.length - 1].time;
			float outlineSwitchValue = ((!(num3 < time)) ? interactableLoop.Evaluate(num3 - time) : interactableFlash2.Evaluate(num3));
			SetOutlineSwitchValue(outlineSwitchValue);
		}
		OnUpdate();
	}

	protected virtual void SetOutlineSwitchValue(float value)
	{
		if (renderers == null)
		{
			return;
		}
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer != null && meshRenderer.enabled)
			{
				meshRenderer.material.SetFloat("_OutlineSwitch", value);
			}
		}
	}

	public void DoActivateBlink(ActivateBlink level)
	{
		if (activateBlink >= level)
		{
			doActivateBlink = true;
			activateBlinkTimer = Time.time;
		}
	}

	protected virtual void ToggleInteractable()
	{
		if (isInteractable)
		{
			outlineTimer = Time.time;
		}
		else
		{
			SetOutlineSwitchValue(0f);
		}
	}

	public bool IsMouseOverObject()
	{
		if (GamePlayer.Instance.SelectedObject != base.gameObject)
		{
			return false;
		}
		Vector3 selectedObjectPosition = GamePlayer.Instance.SelectedObjectPosition;
		selectedObjectPosition.z = 0f;
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		vector.z = 0f;
		Vector3 b = Camera.main.WorldToScreenPoint(base.transform.position + Camera.main.transform.right * 1.5f);
		b.z = 0f;
		Vector3 mousePosition = Input.mousePosition;
		float num = Vector3.Distance(vector, b);
		float num2 = Vector3.Distance(mousePosition, vector);
		float num3 = Vector3.Distance(mousePosition, selectedObjectPosition);
		return num2 < num || num3 < num;
	}

	protected IEnumerator SpawnObject()
	{
		GameObject spawnedObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(spawnTypePrefabName));
		if (spawnedObject != null)
		{
			if (GameController.Instance.LevelVisualizer != null)
			{
				if (GameController.Instance.LevelVisualizer.levelRoot != null)
				{
					spawnedObject.transform.parent = GameController.Instance.LevelVisualizer.levelRoot.transform;
				}
				else
				{
					spawnedObject.transform.parent = EditorStateMachine.Instance.levelVisualizer.levelRoot.transform;
				}
				spawnedObject.transform.position = base.gameObject.transform.position + new Vector3(0f, 1f, 0f);
				bool hasRigidBody = false;
				Rigidbody rigidbody = spawnedObject.GetComponent<Rigidbody>();
				if (rigidbody != null)
				{
					hasRigidBody = true;
				}
				if (!hasRigidBody)
				{
					rigidbody = spawnedObject.AddComponent<Rigidbody>();
				}
				Vector3 forceVector = Vector3.up * 5.5f;
				rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
				rigidbody.AddTorque(new Vector3(0f, 50f, 0f));
				spawnedObject.GetComponent<BoxCollider>().enabled = false;
				while (spawnedObject.transform.position.y > base.transform.position.y + 0.5f)
				{
					yield return null;
				}
				spawnedObject.GetComponent<BoxCollider>().enabled = true;
				if (!hasRigidBody)
				{
					UnityEngine.Object.Destroy(spawnedObject.GetComponent<Rigidbody>());
				}
				spawnedObject.GetComponent<Collectible>().StartAttract();
			}
			else
			{
				Debug.LogError("GameController.Instance.LevelVisualizer is null");
			}
		}
		else
		{
			Debug.LogError("InteractableObject could not instantiate object");
		}
		yield return 0;
	}

	protected IEnumerator CenterCharacter()
	{
		Vector3 delta = Vector3.one;
		while (delta.magnitude > 0.1f)
		{
			Transform playerTransform = GamePlayer.Instance.Player.Character.transform;
			delta = base.transform.position - playerTransform.position;
			delta.y = 0f;
			float deltaMagnitude = delta.magnitude;
			if (deltaMagnitude > 0.1f)
			{
				float num = GameController.Instance.PlayerProfile.MovementSpeed * Time.deltaTime;
				if (num > deltaMagnitude)
				{
					num = deltaMagnitude;
				}
				playerTransform.position += delta.normalized * num;
			}
			yield return null;
		}
	}
}
