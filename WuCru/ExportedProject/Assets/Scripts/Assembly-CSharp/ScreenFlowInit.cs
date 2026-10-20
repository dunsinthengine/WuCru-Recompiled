using UnityEngine;

public class ScreenFlowInit : InitialisationObject
{
	private const string LOG_TAG = "[ScreenFlowInit] ";

	[SerializeField]
	private string _screenName;

	[SerializeField]
	private bool _doClearHistory;

	public override void startInitialising()
	{
		_currentState = InitialisationState.INITIALISING;
		if (Facades<ScreenFacade>.Instance != null)
		{
			Facades<ScreenFacade>.Instance.ShowScreen(_screenName, _doClearHistory);
			_currentState = InitialisationState.FINISHED;
		}
		else
		{
			Debug.LogError("[ScreenFlowInit] Screen Facade not found!");
		}
	}
}
