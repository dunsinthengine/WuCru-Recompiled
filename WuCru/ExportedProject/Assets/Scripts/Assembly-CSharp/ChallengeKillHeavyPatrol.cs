public class ChallengeKillHeavyPatrol : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "DefeatEnemyPatrol";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeKillHeavyPatrol;
	}

	public override bool HasCounter()
	{
		return true;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		if (gameplayEngine.HasSetupPattern(typeof(HeavyPatrol)))
		{
			amountToCollect = 3;
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
		if (!character.isPlayer && (character.Controller as AIController).agent.HasObjectTag("heavyPatrol"))
		{
			IncreaseAmountCollected();
		}
	}
}
