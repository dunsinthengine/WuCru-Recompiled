using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCompletionManager : MonoBehaviour
{
	private List<LevelCompletionCondition> conditions = new List<LevelCompletionCondition>();

	private List<bool> completed = new List<bool>();

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		AddCondition(LevelCompletionCondition.ReachEndFlag);
		if (GameController.Instance.ObjectiveController.CurrentObjective != null)
		{
			LevelCompletionCondition[] completionConditions = ObjectiveData.GetCompletionConditions(GameController.Instance.ObjectiveController.CurrentObjective);
			if (completionConditions != null)
			{
				LevelCompletionCondition[] array = completionConditions;
				foreach (LevelCompletionCondition condition in array)
				{
					AddCondition(condition);
				}
			}
		}
		else
		{
			Debug.LogError("No CurrentObjective to set LevelCompletionManager from!");
		}
		GameController instance = GameController.Instance;
		instance.OnReachFlag = (GameController.OnReachFlagDelegate)Delegate.Combine(instance.OnReachFlag, new GameController.OnReachFlagDelegate(OnReachFlag));
		SetupLevelObjectSubscribers();
	}

	public void CheatComplete()
	{
		foreach (LevelCompletionCondition condition in conditions)
		{
			if ((condition == LevelCompletionCondition.RescueFriend || condition == LevelCompletionCondition.CollectItem) && GameController.Instance.ObjectiveController.CurrentObjective != null)
			{
				ObjectiveLine[] lines = GameController.Instance.ObjectiveController.CurrentObjective.lines;
				foreach (ObjectiveLine objectiveLine in lines)
				{
					if (GameController.Instance.CurrentLevel == null || objectiveLine.seed == GameController.Instance.CurrentLevel.Seed)
					{
						switch (objectiveLine.patternID)
						{
						case ObjectivePatternID.RescueCole:
							ReportRescueFriend(Team.GetID(MinifigType.Cole, 0));
							break;
						case ObjectivePatternID.RescueZane:
							ReportRescueFriend(Team.GetID(MinifigType.Zane, 0));
							break;
						case ObjectivePatternID.RescueJay:
							ReportRescueFriend(Team.GetID(MinifigType.Jay, 0));
							break;
						case ObjectivePatternID.RescueKai:
							ReportRescueFriend(Team.GetID(MinifigType.Kai, 0));
							break;
						case ObjectivePatternID.RescueNyah:
							ReportRescueFriend(Team.GetID(MinifigType.Nya, 0));
							break;
						case ObjectivePatternID.SamuraiXCave:
							ReportRescueFriend(Team.GetID(MinifigType.Nya, 0));
							break;
						}
					}
					GameController.Instance.ObjectiveController.OnObjectiveCompleted(objectiveLine);
				}
			}
			if (condition == LevelCompletionCondition.KillBoss)
			{
				ReportCondition(LevelCompletionCondition.KillBoss);
			}
			if (condition == LevelCompletionCondition.ReachEndFlag)
			{
				ReportCondition(LevelCompletionCondition.ReachEndFlag);
			}
			if (condition == LevelCompletionCondition.CollectItem)
			{
				ReportCondition(LevelCompletionCondition.CollectItem);
			}
		}
		if (GameController.Instance.CurrentLevel != null)
		{
			GameController.Instance.GetComponent<GameControllerStatePlaying>().EndLevel();
		}
	}

	public void ReportRescueFriend(int ninjaID)
	{
		if (ninjaID != 50 && !GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Contains(ninjaID))
		{
			GameController.Instance.PlayerProfile.UnlockNinja(ninjaID);
		}
		ReportCondition(LevelCompletionCondition.RescueFriend);
	}

	public void SetupLevelObjectSubscribers()
	{
		if (GameController.Instance.CurrentLevel == null)
		{
			return;
		}
		AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
		foreach (AIAgent aIAgent in agents)
		{
			if (!aIAgent.isFriend)
			{
				aIAgent.OnAIAgentDead = (AIAgent.OnAIAgentDeadDelegate)Delegate.Combine(aIAgent.OnAIAgentDead, new AIAgent.OnAIAgentDeadDelegate(OnEnemyKilled));
			}
		}
		LevelNode[,] nodes = GameController.Instance.CurrentLevel.Nodes;
		int length = nodes.GetLength(0);
		int length2 = nodes.GetLength(1);
		for (int j = 0; j < length; j++)
		{
			for (int k = 0; k < length2; k++)
			{
				LevelNode levelNode = nodes[j, k];
				foreach (GameObject @object in levelNode.Objects)
				{
					Collectible componentInChildren = @object.GetComponentInChildren<Collectible>();
					if (componentInChildren != null)
					{
						componentInChildren.OnPickupTrigger = (Collectible.OnPickupDelegate)Delegate.Combine(componentInChildren.OnPickupTrigger, new Collectible.OnPickupDelegate(OnCollectiblePickup));
					}
					InteractableObject componentInChildren2 = @object.GetComponentInChildren<InteractableObject>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.OnInteract = (InteractableObject.OnInteractDelegate)Delegate.Combine(componentInChildren2.OnInteract, new InteractableObject.OnInteractDelegate(OnInteractableInteract));
					}
				}
			}
		}
	}

	public bool HasCompleted()
	{
		if (completed.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (bool item in completed)
		{
			if (!item)
			{
				return false;
			}
			num++;
		}
		return true;
	}

	public void AddCondition(LevelCompletionCondition condition)
	{
		conditions.Add(condition);
		completed.Add(false);
	}

	public void OnCollectiblePickup(Level level, Collectible collectible)
	{
	}

	public void OnInteractableInteract(Level level, InteractableObject interactable)
	{
		if (interactable.GetType() == typeof(HoloLog) || interactable.GetType() == typeof(BiteBike) || interactable.GetType() == typeof(CaveEntrance))
		{
			ReportCondition(LevelCompletionCondition.CollectItem);
		}
	}

	public void OnEnemyKilled(Level level, AIAgent aiAgent)
	{
		if (aiAgent.isBoss)
		{
			ReportCondition(LevelCompletionCondition.KillBoss);
		}
		if (aiAgent.GetAgentType() == AIAgentType.StoneWarrior)
		{
			HandleStoneWarriorDeath(aiAgent.Character);
		}
		else if (GameController.Instance.ObjectiveController.ShouldEnemySpawnObject(aiAgent))
		{
			string spawnTypePrefabName = GameController.Instance.ObjectiveController.ObjectToSpawn();
			aiAgent.Character.SpawnObject(spawnTypePrefabName);
		}
	}

	public void HandleStoneWarriorDeath(Character character = null)
	{
	}

	public void HandleStoneWarriorProgress()
	{
		LevelExtensionController.LevelExtensionSettings levelExtensionSettings = GameController.Instance.LevelExtensionController.GetLevelExtensionSettings("DayOfTheDeparted");
		if (levelExtensionSettings.collected <= levelExtensionSettings.total)
		{
			DOTDProgressBar dOTDProgressBar = UnityEngine.Object.FindObjectOfType<DOTDProgressBar>();
			dOTDProgressBar.IncreaseCounter(levelExtensionSettings.collected - 1, levelExtensionSettings.collected, levelExtensionSettings.total);
		}
		int num = GameController.Instance.CurrentLevel.Agents.Count((AIAgent a) => !a.Character.IsDead() && a.GetAgentType() == AIAgentType.StoneWarrior);
		int num2 = UnityEngine.Object.FindObjectsOfType<StoneWarriorDrop>().Length;
		if (num == 0 && num2 <= 1)
		{
			(GameController.Instance.currentState as GameControllerStatePlaying).RevertEnvironmentSettings();
		}
	}

	public void OnReachFlag(Level level)
	{
		Debug.Log("CompletionManager OnReachFlag");
		if (level.LevelTheme.index == level.LevelThemeCollection.themes.Count - 1)
		{
			ReportCondition(LevelCompletionCondition.ReachEndFlag);
		}
	}

	private void ReportCondition(LevelCompletionCondition condition)
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (conditions[i] == condition && !completed[i])
			{
				completed[i] = true;
			}
		}
	}
}
