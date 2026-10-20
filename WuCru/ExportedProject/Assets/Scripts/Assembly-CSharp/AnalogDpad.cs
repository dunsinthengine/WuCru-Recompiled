using System;
using UnityEngine;

public class AnalogDpad
{
	private const string LOG_TAG = "[AnalogDpad] ";

	private const string HORZ_AXIS_DEFAULT = "Horizontal";

	private const string VERT_AXIS_DEFAULT = "Vertical";

	private const float PRESS_VALUE_THRESHOLD_DEFAULT = 0.9f;

	private string _horzAxis;

	private string _vertAxis;

	private float _pressValueThreshold = 0.9f;

	private bool _isPressedUp;

	private bool _isPressedDown;

	private bool _isPressedLeft;

	private bool _isPressedRight;

	private bool _wasPressedUp;

	private bool _wasPressedDown;

	private bool _wasPressedLeft;

	private bool _wasPressedRight;

	public bool _pIsPressedUp
	{
		get
		{
			return _isPressedUp;
		}
	}

	public bool _pIsJustPressedUp
	{
		get
		{
			return _isPressedUp && !_wasPressedUp;
		}
	}

	public bool _pIsJustReleasedUp
	{
		get
		{
			return !_isPressedUp && _wasPressedUp;
		}
	}

	public bool _pIsPressedDown
	{
		get
		{
			return _isPressedDown;
		}
	}

	public bool _pIsJustPressedDown
	{
		get
		{
			return _isPressedDown && !_wasPressedDown;
		}
	}

	public bool _pIsJustReleasedDown
	{
		get
		{
			return !_isPressedDown && _wasPressedDown;
		}
	}

	public bool _pIsPressedLeft
	{
		get
		{
			return _isPressedLeft;
		}
	}

	public bool _pIsJustPressedLeft
	{
		get
		{
			return _isPressedLeft && !_wasPressedLeft;
		}
	}

	public bool _pIsJustReleasedLeft
	{
		get
		{
			return !_isPressedLeft && _wasPressedLeft;
		}
	}

	public bool _pIsPressedRight
	{
		get
		{
			return _isPressedRight;
		}
	}

	public bool _pIsJustPressedRight
	{
		get
		{
			return _isPressedRight && !_wasPressedRight;
		}
	}

	public bool _pIsJustReleasedRight
	{
		get
		{
			return !_isPressedRight && _wasPressedRight;
		}
	}

	public event Action _pOnJustPressedUp;

	public event Action _pOnJustReleasedUp;

	public event Action _pOnJustPressedDown;

	public event Action _pOnJustReleasedDown;

	public event Action _pOnJustPressedLeft;

	public event Action _pOnJustReleasedLeft;

	public event Action _pOnJustPressedRight;

	public event Action _pOnJustReleasedRight;

	public AnalogDpad(string horzAxis = "Horizontal", string vertAxis = "Vertical", float pressValueThreshold = 0.9f)
	{
		_horzAxis = horzAxis;
		_vertAxis = vertAxis;
		_pressValueThreshold = pressValueThreshold;
	}

	public void OnUpdate()
	{
		_wasPressedUp = _isPressedUp;
		_wasPressedDown = _isPressedDown;
		_wasPressedLeft = _isPressedLeft;
		_wasPressedRight = _isPressedRight;
		float axis = Input.GetAxis(_horzAxis);
		float axis2 = Input.GetAxis(_vertAxis);
		_isPressedUp = axis2 >= _pressValueThreshold;
		_isPressedDown = axis2 <= 0f - _pressValueThreshold;
		_isPressedLeft = axis <= 0f - _pressValueThreshold;
		_isPressedRight = axis >= _pressValueThreshold;
		if (_pIsJustPressedUp && _pOnJustPressedUp != null)
		{
			_pOnJustPressedUp();
		}
		if (_pIsJustReleasedUp && _pOnJustReleasedUp != null)
		{
			_pOnJustReleasedUp();
		}
		if (_pIsJustPressedDown && _pOnJustPressedDown != null)
		{
			_pOnJustPressedDown();
		}
		if (_pIsJustReleasedDown && _pOnJustReleasedDown != null)
		{
			_pOnJustReleasedDown();
		}
		if (_pIsJustPressedLeft && _pOnJustPressedLeft != null)
		{
			_pOnJustPressedLeft();
		}
		if (_pIsJustReleasedLeft && _pOnJustReleasedLeft != null)
		{
			_pOnJustReleasedLeft();
		}
		if (_pIsJustPressedRight && _pOnJustPressedRight != null)
		{
			_pOnJustPressedRight();
		}
		if (_pIsJustReleasedRight && _pOnJustReleasedRight != null)
		{
			_pOnJustReleasedRight();
		}
	}
}
