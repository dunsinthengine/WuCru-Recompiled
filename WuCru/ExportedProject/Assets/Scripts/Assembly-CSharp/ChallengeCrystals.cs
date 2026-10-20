using System;
using UnityEngine;

public class ChallengeCrystals : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "Crystals";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeAnyCrystal;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_earth);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_electricity);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_energy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_fire);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_ice);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_water);
		if (numObjectsOfType >= 2)
		{
			amountToCollect = UnityEngine.Random.Range(2, Math.Min(numObjectsOfType, 6));
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

	public override bool HasCounter()
	{
		return true;
	}

	public override void OnInteractableInteract(Level level, InteractableObject interactable)
	{
		if (interactable.GetType() == typeof(CrystalShard))
		{
			IncreaseAmountCollected();
		}
	}
}
