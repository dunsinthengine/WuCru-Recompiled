using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NinjaStatsGUI : MonoBehaviour
{
	public RawImage ninjaImage;

	public Text levelText;

	public Button levelUpButton;

	public Image levelProgressBar;

	public GameObject levelUpFX;

	public Image levelUpSkillCoin;

	public ScreenFlash screenFlash;

	public Text starsText;

	public Text shardsText;

	public Text skillPointsText;

	public float progressBarFillSpeed = 0.3f;

	public GameObject[] elementalPowerEffects;

	private int totalStars;

	private List<Coroutine> activeCoroutines = new List<Coroutine>();

	private NinjaScreenController ninjaScreenController;

	private void Start()
	{
		UpdateNinjaImage();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		levelText.text = playerProfile.Level.ToString();
		ninjaScreenController = Object.FindObjectOfType<NinjaScreenController>();
		if (NinjaScreenController.OpeningPagePreReset == NinjaScreenOpeningPage.NinjaStats)
		{
			activeCoroutines.Add(StartCoroutine(LevelUp()));
		}
		totalStars = 0;
		foreach (LevelUnlockData value in playerProfile.LevelUnlocked.Values)
		{
			int num = (value.starCompletion ? 1 : 0);
			int num2 = (value.starChallenges ? 1 : 0);
			int num3 = (value.starObjects ? 1 : 0);
			int num4 = num + num2 + num3;
			totalStars += num4;
		}
		starsText.text = totalStars.ToString();
		shardsText.text = GameController.Instance.MissionStats.NumCrystalsCollected.ToString();
		skillPointsText.text = playerProfile.AvailableSkillPoints.ToString();
		levelProgressBar.material = new Material(levelProgressBar.material);
	}

	private void OnEnable()
	{
		Debug.Log("enable");
		SetLevelProgressBar();
		SetAvailableSkillPoints();
	}

	public void OnDestroy()
	{
		RenderTexture renderTexture = ninjaImage.texture as RenderTexture;
		if ((bool)renderTexture)
		{
			renderTexture.Release();
		}
	}

	public void UpdateNinjaImage()
	{
		RenderTexture texture = CharacterFullbodyRenderer.Generate(0, (int)ninjaImage.rectTransform.sizeDelta.x, (int)ninjaImage.rectTransform.sizeDelta.y);
		ninjaImage.texture = texture;
	}

	private void SetAvailableSkillPoints()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		skillPointsText.text = playerProfile.AvailableSkillPoints.ToString();
	}

	private void SetLevelProgressBar()
	{
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		int num = PlayerProfile.xpForLevel(playerProfile.Level);
		int num2 = PlayerProfile.xpForLevel(playerProfile.Level + 1);
		int num3 = playerProfile.Studs - num;
		float num4 = (float)num3 / (float)(num2 - num);
		if (num4 > 1f)
		{
			num4 = 1f;
		}
		foreach (Coroutine activeCoroutine in activeCoroutines)
		{
			StopCoroutine(activeCoroutine);
		}
		activeCoroutines.Add(StartCoroutine(FillProgressBar(num4)));
	}

	private IEnumerator FillProgressBar(float value)
	{
		yield return new WaitForSeconds(0.3f);
		float progression = 0f;
		float speed = 1f / progressBarFillSpeed;
		while (progression < value)
		{
			progression += speed * Time.deltaTime;
			levelProgressBar.fillAmount = progression;
			yield return null;
		}
	}

	private IEnumerator LevelUp()
	{
		Debug.Log("Start LevelUp Sequence");
		Object.FindObjectOfType<ScreenFlash>();
		PlayerProfile profile = GameController.Instance.PlayerProfile;
		Animator animator = ninjaScreenController.menuCharacter.GetComponent<Animator>();
		animator.SetTrigger(TeahouseTriggers.power);
		GameObject fxGO = Object.Instantiate(elementalPowerEffects[(int)(GameController.Instance.PlayerProfile.GetMinifigDefinition().elementalPower - 1)]);
		fxGO.transform.SetParent(ninjaScreenController.menuCharacter.leftHandTransform, false);
		AudioPlayer.PlaySound("music/level/up");
		yield return new WaitForSeconds(0.5f);
		if (screenFlash != null)
		{
			screenFlash.FlashAlpha(1f, 2.5f, Color.white);
		}
		GameObject levelupFXGO = Object.Instantiate(levelUpFX, ninjaScreenController.menuCharacter.transform.position, ninjaScreenController.menuCharacter.transform.rotation);
		ParticleSystem[] levelupFXParticles = levelupFXGO.GetComponentsInChildren<ParticleSystem>();
		ninjaScreenController.RefreshLevel();
		SetLevelProgressBar();
		levelText.text = profile.Level.ToString();
		skillPointsText.text = profile.AvailableSkillPoints.ToString();
		while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == TeahouseTriggers.levelUp)
		{
			yield return null;
		}
		levelUpSkillCoin.gameObject.SetActive(true);
		levelUpSkillCoin.enabled = true;
		AudioPlayer.PlaySound("ui/panel/impact");
		yield return new WaitForSeconds(0.5f);
		levelUpSkillCoin.gameObject.SetActive(false);
		levelUpSkillCoin.enabled = false;
		Object.Destroy(levelUpSkillCoin);
		bool particlesAlive = true;
		while (particlesAlive)
		{
			particlesAlive = false;
			for (int i = 0; i < levelupFXParticles.Length; i++)
			{
				particlesAlive |= levelupFXParticles[i].IsAlive();
			}
			yield return null;
		}
		Object.Destroy(levelupFXGO);
		yield return null;
	}
}
