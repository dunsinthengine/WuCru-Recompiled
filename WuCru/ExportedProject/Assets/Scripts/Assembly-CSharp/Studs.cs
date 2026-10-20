using System.Collections;
using Fabric;
using UnityEngine;

public class Studs : Collectible
{
	public ParticleSystem pickupFX;

	public MeshRenderer studRenderer;

	public float attractionDistance = 1.8f;

	public float rotationSpeed = 42f;

	public float timeStudPickup;

	public float timeLastStudPickup;

	public float timeSinceLastPickup;

	public float studPitchIncrease;

	private float studPitchFactor = 22f;

	public bool pickedUp;

	private bool isAttracted;

	private StudsCounter studsCounter;

	private void Start()
	{
		rotationSpeed += Random.Range(-1f, 1f);
		studsCounter = Object.FindObjectOfType<StudsCounter>();
	}

	public void SetAttracted()
	{
		isAttracted = true;
	}

	private void Update()
	{
		if (studRenderer.isVisible)
		{
			studRenderer.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed, Space.World);
			if (GamePlayer.Instance.Player != null && Vector3.Distance(base.transform.position, GamePlayer.Instance.Player.Character.transform.position) < attractionDistance && !isAttracted && levelNode.Elevation == GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation)
			{
				isAttracted = true;
				StartAttract();
			}
		}
	}

	protected override void OnPickup(Character character)
	{
		if (!pickedUp)
		{
			pickedUp = true;
			StartCoroutine(DoPickup());
		}
	}

	private IEnumerator DoPickup()
	{
		studRenderer.enabled = false;
		pickupFX.Play();
		timeStudPickup = Time.realtimeSinceStartup;
		timeSinceLastPickup = GamePlayer.Instance.GetTimeSinceLastPickup(timeStudPickup);
		float applyPitchTime = 0.8f;
		if (timeSinceLastPickup < applyPitchTime)
		{
			studPitchIncrease = GamePlayer.Instance.StudPitchIncrease();
		}
		else
		{
			studPitchIncrease = GamePlayer.Instance.StudPitchReset();
		}
		EventManager.Instance.PostEvent("props/stud/pickup", EventAction.SetPitchProperty, 1f + studPitchIncrease / studPitchFactor);
		EventManager.Instance.PostEvent("props/stud/pickup");
		if (studsCounter == null)
		{
			studsCounter = Object.FindObjectOfType<StudsCounter>();
		}
		if (studsCounter != null)
		{
			studsCounter.Pickup();
			GamePlayer.Instance.SetPickupTime();
		}
		while (!pickupFX.isStopped)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
	}

	public override void StartAttract()
	{
		StartCoroutine(DoAttract());
	}

	private IEnumerator DoAttract()
	{
		float startTime = Time.time;
		float t = 0f;
		while (t <= 1f)
		{
			yield return null;
			t = (Time.time - startTime) / attractionDuration;
			if (t > 1f)
			{
				t = 1f;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, GamePlayer.Instance.Player.Character.transform.position, t);
			if (pickupFX.isPlaying)
			{
				break;
			}
		}
	}
}
