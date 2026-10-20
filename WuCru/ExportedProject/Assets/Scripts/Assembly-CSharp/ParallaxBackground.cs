using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
	public Vector3 _deviceOrientation = new Vector3(0f, 0f, 0f);

	public Vector3 _maxTilt = new Vector3(0.5f, 0.5f, 0f);

	public Vector3 _midgroundMaxMovement = new Vector3(25f, 25f, 0f);

	public Vector3 _midBackMaxMovement = new Vector3(50f, 50f, 0f);

	public Vector3 _foregroundMaxMovement = new Vector3(150f, 150f, 0f);

	public Vector3 _farBackMaxMovement = new Vector3(150f, 150f, 0f);

	public float _speed = 3f;

	[SerializeField]
	private GameObject _midGround;

	[SerializeField]
	private GameObject _midGround2;

	[SerializeField]
	private GameObject _midBackGround;

	[SerializeField]
	private GameObject _backGround;

	[SerializeField]
	private GameObject _farBack;

	private Vector3 _midgroundStart;

	private Vector3 _midground2Start;

	private Vector3 _backgroundStart;

	private Vector3 _midBackStart;

	private Vector3 _farBackStart;

	private Vector3 _orientationDiff = new Vector3(0f, 0f, 0f);

	private void Start()
	{
		_midgroundStart = _midGround.transform.localPosition;
		_midground2Start = _midGround2.transform.localPosition;
		_midBackStart = _midBackGround.transform.localPosition;
		_backgroundStart = _backGround.transform.localPosition;
		_farBackStart = _farBack.transform.localPosition;
	}

	private void Update()
	{
		_deviceOrientation = Input.acceleration;
		_deviceOrientation += _orientationDiff;
		float num = Mathf.Clamp(_deviceOrientation.x * 4f, 0f - _maxTilt.x, _maxTilt.x);
		float num2 = Mathf.Clamp(_deviceOrientation.y * 4f, 0f - _maxTilt.y, _maxTilt.y);
		float b = ((!(num > 0f)) ? Mathf.Lerp(_midgroundStart.x, _midgroundMaxMovement.x, Mathf.Abs(num)) : Mathf.Lerp(_midgroundStart.x, 0f - _midgroundMaxMovement.x, num));
		float b2 = ((!(num2 > 0f)) ? Mathf.Lerp(_midgroundStart.y, _midgroundMaxMovement.y, Mathf.Abs(num2)) : Mathf.Lerp(_midgroundStart.y, 0f - _midgroundMaxMovement.y, num2));
		float x = Mathf.Lerp(_midGround.transform.localPosition.x, b, Time.deltaTime * _speed);
		float y = Mathf.Lerp(_midGround.transform.localPosition.y, b2, Time.deltaTime * _speed);
		_midGround.transform.localPosition = new Vector3(x, y, 0f);
		float b3 = ((!(num > 0f)) ? Mathf.Lerp(_midground2Start.x, _midgroundMaxMovement.x, Mathf.Abs(num)) : Mathf.Lerp(_midground2Start.x, 0f - _midgroundMaxMovement.x, num));
		float b4 = ((!(num2 > 0f)) ? Mathf.Lerp(_midground2Start.y, _midgroundMaxMovement.y, Mathf.Abs(num2)) : Mathf.Lerp(_midground2Start.y, 0f - _midgroundMaxMovement.y, num2));
		float x2 = Mathf.Lerp(_midGround2.transform.localPosition.x, b3, Time.deltaTime * _speed);
		float y2 = Mathf.Lerp(_midGround2.transform.localPosition.y, b4, Time.deltaTime * _speed);
		_midGround2.transform.localPosition = new Vector3(x2, y2, 0f);
		float b5 = ((!(num > 0f)) ? Mathf.Lerp(_backgroundStart.x, _foregroundMaxMovement.x, Mathf.Abs(num)) : Mathf.Lerp(_backgroundStart.x, 0f - _foregroundMaxMovement.x, num));
		float b6 = ((!(num2 > 0f)) ? Mathf.Lerp(_backgroundStart.y, _foregroundMaxMovement.y, Mathf.Abs(num2)) : Mathf.Lerp(_backgroundStart.y, 0f - _foregroundMaxMovement.y, num2));
		float x3 = Mathf.Lerp(_backGround.transform.localPosition.x, b5, Time.deltaTime * _speed);
		float y3 = Mathf.Lerp(_backGround.transform.localPosition.y, b6, Time.deltaTime * _speed);
		_backGround.transform.localPosition = new Vector3(x3, y3, 0f);
		float b7 = ((!(num > 0f)) ? Mathf.Lerp(_farBackStart.x, _farBackMaxMovement.x, Mathf.Abs(num)) : Mathf.Lerp(_farBackStart.x, 0f - _farBackMaxMovement.x, num));
		float b8 = ((!(num2 > 0f)) ? Mathf.Lerp(_farBackStart.y, _farBackMaxMovement.y, Mathf.Abs(num2)) : Mathf.Lerp(_farBackStart.y, 0f - _farBackMaxMovement.y, num2));
		float x4 = Mathf.Lerp(_farBack.transform.localPosition.x, b7, Time.deltaTime * _speed);
		float y4 = Mathf.Lerp(_farBack.transform.localPosition.y, b8, Time.deltaTime * _speed);
		_farBack.transform.localPosition = new Vector3(x4, y4, 0f);
		float b9 = ((!(num > 0f)) ? Mathf.Lerp(_midBackStart.x, _midBackMaxMovement.x, Mathf.Abs(num)) : Mathf.Lerp(_midBackStart.x, 0f - _midBackMaxMovement.x, num));
		float b10 = ((!(num2 > 0f)) ? Mathf.Lerp(_midBackStart.y, _midBackMaxMovement.y, Mathf.Abs(num2)) : Mathf.Lerp(_midBackStart.y, 0f - _midBackMaxMovement.y, num2));
		float x5 = Mathf.Lerp(_midBackGround.transform.localPosition.x, b9, Time.deltaTime * _speed);
		float y5 = Mathf.Lerp(_midBackGround.transform.localPosition.y, b10, Time.deltaTime * _speed);
		_midBackGround.transform.localPosition = new Vector3(x5, y5, 0f);
	}

	public void SetDefaultOrientation()
	{
		_orientationDiff = -Input.acceleration;
	}
}
