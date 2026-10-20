using System.Collections.Generic;
using System.Text;
using AmuzoEngine;
using UnityEngine;

public class ScreenResults : ScreenBase
{
	public enum STATE
	{
		INIT = 0,
		CRYSTALS_SMASHED = 1,
		CRYSTALS_SMASHED_RESULT = 2,
		COLLECTION = 3,
		COLLECTION_RESULT = 4,
		OBJECTIVES = 5,
		OBJECTIVES_RESULT = 6,
		DONE = 7
	}

	private static bool _inResults;

	public BoxTransitionFX _boxTransitionFX;

	public UIWidget _stage1Crystal;

	public ParticleSystem _stage1Particles;

	public ParticleSystem[] _collectionParticles;

	public UIWidget[] _ticks;

	public UIWidget[] _crosses;

	public Transform[] _positionsOdd;

	public Transform[] _positionsEven;

	public Transform[] _positionsOddOrdered;

	public Transform[] _positionsEvenOrdered;

	public UIWidget[] _poolChestWood;

	public UIWidget[] _poolChestSilver;

	public UIWidget[] _poolChestGold;

	public UIWidget[] _poolCrystalNormal;

	public UIWidget[] _poolCrystalEarth;

	public UIWidget[] _poolCrystalFire;

	public UIWidget[] _poolCrystalIce;

	public UIWidget[] _poolCrystalElectricity;

	public UIWidget[] _poolCrystalEnergy;

	public UIWidget[] _poolCrystalWater;

	public Transform _objectivesPos;

	public Transform _objectivesEvenPos;

	public Transform _objectivesOddPos;

	public UILabel[] _objectiveLabels;

	public UITexture[] _objectiveIcons;

	public ParticleSystem[] _cat1Shinies;

	public ParticleSystem[] _cat2Shinies;

	public ParticleSystem[] _cat3Shinies;

	private int _studsAcquired;

	private STATE _currentState;

	private float _stateStartTime;

	private bool _dummyBool;

	private int _dummyInt;

	private Vector3 _dummyVector;

	private int _collectionSize;

	private int _objectiveSize;

	private UIWidget[] _collectionWidgets = new UIWidget[9];

	private bool[] _collectionMet = new bool[9];

	private bool[] _collectionParticlesFired = new bool[9];

	private bool[] _objectiveMet = new bool[3];

	private bool _allCollectionsMet;

	private bool _allChallengesMet;

	private bool _triggeredResultParticles;

	public static bool _pInResults
	{
		get
		{
			return _inResults;
		}
	}

	protected override void OnScreenExitComplete()
	{
		_inResults = false;
		base.OnScreenExitComplete();
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
		InGameCam._pInstance.gameObject.SetActive(false);
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		Texture2D[] array = CharacterPreviewRenderer.Generate(new int[1], 256, 256);
		array[0].wrapMode = TextureWrapMode.Clamp;
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenResultsExp screenInstance = instance.GetScreenInstance<ScreenResultsExp>();
		screenInstance._playerTexture.mainTexture = array[0];
		_inResults = true;
		_boxTransitionFX.Begin();
		if (StudsCounter._pInstance != null)
		{
			_studsAcquired = StudsCounter._pInstance.StudsCollected;
			int num = PlayerProfile.xpForLevel(1);
			int studs = GameController.Instance.PlayerProfile.Studs;
			if (studs < num)
			{
				_studsAcquired = num;
			}
			ScreenResultsExp._pStudsAcquired = _studsAcquired;
		}
		else
		{
			ScreenResultsExp._pStudsAcquired = 0;
		}
		Debug.Log("Studs Acquired = " + _studsAcquired);
		int num2 = PlayerProfile.LevelForXP(GameController.Instance.PlayerProfile.Studs);
		int studs2 = GameController.Instance.PlayerProfile.Studs;
		int num3 = studs2 + _studsAcquired;
		int num4 = PlayerProfile.xpForLevel(num2 + 1);
		int num5 = num4 - studs2;
		int num6 = 0;
		if (num3 >= num4 && num4 != 0)
		{
			num2++;
			num6++;
			GameController.Instance.PlayerProfile.Level++;
			while (num3 >= num4 && num4 != 0)
			{
				num4 = PlayerProfile.xpForLevel(num2 + 1);
				if (num3 >= num4 && num4 != 0)
				{
					num2++;
					num6++;
					GameController.Instance.PlayerProfile.Level++;
				}
			}
		}
		if (num6 > 0)
		{
		}
		if (num6 > 0)
		{
			GameController.Instance.PlayerProfile.Save();
		}
		GlobalInGameData.SaveState(_studsAcquired);
		ChangeState(STATE.INIT);
	}

	public void OnContinue()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		if (ScreenResultsCard._acquiredCard == ECOLLECTABLE_CARD.INVALID)
		{
			ScreenResultsExp screenInstance = instance.GetScreenInstance<ScreenResultsExp>();
			if (GlobalInGameData.ShouldLaunchMovieCampaign())
			{
				GlobalInGameData.LaunchMovieCampaign();
			}
			Navigate(screenInstance);
		}
		else
		{
			ScreenResultsCard screenInstance2 = instance.GetScreenInstance<ScreenResultsCard>();
			Navigate(screenInstance2);
		}
	}

	private void ChangeState(STATE newState)
	{
		_currentState = newState;
		_stateStartTime = RealTime.time;
		switch (_currentState)
		{
		case STATE.INIT:
			StartInit();
			break;
		case STATE.CRYSTALS_SMASHED:
			StartCrystalsSmashed();
			break;
		case STATE.CRYSTALS_SMASHED_RESULT:
			StartCrystalsSmashedResult();
			break;
		case STATE.COLLECTION:
			StartCollection();
			break;
		case STATE.COLLECTION_RESULT:
			StartCollectionResult();
			break;
		case STATE.OBJECTIVES:
			StartObjectives();
			break;
		case STATE.OBJECTIVES_RESULT:
			StartObjectivesResult();
			break;
		}
	}

	protected override void Update()
	{
		base.Update();
		switch (_currentState)
		{
		case STATE.INIT:
			UpdateInit();
			break;
		case STATE.CRYSTALS_SMASHED:
			UpdateCrystalsSmashed();
			break;
		case STATE.CRYSTALS_SMASHED_RESULT:
			UpdateCrystalsSmashedResult();
			break;
		case STATE.COLLECTION:
			UpdateCollection();
			break;
		case STATE.COLLECTION_RESULT:
			UpdateCollectionResult();
			break;
		case STATE.OBJECTIVES:
			UpdateObjectives();
			break;
		case STATE.OBJECTIVES_RESULT:
			UpdateObjectivesResult();
			break;
		}
	}

	private void StartInit()
	{
		_allCollectionsMet = true;
		_stage1Crystal.alpha = 0f;
		_stage1Crystal.transform.localScale = Vector3.one * 3f;
		int num = _ticks.Length;
		for (int i = 0; i < num; i++)
		{
			_ticks[i].enabled = false;
		}
		int num2 = _crosses.Length;
		for (int j = 0; j < num2; j++)
		{
			_crosses[j].enabled = false;
		}
		int num3 = _poolChestWood.Length;
		for (int k = 0; k < num3; k++)
		{
			_poolChestWood[k].enabled = false;
		}
		num3 = _poolChestSilver.Length;
		for (int l = 0; l < num3; l++)
		{
			_poolChestSilver[l].enabled = false;
		}
		num3 = _poolChestGold.Length;
		for (int m = 0; m < num3; m++)
		{
			_poolChestGold[m].enabled = false;
		}
		num3 = _poolCrystalEarth.Length;
		for (int n = 0; n < num3; n++)
		{
			_poolCrystalEarth[n].enabled = false;
		}
		num3 = _poolCrystalElectricity.Length;
		for (int num4 = 0; num4 < num3; num4++)
		{
			_poolCrystalElectricity[num4].enabled = false;
		}
		num3 = _poolCrystalEnergy.Length;
		for (int num5 = 0; num5 < num3; num5++)
		{
			_poolCrystalEnergy[num5].enabled = false;
		}
		num3 = _poolCrystalFire.Length;
		for (int num6 = 0; num6 < num3; num6++)
		{
			_poolCrystalFire[num6].enabled = false;
		}
		num3 = _poolCrystalNormal.Length;
		for (int num7 = 0; num7 < num3; num7++)
		{
			_poolCrystalNormal[num7].enabled = false;
		}
		num3 = _poolCrystalWater.Length;
		for (int num8 = 0; num8 < num3; num8++)
		{
			_poolCrystalWater[num8].enabled = false;
		}
		num3 = _poolCrystalIce.Length;
		for (int num9 = 0; num9 < num3; num9++)
		{
			_poolCrystalIce[num9].enabled = false;
		}
		num3 = _objectiveIcons.Length;
		for (int num10 = 0; num10 < num3; num10++)
		{
			_objectiveIcons[num10].enabled = false;
		}
		num3 = _objectiveLabels.Length;
		for (int num11 = 0; num11 < num3; num11++)
		{
			_objectiveLabels[num11].enabled = false;
		}
	}

	private void UpdateInit()
	{
		float num = RealTime.time - _stateStartTime;
		if (num >= 1f)
		{
			ChangeState(STATE.CRYSTALS_SMASHED);
		}
	}

	private void StartCrystalsSmashed()
	{
		_dummyBool = false;
	}

	private void UpdateCrystalsSmashed()
	{
		float num = RealTime.time - _stateStartTime;
		float num2 = 0.1f;
		if (num < num2)
		{
			float num3 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, num2, 0f, 1f);
			_stage1Crystal.alpha = num3;
			_stage1Crystal.transform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, num3);
		}
		else if (!_dummyBool)
		{
			_dummyBool = true;
			_stage1Crystal.transform.localScale = Vector3.one;
			_stage1Crystal.alpha = 1f;
			_stage1Particles.Play();
			AudioPlayer.PlaySound("ui/celebration/countup/tick");
		}
		if (num > 0.5f)
		{
			ChangeState(STATE.CRYSTALS_SMASHED_RESULT);
		}
	}

	private void StartCrystalsSmashedResult()
	{
		_ticks[0].enabled = true;
		_ticks[0].alpha = 0f;
		_triggeredResultParticles = false;
	}

	private void UpdateCrystalsSmashedResult()
	{
		float num = RealTime.time - _stateStartTime;
		float num2 = 0.2f;
		if (num < num2)
		{
			float num3 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, num2, 0f, 1f);
			_ticks[0].alpha = num3;
			_ticks[0].transform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, num3);
			return;
		}
		if (!_triggeredResultParticles)
		{
			_triggeredResultParticles = true;
			int num4 = _cat1Shinies.Length;
			AudioPlayer.PlaySound("ui/panel/impact");
			for (int i = 0; i < num4; i++)
			{
				_cat1Shinies[i].Play();
			}
		}
		else
		{
			AudioPlayer.PlaySound("player/attack/fail");
		}
		_ticks[0].transform.localScale = Vector3.one;
		_ticks[0].alpha = 1f;
		ChangeState(STATE.COLLECTION);
	}

	private void StartCollection()
	{
		int numChestsWoodToCollect = GameController.Instance.MissionStats.NumChestsWoodToCollect;
		numChestsWoodToCollect += GameController.Instance.MissionStats.NumChestsSilverToCollect;
		numChestsWoodToCollect += GameController.Instance.MissionStats.NumChestsGoldToCollect;
		int numCrystalsToCollect = GameController.Instance.MissionStats.NumCrystalsToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsEarthToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsFireToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsIceToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsElectricityToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsEnergyToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsWaterToCollect;
		_collectionSize = numCrystalsToCollect + numChestsWoodToCollect;
		_dummyInt = 0;
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumChestsWoodToCollect, GameController.Instance.MissionStats.NumChestsWoodCollected, _poolChestWood);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumChestsSilverToCollect, GameController.Instance.MissionStats.NumChestsSilverCollected, _poolChestSilver);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumChestsGoldToCollect, GameController.Instance.MissionStats.NumChestsGoldCollected, _poolChestGold);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsToCollect, GameController.Instance.MissionStats.NumCrystalsCollected, _poolCrystalNormal);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsEarthToCollect, GameController.Instance.MissionStats.NumCrystalsEarthCollected, _poolCrystalEarth);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsFireToCollect, GameController.Instance.MissionStats.NumCrystalsFireCollected, _poolCrystalFire);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsIceToCollect, GameController.Instance.MissionStats.NumCrystalsIceCollected, _poolCrystalIce);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsElectricityToCollect, GameController.Instance.MissionStats.NumCrystalsElectricityCollected, _poolCrystalElectricity);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsEnergyToCollect, GameController.Instance.MissionStats.NumCrystalsEnergyCollected, _poolCrystalEnergy);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsWaterToCollect, GameController.Instance.MissionStats.NumCrystalsWaterCollected, _poolCrystalWater);
		float num = 99999f;
		int num2 = 0;
		if ((float)_collectionSize % 2f == 0f)
		{
			for (int i = 0; i < _collectionSize; i++)
			{
				if (_positionsEven[i].transform.position.x < num)
				{
					num = _positionsEven[i].transform.position.x;
					num2 = i;
				}
			}
			int num3 = _positionsEvenOrdered.Length;
			int num4 = 0;
			for (int j = 0; j < num3; j++)
			{
				if (_positionsEvenOrdered[j] == _positionsEven[num2])
				{
					num4 = j;
					break;
				}
			}
			for (int k = 0; k < _collectionSize; k++)
			{
				_collectionWidgets[k].transform.position = _positionsEvenOrdered[num4].position;
				Vector3 position = _positionsEvenOrdered[num4].position;
				position.z = -2f;
				_collectionParticles[k].transform.position = position;
				_collectionParticlesFired[k] = false;
				num4++;
			}
		}
		else
		{
			for (int l = 0; l < _collectionSize; l++)
			{
				if (_positionsOdd[l].transform.position.x < num)
				{
					num = _positionsOdd[l].transform.position.x;
					num2 = l;
				}
			}
			int num5 = _positionsOddOrdered.Length;
			int num6 = 0;
			for (int m = 0; m < num5; m++)
			{
				if (_positionsOddOrdered[m] == _positionsOdd[num2])
				{
					num6 = m;
					break;
				}
			}
			for (int n = 0; n < _collectionSize; n++)
			{
				_collectionWidgets[n].transform.position = _positionsOddOrdered[num6].position;
				Vector3 position2 = _positionsOddOrdered[num6].position;
				position2.z = -2f;
				_collectionParticles[n].transform.position = position2;
				_collectionParticlesFired[n] = false;
				num6++;
			}
		}
		_dummyInt = 0;
		_dummyBool = false;
	}

	private void PopulateCollectionWidgets(int numElements, int numCollected, UIWidget[] pool)
	{
		if (numElements != 0 && numCollected < numElements)
		{
			_allCollectionsMet = false;
		}
		for (int i = 0; i < numElements; i++)
		{
			_collectionMet[_dummyInt] = i < numCollected;
			_collectionWidgets[_dummyInt] = pool[i];
			_dummyInt++;
		}
	}

	private void UpdateCollection()
	{
		float num = 0.1f;
		float num2 = 0.25f;
		float num3 = RealTime.time - _stateStartTime;
		float num4 = num3 - (num + num2) * (float)_dummyInt;
		if (!_dummyBool)
		{
			AudioPlayer.PlaySound("ui/celebration/countup/tick");
			_dummyBool = true;
			_dummyVector = _collectionWidgets[_dummyInt].transform.localPosition;
		}
		if (num4 < num)
		{
			float num5 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num4, num, 0f, 1f);
			_collectionWidgets[_dummyInt].enabled = true;
			_collectionWidgets[_dummyInt].alpha = num5;
			_collectionWidgets[_dummyInt].transform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, num5);
			return;
		}
		_collectionWidgets[_dummyInt].transform.localScale = Vector3.one;
		_collectionWidgets[_dummyInt].alpha = ((!_collectionMet[_dummyInt]) ? 0.25f : 1f);
		if (num4 - num < num2)
		{
			if (!_collectionMet[_dummyInt])
			{
				_collectionWidgets[_dummyInt].transform.localPosition = _dummyVector + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0f);
			}
			else if (!_collectionParticlesFired[_dummyInt])
			{
				_collectionParticles[_dummyInt].Play();
				_collectionParticlesFired[_dummyInt] = true;
			}
		}
		else
		{
			_collectionWidgets[_dummyInt].transform.localPosition = _dummyVector;
			_dummyInt++;
			_dummyBool = false;
			if (_dummyInt >= _collectionSize)
			{
				ChangeState(STATE.COLLECTION_RESULT);
			}
		}
	}

	private void StartCollectionResult()
	{
		_ticks[1].enabled = _allCollectionsMet;
		_ticks[1].alpha = 0f;
		_crosses[1].enabled = !_allCollectionsMet;
		_crosses[1].alpha = 0f;
		_crosses[1].transform.localPosition = _ticks[1].transform.localPosition;
		_dummyVector = _crosses[1].transform.localPosition;
		_triggeredResultParticles = false;
	}

	private void UpdateCollectionResult()
	{
		float num = RealTime.time - _stateStartTime;
		float num2 = 0.2f;
		if (num < num2)
		{
			float num3 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, num2, 0f, 1f);
			Vector3 localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, num3);
			_ticks[1].alpha = num3;
			_ticks[1].transform.localScale = localScale;
			_crosses[1].alpha = num3;
			_crosses[1].transform.localScale = localScale;
			return;
		}
		_ticks[1].transform.localScale = Vector3.one;
		_ticks[1].alpha = 1f;
		_crosses[1].transform.localScale = Vector3.one;
		_crosses[1].alpha = 1f;
		_crosses[1].transform.localPosition = _dummyVector + new Vector3(Random.Range(-4, 4), Random.Range(-4, 4), 0f);
		if (_allCollectionsMet && !_triggeredResultParticles)
		{
			AudioPlayer.PlaySound("ui/panel/impact");
			_triggeredResultParticles = true;
			int num4 = _cat2Shinies.Length;
			for (int i = 0; i < num4; i++)
			{
				_cat2Shinies[i].Play();
			}
		}
		else
		{
			AudioPlayer.PlaySound("player/attack/fail");
		}
		if (num > num2 + 0.25f)
		{
			_crosses[1].transform.localPosition = _dummyVector;
			ChangeState(STATE.OBJECTIVES);
		}
	}

	private void StartObjectives()
	{
		Dictionary<LevelChallengeBase, bool>.Enumerator enumerator = GameController.Instance.MissionStats.Challenges.GetEnumerator();
		int num = _objectiveMet.Length;
		for (int i = 0; i < num; i++)
		{
			_objectiveMet[i] = false;
		}
		_allChallengesMet = true;
		int num2 = 0;
		while (enumerator.MoveNext())
		{
			string path = enumerator.Current.Key.IconPath();
			Texture mainTexture = Resources.Load<Texture>(path);
			_objectiveIcons[num2].mainTexture = mainTexture;
			bool flag = enumerator.Current.Key.HasCounter();
			_objectiveMet[num2] = enumerator.Current.Value;
			if (!_objectiveMet[num2])
			{
				_allChallengesMet = false;
			}
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(enumerator.Current.Key.AmountCollected());
				stringBuilder.Append("/");
				stringBuilder.Append(enumerator.Current.Key.AmountToCollect());
				string text = stringBuilder.ToString();
				_objectiveLabels[num2].text = text;
			}
			else
			{
				_objectiveLabels[num2].text = string.Empty;
			}
			num2++;
		}
		_objectiveSize = num2;
		Debug.Log(_objectiveSize + " = " + (float)_objectiveSize % 2f);
		if ((float)_objectiveSize % 2f == 0f)
		{
			_objectivesPos.localPosition = _objectivesEvenPos.localPosition;
		}
		else
		{
			_objectivesPos.localPosition = _objectivesOddPos.localPosition;
		}
		_dummyInt = 0;
	}

	private void UpdateObjectives()
	{
		if (_objectiveSize == 0)
		{
			ChangeState(STATE.OBJECTIVES_RESULT);
			return;
		}
		float num = 0.1f;
		float num2 = 0.25f;
		float num3 = RealTime.time - _stateStartTime;
		float num4 = num3 - (num + num2) * (float)_dummyInt;
		if (!_dummyBool)
		{
			_dummyBool = true;
			_dummyVector = _objectiveIcons[_dummyInt].transform.localPosition;
			_objectiveIcons[_dummyInt].enabled = true;
			_objectiveLabels[_dummyInt].enabled = true;
			AudioPlayer.PlaySound("ui/celebration/countup/tick");
		}
		if (num4 < num)
		{
			float num5 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num4, num, 0f, 1f);
			_objectiveIcons[_dummyInt].enabled = true;
			_objectiveIcons[_dummyInt].alpha = num5;
			_objectiveLabels[_dummyInt].alpha = num5;
			_objectiveIcons[_dummyInt].transform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, num5);
			return;
		}
		_objectiveIcons[_dummyInt].transform.localScale = Vector3.one;
		_objectiveIcons[_dummyInt].alpha = 1f;
		_objectiveLabels[_dummyInt].alpha = 1f;
		if (num4 - num < num2)
		{
			if (!_objectiveMet[_dummyInt])
			{
				_objectiveIcons[_dummyInt].transform.localPosition = _dummyVector + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0f);
			}
			return;
		}
		_objectiveIcons[_dummyInt].transform.localPosition = _dummyVector;
		_dummyInt++;
		_dummyBool = false;
		if (_dummyInt >= _objectiveSize)
		{
			ChangeState(STATE.OBJECTIVES_RESULT);
		}
	}

	private void StartObjectivesResult()
	{
		_ticks[2].enabled = _allChallengesMet;
		_ticks[2].alpha = 0f;
		_crosses[2].enabled = !_allChallengesMet;
		_crosses[2].alpha = 0f;
		_crosses[2].transform.localPosition = _ticks[2].transform.localPosition;
		_dummyVector = _crosses[1].transform.localPosition;
		_triggeredResultParticles = false;
	}

	private void UpdateObjectivesResult()
	{
		float num = RealTime.time - _stateStartTime;
		float num2 = 0.2f;
		if (num < num2)
		{
			float num3 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, num2, 0f, 1f);
			Vector3 localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, num3);
			_ticks[2].alpha = num3;
			_ticks[2].transform.localScale = localScale;
			_crosses[2].alpha = num3;
			_crosses[2].transform.localScale = localScale;
			return;
		}
		_ticks[2].transform.localScale = Vector3.one;
		_ticks[2].alpha = 1f;
		_crosses[2].transform.localScale = Vector3.one;
		_crosses[2].alpha = 1f;
		_crosses[2].transform.localPosition = _dummyVector + new Vector3(Random.Range(-4, 4), Random.Range(-4, 4), 0f);
		if (_allChallengesMet && !_triggeredResultParticles)
		{
			_triggeredResultParticles = true;
			int num4 = _cat3Shinies.Length;
			AudioPlayer.PlaySound("ui/panel/impact");
			for (int i = 0; i < num4; i++)
			{
				_cat3Shinies[i].Play();
			}
		}
		else
		{
			AudioPlayer.PlaySound("player/attack/fail");
		}
		if (num > num2 + 0.25f)
		{
			_crosses[2].transform.localPosition = _dummyVector;
			ChangeState(STATE.DONE);
		}
	}
}
