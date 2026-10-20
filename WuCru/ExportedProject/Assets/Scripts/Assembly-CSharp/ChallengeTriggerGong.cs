public class ChallengeTriggerGong : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "TriggerGong";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeTriggerGong;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		if (level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.BellTower) >= 1)
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
		if (interactable.GetType() == typeof(BellTower))
		{
			IncreaseAmountCollected();
		}
	}
}
