using UnityEngine;

public class DebugNWCCameraProblem : MonoBehaviour
{
	private float _lastCheckTime;

	private Camera[] _allCameras;

	private GameObject _debugCam;

	private string _hitItems;

	private float _stepSize;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_stepSize = 1f;
	}

	private void OnGUI()
	{
		float num = RealTime.time - _lastCheckTime;
		if (num > 1f)
		{
			_allCameras = Object.FindObjectsOfType<Camera>();
		}
		if (_allCameras == null)
		{
			return;
		}
		int num2 = _allCameras.Length;
		for (int i = 0; i < num2; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(_allCameras[i].name);
			if (GUILayout.Button(_allCameras[i].enabled.ToString(), GUILayout.Height(50f)))
			{
				_allCameras[i].enabled = !_allCameras[i].enabled;
			}
			_allCameras[i].depth = int.Parse(GUILayout.TextField(_allCameras[i].depth.ToString(), GUILayout.Height(50f), GUILayout.Width(50f)));
			if (GUILayout.Button(_allCameras[i].clearFlags.ToString(), GUILayout.Height(50f)))
			{
				switch (_allCameras[i].clearFlags)
				{
				case CameraClearFlags.Color:
					_allCameras[i].clearFlags = CameraClearFlags.Depth;
					break;
				case CameraClearFlags.Depth:
					_allCameras[i].clearFlags = CameraClearFlags.Nothing;
					break;
				case CameraClearFlags.Nothing:
					_allCameras[i].clearFlags = CameraClearFlags.Skybox;
					break;
				case CameraClearFlags.Skybox:
					_allCameras[i].clearFlags = CameraClearFlags.Color;
					break;
				}
			}
			ForceClearCamera component = _allCameras[i].GetComponent<ForceClearCamera>();
			if (component == null)
			{
				if (GUILayout.Button("Add Force Clear", GUILayout.Height(50f)))
				{
					component = _allCameras[i].gameObject.AddComponent<ForceClearCamera>();
					component.clearColor = _allCameras[i].clearFlags == CameraClearFlags.Color || _allCameras[i].clearFlags == CameraClearFlags.Skybox;
					component.clearDepth = _allCameras[i].clearFlags == CameraClearFlags.Depth;
					component.bgColor = Color.blue;
				}
			}
			else if (GUILayout.Button("Remove Force Clear", GUILayout.Height(50f)))
			{
				Object.Destroy(component);
			}
			GUILayout.EndHorizontal();
		}
		if (_debugCam == null)
		{
			if (GUILayout.Button("Create Debug Cam", GUILayout.Height(50f)))
			{
				_debugCam = new GameObject();
				Camera camera = _debugCam.AddComponent<Camera>();
				camera.depth = 900f;
				camera.transform.position = new Vector3(-2.15f, 0f, -3.29f);
				camera.transform.localEulerAngles = new Vector3(0f, 34.44f, 0f);
			}
			return;
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("X", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			Object.Destroy(_debugCam);
		}
		if (GUILayout.Button("<", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.position -= _debugCam.transform.right * _stepSize;
		}
		if (GUILayout.Button(">", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.position += _debugCam.transform.right * _stepSize;
		}
		if (GUILayout.Button("^", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.position -= _debugCam.transform.up * _stepSize;
		}
		if (GUILayout.Button("V", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.position += _debugCam.transform.up * _stepSize;
		}
		if (GUILayout.Button("B", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.position -= _debugCam.transform.forward * _stepSize;
		}
		if (GUILayout.Button("F", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.position += _debugCam.transform.forward * _stepSize;
		}
		if (GUILayout.Button("R>", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.localEulerAngles += new Vector3(0f, 25f, 0f) * _stepSize;
		}
		if (GUILayout.Button("R<", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			_debugCam.transform.localEulerAngles -= new Vector3(0f, 25f, 0f) * _stepSize;
		}
		_stepSize = GUILayout.HorizontalSlider(_stepSize, 0.1f, 5f, GUILayout.Width(200f));
		if (GUILayout.Button("PICK", GUILayout.Height(50f), GUILayout.Width(50f)))
		{
			Camera component2 = _debugCam.GetComponent<Camera>();
			Ray ray = component2.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit[] array = Physics.RaycastAll(ray);
			if (array != null)
			{
				for (int j = 0; j < array.Length; j++)
				{
					_hitItems = _hitItems + array[j].collider.name + "\n";
				}
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Hit Items: " + _hitItems);
		GUI.Box(new Rect(new Vector2(Screen.width / 2 - 1, Screen.height / 2 - 1), new Vector2(2f, 2f)), "X");
	}
}
