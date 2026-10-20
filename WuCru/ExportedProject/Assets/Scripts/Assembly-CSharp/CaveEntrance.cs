using UnityEngine;

public class CaveEntrance : InteractableObject
{
	protected override void OnInitialize()
	{
	}

	protected override void OnEnter(Character character)
	{
		if (!base.HasTriggered)
		{
			Debug.Log("CaveEntrance on enter");
			if (OnInteract != null)
			{
				OnInteract(GameController.Instance.CurrentLevel, this);
			}
			else
			{
				Debug.LogWarning("No subscribers!");
			}
			Animation component = base.gameObject.GetComponent<Animation>();
			component.Play("CaveOpening");
			(GameController.Instance.currentState as GameControllerStatePlaying).OnIngameObjectiveReached();
		}
	}

	protected override void OnExit(Character character)
	{
	}

	protected override void OnUpdate()
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
	}
}
