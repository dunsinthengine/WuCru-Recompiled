public class ChallengeDefault : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "Default";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeStuds;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.Studs);
		if (numObjectsOfType >= 5)
		{
			amountToCollect = 5;
		}
		else
		{
			amountToCollect = numObjectsOfType;
		}
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

	public override bool HasCounter()
	{
		return true;
	}

	public override void OnCollectiblePickup(Level level, Collectible collectible)
	{
		if (collectible.GetType() == typeof(Studs))
		{
			IncreaseAmountCollected();
		}
	}
}
