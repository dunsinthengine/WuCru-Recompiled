using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnvilBuildInfo
{
	private static List<string> keys;

	private static List<string> values;

	private static string data;

	private static int _width = -1;

	private static int _height = -1;

	public static string _pBuildDate
	{
		get
		{
			return GetVar("BUILD_DATE", "1985-08-26 01:20:00");
		}
	}

	public static string _pBambooVersion
	{
		get
		{
			return GetVar("BAMBOO_VERSION", "0");
		}
	}

	public static string _pProject
	{
		get
		{
			return GetVar("PROJECT", "OURTEC");
		}
	}

	public static string _pProjectCode
	{
		get
		{
			return GetVar("PROJECT_CODE", "OURTEC");
		}
	}

	public static string _pBuildId
	{
		get
		{
			return GetVar("BUILD_ID", "Internal");
		}
	}

	public static string _pTitle
	{
		get
		{
			return GetVar("TITLE", "Project Title");
		}
	}

	public static string _pPlatform
	{
		get
		{
			return GetVar("PLATFORM", "Web");
		}
	}

	public static string _pOsVersion
	{
		get
		{
			return GetVar("OS_VERSION", string.Empty);
		}
	}

	public static string _pVersion
	{
		get
		{
			return GetVar("VERSION", "0");
		}
	}

	public static string _pBundleVersion
	{
		get
		{
			return GetVar("BUNDLE_VERSION", "0");
		}
	}

	public static string _pDeviceCapabilities
	{
		get
		{
			return GetVar("DEVICE_CAPS", string.Empty);
		}
	}

	public static int _pWidth
	{
		get
		{
			if (_width != -1)
			{
				return _width;
			}
			string var = GetVar("WIDTH", Screen.width.ToString());
			_width = Screen.width;
			int.TryParse(var, out _width);
			return _width;
		}
	}

	public static int _pHeight
	{
		get
		{
			if (_height != -1)
			{
				return _height;
			}
			string var = GetVar("HEIGHT", Screen.height.ToString());
			_height = Screen.height;
			int.TryParse(var, out _height);
			return _height;
		}
	}

	public static string _pColour
	{
		get
		{
			return GetVar("COLOR", "#000");
		}
	}

	public static string _pBundleId
	{
		get
		{
			return GetVar("BUNDLE_ID", string.Empty);
		}
	}

	public static string _pCertificate
	{
		get
		{
			return GetVar("CERTIFICATE", string.Empty);
		}
	}

	public static string _pAppName
	{
		get
		{
			return GetVar("APP_NAME", "OURTEC");
		}
	}

	public static bool _pSplitApk
	{
		get
		{
			bool result = false;
			bool.TryParse(GetVar("SPLIT_APK", "false"), out result);
			return result;
		}
	}

	public static string _pDefines
	{
		get
		{
			return GetVar("DEFINES", string.Empty);
		}
	}

	public static void Initialize(string data)
	{
		string[] array = data.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
		keys = new List<string>();
		values = new List<string>();
		AnvilBuildInfo.data = data;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=');
			if (array2.Length >= 1)
			{
				keys.Add(array2[0]);
				Debug.Log("Anvil added key: " + array2[0]);
				if (array2.Length < 2)
				{
					values.Add(string.Empty);
					continue;
				}
				values.Add(array2[1]);
				Debug.Log("Anvil added value: " + array2[1]);
			}
		}
	}

	private static void Initialize()
	{
		if (keys == null)
		{
			TextAsset textAsset = Resources.Load("BuildInfo") as TextAsset;
			if (textAsset == null)
			{
				Initialize(string.Empty);
			}
			else
			{
				Initialize(textAsset.text);
			}
		}
	}

	public static string GetVar(string key, string defaultValue)
	{
		Initialize();
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i] == key)
			{
				if (values[i] == string.Empty)
				{
					return defaultValue;
				}
				return values[i];
			}
		}
		return defaultValue;
	}

	public static string GetDefine(string partialKey, string defaultValue)
	{
		Initialize();
		string var = GetVar("DEFINES", string.Empty);
		string[] array = var.Split(' ');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Contains(partialKey))
			{
				return array[i].Substring(partialKey.Length);
			}
		}
		return string.Empty;
	}

	public static string GetData()
	{
		return data;
	}
}
