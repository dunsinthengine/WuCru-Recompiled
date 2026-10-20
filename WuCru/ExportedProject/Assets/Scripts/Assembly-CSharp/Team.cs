using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Team
{
	public static Dictionary<int, Dictionary<int, TeamEntry>> team;

	private static Dictionary<MinifigType, List<int>> typeIDs;

	private static int[] shardsList;

	static Team()
	{
		team = new Dictionary<int, Dictionary<int, TeamEntry>>();
		typeIDs = new Dictionary<MinifigType, List<int>>();
		shardsList = new int[5];
		TextAsset textAsset = Resources.Load("teamskill") as TextAsset;
		using (StringReader stringReader = new StringReader(textAsset.text))
		{
			stringReader.ReadLine();
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split(',');
				int result;
				if (int.TryParse(array[0], out result))
				{
					MinifigType minifigType = MinifigType.None;
					if (Enum.IsDefined(typeof(MinifigType), array[1]))
					{
						minifigType = (MinifigType)Enum.Parse(typeof(MinifigType), array[1]);
					}
					int result2;
					int result3;
					int result4;
					int result5;
					int result6;
					int result7;
					int result8;
					int result9;
					int result10;
					if (int.TryParse(array[2], out result2) && int.TryParse(array[3], out result3) && int.TryParse(array[4], out result4) && int.TryParse(array[5], out result5) && int.TryParse(array[6], out result6) && int.TryParse(array[7], out result7) && int.TryParse(array[8], out result8) && int.TryParse(array[9], out result9) && int.TryParse(array[10], out result10))
					{
						Dictionary<int, TeamEntry> dictionary = null;
						if (team.ContainsKey(result))
						{
							dictionary = team[result];
						}
						else
						{
							dictionary = new Dictionary<int, TeamEntry>();
							team.Add(result, dictionary);
						}
						dictionary.Add(result2 - 1, new TeamEntry
						{
							id = result,
							ninja = minifigType,
							level = result2 - 1,
							weapon = result3,
							element = result4,
							spinjitzu = result5,
							awareness = result6,
							stamina = result7,
							agility = result8,
							strength = result9
						});
						List<int> list = null;
						if (typeIDs.ContainsKey(minifigType))
						{
							list = typeIDs[minifigType];
						}
						else
						{
							list = new List<int>();
							typeIDs.Add(minifigType, list);
						}
						list.Add(result);
						shardsList[result2 - 1] = result10;
					}
				}
			}
		}
	}

	public static void Configure(Character character, int id, int level)
	{
		if (id == 0)
		{
			character.SpinjitzuDuration = GameController.Instance.PlayerProfile.SpinjitzuDuration;
			return;
		}
		Dictionary<int, TeamEntry> dictionary = team[id];
		if (!dictionary.ContainsKey(level))
		{
			Debug.LogError("Team id " + id + " does not contain level " + level);
		}
		TeamEntry teamEntry = dictionary[level];
		character.SpinjitzuDuration = PlayerProfile.SpinjitzuDurationForLevel(teamEntry.spinjitzu);
	}

	public static int LevelForShards(int value)
	{
		int result = 0;
		for (int i = 0; i < shardsList.Length; i++)
		{
			if (shardsList[i] <= value)
			{
				result = i;
			}
		}
		return result;
	}

	public static int ShardsForLevel(int level)
	{
		if (level >= shardsList.Length)
		{
			return -1;
		}
		return shardsList[level];
	}

	public static int GetID(MinifigType type, int level)
	{
		if (typeIDs.ContainsKey(type) && typeIDs[type].Count > level)
		{
			return typeIDs[type][level];
		}
		return -1;
	}

	public static MinifigType GetTypeForID(int id)
	{
		foreach (KeyValuePair<MinifigType, List<int>> typeID in typeIDs)
		{
			if (typeID.Value.Contains(id))
			{
				return typeID.Key;
			}
		}
		return MinifigType.None;
	}

	public static List<int> GetIDsForType(MinifigType type)
	{
		if (typeIDs.ContainsKey(type))
		{
			return typeIDs[type];
		}
		return new List<int>();
	}
}
