using UnityEngine;

public struct SimpleTransform
{
	private Vector3 _xaxis;

	private Vector3 _yaxis;

	private Vector3 _zaxis;

	private Vector3 _position;

	private Quaternion _rotation;

	private Vector3 TransposedXaxis
	{
		get
		{
			return new Vector3(_xaxis.x, _yaxis.x, _zaxis.x);
		}
	}

	private Vector3 TransposedYaxis
	{
		get
		{
			return new Vector3(_xaxis.y, _yaxis.y, _zaxis.y);
		}
	}

	private Vector3 TransposedZaxis
	{
		get
		{
			return new Vector3(_xaxis.z, _yaxis.z, _zaxis.z);
		}
	}

	public Vector3 position
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

	public Quaternion rotation
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

	public Vector3 right
	{
		get
		{
			return _xaxis;
		}
	}

	public Vector3 up
	{
		get
		{
			return _yaxis;
		}
	}

	public Vector3 forward
	{
		get
		{
			return _zaxis;
		}
	}

	public Vector3 this[int i]
	{
		get
		{
			switch (i)
			{
			case 0:
				return _xaxis;
			case 1:
				return _yaxis;
			case 2:
				return _zaxis;
			default:
				return Vector3.zero;
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

	public SimpleTransform(Vector3 position, Quaternion rotation)
	{
		this = default(SimpleTransform);
		this.position = position;
		this.rotation = rotation;
	}

	public SimpleTransform(Transform unityTransform)
	{
		this = default(SimpleTransform);
		position = unityTransform.position;
		rotation = unityTransform.rotation;
	}

	public void SetIdentity()
	{
		position = Vector3.zero;
		rotation = Quaternion.identity;
	}

	public Vector3 TransformPoint(Vector3 p)
	{
		return _position + p.x * _xaxis + p.y * _yaxis + p.z * _zaxis;
	}

	public Vector3 TransformDirection(Vector3 d)
	{
		return d.x * _xaxis + d.y * _yaxis + d.z * _zaxis;
	}

	public Vector3 InverseTransformPoint(Vector3 p)
	{
		return (p.x - _position.x) * TransposedXaxis + (p.y - _position.y) * TransposedYaxis + (p.z - _position.z) * TransposedZaxis;
	}

	public Vector3 InverseTransformDirection(Vector3 d)
	{
		return d.x * TransposedXaxis + d.y * TransposedYaxis + d.z * TransposedZaxis;
	}

	private void UpdateAxes()
	{
		_xaxis.x = 1f - 2f * _rotation.y * _rotation.y - 2f * _rotation.z * _rotation.z;
		_xaxis.y = 2f * _rotation.x * _rotation.y + 2f * _rotation.w * _rotation.z;
		_xaxis.z = 2f * _rotation.x * _rotation.z - 2f * _rotation.w * _rotation.y;
		_yaxis.x = 2f * _rotation.x * _rotation.y - 2f * _rotation.w * _rotation.z;
		_yaxis.y = 1f - 2f * _rotation.x * _rotation.x - 2f * _rotation.z * _rotation.z;
		_yaxis.z = 2f * _rotation.y * _rotation.z + 2f * _rotation.w * _rotation.x;
		_zaxis.x = 2f * _rotation.x * _rotation.z + 2f * _rotation.w * _rotation.y;
		_zaxis.y = 2f * _rotation.y * _rotation.z - 2f * _rotation.w * _rotation.x;
		_zaxis.z = 1f - 2f * _rotation.x * _rotation.x - 2f * _rotation.y * _rotation.y;
	}

	public static implicit operator SimpleTransform(Transform src)
	{
		return new SimpleTransform(src);
	}

	public override string ToString()
	{
		return string.Format("[{0},{1},{2},{3}]", _xaxis, _yaxis, _zaxis, _position);
	}
}
