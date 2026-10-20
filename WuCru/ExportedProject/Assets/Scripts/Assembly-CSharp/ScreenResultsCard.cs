using UnityEngine;

public class ScreenResultsCard : ScreenBase
{
	public enum STATE
	{
		INIT = 0,
		CARD_REVEAL = 1,
		CARD_REVEAL_DUPLICATE = 2,
		DONE = 3
	}

	private const int NUM_STUDS = 50;

	public static ECOLLECTABLE_CARD _acquiredCard;

	public BoxTransitionFX _boxTransitionFX;

	public Animator _revealAnimator;

	public Animator _glowAnimator;

	public Animator _duplicateAnimator;

	public ParticleSystem _revealSmoke;

	public UITexture _card;

	public UITexture _duplicateCard1;

	public UITexture _duplicateCard2;

	public UILabel _studCountLabel;

	private STATE _currentState;

	private float _stateStartTime;

	private bool _isNewCard;

	protected override void OnScreenExitComplete()
	{
		_card.mainTexture = null;
		_duplicateCard1.mainTexture = null;
		_duplicateCard2.mainTexture = null;
		base.OnScreenExitComplete();
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
	}

	protected override void OnShowScreen()
	{
		base.OnShowScreen();
		int acquiredCard = (int)_acquiredCard;
		if (GameController.Instance.PlayerProfile.HasCardBeenUnlocked(acquiredCard))
		{
			_isNewCard = false;
			_studCountLabel.text = 50.ToString();
			ScreenResultsExp._pStudsAcquired += 50;
		}
		else
		{
			_isNewCard = true;
			GameController.Instance.PlayerProfile.UnlockCard(acquiredCard);
		}
		Debug.Log("Acquired Card = " + _acquiredCard);
		_card.mainTexture = Resources.Load(CollectableCards.GetCardPath(_acquiredCard)) as Texture;
		_duplicateCard1.mainTexture = _card.mainTexture;
		_duplicateCard2.mainTexture = _card.mainTexture;
		_boxTransitionFX.Begin();
		_duplicateAnimator.gameObject.SetActive(false);
		_revealAnimator.gameObject.SetActive(false);
		_glowAnimator.gameObject.SetActive(false);
		ChangeState(STATE.INIT);
	}

	public void OnContinue()
	{
		AudioPlayer.PlaySound("ui/button/sword/shing");
		_glowAnimator.gameObject.SetActive(false);
		ChangeState(STATE.DONE);
		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		ScreenResultsExp screenInstance = instance.GetScreenInstance<ScreenResultsExp>();
		if (GlobalInGameData.ShouldLaunchMovieCampaign())
		{
			GlobalInGameData.LaunchMovieCampaign();
		}
		Navigate(screenInstance);
	}

	private void ChangeState(STATE newState)
	{
		Debug.Log(string.Concat("ChangeState: ", _currentState, " -> ", newState));
		_currentState = newState;
		_stateStartTime = RealTime.time;
		switch (_currentState)
		{
		case STATE.INIT:
			StartInit();
			break;
		case STATE.CARD_REVEAL:
			StartCardReveal();
			break;
		case STATE.CARD_REVEAL_DUPLICATE:
			StartCardRevealDuplicate();
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
		case STATE.CARD_REVEAL:
			UpdateCardReveal();
			break;
		case STATE.CARD_REVEAL_DUPLICATE:
			UpdateCardRevealDuplicate();
			break;
		}
	}

	private void StartInit()
	{
	}

	private void UpdateInit()
	{
		float num = RealTime.time - _stateStartTime;
		if (num >= 0.5f)
		{
			if (_isNewCard)
			{
				ChangeState(STATE.CARD_REVEAL);
			}
			else
			{
				ChangeState(STATE.CARD_REVEAL_DUPLICATE);
			}
		}
	}

	private void StartCardReveal()
	{
		_revealAnimator.gameObject.SetActive(true);
		_revealAnimator.SetTrigger("Play");
	}

	private void UpdateCardReveal()
	{
		float num = RealTime.time - _stateStartTime;
		if (num >= 1f)
		{
			_glowAnimator.gameObject.SetActive(true);
		}
	}

	private void StartCardRevealDuplicate()
	{
		_duplicateAnimator.gameObject.SetActive(true);
		_duplicateAnimator.SetTrigger("Play");
	}

	private void UpdateCardRevealDuplicate()
	{
	}
}
