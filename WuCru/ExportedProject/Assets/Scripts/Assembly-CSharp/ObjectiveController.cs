using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
	public static LevelUnlockData _desiredUnlock;

	private ObjectiveData currentObjective;

	public ObjectiveData CurrentObjective
	{
		get
		{
			return currentObjective;
		}
		set
		{
			currentObjective = value;
		}
	}

	private void Awake()
	{
		GameController instance = GameController.Instance;
		instance.OnEndLevel = (GameController.OnEndLevelDelegate)Delegate.Combine(instance.OnEndLevel, new GameController.OnEndLevelDelegate(OnEndLevel));
	}

	public void CompleteAll()
	{
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			objectiveLine.Completed = true;
			OnObjectiveCompleted(objectiveLine);
		}
	}

	public void OnObjectiveCompleted(ObjectiveLine line)
	{
		Debug.Log("***** ObjectiveController -> OnObjectiveCompleted(line) *****");
		if (line.objectiveEvent != ObjectiveEvent.None)
		{
			switch (line.objectiveEvent)
			{
			case ObjectiveEvent.UnlockGarageButton:
			{
				GameController.Instance.PlayerProfile.ShowGarageInTopBar = true;
				int iD = Team.GetID(MinifigType.Nya, 0);
				GameController.Instance.PlayerProfile.UnlockNinja(iD);
				break;
			}
			case ObjectiveEvent.UnlockWorldMapButton:
				GameController.Instance.PlayerProfile.ShowWorldMapInTopBar = true;
				break;
			case ObjectiveEvent.UnlockSpinjitzu:
				GameController.Instance.PlayerProfile.ShowSpinjitzuUnlock = true;
				break;
			}
		}
	}

	public bool InitiateObjective(int season, int chapter, bool cheat = false)
	{
		bool flag = false;
		if (currentObjective != null)
		{
			flag = currentObjective.season != season;
			Debug.Log("CurrentObjectiveSeason = " + currentObjective.season + " / proposed season: " + season);
		}
		if (IsCurrentObjectiveCompleted() || cheat || flag)
		{
			currentObjective = ObjectiveData.Generate(GetObjectiveLinesForSeasonChapter(season, chapter));
			Debug.Log("Initiating Objective season: " + season + ", chapter: " + chapter);
			DebugLogCurrentObjective();
			bool flag2 = GameController.Instance.PlayerProfile.IsAreaMapUnlocked(currentObjective.areaMap);
			Debug.Log("<color=yellow>IsAreaUnlocked(" + currentObjective.areaMap + ") = " + flag2 + "</color>");
			if (!flag2)
			{
				Debug.Log("Unlocking" + currentObjective.areaMap);
				GameController.Instance.PlayerProfile.UnlockArea(currentObjective.areaMap);
			}
			GameController.Instance.SetAreaMapMetaLayer(GameController.Instance.areaMapDB.areaMaps[currentObjective.areaMap]);
			int startRow = GameController.Instance.areaMapDB.areaMaps[currentObjective.areaMap].startRow;
			int startCol = GameController.Instance.areaMapDB.areaMaps[currentObjective.areaMap].startCol;
			int seedFromPosition = GameController.Instance.areaMapDB.areaMaps[currentObjective.areaMap].GetSeedFromPosition(startRow, startCol);
			bool flag3 = GameController.Instance.areaMapDB.areaMaps[currentObjective.areaMap].IsUnlocked(seedFromPosition);
			Debug.Log("First Area: " + seedFromPosition + " unlocked? " + flag3);
			if (!flag3)
			{
				Debug.Log("First Area was not unlocked, unlocking...");
				_desiredUnlock = new LevelUnlockData();
				_desiredUnlock.season = season;
				_desiredUnlock.seed = seedFromPosition;
			}
			return currentObjective != null;
		}
		return false;
	}

	public bool CheckIfNextObjectiveOnNewMap()
	{
		if (IsCurrentObjectiveCompleted())
		{
			int season = currentObjective.season;
			int chapter = currentObjective.chapter;
			int num = GameController.Instance.NumChapters(season);
			if (chapter < num)
			{
				ObjectiveData objectiveData = ObjectiveData.Generate(GetObjectiveLinesForSeasonChapter(season, chapter + 1));
				if (objectiveData.areaMap != currentObjective.areaMap)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsCurrentObjectiveCompleted()
	{
		if (currentObjective == null)
		{
			return true;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (!objectiveLine.Completed)
			{
				return false;
			}
		}
		return true;
	}

	public bool ShouldEnemySpawnObject(AIAgent agent)
	{
		if (currentObjective == null)
		{
			return false;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (objectiveLine.objectiveType == ObjectiveType.Gather && (objectiveLine.seed == -1 || objectiveLine.seed == GameController.Instance.CurrentLevel.Seed) && (objectiveLine.levelSequence == -1 || objectiveLine.levelSequence == GameController.Instance.CurrentLevel.LevelTheme.index))
			{
				MinifigType type = CharacterBuilder.minifigDefinitions[agent.Character.NinjaID].type;
				MinifigClass minifigClass = CharacterBuilder.minifigDefinitions[agent.Character.NinjaID].minifigClass;
				if (objectiveLine.objectSpawnType == ObjectiveObjectSpawnType.ConstrictaiWarrior && minifigClass == MinifigClass.Warrior && type == MinifigType.Constrictai)
				{
					return true;
				}
				if (objectiveLine.objectSpawnType == ObjectiveObjectSpawnType.FangpyreWarrior && minifigClass == MinifigClass.Warrior && type == MinifigType.Fangpyre)
				{
					return true;
				}
				if (objectiveLine.objectSpawnType == ObjectiveObjectSpawnType.HypnobraiWarrior && minifigClass == MinifigClass.Warrior && type == MinifigType.Hynobrai)
				{
					return true;
				}
				if (objectiveLine.objectSpawnType == ObjectiveObjectSpawnType.VenomariWarrior && minifigClass == MinifigClass.Warrior && type == MinifigType.Venomari)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ShouldSpawnObject(string interactableType)
	{
		if (currentObjective == null)
		{
			return false;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (objectiveLine.objectiveType == ObjectiveType.Gather && objectiveLine.objectSpawnType.ToString().ToLower() == interactableType.ToLower() && (objectiveLine.seed == -1 || objectiveLine.seed == GameController.Instance.CurrentLevel.Seed) && (objectiveLine.levelSequence == -1 || objectiveLine.levelSequence == GameController.Instance.CurrentLevel.LevelTheme.index))
			{
				return true;
			}
		}
		return false;
	}

	public string ObjectToSpawn()
	{
		if (currentObjective == null)
		{
			return string.Empty;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (objectiveLine.objectiveType == ObjectiveType.Gather && (objectiveLine.seed == -1 || objectiveLine.seed == GameController.Instance.CurrentLevel.Seed) && (objectiveLine.levelSequence == -1 || objectiveLine.levelSequence == GameController.Instance.CurrentLevel.LevelTheme.index))
			{
				switch (objectiveLine.objectType)
				{
				case ObjectiveObjectType.SummonStone:
					return "Prefabs/SummonStone";
				case ObjectiveObjectType.Studs:
					return "Prefabs/Knob_Pickup";
				case ObjectiveObjectType.ConstrictaiShard:
					return "Prefabs/ConstrictaiShard";
				case ObjectiveObjectType.FangpyreShard:
					return "Prefabs/FangpyreShard";
				case ObjectiveObjectType.HypnobraiShard:
					return "Prefabs/HypnobraiShard";
				case ObjectiveObjectType.VenomariShard:
					return "Prefabs/VenomariShard";
				}
			}
		}
		return string.Empty;
	}

	public string GetNumCollectiblesRemainingText()
	{
		if (currentObjective != null)
		{
			ObjectiveLine[] lines = currentObjective.lines;
			foreach (ObjectiveLine objectiveLine in lines)
			{
				if (!objectiveLine.Completed && objectiveLine.objectiveType == ObjectiveType.Gather)
				{
					return "(" + objectiveLine.objectCounter + "/" + objectiveLine.amountToComplete + ")";
				}
			}
		}
		return string.Empty;
	}

	public void OnCollectiblePickup(Level level, Collectible collectible)
	{
		if (currentObjective == null)
		{
			return;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (!objectiveLine.Completed && objectiveLine.objectiveType == ObjectiveType.Gather && objectiveLine.objectType.ToString().ToLower() == collectible.GetType().ToString().ToLower())
			{
				if (objectiveLine.seed == -1 || objectiveLine.seed == level.Seed)
				{
					objectiveLine.objectCounter++;
				}
				if (objectiveLine.objectCounter >= objectiveLine.amountToComplete)
				{
					objectiveLine.Completed = true;
				}
				break;
			}
		}
	}

	public void OnInteractableInteract(Level level, InteractableObject interactable)
	{
		if (currentObjective == null)
		{
			return;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (!objectiveLine.Completed && objectiveLine.objectiveType == ObjectiveType.Gather && objectiveLine.objectType.ToString().ToLower() == interactable.GetType().ToString().ToLower())
			{
				if (objectiveLine.seed == -1 || objectiveLine.seed == level.Seed)
				{
					objectiveLine.objectCounter++;
				}
				if (objectiveLine.objectCounter >= objectiveLine.amountToComplete)
				{
					objectiveLine.Completed = true;
				}
				break;
			}
		}
	}

	public void OnEndLevel(Level level)
	{
		if (currentObjective == null)
		{
			return;
		}
		ObjectiveLine[] lines = currentObjective.lines;
		foreach (ObjectiveLine objectiveLine in lines)
		{
			if (!objectiveLine.Completed && objectiveLine.objectiveType == ObjectiveType.Complete && (objectiveLine.seed == -1 || objectiveLine.seed == level.Seed))
			{
				objectiveLine.Completed = true;
			}
		}
	}

	private List<string> GetObjectiveLinesForSeasonChapter(int season, int chapter)
	{
		TextAsset objectiveFile = GameController.Instance.storyDB.GetObjectiveFile(season);
		List<string> list = new List<string>();
		using (StringReader stringReader = new StringReader(objectiveFile.text))
		{
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split('\t');
				if (array[0] == season.ToString() && array[1] == chapter.ToString())
				{
					list.Add(text);
				}
			}
			return list;
		}
	}

	public void DebugLogCurrentObjective()
	{
		if (currentObjective != null)
		{
			Debug.Log("----------");
			Debug.Log("CURRENT OBJECTIVE: ");
			Debug.Log("Season: " + currentObjective.season + ", Chapter: " + currentObjective.chapter + ", AreaMap: " + currentObjective.areaMap + ", Description: " + currentObjective.description);
			Debug.Log("SUB-OBJECTIVES: ");
			ObjectiveLine[] lines = currentObjective.lines;
			foreach (ObjectiveLine objectiveLine in lines)
			{
				Debug.Log(string.Concat("ObjectiveType: ", objectiveLine.objectiveType, ", AmountToComplete: ", objectiveLine.amountToComplete, ", ObjectType: ", objectiveLine.objectType, ", ObjectSpawnType: ", objectiveLine.objectSpawnType, ", ObjectSpawnChance: ", objectiveLine.spawnChance, ", Seed: ", objectiveLine.seed, ", LevelSequence: ", objectiveLine.levelSequence, ", PatternID: ", objectiveLine.patternID, ", DescriptionStringID: ", objectiveLine.descriptionStringID, ", Completed: ", objectiveLine.Completed));
			}
			Debug.Log("----------");
		}
		else
		{
			Debug.Log("No CurrentObjective");
		}
	}
}
