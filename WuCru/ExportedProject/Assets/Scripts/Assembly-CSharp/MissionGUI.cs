using System;
using UnityEngine;

public class MissionGUI : MonoBehaviour
{
	private ChallengeBox challengeBox;

	private int lastCounterValue;

	private void Start()
	{
		challengeBox = GetComponent<ChallengeBox>();
		challengeBox.Initialize();
		if (GameController.Instance.MissionController.CurrentMission != null && !GameController.Instance.MissionController.CurrentMission.IsCompleted())
		{
			challengeBox.SetChallengeSprite(Resources.Load<Sprite>(GameController.Instance.MissionController.CurrentMission.MissionIngameImagePath));
			Mission currentMission = GameController.Instance.MissionController.CurrentMission;
			currentMission.OnIncreaseCompletion = (Mission.OnIncreaseCompletionDelegate)Delegate.Combine(currentMission.OnIncreaseCompletion, new Mission.OnIncreaseCompletionDelegate(OnIncreaseCompletion));
			challengeBox.SetChallengeCounter(GameController.Instance.MissionController.CurrentMission.CompletionCount, GameController.Instance.MissionController.CurrentMission.ActionCount, false);
			challengeBox.AnimIn(1.6f);
			if (GameController.Instance.MissionController.CurrentMission.IsCompleted())
			{
				challengeBox.SetChallengeComplete();
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (GameController.Instance.MissionController != null && GameController.Instance.MissionController.CurrentMission != null)
		{
			Mission currentMission = GameController.Instance.MissionController.CurrentMission;
			currentMission.OnIncreaseCompletion = (Mission.OnIncreaseCompletionDelegate)Delegate.Remove(currentMission.OnIncreaseCompletion, new Mission.OnIncreaseCompletionDelegate(OnIncreaseCompletion));
		}
	}

	public void OnIncreaseCompletion(int completionCount, int actionCount)
	{
		challengeBox.SetChallengeCounter(completionCount, actionCount);
		if (GameController.Instance.MissionController.CurrentMission.IsCompleted())
		{
			challengeBox.SetChallengeComplete();
		}
		else
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
		}
	}
}
