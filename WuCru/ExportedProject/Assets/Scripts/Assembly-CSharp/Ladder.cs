using UnityEngine;

public class Ladder : InteractableObject
{
	private bool clicked;

	private Character clickedByCharacter;

	private bool setClimbTrigger;

	private bool goingUp;

	private float lastClimbTriggerTime = -10f;

	private const float CLIMB_TRIGGER_COOLDOWN = 1.0f;

	public override int PickingPriority()
	{
		return 0;
	}

	public void Awake()
	{
		onlyPlayer = true;
		interactMinElevationDif = -1;
		interactMaxElevationDif = 0;
		clicked = false;
		clickedByCharacter = null;
		setClimbTrigger = false;
		goingUp = true;
		activateBlink = ActivateBlink.ON_GOTO_NOTE;
	}

	protected override void OnEnter(Character character)
	{
		if (character == null)
		{
			Debug.Log("Character is null!");
		}
		else if (character.Controller == null)
		{
			Debug.Log("Character controller is null");
		}
		else if (character.isPlayer)
		{
			clickedByCharacter = character;
			setClimbTrigger = false;
		}
	}

	protected override void OnExit(Character character)
	{
		if (clicked)
		{
			clicked = false;
		}
		if (character != null && character.isPlayer && clickedByCharacter == character)
		{
			clickedByCharacter = null;
			setClimbTrigger = false;
		}
	}

	protected override void OnUpdate()
	{
		if (Time.time - lastClimbTriggerTime < CLIMB_TRIGGER_COOLDOWN)
		{
			return;
		}
		if ((clickedByCharacter != null && clickedByCharacter.isPlayer) || playerIsInTriggerZone)
		{
			if (clickedByCharacter == null && GamePlayer.Instance != null && GamePlayer.Instance.Player != null)
			{
				clickedByCharacter = GamePlayer.Instance.Player.Character;
				if (clickedByCharacter != null && !clickedByCharacter.IsClimbing())
				{
					setClimbTrigger = false;
				}
			}
			if (clickedByCharacter != null && clickedByCharacter.inputVector.magnitude > 0.01f * Time.deltaTime)
			{
				LevelDirection levelDirectionForVector = LevelHelper.GetLevelDirectionForVector(clickedByCharacter.inputVector);
				Vector3 vector = base.transform.parent.InverseTransformPoint(clickedByCharacter.transform.position);
				Vector3 vector2 = base.transform.parent.InverseTransformDirection(clickedByCharacter.inputVector);
				float num = 0.5f - vector.z;
				float num2 = Vector3.Dot(vector2.normalized, Vector3.forward);
				float num3 = 0.75f;
				if (((!(num < 0f)) ? num2 : (0f - num2)) > num3 && Mathf.Abs(vector.x) < Mathf.Lerp(0.1f, 0.4f, (Mathf.Abs(num2) - num3) * 4f) && ((!(num < 0f)) ? vector2.z : (0f - vector2.z)) > 0.1f && !clickedByCharacter.IsClimbing() && !setClimbTrigger)
				{
					Debug.Log("Climb Trigger");
					clickedByCharacter.Animator.SetTrigger(TriggerHash.climb);
					setClimbTrigger = true;
					lastClimbTriggerTime = Time.time;
				}
			}
		}
		if (setClimbTrigger && clickedByCharacter != null && !clickedByCharacter.IsClimbing() && Time.time - lastClimbTriggerTime >= CLIMB_TRIGGER_COOLDOWN)
		{
			setClimbTrigger = false;
			clickedByCharacter = null;
		}
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		if (!character.IsClimbing())
		{
			if (character == null)
			{
				Debug.LogError("Character is null");
			}
			if (levelNode == null)
			{
				Debug.LogError("LevelNode is null");
			}
			clicked = true;
			clickedByCharacter = character;
			setClimbTrigger = false;
			character.LookAt(base.transform.position);
			if (character.CurrentPositionNode.Elevation <= levelNode.Elevation)
			{
				goingUp = true;
			}
			else
			{
				goingUp = false;
			}
			character.Controller.WalkTo((!goingUp) ? levelNode : GetLadderNeighbor());
		}
	}

	public LevelNode GetLadderNeighbor()
	{
		LevelDirection levelDirectionForVector = LevelHelper.GetLevelDirectionForVector(base.transform.forward);
		return levelNode.GetNeighbour(GamePlayer.Instance.Level.Nodes, levelDirectionForVector);
	}
}
