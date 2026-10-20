public class ChallengeMetalChest : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "MetalChest";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeMetalChest;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.MetalChest);
		if (numObjectsOfType <= 0)
		{
			return false;
		}
		amountToCollect = numObjectsOfType;
		return true;
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
		if (interactable.GetType() == typeof(Chest) && (interactable as Chest).chestType == ChestType.Metal)
		{
			IncreaseAmountCollected();
		}
	}
}
