using UnityEngine;

public class CamSettingsManager : MonoBehaviour
{
	private CamSettings current;

	private CamSettings old;

	private float startTime;

	private float duration;

	private bool wasInCombat;

	private int combatIndex;

	private AIAgent currentAttacker;

	private int explorationIndex;

	private int explorationIndexPrev;

	private InGameCamState state;

	private ExplorationCamTransitionManager explorationMgr = new ExplorationCamTransitionManager();

	public Vector3 PositionOffset
	{
		get
		{
			if (GamePlayer.Instance != null && GamePlayer.Instance.State == GamePlayerState.Combat)
			{
				if (Time.time - startTime >= duration)
				{
					return GameController.Instance.camSettingsDB.combatPositionOffsets[combatIndex];
				}
				float t = (Time.time - startTime) / duration;
				return Vector3.Lerp(old.positionOffset, GameController.Instance.camSettingsDB.combatPositionOffsets[combatIndex], t);
			}
			if (state == InGameCamState.Exploring)
			{
				if (Time.time - startTime >= duration)
				{
					return Vector3.Lerp(GameController.Instance.camSettingsDB.explorationPositionOffsets[explorationIndexPrev], GameController.Instance.camSettingsDB.explorationPositionOffsets[explorationIndex], explorationMgr.ShiftLerpValue);
				}
				float t2 = (Time.time - startTime) / duration;
				return Vector3.Lerp(old.positionOffset, Vector3.Lerp(GameController.Instance.camSettingsDB.explorationPositionOffsets[explorationIndexPrev], GameController.Instance.camSettingsDB.explorationPositionOffsets[explorationIndex], explorationMgr.ShiftLerpValue), t2);
			}
			if (Time.time - startTime >= duration)
			{
				return current.positionOffset;
			}
			float t3 = (Time.time - startTime) / duration;
			return Vector3.Lerp(old.positionOffset, current.positionOffset, t3);
		}
	}

	public Vector3 LookAtOffset
	{
		get
		{
			if (Time.time - startTime >= duration)
			{
				return current.lookAtOffset;
			}
			float t = (Time.time - startTime) / duration;
			return Vector3.Lerp(old.lookAtOffset, current.lookAtOffset, t);
		}
	}

	public float Dist
	{
		get
		{
			if (Time.time - startTime >= duration)
			{
				return current.dist;
			}
			float t = (Time.time - startTime) / duration;
			return Mathf.Lerp(old.dist, current.dist, t);
		}
	}

	public float InterpolationSpeed
	{
		get
		{
			if (Time.time - startTime >= duration)
			{
				return current.interpolationSpeed;
			}
			float t = (Time.time - startTime) / duration;
			return Mathf.Lerp(old.interpolationSpeed, current.interpolationSpeed, t);
		}
	}

	public float TransitionOutTime
	{
		get
		{
			if (Time.time - startTime >= duration)
			{
				return current.transitionOutTime;
			}
			float t = (Time.time - startTime) / duration;
			return Mathf.Lerp(old.transitionOutTime, current.transitionOutTime, t);
		}
	}

	private void Awake()
	{
		state = InGameCamState.Starting;
		current = GameController.Instance.camSettingsDB.stateSettings[InGameCamState.Starting];
		old = current;
		startTime = Time.time;
		duration = 0f;
	}

	private void Update()
	{
		if (GamePlayer.Instance != null)
		{
			if (GamePlayer.Instance.State == GamePlayerState.Combat && GamePlayer.Instance.CombatManager.CurrentAttacker != null && !wasInCombat)
			{
				InitializeCombat();
			}
			if (GamePlayer.Instance.State != GamePlayerState.Combat && wasInCombat)
			{
				wasInCombat = false;
			}
			if (wasInCombat && GamePlayer.Instance.CombatManager.CurrentAttacker != null && GamePlayer.Instance.CombatManager.CurrentAttacker != currentAttacker)
			{
				InitializeCombat();
			}
		}
	}

	public void EvaluateExplorationCam()
	{
		if (GamePlayer.Instance.Player.Character == null)
		{
			return;
		}
		int num = int.MaxValue;
		int num2 = 0;
		int index = 0;
		foreach (Vector3 explorationPositionOffset in GameController.Instance.camSettingsDB.explorationPositionOffsets)
		{
			int num3 = 0;
			Vector3 vector2 = GamePlayer.Instance.Player.Character.transform.position + new Vector3(0f, 0.4f, 0f);
			Vector3 vector3 = GamePlayer.Instance.Player.Character.transform.position + explorationPositionOffset + new Vector3(0f, 0f, 0f - GameController.Instance.camSettingsDB.stateSettings[InGameCamState.Exploring].dist);
			float num4 = 0.2f;
			for (float num5 = 0f - num4; num5 <= num4; num5 += num4)
			{
				Vector3 vector4 = vector2 + GamePlayer.Instance.Player.Character.transform.right * num5;
				RaycastHit[] array = Physics.RaycastAll(vector3, (vector4 - vector3).normalized, Vector3.Distance(vector4, vector3), -1);
				RaycastHit[] array2 = array;
				foreach (RaycastHit raycastHit in array2)
				{
					if (raycastHit.collider.gameObject.tag == "LevelMesh")
					{
						num3++;
					}
				}
			}
			if (num3 < num)
			{
				index = num2;
				num = num3;
			}
			num2++;
			if (num3 == 0)
			{
				break;
			}
		}
		if (explorationMgr.EvaluateNewIndex(index))
		{
			explorationIndexPrev = explorationIndex;
			explorationIndex = index;
			explorationMgr.SetNewIndex();
		}
	}

	private void InitializeCombat()
	{
		int num = int.MaxValue;
		int num2 = 0;
		currentAttacker = GamePlayer.Instance.CombatManager.CurrentAttacker;
		EnemyCombatSlot combatSlotByAgent = GamePlayer.Instance.CombatManager.GetCombatSlotByAgent(currentAttacker);
		LevelNode playerCombatSlot = GamePlayer.Instance.CombatManager.PlayerCombatSlot;
		foreach (Vector3 combatPositionOffset in GameController.Instance.camSettingsDB.combatPositionOffsets)
		{
			int num3 = 0;
			num3 += 2 * GetNumCombatOccluders(playerCombatSlot, new Vector3(playerCombatSlot.Col, playerCombatSlot.Elevation, playerCombatSlot.Row) + combatPositionOffset + new Vector3(0f, 0f, 0f - GameController.Instance.camSettingsDB.stateSettings[InGameCamState.Combat].dist));
			num3 += GetNumCombatOccluders(combatSlotByAgent.levelNode, new Vector3(combatSlotByAgent.levelNode.Col, combatSlotByAgent.levelNode.Elevation, combatSlotByAgent.levelNode.Row) + combatPositionOffset + new Vector3(0f, 0f, 0f - GameController.Instance.camSettingsDB.stateSettings[InGameCamState.Combat].dist));
			if (num3 < num)
			{
				combatIndex = num2;
				num = num3;
			}
			num2++;
		}
		wasInCombat = true;
	}

	private int GetNumCombatOccluders(LevelNode node, Vector3 camPos)
	{
		int num = 0;
		for (float num2 = 0.4f; num2 <= 1f; num2 += 0.4f)
		{
			Vector3 vector = new Vector3(node.Col, (float)node.Elevation + num2, node.Row);
			RaycastHit[] array = Physics.RaycastAll(camPos, (vector - camPos).normalized, Vector3.Distance(vector, camPos), -1);
			RaycastHit[] array2 = array;
			foreach (RaycastHit raycastHit in array2)
			{
				Character component = raycastHit.collider.GetComponent<Character>();
				if (component == null)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void Transition(InGameCamState state, float duration)
	{
		if (Time.time - startTime >= duration)
		{
			old = current;
		}
		else
		{
			old = new CamSettings();
			old.positionOffset = PositionOffset;
			old.lookAtOffset = LookAtOffset;
			old.dist = Dist;
			old.interpolationSpeed = InterpolationSpeed;
			old.transitionOutTime = TransitionOutTime;
		}
		this.duration = duration;
		startTime = Time.time;
		current = GameController.Instance.camSettingsDB.stateSettings[state];
		this.state = state;
	}

	public float GetTransitionTime(InGameCamState state)
	{
		return GameController.Instance.camSettingsDB.stateSettings[state].transitionOutTime;
	}
}
