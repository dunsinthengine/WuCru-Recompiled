using UnityEngine;

public class ExternalLogger : InitialisationObject
{
	private static ExternalLogger _instance;

	public static ExternalLogger _pInstance
	{
		get
		{
			if (_instance == null)
			{
				Debug.LogError("EXTERNAL LOGGER NEEDS TO BE SET UP IN THE MAIN SCENE!");
			}
			return _instance;
		}
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		FinaliseLog("OnDestroy");
	}

	private void OnApplicationQuit()
	{
		FinaliseLog("OnApplicationQuit");
	}

	private void OnApplicationFocus(bool isPaused)
	{
	}

	public void FinaliseLog(string endOfFile)
	{
	}

	protected override void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		base.Awake();
		Object.DontDestroyOnLoad(this);
		Screen.sleepTimeout = -1;
		_instance = this;
	}

	public override void startInitialising()
	{
		_currentState = InitialisationState.FINISHED;
	}

	private void OnLog(string message, string stacktrace, LogType type)
	{
	}

	private void LateUpdate()
	{
	}
}
