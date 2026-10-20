using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StoneWarriorDrop : Collectible
{
	[SerializeField]
	private GameObject lamp;

	[SerializeField]
	private GameObject innerLamp;

	private bool isFlying;

	public override void StartAttract()
	{
	}

	protected override void OnPickup(Character character)
	{
		if (!character.IsInCombat())
		{
			StartCoroutine(FlyAway());
			GetComponent<BoxCollider>().enabled = false;
		}
	}

	private IEnumerator DelayedAttract()
	{
		yield return new WaitForSeconds(1.2f);
		_003CStartAttract_003E__BaseCallProxy0();
	}

	private void Update()
	{
		if (!isFlying)
		{
			lamp.transform.Rotate(Vector3.up, Time.deltaTime * 42f, Space.World);
		}
	}

	private IEnumerator FlyAway()
	{
		isFlying = true;
		float startTime = Time.timeSinceLevelLoad;
		float startY = lamp.transform.position.y;
		Vector3 speed = Vector3.down * 0.0025f;
		bool hasSendProgress = false;
		float distTravelled = 0f;
		Quaternion innerLampStartRot = innerLamp.transform.localRotation;
		while (lamp.transform.position.y < startY + 6f)
		{
			distTravelled = lamp.transform.position.y - startY;
			innerLamp.transform.localRotation = Quaternion.Lerp(innerLampStartRot, Quaternion.Euler(270f, 0f, 0f), distTravelled / 1.5f);
			lamp.transform.Rotate(Vector3.up, Time.deltaTime * 30f, Space.World);
			speed.y += 0.015f * Time.deltaTime;
			speed.x -= 0.005f * Time.deltaTime;
			speed.z += 0.005f * Time.deltaTime;
			lamp.transform.position += speed;
			if (!hasSendProgress && distTravelled > 0.2f && GameController.Instance.currentState is GameControllerStatePlaying)
			{
				hasSendProgress = true;
				(GameController.Instance.currentState as GameControllerStatePlaying).CompletionMgr.HandleStoneWarriorProgress();
			}
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(2f);
		ParticleSystem[] particleSystem = lamp.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < particleSystem.Length; i++)
		{
			particleSystem[i].Stop();
		}
		Object.Destroy(base.gameObject);
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CStartAttract_003E__BaseCallProxy0()
	{
		base.StartAttract();
	}
}
