using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenPause : ScreenBase
{
	public GameObject[] _pauseBits;

	public GameObject _confirmBits;

	public Action _onMapAction;

	public Action _onContinueAction;

	public UILabel _ver;

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

	public UISprite _soundIcon;

	public Transform _objectivesPos;

	public Transform _objectivesEvenPos;

	public Transform _objectivesOddPos;

	public UILabel[] _objectiveLabels;

	public UITexture[] _objectiveIcons;

	private int _dummyInt;

	private UIWidget[] _collectionWidgets = new UIWidget[9];

	private int _collectionSize;

	public Transform[] _positionsOdd;

	public Transform[] _positionsEven;

	public Transform[] _positionsOddOrdered;

	public Transform[] _positionsEvenOrdered;

	public UIButton _mapScreenButton;

	public BoxTransitionFX _boxTransitionFX;

	private bool _isExitFromPause;

	private bool _sfxHack;

	private int _sfxHackCounter;

	protected override void Update()
	{
		base.Update();
		if (_sfxHack)
		{
			if (Time.timeScale != 1f)
			{
				Time.timeScale = 1f;
			}
			_sfxHackCounter++;
			if (_sfxHackCounter >= 10)
			{
				_sfxHackCounter = 0;
				_sfxHack = false;
				Time.timeScale = 0f;
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape) && base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			OnContinue();
		}
	}

	protected override void OnShowScreen()
	{
		_sfxHack = false;
		_sfxHackCounter = 0;
		_isExitFromPause = false;
		_confirmBits.SetActive(false);
		for (int i = 0; i < _pauseBits.Length; i++)
		{
			_pauseBits[i].SetActive(true);
		}
		_boxTransitionFX.enabled = true;
		base.OnShowScreen();
		Time.timeScale = 0f;
		_ver.text = "v" + AnvilBuildInfo._pBambooVersion;
		_mapScreenButton.isEnabled = GameController.Instance.PlayerProfile.TotalLevelsPlayed > 0;
		int num = _poolChestWood.Length;
		for (int j = 0; j < num; j++)
		{
			_poolChestWood[j].enabled = false;
		}
		num = _poolChestSilver.Length;
		for (int k = 0; k < num; k++)
		{
			_poolChestSilver[k].enabled = false;
		}
		num = _poolChestGold.Length;
		for (int l = 0; l < num; l++)
		{
			_poolChestGold[l].enabled = false;
		}
		num = _poolCrystalEarth.Length;
		for (int m = 0; m < num; m++)
		{
			_poolCrystalEarth[m].enabled = false;
		}
		num = _poolCrystalElectricity.Length;
		for (int n = 0; n < num; n++)
		{
			_poolCrystalElectricity[n].enabled = false;
		}
		num = _poolCrystalEnergy.Length;
		for (int num2 = 0; num2 < num; num2++)
		{
			_poolCrystalEnergy[num2].enabled = false;
		}
		num = _poolCrystalFire.Length;
		for (int num3 = 0; num3 < num; num3++)
		{
			_poolCrystalFire[num3].enabled = false;
		}
		num = _poolCrystalNormal.Length;
		for (int num4 = 0; num4 < num; num4++)
		{
			_poolCrystalNormal[num4].enabled = false;
		}
		num = _poolCrystalWater.Length;
		for (int num5 = 0; num5 < num; num5++)
		{
			_poolCrystalWater[num5].enabled = false;
		}
		num = _poolCrystalIce.Length;
		for (int num6 = 0; num6 < num; num6++)
		{
			_poolCrystalIce[num6].enabled = false;
		}
		num = _objectiveIcons.Length;
		for (int num7 = 0; num7 < num; num7++)
		{
			_objectiveIcons[num7].enabled = false;
		}
		num = _objectiveLabels.Length;
		for (int num8 = 0; num8 < num; num8++)
		{
			_objectiveLabels[num8].enabled = false;
		}
		_dummyInt = 0;
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
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumChestsWoodToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.WoodChest), _poolChestWood);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumChestsSilverToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.MetalChest), _poolChestSilver);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumChestsGoldToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.GoldChest), _poolChestGold);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard), _poolCrystalNormal);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsEarthToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_earth), _poolCrystalEarth);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsFireToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_fire), _poolCrystalFire);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsIceToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_ice), _poolCrystalIce);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsElectricityToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_electricity), _poolCrystalElectricity);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsEnergyToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_energy), _poolCrystalEnergy);
		PopulateCollectionWidgets(GameController.Instance.MissionStats.NumCrystalsWaterToCollect, GamePlayer.Instance.Level.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard_water), _poolCrystalWater);
		float num9 = 99999f;
		int num10 = 0;
		if ((float)_collectionSize % 2f == 0f)
		{
			for (int num11 = 0; num11 < _collectionSize; num11++)
			{
				if (_positionsEven[num11].transform.position.x < num9)
				{
					num9 = _positionsEven[num11].transform.position.x;
					num10 = num11;
				}
			}
			int num12 = _positionsEvenOrdered.Length;
			int num13 = 0;
			for (int num14 = 0; num14 < num12; num14++)
			{
				if (_positionsEvenOrdered[num14] == _positionsEven[num10])
				{
					num13 = num14;
					break;
				}
			}
			for (int num15 = 0; num15 < _collectionSize; num15++)
			{
				_collectionWidgets[num15].transform.position = _positionsEvenOrdered[num13].position;
				Vector3 position = _positionsEvenOrdered[num13].position;
				position.z = -2f;
				num13++;
			}
		}
		else
		{
			for (int num16 = 0; num16 < _collectionSize; num16++)
			{
				if (_positionsOdd[num16].transform.position.x < num9)
				{
					num9 = _positionsOdd[num16].transform.position.x;
					num10 = num16;
				}
			}
			int num17 = _positionsOddOrdered.Length;
			int num18 = 0;
			for (int num19 = 0; num19 < num17; num19++)
			{
				if (_positionsOddOrdered[num19] == _positionsOdd[num10])
				{
					num18 = num19;
					break;
				}
			}
			for (int num20 = 0; num20 < _collectionSize; num20++)
			{
				_collectionWidgets[num20].transform.position = _positionsOddOrdered[num18].position;
				Vector3 position2 = _positionsOddOrdered[num18].position;
				position2.z = -2f;
				num18++;
			}
		}
		string path = GameController.Instance.CurrentLevel.Challenge.IconPath();
		Texture mainTexture = Resources.Load<Texture>(path);
		_objectiveIcons[0].mainTexture = mainTexture;
		bool flag = GameController.Instance.CurrentLevel.Challenge.HasCounter();
		bool isCompleted = GameController.Instance.CurrentLevel.Challenge.IsCompleted;
		if (flag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GameController.Instance.CurrentLevel.Challenge.AmountCollected());
			stringBuilder.Append("/");
			stringBuilder.Append(GameController.Instance.CurrentLevel.Challenge.AmountToCollect());
			string text = stringBuilder.ToString();
			_objectiveLabels[0].text = text;
		}
		else
		{
			_objectiveLabels[0].text = string.Empty;
		}
		_objectiveIcons[0].enabled = true;
		_objectiveLabels[0].enabled = true;
		_objectiveIcons[0].alpha = ((!isCompleted) ? 0.25f : 1f);
		int num21 = 1;
		if ((float)num21 % 2f == 0f)
		{
			_objectivesPos.localPosition = _objectivesEvenPos.localPosition;
		}
		else
		{
			_objectivesPos.localPosition = _objectivesOddPos.localPosition;
		}
		_boxTransitionFX.Begin();
		RefreshSoundIcon();
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
		Time.timeScale = 1f;
		if (!_isExitFromPause)
		{
			_onMapAction = null;
		}
		_onContinueAction = null;
	}

	public void OnSoundButton()
	{
		if (!_sfxHack)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			if (!GameController.Instance.PlayerProfile.SoundFXDisabled || !GameController.Instance.PlayerProfile.MusicDisabled)
			{
				GameController.Instance.PlayerProfile.SoundFXDisabled = true;
				GameController.Instance.PlayerProfile.MusicDisabled = true;
				GameAudioManager.Instance.FxMuted = true;
				GameAudioManager.Instance.MusicMuted = true;
				_sfxHack = true;
			}
			else
			{
				GameController.Instance.PlayerProfile.SoundFXDisabled = false;
				GameController.Instance.PlayerProfile.MusicDisabled = false;
				GameAudioManager.Instance.FxMuted = false;
				GameAudioManager.Instance.MusicMuted = false;
				_sfxHack = true;
			}
			RefreshSoundIcon();
		}
	}

	public void OnMap()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		_confirmBits.SetActive(true);
		for (int i = 0; i < _pauseBits.Length; i++)
		{
			_pauseBits[i].SetActive(false);
		}
	}

	public void onEndFlowCompleteFromPause()
	{
		if (_onMapAction != null)
		{
			_onMapAction();
		}
		_onMapAction = null;
		SceneManager.LoadScene("Empty");
	}

	public void onEndFlowComplete()
	{
		SceneManager.LoadScene("Empty");
		if (GlobalInGameData.ShouldLaunchHandsOfTimeCampaign())
		{
			GlobalInGameData.LaunchHandsOfTimeCampaign();
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenCutsceneHandsOfTimeIntro screenInstance = instance.GetScreenInstance<ScreenCutsceneHandsOfTimeIntro>();
			ScreenHud._pInstance.Navigate(screenInstance);
		}
		else
		{
			ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
			ScreenMap screenInstance2 = instance2.GetScreenInstance<ScreenMap>();
			ScreenHud._pInstance.Navigate(screenInstance2);
		}
	}

	public void OnContinue()
	{
		if (base._pCurrentTweenType == ScreenTweenType.Idle)
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			if (_onContinueAction != null)
			{
				_onContinueAction();
			}
			ScreenHud._pInstance.OnUnpause();
			EndFlow();
		}
	}

	public void onConfirmButton()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		_isExitFromPause = true;
		EndFlow(OnPauseScreenConfirmComplete);
	}

	public void onCancelButton()
	{
		AudioPlayer.PlaySound("ui/button/back/click");
		_boxTransitionFX.enabled = false;
		_confirmBits.SetActive(false);
		for (int i = 0; i < _pauseBits.Length; i++)
		{
			_pauseBits[i].SetActive(true);
		}
	}

	private void OnPauseScreenConfirmComplete()
	{
		if (GlobalInGameData.ShouldLaunchHandsOfTimeCampaign())
		{
			GlobalInGameData.LaunchHandsOfTimeCampaign();
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenCutsceneHandsOfTimeIntro screenInstance = instance.GetScreenInstance<ScreenCutsceneHandsOfTimeIntro>();
			ScreenHud._pInstance.Navigate(screenInstance, false, false, onEndFlowCompleteFromPause);
		}
		else
		{
			ScreenFacade instance2 = Facades<ScreenFacade>.Instance;
			ScreenMap screenInstance2 = instance2.GetScreenInstance<ScreenMap>();
			ScreenHud._pInstance.Navigate(screenInstance2, false, false, onEndFlowCompleteFromPause);
		}
	}

	private void PopulateCollectionWidgets(int numElements, int numCollected, UIWidget[] pool)
	{
		Debug.Log("Num Collected: " + numCollected + "/" + numElements);
		for (int i = 0; i < numElements; i++)
		{
			_collectionWidgets[_dummyInt] = pool[i];
			_collectionWidgets[_dummyInt].enabled = true;
			_collectionWidgets[_dummyInt].alpha = ((i >= numCollected) ? 0.25f : 1f);
			_dummyInt++;
		}
	}

	private void RefreshSoundIcon()
	{
		_soundIcon.spriteName = ((!GameController.Instance.PlayerProfile.SoundFXDisabled || !GameController.Instance.PlayerProfile.MusicDisabled) ? "SoundON_icon" : "SoundOFF_icon");
	}
}
