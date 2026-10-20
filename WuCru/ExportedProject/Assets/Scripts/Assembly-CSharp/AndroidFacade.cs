using System;
using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class AndroidFacade : MonoBehaviour
{
	public enum EOnPermissionDeniedAction
	{
		CONTINUE = 0,
		RETRY = 1,
		QUIT = 2,
		SETTINGS = 3
	}

	public delegate void DOnPrePermissionRequest(int requestCount, Action doRequest);

	public delegate void DOnPermissionResult(int result, Action doRetryOnDenied);

	private const string LOG_TAG = "[AndroidFacade] ";

	private const string DEFAULT_PERMISSION_PREFIX = "android.permission.";

	private const string PERMISSION_REQUEST_ID = "request";

	private const string PERMISSION_REQUEST_PERMISSION_COUNT = "permissionCount";

	private const string PERMISSION_REQUEST_PERMISSION = "permission";

	private const string PERMISSION_REQUEST_RESULT = "result";

	public const int PERMISSION_GRANTED = 0;

	public const int PERMISSION_DENIED = -1;

	public const int PERMISSION_DENIED_FINAL = -2;

	public const int PERMISSION_ERROR = -3;

	private static SimpleDataSerializer _sAndroidApiDataSerializer = new SimpleDataSerializer('#', ';', '=', "null");

	private static SimpleIntegerIdGenerator _sPermissionRequestIdGenerator = new SimpleIntegerIdGenerator();

	private static Dictionary<int, Action<int>> _sActivePermissionRequests = new Dictionary<int, Action<int>>();

	private static bool _sIsRequestingSettingsApp = false;

	private static Action<bool> _sOnRequestSettingsAppResult = null;

	private static AndroidJavaClass _javaUnityPlayerClass;

	private static AndroidJavaObject _javaAndroidActivity;

	private static AndroidJavaClass _javaAndroidFacadeClass;

	private static AndroidJavaObject _javaAndroidFacade;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (Application.platform == RuntimePlatform.Android) { InitJavaRefs(); }
	}

	public void OnRequestPermissionResult(string resultData)
	{
		int? requestId = null;
		string[] permissions = null;
		int[] results = null;
		int temp;
		_sAndroidApiDataSerializer.DeserializeRecords(resultData, (int recordCount) =>
		{
			if (recordCount > 0)
			{
				permissions = new string[recordCount - 1];
				results = new int[recordCount - 1];
			}
		}, null, (int recordIndex, int pairIndex, string key, string value) =>
		{
			if (recordIndex == 0)
			{
				if (string.Equals(key, "request", StringComparison.OrdinalIgnoreCase))
				{
					if (int.TryParse(value, out temp))
					{
						requestId = temp;
					}
					else
					{
						Debug.LogError("[AndroidFacade] Cannot parse permission request ID '" + value + "' in OnRequestPermissionResult");
					}
				}
				else if (string.Equals(key, "permissionCount", StringComparison.OrdinalIgnoreCase))
				{
					if (int.TryParse(value, out temp))
					{
						if (temp != permissions.Length)
						{
							Debug.LogError("[AndroidFacade] Permission request permission count '" + temp + "' does not match record count '" + permissions.Length + "' in OnRequestPermissionResult");
						}
					}
					else
					{
						Debug.LogError("[AndroidFacade] Cannot parse permission request permission count '" + value + "' in OnRequestPermissionResult");
					}
				}
				else
				{
					Debug.LogWarning("[AndroidFacade] Unexpected key '" + key + "' and value '" + value + "' in OnRequestPermissionResult");
				}
			}
			else if (string.Equals(key, "permission", StringComparison.OrdinalIgnoreCase))
			{
				permissions[recordIndex - 1] = value;
			}
			else if (string.Equals(key, "result", StringComparison.OrdinalIgnoreCase))
			{
				if (int.TryParse(value, out temp))
				{
					results[recordIndex - 1] = temp;
				}
				else
				{
					Debug.LogError("[AndroidFacade] Cannot parse permission request result '" + value + "' in OnRequestPermissionResult");
				}
			}
			else
			{
				Debug.LogWarning("[AndroidFacade] Unexpected key '" + key + "' and value '" + value + "' in OnRequestPermissionResult");
			}
		});
		if (!requestId.HasValue)
		{
			Debug.LogError("[AndroidFacade] No permission request ID in OnRequestPermissionResult");
		}
		else if (permissions == null || results == null)
		{
			Debug.LogError("[AndroidFacade] No permissions/results in OnRequestPermissionResult");
		}
		else
		{
			ProcessPermissionRequestResult(requestId.Value, permissions, results);
		}
	}

	public static void ProcessPermissionRequestResult(int requestId, IList<string> permissions, IList<int> results)
	{
		if (!_sActivePermissionRequests.ContainsKey(requestId))
		{
			Debug.LogError("[AndroidFacade] Invalid permission request ID '" + requestId + "' in OnRequestPermissionResult");
			return;
		}
		int num = 0;
		for (int i = 0; i < permissions.Count; i++)
		{
			if (num == -2)
			{
				break;
			}
			int num2 = results[i];
			switch (num)
			{
			case 0:
				if (num2 == -1 || num2 == -2)
				{
					num = num2;
				}
				break;
			case -1:
				if (num2 == -2)
				{
					num = num2;
				}
				break;
			}
		}
		Action<int> action = _sActivePermissionRequests[requestId];
		_sActivePermissionRequests.Remove(requestId);
		action(num);
	}

	public static bool IsPermissionGranted(string permission)
	{
		if (string.IsNullOrEmpty(permission))
		{
			return false;
		}
		permission = EnsureFullPermissionId(permission);
		if (_javaAndroidFacade == null)
		{
			Debug.LogError("[AndroidFacade] Can't check permission because _javaAndroidFacade is null");
			return false;
		}
		int num = _javaAndroidFacade.Call<int>("CheckPermission", new object[1] { permission });
		return num == 0;
	}

	public static bool ArePermissionsGranted(IList<string> permissions)
	{
		for (int i = 0; i < permissions.Count; i++)
		{
			if (!string.IsNullOrEmpty(permissions[i]) && !IsPermissionGranted(permissions[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static void DoPermissionFlow(IList<string> perms, DOnPrePermissionRequest onPrePermissionRequest, DOnPermissionResult onPermissionResult)
	{
		if (_javaAndroidFacade == null)
		{
			Debug.LogError("[AndroidFacade] Can't request permissions because _javaAndroidFacade is null");
			onPermissionResult(-3, null);
			return;
		}
		string[] array = new string[perms.Count];
		perms.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = EnsureFullPermissionId(array[i]);
		}
		perms = array;
		if (ArePermissionsGranted(perms))
		{
			onPermissionResult(0, null);
			return;
		}
		Action startRequest = null;
		Action<int> onResult = (int result) =>
		{
			onPermissionResult(result, (result != -1) ? null : startRequest);
		};
		int requestCount = 0;
		Action doRequest = () =>
		{
			int pNew = _sPermissionRequestIdGenerator._pNew;
			_sActivePermissionRequests.Add(pNew, onResult);
			requestCount++;
			string[] array2 = new string[perms.Count];
			perms.CopyTo(array2, 0);
			Debug.Log("[AndroidFacade] " + string.Format("Calling CheckRequestPermissions with request ID {0}, and {1} perms ({2}, ...)", pNew, array2.Length, array2[0]));
			_javaAndroidFacade.Call("CheckRequestPermissions", pNew, array2);
		};
		startRequest = () =>
		{
			if (onPrePermissionRequest != null)
			{
				onPrePermissionRequest(requestCount, doRequest);
			}
			else
			{
				doRequest();
			}
		};
		startRequest();
	}

	public static void DoPermissionFlow(string perm, DOnPrePermissionRequest onPrePermissionRequest, DOnPermissionResult onPermissionResult)
	{
		List<string> list = new List<string>();
		list.Add(perm);
		DoPermissionFlow(list, onPrePermissionRequest, onPermissionResult);
	}

	public static string EnsureFullPermissionId(string perm)
	{
		if (perm.Contains("."))
		{
			return perm;
		}
		return "android.permission." + perm;
	}

	public void OnRequestSettingsAppResult(string resultData)
	{
		bool wasSettingsAppLaunched = false;
		int resultCode = -1;
		_sAndroidApiDataSerializer.DeserializePairs(resultData, null, (int pairIndex, string key, string value) =>
		{
			if (string.Equals(key, "wasLaunched", StringComparison.OrdinalIgnoreCase))
			{
				bool.TryParse(value, out wasSettingsAppLaunched);
			}
			else if (string.Equals(key, "resultCode", StringComparison.OrdinalIgnoreCase))
			{
				int.TryParse(value, out resultCode);
			}
		});
		ProcessSettingsAppRequestResult(wasSettingsAppLaunched, resultCode);
	}

	public static void RequestSettingsApp(string settingsAction, Action<bool> onResult)
	{
		if (InitSettingsAppRequest(onResult))
		{
			_javaAndroidFacade.Call("RequestSettingsApp", settingsAction);
		}
	}

	public static void RequestSettingsApp_ApplicationDetails(string applicationPackageId, Action<bool> onResult)
	{
		if (InitSettingsAppRequest(onResult))
		{
			_javaAndroidFacade.Call("RequestSettingsApp_ApplicationDetails", applicationPackageId);
		}
	}

	private static void InitJavaRefs()
	{
		if (Application.platform != RuntimePlatform.Android) return;
		_javaUnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		_javaAndroidActivity = _javaUnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
		_javaAndroidFacadeClass = new AndroidJavaClass("com.amuzo.androidfacade.AndroidFacade");
		_javaAndroidFacade = _javaAndroidFacadeClass.CallStatic<AndroidJavaObject>("Create", new object[1] { _javaAndroidActivity });
	}

	private static bool InitSettingsAppRequest(Action<bool> onResult)
	{
		if (_sIsRequestingSettingsApp)
		{
			if (onResult != null)
			{
				onResult(false);
			}
			return false;
		}
		_sIsRequestingSettingsApp = true;
		_sOnRequestSettingsAppResult = onResult;
		return true;
	}

	private static void ProcessSettingsAppRequestResult(bool wasLaunched, int resultCode)
	{
		_sIsRequestingSettingsApp = false;
		Action<bool> sOnRequestSettingsAppResult = _sOnRequestSettingsAppResult;
		_sOnRequestSettingsAppResult = null;
		if (sOnRequestSettingsAppResult != null)
		{
			sOnRequestSettingsAppResult(wasLaunched && resultCode == 0);
			sOnRequestSettingsAppResult = null;
		}
	}
}
