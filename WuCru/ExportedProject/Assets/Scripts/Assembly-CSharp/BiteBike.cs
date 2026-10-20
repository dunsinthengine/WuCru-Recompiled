using System.Collections;
using UnityEngine;

public class BiteBike : InteractableObject
{
	public int vehicleID;

	protected override void OnInitialize()
	{
	}

	protected override void OnEnter(Character character)
	{
		Debug.Log("BiteBike on enter");
		if (OnInteract != null)
		{
			OnInteract(GameController.Instance.CurrentLevel, this);
		}
		else
		{
			Debug.LogWarning("No subscribers!");
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

	private IEnumerator ShakeAndDisappear()
	{
		Animator animator = GetComponentInParent<Animator>();
		animator.SetTrigger("enter");
		yield return new WaitForSeconds(1f);
		(GameController.Instance.currentState as GameControllerStatePlaying).OnIngameObjectiveReached();
		Object.Destroy(animator);
		float startTime = Time.time;
		float duration = 1f;
		NinjaTeamGUI gui = Object.FindObjectOfType<NinjaTeamGUI>();
		Vector3 targetPos = Camera.main.ScreenToWorldPoint(gui.imageActiveNinja.transform.position);
		targetPos = new Vector3(targetPos.x - 5f, targetPos.y, base.transform.parent.transform.position.z);
		Vector3 startPos = base.transform.parent.transform.position;
		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			base.transform.parent.gameObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
			base.transform.parent.gameObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
			yield return 0;
		}
		base.transform.parent.gameObject.SetActive(false);
	}
}
