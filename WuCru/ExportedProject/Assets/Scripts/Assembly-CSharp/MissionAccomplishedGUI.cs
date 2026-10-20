using System.Collections;
using Fabric;
using UnityEngine;
using UnityEngine.UI;

public class MissionAccomplishedGUI : MonoBehaviour
{
	public delegate void OnDoneDelegate();

	public CompletionStarContainerGUI star1;

	public CompletionStarContainerGUI star2;

	public CompletionStarContainerGUI star3;

	public Text textCompletion;

	public Text textObjects;

	public Text textChallenges;

	public Image imageCompletion;

	public Image bgField1;

	public Image bgField2;

	public Image bgField3;

	public Button buttonOK;

	public Text textInstructions;

	public CompletionChallengeContainer[] challengeContainers;

	public GameObject collectionContainerGrid;

	private bool star1Given;

	private bool star2Given;

	private bool star3Given;

	private bool listingObjects;

	private bool objectsListed;

	private bool challengesListed;

	private bool listingChallenges;

	private bool onDoneFired;

	private float minorStepDelay = 0.32f;

	private float microStepDelay = 0.08f;

	public OnDoneDelegate OnDone;

	private Material failedMaterial;

	private bool skipStep;

	private Vector2 _tweenStartPosition;

	private Vector2 _tweenEndPosition;

	private RectTransform _rectTransform;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_tweenEndPosition = _rectTransform.anchoredPosition;
		_tweenStartPosition = _tweenEndPosition + new Vector2(0f, 570f);
		_rectTransform.anchoredPosition = _tweenStartPosition;
		failedMaterial = new Material(imageCompletion.material);
		failedMaterial.color = new Color(1f, 1f, 1f, 0.3f);
		textCompletion.gameObject.SetActive(false);
		textObjects.gameObject.SetActive(false);
		textChallenges.gameObject.SetActive(false);
		bgField1.gameObject.SetActive(false);
		bgField2.gameObject.SetActive(false);
		bgField3.gameObject.SetActive(false);
		imageCompletion.enabled = false;
		buttonOK.gameObject.SetActive(false);
		textInstructions.enabled = false;
		for (int i = 0; i < challengeContainers.Length; i++)
		{
			challengeContainers[i].gameObject.SetActive(false);
		}
		skipStep = false;
		StartCoroutine(DoGiveCompletion());
	}

	private void Update()
	{
		if (star1Given && !listingObjects && !objectsListed && !star2Given && !star3Given)
		{
			ListObjects();
		}
		if (star2Given && !star3Given && !challengesListed && !listingChallenges)
		{
			ListChallenges();
		}
		if (star3Given && !onDoneFired)
		{
			buttonOK.gameObject.SetActive(true);
			textInstructions.enabled = true;
			onDoneFired = true;
			if (OnDone != null)
			{
				OnDone();
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			skipStep = true;
		}
	}

	public void GiveStar(int num, bool completed)
	{
		StartCoroutine(DoGiveStar(num, completed));
	}

	public void ListObjects()
	{
		StartCoroutine(DoListObjects());
	}

	public void ListChallenges()
	{
		StartCoroutine(DoListChallenges());
	}

	private IEnumerator DoGiveStar(int num, bool completed)
	{
		CompletionStarContainerGUI element = null;
		switch (num)
		{
		case 1:
			element = star1;
			break;
		case 2:
			element = star2;
			break;
		case 3:
			element = star3;
			break;
		default:
			Debug.LogError("Index out of bounds - cannot give unknown star to player/level");
			break;
		}
		if (element != null)
		{
			Vector3 targetPos = element.root.transform.localPosition;
			Vector3 startPos = targetPos;
			float startTime = Time.time;
			float duration = 0.2f;
			element.GiveStar(completed);
			if (!completed)
			{
				element.imageStar.material = failedMaterial;
			}
			if (!skipStep)
			{
				if (completed)
				{
					element.root.GetComponentInChildren<Animator>().SetTrigger("RotateAndScale");
				}
				while (Time.time - startTime < duration)
				{
					float t = (Time.time - startTime) / duration;
					yield return 0;
				}
			}
			element.root.transform.localPosition = targetPos;
			if (completed)
			{
				EventManager.Instance.PostEvent("ui/button/sword/shing");
			}
			else
			{
				EventManager.Instance.PostEvent("player/attack/fail", GamePlayer.Instance.Player.Character.gameObject);
			}
		}
		else
		{
			Debug.LogError("DoGiveStar...element was null!");
		}
		if (num == 1)
		{
			star1Given = true;
		}
		if (num == 2)
		{
			star2Given = true;
		}
		if (num == 3)
		{
			star3Given = true;
		}
		yield return 0;
	}

	private IEnumerator DoGiveCompletion()
	{
		float entranceDuration = 0.5f;
		float startTime = Time.time;
		if (!skipStep)
		{
			AudioPlayer.PlaySound("ui/panel/in");
			while (Time.time - startTime < entranceDuration)
			{
				float t = (Time.time - startTime) / entranceDuration;
				_rectTransform.anchoredPosition = Vector3.Lerp(_tweenStartPosition, _tweenEndPosition, t);
				yield return 0;
			}
		}
		_rectTransform.anchoredPosition = _tweenEndPosition;
		AudioPlayer.StopSound("ui/panel/in");
		AudioPlayer.PlaySound("ui/panel/impact");
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		textCompletion.gameObject.SetActive(true);
		bgField1.gameObject.SetActive(true);
		AudioPlayer.PlaySound("fx/story/text/appears");
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		imageCompletion.enabled = true;
		AudioPlayer.PlaySound("fx/story/text/appears");
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		GiveStar(1, true);
		yield return 0;
	}

	private IEnumerator DoListObjects()
	{
		listingObjects = true;
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
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		bgField2.gameObject.SetActive(true);
		textObjects.gameObject.SetActive(true);
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		for (int i = 0; i < GameController.Instance.MissionStats.NumCrystalsToCollect; i++)
		{
			GameObject completionObjectGO = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog = completionObjectGO.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO.transform.SetParent(collectionContainerGrid.transform);
			if (i + 1 <= GameController.Instance.MissionStats.NumCrystalsCollected)
			{
				cog.SetCompleted(CompletionObjectType.Crystal, true);
			}
			else
			{
				cog.SetCompleted(CompletionObjectType.Crystal, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int j = 0; j < GameController.Instance.MissionStats.NumCrystalsEarthToCollect; j++)
		{
			GameObject completionObjectGO2 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog2 = completionObjectGO2.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO2.transform.SetParent(collectionContainerGrid.transform);
			if (j + 1 <= GameController.Instance.MissionStats.NumCrystalsEarthCollected)
			{
				cog2.SetCompleted(CompletionObjectType.CrystalEarth, true);
			}
			else
			{
				cog2.SetCompleted(CompletionObjectType.CrystalEarth, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int k = 0; k < GameController.Instance.MissionStats.NumCrystalsFireToCollect; k++)
		{
			GameObject completionObjectGO3 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog3 = completionObjectGO3.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO3.transform.SetParent(collectionContainerGrid.transform);
			if (k + 1 <= GameController.Instance.MissionStats.NumCrystalsFireCollected)
			{
				cog3.SetCompleted(CompletionObjectType.CrystalFire, true);
			}
			else
			{
				cog3.SetCompleted(CompletionObjectType.CrystalFire, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int l = 0; l < GameController.Instance.MissionStats.NumCrystalsIceToCollect; l++)
		{
			GameObject completionObjectGO4 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog4 = completionObjectGO4.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO4.transform.SetParent(collectionContainerGrid.transform);
			if (l + 1 <= GameController.Instance.MissionStats.NumCrystalsIceCollected)
			{
				cog4.SetCompleted(CompletionObjectType.CrystalIce, true);
			}
			else
			{
				cog4.SetCompleted(CompletionObjectType.CrystalIce, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int m = 0; m < GameController.Instance.MissionStats.NumCrystalsElectricityToCollect; m++)
		{
			GameObject completionObjectGO5 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog5 = completionObjectGO5.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO5.transform.SetParent(collectionContainerGrid.transform);
			if (m + 1 <= GameController.Instance.MissionStats.NumCrystalsElectricityCollected)
			{
				cog5.SetCompleted(CompletionObjectType.CrystalElectricity, true);
			}
			else
			{
				cog5.SetCompleted(CompletionObjectType.CrystalElectricity, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int n = 0; n < GameController.Instance.MissionStats.NumCrystalsEnergyToCollect; n++)
		{
			GameObject completionObjectGO6 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog6 = completionObjectGO6.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO6.transform.SetParent(collectionContainerGrid.transform);
			if (n + 1 <= GameController.Instance.MissionStats.NumCrystalsEnergyCollected)
			{
				cog6.SetCompleted(CompletionObjectType.CrystalEnergy, true);
			}
			else
			{
				cog6.SetCompleted(CompletionObjectType.CrystalEnergy, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int num = 0; num < GameController.Instance.MissionStats.NumCrystalsWaterToCollect; num++)
		{
			GameObject completionObjectGO7 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog7 = completionObjectGO7.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO7.transform.SetParent(collectionContainerGrid.transform);
			if (num + 1 <= GameController.Instance.MissionStats.NumCrystalsWaterCollected)
			{
				cog7.SetCompleted(CompletionObjectType.CrystalWater, true);
			}
			else
			{
				cog7.SetCompleted(CompletionObjectType.CrystalWater, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		int numChestsWood = GameController.Instance.MissionStats.NumChestsWoodToCollect;
		int numChestsSilver = GameController.Instance.MissionStats.NumChestsSilverToCollect;
		int numChestsGold = GameController.Instance.MissionStats.NumChestsGoldToCollect;
		for (int num2 = 0; num2 < numChestsWood; num2++)
		{
			GameObject completionObjectGO8 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog8 = completionObjectGO8.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO8.transform.SetParent(collectionContainerGrid.transform);
			if (num2 + 1 <= GameController.Instance.MissionStats.NumChestsWoodCollected)
			{
				cog8.SetCompleted(CompletionObjectType.ChestWood, true);
			}
			else
			{
				cog8.SetCompleted(CompletionObjectType.ChestWood, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int num3 = 0; num3 < numChestsSilver; num3++)
		{
			GameObject completionObjectGO9 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog9 = completionObjectGO9.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO9.transform.SetParent(collectionContainerGrid.transform);
			if (num3 + 1 <= GameController.Instance.MissionStats.NumChestsSilverCollected)
			{
				cog9.SetCompleted(CompletionObjectType.ChestSilver, true);
			}
			else
			{
				cog9.SetCompleted(CompletionObjectType.ChestSilver, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		for (int num4 = 0; num4 < numChestsGold; num4++)
		{
			GameObject completionObjectGO10 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI cog10 = completionObjectGO10.GetComponent<CompletionObjectContainerGUI>();
			completionObjectGO10.transform.SetParent(collectionContainerGrid.transform);
			if (num4 + 1 <= GameController.Instance.MissionStats.NumChestsGoldCollected)
			{
				cog10.SetCompleted(CompletionObjectType.ChestGold, true);
			}
			else
			{
				cog10.SetCompleted(CompletionObjectType.ChestGold, false);
			}
			AudioPlayer.PlaySound("fx/story/text/appears");
			if (!skipStep)
			{
				yield return new WaitForSeconds(microStepDelay);
			}
		}
		yield return 0;
		objectsListed = true;
		listingObjects = false;
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		GiveStar(2, numTotalObjects <= GameController.Instance.MissionStats.NumChestsCollected + GameController.Instance.MissionStats.NumCrystalsCollectedAllTypes);
	}

	private IEnumerator DoListChallenges()
	{
		listingChallenges = true;
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		bgField3.gameObject.SetActive(true);
		textChallenges.gameObject.SetActive(true);
		if (!skipStep)
		{
			yield return new WaitForSeconds(minorStepDelay);
		}
		int i = 0;
		foreach (LevelChallengeBase c in GameController.Instance.MissionStats.Challenges.Keys)
		{
			challengeContainers[i].gameObject.SetActive(true);
			challengeContainers[i].imageChallenge.sprite = Resources.Load<Sprite>(c.IconPath());
			if (GameController.Instance.MissionStats.Challenges[c])
			{
				challengeContainers[i].imageCheckmark.gameObject.SetActive(true);
				challengeContainers[i].imageFailed.gameObject.SetActive(false);
			}
			else
			{
				challengeContainers[i].imageCheckmark.gameObject.SetActive(false);
				challengeContainers[i].imageFailed.gameObject.SetActive(true);
			}
			if (!skipStep)
			{
				yield return new WaitForSeconds(minorStepDelay);
			}
			i++;
		}
		int completed = 0;
		foreach (LevelChallengeBase key in GameController.Instance.MissionStats.Challenges.Keys)
		{
			if (GameController.Instance.MissionStats.Challenges[key])
			{
				completed++;
			}
		}
		GiveStar(3, completed == GameController.Instance.MissionStats.Challenges.Count);
		Debug.Log("Challenges: completed " + completed + " of " + GameController.Instance.MissionStats.Challenges.Count);
		challengesListed = true;
		listingChallenges = false;
		yield return 0;
	}
}
