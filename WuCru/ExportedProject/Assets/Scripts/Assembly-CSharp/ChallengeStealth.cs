public class ChallengeStealth : LevelChallengeBase
{
	public override string _pName
	{
		get
		{
			return "Stealth";
		}
	}

	public override string IconPath()
	{
		return Paths.challengeStealth;
	}

	public override bool EvaluateSetup(Level level, GameplayEngine gameplayEngine)
	{
		if (gameplayEngine.HasSetupPattern(typeof(GuardedChokepoint)))
		{
			return false;
		}
		return true;
	}

	public override bool EvaluateCompletion()
	{
		return false;
	}

	public override void OnPlayerSpotted(LevelNode position, bool visible)
	{
		if (!hasFailed)
		{
			hasFailed = true;
		}
	}

	public override bool ReverseCompletion()
	{
		return true;
	}

	public override void OnEndFlagReached(Level level)
	{
		if (!hasFailed)
		{
			isCompleted = true;
			if (OnCompletion != null)
			{
				OnCompletion(this);
			}
		}
	}
}
