using Fabric;
using UnityEngine;

public class Bush : InteractableObject
{
	private bool clicked;

	private Character clickedByCharacter;

	private bool tigersprintSet;

	private bool isHidingPlayer;

	private bool _spawnedChicken;

	public override int PickingPriority()
	{
		return 20;
	}

	public void Awake()
	{
		onlyPlayer = true;
		activateBlink = ActivateBlink.ON_GOTO_NOTE;
	}

	protected override void OnEnter(Character character)
	{
		if (character == null)
		{
			Debug.Log("Character is null!");
			return;
		}
		if (character.Controller == null)
		{
			Debug.Log("Character controller is null");
			return;
		}
		if (character.isPlayer && !_spawnedChicken && Random.Range(0, 100) < 10)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("RandomChicken"));
			gameObject.transform.position = base.transform.position;
			_spawnedChicken = true;
		}
		EventManager.Instance.PostEvent("props/bush/rattle", base.gameObject);
		Animator componentInChildren = GetComponentInChildren<Animator>();
		if (componentInChildren != null)
		{
			componentInChildren.SetTrigger("enter");
		}
	}

	protected override void OnExit(Character character)
	{
		if (clicked)
		{
			character.Controller.ToggleHide(false);
			clicked = false;
		}
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		ScreenHud._pInstance.HandleBushFTUE();
		clicked = true;
		tigersprintSet = false;
		clickedByCharacter = character;
		if ((LevelHelper.NodeToVector3(character.CurrentPositionNode) - LevelHelper.NodeToVector3(levelNode)).magnitude < 0.1f)
		{
			Trigger(character);
		}
		else
		{
			character.Controller.WalkTo(levelNode);
		}
	}

	private void Trigger(Character character)
	{
		if (Chest._pWalkingToChest != null)
		{
			int row = Chest._pWalkingToChest.levelNode.Row;
			int col = Chest._pWalkingToChest.levelNode.Col;
			int row2 = levelNode.Row;
			int col2 = levelNode.Col;
			if (Mathf.Abs(row - row2) == 1 || Mathf.Abs(col - col2) == 1)
			{
				Chest._pReturnToBush = this;
				return;
			}
		}
		if (clicked && clickedByCharacter == character)
		{
			character.Controller.ToggleHide(true);
			clicked = false;
			tigersprintSet = false;
			clickedByCharacter = null;
			GamePlayer.Instance.SelectedObject = null;
			StartCoroutine(CenterCharacter());
		}
	}

	protected override void OnUpdate()
	{
		if (clicked && clickedByCharacter != null)
		{
			float num = Vector3.Distance(clickedByCharacter.transform.position, LevelHelper.NodeToVector3(levelNode));
			float num2 = levelNode.Elevation - clickedByCharacter.CurrentPositionNode.Elevation;
			if (clickedByCharacter.CurrentDestination == null || clickedByCharacter.CurrentDestination != levelNode)
			{
				clicked = false;
				tigersprintSet = false;
			}
			else if (num < 2f && num > 1.6f && !tigersprintSet && num2 == 0f && !clickedByCharacter.IsHiding())
			{
				tigersprintSet = true;
				clickedByCharacter.Animator.SetTrigger(TriggerHash.tigersprint);
				GamePlayer.Instance.SelectedObject = null;
			}
		}
	}
}
