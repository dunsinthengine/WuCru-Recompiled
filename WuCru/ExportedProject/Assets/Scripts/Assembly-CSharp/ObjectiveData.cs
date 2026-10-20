using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectiveData
{
	[SerializeField]
	public int season;

	[SerializeField]
	public int chapter;

	[SerializeField]
	public string areaMap;

	[SerializeField]
	public string description = "None";

	[SerializeField]
	public ObjectiveLine[] lines;

	public bool IsCompleted()
	{
		ObjectiveLine[] array = lines;
		foreach (ObjectiveLine objectiveLine in array)
		{
			if (!objectiveLine.Completed)
			{
				return false;
			}
		}
		return true;
	}

	public static ObjectiveData Generate(List<string> objectiveLines)
	{
		ObjectiveData objectiveData = new ObjectiveData();
		objectiveData.lines = new ObjectiveLine[objectiveLines.Count];
		bool flag = false;
		int num = 0;
		foreach (string objectiveLine2 in objectiveLines)
		{
			ObjectiveLine objectiveLine = new ObjectiveLine();
			string[] array = objectiveLine2.Split('\t');
			objectiveLine.season = Convert.ToInt32(array[0]);
			objectiveLine.chapter = Convert.ToInt32(array[1]);
			objectiveLine.areaMap = array[2];
			if (array[3].Length > 0)
			{
				objectiveLine.objectiveType = (ObjectiveType)Enum.Parse(typeof(ObjectiveType), array[3]);
			}
			if (array[4].Length > 0)
			{
				objectiveLine.amountToComplete = Convert.ToInt32(array[4]);
			}
			if (array[5].Length > 0)
			{
				objectiveLine.objectType = (ObjectiveObjectType)Enum.Parse(typeof(ObjectiveObjectType), array[5]);
			}
			if (array[6].Length > 0)
			{
				objectiveLine.objectSpawnType = (ObjectiveObjectSpawnType)Enum.Parse(typeof(ObjectiveObjectSpawnType), array[6]);
			}
			if (array[7].Length > 0)
			{
				objectiveLine.spawnChance = Convert.ToSingle(array[7]);
			}
			if (array[8].Length > 0)
			{
				objectiveLine.seed = Convert.ToInt32(array[8]);
			}
			if (array[9].Length > 0)
			{
				objectiveLine.levelSequence = Convert.ToInt32(array[9]);
			}
			if (array[10].Length > 0)
			{
				objectiveLine.patternID = (ObjectivePatternID)Enum.Parse(typeof(ObjectivePatternID), array[10]);
			}
			if (array[11].Length > 0)
			{
				objectiveLine.descriptionStringID = array[11];
			}
			if (array[12].Length > 0)
			{
				objectiveLine.Completed = Convert.ToBoolean(array[12]);
			}
			if (array[13].Length > 0)
			{
				objectiveLine.objectiveEvent = (ObjectiveEvent)Enum.Parse(typeof(ObjectiveEvent), array[13]);
			}
			if (!flag)
			{
				flag = true;
				objectiveData.season = objectiveLine.season;
				objectiveData.chapter = objectiveLine.chapter;
				objectiveData.areaMap = objectiveLine.areaMap;
				objectiveData.description = objectiveLine.descriptionStringID;
			}
			objectiveData.lines[num++] = objectiveLine;
		}
		return objectiveData;
	}

	public static LevelCompletionCondition[] GetCompletionConditions(ObjectiveData objective)
	{
		List<LevelCompletionCondition> list = new List<LevelCompletionCondition>();
		Level currentLevel = GameController.Instance.CurrentLevel;
		ObjectiveLine[] array = objective.lines;
		foreach (ObjectiveLine objectiveLine in array)
		{
			if ((currentLevel == null || currentLevel.Seed == objectiveLine.seed || objectiveLine.seed == -1) && objectiveLine.patternID != ObjectivePatternID.None)
			{
				if (objectiveLine.patternID == ObjectivePatternID.Oroborous || objectiveLine.patternID == ObjectivePatternID.ConstrictaiBoss || objectiveLine.patternID == ObjectivePatternID.FangpyreBoss || objectiveLine.patternID == ObjectivePatternID.HypnobraiBoss || objectiveLine.patternID == ObjectivePatternID.VenomariBoss)
				{
					list.Add(LevelCompletionCondition.KillBoss);
				}
				if (objectiveLine.patternID == ObjectivePatternID.RescueCole || objectiveLine.patternID == ObjectivePatternID.RescueZane || objectiveLine.patternID == ObjectivePatternID.RescueKai || objectiveLine.patternID == ObjectivePatternID.RescueJay || objectiveLine.patternID == ObjectivePatternID.RescueNyah || objectiveLine.patternID == ObjectivePatternID.MeetMasterWu)
				{
					list.Add(LevelCompletionCondition.RescueFriend);
				}
				if (objectiveLine.patternID == ObjectivePatternID.FindTheHoloLog || objectiveLine.patternID == ObjectivePatternID.FindTheBiteBike || objectiveLine.patternID == ObjectivePatternID.SamuraiXCave)
				{
					list.Add(LevelCompletionCondition.CollectItem);
				}
			}
		}
		return list.ToArray();
	}
}
