using UnityEngine;

namespace Cape.Utils.UI
{
	public class Parallax : MonoBehaviour
	{
		[SerializeField]
		private Vector2 _maxShift = Vector2.zero;

		[SerializeField]
		private float _minRotationRate = 0.005f;

		[SerializeField]
		private float _maxRotationRate = 5f;

		[SerializeField]
		private float _rotationRateSmoothing = 0.5f;

		[SerializeField]
		private bool _useEasingFunction;

		[SerializeField]
		private bool _invertEasingFunction;

		[SerializeField]
		private Easing.Function _easingFunction;

		[SerializeField]
		private ParallaxLayer[] _layers;

		private Vector2 _maxDistance;

		private Vector3 _minPosition;

		private Vector3 _maxPosition;

		private Vector2 _minPositionCanvas;

		private Vector2 _maxPositionCanvas;

		private Vector3 _rotationRate;

		private Vector3 _previousRotationRate;

		private Vector3 _layerDelta;

		private Vector3 _layerPosition;

		private Vector2 _movementBoundsRatio;

		private void Start()
		{
			_movementBoundsRatio = Vector2.zero;
			if (_layers[0].AnchoredTransform == null)
			{
				Vector3 vector = default(Vector3);
				vector.Set(_maxShift.x, _maxShift.y, 0f);
				_minPosition = _layers[0].OriginalPosition - vector;
				_maxPosition = _layers[0].OriginalPosition + vector;
				_maxDistance = _maxPosition - _layers[0].OriginalPosition;
			}
			else
			{
				Vector2 vector2 = default(Vector2);
				vector2.Set(_maxShift.x, _maxShift.y);
				_minPositionCanvas = _layers[0].OriginalPositionCanvas - vector2;
				_maxPositionCanvas = _layers[0].OriginalPositionCanvas + vector2;
				_maxDistance = _maxPositionCanvas - _layers[0].OriginalPositionCanvas;
			}
			_rotationRate = default(Vector3);
			Input.gyro.enabled = true;
		}

		private void OnGUI()
		{
			GUI.Label(new Rect(0f, 0f, 500f, 200f), string.Concat(_rotationRate, " / ", Input.gyro.rotationRateUnbiased));
		}

		private void Update()
		{
			ProcessRotationRate();
			if (_rotationRate == Vector3.zero)
			{
				return;
			}
			UpdateMovementBoundsRatio();
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < _layers.Length; i++)
			{
				if (_layers[i].Velocity == Vector2.zero)
				{
					continue;
				}
				UpdateLayerDelta(i);
				if (i == 0)
				{
					if (_layers[i].AnchoredTransform == null)
					{
						_layers[i].transform.Translate(_layerDelta, Space.Self);
						_layerPosition = _layers[i].transform.localPosition;
						_layerPosition.x = Mathf.Clamp(_layerPosition.x, _minPosition.x, _maxPosition.x);
						_layerPosition.y = Mathf.Clamp(_layerPosition.y, _minPosition.y, _maxPosition.y);
						_layerPosition.z = 0f;
						_layers[i].transform.localPosition = _layerPosition;
						if (_layers[0].transform.localPosition.x <= _minPosition.x || _layers[0].transform.localPosition.x >= _maxPosition.x)
						{
							flag = true;
						}
						if (_layers[0].transform.localPosition.y <= _minPosition.y || _layers[0].transform.localPosition.y >= _maxPosition.y)
						{
							flag2 = true;
						}
					}
					else
					{
						Vector2 anchoredPosition = _layers[i].AnchoredTransform.anchoredPosition;
						anchoredPosition.x += _layerDelta.x;
						anchoredPosition.y += _layerDelta.y;
						anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, _minPositionCanvas.x, _maxPositionCanvas.x);
						anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, _minPositionCanvas.y, _maxPositionCanvas.y);
						_layers[i].AnchoredTransform.anchoredPosition = anchoredPosition;
						if (_layers[0].AnchoredTransform.anchoredPosition.x <= _minPositionCanvas.x || _layers[0].AnchoredTransform.anchoredPosition.x >= _maxPositionCanvas.x)
						{
							flag = true;
						}
						if (_layers[0].AnchoredTransform.anchoredPosition.y <= _minPositionCanvas.y || _layers[0].AnchoredTransform.anchoredPosition.y >= _maxPositionCanvas.y)
						{
							flag2 = true;
						}
					}
				}
				else
				{
					if (flag)
					{
						_layerDelta.x = 0f;
					}
					if (flag2)
					{
						_layerDelta.y = 0f;
					}
					if (_layers[i].AnchoredTransform == null)
					{
						_layers[i].transform.Translate(_layerDelta, Space.Self);
						continue;
					}
					Vector2 anchoredPosition2 = _layers[i].AnchoredTransform.anchoredPosition;
					anchoredPosition2.x += _layerDelta.x;
					anchoredPosition2.y += _layerDelta.y;
					_layers[i].AnchoredTransform.anchoredPosition = anchoredPosition2;
				}
			}
		}

		private void ProcessRotationRate()
		{
			_rotationRate = Input.gyro.rotationRateUnbiased * 20f;
			if ((_rotationRate.x > 0f - _minRotationRate && _rotationRate.x < _minRotationRate) || (_rotationRate.y > 0f - _minRotationRate && _rotationRate.y < _minRotationRate))
			{
				_rotationRate = Vector3.zero;
			}
			_rotationRate.x = Mathf.Clamp(_rotationRate.x, 0f - _maxRotationRate, _maxRotationRate);
			_rotationRate.y = Mathf.Clamp(_rotationRate.y, 0f - _maxRotationRate, _maxRotationRate);
			_rotationRate /= _maxRotationRate;
			if (!Mathf.Approximately(_rotationRateSmoothing, 0f) && !Mathf.Approximately(_rotationRateSmoothing, 1f))
			{
				float t = 100f * _rotationRateSmoothing * Time.deltaTime;
				_rotationRate.x = Mathf.Lerp(_previousRotationRate.x, _rotationRate.x, t);
				_rotationRate.y = Mathf.Lerp(_previousRotationRate.y, _rotationRate.y, t);
				_rotationRate.z = 0f;
				_previousRotationRate = _rotationRate;
			}
		}

		private void UpdateMovementBoundsRatio()
		{
			if (_layers[0].AnchoredTransform == null)
			{
				Vector3 vector = _layers[0].transform.localPosition - _layers[0].OriginalPosition;
				vector.x = Mathf.Abs(vector.x);
				vector.y = Mathf.Abs(vector.y);
				if (_maxDistance.x > 0f)
				{
					_movementBoundsRatio.x = vector.x / _maxDistance.x;
				}
				else
				{
					_movementBoundsRatio.x = 0f;
				}
				if (_maxDistance.y > 0f)
				{
					_movementBoundsRatio.y = vector.y / _maxDistance.y;
				}
				else
				{
					_movementBoundsRatio.y = 0f;
				}
			}
			else
			{
				Vector2 vector2 = _layers[0].AnchoredTransform.anchoredPosition - _layers[0].OriginalPositionCanvas;
				vector2.x = Mathf.Abs(vector2.x);
				vector2.y = Mathf.Abs(vector2.y);
				if (_maxDistance.x > 0f)
				{
					_movementBoundsRatio.x = vector2.x / _maxDistance.x;
				}
				else
				{
					_movementBoundsRatio.x = 0f;
				}
				if (_maxDistance.y > 0f)
				{
					_movementBoundsRatio.y = vector2.y / _maxDistance.y;
				}
				else
				{
					_movementBoundsRatio.y = 0f;
				}
			}
			if (_useEasingFunction)
			{
				_movementBoundsRatio.x = Easing.Interpolate(_movementBoundsRatio.x, _easingFunction);
				_movementBoundsRatio.y = Easing.Interpolate(_movementBoundsRatio.y, _easingFunction);
				if (_invertEasingFunction)
				{
					_movementBoundsRatio = Vector2.one - _movementBoundsRatio;
				}
			}
		}

		private void UpdateLayerDelta(int index)
		{
			_layerDelta = Vector3.zero;
			if (_useEasingFunction)
			{
				Vector2 zero = Vector2.zero;
				zero.x = 0.1f * _layers[index].Velocity.x + 0.9f * _layers[index].Velocity.x * _movementBoundsRatio.x;
				zero.y = 0.1f * _layers[index].Velocity.y + 0.9f * _layers[index].Velocity.y * _movementBoundsRatio.y;
				_layerDelta.x = _rotationRate.y * zero.x;
				_layerDelta.y = _rotationRate.x * zero.y;
			}
			else
			{
				_layerDelta.x = _rotationRate.y * _layers[index].Velocity.x;
				_layerDelta.y = _rotationRate.x * _layers[index].Velocity.y;
			}
		}
	}
}
