using UnityEngine;

public class WebConfigParams : InitialisationObject
{
	private const string LOG_TAG = "[WebConfigParams] ";

	private const string DEFAULT_JAVASCRIPT_GET_BOOT_PARAMS = "GetConfigParams";

	private const string DEFAULT_EDITOR_BOOT_PARAMS = "lang=EN";

	private static string _bootParams;

	[SerializeField]
	private string _editorBootParams = "lang=EN";

	protected override void Awake()
	{
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public override void startInitialising()
	{
		_currentState = InitialisationState.FINISHED;
	}
}
