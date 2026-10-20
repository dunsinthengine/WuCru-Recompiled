using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableMinifigParts : MonoBehaviour
{
	[Serializable]
	public class PartDefinition
	{
		public enum PART_TYPE
		{
			FACE = 0,
			HAIR = 1,
			TORSO = 2,
			HELMET = 3
		}

		public string[] _associatedPaths;

		public PART_TYPE _partType;

		public bool _helmetHidesHair;

		public bool _priorityUnlock;
	}

	private static UnlockableMinifigParts _instance;

	public int[] _validSeedLocations;

	public PartDefinition[] _partDefinitions;

	public static UnlockableMinifigParts _pInstance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_instance = this;
	}

	public bool IsSeedAvailable(int seed)
	{
		if (GameController.Instance.PlayerProfile.HasUsedMinifigPartLocation(seed))
		{
			return false;
		}
		int num = _validSeedLocations.Length;
		for (int i = 0; i < num; i++)
		{
			if (_validSeedLocations[i] == seed)
			{
				return true;
			}
		}
		return false;
	}

	public void UnlockItem()
	{
		int pSeed = ScreenMapConfirm._chosenMapInfo._pSeed;
		List<int> list = new List<int>();
		int num = _partDefinitions.Length;
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			if (_partDefinitions[i]._priorityUnlock && !GameController.Instance.PlayerProfile.HasCollectedMinifigPartByIndex(i))
			{
				flag = true;
				break;
			}
		}
		for (int j = 0; j < num; j++)
		{
			if (!GameController.Instance.PlayerProfile.HasCollectedMinifigPartByIndex(j) && ((flag && _partDefinitions[j]._priorityUnlock) || !flag))
			{
				list.Add(j);
			}
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		GameController.Instance.PlayerProfile.CollectMinifigPart(list[index], pSeed);
	}
}
