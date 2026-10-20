using AmuzoEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenResultsExp : ScreenBase
{
	private static int _studsAcquired;

	public UITexture _playerTexture;

	public UILabel _studCounter;

	public UILabel _levelCounter;

	public Transform _xpBarStart;

	public Transform _xpBarCurrent;

	public Transform _xpBarEnd;

	public GameObject _studPiece;

	public GameObject _vehiclePiece;

	public GameObject _clothingPiece;

	private int _displayedLevel;

	private int _startingStuds;

	private float _startingTime;

	private bool _completed;

	private float _sfxTime = 0.1f;

	private float _sfxTimer;

	private bool _doOnce;

	public static int _pStudsAcquired
	{
		get
		{
			return _studsAcquired;
		}
		set
		{
			_studsAcquired = value;
		}
	}

	protected override void OnScreenExitComplete()
	{
		_playerTexture.mainTexture = null;
		ScreenHud._pInstance.ClearPlayerTexture();
		base.OnScreenExitComplete();
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
	}

	protected override void OnShowScreen()
	{
		_doOnce = false;
		_vehiclePiece.SetActive(false);
		_clothingPiece.SetActive(false);
		_sfxTimer = 0f;
		_startingStuds = GameController.Instance.PlayerProfile.Studs - _pStudsAcquired;
		_displayedLevel = PlayerProfile.LevelForXP(_startingStuds);
		_levelCounter.text = _displayedLevel.ToString();
		int startingStuds = _startingStuds;
		int num = PlayerProfile.xpForLevel(_displayedLevel);
		int num2 = PlayerProfile.xpForLevel(_displayedLevel + 1);
		if (num2 == 0)
		{
			_xpBarCurrent.localPosition = _xpBarEnd.localPosition;
		}
		else
		{
			float t = Mathf.InverseLerp(num, num2, startingStuds);
			_xpBarCurrent.localPosition = Vector3.Lerp(_xpBarStart.localPosition, _xpBarEnd.localPosition, t);
		}
		_startingTime = RealTime.time;
		_studCounter.text = _pStudsAcquired.ToString();
		_completed = false;
		base.OnShowScreen();
	}

	public void OnContinue()
	{
		_clothingPiece.SetActive(false);
		_vehiclePiece.SetActive(false);
		Level level = GamePlayer.Instance.Level;
		AudioPlayer.StopSound(level.LevelStyle.AmbienceAudioEventName);
		AudioPlayer.StopSound(level.LevelStyle.MusicAudioEventName);
		AudioPlayer.PlaySound("ui/button/sword/shing");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		SceneManager.LoadScene("Empty");
		if (GameController.Instance.ObjectiveController.CurrentObjective != null)
		{
			int num = GameController.Instance.ObjectiveController.CurrentObjective.lines.Length;
			for (int i = 0; i < num; i++)
			{
				ObjectiveLine objectiveLine = GameController.Instance.ObjectiveController.CurrentObjective.lines[i];
				Debug.Log("<color=blue>Objective Seed: " + objectiveLine.seed + " Chapter:" + objectiveLine.chapter + " Season:" + objectiveLine.season + " Completed:" + objectiveLine.Completed + "</color>");
				if (!objectiveLine.Completed)
				{
					continue;
				}
				string text = "Season_" + objectiveLine.season + "_chapter_" + objectiveLine.chapter;
				if (objectiveLine.season == 1)
				{
					switch (objectiveLine.chapter)
					{
					case 1:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel1>());
						return;
					case 2:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel2>());
						return;
					case 3:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel3>());
						return;
					case 4:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel4>());
						return;
					case 5:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel5>());
						return;
					case 6:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel6>());
						return;
					case 7:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel7>());
						return;
					case 8:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel8>());
						return;
					case 9:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel9>());
						return;
					case 10:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel10>());
						return;
					case 11:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel11>());
						return;
					case 12:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel12>());
						return;
					case 13:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel13>());
						return;
					case 14:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel14>());
						return;
					case 15:
						Navigate(instance.GetScreenInstance<ScreenCutsceneLevel15>());
						return;
					}
				}
				else if (objectiveLine.season == 2)
				{
					switch (objectiveLine.chapter)
					{
					case 1:
						Navigate(instance.GetScreenInstance<ScreenCutsceneBlade1>());
						return;
					case 2:
						Navigate(instance.GetScreenInstance<ScreenCutsceneBlade2>());
						return;
					case 3:
						Navigate(instance.GetScreenInstance<ScreenCutsceneBlade3>());
						return;
					case 4:
						Navigate(instance.GetScreenInstance<ScreenCutsceneBlade4>());
						return;
					case 5:
						Navigate(instance.GetScreenInstance<ScreenCutsceneHandsOfTimeFinal>());
						return;
					}
				}
				else if (objectiveLine.season == 3)
				{
					int chapter = objectiveLine.chapter;
					if (chapter == 6)
					{
						Navigate(instance.GetScreenInstance<ScreenCutsceneCityFinal>());
						return;
					}
				}
			}
		}
		GameController.Instance.SetState(GameController.Instance.GetComponent<GameControllerStateIdle>());
		if (GameController.Instance.ObjectiveController.CheckIfNextObjectiveOnNewMap())
		{
			ScreenWorldMap screenInstance = instance.GetScreenInstance<ScreenWorldMap>();
			Navigate(screenInstance);
		}
		else
		{
			ScreenMap screenInstance2 = instance.GetScreenInstance<ScreenMap>();
			Navigate(screenInstance2);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!_completed)
		{
			float num = RealTime.time - _startingTime;
			if (num > 1.5f && num < 2.5f)
			{
				float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.Linear, num - 1f, 1f, 0f, 1f);
				float num2 = Mathf.Lerp(_pStudsAcquired, 0f, t);
				int num3 = _startingStuds + (_pStudsAcquired - (int)num2);
				int num4 = PlayerProfile.xpForLevel(_displayedLevel);
				int num5 = PlayerProfile.xpForLevel(_displayedLevel + 1);
				_sfxTimer += Time.deltaTime;
				if (_sfxTimer > _sfxTime && num2 > 0f)
				{
					AudioPlayer.PlaySound("ui/celebration/countup/tick");
					_sfxTimer = 0f;
				}
				if (num5 == 0)
				{
					_xpBarCurrent.localPosition = _xpBarEnd.localPosition;
				}
				else
				{
					float t2 = Mathf.InverseLerp(num4, num5, num3);
					_xpBarCurrent.localPosition = Vector3.Lerp(_xpBarStart.localPosition, _xpBarEnd.localPosition, t2);
				}
				_displayedLevel = PlayerProfile.LevelForXP(num3);
				_levelCounter.text = _displayedLevel.ToString();
				_studCounter.text = Mathf.RoundToInt(num2).ToString();
			}
			else if (num >= 2.5f)
			{
				_studCounter.text = "0";
				_completed = true;
				int num6 = _startingStuds + _pStudsAcquired;
				int num7 = PlayerProfile.xpForLevel(_displayedLevel);
				int num8 = PlayerProfile.xpForLevel(_displayedLevel + 1);
				if (num8 == 0)
				{
					_xpBarCurrent.localPosition = _xpBarEnd.localPosition;
				}
				else
				{
					float t3 = Mathf.InverseLerp(num7, num8, num6);
					_xpBarCurrent.localPosition = Vector3.Lerp(_xpBarStart.localPosition, _xpBarEnd.localPosition, t3);
				}
				_displayedLevel = PlayerProfile.LevelForXP(num6);
				_levelCounter.text = _displayedLevel.ToString();
			}
		}
		else if (!_doOnce)
		{
			_doOnce = true;
			if (GameController.Instance._collectedVehiclePart)
			{
				_studPiece.SetActive(false);
				_vehiclePiece.SetActive(true);
				AudioPlayer.PlaySound("ui/panel/impact");
			}
			else if (GameController.Instance._collectedClothingPart)
			{
				_studPiece.SetActive(false);
				_clothingPiece.SetActive(true);
				AudioPlayer.PlaySound("ui/panel/impact");
			}
		}
	}
}
