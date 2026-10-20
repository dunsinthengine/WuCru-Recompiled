using UnityEngine;

public class PrintDebug : MonoBehaviour
{
	public UILabel _debug;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Application.logMessageReceived += HandleLog;
	}

	private void HandleLog(string str, string stack, LogType type)
	{
		if (type == LogType.Error)
		{
			_debug.text = "\n[FF0000]" + str + "[-]" + _debug.text;
		}
		else
		{
			_debug.text = "\n" + str + _debug.text;
		}
		_debug.text = _debug.text.Substring(0, 1000);
	}
}
