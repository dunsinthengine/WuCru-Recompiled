using System.Text;
using AmuzoEngine;
using UnityEngine;

public class ObjectiveBar : MonoBehaviour
{
	public enum STATE
	{
		INTRO = 0,
		WAIT_AT_BOTTOM = 1,
		TRANSITION_OFF_SCREEN = 2,
		OFF_SCREEN = 3,
		TRANSITION_ON_SCREEN = 4,
		COMPLETED = 5
	}

	public Transform _midScreenPos;

	public Transform _bottomScreenPos;

	public Transform _offScreenPos;

	public UISprite _completedMarker;

	public UILabel _objectiveName;

	public UITexture _objectiveIcon;

	public UILabel _objectiveProgress;

	private STATE _currentState;

	private float _stateStartTime;

	private StringBuilder _dummySB = new StringBuilder();

	private int _lastKnownAmountCollected;

	private bool _completed;

	private float _objectiveProgressChangeTime;

	public void OnShowScreen()
	{
		_completedMarker.enabled = false;
		ChangeState(STATE.INTRO);
		_objectiveName.text = Localise("Challenges." + GameController.Instance.CurrentLevel.Challenge._pName);
		Texture mainTexture = Resources.Load<Texture>(GameController.Instance.CurrentLevel.Challenge.IconPath());
		_objectiveIcon.mainTexture = mainTexture;
		RefreshProgress();
		_completed = false;
	}

	public void OnUpdate()
	{
		if (!(GameController.Instance == null) && GameController.Instance.CurrentLevel != null && GameController.Instance.CurrentLevel.Challenge != null)
		{
			float num = Time.time - _objectiveProgressChangeTime;
			if (num < 0.2f)
			{
				float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 0.2f, 0f, 1f);
				_objectiveIcon.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t);
				_objectiveProgress.transform.localScale = _objectiveIcon.transform.localScale;
			}
			else
			{
				_objectiveIcon.transform.localScale = Vector3.one;
				_objectiveProgress.transform.localScale = Vector3.one;
			}
			switch (_currentState)
			{
			case STATE.INTRO:
				UpdateIntro();
				break;
			case STATE.WAIT_AT_BOTTOM:
				UpdateWaitAtBottom();
				break;
			case STATE.TRANSITION_OFF_SCREEN:
				UpdateTransitionOffScreen();
				break;
			case STATE.TRANSITION_ON_SCREEN:
				UpdateTransitionOnScreen();
				break;
			case STATE.OFF_SCREEN:
				UpdateOffScreen();
				break;
			case STATE.COMPLETED:
				UpdateCompleted();
				break;
			}
		}
	}

	public void OnChallengeUpdated()
	{
		ChangeState(STATE.TRANSITION_ON_SCREEN);
	}

	protected string Localise(string key)
	{
		string text = LocalisationFacade.Instance.GetString(key);
		if (text == string.Empty || text == null)
		{
			return key;
		}
		return text;
	}

	private void RefreshProgress()
	{
		_dummySB.Length = 0;
		if (GameController.Instance.CurrentLevel.Challenge.HasCounter())
		{
			_lastKnownAmountCollected = GameController.Instance.CurrentLevel.Challenge.AmountCollected();
			_dummySB.Append(_lastKnownAmountCollected);
			_dummySB.Append("/");
			_dummySB.Append(GameController.Instance.CurrentLevel.Challenge.AmountToCollect());
		}
		else
		{
			_dummySB.Append(string.Empty);
		}
		_objectiveProgress.text = _dummySB.ToString();
		_objectiveIcon.transform.localScale = Vector3.one * 2f;
		_objectiveProgress.transform.localScale = Vector3.one * 2f;
		_objectiveProgressChangeTime = Time.time;
	}

	private void ChangeState(STATE newState)
	{
		_currentState = newState;
		_stateStartTime = Time.time;
		switch (_currentState)
		{
		case STATE.INTRO:
			StartIntro();
			break;
		case STATE.WAIT_AT_BOTTOM:
			StartWaitAtBottom();
			break;
		case STATE.TRANSITION_OFF_SCREEN:
			StartTransitionOffScreen();
			break;
		case STATE.TRANSITION_ON_SCREEN:
			StartTransitionOnScreen();
			break;
		case STATE.OFF_SCREEN:
			StartOffScreen();
			break;
		case STATE.COMPLETED:
			StartCompleted();
			break;
		}
	}

	private void StartIntro()
	{
		base.transform.localPosition = _midScreenPos.localPosition;
		base.transform.localScale = Vector3.zero;
	}

	private void UpdateIntro()
	{
		float num = Time.time - _stateStartTime;
		if (num < 1f)
		{
			return;
		}
		if (num < 1.5f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num - 1f, 0.5f, 0f, 1f);
			base.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t);
		}
		else if (!(num < 2.5f))
		{
			if (num < 3f)
			{
				float t2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num - 2.5f, 0.5f, 0f, 1f);
				base.transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, t2);
				base.transform.localPosition = Vector3.Lerp(_midScreenPos.localPosition, _bottomScreenPos.localPosition, t2);
			}
			else
			{
				ChangeState(STATE.WAIT_AT_BOTTOM);
			}
		}
	}

	private void StartWaitAtBottom()
	{
	}

	private void UpdateWaitAtBottom()
	{
		float num = Time.time - _stateStartTime;
		if (num > 3f)
		{
			ChangeState(STATE.TRANSITION_OFF_SCREEN);
		}
		else if (_lastKnownAmountCollected != GameController.Instance.CurrentLevel.Challenge.AmountCollected())
		{
			RefreshProgress();
			if (GameController.Instance.CurrentLevel.Challenge.AmountToCollect() == _lastKnownAmountCollected)
			{
				ChangeState(STATE.COMPLETED);
			}
		}
	}

	private void StartTransitionOnScreen()
	{
	}

	private void UpdateTransitionOnScreen()
	{
		float num = Time.time - _stateStartTime;
		if (num < 1f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 1f, 0f, 1f);
			base.transform.localPosition = Vector3.Lerp(_offScreenPos.localPosition, _bottomScreenPos.localPosition, t);
			return;
		}
		base.transform.localPosition = _bottomScreenPos.localPosition;
		RefreshProgress();
		if (GameController.Instance.CurrentLevel.Challenge.AmountToCollect() == _lastKnownAmountCollected)
		{
			ChangeState(STATE.COMPLETED);
		}
		else
		{
			ChangeState(STATE.WAIT_AT_BOTTOM);
		}
	}

	private void StartTransitionOffScreen()
	{
	}

	private void UpdateTransitionOffScreen()
	{
		float num = Time.time - _stateStartTime;
		if (num < 1f)
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 1f, 0f, 1f);
			base.transform.localPosition = Vector3.Lerp(_bottomScreenPos.localPosition, _offScreenPos.localPosition, t);
		}
		else
		{
			base.transform.localPosition = _offScreenPos.localPosition;
			ChangeState(STATE.OFF_SCREEN);
		}
	}

	private void StartOffScreen()
	{
	}

	private void UpdateOffScreen()
	{
		if (_lastKnownAmountCollected != GameController.Instance.CurrentLevel.Challenge.AmountCollected() && !_completed)
		{
			ChangeState(STATE.TRANSITION_ON_SCREEN);
		}
	}

	private void StartCompleted()
	{
		_completedMarker.enabled = true;
		_completedMarker.alpha = 0f;
		_completed = true;
	}

	private void UpdateCompleted()
	{
		float num = Time.time - _stateStartTime;
		if (num < 0.5f)
		{
			float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseInCircle, num, 0.5f, 0f, 1f);
			_completedMarker.transform.localScale = Vector3.Lerp(Vector3.one * 5f, Vector3.one, num2);
			_completedMarker.alpha = num2;
		}
		else
		{
			_completedMarker.alpha = 1f;
			_completedMarker.transform.localScale = Vector3.one;
			ChangeState(STATE.WAIT_AT_BOTTOM);
		}
	}
}
