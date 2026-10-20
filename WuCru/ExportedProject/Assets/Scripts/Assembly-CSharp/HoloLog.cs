using UnityEngine;

public class HoloLog : InteractableObject
{
	protected override void OnInitialize()
	{
	}

	protected override void OnEnter(Character character)
	{
		Debug.Log("HoloLog on enter");
		if (OnInteract != null)
		{
			OnInteract(GameController.Instance.CurrentLevel, this);
			(GameController.Instance.currentState as GameControllerStatePlaying).OnIngameObjectiveReached();
		}
		else
		{
			Debug.LogWarning("No subscribers!");
		}
		ParticleSystem componentInChildren = base.gameObject.transform.parent.GetComponentInChildren<ParticleSystem>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(false);
		}
		base.gameObject.SetActive(false);
	}

	protected override void OnExit(Character character)
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
	}

	protected override void OnUpdate()
	{
	}
}
