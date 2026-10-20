public class ChallengeWoodChest : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "WoodChest";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeWoodChest;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WoodChest);
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
		if (interactable.GetType() == typeof(Chest) && (interactable as Chest).chestType == ChestType.Wood)
		{
			IncreaseAmountCollected();
		}
	}
}
