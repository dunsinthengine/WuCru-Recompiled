using System;
using Cape.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPlayGUI : MonoBehaviour
{
	public Image imageMapBG;

	public Text wuCruText;

	public Image wuCruSwushImg;

	public Image imageLevel;

	public Image imageLevelOverlay;

	public Image imageStar1;

	public Image imageStar2;

	public Image imageStar3;

	public Button buttonOK;

	public Button buttonCancel;

	public Action onOKAction;

	public Action onCancelAction;

	public Text textLevelName;

	public NinjaPortrait[] portraits;

	public Texture missingTexture;

	public Sprite missingFrameTexture;

	public Sprite missingBackgroundTexture;

	public RawImage[] imageNinjas;

	private void Awake()
	{
		OnLocalize();
		Localization.Instance.LocalizationChanged += OnLocalize;
		GameControllerStateAreaMap gameControllerStateAreaMap = GameController.Instance.currentState as GameControllerStateAreaMap;
		imageLevelOverlay.gameObject.SetActive(false);
		int areaMapSeedToPlay = gameControllerStateAreaMap.AreaMapSeedToPlay;
		if (gameControllerStateAreaMap != null)
		{
			imageLevel.sprite = GameController.Instance.styleDB._styles[gameControllerStateAreaMap.AreaMapNodeToPlay.styleIndex].previewImage;
			Sprite levelImageOverlay = GameController.Instance.LevelExtensionController.GetLevelImageOverlay(areaMapSeedToPlay);
			if ((bool)levelImageOverlay)
			{
				imageLevelOverlay.gameObject.SetActive(true);
				imageLevelOverlay.sprite = levelImageOverlay;
			}
		}
		else if (GameController.Instance.currentState is GameControllerStatePlaying)
		{
			imageLevel.sprite = GameController.Instance.CurrentLevel.LevelStyle.previewImage;
		}
		string confirmPlaySoundName = GameController.Instance.LevelExtensionController.GetConfirmPlaySoundName(areaMapSeedToPlay);
		if (confirmPlaySoundName != null)
		{
			AudioPlayer.PlaySound(confirmPlaySoundName);
		}
		string areaMapStarIconFull = Paths.areaMapStarIconFull;
		string areaMapStarIconEmpty = Paths.areaMapStarIconEmpty;
		imageStar1.sprite = Resources.Load<Sprite>(areaMapStarIconEmpty);
		imageStar2.sprite = Resources.Load<Sprite>(areaMapStarIconEmpty);
		imageStar3.sprite = Resources.Load<Sprite>(areaMapStarIconEmpty);
		Color color = imageStar1.color;
		Color color2 = imageStar2.color;
		Color color3 = imageStar3.color;
		color.a = 0.7f;
		color2.a = 0.7f;
		color3.a = 0.7f;
		switch (GameController.Instance.areaMapMetaLayer.GetNumStars(areaMapSeedToPlay))
		{
		case 3:
			imageStar3.sprite = Resources.Load<Sprite>(areaMapStarIconFull);
			imageStar2.sprite = Resources.Load<Sprite>(areaMapStarIconFull);
			imageStar1.sprite = Resources.Load<Sprite>(areaMapStarIconFull);
			color3.a = 1f;
			color2.a = 1f;
			color.a = 1f;
			break;
		case 2:
			imageStar2.sprite = Resources.Load<Sprite>(areaMapStarIconFull);
			imageStar1.sprite = Resources.Load<Sprite>(areaMapStarIconFull);
			color2.a = 1f;
			color.a = 1f;
			break;
		case 1:
			imageStar1.sprite = Resources.Load<Sprite>(areaMapStarIconFull);
			color.a = 1f;
			break;
		}
		imageStar1.color = color;
		imageStar2.color = color2;
		imageStar3.color = color3;
		buttonOK.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			onOKAction();
			UnityEngine.Object.Destroy(base.gameObject);
		});
		buttonCancel.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			onCancelAction();
			UnityEngine.Object.Destroy(base.gameObject);
		});
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		if (playerProfile.NinjaTeamSlotCount > 1 && GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Count > 1)
		{
			wuCruText.gameObject.SetActive(true);
			wuCruSwushImg.gameObject.SetActive(true);
			Texture2D[] array = CharacterPreviewRenderer.Generate(playerProfile.NinjaTeam, (int)imageNinjas[0].rectTransform.sizeDelta.x * 2, (int)imageNinjas[0].rectTransform.sizeDelta.y * 2);
			for (int num = 0; num < imageNinjas.Length; num++)
			{
				if (num < GameController.Instance.PlayerProfile.NinjaTeamSlotCount && num < GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Count)
				{
					int ninjaInTeam = playerProfile.GetNinjaInTeam(num);
					if (ninjaInTeam >= 0)
					{
						portraits[num].Configure(ninjaInTeam, array[num]);
					}
					else if (num < playerProfile.NinjaTeamSlotCount)
					{
						SetMissing(portraits[num]);
					}
					else
					{
						SetMissing(portraits[num]);
					}
					portraits[num].button.onClick.AddListener(() =>
					{
						NinjaScreenController.OpeningPage = NinjaScreenOpeningPage.CruStats;
						SceneManager.LoadScene("NinjaScreen");
					});
				}
				else
				{
					portraits[num].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			for (int num2 = 0; num2 < imageNinjas.Length; num2++)
			{
				portraits[num2].gameObject.SetActive(false);
			}
			wuCruText.gameObject.SetActive(false);
			wuCruSwushImg.gameObject.SetActive(false);
		}
	}

	private void SetMissing(NinjaPortrait portrait)
	{
		NinjaPortraitData component = GameController.Instance.GetComponent<NinjaPortraitData>();
		portrait.portrait.texture = missingTexture;
		portrait.portrait.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		Vector3 localPosition = portrait.portrait.transform.localPosition;
		localPosition.y = 0f;
		portrait.portrait.transform.localPosition = localPosition;
		portrait.power.enabled = false;
		portrait.weapon.enabled = false;
		portrait.SetFrameState(NinjaPortraitState.Active);
		portrait.background.color = component.FrameColor(NinjaPortraitState.Disabled);
		portrait.frame[0].sprite = missingFrameTexture;
		portrait.frame[0].fillAmount = 1f;
		portrait.frame[1].enabled = false;
		portrait.background.sprite = missingBackgroundTexture;
	}

	private void OnDestroy()
	{
		Localization.Instance.LocalizationChanged -= OnLocalize;
		for (int i = 0; i < imageNinjas.Length; i++)
		{
			if (i < GameController.Instance.PlayerProfile.NinjaTeamSlotCount)
			{
				RenderTexture renderTexture = portraits[i].portrait.texture as RenderTexture;
				portraits[i].portrait.texture = null;
				imageNinjas[i] = null;
				if ((bool)renderTexture)
				{
					renderTexture.Release();
					UnityEngine.Object.DestroyImmediate(renderTexture);
				}
				UnityEngine.Object.DestroyImmediate(portraits[i].gameObject);
			}
		}
	}

	public void OnLocalize()
	{
		GameControllerStateAreaMap gameControllerStateAreaMap = GameController.Instance.currentState as GameControllerStateAreaMap;
		string levelName = gameControllerStateAreaMap.AreaMapNodeToPlay.levelName;
		if (string.IsNullOrEmpty(levelName))
		{
			string styleName = GameController.Instance.styleDB._styles[gameControllerStateAreaMap.AreaMapNodeToPlay.styleIndex].styleName;
			string collectionName = GameController.Instance.themeDB.themeCollections.values[gameControllerStateAreaMap.AreaMapNodeToPlay.themeCollectionIndex].collectionName;
			textLevelName.text = Localization.Instance.Get(styleName) + " " + Localization.Instance.Get(collectionName);
		}
		else
		{
			textLevelName.text = Localization.Instance.Get(levelName);
		}
	}
}
