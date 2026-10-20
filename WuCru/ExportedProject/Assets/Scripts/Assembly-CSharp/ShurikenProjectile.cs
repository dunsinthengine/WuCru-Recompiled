using UnityEngine;

public class ShurikenProjectile : MonoBehaviour
{
	public Character character;

	public GameObject projectileGO;

	public float speed = 10f;

	public Vector3 target;

	private Vector3 dir;

	private float dist;

	private float maxDist = 20f;

	private float rotationSpeed = 100f;

	private float rotation;

	private void Start()
	{
		dir = (target - projectileGO.transform.position).normalized;
		base.gameObject.GetComponent<BoxCollider>().isTrigger = true;
		dist = 0f;
		MeleeWeaponTrail component = GetComponent<MeleeWeaponTrail>();
		DamageType type = DamageType.SneakAttack;
		GamePlayer.Instance.CombatManager.SetWeaponTrailColor(character, component, type);
		component.Emit = true;
	}

	private void Update()
	{
		Vector3 vector = dir * speed * Time.deltaTime;
		dist += vector.magnitude;
		projectileGO.transform.position += vector;
		rotation += rotationSpeed * Time.deltaTime;
		base.transform.Rotate(rotation, 0f, 0f);
		if (dist > maxDist)
		{
			Done(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Character component = other.GetComponent<Character>();
		if (component != null && !component.isPlayer && !component.IsDead())
		{
			if (component.IsElementalBoss())
			{
				if (GamePlayer.Instance.Player.Character.elementalPower == component.elementalPower)
				{
					Debug.Log("HIT ENEMY");
					component.TakeDamage(Mathf.RoundToInt((float)GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.SneakAttack) * 0.8f), base.gameObject.transform.position, DamageType.SneakAttack);
					GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.red);
					Done(true);
					AIAgent agent = (component.Controller as AIController).agent;
					agent.SetMaximumAlert();
					agent.SetState(AIAgentState.Alert);
				}
				else
				{
					AIAgent agent2 = (component.Controller as AIController).agent;
					if (agent2.State < AIAgentState.Alert)
					{
						agent2.SetMaximumAlert();
						agent2.SetState(AIAgentState.Alert);
					}
					Done(false);
				}
			}
			else
			{
				Debug.Log("HIT ENEMY");
				component.TakeDamage(Mathf.RoundToInt((float)GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.SneakAttack) * 0.8f), base.gameObject.transform.position, DamageType.SneakAttack);
				GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.red);
				Done(true);
			}
		}
		if (other.gameObject.tag == "LevelMesh")
		{
			Done(false);
		}
	}

	private void Done(bool didDamage)
	{
		Object.Destroy(base.gameObject);
	}
}
