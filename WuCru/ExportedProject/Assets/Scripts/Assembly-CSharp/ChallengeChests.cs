using System;
using UnityEngine;

public class ChallengeChests : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "Chests";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeAnyChest;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.GoldChest);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.MetalChest);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WoodChest);
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
		if (interactable.GetType() == typeof(Chest))
		{
			IncreaseAmountCollected();
		}
	}
}
