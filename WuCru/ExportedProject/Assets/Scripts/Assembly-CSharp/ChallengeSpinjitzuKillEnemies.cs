using System;
using UnityEngine;

public class ChallengeSpinjitzuKillEnemies : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "SpinjitzuDefeatEnemies";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeSpinjitzuEnemies;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		if (GameController.Instance.PlayerProfile.SpinjitzuCount <= 0)
		{
			return false;
		}
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.GruntEnemy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.ScoutEnemy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WarriorEnemy);
		if (GameController.Instance.PlayerProfile.SpinjitzuCount > 1 && numObjectsOfType >= 2)
		{
			amountToCollect = UnityEngine.Random.Range(2, Math.Min(numObjectsOfType, 7));
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

	public override void OnCharacterDead(Level level, Character character, DamageType damageType)
	{
		if (!character.isPlayer && damageType == DamageType.Spinjitzu)
		{
			IncreaseAmountCollected();
		}
	}
}
