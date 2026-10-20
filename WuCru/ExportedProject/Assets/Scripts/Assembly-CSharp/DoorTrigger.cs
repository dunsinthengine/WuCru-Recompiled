using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : InteractableObject
{
	private Coroutine animateHandleCoroutine;

	public Transform pivot;

	public float rotateSpeed = 90f;

	public float rotatedAngle = 30f;

	[Header("Audio")]
	[SerializeField]
	private string doorTriggerAudioEvent;

	public override int PickingPriority()
	{
		return 10;
	}

	protected override void OnInitialize()
	{
		Vector3 eulerAngles = pivot.localRotation.eulerAngles;
		eulerAngles.y = 0f - rotatedAngle;
		pivot.localRotation = Quaternion.Euler(eulerAngles);
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
		List<Door> list = new List<Door>(Object.FindObjectsOfType<Door>());
		list.RemoveAll((Door x) => x.Type != Door.DoorType.TriggeredDoor);
		if (list.Count == 0)
		{
			Debug.LogError("No triggered doors in level");
			return;
		}
		if (list.Count > 1)
		{
			Debug.LogWarning("More than 1 triggered door in level");
		}
		for (int num = 0; num < list.Count; num++)
		{
			list[num].OpenDoor();
			list[num].StartCloseDoorTimer();
		}
		if (animateHandleCoroutine != null)
		{
			StopCoroutine(animateHandleCoroutine);
			animateHandleCoroutine = null;
		}
		AudioPlayer.PlaySound(doorTriggerAudioEvent, base.gameObject);
		animateHandleCoroutine = StartCoroutine(AnimateHandle());
	}

	private IEnumerator AnimateHandle()
	{
		StartCoroutine(RotateHandle(rotatedAngle));
		yield return StartCoroutine(Door.Wait(8f));
		yield return StartCoroutine(RotateHandle(0f - rotatedAngle));
		animateHandleCoroutine = null;
	}

	private IEnumerator RotateHandle(float angle)
	{
		if (angle < 0f)
		{
			angle += 360f;
		}
		Vector3 euler;
		while (true)
		{
			euler = pivot.localRotation.eulerAngles;
			float currentAngle = euler.y;
			float delta = angle - currentAngle;
			if (delta < -180f)
			{
				delta += 360f;
			}
			if (delta > 180f)
			{
				delta -= 360f;
			}
			float step = Mathf.Sign(delta) * rotateSpeed * Time.deltaTime;
			if (Mathf.Abs(delta) < Mathf.Abs(step))
			{
				break;
			}
			euler.y += step;
			pivot.localRotation = Quaternion.Euler(euler);
			yield return null;
		}
		euler.y = angle;
		pivot.localRotation = Quaternion.Euler(euler);
	}

	protected override void OnExit(Character character)
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		character.Controller.WalkTo(levelNode);
	}
}
