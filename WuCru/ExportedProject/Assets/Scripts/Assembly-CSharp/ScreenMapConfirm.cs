using System.Text;
using Fabric;
using UnityEngine;

public class ScreenMapConfirm : ScreenRoster
{
	public static MapButtonInfo _chosenMapInfo;

	public BoxTransitionFX _transitionFX;

	public UITexture _previewTexture;

	public UILabel _levelName;

	public UISprite[] _stars;

	public ScreenObjectTransitionWidget _goButtonWidget;

	private bool _transitioning;

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		_transitionFX.Begin();
		Time.timeScale = 1f;
		ScreenHeaderNavigation._pInstance.DisableButtons();
		int numStars = GameController.Instance.areaMapMetaLayer.GetNumStars(_chosenMapInfo._pSeed);
		_stars[0].color = ((numStars <= 0) ? Color.grey : Color.white);
		_stars[1].color = ((numStars <= 1) ? Color.grey : Color.white);
		_stars[2].color = ((numStars <= 2) ? Color.grey : Color.white);
		Sprite previewImage = GameController.Instance.styleDB._styles[_chosenMapInfo._pNode.styleIndex].previewImage;
		_previewTexture.mainTexture = previewImage.texture;
		StringBuilder stringBuilder = new StringBuilder();
		string levelName = _chosenMapInfo._pNode.levelName;
		if (string.IsNullOrEmpty(levelName))
		{
			string styleName = GameController.Instance.styleDB._styles[_chosenMapInfo._pNode.styleIndex].styleName;
			string collectionName = GameController.Instance.themeDB.themeCollections.values[_chosenMapInfo._pNode.themeCollectionIndex].collectionName;
			stringBuilder.Append("LevelName.");
			stringBuilder.Append(styleName);
			string value = Localise(stringBuilder.ToString());
			string value2 = Localise("LevelName.SpaceCharacter");
			stringBuilder.Length = 0;
			stringBuilder.Append("LevelName.");
			stringBuilder.Append(collectionName);
			string value3 = Localise(stringBuilder.ToString());
			stringBuilder.Length = 0;
			stringBuilder.Append(value);
			stringBuilder.Append(value2);
			stringBuilder.Append(value3);
			_levelName.text = stringBuilder.ToString();
		}
		else
		{
			stringBuilder.Append("LevelName.");
			stringBuilder.Append(levelName);
			_levelName.text = Localise(stringBuilder.ToString());
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void HandleDeviceBackButton()
	{
		if (base._pCurrentTweenType != ScreenTweenType.Idle || _transitioning)
		{
			return;
		}
		if (_isFTUE)
		{
			if (!base._pInRadialMenu)
			{
				OnSlot1();
				return;
			}
			int num = _rosterInfo.Length;
			for (int i = 0; i < num; i++)
			{
				if (_rosterInfo[i]._minifigType == MinifigType.Zane)
				{
					OnHeroTap(_rosterInfo[i]);
					break;
				}
			}
		}
		else if (base._pInRadialMenu)
		{
			OnClick();
		}
		else
		{
			OnBack();
		}
	}

	public void OnBack()
	{
		if (!_transitioning)
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			_transitioning = false;
			TryChangeWidgetSets(base.gameObject, "Back");
			EndFlow(OnBackFlowComplete);
		}
	}

	public void OnBeginLevel()
	{
		if (!_transitioning)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_transitioning = true;
			int pSeed = _chosenMapInfo._pSeed;
			AreaMapNode pNode = _chosenMapInfo._pNode;
			GameAudioManager.Instance.StopAllMusic(false);
			GameAudioManager.Instance.StopAmbience(true, 1f);
			EventManager.Instance.PostEvent("music", EventAction.UnloadAudio);
			EventManager.Instance.PostEvent("ambience", EventAction.UnloadAudio);
			GameController.Instance.PlayerProfile.LastPlayedSeason = GameController.Instance.areaMapMetaLayer.season;
			GameController.Instance.styleIndex = pNode.styleIndex;
			GameController.Instance.themeCollectionIndex = pNode.themeCollectionIndex;
			GameController.Instance.seed = pSeed;
			GameController.Instance.areaMapMetaLayer.currentRow = pNode.row;
			GameController.Instance.areaMapMetaLayer.currentCol = pNode.col;
			GameController.Instance.PlayerProfile.SetCurrentAreaPosition(pNode.row, pNode.col);
			GameController.Instance.PlayerProfile.Save();
			TryChangeWidgetSets(base.gameObject, "Default");
			EndFlow(OnFlowComplete);
			_goButtonWidget.TweenOut();
		}
	}

	private void OnBackFlowComplete()
	{
		ScreenHeaderNavigation._pInstance.EnableButtons();
	}

	private void OnFlowComplete()
	{
		_transitioning = false;
		ScreenHeaderNavigation._pInstance.EnableButtons();
		ScreenHeaderNavigation._pInstance.CloseHeader();
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenMap screenInstance = instance.GetScreenInstance<ScreenMap>();
		ScreenLevelLoading screenInstance2 = instance.GetScreenInstance<ScreenLevelLoading>();
		screenInstance.Navigate(screenInstance2);
	}

	public override void OnHeroTap(RosterInfo source)
	{
		base.OnHeroTap(source);
	}
}
