using System;
using UnityEngine;

[Serializable]
public class StoryDB : ScriptableObject
{
	[SerializeField]
	public DictionarySeasonToFile storyFiles = new DictionarySeasonToFile();

	[SerializeField]
	public DictionarySeasonToFile objectiveFiles = new DictionarySeasonToFile();

	public TextAsset GetStoryFile(int season)
	{
		if (storyFiles.ContainsKey(season))
		{
			return storyFiles[season];
		}
		Debug.Log("Could not get story file for season: " + season);
		return null;
	}

	public TextAsset GetObjectiveFile(int season)
	{
		if (objectiveFiles.ContainsKey(season))
		{
			return objectiveFiles[season];
		}
		return null;
	}
}
