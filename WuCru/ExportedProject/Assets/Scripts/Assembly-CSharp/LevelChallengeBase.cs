public abstract class LevelChallengeBase
{
	public delegate void OnCompletionDelegate(LevelChallengeBase challenge);

	public delegate void OnFailedDelegate(LevelChallengeBase challenge);

	public delegate void OnCounterChangeDelegate(LevelChallengeBase challenge);

	protected bool isCompleted;

	protected bool hasFailed;

	private bool hasReportedFailed;

	protected int amountCollected;

	protected int amountToCollect;

	public OnCompletionDelegate OnCompletion;

	public OnFailedDelegate OnFailed;

	public OnCounterChangeDelegate OnCounterChange;

	public virtual string _pName
	{
		get
		{
			return "LevelChallengeBase";
		}
	}

	public bool IsCompleted
	{
		get
		{
			return (!ReverseCompletion()) ? isCompleted : (!hasFailed);
		}
	}

	public bool HasFailed
	{
		get
		{
			return hasFailed;
		}
	}

	public abstract bool EvaluateSetup(Level level, GameplayEngine gameplayEngine);

	public abstract bool EvaluateCompletion();

	public virtual bool ReverseCompletion()
	{
		return false;
	}

	public virtual bool HasCounter()
	{
		return false;
	}

	public virtual string IconPath()
	{
		return Paths.challengeDefault;
	}

	public int AmountToCollect()
	{
		return amountToCollect;
	}

	public int AmountCollected()
	{
		return amountCollected;
	}

	public virtual void OnInteractableInteract(Level level, InteractableObject interactable)
	{
	}

	public virtual void OnCollectiblePickup(Level level, Collectible collectible)
	{
	}

	public virtual void OnCharacterDead(Level level, Character character, DamageType damageType)
	{
	}

	public virtual void OnPlayerSpotted(LevelNode position, bool visible)
	{
	}

	public virtual void OnEndFlagReached(Level level)
	{
	}

	public void Update()
	{
		if (!isCompleted)
		{
			isCompleted = EvaluateCompletion();
			if (isCompleted)
			{
				AnalyticsEvents.LogChallengeEnded(GetType().ToString(), true, GameController.Instance.CurrentLevel.Seed);
				if (OnCompletion != null)
				{
					OnCompletion(this);
				}
			}
		}
		if (ReverseCompletion() && hasFailed && !hasReportedFailed)
		{
			hasReportedFailed = true;
			if (OnFailed != null)
			{
				OnFailed(this);
			}
		}
	}

	protected void IncreaseAmountCollected()
	{
		if (amountCollected < amountToCollect && !isCompleted && !hasFailed)
		{
			amountCollected++;
			if (OnCounterChange != null)
			{
				OnCounterChange(this);
			}
		}
	}
}
