using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableVehicleParts : MonoBehaviour
{
	[Serializable]
	public class PartDefinition
	{
		public enum PART_TYPE
		{
			LEFT = 0,
			MIDDLE = 1,
			RIGHT = 2
		}

		public GameObject _partPrefab;

		public PART_TYPE _partType;

		public int _validSeedLocation;

		public bool _collected;

		public bool _viewed;
	}

	[Serializable]
	public class VehicleDefinition
	{
		public enum VEHICLE_TYPE
		{
			V1 = 0,
			V2 = 1,
			V3 = 2,
			V4 = 3,
			V5 = 4,
			V6 = 5,
			V7 = 6
		}

		public string _name;

		public VEHICLE_TYPE _vehicleNumber;

		public int _season;

		public PartDefinition[] _parts;
	}

	private static UnlockableVehicleParts _instance;

	public VehicleDefinition[] _vehicleDefinitions;

	public static UnlockableVehicleParts _pInstance
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
		if (GameController.Instance.PlayerProfile.HasUsedVehiclePartLocation(seed))
		{
			return false;
		}
		int num = _vehicleDefinitions.Length;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < _vehicleDefinitions[i]._parts.Length; j++)
			{
				if (_vehicleDefinitions[i]._parts[j]._validSeedLocation == seed)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UnlockPart()
	{
		int pSeed = ScreenMapConfirm._chosenMapInfo._pSeed;
		if (GameController.Instance.PlayerProfile.HasUsedVehiclePartLocation(pSeed))
		{
			return;
		}
		int num = _vehicleDefinitions.Length;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < _vehicleDefinitions[i]._parts.Length; j++)
			{
				if (pSeed == _vehicleDefinitions[i]._parts[j]._validSeedLocation)
				{
					GameController.Instance.PlayerProfile.CollectVehiclePart(pSeed);
					_vehicleDefinitions[i]._parts[j]._collected = true;
					Debug.LogError("Vehicle part [" + j + "] for [" + _vehicleDefinitions[i]._name + "] unlocked.");
					return;
				}
			}
		}
	}

	public void forceVehicleUnlocked(int vehicleIndex)
	{
		int num = _vehicleDefinitions.Length;
		for (int i = 0; i < _vehicleDefinitions[vehicleIndex]._parts.Length; i++)
		{
			int validSeedLocation = _vehicleDefinitions[vehicleIndex]._parts[i]._validSeedLocation;
			if (!GameController.Instance.PlayerProfile.HasUsedVehiclePartLocation(validSeedLocation))
			{
				GameController.Instance.PlayerProfile.CollectVehiclePart(validSeedLocation);
				_vehicleDefinitions[vehicleIndex]._parts[i]._collected = true;
				Debug.LogError("Vehicle part [" + i + "] for [" + _vehicleDefinitions[vehicleIndex]._name + "] unlocked.");
			}
		}
	}

	public void updateCollected(List<int> uvp)
	{
		for (int i = 0; i < _vehicleDefinitions.Length; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int validSeedLocation = _vehicleDefinitions[i]._parts[j]._validSeedLocation;
				if (uvp.Contains(validSeedLocation))
				{
					_vehicleDefinitions[i]._parts[j]._collected = true;
				}
			}
		}
	}

	public void updateViewed(List<int> vvp)
	{
		for (int i = 0; i < _vehicleDefinitions.Length; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int validSeedLocation = _vehicleDefinitions[i]._parts[j]._validSeedLocation;
				if (vvp.Contains(validSeedLocation))
				{
					_vehicleDefinitions[i]._parts[j]._viewed = true;
				}
			}
		}
	}

	public bool isPartUnlocked(int vehicleIndex, int partIndex)
	{
		if (vehicleIndex < 0 || vehicleIndex >= _vehicleDefinitions.Length)
		{
			return false;
		}
		return _vehicleDefinitions[vehicleIndex]._parts[partIndex]._collected;
	}

	public bool hasPartBeenViewed(int vehicleIndex, int partIndex)
	{
		if (vehicleIndex < 0 || vehicleIndex >= _vehicleDefinitions.Length)
		{
			return false;
		}
		return _vehicleDefinitions[vehicleIndex]._parts[partIndex]._viewed;
	}

	public bool vehicleGoldenTrail()
	{
		int num = 0;
		for (int i = 0; i < _vehicleDefinitions.Length; i++)
		{
			for (int j = 0; j < _vehicleDefinitions[i]._parts.Length; j++)
			{
				if (_vehicleDefinitions[i]._parts[j]._collected && !_vehicleDefinitions[i]._parts[j]._viewed)
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public void setPartHasBeenViewed(int vehicleIndex, int partIndex)
	{
		if (vehicleIndex >= 0 && vehicleIndex < _vehicleDefinitions.Length)
		{
			_vehicleDefinitions[vehicleIndex]._parts[partIndex]._viewed = true;
			Debug.LogError("Vehicle part [" + partIndex + "] for [" + _vehicleDefinitions[vehicleIndex]._name + "] was viewed.");
		}
	}

	public int numPartUnlocked(int vehicleIndex)
	{
		if (vehicleIndex < 0 || vehicleIndex >= _vehicleDefinitions.Length)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < _vehicleDefinitions[vehicleIndex]._parts.Length; i++)
		{
			if (_vehicleDefinitions[vehicleIndex]._parts[i]._collected)
			{
				num++;
			}
		}
		return num;
	}

	public void UnlockVehiclePartSpecial(int vehicleIndex, int partIndex)
	{
		if (vehicleIndex >= 0 && vehicleIndex < _vehicleDefinitions.Length && partIndex >= 0 && partIndex < _vehicleDefinitions[vehicleIndex]._parts.Length)
		{
			int validSeedLocation = _vehicleDefinitions[vehicleIndex]._parts[partIndex]._validSeedLocation;
			if (!GameController.Instance.PlayerProfile.HasUsedVehiclePartLocation(validSeedLocation))
			{
				GameController.Instance.PlayerProfile.CollectVehiclePart(validSeedLocation);
				_vehicleDefinitions[vehicleIndex]._parts[partIndex]._collected = true;
				Debug.LogError("Vehicle part [" + partIndex + "] for [" + _vehicleDefinitions[vehicleIndex]._name + "] unlocked.");
			}
		}
	}

	public void ViewVehiclePartSpecial(int vehicleIndex, int partIndex)
	{
		if (vehicleIndex >= 0 && vehicleIndex < _vehicleDefinitions.Length && partIndex >= 0 && partIndex < _vehicleDefinitions[vehicleIndex]._parts.Length)
		{
			int validSeedLocation = _vehicleDefinitions[vehicleIndex]._parts[partIndex]._validSeedLocation;
			if (!GameController.Instance.PlayerProfile.HasViewedVehiclePartLocation(validSeedLocation))
			{
				GameController.Instance.PlayerProfile.ViewVehiclePart(validSeedLocation);
				_vehicleDefinitions[vehicleIndex]._parts[partIndex]._viewed = true;
				Debug.LogError("Vehicle part [" + partIndex + "] for [" + _vehicleDefinitions[vehicleIndex]._name + "] was viewed.");
			}
		}
	}
}
