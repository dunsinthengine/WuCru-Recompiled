using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class ScreenStickerBook : ScreenBase
{
	public enum STATE
	{
		INIT = 0,
		FLIP_NEXT_A = 1,
		FLIP_NEXT_B = 2,
		FLIP_PREV_A = 3,
		FLIP_PREV_B = 4,
		ZOOMING_CARD = 5,
		VIEWING_CARD = 6,
		UNZOOOMING_CARD = 7,
		UNLOCK_NEW = 8,
		DONE = 9
	}

	public const int NUM_ITEMS_PER_PAGE = 4;

	public UIWidget _previousPage;

	public UIWidget _nextPage;

	public UIButton _prevButton;

	public UIButton _nextButton;

	public UITexture _zoomTexture;

	public UIWidget _zoomReference;

	public UITexture[] _leftPageElems;

	public UITexture[] _rightPageElems;

	public UITexture[] _leftPageFlipElems;

	public UITexture[] _rightPageFlipElems;

	public GoldenTrailFX[] _goldenTrails;

	private STATE _currentState;

	private float _stateStartTime;

	private int _currentPage;

	private float _introT;

	private UITexture _zoomStartTexture;

	private List<UITexture> _newElems = new List<UITexture>();

	private List<Texture> _newElemsTextures = new List<Texture>();

	protected override void OnScreenExitComplete()
	{
		UICamera.fallThrough = null;
		int num = _leftPageElems.Length;
		for (int i = 0; i < num; i++)
		{
			_leftPageElems[i].mainTexture = null;
		}
		num = _rightPageElems.Length;
		for (int j = 0; j < num; j++)
		{
			_rightPageElems[j].mainTexture = null;
		}
		num = _leftPageFlipElems.Length;
		for (int k = 0; k < num; k++)
		{
			_leftPageFlipElems[k].mainTexture = null;
		}
		num = _rightPageFlipElems.Length;
		for (int l = 0; l < num; l++)
		{
			_rightPageFlipElems[l].mainTexture = null;
		}
		num = _goldenTrails.Length;
		for (int m = 0; m < num; m++)
		{
			_goldenTrails[m].SetVisible(false);
			for (int n = 0; n < _goldenTrails[m]._textures.Length; n++)
			{
				_goldenTrails[m]._textures[n].mainTexture = null;
			}
		}
		_zoomTexture.mainTexture = null;
		_newElemsTextures.Clear();
		_newElems.Clear();
		base.OnScreenExitComplete();
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
	}

	public void OnDrag(Vector2 vel)
	{
		if (vel.x < -5f)
		{
			FlipNext();
		}
		else if (vel.x > 5f)
		{
			FlipPrev();
		}
	}

	public void OnClick()
	{
		if (_currentState == STATE.VIEWING_CARD)
		{
			ChangeState(STATE.UNZOOOMING_CARD);
		}
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		UICamera.fallThrough = base.gameObject;
		int num = _leftPageElems.Length;
		for (int i = 0; i < num; i++)
		{
			_leftPageElems[i].enabled = false;
		}
		num = _rightPageElems.Length;
		for (int j = 0; j < num; j++)
		{
			_rightPageElems[j].enabled = false;
		}
		num = _leftPageFlipElems.Length;
		for (int k = 0; k < num; k++)
		{
			_leftPageFlipElems[k].enabled = false;
		}
		num = _rightPageFlipElems.Length;
		for (int l = 0; l < num; l++)
		{
			_rightPageFlipElems[l].enabled = false;
		}
		Texture mainTexture = Resources.Load("CardFramePulse") as Texture;
		num = _goldenTrails.Length;
		for (int m = 0; m < num; m++)
		{
			_goldenTrails[m].SetVisible(false);
			for (int n = 0; n < _goldenTrails[m]._textures.Length; n++)
			{
				_goldenTrails[m]._textures[n].mainTexture = mainTexture;
			}
		}
		_introT = 0.5f;
		_currentPage = -1;
		_previousPage.gameObject.SetActive(false);
		_nextPage.gameObject.SetActive(false);
		_zoomTexture.mainTexture = null;
		ChangeState(STATE.INIT);
	}

	public void OnContinue()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenResultsExp screenInstance = instance.GetScreenInstance<ScreenResultsExp>();
		if (GlobalInGameData.ShouldLaunchMovieCampaign())
		{
			GlobalInGameData.LaunchMovieCampaign();
		}
		Navigate(screenInstance);
	}

	public void PerformZoom(UITexture _source)
	{
		if (_currentState != STATE.DONE)
		{
			if (_currentState == STATE.VIEWING_CARD)
			{
				OnClick();
			}
		}
		else if (!IsNotACard(_source))
		{
			_zoomTexture.mainTexture = _source.mainTexture;
			_zoomTexture.transform.position = _source.transform.position;
			_zoomTexture.width = _source.width;
			_zoomTexture.height = _source.height;
			GoldenTrailFX componentInChildren = _source.GetComponentInChildren<GoldenTrailFX>();
			if (componentInChildren != null)
			{
				componentInChildren.SetVisible(false);
			}
			_zoomStartTexture = _source;
			ChangeState(STATE.ZOOMING_CARD);
		}
	}

	private bool IsNotACard(UITexture _source)
	{
		if (_source == null)
		{
			return true;
		}
		if (_source.mainTexture == null)
		{
			return true;
		}
		if (_source.mainTexture.name == "Missing_Card_PL")
		{
			return true;
		}
		return false;
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
		case STATE.FLIP_NEXT_A:
			StartFlipNextA();
			break;
		case STATE.FLIP_NEXT_B:
			StartFlipNextB();
			break;
		case STATE.FLIP_PREV_A:
			StartFlipPrevA();
			break;
		case STATE.FLIP_PREV_B:
			StartFlipPrevB();
			break;
		case STATE.ZOOMING_CARD:
			StartZoom();
			break;
		case STATE.UNZOOOMING_CARD:
			StartUnzoom();
			break;
		case STATE.DONE:
			StartDone();
			break;
		case STATE.UNLOCK_NEW:
			StartUnlockNew();
			break;
		case STATE.VIEWING_CARD:
			break;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (_introT > 0f)
		{
			_introT -= Time.deltaTime;
			if (_introT <= 0f)
			{
				LoadTextures(-1);
				ChangeState(STATE.FLIP_NEXT_A);
			}
		}
		switch (_currentState)
		{
		case STATE.INIT:
			UpdateInit();
			break;
		case STATE.FLIP_NEXT_A:
			UpdateFlipNextA();
			break;
		case STATE.FLIP_NEXT_B:
			UpdateFlipNextB();
			break;
		case STATE.FLIP_PREV_A:
			UpdateFlipPrevA();
			break;
		case STATE.FLIP_PREV_B:
			UpdateFlipPrevB();
			break;
		case STATE.ZOOMING_CARD:
			UpdateZoom();
			break;
		case STATE.UNZOOOMING_CARD:
			UpdateUnzoom();
			break;
		case STATE.UNLOCK_NEW:
			UpdateUnlockNew();
			break;
		case STATE.VIEWING_CARD:
			break;
		}
	}

	private void StartInit()
	{
	}

	private void UpdateInit()
	{
		float num = RealTime.time - _stateStartTime;
		if (num > 0.5f)
		{
			ChangeState(STATE.DONE);
		}
	}

	private void StartFlipNextA()
	{
		Vector3 localScale = _nextPage.transform.localScale;
		localScale.x = 1f;
		_nextPage.transform.localScale = localScale;
		_nextPage.gameObject.SetActive(true);
		localScale = _previousPage.transform.localScale;
		localScale.x = 0f;
		_previousPage.transform.localScale = localScale;
		_previousPage.gameObject.SetActive(false);
		int num = _rightPageElems.Length;
		for (int i = 0; i < num; i++)
		{
			_rightPageElems[i].enabled = true;
		}
		num = _leftPageFlipElems.Length;
		for (int j = 0; j < num; j++)
		{
			_leftPageFlipElems[j].enabled = true;
		}
		if (_currentPage != -1)
		{
			num = _rightPageFlipElems.Length;
			for (int k = 0; k < num; k++)
			{
				_rightPageFlipElems[k].enabled = true;
			}
		}
		num = _goldenTrails.Length;
		for (int l = 0; l < num; l++)
		{
			_goldenTrails[l].SetVisible(false);
		}
	}

	private void UpdateFlipNextA()
	{
		float num = RealTime.time - _stateStartTime;
		if (num < 0.25f)
		{
			float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 0.25f, 0f, 1f);
			Vector3 localScale = _nextPage.transform.localScale;
			localScale.x = 1f - num2;
			_nextPage.transform.localScale = localScale;
		}
		else
		{
			ChangeState(STATE.FLIP_NEXT_B);
		}
	}

	private void StartFlipNextB()
	{
		_nextPage.gameObject.SetActive(false);
		_previousPage.gameObject.SetActive(true);
	}

	private void UpdateFlipNextB()
	{
		float num = RealTime.time - _stateStartTime;
		if (num < 0.25f)
		{
			float x = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.25f, 0f, 1f);
			Vector3 localScale = _previousPage.transform.localScale;
			localScale.x = x;
			_previousPage.transform.localScale = localScale;
			return;
		}
		int num2 = _leftPageElems.Length;
		for (int i = 0; i < num2; i++)
		{
			_leftPageElems[i].mainTexture = _leftPageFlipElems[i].mainTexture;
			_leftPageElems[i].enabled = true;
		}
		_previousPage.gameObject.SetActive(false);
		_currentPage++;
		num2 = _leftPageFlipElems.Length;
		for (int j = 0; j < num2; j++)
		{
			_leftPageFlipElems[j].mainTexture = null;
		}
		if (_newElems.Count == 0)
		{
			ChangeState(STATE.DONE);
		}
		else
		{
			ChangeState(STATE.UNLOCK_NEW);
		}
	}

	private void StartFlipPrevA()
	{
		Vector3 localScale = _nextPage.transform.localScale;
		localScale.x = 1f;
		_previousPage.transform.localScale = localScale;
		_previousPage.gameObject.SetActive(true);
		localScale = _nextPage.transform.localScale;
		localScale.x = 0f;
		_nextPage.transform.localScale = localScale;
		_nextPage.gameObject.SetActive(false);
		int num = _goldenTrails.Length;
		for (int i = 0; i < num; i++)
		{
			_goldenTrails[i].SetVisible(false);
		}
	}

	private void UpdateFlipPrevA()
	{
		float num = RealTime.time - _stateStartTime;
		if (num < 0.25f)
		{
			float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 0.25f, 0f, 1f);
			Vector3 localScale = _previousPage.transform.localScale;
			localScale.x = 1f - num2;
			_previousPage.transform.localScale = localScale;
		}
		else
		{
			ChangeState(STATE.FLIP_PREV_B);
		}
	}

	private void StartFlipPrevB()
	{
		_previousPage.gameObject.SetActive(false);
		_nextPage.gameObject.SetActive(true);
		int num = _rightPageFlipElems.Length;
		for (int i = 0; i < num; i++)
		{
			_rightPageFlipElems[i].enabled = true;
		}
	}

	private void UpdateFlipPrevB()
	{
		float num = RealTime.time - _stateStartTime;
		if (num < 0.25f)
		{
			float x = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.25f, 0f, 1f);
			Vector3 localScale = _nextPage.transform.localScale;
			localScale.x = x;
			_nextPage.transform.localScale = localScale;
			return;
		}
		_nextPage.gameObject.SetActive(false);
		_currentPage--;
		int num2 = _rightPageElems.Length;
		for (int i = 0; i < num2; i++)
		{
			_rightPageElems[i].mainTexture = _rightPageFlipElems[i].mainTexture;
			_rightPageElems[i].enabled = true;
		}
		num2 = _rightPageFlipElems.Length;
		for (int j = 0; j < num2; j++)
		{
			_rightPageFlipElems[j].mainTexture = null;
		}
		if (_newElems.Count == 0)
		{
			ChangeState(STATE.DONE);
		}
		else
		{
			ChangeState(STATE.UNLOCK_NEW);
		}
	}

	private void StartZoom()
	{
		ECOLLECTABLE_CARD cardID = CollectableCards.GetCardID(_zoomStartTexture.mainTexture);
		GameController.Instance.PlayerProfile.MarkUnseenCardAsSeen((int)cardID);
		_zoomStartTexture.enabled = false;
		_prevButton.isEnabled = false;
		_nextButton.isEnabled = false;
	}

	private void UpdateZoom()
	{
		float num = RealTime.time - _stateStartTime;
		if (num < 0.5f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			_zoomTexture.transform.position = Vector3.Lerp(_zoomStartTexture.transform.position, _zoomReference.transform.position, t);
			_zoomTexture.width = (int)Mathf.Lerp(_zoomStartTexture.width, _zoomReference.width, t);
			_zoomTexture.height = (int)Mathf.Lerp(_zoomStartTexture.height, _zoomReference.height, t);
		}
		else
		{
			_zoomTexture.transform.position = _zoomReference.transform.position;
			_zoomTexture.width = _zoomReference.width;
			_zoomTexture.height = _zoomReference.height;
			ChangeState(STATE.VIEWING_CARD);
		}
	}

	private void StartUnzoom()
	{
	}

	private void UpdateUnzoom()
	{
		float num = RealTime.time - _stateStartTime;
		if (num < 0.5f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			_zoomTexture.transform.position = Vector3.Lerp(_zoomReference.transform.position, _zoomStartTexture.transform.position, t);
			_zoomTexture.width = (int)Mathf.Lerp(_zoomReference.width, _zoomStartTexture.width, t);
			_zoomTexture.height = (int)Mathf.Lerp(_zoomReference.height, _zoomStartTexture.height, t);
		}
		else
		{
			_zoomTexture.transform.position = _zoomStartTexture.transform.position;
			_zoomTexture.width = _zoomStartTexture.width;
			_zoomTexture.height = _zoomStartTexture.height;
			_zoomStartTexture.enabled = true;
			_zoomTexture.mainTexture = null;
			ChangeState(STATE.DONE);
		}
	}

	private void StartUnlockNew()
	{
		_prevButton.isEnabled = false;
		_nextButton.isEnabled = false;
		_zoomTexture.width = _newElems[0].width;
		_zoomTexture.height = _newElems[0].height;
		_zoomTexture.mainTexture = _newElemsTextures[0];
		_zoomTexture.transform.position = _newElems[0].transform.position;
		_zoomTexture.transform.localScale = Vector3.one * 5f;
		_zoomTexture.alpha = 0f;
	}

	private void UpdateUnlockNew()
	{
		float num = RealTime.time - _stateStartTime;
		float num2 = 0.5f;
		if (num < num2)
		{
			float num3 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, num2, 0f, 1f);
			_zoomTexture.transform.localScale = Vector3.Lerp(Vector3.one * 5f, Vector3.one, num3);
			_zoomTexture.alpha = num3;
			return;
		}
		_newElems[0].mainTexture = _zoomTexture.mainTexture;
		GoldenTrailFX componentInChildren = _newElems[0].GetComponentInChildren<GoldenTrailFX>();
		if (componentInChildren != null)
		{
			componentInChildren.SetVisible(true);
		}
		_zoomTexture.mainTexture = null;
		_newElems.RemoveAt(0);
		_newElemsTextures.RemoveAt(0);
		if (_newElemsTextures.Count > 0)
		{
			ChangeState(STATE.UNLOCK_NEW);
		}
		else
		{
			ChangeState(STATE.DONE);
		}
	}

	private void StartDone()
	{
		_prevButton.isEnabled = _currentPage > 0 && _newElems.Count == 0;
		_nextButton.isEnabled = (_currentPage + 1) * 4 * 2 < 13 && _newElems.Count == 0;
	}

	public void Close()
	{
		EndFlow();
	}

	public void FlipNext()
	{
		if (_currentState == STATE.DONE && (_currentPage + 1) * 4 * 2 < 13)
		{
			LoadTextures(_currentPage + 1, true);
			ChangeState(STATE.FLIP_NEXT_A);
		}
	}

	public void FlipPrev()
	{
		if (_currentState == STATE.DONE && _currentPage != 0)
		{
			LoadTextures(_currentPage - 1);
			ChangeState(STATE.FLIP_PREV_A);
		}
	}

	private void LoadTextures(int target, bool goingNext = false)
	{
		_newElems.Clear();
		_newElemsTextures.Clear();
		if (target == -1)
		{
			int num = 4;
			for (int i = 0; i < num; i++)
			{
				int num2 = i + 1;
				ECOLLECTABLE_CARD eCOLLECTABLE_CARD = (ECOLLECTABLE_CARD)num2;
				bool flag = GameController.Instance.PlayerProfile.IsCardUnseen((int)eCOLLECTABLE_CARD);
				_leftPageFlipElems[i].mainTexture = Resources.Load(CollectableCards.GetCardPath(eCOLLECTABLE_CARD)) as Texture;
				if (flag)
				{
					_newElems.Add(_leftPageElems[i]);
					_newElemsTextures.Add(_leftPageFlipElems[i].mainTexture);
					_leftPageFlipElems[i].mainTexture = Resources.Load(CollectableCards.GetCardPath(ECOLLECTABLE_CARD.INVALID)) as Texture;
				}
				eCOLLECTABLE_CARD = (ECOLLECTABLE_CARD)(num2 + 4);
				flag = GameController.Instance.PlayerProfile.IsCardUnseen((int)eCOLLECTABLE_CARD);
				_rightPageElems[i].mainTexture = Resources.Load(CollectableCards.GetCardPath(eCOLLECTABLE_CARD)) as Texture;
				if (flag)
				{
					_newElems.Add(_rightPageElems[i]);
					_newElemsTextures.Add(_rightPageElems[i].mainTexture);
					_rightPageElems[i].mainTexture = Resources.Load(CollectableCards.GetCardPath(ECOLLECTABLE_CARD.INVALID)) as Texture;
				}
			}
		}
		else if (goingNext)
		{
			int num3 = 4;
			for (int j = 0; j < num3; j++)
			{
				int num4 = j + 1 + target * 4 * 2;
				_rightPageFlipElems[j].mainTexture = _rightPageElems[j].mainTexture;
				bool flag2 = num4 >= 13;
				ECOLLECTABLE_CARD cardID = (ECOLLECTABLE_CARD)num4;
				bool flag3 = !flag2 && GameController.Instance.PlayerProfile.IsCardUnseen((int)cardID);
				_leftPageFlipElems[j].mainTexture = ((!flag2) ? (Resources.Load(CollectableCards.GetCardPath((ECOLLECTABLE_CARD)num4)) as Texture) : null);
				if (flag3)
				{
					_newElems.Add(_leftPageElems[j]);
					_newElemsTextures.Add(_leftPageFlipElems[j].mainTexture);
					_leftPageFlipElems[j].mainTexture = Resources.Load(CollectableCards.GetCardPath(ECOLLECTABLE_CARD.INVALID)) as Texture;
				}
				num4 = j + 1 + target * 4 * 2 + 4;
				flag2 = num4 >= 13;
				cardID = (ECOLLECTABLE_CARD)num4;
				flag3 = !flag2 && GameController.Instance.PlayerProfile.IsCardUnseen((int)cardID);
				_rightPageElems[j].mainTexture = ((!flag2) ? (Resources.Load(CollectableCards.GetCardPath((ECOLLECTABLE_CARD)num4)) as Texture) : null);
				if (flag3)
				{
					_newElems.Add(_rightPageElems[j]);
					_newElemsTextures.Add(_rightPageElems[j].mainTexture);
					_rightPageElems[j].mainTexture = Resources.Load(CollectableCards.GetCardPath(ECOLLECTABLE_CARD.INVALID)) as Texture;
				}
			}
		}
		else
		{
			if (goingNext)
			{
				return;
			}
			int num5 = 4;
			for (int k = 0; k < num5; k++)
			{
				_leftPageFlipElems[k].mainTexture = _leftPageElems[k].mainTexture;
				int num6 = k + 1 + target * 4 * 2 + 4;
				bool flag4 = num6 >= 13;
				ECOLLECTABLE_CARD cardID2 = (ECOLLECTABLE_CARD)num6;
				bool flag5 = !flag4 && GameController.Instance.PlayerProfile.IsCardUnseen((int)cardID2);
				_rightPageFlipElems[k].mainTexture = ((!flag4) ? (Resources.Load(CollectableCards.GetCardPath((ECOLLECTABLE_CARD)num6)) as Texture) : null);
				if (flag5)
				{
					_newElems.Add(_rightPageElems[k]);
					_newElemsTextures.Add(_rightPageFlipElems[k].mainTexture);
					_rightPageFlipElems[k].mainTexture = Resources.Load(CollectableCards.GetCardPath(ECOLLECTABLE_CARD.INVALID)) as Texture;
				}
				num6 = k + 1 + target * 4 * 2;
				flag4 = num6 >= 13;
				cardID2 = (ECOLLECTABLE_CARD)num6;
				flag5 = !flag4 && GameController.Instance.PlayerProfile.IsCardUnseen((int)cardID2);
				_leftPageElems[k].mainTexture = ((!flag4) ? (Resources.Load(CollectableCards.GetCardPath((ECOLLECTABLE_CARD)num6)) as Texture) : null);
				if (flag5)
				{
					_newElems.Add(_leftPageElems[k]);
					_newElemsTextures.Add(_leftPageElems[k].mainTexture);
					_leftPageElems[k].mainTexture = Resources.Load(CollectableCards.GetCardPath(ECOLLECTABLE_CARD.INVALID)) as Texture;
				}
			}
		}
	}
}
