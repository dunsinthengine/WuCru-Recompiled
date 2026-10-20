using UnityEngine;

public class ScreenAdWarning : MonoBehaviour
{
	private const int STARTING_NUMBER = 3;

	private const float TIME_PER_NUMBER = 1f;

	private const float SPEED_SPIN = 360f;

	public GameObject _spinner;

	public UILabel _numberLabel;

	public ScreenObjectTransitionWidget _noNoSign;

	private int _currentNumber;

	private float _timerNumber;

	private Vector3 _rotation = default(Vector3);

	private void OnEnable()
	{
		_currentNumber = 3;
		_timerNumber = 1f;
		if (_numberLabel != null)
		{
			_numberLabel.text = _currentNumber.ToString();
		}
		if (_noNoSign != null)
		{
			_noNoSign.TweenIn();
		}
		PlaySfx();
	}

	private void Update()
	{
		UpdateNumber();
		UpdateSpinner();
	}

	private void UpdateNumber()
	{
		if (_currentNumber <= 0)
		{
			return;
		}
		_timerNumber -= Time.deltaTime;
		if (_timerNumber < 0f)
		{
			_timerNumber = 1f;
			_currentNumber--;
			if (_numberLabel != null)
			{
				_numberLabel.text = _currentNumber.ToString();
			}
			PlaySfx();
		}
	}

	private void UpdateSpinner()
	{
		if (!(_spinner == null))
		{
			_rotation.Set(0f, 0f, 360f * Time.deltaTime);
			_spinner.transform.Rotate(_rotation);
		}
	}

	private void PlaySfx()
	{
		AudioPlayer.PlaySound("ui/celebration/countup/tick");
	}
}
