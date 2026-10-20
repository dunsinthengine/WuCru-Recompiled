using System.Collections;
using UnityEngine;

public class Campfire : InteractableObject
{
	public enum CampFireState
	{
		Idle = 0,
		Distract = 1,
		Explode = 2
	}

	private float tapTime;

	public ParticleSystem distractFX;

	public ParticleSystem explodeFX;

	public float pickDistance = 6f;

	public float explodeTimer = 3f;

	public float beaconEventRadius = 8f;

	public int explosionDamage = 100;

	public float explosionRadius = 3f;

	public CampFireState State;

	public override int PickingPriority()
	{
		return 13;
	}

	protected override void OnInitialize()
	{
		tapTime = 0f;
		State = CampFireState.Idle;
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		float num = Vector3.Distance(character.transform.position, base.transform.position);
		if (num < pickDistance && State == CampFireState.Idle)
		{
			distractFX.Play();
			StartCoroutine(Foo());
			State = CampFireState.Distract;
			AIBeacon.Instance.ReportEvent(null, levelNode, base.transform.position, beaconEventRadius, 1f);
			tapTime = Time.time;
			Debug.Log("Distract! tapTime: " + tapTime);
		}
	}

	protected override void OnUpdate()
	{
		if (State == CampFireState.Distract)
		{
			if (Input.GetMouseButton(0) && IsMouseOverObject())
			{
				if (!(tapTime > 0f) || !(Time.time > tapTime + explodeTimer))
				{
					return;
				}
				explodeFX.Play();
				State = CampFireState.Explode;
				Character[] array = Object.FindObjectsOfType<Character>();
				for (int i = 0; i < array.Length; i++)
				{
					if (Vector3.Distance(base.transform.position, array[i].transform.position) <= explosionRadius && !array[i].isPlayer)
					{
						array[i].TakeDamage(explosionDamage, base.transform.position, DamageType.Explosion);
					}
				}
				Debug.Log("Explode! tapTime: " + (tapTime + explodeTimer) + " time: " + Time.time);
			}
			else if (tapTime > 0f)
			{
				Debug.Log("Explode timer reset");
				tapTime = 0f;
				State = CampFireState.Idle;
			}
		}
		else if (State == CampFireState.Explode && !explodeFX.IsAlive())
		{
			tapTime = 0f;
			State = CampFireState.Idle;
			Debug.Log("Explode end tapTime: " + tapTime + " time: " + Time.time);
		}
	}

	private IEnumerator Foo()
	{
		Debug.Log("Playing");
		yield return StartCoroutine(Door.Wait(1f));
		Debug.Log("Stop");
		distractFX.Stop();
	}

	protected override void OnEnter(Character character)
	{
	}

	protected override void OnExit(Character character)
	{
	}
}
