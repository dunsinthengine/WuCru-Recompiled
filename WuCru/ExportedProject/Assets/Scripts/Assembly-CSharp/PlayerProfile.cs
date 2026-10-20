using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerProfile
{
	private const string KEY_DATA = "data";

	private const string KEY_DATA_OBJ = "dataObj";

	private static int[] xp;

	public static List<int> newlyUnlockedNinjas;

	public static List<int> newlyUnlockedVehicles;

	[SerializeField]
	private List<string> areasUnlocked = new List<string>();

	[SerializeField]
	private string currentArea;

	[SerializeField]
	private DictionaryAreaPositon currentAreaPosition = new DictionaryAreaPositon();

	[SerializeField]
	private DictionaryLevelExtensionIndex levelExtensionData = new DictionaryLevelExtensionIndex();

	[SerializeField]
	private List<int> unlockedNinjaSKUs = new List<int>();

	[SerializeField]
	private List<string> completedMissions = new List<string>();

	[SerializeField]
	private List<int> ninjaTeam;

	[SerializeField]
	private bool showWorldMapInTopBar;

	[SerializeField]
	private bool showGarageInTopBar;

	[SerializeField]
	private bool showSpinjitzuUnlock;

	[SerializeField]
	private bool firstTimeCruShow = true;

	[SerializeField]
	private long timestamp;

	[SerializeField]
	private int studs;

	[SerializeField]
	private List<int> unlockedVehicles = new List<int>();

	[SerializeField]
	private int _skillCoinChestCounter;

	[SerializeField]
	private int _skillCoinChestDenyCounter;

	[SerializeField]
	private bool _seenVehiclePartFTUE;

	[SerializeField]
	private int kaiTokens;

	[SerializeField]
	private int jayTokens;

	[SerializeField]
	private int coleTokens;

	[SerializeField]
	private int zaneTokens;

	[SerializeField]
	private int nyaTokens;

	[SerializeField]
	private int lloydTokens;

	[SerializeField]
	private int level;

	[SerializeField]
	private int weaponLevel;

	[SerializeField]
	private int spinjitzuLevel;

	[SerializeField]
	private int staminaLevel;

	[SerializeField]
	private int agilityLevel;

	[SerializeField]
	private int strengthLevel;

	[SerializeField]
	private int reconfigureCount;

	[SerializeField]
	private int skillPointDropCount;

	[SerializeField]
	private int lastSeenKaiLevel;

	[SerializeField]
	private int lastSeenJayLevel;

	[SerializeField]
	private int lastSeenColeLevel;

	[SerializeField]
	private int lastSeenZaneLevel;

	[SerializeField]
	private int lastSeenNyaLevel;

	[SerializeField]
	private int lastSeenLloydLevel;

	[SerializeField]
	private int kaiLevel;

	[SerializeField]
	private int jayLevel;

	[SerializeField]
	private int coleLevel;

	[SerializeField]
	private int zaneLevel;

	[SerializeField]
	private int nyaLevel;

	[SerializeField]
	private int lloydLevel;

	[SerializeField]
	private int kaiSuitLevel;

	[SerializeField]
	private int jaySuitLevel;

	[SerializeField]
	private int coleSuitLevel;

	[SerializeField]
	private int zaneSuitLevel;

	[SerializeField]
	private int nyaSuitLevel;

	[SerializeField]
	private int lloydSuitLevel;

	[SerializeField]
	private DictionaryChapterCursor chapterCursor = new DictionaryChapterCursor();

	[SerializeField]
	private int lastPlayedSeason = 1;

	[SerializeField]
	private bool characterPowerLocked;

	[SerializeField]
	private DictionaryLevelsUnlocked levelsUnlocked = new DictionaryLevelsUnlocked();

	[SerializeField]
	private List<PlayerBriefingType> briefingCompleted = new List<PlayerBriefingType>();

	[SerializeField]
	private List<int> levelsDiscovered = new List<int>();

	[SerializeField]
	private int totalLevelsPlayed;

	[SerializeField]
	private string language = string.Empty;

	[SerializeField]
	private bool musicDisabled;

	[SerializeField]
	private bool soundFXDisabled;

	[SerializeField]
	private uint[] colors = new uint[16];

	[SerializeField]
	private string faceTexture;

	[SerializeField]
	private string legsTexture;

	[SerializeField]
	private string pelvisTexture;

	[SerializeField]
	private string torsoTexture;

	[SerializeField]
	private string headModel;

	[SerializeField]
	private string hairModel;

	[SerializeField]
	private string helmetModel;

	[SerializeField]
	private bool helmetHidesHair;

	[SerializeField]
	private bool showHelmet;

	[SerializeField]
	private string suitModel;

	[SerializeField]
	private WeaponType weapon;

	[SerializeField]
	private bool usingAmuzoMissionObjectives;

	[SerializeField]
	private ElementalPower elementalPower;

	[SerializeField]
	private List<int> collectedSkillPointLevels = new List<int>();

	[SerializeField]
	private List<int> unlockedCollectableMinifigParts = new List<int>();

	[SerializeField]
	private List<int> usedCollectableMinifigPartLocations = new List<int>();

	[SerializeField]
	private List<int> viewedCollectableMinifigParts = new List<int>();

	[SerializeField]
	private List<int> usedCollectableVehiclePartLocations = new List<int>();

	[SerializeField]
	private List<int> viewedCollectableVehicleParts = new List<int>();

	[SerializeField]
	private List<int> _unlockedCards = new List<int>();

	[SerializeField]
	private List<int> _unseenCards = new List<int>();

	[SerializeField]
	private bool seenCharacterSwitchFTUE;

	[SerializeField]
	private bool seenBushesFTUE;

	[SerializeField]
	private bool _seenNotificationPrompt;

	public DictionaryLevelsUnlocked LevelUnlocked
	{
		get
		{
			return levelsUnlocked;
		}
	}

	public int TotalLevelsPlayed
	{
		get
		{
			return totalLevelsPlayed;
		}
		set
		{
			totalLevelsPlayed = value;
		}
	}

	public int TotalUniqueLevelsPlayed
	{
		get
		{
			return levelsUnlocked.Values.Where((LevelUnlockData x) => x.starCompletion).Count();
		}
	}

	public int NumS1A1LevelsPlayed
	{
		get
		{
			return levelsUnlocked.Values.Where((LevelUnlockData x) => x.seed < 2000 && x.starCompletion).Count();
		}
	}

	public string Language
	{
		get
		{
			return language;
		}
		set
		{
			language = value;
		}
	}

	public bool MusicDisabled
	{
		get
		{
			return musicDisabled;
		}
		set
		{
			musicDisabled = value;
		}
	}

	public bool SoundFXDisabled
	{
		get
		{
			return soundFXDisabled;
		}
		set
		{
			soundFXDisabled = value;
		}
	}

	public bool _pSeenVehiclePartFTUE
	{
		get
		{
			return _seenVehiclePartFTUE;
		}
		set
		{
			_seenVehiclePartFTUE = value;
		}
	}

	public bool _pSeenNotificationPrompt
	{
		get
		{
			return _seenNotificationPrompt;
		}
		set
		{
			_seenNotificationPrompt = value;
		}
	}

	public List<int> UnlockedNinjaSKUs
	{
		get
		{
			return unlockedNinjaSKUs;
		}
	}

	public List<string> CompletedMissions
	{
		get
		{
			return completedMissions;
		}
	}

	public int[] NinjaTeam
	{
		get
		{
			return ninjaTeam.ToArray();
		}
	}

	public int NinjaTeamSlotCount
	{
		get
		{
			if (level >= 15)
			{
				return 5;
			}
			if (level >= 8)
			{
				return 4;
			}
			if (level >= 2)
			{
				return 3;
			}
			if (level >= 1)
			{
				return 2;
			}
			return 1;
		}
	}

	public DictionaryChapterCursor ChapterCursor
	{
		get
		{
			return chapterCursor;
		}
	}

	public int LastPlayedSeason
	{
		get
		{
			return lastPlayedSeason;
		}
		set
		{
			lastPlayedSeason = value;
		}
	}

	public bool ShowWorldMapInTopBar
	{
		get
		{
			return showWorldMapInTopBar;
		}
		set
		{
			showWorldMapInTopBar = value;
		}
	}

	public bool ShowGarageInTopBar
	{
		get
		{
			return showGarageInTopBar;
		}
		set
		{
			showGarageInTopBar = value;
		}
	}

	public bool ShowSpinjitzuUnlock
	{
		get
		{
			return showSpinjitzuUnlock;
		}
		set
		{
			showSpinjitzuUnlock = value;
		}
	}

	public bool FirstTimeCruShow
	{
		get
		{
			return firstTimeCruShow;
		}
		set
		{
			firstTimeCruShow = value;
		}
	}

	public int Studs
	{
		get
		{
			return studs;
		}
		set
		{
			studs = value;
		}
	}

	public bool _pSeenCharacterSwitchTutorial
	{
		get
		{
			return seenCharacterSwitchFTUE;
		}
		set
		{
			seenCharacterSwitchFTUE = value;
		}
	}

	public bool _pSeenBushesTutorial
	{
		get
		{
			return seenBushesFTUE;
		}
		set
		{
			seenBushesFTUE = value;
		}
	}

	public int KaiTokens
	{
		get
		{
			return kaiTokens;
		}
		set
		{
			kaiTokens = value;
		}
	}

	public int JayTokens
	{
		get
		{
			return jayTokens;
		}
		set
		{
			jayTokens = value;
		}
	}

	public int ColeTokens
	{
		get
		{
			return coleTokens;
		}
		set
		{
			coleTokens = value;
		}
	}

	public int ZaneTokens
	{
		get
		{
			return zaneTokens;
		}
		set
		{
			Debug.Log("Zane Tokens now: " + zaneTokens + " -> " + value);
			zaneTokens = value;
		}
	}

	public int NyaTokens
	{
		get
		{
			return nyaTokens;
		}
		set
		{
			nyaTokens = value;
		}
	}

	public int LloydTokens
	{
		get
		{
			return lloydTokens;
		}
		set
		{
			lloydTokens = value;
		}
	}

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
		}
	}

	public int WeaponLevel
	{
		get
		{
			return weaponLevel;
		}
		set
		{
			weaponLevel = value;
		}
	}

	public int SpinjitzuLevel
	{
		get
		{
			return spinjitzuLevel;
		}
		set
		{
			spinjitzuLevel = value;
		}
	}

	public int StaminaLevel
	{
		get
		{
			return staminaLevel;
		}
		set
		{
			staminaLevel = value;
		}
	}

	public int AgilityLevel
	{
		get
		{
			return agilityLevel;
		}
		set
		{
			agilityLevel = value;
		}
	}

	public int StrengthLevel
	{
		get
		{
			return strengthLevel;
		}
		set
		{
			strengthLevel = value;
		}
	}

	public int ReconfigureCount
	{
		get
		{
			return reconfigureCount;
		}
		set
		{
			reconfigureCount = value;
		}
	}

	public int SkillPointDropCount
	{
		get
		{
			return skillPointDropCount;
		}
		set
		{
			skillPointDropCount = value;
		}
	}

	public bool CharacterPowerLocked
	{
		get
		{
			return characterPowerLocked;
		}
		set
		{
			characterPowerLocked = value;
		}
	}

	public int AvailableSkillPoints
	{
		get
		{
			return Mathf.Min(Level + SkillPointDropCount, 50) - SkillSum(WeaponLevel) - SkillSum(SpinjitzuLevel) - SkillSum(StaminaLevel) - SkillSum(AgilityLevel) - SkillSum(StrengthLevel);
		}
	}

	public int TotalSkillPointsAcquired
	{
		get
		{
			return Mathf.Min(Level + SkillPointDropCount, 50);
		}
	}

	public int _pLastSeenKaiLevel
	{
		get
		{
			return lastSeenKaiLevel;
		}
		set
		{
			lastSeenKaiLevel = value;
		}
	}

	public int _pLastSeenJayLevel
	{
		get
		{
			return lastSeenJayLevel;
		}
		set
		{
			lastSeenJayLevel = value;
		}
	}

	public int _pLastSeenColeLevel
	{
		get
		{
			return lastSeenColeLevel;
		}
		set
		{
			lastSeenColeLevel = value;
		}
	}

	public int _pLastSeenZaneLevel
	{
		get
		{
			return lastSeenZaneLevel;
		}
		set
		{
			lastSeenZaneLevel = value;
		}
	}

	public int _pLastSeenNyaLevel
	{
		get
		{
			return lastSeenNyaLevel;
		}
		set
		{
			lastSeenNyaLevel = value;
		}
	}

	public int _pLastSeenLloydLevel
	{
		get
		{
			return lastSeenLloydLevel;
		}
		set
		{
			lastSeenLloydLevel = value;
		}
	}

	public int KaiLevel
	{
		get
		{
			return kaiLevel;
		}
		set
		{
			if (kaiLevel == kaiSuitLevel)
			{
				KaiSuitLevel = value;
			}
			kaiLevel = value;
			UnlockNinja(Team.GetID(MinifigType.Kai, value));
		}
	}

	public int JayLevel
	{
		get
		{
			return jayLevel;
		}
		set
		{
			if (jayLevel == jaySuitLevel)
			{
				JaySuitLevel = value;
			}
			jayLevel = value;
			UnlockNinja(Team.GetID(MinifigType.Jay, value));
		}
	}

	public int ColeLevel
	{
		get
		{
			return coleLevel;
		}
		set
		{
			if (coleLevel == coleSuitLevel)
			{
				ColeSuitLevel = value;
			}
			coleLevel = value;
			UnlockNinja(Team.GetID(MinifigType.Cole, value));
		}
	}

	public int ZaneLevel
	{
		get
		{
			return zaneLevel;
		}
		set
		{
			if (zaneLevel == zaneSuitLevel)
			{
				ZaneSuitLevel = value;
			}
			zaneLevel = value;
			UnlockNinja(Team.GetID(MinifigType.Zane, value));
		}
	}

	public int NyaLevel
	{
		get
		{
			return nyaLevel;
		}
		set
		{
			if (nyaLevel == nyaSuitLevel)
			{
				NyaSuitLevel = value;
			}
			nyaLevel = value;
			UnlockNinja(Team.GetID(MinifigType.Nya, value));
		}
	}

	public int LloydLevel
	{
		get
		{
			return lloydLevel;
		}
		set
		{
			if (lloydLevel == lloydSuitLevel)
			{
				LloydSuitLevel = value;
			}
			lloydLevel = value;
			UnlockNinja(Team.GetID(MinifigType.Lloyd, value));
		}
	}

	public int KaiSuitLevel
	{
		get
		{
			return kaiSuitLevel;
		}
		set
		{
			kaiSuitLevel = value;
		}
	}

	public int JaySuitLevel
	{
		get
		{
			return jaySuitLevel;
		}
		set
		{
			jaySuitLevel = value;
		}
	}

	public int ColeSuitLevel
	{
		get
		{
			return coleSuitLevel;
		}
		set
		{
			coleSuitLevel = value;
		}
	}

	public int ZaneSuitLevel
	{
		get
		{
			return zaneSuitLevel;
		}
		set
		{
			zaneSuitLevel = value;
		}
	}

	public int NyaSuitLevel
	{
		get
		{
			return nyaSuitLevel;
		}
		set
		{
			nyaSuitLevel = value;
		}
	}

	public int LloydSuitLevel
	{
		get
		{
			return lloydSuitLevel;
		}
		set
		{
			lloydSuitLevel = value;
		}
	}

	public Color32 HeadColor
	{
		get
		{
			return i2c(colors[0]);
		}
		set
		{
			colors[0] = c2i(value);
		}
	}

	public Color32 HandColor
	{
		get
		{
			return i2c(colors[1]);
		}
		set
		{
			colors[1] = c2i(value);
		}
	}

	public Color32 ArmColor
	{
		get
		{
			return i2c(colors[2]);
		}
		set
		{
			colors[2] = c2i(value);
		}
	}

	public Color32 LegColor
	{
		get
		{
			return i2c(colors[3]);
		}
		set
		{
			colors[3] = c2i(value);
		}
	}

	public Color32 TorsoColor
	{
		get
		{
			return i2c(colors[4]);
		}
		set
		{
			colors[4] = c2i(value);
		}
	}

	public Color32 PelvisColor
	{
		get
		{
			return i2c(colors[5]);
		}
		set
		{
			colors[5] = c2i(value);
		}
	}

	public Color32 QuiverColor
	{
		get
		{
			return i2c(colors[6]);
		}
		set
		{
			colors[6] = c2i(value);
		}
	}

	public Color32 HelmetColor
	{
		get
		{
			return i2c(colors[7]);
		}
		set
		{
			colors[7] = c2i(value);
		}
	}

	public Color32 HairColor
	{
		get
		{
			return i2c(colors[8]);
		}
		set
		{
			colors[8] = c2i(value);
		}
	}

	public string FaceTexture
	{
		get
		{
			return faceTexture;
		}
		set
		{
			faceTexture = value;
		}
	}

	public string LegsTexture
	{
		get
		{
			return legsTexture;
		}
		set
		{
			legsTexture = value;
		}
	}

	public string PelvisTexture
	{
		get
		{
			return pelvisTexture;
		}
		set
		{
			pelvisTexture = value;
		}
	}

	public string TorsoTexture
	{
		get
		{
			return torsoTexture;
		}
		set
		{
			torsoTexture = value;
		}
	}

	public string HeadModel
	{
		get
		{
			return headModel;
		}
		set
		{
			headModel = value;
		}
	}

	public string HairModel
	{
		get
		{
			return hairModel;
		}
		set
		{
			hairModel = value;
		}
	}

	public string HelmetModel
	{
		get
		{
			return helmetModel;
		}
		set
		{
			helmetModel = value;
		}
	}

	public bool HelmetHidesHair
	{
		get
		{
			return helmetHidesHair;
		}
		set
		{
			helmetHidesHair = value;
		}
	}

	public bool ShowHelmet
	{
		get
		{
			return showHelmet;
		}
		set
		{
			showHelmet = value;
		}
	}

	public string SuitModel
	{
		get
		{
			return suitModel;
		}
		set
		{
			suitModel = value;
		}
	}

	public WeaponType Weapon
	{
		get
		{
			return weapon;
		}
		set
		{
			weapon = value;
		}
	}

	public ElementalPower ElementalPower
	{
		get
		{
			return elementalPower;
		}
		set
		{
			elementalPower = value;
		}
	}

	public int MaxHealth
	{
		get
		{
			switch (staminaLevel)
			{
			default:
				return 100;
			case 1:
				return 150;
			case 2:
				return 200;
			case 3:
				return 250;
			case 4:
				return 300;
			}
		}
	}

	public float MovementSpeed
	{
		get
		{
			switch (agilityLevel)
			{
			default:
				return 4f;
			case 1:
				return 4.25f;
			case 2:
				return 4.5f;
			case 3:
				return 4.75f;
			case 4:
				return 5f;
			}
		}
	}

	public float DamageMultiplier
	{
		get
		{
			float num = 1f;
			switch (strengthLevel)
			{
			case 0:
				num = num;
				break;
			case 1:
				num += 0.25f;
				break;
			case 2:
				num += 0.5f;
				break;
			case 3:
				num += 0.75f;
				break;
			case 4:
				num++;
				break;
			}
			switch (WeaponLevel)
			{
			case 0:
				num = num;
				break;
			case 1:
				num += 0.25f;
				break;
			case 2:
				num += 0.5f;
				break;
			case 3:
				num += 0.75f;
				break;
			case 4:
				num++;
				break;
			}
			return num;
		}
	}

	public int SpinjitzuCount
	{
		get
		{
			return SpinjitzuCountForLevel(spinjitzuLevel);
		}
	}

	public float SpinjitzuDuration
	{
		get
		{
			return SpinjitzuDurationForLevel(spinjitzuLevel);
		}
	}

	public long Timestamp
	{
		get
		{
			return timestamp;
		}
	}

	static PlayerProfile()
	{
		newlyUnlockedNinjas = new List<int>();
		newlyUnlockedVehicles = new List<int>();
		List<int> list = new List<int>();
		TextAsset textAsset = Resources.Load("levels") as TextAsset;
		using (StringReader stringReader = new StringReader(textAsset.text))
		{
			stringReader.ReadLine();
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split(',');
				int result;
				if (int.TryParse(array[2], out result))
				{
					list.Add(result);
				}
			}
		}
		xp = list.ToArray();
	}

	public PlayerProfile()
	{
		unlockedNinjaSKUs.Add(0);
		ninjaTeam = new List<int>();
		ninjaTeam.Add(0);
		areasUnlocked.Add("S1A1");
		string[] firstLevelOfAllSeasons = GameController.Instance.areaMapDB.GetFirstLevelOfAllSeasons();
		string[] array = firstLevelOfAllSeasons;
		foreach (string item in array)
		{
			areasUnlocked.Add(item);
		}
	}

	private static uint c2i(Color32 c)
	{
		return (uint)((c.r << 24) | (c.g << 16) | (c.b << 8) | c.a);
	}

	private static Color32 i2c(uint i)
	{
		return new Color32((byte)((i >> 24) & 0xFF), (byte)((i >> 16) & 0xFF), (byte)((i >> 8) & 0xFF), (byte)(i & 0xFF));
	}

	public static int LevelForXP(int value)
	{
		int result = 0;
		for (int i = 0; i < xp.Length && xp[i] <= value; i++)
		{
			result = i;
		}
		return result;
	}

	public static int VehicleLevelForTokens(int value)
	{
		if (value < 10)
		{
			return 0;
		}
		if (value < 20)
		{
			return 1;
		}
		return 2;
	}

	public static int xpForLevel(int level)
	{
		if (level < 0 || level >= xp.Length)
		{
			return 0;
		}
		return xp[level];
	}

	public bool isSkillCoinAutoDrop()
	{
		if (_skillCoinChestCounter >= 7)
		{
			_skillCoinChestDenyCounter = 3;
			_skillCoinChestCounter = 0;
			return true;
		}
		return false;
	}

	public void increaseSkillCoinRate()
	{
		if (_skillCoinChestCounter < 7)
		{
			_skillCoinChestCounter++;
		}
	}

	public bool isSkillCoinDenied()
	{
		if (_skillCoinChestDenyCounter > 0)
		{
			_skillCoinChestDenyCounter--;
			return true;
		}
		return false;
	}

	public void enableDenySkillCoin()
	{
		_skillCoinChestDenyCounter = 3;
	}

	public List<int> GetUsedVehicleParts()
	{
		return usedCollectableVehiclePartLocations;
	}

	public List<int> GetViewedVehicleParts()
	{
		return viewedCollectableVehicleParts;
	}

	public bool HasAnyUnseenMinfigParts()
	{
		return viewedCollectableMinifigParts.Count < unlockedCollectableMinifigParts.Count;
	}

	public bool HasCollectedMinifigPartByIndex(int index)
	{
		return unlockedCollectableMinifigParts.Contains(index);
	}

	public bool HasSeenMinifigPartByIndex(int index)
	{
		return viewedCollectableMinifigParts.Contains(index);
	}

	public void MarkMinifigPartSeen(int index)
	{
		viewedCollectableMinifigParts.Add(index);
	}

	public void CollectMinifigPart(int index, int seed)
	{
		if (usedCollectableMinifigPartLocations.Contains(seed))
		{
			Debug.LogError("Seed: " + seed + " Already used, cannot unlock minfig part from this location");
			return;
		}
		if (unlockedCollectableMinifigParts.Contains(index))
		{
			Debug.LogError("Minifig part " + index + " already unlocked");
			return;
		}
		GameController.Instance._collectedClothingPart = true;
		usedCollectableMinifigPartLocations.Add(seed);
		unlockedCollectableMinifigParts.Add(index);
		Debug.Log("Collected part: " + index + " in level: " + seed);
	}

	public bool HasUsedMinifigPartLocation(int seed)
	{
		return usedCollectableMinifigPartLocations.Contains(seed);
	}

	public void UnlockCard(int id)
	{
		if (_unlockedCards == null)
		{
			_unlockedCards = new List<int>();
		}
		if (_unseenCards == null)
		{
			_unseenCards = new List<int>();
		}
		if (!_unlockedCards.Contains(id))
		{
			_unlockedCards.Add(id);
			_unseenCards.Add(id);
		}
	}

	public int[] GetUnseenCards()
	{
		return _unseenCards.ToArray();
	}

	public void MarkUnseenCardAsSeen(int cardID)
	{
		_unseenCards.Remove(cardID);
	}

	public bool IsCardUnseen(int cardID)
	{
		return _unseenCards.Contains(cardID);
	}

	public bool HasCardBeenUnlocked(int id)
	{
		if (_unlockedCards == null)
		{
			return false;
		}
		return _unlockedCards.Contains(id);
	}

	public void CollectVehiclePart(int seed)
	{
		if (usedCollectableVehiclePartLocations.Contains(seed))
		{
			Debug.LogError("Seed: " + seed + " Already used, cannot unlock vehicle part from this location");
			return;
		}
		GameController.Instance._collectedVehiclePart = true;
		usedCollectableVehiclePartLocations.Add(seed);
		Debug.Log("Collected part:  in level: " + seed);
	}

	public void ViewVehiclePart(int seed)
	{
		if (viewedCollectableVehicleParts.Contains(seed))
		{
			Debug.LogError("Seed: " + seed + " Already used, cannot view vehicle part from this location");
			return;
		}
		viewedCollectableVehicleParts.Add(seed);
		Debug.Log("Viewed part  which was from level " + seed);
	}

	public bool HasUsedVehiclePartLocation(int seed)
	{
		return usedCollectableVehiclePartLocations.Contains(seed);
	}

	public bool HasViewedVehiclePartLocation(int seed)
	{
		return viewedCollectableVehicleParts.Contains(seed);
	}

	public void MarkMinfigPartLocationUsed(int seed)
	{
		if (!HasUsedMinifigPartLocation(seed))
		{
		}
	}

	public bool IsAreaMapUnlocked(AreaMapMetaLayer areaMapMetaLayer)
	{
		if (areasUnlocked.Contains(areaMapMetaLayer.areaName))
		{
			return true;
		}
		return false;
	}

	public bool IsAreaMapUnlocked(string areaMapName)
	{
		return areasUnlocked.Contains(areaMapName);
	}

	public void UnlockArea(string areaName)
	{
		if (!areasUnlocked.Contains(areaName))
		{
			areasUnlocked.Add(areaName);
		}
	}

	public string GetCurrentArea()
	{
		return currentArea;
	}

	public void SetCurrentArea(string area)
	{
		currentArea = area;
	}

	public void HandleAmuzoObjectives()
	{
		if (usingAmuzoMissionObjectives)
		{
			return;
		}
		Debug.Log("<color=yellow>Player isn't using amuzo objectives, we need to patch their save data so that they're re-aligned with the changed objective table</color>");
		Debug.Log("Current Season [" + GameController.Instance.ObjectiveController.CurrentObjective.season + "]");
		if (GameController.Instance.ObjectiveController.CurrentObjective.season != 1)
		{
			return;
		}
		Debug.Log("<color=yellow>They're on Season 1, as expected, their chapter is: " + GameController.Instance.ObjectiveController.CurrentObjective.chapter + "</color>");
		bool flag = false;
		bool flag2 = false;
		switch (GameController.Instance.ObjectiveController.CurrentObjective.chapter)
		{
		case 1:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 1, true);
			break;
		case 2:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 2, true);
			break;
		case 3:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 3, true);
			break;
		case 4:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 4, true);
			break;
		case 5:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 6, true);
			break;
		case 6:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 7, true);
			break;
		case 7:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 7, true);
			break;
		case 8:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 8, true);
			flag = true;
			break;
		case 9:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 8, true);
			flag = true;
			break;
		case 10:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 8, true);
			flag = true;
			break;
		case 11:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 9, true);
			flag = true;
			break;
		case 12:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 10, true);
			flag = true;
			flag2 = true;
			break;
		case 13:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 11, true);
			flag = true;
			flag2 = true;
			break;
		case 14:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 11, true);
			flag = true;
			flag2 = true;
			break;
		case 15:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 11, true);
			flag = true;
			flag2 = true;
			break;
		case 16:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 12, true);
			flag = true;
			flag2 = true;
			break;
		case 17:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 12, true);
			flag = true;
			flag2 = true;
			break;
		case 18:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 13, true);
			flag = true;
			flag2 = true;
			break;
		case 19:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 14, true);
			flag = true;
			flag2 = true;
			break;
		case 20:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 14, true);
			flag = true;
			flag2 = true;
			break;
		case 21:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 15, true);
			flag = true;
			flag2 = true;
			break;
		case 22:
			GameController.Instance.ObjectiveController.InitiateObjective(1, 16, true);
			flag = true;
			flag2 = true;
			break;
		}
		Debug.Log("<color=yellow>We redirected their chapter to: " + GameController.Instance.ObjectiveController.CurrentObjective.chapter + "</color>");
		if (flag)
		{
			GameController.Instance.PlayerProfile.UnlockArea("S1A2");
			AreaMapMetaLayer areaMapMetaLayer = GameController.Instance.areaMapDB.areaMaps["S1A2"];
			int startCol = areaMapMetaLayer.startCol;
			int startRow = areaMapMetaLayer.startRow;
			int baseSeed = areaMapMetaLayer.baseSeed;
			AreaMapNode areaMapNode = areaMapMetaLayer.rowArray[startRow].colArray[startCol];
			int num = baseSeed + startRow + areaMapMetaLayer.rowArray.Length * startCol;
			if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(num) && !areaMapNode.isBlocked)
			{
				LevelUnlockData levelUnlockData = new LevelUnlockData();
				levelUnlockData.season = areaMapMetaLayer.season;
				levelUnlockData.seed = num;
				GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData.seed, levelUnlockData);
				Debug.Log("<color=yellow>Level Unlock" + levelUnlockData.seed + "</color>");
			}
		}
		if (flag2)
		{
			GameController.Instance.PlayerProfile.UnlockArea("S1A3");
			AreaMapMetaLayer areaMapMetaLayer2 = GameController.Instance.areaMapDB.areaMaps["S1A3"];
			int startCol2 = areaMapMetaLayer2.startCol;
			int startRow2 = areaMapMetaLayer2.startRow;
			int baseSeed2 = areaMapMetaLayer2.baseSeed;
			AreaMapNode areaMapNode2 = areaMapMetaLayer2.rowArray[startRow2].colArray[startCol2];
			int num2 = baseSeed2 + startRow2 + areaMapMetaLayer2.rowArray.Length * startCol2;
			if (!GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(num2) && !areaMapNode2.isBlocked)
			{
				LevelUnlockData levelUnlockData2 = new LevelUnlockData();
				levelUnlockData2.season = areaMapMetaLayer2.season;
				levelUnlockData2.seed = num2;
				GameController.Instance.PlayerProfile.LevelUnlocked.Add(levelUnlockData2.seed, levelUnlockData2);
				Debug.Log("<color=yellow>Level Unlock" + levelUnlockData2.seed + "</color>");
			}
		}
		usingAmuzoMissionObjectives = true;
		Save();
	}

	public void MarkAmuzoObjectives()
	{
		usingAmuzoMissionObjectives = true;
	}

	public AreaPosition GetCurrentAreaPosition(string area = null)
	{
		if (area == null)
		{
			area = GetCurrentArea();
		}
		if (currentAreaPosition.ContainsKey(area))
		{
			return currentAreaPosition[area];
		}
		return null;
	}

	public void SetCurrentAreaPosition(int row, int col, string area = null)
	{
		if (area == null)
		{
			area = GetCurrentArea();
			if (area == null || area == string.Empty)
			{
				return;
			}
		}
		if (!currentAreaPosition.ContainsKey(area))
		{
			currentAreaPosition.Add(area, new AreaPosition());
		}
		currentAreaPosition[area].row = row;
		currentAreaPosition[area].col = col;
	}

	public LevelExtensionData GetLevelExtensionData(string extensionName)
	{
		if (levelExtensionData.ContainsKey(extensionName))
		{
			return levelExtensionData[extensionName];
		}
		return null;
	}

	public void SetLevelExtensionData(string extensionName, LevelExtensionData data)
	{
		Debug.Log("Saving SetLevelExtensionData " + extensionName);
		levelExtensionData[extensionName] = data;
	}

	public bool IsLevelDiscovered(int seed)
	{
		if (levelsDiscovered.Contains(seed))
		{
			return true;
		}
		return false;
	}

	public void DiscoverLevel(int seed)
	{
		if (!levelsDiscovered.Contains(seed))
		{
			levelsDiscovered.Add(seed);
		}
	}

	public bool IsMissionCompleted(string missionID)
	{
		if (completedMissions.Contains(missionID))
		{
			return true;
		}
		return false;
	}

	public void CompleteMission(string missionID)
	{
		if (!completedMissions.Contains(missionID))
		{
			completedMissions.Add(missionID);
		}
	}

	public void UnlockNinja(int sku)
	{
		if (!unlockedNinjaSKUs.Contains(sku))
		{
			unlockedNinjaSKUs.Add(sku);
			newlyUnlockedNinjas.Add(sku);
		}
	}

	public bool SetNinjaInTeam(int sku, int slotPosition)
	{
		if (ninjaTeam.Contains(sku))
		{
			return false;
		}
		if (!unlockedNinjaSKUs.Contains(sku))
		{
			return false;
		}
		if (slotPosition >= NinjaTeamSlotCount)
		{
			return false;
		}
		slotPosition = Mathf.Clamp(slotPosition, 0, 4);
		if (slotPosition >= ninjaTeam.Count)
		{
			ninjaTeam.Add(sku);
		}
		else
		{
			ninjaTeam[slotPosition] = sku;
		}
		return true;
	}

	public bool HasSeenBriefing(PlayerBriefingType type)
	{
		if (briefingCompleted.Contains(type))
		{
			return true;
		}
		return false;
	}

	public void BriefingComplete(PlayerBriefingType type)
	{
		if (!briefingCompleted.Contains(type))
		{
			briefingCompleted.Add(type);
		}
	}

	public int GetNinjaInTeam(int slotPosition)
	{
		if (slotPosition >= ninjaTeam.Count)
		{
			return -1;
		}
		return ninjaTeam[slotPosition];
	}

	public bool SetNinjaInTeam(int sku)
	{
		if (ninjaTeam.Contains(sku))
		{
			return false;
		}
		if (!unlockedNinjaSKUs.Contains(sku))
		{
			return false;
		}
		int num = ninjaTeam.Count;
		if (num >= NinjaTeamSlotCount)
		{
			num = NinjaTeamSlotCount - 1;
		}
		if (num >= ninjaTeam.Count)
		{
			ninjaTeam.Add(sku);
		}
		else
		{
			ninjaTeam[num] = sku;
		}
		return true;
	}

	public bool IsNinjaInTeam(int sku)
	{
		if (ninjaTeam.Contains(sku))
		{
			return true;
		}
		return false;
	}

	public void RemoveNinjaFromTeam(int ninjaID)
	{
		List<int> list = new List<int>();
		foreach (int item in ninjaTeam)
		{
			if (item != ninjaID)
			{
				list.Add(item);
			}
		}
		ninjaTeam = list;
	}

	public int GetChapterCursor(int season)
	{
		if (chapterCursor.ContainsKey(season))
		{
			return chapterCursor[season];
		}
		return -1;
	}

	public void SetChapterCursor(int season)
	{
		int chapter = GetChapterCursor(season);
		SetChapterCursor(season, chapter);
	}

	public void SetChapterCursor(int season, int chapter)
	{
		Debug.Log("Set Chapter Cursor: " + season + " / " + chapter);
		int num = season;
		int num2 = chapter - 1;
		if (chapterCursor.ContainsKey(season))
		{
			if (chapter == chapterCursor[season])
			{
				return;
			}
			chapterCursor[season] = chapter;
		}
		else
		{
			chapterCursor.Add(season, chapter);
			num--;
			num2 = GetChapterCursor(num);
		}
		if (num >= 1 && num2 >= 1)
		{
			AnalyticsEvents.LogObjectiveCompleted(num, num2);
		}
	}

	private int SkillSum(int value)
	{
		switch (value)
		{
		case 5:
			return 15;
		case 4:
			return 10;
		case 3:
			return 6;
		case 2:
			return 3;
		case 1:
			return 1;
		default:
			return 0;
		}
	}

	public bool HasCurrentLevelCollectedSkillCoin()
	{
		return collectedSkillPointLevels.Contains(GameController.Instance.CurrentLevel.Seed);
	}

	public void MarkSkillCoinCollectedForCurrentLevel()
	{
		collectedSkillPointLevels.Add(GameController.Instance.CurrentLevel.Seed);
	}

	public int GetNinjaLevel(MinifigType type)
	{
		switch (type)
		{
		case MinifigType.Kai:
			return KaiLevel;
		case MinifigType.Jay:
			return JayLevel;
		case MinifigType.Cole:
			return ColeLevel;
		case MinifigType.Zane:
			return ZaneLevel;
		case MinifigType.Nya:
			return NyaLevel;
		case MinifigType.Lloyd:
			return LloydLevel;
		default:
			return 0;
		}
	}

	public void SetNinjaLevel(MinifigType type, int value)
	{
		switch (type)
		{
		case MinifigType.Kai:
			KaiLevel = value;
			break;
		case MinifigType.Jay:
			JayLevel = value;
			break;
		case MinifigType.Cole:
			ColeLevel = value;
			break;
		case MinifigType.Zane:
			ZaneLevel = value;
			break;
		case MinifigType.Nya:
			NyaLevel = value;
			break;
		case MinifigType.Lloyd:
			LloydLevel = value;
			break;
		default:
			Debug.LogError("SetNinjaLevel for unsupported type " + type);
			break;
		}
	}

	public int GetNinjaTokens(MinifigType type)
	{
		switch (type)
		{
		case MinifigType.Kai:
			return KaiTokens;
		case MinifigType.Jay:
			return JayTokens;
		case MinifigType.Cole:
			return ColeTokens;
		case MinifigType.Zane:
			return ZaneTokens;
		case MinifigType.Nya:
			return NyaTokens;
		case MinifigType.Lloyd:
			return LloydTokens;
		default:
			return 0;
		}
	}

	public Color32[] GetColors()
	{
		Color32[] array = new Color32[colors.Length];
		for (int i = 0; i < colors.Length; i++)
		{
			array[i] = i2c(colors[i]);
		}
		return array;
	}

	public MinifigDefinition GetMinifigDefinition()
	{
		MinifigDefinition minifigDefinition = new MinifigDefinition();
		minifigDefinition.hairModel = HairModel;
		minifigDefinition.helmetModel = HelmetModel;
		minifigDefinition.helmetReplacesHead = false;
		minifigDefinition.helmetHidesHair = HelmetHidesHair;
		minifigDefinition.showHelmet = ShowHelmet;
		minifigDefinition.weapon = Weapon;
		minifigDefinition.faceDecal = FaceTexture;
		minifigDefinition.torsoDecal = TorsoTexture;
		minifigDefinition.pelvisDecal = PelvisTexture;
		minifigDefinition.legDecal = LegsTexture;
		minifigDefinition.colorSlots = null;
		minifigDefinition.colors = GetColors();
		minifigDefinition.weapon = Weapon;
		minifigDefinition.elementalPower = ElementalPower;
		minifigDefinition.id = 0;
		return minifigDefinition;
	}

	public static int SpinjitzuCountForLevel(int spinjitzuLevel)
	{
		if (spinjitzuLevel >= 5)
		{
			return 4;
		}
		if (spinjitzuLevel > 4)
		{
			return 4;
		}
		if (spinjitzuLevel > 3)
		{
			return 4;
		}
		if (spinjitzuLevel > 2)
		{
			return 4;
		}
		if (spinjitzuLevel > 1)
		{
			return 3;
		}
		if (spinjitzuLevel > 0)
		{
			return 2;
		}
		return 1;
	}

	public static float SpinjitzuDurationForLevel(int spinjitzuLevel)
	{
		if (spinjitzuLevel >= 4)
		{
			return 6f;
		}
		if (spinjitzuLevel > 2)
		{
			return 5f;
		}
		if (spinjitzuLevel > 1)
		{
			return 4f;
		}
		if (spinjitzuLevel > 0)
		{
			return 3f;
		}
		return 3f;
	}

	public void LoadLocal()
	{
		Debug.Log("<color=green>##LOADING PROFILE##</color>");
		if (!PlayerPrefs.HasKey("data"))
		{
			LoadLocalFromFile();
		}
		else
		{
			LoadLocalFromPrefs();
		}
	}

	private void LoadLocalFromPrefs()
	{
		string text = PlayerPrefs.GetString("data");
		if (!string.IsNullOrEmpty(text))
		{
			JsonUtility.FromJsonOverwrite(text, this);
		}
		text = PlayerPrefs.GetString("dataObj");
		if (!string.IsNullOrEmpty(text))
		{
			GameController.Instance.ObjectiveController.CurrentObjective = JsonUtility.FromJson<ObjectiveData>(text);
		}
	}

	private void LoadLocalFromFile()
	{
		string text = Paths.saveGameFilename;
		if (File.Exists(text))
		{
			Debug.Log("LOADING - Data found at Unity Defined persistent data path: " + text);
		}
		else
		{
			Debug.Log("LOADING - Data NOT found at Unity Defined persistent path, attempting hidden file retrieval");
			try
			{
				IntPtr javaClass = AndroidJNI.FindClass("android/content/ContextWrapper");
				IntPtr methodID = AndroidJNIHelper.GetMethodID(javaClass, "getFilesDir", "()Ljava/io/File;");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						IntPtr obj = AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, new jvalue[0]);
						IntPtr javaClass2 = AndroidJNI.FindClass("java/io/File");
						IntPtr methodID2 = AndroidJNIHelper.GetMethodID(javaClass2, "getAbsolutePath", "()Ljava/lang/String;");
						string text2 = AndroidJNI.CallStringMethod(obj, methodID2, new jvalue[0]);
						if (text2 != null)
						{
							Debug.Log("LOADING - found internal path for this device:" + text2);
							text = Path.Combine(text2, "savegame.txt");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log("LOADING - exception on trying to find data directory: " + ex.ToString());
			}
			Debug.Log("LOADING - looking for data at: " + text);
		}
		if (File.Exists(text))
		{
			Debug.Log("LOADING - Existing save data found!");
			string[] array = File.ReadAllLines(text);
			if (array.Length > 0)
			{
				JsonUtility.FromJsonOverwrite(array[0], this);
				if (array.Length > 1)
				{
					GameController.Instance.ObjectiveController.CurrentObjective = JsonUtility.FromJson<ObjectiveData>(array[1]);
				}
			}
			HandleAmuzoObjectives();
		}
		else
		{
			Debug.Log("LOADING - no previously saved data found");
		}
		Debug.Log("levels: " + totalLevelsPlayed);
	}

	public void Save()
	{
		Debug.Log("## SAVING PLAYERPROFILE ##");
		Debug.Log(Paths.saveGameFilename);
		timestamp = DateTime.UtcNow.Ticks;
		string value = JsonUtility.ToJson(this);
		string value2 = JsonUtility.ToJson(GameController.Instance.ObjectiveController.CurrentObjective);
		PlayerPrefs.SetString("data", value);
		PlayerPrefs.SetString("dataObj", value2);
	}

	public static void Reset()
	{
		File.Delete(Paths.saveGameFilename);
	}
}
