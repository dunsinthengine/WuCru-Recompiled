using System;
using System.Collections;
using Fabric;
using UnityEngine;
using UnityEngine.UI;

public class MissionStarGUI : MonoBehaviour
{
	public delegate void OnDoneTransferToCounterDelegate();

	public Image imageStar;

	public GameObject studPrefab;

	public Text textHeadline;

	public Text textHeadline2;

	public Text textHeadline3;

	public Text textInstructions;

	public ScreenFlash screenFlash;

	public Image imageCompletion;

	public Image imageObjects;

	public Image imageChallenges;

	public Image imageStarOverlay;

	public GameObject starOverlayParent;

	private float initPosVariance = 80f;

	private float endPosVariance = 20f;

	private int numCompletionStuds = 50;

	private float studDelay = 0.05f;

	private int numStudsReachedCounter;

	private StudsCounter studsCounter;

	public OnDoneTransferToCounterDelegate OnDoneTransferToCounter;

	private bool isCollecting;

	private bool skipStep;

	public float studPitchIncrease;

	private void Awake()
	{
		imageStar.enabled = false;
		textHeadline.gameObject.SetActive(false);
		imageStarOverlay.gameObject.SetActive(false);
		textInstructions.enabled = false;
		studsCounter = UnityEngine.Object.FindObjectOfType<StudsCounter>();
		screenFlash = UnityEngine.Object.FindObjectOfType<ScreenFlash>();
		StartCoroutine(CollectStuds());
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			skipStep = true;
		}
	}

	private IEnumerator CollectStuds()
	{
		int numTotalChests = GameController.Instance.MissionStats.NumChestsWoodToCollect;
		numTotalChests += GameController.Instance.MissionStats.NumChestsSilverToCollect;
		numTotalChests += GameController.Instance.MissionStats.NumChestsGoldToCollect;
		int numTotalCrystals = GameController.Instance.MissionStats.NumCrystalsToCollect;
		numTotalCrystals += GameController.Instance.MissionStats.NumCrystalsEarthToCollect;
		numTotalCrystals += GameController.Instance.MissionStats.NumCrystalsFireToCollect;
		numTotalCrystals += GameController.Instance.MissionStats.NumCrystalsIceToCollect;
		numTotalCrystals += GameController.Instance.MissionStats.NumCrystalsElectricityToCollect;
		numTotalCrystals += GameController.Instance.MissionStats.NumCrystalsEnergyToCollect;
		numTotalCrystals += GameController.Instance.MissionStats.NumCrystalsWaterToCollect;
		int numTotalObjects = numTotalCrystals + numTotalChests;
		bool wonObjects = numTotalObjects <= GameController.Instance.MissionStats.NumChestsCollected + GameController.Instance.MissionStats.NumCrystalsCollectedAllTypes;
		bool wonChallenges = true;
		foreach (LevelChallengeBase key in GameController.Instance.MissionStats.Challenges.Keys)
		{
			if (!GameController.Instance.MissionStats.Challenges[key])
			{
				wonChallenges = false;
				Debug.Log("MissionStarGUI: Did not win all challenges. Challenge '" + key.GetType().ToString() + "' not completed");
			}
		}
		if (wonChallenges)
		{
			Debug.Log("MissionStarGUI: All challenges won!");
		}
		LevelUnlockData ud = null;
		if (GameController.Instance.PlayerProfile.LevelUnlocked.ContainsKey(GameController.Instance.CurrentLevel.Seed))
		{
			ud = GameController.Instance.PlayerProfile.LevelUnlocked[GameController.Instance.CurrentLevel.Seed];
		}
		if (ud == null)
		{
			ud = new LevelUnlockData();
		}
		if (!ud.starCompletion)
		{
			numCompletionStuds = 50;
			numStudsReachedCounter = 0;
			skipStep = false;
			StartCoroutine(DoCollectStuds(MissionRewardType.Completion));
			while (isCollecting)
			{
				yield return 0;
			}
		}
		if (!ud.starObjects && wonObjects)
		{
			numCompletionStuds = 50;
			numStudsReachedCounter = 0;
			skipStep = false;
			StartCoroutine(DoCollectStuds(MissionRewardType.Objects));
			while (isCollecting)
			{
				yield return 0;
			}
		}
		if (!ud.starChallenges && wonChallenges)
		{
			numCompletionStuds = 50;
			numStudsReachedCounter = 0;
			skipStep = false;
			StartCoroutine(DoCollectStuds(MissionRewardType.Challenges));
			while (isCollecting)
			{
				yield return 0;
			}
		}
		yield return 0;
		textInstructions.enabled = true;
		if (OnDoneTransferToCounter != null)
		{
			OnDoneTransferToCounter();
		}
	}

	public Vector3 GetCenterPosition()
	{
		return imageStar.transform.localPosition;
	}

	private IEnumerator DoCollectStuds(MissionRewardType rewardType)
	{
		isCollecting = true;
		imageStar.enabled = true;
		switch (rewardType)
		{
		case MissionRewardType.Completion:
			textHeadline.gameObject.SetActive(true);
			textHeadline2.gameObject.SetActive(false);
			textHeadline3.gameObject.SetActive(false);
			imageCompletion.gameObject.SetActive(true);
			imageObjects.gameObject.SetActive(false);
			imageChallenges.gameObject.SetActive(false);
			break;
		case MissionRewardType.Objects:
			textHeadline.gameObject.SetActive(false);
			textHeadline2.gameObject.SetActive(true);
			textHeadline3.gameObject.SetActive(false);
			imageCompletion.gameObject.SetActive(false);
			imageObjects.gameObject.SetActive(true);
			imageChallenges.gameObject.SetActive(false);
			break;
		case MissionRewardType.Challenges:
			textHeadline.gameObject.SetActive(false);
			textHeadline2.gameObject.SetActive(false);
			textHeadline3.gameObject.SetActive(true);
			imageCompletion.gameObject.SetActive(false);
			imageObjects.gameObject.SetActive(false);
			imageChallenges.gameObject.SetActive(true);
			break;
		}
		float entranceDuration = 0.5f;
		float startTime = Time.time;
		Vector3 targetPos = base.gameObject.transform.position;
		Vector3 fromPos = targetPos + new Vector3(0f, 500f, 0f);
		imageStarOverlay.gameObject.SetActive(true);
		starOverlayParent.GetComponent<Animator>().SetTrigger("RotateAndScale");
		while (Time.time - startTime < entranceDuration)
		{
			yield return 0;
		}
		screenFlash.FlashAlpha(1f, 2.5f, Color.yellow);
		EventManager.Instance.PostEvent("ui/button/sword/shing");
		base.gameObject.transform.position = targetPos;
		if (studsCounter != null)
		{
			for (int i = 0; i < numCompletionStuds; i++)
			{
				GameObject studGO = UnityEngine.Object.Instantiate(studPrefab);
				StudsForGUI stud = studGO.GetComponent<StudsForGUI>();
				Vector3 from = GetCenterPosition() + new Vector3(UnityEngine.Random.Range(0f - initPosVariance, initPosVariance), UnityEngine.Random.Range(0f - initPosVariance, initPosVariance), 0f);
				Vector3 to = studsCounter.GetCenterPosition() + new Vector3(UnityEngine.Random.Range(0f - endPosVariance, endPosVariance), UnityEngine.Random.Range(0f - endPosVariance, endPosVariance), 0f);
				float duration = UnityEngine.Random.Range(0.08f, 0.28f);
				stud.Fly(from, to, duration);
				stud.OnReachTarget = (StudsForGUI.OnReachTargetDelegate)Delegate.Combine(stud.OnReachTarget, new StudsForGUI.OnReachTargetDelegate(OnStudReachTarget));
				if (!skipStep)
				{
					yield return new WaitForSeconds(studDelay);
				}
			}
		}
		else
		{
			Debug.LogError("StudsCounter is null - cannot transfer completion-studs to counter");
		}
		yield return 0;
	}

	public void OnStudReachTarget()
	{
		studsCounter.Pickup();
		numStudsReachedCounter++;
		studPitchIncrease = (float)numStudsReachedCounter / (float)numCompletionStuds;
		EventManager.Instance.PostEvent("props/stud/pickup", EventAction.SetPitchProperty, 1f + studPitchIncrease);
		AudioPlayer.PlaySound("props/stud/pickup");
		if (numStudsReachedCounter >= numCompletionStuds)
		{
			isCollecting = false;
		}
	}
}
