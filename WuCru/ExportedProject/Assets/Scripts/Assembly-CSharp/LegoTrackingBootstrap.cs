using Cape.LegoSdk.Core;
using UnityEngine;

public class LegoTrackingBootstrap : MonoBehaviour
{
	private static bool _trackingReady;

	private bool _done;

	public static bool _pTrackingReady
	{
		get
		{
			return _trackingReady;
		}
	}

	private void Update()
	{
		if (LegoCoreSdk.Instance.State == CoreSdkState.Initialized && !_done)
		{
			Debug.Log("Initialising Tracking...");
			_done = true;
		}
	}
}
