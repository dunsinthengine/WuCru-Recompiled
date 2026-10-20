using System;
using UnityEngine;

public class ChallengeDismantleTraps : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "DismantleTraps";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeDismantleTraps;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.BearTrap);
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

	public override void OnInteractableInteract(Level level, InteractableObject interactable)
	{
		if (interactable.GetType() == typeof(BearTrap))
		{
			IncreaseAmountCollected();
		}
	}
}
