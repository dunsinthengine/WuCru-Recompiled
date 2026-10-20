using System;
using UnityEngine;

public class ChallengeSneakAttackEnemies : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "SneakAttackEnemies";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeSneakAttack;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.GruntEnemy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.ScoutEnemy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WarriorEnemy);
		if (numObjectsOfType >= 2)
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
		if (!character.isPlayer && damageType == DamageType.SneakAttack)
		{
			IncreaseAmountCollected();
		}
	}
}
