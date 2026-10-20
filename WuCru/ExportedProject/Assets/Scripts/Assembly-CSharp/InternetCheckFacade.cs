using System;
using UnityEngine;

public class InternetCheckFacade : MonoBehaviour
{
	public static bool _pIsReadyForInternetChecks
	{
		get
		{
			return true;
		}
	}

	public static bool _pIsOnline
	{
		get
		{
			if (InternetReachabilityVerifier.Instance == null)
			{
				return false;
			}
			return InternetReachabilityVerifier.Instance.status == InternetReachabilityVerifier.Status.NetVerified;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void RequestInternetCheck(Action<bool> resultCallback)
	{
		if (resultCallback != null)
		{
			resultCallback(_pIsOnline);
		}
	}

	public static void DoRecheckInternetFromFlow()
	{
	}
}
