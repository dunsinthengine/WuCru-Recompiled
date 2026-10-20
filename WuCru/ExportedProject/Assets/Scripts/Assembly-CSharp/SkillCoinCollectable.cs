using System.Collections;
using UnityEngine;

public class SkillCoinCollectable : Collectible
{
	public ParticleSystem pickupFX;

	public float attractionDistance = 1.8f;

	public float rotationSpeed = 42f;

	private bool isAttracted;

	private bool _startedPickup;

	private void Start()
	{
		rotationSpeed += Random.Range(-1f, 1f);
	}

	public void SetAttracted()
	{
		isAttracted = true;
	}

	private void Update()
	{
		base.gameObject.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
		if (GamePlayer.Instance.Player != null && Vector3.Distance(base.transform.position, GamePlayer.Instance.Player.Character.transform.position) < attractionDistance && !isAttracted && levelNode.Elevation == GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation)
		{
			isAttracted = true;
			StartAttract();
		}
	}

	protected override void OnPickup(Character character)
	{
		StartCoroutine(DoPickup());
	}

	private IEnumerator DoPickup()
	{
		if (!_startedPickup)
		{
			_startedPickup = true;
			pickupFX.Play();
			while (!pickupFX.isStopped)
			{
				yield return new WaitForSeconds(0.1f);
			}
			Object.Destroy(base.gameObject);
			Debug.Log("COLLECTED A SKILL COIN!");
			GameController.Instance.PlayerProfile.SkillPointDropCount++;
			GameController.Instance.PlayerProfile.MarkSkillCoinCollectedForCurrentLevel();
		}
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
