using System.Collections.Generic;
using UnityEngine;

public class ElementalAttack : MonoBehaviour
{
	public enum ElementalAttackState
	{
		NONE = 0,
		CHARGING = 1,
		FLYING = 2,
		EXPLODING = 3
	}

	private ElementalAttackState currentState;

	private ElementalAttackCharger charger;

	private Vector3 target;

	private Character character;

	private float speed = 7.5f;

	private float baseExplosionRange = 3f;

	private int baseDamage = 500;

	private int damFalloffFactor = 1;

	private Vector3 dir;

	private float currentDist;

	private float maxFlyDist = 20f;

	private float rotationSpeed = 100f;

	private float rotation;

	private float chargeAmount;

	[SerializeField]
	private GameObject chargeEffect;

	private ParticleSystem[] chargeParticles;

	private Dictionary<ParticleSystem, float> chargeParticlesSizes;

	[SerializeField]
	private GameObject flyingEffect;

	private ParticleSystem[] flyingParticles;

	private Dictionary<ParticleSystem, float> flyingParticlesSizes;

	[SerializeField]
	private GameObject explodeEffect;

	private ParticleSystem[] explodeParticles;

	private Dictionary<ParticleSystem, float> explodeParticlesSizes;

	private GameObject currentEffect;

	private ParticleSystem[] currentParticles;

	private float startFlyTime;

	public void Initialize(Character character, ElementalAttackCharger charger)
	{
		this.character = character;
		this.charger = charger;
		MeleeWeaponTrail component = GetComponent<MeleeWeaponTrail>();
		DamageType type = DamageType.Explosion;
		GamePlayer.Instance.CombatManager.SetWeaponTrailColor(character, component, type);
		component.Emit = true;
		chargeParticles = chargeEffect.GetComponentsInChildren<ParticleSystem>();
		chargeParticlesSizes = new Dictionary<ParticleSystem, float>();
		for (int i = 0; i < chargeParticles.Length; i++)
		{
			chargeParticlesSizes.Add(chargeParticles[i], chargeParticles[i].startSize);
		}
		chargeEffect.SetActive(false);
		flyingParticles = flyingEffect.GetComponentsInChildren<ParticleSystem>();
		flyingParticlesSizes = new Dictionary<ParticleSystem, float>();
		for (int j = 0; j < flyingParticles.Length; j++)
		{
			flyingParticlesSizes.Add(flyingParticles[j], flyingParticles[j].startSize);
		}
		flyingEffect.SetActive(false);
		explodeParticles = explodeEffect.GetComponentsInChildren<ParticleSystem>();
		explodeParticlesSizes = new Dictionary<ParticleSystem, float>();
		for (int k = 0; k < explodeParticles.Length; k++)
		{
			explodeParticlesSizes.Add(explodeParticles[k], explodeParticles[k].startSize);
		}
		explodeEffect.SetActive(false);
	}

	private void HandleChargerUpdate(float manaAmount, float charge)
	{
		chargeAmount += charge;
	}

	public float GetChargeAmount()
	{
		return chargeAmount;
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		switch (currentState)
		{
		case ElementalAttackState.CHARGING:
			base.transform.position = character.handTransform.position;
			{
				foreach (KeyValuePair<ParticleSystem, float> chargeParticlesSize in chargeParticlesSizes)
				{
					chargeParticlesSize.Key.startSize = chargeParticlesSize.Value * chargeAmount;
				}
				break;
			}
		case ElementalAttackState.FLYING:
		{
			Vector3 vector = dir * speed * Time.deltaTime;
			currentDist += vector.magnitude;
			base.transform.position += vector;
			rotation += rotationSpeed * Time.deltaTime;
			base.transform.Rotate(rotation, 0f, 0f);
			if (currentDist > maxFlyDist)
			{
				FadeAway();
			}
			break;
		}
		case ElementalAttackState.EXPLODING:
		{
			foreach (KeyValuePair<ParticleSystem, float> explodeParticlesSize in explodeParticlesSizes)
			{
				explodeParticlesSize.Key.startSize = explodeParticlesSize.Value * chargeAmount;
			}
			break;
		}
		}
	}

	public void SetTarget(Vector3 target)
	{
		this.target = target;
	}

	public void SetState(ElementalAttackState state)
	{
		Debug.Log("ElementalAttack SetState " + state);
		if (currentEffect != null)
		{
			currentEffect.SetActive(false);
		}
		if (currentParticles != null)
		{
			for (int i = 0; i < currentParticles.Length; i++)
			{
				currentParticles[i].Stop();
			}
		}
		switch (state)
		{
		case ElementalAttackState.CHARGING:
			currentEffect = chargeEffect;
			currentParticles = chargeParticles;
			break;
		case ElementalAttackState.FLYING:
		{
			currentEffect = flyingEffect;
			currentParticles = flyingParticles;
			foreach (KeyValuePair<ParticleSystem, float> flyingParticlesSize in flyingParticlesSizes)
			{
				flyingParticlesSize.Key.startSize = flyingParticlesSize.Value * chargeAmount;
			}
			float num = base.transform.position.y - character.transform.position.y;
			target.y += num;
			dir = (target - base.transform.position).normalized;
			currentDist = 0f;
			startFlyTime = Time.realtimeSinceStartup;
			break;
		}
		case ElementalAttackState.EXPLODING:
			currentEffect = explodeEffect;
			currentParticles = explodeParticles;
			DoExplosion();
			break;
		}
		currentState = state;
		currentEffect.SetActive(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (currentState != ElementalAttackState.FLYING)
		{
			return;
		}
		Character component = other.GetComponent<Character>();
		if (component != null && !component.isPlayer && !component.IsDead())
		{
			SetState(ElementalAttackState.EXPLODING);
			return;
		}
		if (other.gameObject.tag == "LevelMesh")
		{
			SetState(ElementalAttackState.EXPLODING);
			return;
		}
		CrystalShard component2 = other.GetComponent<CrystalShard>();
		if (component2 != null)
		{
			if (component2.elementalPower == ElementalPower.None || component2.elementalPower == component.elementalPower)
			{
				component2.Explode(chargeAmount * 10f);
			}
			SetState(ElementalAttackState.EXPLODING);
			return;
		}
		BellTower component3 = other.GetComponent<BellTower>();
		if (component3 != null)
		{
			component3.SuccessHit(chargeAmount * 10f);
			SetState(ElementalAttackState.EXPLODING);
		}
	}

	public void FadeAway()
	{
		Object.Destroy(base.gameObject);
	}

	private void DoExplosion()
	{
		AudioPlayer.PlaySound("props/dark/crystal/explode", base.gameObject);
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.red);
		InGameCam._pInstance.Shake(0.16f, 0.5f);
		float num = baseExplosionRange * chargeAmount;
		RangeSphere component = ((GameObject)Object.Instantiate(Resources.Load("Prefabs/FXs/RangeSphere"))).GetComponent<RangeSphere>();
		component.transform.parent = base.transform;
		component.transform.localPosition = Vector3.zero;
		component.Show(num, 1f);
		AIAgent[] array = ((GameController.Instance.currentState.GetType() != typeof(GameControllerStatePlaying)) ? EditorStateMachine.Instance.level.Agents : GameController.Instance.CurrentLevel.Agents);
		AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
		foreach (AIAgent aIAgent in agents)
		{
			if (!aIAgent.isFriend)
			{
				float num2 = Vector3.Distance(base.transform.position, aIAgent.Character.transform.position);
				if (num2 <= num)
				{
					int num3 = Mathf.Max(Mathf.RoundToInt(num2), 1);
					int amount = baseDamage / num3 / damFalloffFactor;
					aIAgent.Character.TakeDamage(amount, aIAgent.Character.transform.position, DamageType.Explosion);
				}
			}
		}
	}
}
