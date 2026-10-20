using UnityEngine;

public class AIController : MonoBehaviour, ICharacterController
{
	public AIAgent agent;

	private Character character;

	private LevelNode startLocation;

	private AIBehaviour currentBehaviour;

	private float lastUpdateTime;

	private float updateInterval = 0.1f;

	public Character Character
	{
		get
		{
			return character;
		}
	}

	public LevelNode StartLocation
	{
		get
		{
			return startLocation;
		}
	}

	public AIBehaviour CurrentBehaviour
	{
		get
		{
			return currentBehaviour;
		}
	}

	public Character CurrentEnemy
	{
		get
		{
			return null;
		}
	}

	public bool CanSeePlayer()
	{
		return agent.IsPlayerVisible;
	}

	public void SetBehaviour(AIBehaviour behaviour)
	{
		if (currentBehaviour != null)
		{
			if (behaviour != null && currentBehaviour.GetType() == behaviour.GetType())
			{
				return;
			}
			currentBehaviour.Exit(this);
		}
		currentBehaviour = behaviour;
		if (currentBehaviour != null)
		{
			currentBehaviour.Initialize(this);
		}
	}

	private void Awake()
	{
	}

	public void InitializeFromAgent()
	{
		GameObject gameObject = Object.Instantiate(agent.characterPrefab);
		gameObject.transform.parent = base.transform;
		character = gameObject.GetComponent<Character>();
		character.UpdateLevel(GamePlayer.Instance.Level.Nodes);
		character.Speed = 1f;
		character.ForceMove(base.transform.position);
		startLocation = Character.CurrentPathNode;
	}

	public void ToggleHide(bool hidden)
	{
	}

	public void ToggleTrapped(bool trapped)
	{
		if (trapped)
		{
			character.ResetPath();
			character.Animator.SetTrigger(TriggerHash.trapped);
		}
		else if (character.IsTrapped())
		{
			character.Animator.SetTrigger(TriggerHash.untrapped);
		}
	}

	public void Die()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Characters/Stnd_Nindroid_Death"));
		gameObject.transform.position = character.transform.position + Vector3.up * 0.1f;
		gameObject.transform.rotation = character.transform.rotation;
		MenuCharacter component = gameObject.GetComponent<MenuCharacter>();
		CharacterBuilder.ConfigureMinifig(component, character.NinjaID);
		if ((bool)component.hairGameObject)
		{
			component.hairGameObject.AddComponent<Rigidbody>();
			component.hairGameObject.GetComponentInChildren<Collider>().enabled = true;
		}
		if ((bool)component.helmetGameObject)
		{
			component.helmetGameObject.AddComponent<Rigidbody>();
			component.helmetGameObject.GetComponentInChildren<Collider>().enabled = true;
			ParticleSystem[] componentsInChildren = component.helmetGameObject.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i].gameObject);
			}
		}
		if ((bool)component.weaponGameObject)
		{
			component.weaponGameObject.AddComponent<Rigidbody>();
			component.weaponGameObject.GetComponentInChildren<Collider>().enabled = true;
		}
	}

	public void WalkTo(LevelNode target, bool force = false)
	{
		if (!character.IsTrapped())
		{
			character.WalkTo(target);
		}
	}

	private void Update()
	{
		if (lastUpdateTime + updateInterval < Time.time)
		{
			lastUpdateTime = Time.time;
			if (currentBehaviour != null)
			{
				currentBehaviour.Update(this);
			}
		}
	}
}
