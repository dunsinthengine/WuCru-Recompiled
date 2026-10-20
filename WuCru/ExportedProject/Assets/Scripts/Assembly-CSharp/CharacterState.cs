using UnityEngine;

public abstract class CharacterState : MonoBehaviour
{
	public abstract CharacterStateType StateType { get; }

	private void Awake()
	{
		base.enabled = false;
	}

	public abstract void OnStateEnter(Character character);

	public abstract void OnStateUpdate(Character character);

	public abstract void OnStateExit(Character character);
}
