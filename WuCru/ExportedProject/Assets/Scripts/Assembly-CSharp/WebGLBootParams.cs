using UnityEngine;

public class WebGLBootParams : InitialisationObject
{
	private const string LOG_TAG = "[WebGLBootParams] ";

	private const string DEFAULT_JAVASCRIPT_GET_BOOT_PARAMS = "GetBootParams";

	private const string DEFAULT_EDITOR_BOOT_PARAMS = "lang=EN";

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
