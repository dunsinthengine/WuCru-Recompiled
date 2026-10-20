using System.Collections;
using UnityEngine;

public class EnemyCopter : TraversalEnemy
{
	public enum CopterState
	{
		Enter = 0,
		Brake = 1,
		AttackDelay = 2,
		BeginFiring = 3,
		Hover = 4,
		BeginExit = 5,
		Exiting = 6,
		Finished = 7
	}

	public CopterState copterState;

	[SerializeField]
	private string shootAudioEvent = "npc/traversal/enemy/shoots/light";

	[SerializeField]
	private string missileTrailAudioEvent = "npc/traversal/missile/trail";

	public Vector3 velocity;

	public CopterState _CopterState
	{
		set
		{
			copterState = value;
		}
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void OnStateChange()
	{
		base.OnStateChange();
		TraversalEnemyState traversalEnemyState = state;
		if (traversalEnemyState == TraversalEnemyState.Visible)
		{
			StartCoroutine(AttackPlayer());
		}
	}

	private IEnumerator AttackPlayer()
	{
		TraversalController traversalController = TraversalController.Instance;
		int playerVehicleID = 0;
		if (playerVehicleID == -1)
		{
			playerVehicleID = 3;
		}
		VehicleDefinition def = VehicleDefinition.vehicleDefinitions[playerVehicleID];
		velocity = Vector3.zero;
		float startTime = 0f;
		float hoverDuration = Random.Range(traversalController.enemyHoverTimeMin, traversalController.enemyHoverTimeMax);
		float enemyAcceleration = (float)def.speed[0] / 0.5f;
		float targetSpeed = traversalController.vehicleBaseSpeed;
		float brakingDistance = targetSpeed * targetSpeed / (2f * enemyAcceleration);
		float startingPosition = base.transform.position.x - traversalController.traversalCamera.transform.position.x;
		_CopterState = CopterState.Enter;
		while (base.transform.position.x - traversalController.traversalCamera.transform.position.x > startingPosition - brakingDistance - (float)Random.Range(20, 60) && (bool)traversalController.car)
		{
			yield return null;
		}
		_CopterState = CopterState.Brake;
		Vector3 targetVelocity = traversalController.vehicleBaseDirection * (targetSpeed - Random.Range(0f, traversalController.enemyHoverSpeed));
		while (velocity.sqrMagnitude < targetVelocity.sqrMagnitude && (bool)traversalController.car)
		{
			velocity = Vector3.MoveTowards(velocity, targetVelocity, enemyAcceleration * Time.deltaTime);
			base.transform.position += velocity * Time.deltaTime;
			yield return null;
		}
		_CopterState = CopterState.AttackDelay;
		startTime = Time.timeSinceLevelLoad;
		float enemyAttackDelay = Random.Range(traversalController.enemyAttackDelayMin, traversalController.enemyAttackDelayMax);
		while (Time.timeSinceLevelLoad - startTime < enemyAttackDelay && (bool)traversalController.car)
		{
			base.transform.position += velocity * Time.deltaTime;
			yield return null;
		}
		_CopterState = CopterState.BeginFiring;
		StartCoroutine(BeginFiring(base.gameObject));
		_CopterState = CopterState.Hover;
		startTime = Time.timeSinceLevelLoad;
		while (Time.timeSinceLevelLoad - startTime < hoverDuration && (bool)traversalController.car)
		{
			base.transform.position += velocity * Time.deltaTime;
			yield return null;
		}
		_CopterState = CopterState.BeginExit;
		targetVelocity = traversalController.vehicleBaseSpeed * -traversalController.vehicleBaseDirection;
		while (Mathf.Abs(velocity.sqrMagnitude - targetVelocity.sqrMagnitude) > 0f)
		{
			velocity = Vector3.MoveTowards(velocity, targetVelocity, enemyAcceleration * Time.deltaTime);
			base.transform.position += velocity * Time.deltaTime;
			yield return null;
		}
		_CopterState = CopterState.Exiting;
		while (IsVisible())
		{
			base.transform.position += velocity * Time.deltaTime;
			yield return null;
		}
		_CopterState = CopterState.Finished;
	}

	private IEnumerator BeginFiring(GameObject go)
	{
		while (!IsVisible())
		{
			yield return null;
		}
		TraversalController traversalController = TraversalController.Instance;
		Vehicle vehicle = go.GetComponent<Vehicle>();
		while ((bool)traversalController.car && IsVisible())
		{
			Vector3 carOffset = Vector3.zero;
			Vector3 delta = traversalController.car.transform.position + carOffset - go.transform.position;
			float r = Random.value;
			delta = Quaternion.Euler(0f, 0f, (r > 1f - traversalController.enemyMissileMisfireChance / 2f) ? traversalController.enemyMissileMisfireAngle : ((!(r > 1f - traversalController.enemyMissileMisfireChance)) ? 0f : (0f - traversalController.enemyMissileMisfireAngle))) * delta;
			if (delta.x < carOffset.x)
			{
				GameObject m = Object.Instantiate(traversalController.missile, vehicle.missileSpawnPoint, Quaternion.LookRotation(delta, Vector3.up));
				m.SetActive(true);
				TraversalMissile traversalMissile = m.GetComponent<TraversalMissile>();
				traversalMissile.baseVelocity = velocity;
				traversalMissile.owner = vehicle;
				traversalMissile.speed = traversalController.enemyMissileSpeed;
				traversalMissile.acceleration = 100f;
				traversalMissile.delay = traversalController.enemyMissileDelay;
				AudioPlayer.PlaySound(shootAudioEvent, go);
				AudioPlayer.PlaySound(missileTrailAudioEvent, m);
				for (int i = 0; i < vehicle.muzzleFlashEmitters.Length; i++)
				{
					if ((bool)vehicle.muzzleFlashParticleSystem)
					{
						GameObject gameObject = Object.Instantiate(vehicle.muzzleFlashParticleSystem);
						gameObject.transform.SetParent(vehicle.muzzleFlashEmitters[i], false);
						gameObject.transform.localPosition = Vector3.zero;
					}
				}
				yield return new WaitForSeconds(Random.Range(1f, 2f));
			}
			else
			{
				yield return null;
			}
		}
	}
}
