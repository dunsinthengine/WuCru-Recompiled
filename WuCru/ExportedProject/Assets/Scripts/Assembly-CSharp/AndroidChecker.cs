using System;
using AmuzoEngine;
using UnityEngine;

public class AndroidChecker : InitialisationObject
{
	private class PermissionData
	{
		public string _pPerm { get; set; }

		public bool _pIsPhoneElementsEnabled { get; set; }

		public bool _pIsStorageElementsEnabled { get; set; }
	}

	private const int API_LEVEL_MARSHMALLOW = 23;

	private const string HAS_RUN_MARSHMALLOW_KEY = "HasRunMarsh";

	private const string LOG_TAG = "[Android Checker] ";

	public ScreenConfirmPermission manualPopUp;

	private int tick;

	private bool done;

	public override void startInitialising()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			done = true;
			_currentState = InitialisationState.FINISHED;
			return;
		}
		if (manualPopUp == null)
		{
			_currentState = InitialisationState.FINISHED;
			return;
		}
		_currentState = InitialisationState.INITIALISING;
		PermissionData[] permissionData = new PermissionData[1]
		{
			new PermissionData
			{
				_pPerm = "android.permission.WRITE_EXTERNAL_STORAGE",
				_pIsPhoneElementsEnabled = false,
				_pIsStorageElementsEnabled = true
			}
		};
		Action<string, int, Action, Action> onPermissionResult = (string perm, int result, Action doRetry, Action doNext) =>
		{
			switch (result)
			{
			case 0:
				doNext();
				break;
			case -1:
				doRetry();
				break;
			case -2:
				AndroidFacade.RequestSettingsApp_ApplicationDetails(AnvilBuildInfo._pBundleId, (bool settingsResult) =>
				{
					if (AndroidUtils.IsPermissionGranted(perm))
					{
						doNext();
					}
					else
					{
						Application.Quit();
					}
				});
				break;
			case -3:
				Debug.LogError("[AndroidChecker] Permission request error");
				Application.Quit();
				break;
			}
		};
		Action<int> checkPermission = null;
		checkPermission = (int i) =>
		{
			if (i < permissionData.Length)
			{
				AndroidFacade.DoPermissionFlow(permissionData[i]._pPerm, (int requestCount, Action doRequest) =>
				{
					manualPopUp.gameObject.SetActive(true);
					manualPopUp.SetPhoneElementsEnabled(permissionData[i]._pIsPhoneElementsEnabled);
					manualPopUp.SetStorageElementsEnabled(permissionData[i]._pIsStorageElementsEnabled);
					manualPopUp.SetOnConfirmAction(doRequest);
				}, (int result, Action doRetry) =>
				{
					onPermissionResult(permissionData[i]._pPerm, result, doRetry, () =>
					{
						checkPermission(i + 1);
					});
				});
			}
			else
			{
				done = true;
			}
		};
		checkPermission(0);
	}

	public override InitialisationState updateInitialisation()
	{
		if (done)
		{
			_currentState = InitialisationState.FINISHED;
		}
		return _currentState;
	}
}
