using System.Linq;
using UnityEngine;

public class AIPerception : MonoBehaviour
{
	public delegate void OnPlayerSpottedDelegate(LevelNode position, bool visible);

	public delegate void OnPlayerHeardDelegate(LevelNode position, bool heard);

	public AIAgent agent;

	public float visionRadius = 7f;

	public float visionAngle = 180f;

	public float hearingRadius = 2.3f;

	private float hearingSpeedThreshold = 3f;

	private bool playerSpotted;

	public LevelNode lastKnownPlayerLocation;

	private float lastKnownPlayerLocationReportTime;

	private float lastKnownPlayerLocationTimeout = 15f;

	private bool playerHeard;

	private int lostPlayerCount;

	private int lostPlayerMaxCount = 3;

	public OnPlayerSpottedDelegate OnPlayerSpotted;

	public OnPlayerHeardDelegate OnPlayerHeard;

	private bool playerWasInSpinjitzu;

	public LevelNode LastKnownPlayerLocation
	{
		get
		{
			return lastKnownPlayerLocation;
		}
	}

	public bool IsPlayerVisible
	{
		get
		{
			return playerSpotted;
		}
	}

	public void SetLastKnownPlayerLocation(LevelNode node)
	{
		lastKnownPlayerLocation = node;
		lastKnownPlayerLocationReportTime = Time.time;
	}

	public void ResetLastKnownPlayerLocation()
	{
		lastKnownPlayerLocation = null;
	}

	public void SetVisionRadius(float radius)
	{
		visionRadius = radius;
	}

	public void SetVisionAngle(float angle)
	{
		if (angle > 360f)
		{
			angle %= 360f;
		}
		visionAngle = angle;
	}

	public void SetHearingRadius(float radius)
	{
		hearingRadius = radius;
	}

	public void SetHearingSpeedThreashold(float threshold)
	{
		hearingSpeedThreshold = threshold;
	}

	private void Update()
	{
		if (agent.isFriend)
		{
			return;
		}
		if (lastKnownPlayerLocation != null && lastKnownPlayerLocationReportTime + lastKnownPlayerLocationTimeout < Time.time)
		{
			SetLastKnownPlayerLocation(null);
		}
		if (agent.PlayerCharacter != null)
		{
			bool flag = agent.PlayerCharacter.IsInSpinjitzu() && !playerWasInSpinjitzu;
			if (flag)
			{
				playerWasInSpinjitzu = true;
			}
			else if (!agent.PlayerCharacter.IsInSpinjitzu() && playerWasInSpinjitzu)
			{
				playerWasInSpinjitzu = false;
			}
			if (IsPlayerWithinVisibilityZone() && !agent.PlayerCharacter.IsClimbing() && !agent.PlayerCharacter.IsHiding() && !agent.PlayerCharacter.IsDead() && LineOfSightToCharacter(agent.PlayerCharacter) && (!playerSpotted || flag))
			{
				playerSpotted = true;
				if (OnPlayerSpotted != null)
				{
					OnPlayerSpotted(agent.PlayerCharacter.CurrentPositionNode, true);
				}
			}
			if (playerSpotted)
			{
				if (agent.PlayerCharacter.IsDead() || agent.PlayerCharacter.IsClimbing() || (agent.PlayerCharacter.IsHiding() && agent.SpottetPlayerMeter < 0.99f))
				{
					if (OnPlayerSpotted != null)
					{
						OnPlayerSpotted(lastKnownPlayerLocation, false);
					}
					playerSpotted = false;
					lostPlayerCount = 0;
				}
				else if (!IsPlayerWithinVisibilityZone() && Vector3.Distance(agent.Character.transform.position, agent.PlayerCharacter.transform.position) > 1.2f)
				{
					lostPlayerCount++;
					if (lostPlayerCount >= lostPlayerMaxCount)
					{
						if (OnPlayerSpotted != null && playerSpotted)
						{
							OnPlayerSpotted(lastKnownPlayerLocation, false);
						}
						playerSpotted = false;
						lostPlayerCount = 0;
					}
				}
				else
				{
					SetLastKnownPlayerLocation(agent.PlayerCharacter.CurrentPositionNode);
				}
			}
		}
		if (playerSpotted || !(agent.PlayerCharacter != null))
		{
			return;
		}
		Vector3 position = agent.PlayerCharacter.transform.position;
		if (Vector3.Distance(position, base.transform.position) <= hearingRadius)
		{
			if (agent.PlayerCharacter.Speed >= hearingSpeedThreshold && !agent.PlayerCharacter.IsHiding())
			{
				if (OnPlayerHeard != null && !playerHeard)
				{
					SetLastKnownPlayerLocation(agent.PlayerCharacter.CurrentPositionNode);
					OnPlayerHeard(agent.PlayerCharacter.CurrentPositionNode, true);
				}
				playerHeard = true;
			}
			else
			{
				if (OnPlayerHeard != null && playerHeard)
				{
					OnPlayerHeard(agent.PlayerCharacter.CurrentPositionNode, false);
				}
				playerHeard = false;
			}
		}
		else
		{
			if (OnPlayerHeard != null && playerHeard)
			{
				OnPlayerHeard(agent.PlayerCharacter.CurrentPositionNode, false);
			}
			playerHeard = false;
		}
	}

	private bool IsPlayerWithinVisibilityZone()
	{
		if (agent.PlayerCharacter.IsDead())
		{
			return false;
		}
		if (agent.PlayerCharacter.CurrentPositionNode.Elevation != agent.Character.CurrentPositionNode.Elevation)
		{
			return false;
		}
		if (agent.PlayerCharacter.CurrentPositionNode == agent.Character.CurrentPositionNode || agent.PlayerCharacter.CurrentPositionNode == agent.Character.CurrentPathNode)
		{
			Vector3 normalized = (agent.Character.transform.position - agent.PlayerCharacter.transform.position).normalized;
			float num = Vector3.Dot(agent.Character.transform.forward, normalized);
			if (num <= 0f)
			{
				return true;
			}
		}
		float num2 = LevelHelper.Distance(agent.PlayerCharacter.CurrentPositionNode.Location, agent.Character.CurrentPositionNode.Location);
		if (num2 <= visionRadius)
		{
			Vector3 to = agent.PlayerCharacter.transform.position - agent.Character.transform.position;
			float num3 = Vector3.Angle(agent.Character.transform.forward, to);
			if (num3 < visionAngle / 2f)
			{
				return true;
			}
		}
		return false;
	}

	private bool LineOfSightToCharacter(Character character)
	{
		if (Mathf.Abs(agent.PlayerCharacter.CurrentPositionNode.Elevation - agent.Character.CurrentPositionNode.Elevation) >= 2)
		{
			return false;
		}
		if (agent.PlayerCharacter.IsDead())
		{
			return false;
		}
		Vector3 vector = agent.Character.transform.position + agent.Character.GetEyePosition();
		Vector3 direction = character.transform.position + character.GetEyePosition() - vector;
		Debug.DrawLine(vector, character.transform.position + character.GetEyePosition(), Color.red);
		if (DoLineOfSightCheck(character, vector, direction))
		{
			return true;
		}
		direction = character.transform.position - vector;
		Debug.DrawLine(vector, character.transform.position, Color.red);
		if (DoLineOfSightCheck(character, vector, direction))
		{
			return true;
		}
		if (agent.PlayerCharacter.CurrentPositionNode == agent.Character.CurrentPositionNode)
		{
			return true;
		}
		if (agent.PlayerCharacter.CurrentPositionNode == agent.Character.CurrentPathNode)
		{
			return true;
		}
		return false;
	}

	private bool DoLineOfSightCheck(Character character, Vector3 origin, Vector3 direction)
	{
		Ray ray = new Ray(origin, direction);
		RaycastHit[] array = (from h in Physics.RaycastAll(ray, visionRadius)
			orderby h.distance
			select h).ToArray();
		if (array.Length > 0 && (EditorStateMachine.Instance == null || EditorStateMachine.Instance.enemiesCanSee))
		{
			Character component = array[0].collider.GetComponent<Character>();
			if (component != null && component == character)
			{
				return true;
			}
		}
		return false;
	}
}
