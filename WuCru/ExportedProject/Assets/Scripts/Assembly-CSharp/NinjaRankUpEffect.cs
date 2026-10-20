using AmuzoEngine;
using UnityEngine;

public class NinjaRankUpEffect : MonoBehaviour
{
	private enum STATE
	{
		IDLE = 0,
		SHOWING_OLD = 1,
		SHOWING_NEW_CRYSTAL_COUNT = 2,
		GOING_TO_NEW_LEVEL = 3
	}

	public MinifigType _minifigType;

	public UILabel _crystalCount;

	public UILabel _crystalCountShadow;

	public UISprite _crystal;

	public UILabel _level;

	public bool _crystalIconIsGreyscalable;

	private string _currentCrystals;

	private string _currentLevel;

	private string _prevCrystals;

	private string _prevLevel;

	private STATE _currentState;

	private float _stateChangeTime;

	private Vector3 _rootLevelPos;

	private Vector3 _rootCrystalPos;

	private Vector3 _rootCrystalCountPos;

	private bool _hasSetRootPositions;

	public void Display(string currentCrystals, string currentLevel, string prevCrystals, string prevLevel)
	{
		_stateChangeTime = RealTime.time;
		_currentState = STATE.SHOWING_OLD;
		_currentCrystals = currentCrystals;
		_currentLevel = currentLevel;
		_prevCrystals = prevCrystals;
		_prevLevel = prevLevel;
		if (!_hasSetRootPositions)
		{
			_rootLevelPos = _level.transform.localPosition;
			_rootCrystalPos = _crystal.transform.localPosition;
			_rootCrystalCountPos = _crystalCount.transform.localPosition;
		}
		_crystalCount.text = _prevCrystals;
		if (_crystalCountShadow != null)
		{
			_crystalCountShadow.text = _prevCrystals;
		}
		_hasSetRootPositions = true;
		_level.text = _prevLevel;
	}

	public void OnUpdate()
	{
		switch (_currentState)
		{
		case STATE.SHOWING_OLD:
			UpdateShowOld();
			break;
		case STATE.SHOWING_NEW_CRYSTAL_COUNT:
			UpdateShowNewCrystalCount();
			break;
		case STATE.GOING_TO_NEW_LEVEL:
			UpdateShowNew();
			break;
		}
	}

	private void UpdateShowOld()
	{
		float num = RealTime.time - _stateChangeTime;
		if (num > 2f)
		{
			_stateChangeTime = RealTime.time;
			_currentState = STATE.SHOWING_NEW_CRYSTAL_COUNT;
			_level.alpha = 0f;
			_level.text = _currentLevel;
			_crystalCount.alpha = 0f;
			_crystalCount.text = _currentCrystals;
			if (_crystalCountShadow != null)
			{
				_crystalCountShadow.text = _currentCrystals;
			}
		}
		else if (num > 1.5f)
		{
			float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num - 1.5f, 0.5f, 0f, 1f);
			_level.transform.localPosition = _rootLevelPos;
			Vector3 localScale = Vector3.Lerp(Vector3.one, Vector3.one * 4f, num2);
			_level.transform.localScale = localScale;
			_level.alpha = 1f - num2;
			_crystal.transform.localPosition = _rootCrystalPos;
			if (_crystalIconIsGreyscalable)
			{
				_crystal.transform.localScale = localScale;
				Color color = _crystal.color;
				color.r = 1f - num2;
				_crystal.color = color;
			}
			else
			{
				_crystal.transform.localScale = localScale;
				_crystal.alpha = 1f - num2;
			}
			_crystalCount.alpha = 1f - num2;
			_crystalCount.transform.localScale = localScale;
			_crystalCount.transform.localPosition = _rootCrystalCountPos;
			if (_crystalCountShadow != null)
			{
				_crystalCountShadow.alpha = 1f - num2;
			}
		}
		else
		{
			float t = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 1f, 0f, 1f);
			float num3 = Mathf.Sin(Time.time * 90f);
			_level.transform.localPosition = _rootLevelPos + Vector3.right * num3 * Mathf.Lerp(1f, 2f, t);
			_crystal.transform.localPosition = _rootCrystalPos + Vector3.right * num3 * Mathf.Lerp(1f, 2f, t);
			_crystalCount.transform.localPosition = _rootCrystalCountPos + Vector3.right * num3 * Mathf.Lerp(1f, 2f, t);
		}
	}

	private void UpdateShowNewCrystalCount()
	{
		float num = RealTime.time - _stateChangeTime;
		if (num > 0.5f)
		{
			_stateChangeTime = RealTime.time;
			_currentState = STATE.GOING_TO_NEW_LEVEL;
			_level.transform.localPosition = _rootLevelPos;
			if (_crystalIconIsGreyscalable)
			{
				_crystal.color = Color.white;
			}
			else
			{
				_crystal.alpha = 1f;
			}
			_crystal.transform.localScale = Vector3.one;
		}
		else if (num < 0.5f)
		{
			float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			if (_crystalIconIsGreyscalable)
			{
				_crystal.transform.localScale = Vector3.Lerp(Vector3.one * 4f, Vector3.one, num2);
				Color color = _crystal.color;
				color.r = num2;
				_crystal.color = color;
			}
			else
			{
				_crystal.transform.localScale = Vector3.Lerp(Vector3.one * 4f, Vector3.one, num2);
				_crystal.alpha = num2;
			}
			_crystalCount.transform.localScale = Vector3.Lerp(Vector3.one * 4f, Vector3.one, num2);
			_crystalCount.alpha = num2;
			if (_crystalCountShadow != null)
			{
				_crystalCountShadow.alpha = num2;
			}
		}
	}

	private void UpdateShowNew()
	{
		float num = RealTime.time - _stateChangeTime;
		if (num > 0.5f)
		{
			_level.transform.localScale = Vector3.one;
			_level.alpha = 1f;
			_currentState = STATE.IDLE;
		}
		else
		{
			float num2 = AmuzoEngine.Easing.Ease(AmuzoEngine.Easing.EaseType.EaseOutCircle, num, 0.5f, 0f, 1f);
			_level.transform.localScale = Vector3.Lerp(Vector3.one * 4f, Vector3.one, num2);
			_level.alpha = num2;
		}
	}
}
