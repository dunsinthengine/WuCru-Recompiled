using System.Collections.Generic;
using UnityEngine;

public class LevelChallengeSelector
{
	private List<LevelChallengeBase> challenges = new List<LevelChallengeBase>();

	public LevelChallengeSelector()
	{
		challenges.Add(new ChallengeCollectStuds());
		challenges.Add(new ChallengeChests());
		challenges.Add(new ChallengeSneakAttackEnemies());
		challenges.Add(new ChallengeGuardedChest());
		challenges.Add(new ChallengeKillHeavyPatrol());
		challenges.Add(new ChallengeTrapKillEnemies());
		challenges.Add(new ChallengeSpinjitzuKillEnemies());
		challenges.Add(new ChallengeExplodeEnemies());
		challenges.Add(new ChallengeTriggerGong());
		challenges.Add(new ChallengeKillAllEnemies());
		challenges.Add(new ChallengeGoldChest());
		challenges.Add(new ChallengeMetalChest());
		challenges.Add(new ChallengeWoodChest());
		challenges.Add(new ChallengeCrystals());
		challenges.Add(new ChallengeDismantleTraps());
	}

	public void SetupChallenge(Level level, GameplayEngine gameplayEngine)
	{
		List<LevelChallengeBase> potentialChallenges = GetPotentialChallenges(level, gameplayEngine);
		if (potentialChallenges.Count == 0)
		{
			ChallengeDefault challengeDefault = new ChallengeDefault();
			challengeDefault.EvaluateSetup(level, gameplayEngine);
			level.Challenge = challengeDefault;
		}
		else
		{
			level.Challenge = potentialChallenges[Random.Range(0, potentialChallenges.Count)];
			int num = 0;
			while (GameController.Instance.MissionStats.Challenges.ContainsKey(level.Challenge) && potentialChallenges.Count > 1 && num <= 100)
			{
				level.Challenge = potentialChallenges[Random.Range(0, potentialChallenges.Count)];
				num++;
			}
		}
		Debug.Log("Challenge: " + level.Challenge.GetType().ToString() + " potential: " + potentialChallenges.Count);
	}

	public List<LevelChallengeBase> GetPotentialChallenges(Level level, GameplayEngine gameplayEngine)
	{
		List<LevelChallengeBase> list = new List<LevelChallengeBase>();
		foreach (LevelChallengeBase challenge in challenges)
		{
			if (challenge.EvaluateSetup(level, gameplayEngine))
			{
				list.Add(challenge);
			}
		}
		return list;
	}
}
