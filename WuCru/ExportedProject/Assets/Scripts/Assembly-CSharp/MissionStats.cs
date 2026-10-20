using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionStats : MonoBehaviour
{
	private Dictionary<LevelChallengeBase, bool> challenges = new Dictionary<LevelChallengeBase, bool>();

	private int numChestsWoodToCollect;

	private int numChestsSilverToCollect;

	private int numChestsGoldToCollect;

	private int numCrystalsToCollect;

	private int numCrystalsEarthToCollect;

	private int numCrystalsFireToCollect;

	private int numCrystalsIceToCollect;

	private int numCrystalsElectricityToCollect;

	private int numCrystalsEnergyToCollect;

	private int numCrystalsWaterToCollect;

	private int numChestsWoodCollected;

	private int numChestsSilverCollected;

	private int numChestsGoldCollected;

	private int numChestsWoodSoFar;

	private int numChestsSilverSoFar;

	private int numChestsGoldSoFar;

	private int numCrystalsCollected;

	private int numCrystalsEarthCollected;

	private int numCrystalsFireCollected;

	private int numCrystalsIceCollected;

	private int numCrystalsElectricityCollected;

	private int numCrystalsEnergyCollected;

	private int numCrystalsWaterCollected;

	private int numCrystalsSoFar;

	private int numCrystalsEarthSoFar;

	private int numCrystalsFireSoFar;

	private int numCrystalsIceSoFar;

	private int numCrystalsElectricitySoFar;

	private int numCrystalsEnergySoFar;

	private int numCrystalsWaterSoFar;

	private int numStudsCollected;

	private int numEnemies;

	private int numEnemiesKilled;

	private int numDeadNinjas;

	public List<int> deadNinjaIDs = new List<int>();

	public int NumChestsWoodToCollect
	{
		get
		{
			return numChestsWoodToCollect;
		}
	}

	public int NumChestsSilverToCollect
	{
		get
		{
			return numChestsSilverToCollect;
		}
	}

	public int NumChestsGoldToCollect
	{
		get
		{
			return numChestsGoldToCollect;
		}
	}

	public int NumChestsToCollect
	{
		get
		{
			return numChestsWoodToCollect + numChestsSilverToCollect + numChestsGoldToCollect;
		}
	}

	public int NumChestsWoodCollected
	{
		get
		{
			return numChestsWoodCollected;
		}
	}

	public int NumChestsSilverCollected
	{
		get
		{
			return numChestsSilverCollected;
		}
	}

	public int NumChestsGoldCollected
	{
		get
		{
			return numChestsGoldCollected;
		}
	}

	public int NumChestsCollected
	{
		get
		{
			return numChestsWoodCollected + numChestsSilverCollected + numChestsGoldCollected;
		}
	}

	public int NumChestsWoodSoFar
	{
		get
		{
			return numChestsWoodSoFar;
		}
	}

	public int NumChestsSilverSoFar
	{
		get
		{
			return numChestsSilverSoFar;
		}
	}

	public int NumChestsGoldSoFar
	{
		get
		{
			return numChestsGoldSoFar;
		}
	}

	public int NumChestsSoFar
	{
		get
		{
			return numChestsWoodSoFar + numChestsSilverSoFar + numChestsGoldSoFar;
		}
	}

	public int NumCrystalsToCollect
	{
		get
		{
			return numCrystalsToCollect;
		}
	}

	public int NumCrystalsEarthToCollect
	{
		get
		{
			return numCrystalsEarthToCollect;
		}
	}

	public int NumCrystalsFireToCollect
	{
		get
		{
			return numCrystalsFireToCollect;
		}
	}

	public int NumCrystalsIceToCollect
	{
		get
		{
			return numCrystalsIceToCollect;
		}
	}

	public int NumCrystalsElectricityToCollect
	{
		get
		{
			return numCrystalsElectricityToCollect;
		}
	}

	public int NumCrystalsEnergyToCollect
	{
		get
		{
			return numCrystalsEnergyToCollect;
		}
	}

	public int NumCrystalsWaterToCollect
	{
		get
		{
			return numCrystalsWaterToCollect;
		}
	}

	public int NumCrystalsCollected
	{
		get
		{
			return numCrystalsCollected;
		}
	}

	public int NumCrystalsEarthCollected
	{
		get
		{
			return numCrystalsEarthCollected;
		}
	}

	public int NumCrystalsFireCollected
	{
		get
		{
			return numCrystalsFireCollected;
		}
	}

	public int NumCrystalsIceCollected
	{
		get
		{
			return numCrystalsIceCollected;
		}
	}

	public int NumCrystalsElectricityCollected
	{
		get
		{
			return numCrystalsElectricityCollected;
		}
	}

	public int NumCrystalsEnergyCollected
	{
		get
		{
			return numCrystalsEnergyCollected;
		}
	}

	public int NumCrystalsWaterCollected
	{
		get
		{
			return numCrystalsWaterCollected;
		}
	}

	public int NumCrystalsSoFar
	{
		get
		{
			return numCrystalsSoFar;
		}
	}

	public int NumCrystalsEarthSoFar
	{
		get
		{
			return numCrystalsEarthSoFar;
		}
	}

	public int NumCrystalsFireSoFar
	{
		get
		{
			return numCrystalsFireSoFar;
		}
	}

	public int NumCrystalsIceSoFar
	{
		get
		{
			return numCrystalsIceSoFar;
		}
	}

	public int NumCrystalsElectricitySoFar
	{
		get
		{
			return numCrystalsElectricitySoFar;
		}
	}

	public int NumCrystalsEnergySoFar
	{
		get
		{
			return numCrystalsEnergySoFar;
		}
	}

	public int NumCrystalsWaterSoFar
	{
		get
		{
			return numCrystalsWaterSoFar;
		}
	}

	public int NumChallenges
	{
		get
		{
			return GameController.Instance.CurrentLevel.LevelThemeCollection.themes.keys.Count;
		}
	}

	public int NumChallengesCompleted
	{
		get
		{
			return challenges.Values.Where((bool x) => x).Count();
		}
	}

	public int NumStudsCollected
	{
		get
		{
			return numStudsCollected;
		}
		set
		{
			numStudsCollected = value;
		}
	}

	public int NumCrystalsCollectedAllTypes
	{
		get
		{
			return numCrystalsCollected + NumCrystalsEarthCollected + NumCrystalsFireCollected + NumCrystalsIceCollected + NumCrystalsElectricityCollected + NumCrystalsEnergyCollected + NumCrystalsWaterCollected;
		}
	}

	public int NumEnemies
	{
		get
		{
			return numEnemies;
		}
	}

	public int NumEnemiesKilled
	{
		get
		{
			return numEnemiesKilled;
		}
	}

	public int NumDeadNinjas
	{
		get
		{
			return numDeadNinjas;
		}
	}

	public Dictionary<LevelChallengeBase, bool> Challenges
	{
		get
		{
			return challenges;
		}
	}

	private void Awake()
	{
		base.enabled = false;
		Reset();
	}

	public void ReportPlayerStart()
	{
		if (GamePlayer.Instance.Level.LevelTheme.index == 0)
		{
			Reset();
		}
		numChestsWoodToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.WoodChest);
		numChestsSilverToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.MetalChest);
		numChestsGoldToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.GoldChest);
		numCrystalsToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard);
		numCrystalsEarthToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_earth);
		numCrystalsFireToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_fire);
		numCrystalsIceToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_ice);
		numCrystalsElectricityToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_electricity);
		numCrystalsEnergyToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_energy);
		numCrystalsWaterToCollect += GamePlayer.Instance.Level.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard_water);
		numEnemies = GamePlayer.Instance.Level.ChunkDB.AIAgents.Count;
	}

	public void ReportLevelCompleted()
	{
		numChestsWoodCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.WoodChest);
		numChestsSilverCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.MetalChest);
		numChestsGoldCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.GoldChest);
		numCrystalsCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard);
		numCrystalsEarthCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_earth);
		numCrystalsFireCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_fire);
		numCrystalsIceCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_ice);
		numCrystalsElectricityCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_electricity);
		numCrystalsEnergyCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_energy);
		numCrystalsWaterCollected += GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_water);
		if (!challenges.ContainsKey(GameController.Instance.CurrentLevel.Challenge))
		{
			Debug.Log("MissionStats->LevelCompleted. Updating Challenges stats with challenge: " + GameController.Instance.CurrentLevel.Challenge.GetType().ToString());
			Level currentLevel = GameController.Instance.CurrentLevel;
			LevelChallengeBase challenge = currentLevel.Challenge;
			bool isCompleted = challenge.IsCompleted;
			challenges.Add(challenge, isCompleted);
			if (isCompleted)
			{
				if (challenge is ChallengeStealth)
				{
					AnalyticsEvents.LogChallengeEnded(challenge.GetType().ToString(), true, currentLevel.Seed);
				}
			}
			else
			{
				AnalyticsEvents.LogChallengeEnded(challenge.GetType().ToString(), false, currentLevel.Seed);
			}
		}
		else
		{
			Debug.LogError("Could not track challenge, as challenge is already in dictionary aka has been used on previous sequence");
		}
	}

	public int TotalNumberOfObjectsInLevel()
	{
		return NumCrystalsToCollect + NumChestsToCollect;
	}

	public bool CollectedAllObjects()
	{
		return TotalNumberOfObjectsInLevel() <= NumChestsCollected + NumCrystalsCollected;
	}

	public bool NoNinjasKilled()
	{
		return deadNinjaIDs.Count == 0;
	}

	public bool CompletedAllChallenges()
	{
		return NumChallengesCompleted >= NumChallenges;
	}

	public void CalcProgressionStats()
	{
		numChestsWoodSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.WoodChest);
		numChestsSilverSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.MetalChest);
		numChestsGoldSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.GoldChest);
		numCrystalsSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard);
		numCrystalsEarthSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_earth);
		numCrystalsFireSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_fire);
		numCrystalsIceSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_ice);
		numCrystalsElectricitySoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_electricity);
		numCrystalsEnergySoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_energy);
		numCrystalsWaterSoFar = GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_water);
	}

	public void ReportPlayerDead()
	{
		numDeadNinjas++;
		deadNinjaIDs.Add(GamePlayer.Instance.Player.Character.NinjaID);
	}

	public void ReportEnemyDead()
	{
		numEnemiesKilled++;
	}

	private void Reset()
	{
		Debug.Log("MissionStats->Reset. Clearing Challenges stats");
		challenges.Clear();
		numChestsWoodToCollect = 0;
		numChestsSilverToCollect = 0;
		numChestsGoldToCollect = 0;
		numCrystalsToCollect = 0;
		numCrystalsEarthToCollect = 0;
		numCrystalsFireToCollect = 0;
		numCrystalsIceToCollect = 0;
		numCrystalsElectricityToCollect = 0;
		numCrystalsEnergyToCollect = 0;
		numCrystalsWaterToCollect = 0;
		numChestsWoodCollected = 0;
		numChestsSilverCollected = 0;
		numChestsGoldCollected = 0;
		numCrystalsCollected = 0;
		numCrystalsEarthCollected = 0;
		numCrystalsFireCollected = 0;
		numCrystalsIceCollected = 0;
		numCrystalsElectricityCollected = 0;
		numCrystalsEnergyCollected = 0;
		numCrystalsWaterCollected = 0;
		numDeadNinjas = 0;
		numEnemiesKilled = 0;
	}
}
