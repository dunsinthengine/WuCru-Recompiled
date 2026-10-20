public class ChallengeKillAllEnemies : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "DefeatAllEnemies";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeKillAllEnemies;
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

	public override void OnCharacterDead(Level level, Character character, DamageType damageType)
	{
		if (!character.isPlayer)
		{
			IncreaseAmountCollected();
		}
	}
}
