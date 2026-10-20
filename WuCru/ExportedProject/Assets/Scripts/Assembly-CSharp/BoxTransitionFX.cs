using UnityEngine;

public class BoxTransitionFX : MonoBehaviour
{
	private enum STATE
	{
		START_WAIT = 0,
		TWEEN_WIDTH = 1,
		TWEEN_HEIGHT = 2,
		COMPLETE = 3
	}

	public int _startWidth;

	public int _startHeight;

	public int _endWidth;

	public int _endHeight;

	public float _startDelay;

	public float _stateDuration = 0.2f;

	private UIWidget _widget;

	private float _stateStartTime;

	private STATE _currentState = STATE.COMPLETE;

	private UIWidget _pWidget
	{
		get
		{
			if (_widget == null)
			{
				_widget = GetComponent<UIWidget>();
			}
			return _widget;
		}
	}

	public void Begin()
	{
		_stateStartTime = RealTime.time;
		_currentState = STATE.START_WAIT;
		_pWidget.width = _startWidth;
		_pWidget.height = _startHeight;
	}

	private void OnEnable()
	{
		_pWidget.width = _startWidth;
		_pWidget.height = _startHeight;
	}

	private void Update()
	{
		if (_currentState == STATE.COMPLETE)
		{
			return;
		}
		float num = RealTime.time - _stateStartTime;
		float num2 = 0f;
		switch (_currentState)
		{
		case STATE.START_WAIT:
			if (num >= _startDelay)
			{
				ChangeState(STATE.TWEEN_WIDTH);
			}
			break;
		case STATE.TWEEN_WIDTH:
			num2 = num / _stateDuration;
			if (num2 < 1f)
			{
				_pWidget.width = (int)Mathf.Lerp(_startWidth, _endWidth, num2);
				break;
			}
			_pWidget.width = _endWidth;
			ChangeState(STATE.TWEEN_HEIGHT);
			break;
		case STATE.TWEEN_HEIGHT:
			num2 = num / _stateDuration;
			if (num2 < 1f)
			{
				_pWidget.height = (int)Mathf.Lerp(_startHeight, _endHeight, num2);
				break;
			}
			_pWidget.height = _endHeight;
			ChangeState(STATE.COMPLETE);
			break;
		}
	}

	private void ChangeState(STATE newState)
	{
		_currentState = newState;
		_stateStartTime = RealTime.time;
	}
}
