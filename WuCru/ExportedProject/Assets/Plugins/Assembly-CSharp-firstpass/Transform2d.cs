using UnityEngine;

public struct Transform2d
{
	private static Transform2d _identity = new Transform2d(Vector2.zero, 0f);

	private Vector2 _xaxis;

	private Vector2 _yaxis;

	public Vector2 _position;

	private float _rotation;

	private bool _isRightHanded;

	public static Transform2d Identity
	{
		get
		{
			return _identity;
		}
	}

	private Vector2 _transposedXaxis
	{
		get
		{
			return new Vector2(_xaxis.x, _yaxis.x);
		}
	}

	private Vector2 _transposedYaxis
	{
		get
		{
			return new Vector2(_xaxis.y, _yaxis.y);
		}
	}

	private bool _setIsRightHanded
	{
		set
		{
			_isRightHanded = value;
			UpdateAxes();
		}
	}

	public Vector2 Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
		}
	}

	public float Rotation
	{
		get
		{
			return _rotation;
		}
		set
		{
			_rotation = value;
			UpdateAxes();
		}
	}

	public Vector2 Right
	{
		get
		{
			return (!_isRightHanded) ? _xaxis : (-_xaxis);
		}
	}

	public Vector2 Left
	{
		get
		{
			return (!_isRightHanded) ? (-_xaxis) : _xaxis;
		}
	}

	public Vector2 Up
	{
		get
		{
			return _yaxis;
		}
	}

	public bool IsRightHanded
	{
		get
		{
			return _isRightHanded;
		}
		set
		{
			_setIsRightHanded = value;
		}
	}

	public bool IsRotated
	{
		get
		{
			return _rotation != 0f;
		}
		set
		{
			if (!value)
			{
				Rotation = 0f;
			}
		}
	}

	public Vector2 this[int i]
	{
		get
		{
			switch (i)
			{
			case 0:
				return _xaxis;
			case 1:
				return _yaxis;
			default:
				return Vector2.zero;
			}
		}
	}

	public float this[int i, int j]
	{
		get
		{
			return this[i][j];
		}
	}

	public Transform2d(Vector2 position, float rotation)
	{
		this = default(Transform2d);
		Position = position;
		Rotation = rotation;
	}

	public void SetIdentity()
	{
		Position = Vector2.zero;
		Rotation = 0f;
	}

	public Vector2 TransformPoint(Vector2 p)
	{
		return (_rotation != 0f || _isRightHanded) ? (_position + p.x * _xaxis + p.y * _yaxis) : (_position + p);
	}

	public Vector2 TransformDirection(Vector2 d)
	{
		return (_rotation != 0f || _isRightHanded) ? (d.x * _xaxis + d.y * _yaxis) : d;
	}

	public Vector2 InverseTransformPoint(Vector2 p)
	{
		return (_rotation != 0f || _isRightHanded) ? ((p.x - _position.x) * _transposedXaxis + (p.y - _position.y) * _transposedYaxis) : (p - _position);
	}

	public Vector2 InverseTransformDirection(Vector2 d)
	{
		return (_rotation != 0f || _isRightHanded) ? (d.x * _transposedXaxis + d.y * _transposedYaxis) : d;
	}

	private void UpdateAxes()
	{
		_xaxis.x = Mathf.Cos(_rotation);
		_xaxis.y = Mathf.Sin(_rotation);
		_yaxis.x = 0f - _xaxis.y;
		_yaxis.y = _xaxis.x;
		if (_isRightHanded)
		{
			_xaxis.x = 0f - _xaxis.x;
			_yaxis.x = 0f - _yaxis.x;
		}
	}

	public override string ToString()
	{
		return string.Format("[{0},{1}]", Position, Rotation);
	}
}
