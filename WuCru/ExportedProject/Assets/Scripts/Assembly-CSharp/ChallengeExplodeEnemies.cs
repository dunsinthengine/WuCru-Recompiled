using System;
using UnityEngine;

public class ChallengeExplodeEnemies : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "ExplodeEnemies";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeExplodeEnemies;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		int num = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard);
		switch (GameController.Instance.PlayerProfile.ElementalPower)
		{
		case ElementalPower.Earth:
			num += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_earth);
			break;
		case ElementalPower.Electricity:
			num += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_electricity);
			break;
		case ElementalPower.Energy:
			num += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_energy);
			break;
		case ElementalPower.Fire:
			num += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_fire);
			break;
		case ElementalPower.Ice:
			num += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_ice);
			break;
		case ElementalPower.Water:
			num += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_water);
			break;
		}
		int numObjectsOfType = level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.GruntEnemy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.ScoutEnemy);
		numObjectsOfType += level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WarriorEnemy);
		if (num >= 2 && numObjectsOfType >= 2)
		{
			amountToCollect = UnityEngine.Random.Range(2, Math.Min(numObjectsOfType, 3));
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
		if (!character.isPlayer && damageType == DamageType.Explosion)
		{
			IncreaseAmountCollected();
		}
	}
}
