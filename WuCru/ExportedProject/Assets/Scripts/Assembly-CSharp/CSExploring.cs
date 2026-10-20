public class CSExploring : CharacterState
{
	public override CharacterStateType StateType
	{
		get
		{
			return CharacterStateType.Exploring;
		}
	}

	public override void OnStateEnter(Character character)
	{
	}

	public override void OnStateExit(Character character)
	{
	}

	public override void OnStateUpdate(Character character)
	{
		character.MoveStep();
		character.Animator.SetFloat(CSAnimVars.movementSpeed, character.Speed);
	}
}
