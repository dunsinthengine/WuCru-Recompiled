using System;
using UnityEngine;

public class ChallengeGUI : MonoBehaviour
{
	private ChallengeBox challengeBox;

	private int lastCounterValue;

	private bool init;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (init)
		{
			return;
		}
		init = true;
		challengeBox = GetComponent<ChallengeBox>();
		challengeBox.Initialize();
		if (GameController.Instance.CurrentLevel.Challenge != null)
		{
			challengeBox.SetChallengeSprite(Resources.Load<Sprite>(GameController.Instance.CurrentLevel.Challenge.IconPath()));
			if (GameController.Instance.CurrentLevel.Challenge.HasCounter())
			{
				challengeBox.SetChallengeCounter(0, GameController.Instance.CurrentLevel.Challenge.AmountToCollect(), false);
				lastCounterValue = 0;
			}
			LevelChallengeBase challenge = GameController.Instance.CurrentLevel.Challenge;
			challenge.OnCompletion = (LevelChallengeBase.OnCompletionDelegate)Delegate.Combine(challenge.OnCompletion, new LevelChallengeBase.OnCompletionDelegate(OnChallengeCompletion));
			LevelChallengeBase challenge2 = GameController.Instance.CurrentLevel.Challenge;
			challenge2.OnFailed = (LevelChallengeBase.OnFailedDelegate)Delegate.Combine(challenge2.OnFailed, new LevelChallengeBase.OnFailedDelegate(OnChallengeFailed));
			challengeBox.AnimIn();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (GameController.Instance.CurrentLevel != null && GameController.Instance.CurrentLevel.Challenge.HasCounter() && !GameController.Instance.CurrentLevel.Challenge.IsCompleted && (lastCounterValue != GameController.Instance.CurrentLevel.Challenge.AmountCollected() || Input.GetKeyDown(KeyCode.T)))
		{
			lastCounterValue = GameController.Instance.CurrentLevel.Challenge.AmountCollected();
			challengeBox.SetChallengeCounter(lastCounterValue, GameController.Instance.CurrentLevel.Challenge.AmountToCollect());
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			challengeBox.SetChallengeComplete();
		}
	}

	public void OnChallengeCompletion(LevelChallengeBase challenge)
	{
		challengeBox.SetChallengeComplete();
	}

	public void OnChallengeFailed(LevelChallengeBase challenge)
	{
		challengeBox.SetChallengeFailed();
	}
}
