public class ChallengeGuardedChest : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "GuardedChest";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeGuardedChest;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		if (gameplayEngine.HasSetupPattern(typeof(MiniStronghold)))
		{
			amountToCollect = 1;
			return true;
		}
		return false;
	}

	public override bool EvaluateCompletion()
	{
		if (amountCollected >= amountToCollect)
		{
			return true;
		}
		return false;
	}

	public override void OnInteractableInteract(Level level, InteractableObject interactable)
	{
		if (interactable is Chest && interactable.HasObjectTag("guarded"))
		{
			IncreaseAmountCollected();
		}
	}
}
