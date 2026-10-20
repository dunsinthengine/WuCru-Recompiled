using System;
using System.Collections;
using Fabric;
using UnityEngine;
using UnityEngine.UI;

public class MissionXPGUI : MonoBehaviour
{
	public delegate void OnDoneTransferToXPDelegate();

	public GameObject studPrefab;

	public Image imageUpgradeBarFill;

	public Text textLevel;

	public Text textLevelUp;

	public Image imageFrame;

	public ScreenFlash screenFlash;

	public Text textInstructions;

	public RawImage characterImage;

	public Camera characterCamera;

	private float initPosVariance = 20f;

	private float endPosVariance = 80f;

	private float studDelay = 0.025f;

	private int numStudsReachedXPBar;

	private StudsCounter studsCounter;

	private float progression;

	private int studsToRemove;

	private int currentLevel;

	public OnDoneTransferToXPDelegate OnDoneTransferToXP;

	public float studPitchIncrease;

	private bool skipStep;

	private bool levelingUp;

	private Vector2 _tweenStartPosition;

	private Vector2 _tweenEndPosition;

	private RectTransform _rectTransform;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_tweenEndPosition = _rectTransform.anchoredPosition;
		_tweenStartPosition = _tweenEndPosition + new Vector2(0f, 510f);
		_rectTransform.anchoredPosition = _tweenStartPosition;
		screenFlash = UnityEngine.Object.FindObjectOfType<ScreenFlash>();
		textInstructions.enabled = false;
		currentLevel = PlayerProfile.LevelForXP(GameController.Instance.PlayerProfile.Studs);
		int num = currentLevel + 1;
		textLevel.text = currentLevel.ToString();
		textLevelUp.enabled = false;
		RenderTexture texture = CharacterFullbodyRenderer.Generate(0, (int)characterImage.rectTransform.sizeDelta.x, (int)characterImage.rectTransform.sizeDelta.y);
		characterImage.texture = texture;
		StartCoroutine(DoCollectStuds());
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			skipStep = true;
		}
	}

	public void OnDestroy()
	{
		RenderTexture renderTexture = characterImage.texture as RenderTexture;
		if ((bool)renderTexture)
		{
			renderTexture.Release();
		}
	}

	private void FillBar()
	{
		int num = PlayerProfile.xpForLevel(currentLevel);
		int num2 = PlayerProfile.xpForLevel(currentLevel + 1);
		int num3 = GameController.Instance.PlayerProfile.Studs + numStudsReachedXPBar - num;
		progression = (float)num3 / ((float)num2 - (float)num);
		imageUpgradeBarFill.fillAmount = progression;
	}

	public Vector3 GetCenterPosition()
	{
		return imageFrame.transform.localPosition;
	}

	private IEnumerator DoCollectStuds()
	{
		FillBar();
		yield return 0;
		float entranceDuration = 0.5f;
		float startTime = Time.time;
		while (Time.time - startTime < entranceDuration)
		{
			AudioPlayer.PlaySound("ui/panel/in");
			float t = (Time.time - startTime) / entranceDuration;
			_rectTransform.anchoredPosition = Vector3.Lerp(_tweenStartPosition, _tweenEndPosition, t);
			yield return null;
		}
		_rectTransform.anchoredPosition = _tweenEndPosition;
		AudioPlayer.StopSound("ui/panel/in");
		AudioPlayer.PlaySound("ui/panel/impact");
		if (studsToRemove <= 0)
		{
			Debug.Log("StudsToRemove" + studsToRemove);
			SaveState();
			if (OnDoneTransferToXP != null)
			{
				OnDoneTransferToXP();
			}
		}
		else if (studsCounter != null)
		{
			int spawnedStuds = 0;
			while (spawnedStuds < studsToRemove)
			{
				int studsFrom = GameController.Instance.PlayerProfile.Studs + spawnedStuds;
				int studsTo = PlayerProfile.xpForLevel(currentLevel + 1);
				int studsToNextLevel = studsTo - studsFrom;
				int studsToSpawn = Mathf.Min(studsToNextLevel, studsToRemove - spawnedStuds);
				for (int i = 0; i < studsToSpawn; i++)
				{
					GameObject studGO = UnityEngine.Object.Instantiate(studPrefab);
					StudsForGUI stud = studGO.GetComponent<StudsForGUI>();
					Vector3 from = studsCounter.GetCenterPosition() + new Vector3(UnityEngine.Random.Range(0f - initPosVariance, initPosVariance), UnityEngine.Random.Range(0f - initPosVariance, initPosVariance), 0f);
					Vector3 to = GetCenterPosition() + new Vector3(UnityEngine.Random.Range(0f - endPosVariance, endPosVariance), UnityEngine.Random.Range(0f - endPosVariance, endPosVariance), 0f);
					float duration = UnityEngine.Random.Range(0.06f, 0.18f);
					stud.Fly(from, to, duration);
					studsCounter.Remove();
					stud.OnReachTarget = (StudsForGUI.OnReachTargetDelegate)Delegate.Combine(stud.OnReachTarget, new StudsForGUI.OnReachTargetDelegate(OnStudReachTarget));
					if (!skipStep)
					{
						yield return new WaitForSeconds(studDelay);
					}
				}
				spawnedStuds += studsToSpawn;
				if (studsToSpawn >= studsToNextLevel)
				{
					Debug.Log("waiting for level up to start");
					while (!levelingUp)
					{
						yield return null;
					}
					Debug.Log("waiting for level up to end");
					while (levelingUp)
					{
						yield return null;
					}
				}
				yield return null;
			}
			textInstructions.enabled = true;
		}
		else
		{
			Debug.LogError("StudsCounter is null - cannot transfer studs to xp");
		}
	}

	public void LevelUp()
	{
		int level = currentLevel + 1;
		Debug.Log("DoLevelUp");
		AnalyticsEvents.LogLevelUp(level);
		GameController.Instance.PlayerProfile.Level++;
		GameController.Instance.PlayerProfile.Save();
		NinjaScreenController.OpeningPage = NinjaScreenOpeningPage.NinjaStats;
		StartCoroutine(DoLevelUp());
	}

	private IEnumerator DoLevelUp()
	{
		levelingUp = true;
		float scaleDuration = ((!skipStep) ? 1.2f : 0.1f);
		float startTime = Time.time;
		currentLevel++;
		textLevel.text = currentLevel.ToString();
		AudioPlayer.PlaySound("music/level/up");
		textLevelUp.enabled = true;
		Debug.Log("start time: " + startTime + " duration " + scaleDuration);
		screenFlash.FlashAlpha(1f, 2.5f, Color.white);
		while (Time.time - startTime < scaleDuration)
		{
			float t = (Time.time - startTime) / scaleDuration;
			float scaleValue = 1.8f;
			textLevel.transform.localScale = Vector3.Lerp(new Vector3(scaleValue, scaleValue, scaleValue), Vector3.one, t);
			textLevelUp.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, 1f - t);
			yield return 0;
		}
		textLevelUp.enabled = false;
		textLevelUp.transform.localScale = Vector3.one;
		textLevel.transform.localScale = Vector3.one;
		levelingUp = false;
	}

	public void OnStudReachTarget()
	{
		numStudsReachedXPBar++;
		FillBar();
		studPitchIncrease = (float)numStudsReachedXPBar / (float)studsToRemove;
		EventManager.Instance.PostEvent("props/stud/pickup", EventAction.SetPitchProperty, 1f + studPitchIncrease);
		AudioPlayer.PlaySound("props/stud/pickup");
		if (progression >= 1f && !levelingUp)
		{
			LevelUp();
		}
		if (numStudsReachedXPBar >= studsToRemove)
		{
			SaveState();
			if (OnDoneTransferToXP != null)
			{
				OnDoneTransferToXP();
			}
		}
	}

	public void SaveState()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		playerProfile.Studs += studsToRemove;
		playerProfile.TotalLevelsPlayed++;
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Kai, 0)))
		{
			playerProfile.KaiLevel = Team.LevelForShards(playerProfile.KaiTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Jay, 0)))
		{
			playerProfile.JayLevel = Team.LevelForShards(playerProfile.JayTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Cole, 0)))
		{
			playerProfile.ColeLevel = Team.LevelForShards(playerProfile.ColeTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Zane, 0)))
		{
			playerProfile.ZaneLevel = Team.LevelForShards(playerProfile.ZaneTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Nya, 0)))
		{
			playerProfile.NyaLevel = Team.LevelForShards(playerProfile.NyaTokens);
		}
		if (playerProfile.UnlockedNinjaSKUs.Contains(Team.GetID(MinifigType.Lloyd, 0)))
		{
			playerProfile.LloydLevel = Team.LevelForShards(playerProfile.LloydTokens);
		}
		for (int i = 1; i < playerProfile.NinjaTeamSlotCount; i++)
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam < 0)
			{
				continue;
			}
			MinifigType typeForID = Team.GetTypeForID(ninjaInTeam);
			if (typeForID != MinifigType.None)
			{
				int ninjaLevel = playerProfile.GetNinjaLevel(typeForID);
				int num = Team.GetIDsForType(typeForID)[ninjaLevel];
				playerProfile.SetNinjaInTeam(num, i);
				if (!playerProfile.UnlockedNinjaSKUs.Contains(num))
				{
					playerProfile.UnlockedNinjaSKUs.Add(num);
				}
			}
		}
		bool starObjects = GameController.Instance.MissionStats.NumChestsToCollect - GameController.Instance.MissionStats.NumChestsCollected <= 0 && GameController.Instance.MissionStats.NumCrystalsToCollect - GameController.Instance.MissionStats.NumCrystalsCollected <= 0;
		bool starChallenges = true;
		foreach (LevelChallengeBase key in GameController.Instance.MissionStats.Challenges.Keys)
		{
			if (!GameController.Instance.MissionStats.Challenges[key])
			{
				starChallenges = false;
			}
		}
		LevelUnlockData levelUnlockData = new LevelUnlockData();
		levelUnlockData.seed = GameController.Instance.CurrentLevel.Seed;
		levelUnlockData.season = GameController.Instance.PlayerProfile.LastPlayedSeason;
		levelUnlockData.starCompletion = true;
		levelUnlockData.starObjects = starObjects;
		levelUnlockData.starChallenges = starChallenges;
		GameController.Instance.LevelExtensionController.HandleLevelComplete(levelUnlockData.seed);
		GameController.Instance.areaMapMetaLayer.UnlockAfterPlay(levelUnlockData);
	}
}
