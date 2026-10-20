using System;
using UnityEngine;

public class ChallengeCollectStuds : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "CollectStuds";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeStuds;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.Studs);
		if (level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.Studs) >= 10)
		{
			amountToCollect = UnityEngine.Random.Range(10, Math.Min(numObjectsOfType, 26));
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

	public override void OnCollectiblePickup(Level level, Collectible collectible)
	{
		if (collectible.GetType() == typeof(Studs))
		{
			IncreaseAmountCollected();
		}
	}
}
