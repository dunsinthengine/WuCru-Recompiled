using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, ICharacterController
{
	public const int MAX_PATH_LENGTH = 25;

	private Character character;

	private Camera cam;

	public TextMesh textMesh;

	public AnimationCurve movementSpeed;

	public float movementSpeedMinimumDistance;

	public float movementSpeedMaximumDistance;

	private Character currentEnemy;

	private float lastMouseDownTime;

	private float lastMouseHitDownTime;

	private Vector3 lastMouseHitDownPosition;

	private MeshCollider lastMouseHitDownMeshCollider;

	private Vector3 lastMouseHitDownWorldPosition;

	private bool activateJoystickMove;

	private float holdMinDuration = 0.05f;

	private float tapMaxDuration = 0.26f;

	private int frameCounter;

	public Character Character
	{
		get
		{
			return character;
		}
	}

	public Character CurrentEnemy
	{
		get
		{
			return currentEnemy;
		}
	}

	public bool HasEnemySneakAttackTarget { get; set; }

	public List<Bush> BushQueue { get; private set; }

	public Vector3 ElementalAttackTarget { get; set; }

	public ElementalAttack ElementalAttackFx { get; set; }

	public ElementalAttackCharger ElementalCharger { get; set; }

	public bool CanSeePlayer()
	{
		return false;
	}

	private void Awake()
	{
		cam = Camera.main;
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Characters/Ninja_Character"));
		character = gameObject.GetComponent<Character>();
		if (GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			CharacterBuilder.ConfigureMinifig(character, (GameController.Instance.currentState as GameControllerStatePlaying).CurrentNinjaID);
		}
		character.isPlayer = true;
		if (character.Animator != null)
		{
			character.Animator.applyRootMotion = false;
		}
		character.UpdateLevel(GamePlayer.Instance.Level.Nodes);
		gameObject.transform.parent = base.transform;
		textMesh = GetComponentInChildren<TextMesh>();
		BushQueue = new List<Bush>();
		ElementalCharger = GetComponent<ElementalAttackCharger>();
		if (EditorStateMachine.Instance == null || !EditorStateMachine.Instance.debugTextOnCharacters)
		{
			textMesh.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		frameCounter++;
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ChangeWeapon(WeaponType.Katana);
			Character.EquipWeapon(false);
			character.GetSwordTappingIcon().Initialize();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ChangeWeapon(WeaponType.Shuriken);
			Character.EquipWeapon(false);
			character.GetSwordTappingIcon().Initialize();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ChangeWeapon(WeaponType.Nunchuck);
			Character.EquipWeapon(false);
			character.GetSwordTappingIcon().Initialize();
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			ChangeWeapon(WeaponType.Scythe);
			Character.EquipWeapon(false);
			character.GetSwordTappingIcon().Initialize();
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			ChangeWeapon(WeaponType.Axe);
			Character.EquipWeapon(false);
			character.GetSwordTappingIcon().Initialize();
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			ChangeWeapon(WeaponType.Staff);
			Character.EquipWeapon(false);
			character.GetSwordTappingIcon().Initialize();
		}
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			Character.EquipWeapon(true);
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			Character.EquipWeapon(false);
		}
	}

	private void FixedUpdate()
	{
		if (textMesh != null && ((EditorStateMachine.Instance != null && EditorStateMachine.Instance.debugTextOnCharacters) || (GameController.Instance != null && GameController.Instance.debugTextOnCharacters)))
		{
			if (!textMesh.gameObject.activeSelf)
			{
				textMesh.gameObject.SetActive(true);
			}
			textMesh.transform.rotation = Quaternion.identity;
			textMesh.text = Character.CurrentStateName();
		}
		else if (textMesh != null)
		{
			textMesh.text = string.Empty;
		}
	}

	public void ToggleHide(bool hidden)
	{
		if (hidden)
		{
			character.Animator.SetTrigger(TriggerHash.hide);
			return;
		}
		Debug.Log("Toggle hideden false");
		character.Animator.SetTrigger(TriggerHash.move);
	}

	public void ToggleTrapped(bool trapped)
	{
		if (trapped)
		{
			character.ResetPath();
			character.Animator.SetTrigger(TriggerHash.trapped);
		}
		else
		{
			character.Animator.SetTrigger(TriggerHash.untrapped);
		}
	}

	private bool LineOfSight(Character enemy)
	{
		Vector3 vector = Character.transform.position + Character.GetEyePosition();
		Vector3 direction = enemy.transform.position + enemy.GetEyePosition() - vector;
		if (LineOfSightRaycast(enemy, vector, direction))
		{
			return true;
		}
		direction = enemy.transform.position - vector;
		if (LineOfSightRaycast(enemy, vector, direction))
		{
			return true;
		}
		return false;
	}

	private bool LineOfSightRaycast(Character enemy, Vector3 origin, Vector3 direction)
	{
		Ray ray = new Ray(origin, direction);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo) && (EditorStateMachine.Instance == null || EditorStateMachine.Instance.enemiesCanSee))
		{
			Character component = hitInfo.collider.GetComponent<Character>();
			if (component != null && component == enemy)
			{
				Debug.DrawLine(origin, hitInfo.point, Color.white, 5f);
				return true;
			}
			Debug.DrawLine(origin, hitInfo.point, Color.red, 5f);
		}
		Debug.DrawLine(origin, origin + Vector3.Normalize(direction) * 10f, Color.yellow, 5f);
		return false;
	}

	public void SneakAttack(Character enemy)
	{
		PathFinding.Node[] pathIncludeDoors = character.PathFinding.GetPathIncludeDoors(character.CurrentPositionNode, enemy.CurrentPositionNode, character);
		PathFinding.Node[] smoothPath = character.PathFinding.GetSmoothPath(pathIncludeDoors);
		float num = 0f;
		if (pathIncludeDoors != null || smoothPath != null)
		{
			num = character.PathFinding.GetPathLength((smoothPath == null) ? pathIncludeDoors : smoothPath);
		}
		if (character.GetWeapon().type != WeaponType.Shuriken || num < 3f)
		{
			if (enemy.IsDead() || enemy.IsPreparingCombat() || enemy.IsInCombat() || (enemy.Controller as AIController).CanSeePlayer() || (enemy.Controller as AIController).agent.isFriend || pathIncludeDoors == null)
			{
				return;
			}
			bool flag = true;
			PathFinding.Node[] array = pathIncludeDoors;
			foreach (PathFinding.Node node in array)
			{
				if (node.levelNode.Elevation != character.CurrentPositionNode.Elevation)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				currentEnemy = enemy;
				character.Animator.SetTrigger(TriggerHash.sneakAttack);
				HasEnemySneakAttackTarget = true;
				GamePlayer.Instance.MovementPointer.Hide();
				GamePlayer.Instance.MovementPointer.BlinkCharacter(currentEnemy);
			}
		}
		else if (!enemy.IsDead() && LineOfSight(enemy) && !enemy.IsPreparingCombat() && !enemy.IsInCombat() && (enemy.Controller as AIController).agent.State != AIAgentState.Alert && !(enemy.Controller as AIController).agent.isFriend && character.PathFinding.GetPathIncludeDoors(character.CurrentPositionNode, enemy.CurrentPositionNode, character) != null)
		{
			currentEnemy = enemy;
			character.Animator.SetTrigger(TriggerHash.rangedSneakAttack);
			HasEnemySneakAttackTarget = true;
			GamePlayer.Instance.MovementPointer.Hide();
			GamePlayer.Instance.MovementPointer.BlinkCharacter(currentEnemy);
		}
	}

	public void OnEnemySneakAttackDone()
	{
		currentEnemy = null;
	}

	public void SetSpinjitzuEnemyTarget(Character enemy)
	{
		PathFinding.Node[] pathIncludeDoors = character.PathFinding.GetPathIncludeDoors(character.CurrentPositionNode, enemy.CurrentPositionNode, character);
		if (enemy.IsDead() || enemy.IsPreparingCombat() || enemy.IsInCombat() || (enemy.Controller as AIController).CanSeePlayer() || (enemy.Controller as AIController).agent.isFriend || pathIncludeDoors == null)
		{
			return;
		}
		bool flag = true;
		PathFinding.Node[] array = pathIncludeDoors;
		foreach (PathFinding.Node node in array)
		{
			if (node.levelNode.Elevation != character.CurrentPositionNode.Elevation)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			currentEnemy = enemy;
			GamePlayer.Instance.MovementPointer.Hide();
			GamePlayer.Instance.MovementPointer.BlinkCharacter(currentEnemy);
		}
	}

	public void OnSpinjitzuDone()
	{
		currentEnemy = null;
	}

	public void ChargeElemetalAttack()
	{
	}

	public void ChargeElemetalAttackComplete()
	{
		if ((double)ElementalAttackFx.GetChargeAmount() < 0.1)
		{
			character.Animator.SetTrigger(TriggerHash.elementalAttackCancel);
		}
	}

	public void ChangeWeapon(WeaponType type)
	{
		if (type == WeaponType.None)
		{
			Debug.LogError("Unknown weapon type - cannot change weapon");
			return;
		}
		Debug.LogWarning("Using deprecated ChangeWeapon");
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(Paths.characterPrefabBasePath + CharacterBuilder.weaponModels[(int)type]), Vector3.zero, Quaternion.identity);
		Transform parent = character.swordTransform.gameObject.transform.parent;
		Vector3 localPosition = character.swordTransform.localPosition;
		Quaternion localRotation = character.swordTransform.localRotation;
		Object.Destroy(character.swordTransform.gameObject);
		gameObject.gameObject.transform.parent = parent;
		character.swordTransform = gameObject.gameObject.transform;
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localRotation = localRotation;
		Debug.Log("Weapong changed: " + character.GetWeapon().type);
		character.RefreshMeshRenderers();
	}

	public void WalkTo(LevelNode target, bool force = false)
	{
		character.WalkTo(target, force);
	}

	private bool IsClickingOnUI()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			PointerEventData eventData = pointerEventData;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventData, list);
			foreach (RaycastResult item in list)
			{
				if (item.gameObject.tag == "BlockMouse")
				{
					return true;
				}
			}
		}
		return false;
	}

	public void HandlePicking()
	{
		if ((GameController.Instance.currentState is GameControllerStatePlaying && (GameController.Instance.currentState as GameControllerStatePlaying).IsConfirmingQuit) || GamePlayer.Instance.State != GamePlayerState.Exploring || GamePlayer.Instance.Player.Character.IsSwingHitting() || GamePlayer.Instance.Player.Character.IsKnockedDown() || GamePlayer.Instance.Player.Character.IsSneakAttacking() || GamePlayer.Instance.Player.Character.IsPreparingSneakAttack())
		{
			return;
		}
		bool flag = ScreenHud._pInstance._pMouseDown && Time.timeSinceLevelLoad - ScreenHud._pInstance._pLastMouseButtonDownTime <= 0f;
		if (ScreenHud._pInstance._pMouseDown && !this.character.IsInCombat() && !this.character.IsPreparingCombat() && GamePlayer.Instance.State != GamePlayerState.Combat)
		{
			if (GamePlayer.Instance.SelectedObject != null && !Input.GetMouseButtonDown(0))
			{
				InteractableObject component = GamePlayer.Instance.SelectedObject.GetComponent<InteractableObject>();
				if (component != null && component.ShouldCaptureInputWhileMouseOverObject() && component.IsMouseOverObject())
				{
					return;
				}
			}
			MeshCollider meshCollider = null;
			MeshCollider meshCollider2 = null;
			Collectible collectible = null;
			InteractableObject interactableObject = null;
			Character character = null;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			if (flag)
			{
				lastMouseDownTime = Time.timeSinceLevelLoad;
				activateJoystickMove = true;
				RaycastHit[] array = Physics.RaycastAll(ray);
				if (array.Length > 0)
				{
					float num = float.PositiveInfinity;
					float num2 = float.PositiveInfinity;
					float num3 = float.PositiveInfinity;
					float num4 = float.PositiveInfinity;
					float num5 = float.PositiveInfinity;
					List<InteractableObject> list = new List<InteractableObject>();
					RaycastHit[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						RaycastHit raycastHit = array2[i];
						MeshCollider component2 = raycastHit.collider.GetComponent<MeshCollider>();
						Collectible component3 = raycastHit.collider.GetComponent<Collectible>();
						InteractableObject component4 = raycastHit.collider.GetComponent<InteractableObject>();
						Character character2 = raycastHit.collider.GetComponent<Character>();
						if (character2 != null && (character2.isPlayer || character2.IsDead()))
						{
							character2 = null;
						}
						if (component2 != null)
						{
							if (Vector3.Dot(raycastHit.normal, Vector3.up) > 0.9f && raycastHit.distance < num)
							{
								meshCollider = component2;
								num = raycastHit.distance;
							}
							if (raycastHit.distance < num2)
							{
								meshCollider2 = component2;
								num2 = raycastHit.distance;
							}
						}
						if (character2 != null && !character2.IsDead() && character2.IsInteractable)
						{
							if (raycastHit.distance < num5)
							{
								character = character2;
								num5 = raycastHit.distance;
							}
						}
						else if (component4 != null && component4.IsInteractable)
						{
							if (raycastHit.distance < num4)
							{
								interactableObject = component4;
								num4 = raycastHit.distance;
								list.Add(component4);
							}
						}
						else if (component3 != null && raycastHit.distance < num3)
						{
							collectible = component3;
							num3 = raycastHit.distance;
						}
					}
					if (character != null && !this.character.IsElemetalAttackAnticipating() && !this.character.IsElemetalAttacking())
					{
						GamePlayer.Instance.SelectedObject = null;
						if (!this.character.IsInSpinjitzu())
						{
							activateJoystickMove = false;
							SneakAttack(character);
						}
						else
						{
							activateJoystickMove = false;
							SetSpinjitzuEnemyTarget(character);
						}
					}
					else if (list.Count > 0 && !this.character.IsElemetalAttackAnticipating() && !this.character.IsElemetalAttacking())
					{
						list.Sort((InteractableObject a, InteractableObject b) => a.PickingPriority().CompareTo(b.PickingPriority()));
						interactableObject = list[0];
						interactableObject.OnPicked(this.character, flag);
						GamePlayer.Instance.SelectedObject = interactableObject.gameObject;
						if (!this.character.IsHiding() || interactableObject.levelNode != this.character.CurrentPositionNode)
						{
							GamePlayer.Instance.MovementPointer.BlinkInteractable(interactableObject);
						}
					}
					else if (meshCollider != null)
					{
						lastMouseHitDownMeshCollider = meshCollider;
						lastMouseHitDownPosition = Input.mousePosition;
						lastMouseHitDownWorldPosition = ray.GetPoint(num);
						lastMouseHitDownTime = Time.timeSinceLevelLoad;
					}
				}
			}
			if (this.character.IsElemetalAttackAnticipating())
			{
				Vector3 elementalAttackTarget = Vector3.zero;
				if (character != null)
				{
					elementalAttackTarget = LevelHelper.NodeToVector3(character.CurrentPositionNode);
					GamePlayer.Instance.MovementPointer.BlinkCharacter(character);
				}
				else if (interactableObject != null && interactableObject is CrystalShard)
				{
					elementalAttackTarget = LevelHelper.NodeToVector3(interactableObject.levelNode);
					Debug.Log("__BLINK__");
					GamePlayer.Instance.MovementPointer.BlinkInteractable(interactableObject);
				}
				else if (meshCollider2 != null)
				{
					RaycastHit hitInfo;
					if (meshCollider2.Raycast(ray, out hitInfo, float.MaxValue))
					{
						elementalAttackTarget = hitInfo.point;
						if (Vector3.Dot(hitInfo.normal, Vector3.up) < 0.9f)
						{
							elementalAttackTarget.y -= 0.6f;
						}
					}
				}
				else
				{
					float enter;
					new Plane(Vector3.up, this.character.transform.position).Raycast(ray, out enter);
					elementalAttackTarget = ray.GetPoint(enter);
				}
				this.character.Animator.SetTrigger(TriggerHash.elementalAttackPerform);
				ElementalAttackTarget = elementalAttackTarget;
				activateJoystickMove = false;
				collectible = null;
				interactableObject = null;
				character = null;
			}
			if (!this.character.IsElemetalAttackAnticipating() && activateJoystickMove && collectible == null && interactableObject == null && character == null && (!this.character.HasPath() || flag))
			{
				this.character.ResetPath();
				Vector3 position = this.character.transform.position;
				float enter2;
				new Plane(Vector3.up, position).Raycast(ray, out enter2);
				Vector3 point = ray.GetPoint(enter2);
				Vector3 vector = point - position;
				Vector3 direction = Vector3.Normalize(vector);
				float num6 = (Mathf.Clamp(vector.magnitude, movementSpeedMinimumDistance, movementSpeedMaximumDistance) - movementSpeedMinimumDistance) / (movementSpeedMaximumDistance - movementSpeedMinimumDistance);
				if (!this.character.IsHiding() || num6 > 0.2f)
				{
					this.character.Speed = GameController.Instance.PlayerProfile.MovementSpeed * movementSpeed.Evaluate(num6);
					this.character.MoveTowards(direction, this.character.Speed, float.MaxValue);
					this.character.inputVector = vector;
					Debug.DrawLine(position, point, Color.red);
					GamePlayer.Instance.SelectedObject = null;
				}
				if (Time.timeSinceLevelLoad - lastMouseDownTime > holdMinDuration && !HasEnemySneakAttackTarget)
				{
					GamePlayer.Instance.MovementPointer.PositionTarget(point, MovementPointer.MovementPointerStyle.JOYSTICK);
				}
			}
			else
			{
				this.character.Speed = GameController.Instance.PlayerProfile.MovementSpeed;
				this.character.inputVector = Vector3.zero;
				if (!this.character.HasPath() && this.character.IsRunning())
				{
					this.character.Animator.SetBool(VarHash.isMoving, false);
				}
			}
		}
		else
		{
			this.character.Speed = GameController.Instance.PlayerProfile.MovementSpeed;
			this.character.inputVector = Vector3.zero;
			if (!this.character.HasPath() && this.character.IsRunning())
			{
				this.character.Animator.SetBool(VarHash.isMoving, false);
			}
		}
		float num7 = Vector3.Distance(Input.mousePosition, lastMouseHitDownPosition);
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		activateJoystickMove = false;
		Ray ray2 = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] array3 = Physics.RaycastAll(ray2);
		if (!(Time.timeSinceLevelLoad - lastMouseDownTime < tapMaxDuration))
		{
			return;
		}
		float num8 = float.MaxValue;
		if (array3.Length > 0)
		{
			RaycastHit[] array4 = array3;
			for (int num9 = 0; num9 < array4.Length; num9++)
			{
				RaycastHit raycastHit2 = array4[num9];
				MeshCollider component5 = raycastHit2.collider.GetComponent<MeshCollider>();
				if (component5 != null && Vector3.Dot(raycastHit2.normal, Vector3.up) > 0.9f && raycastHit2.distance < num8)
				{
					num8 = raycastHit2.distance;
				}
			}
			Vector3 point2 = ray2.GetPoint(num8);
			LevelNode levelNode = LevelHelper.Vector3ToNode(point2, GamePlayer.Instance.Level.Nodes);
			if (levelNode != null)
			{
				PathFinding.Node[] pathIgnoreDoors = this.character.PathFinding.GetPathIgnoreDoors(this.character.CurrentPositionNode, levelNode, this.character);
				if (pathIgnoreDoors != null && pathIgnoreDoors.Length < 25)
				{
					if (levelNode != this.character.CurrentPositionNode && lastMouseHitDownMeshCollider != null && Time.timeSinceLevelLoad - lastMouseHitDownTime < tapMaxDuration)
					{
						if (!GamePlayer.Instance.MovementPointer.BlinkInteractablesAt(levelNode) && !HasEnemySneakAttackTarget)
						{
							GamePlayer.Instance.MovementPointer.PositionTarget(point2, MovementPointer.MovementPointerStyle.POINTER);
						}
						WalkTo(levelNode);
						GamePlayer.Instance.SelectedObject = null;
					}
				}
				else
				{
					ShowMousePointerError();
				}
			}
			else
			{
				ShowMousePointerError();
			}
		}
		else
		{
			ShowMousePointerError();
		}
	}

	private void ShowMousePointerError()
	{
		if (!(GamePlayer.Instance.SelectedObject != null) && !HasEnemySneakAttackTarget)
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			Vector3 position = character.transform.position;
			float enter;
			new Plane(Vector3.up, position).Raycast(ray, out enter);
			GamePlayer.Instance.MovementPointer.PositionTarget(ray.GetPoint(enter), MovementPointer.MovementPointerStyle.ERROR);
		}
	}
}
