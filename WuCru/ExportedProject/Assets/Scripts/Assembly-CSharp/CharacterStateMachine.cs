using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
	public Character character;

	private CharacterState currentState;

	private void Awake()
	{
		base.enabled = true;
	}

	public void SetState(CharacterStateType type)
	{
		if (currentState != null)
		{
			currentState.OnStateExit(character);
			Object.Destroy(currentState);
			currentState = null;
		}
		currentState = StateFactory(type);
		if (currentState != null)
		{
			currentState.OnStateEnter(character);
		}
	}

	private CharacterState StateFactory(CharacterStateType type)
	{
		switch (type)
		{
		case CharacterStateType.Idle:
			return base.gameObject.AddComponent<CSIdle>();
		case CharacterStateType.Exploring:
			return base.gameObject.AddComponent<CSExploring>();
		default:
			return null;
		}
	}

	private void Update()
	{
		if (currentState != null)
		{
			currentState.OnStateUpdate(character);
		}
	}
}
