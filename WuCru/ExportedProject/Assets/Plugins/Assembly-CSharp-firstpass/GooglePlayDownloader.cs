using System;
using System.IO;
using UnityEngine;

public class GooglePlayDownloader
{
	private static string PUBLIC_KEY_DEFAULT;

	private static AndroidJavaClass detectAndroidJNI;

	private static AndroidJavaClass Environment;

	private const string Environment_MEDIA_MOUNTED = "mounted";

	private static string obb_package;

	private static int obb_version;

	static GooglePlayDownloader()
	{
		PUBLIC_KEY_DEFAULT = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhR3P6gETs6pPlBbihcEuPNrMEZG1QQJ17d/DNMNK66KrPRpMgYE9AF9X17akKEiqXpBjHhXdY58sYLtLqD0yUqRfkB1aKoZroHm1Y8F5O1ukZ7LuD+VaG9khv8eEmV1vo8oEw8cczKvMx9P4uHfOWZFP+iElyf2D5E6j1lCho4B5w2HQyiZrKu0W8bJv6lHzbAJ4x9qTopPFrlsmnb2nCEyrSW5X02eLtNDOk/rmquTlZ9xRDjs85gCIfCLzziXScoWIZ8Fxv6FIUUtkBTKbtw+nY5DAIhqMXucPm4YHds/d+aExqz1wMJcqFceSHkh/KqmMwkLnfFjkIrAtETMIfQIDAQAB";
		if (!RunningOnAndroid())
		{
			return;
		}
		Environment = new AndroidJavaClass("android.os.Environment");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
		{
			androidJavaClass.SetStatic("BASE64_PUBLIC_KEY", PUBLIC_KEY_DEFAULT);
			androidJavaClass.SetStatic("SALT", new byte[20]
			{
				1, 43, 244, 255, 54, 98, 156, 244, 43, 2,
				248, 252, 9, 5, 150, 148, 223, 45, 255, 84
			});
		}
	}

	public static bool RunningOnAndroid()
	{
		if (detectAndroidJNI == null)
		{
			detectAndroidJNI = new AndroidJavaClass("android.os.Build");
		}
		return detectAndroidJNI.GetRawClass() != IntPtr.Zero;
	}

	public static string GetExpansionFilePath()
	{
		populateOBBData();
		if (Environment.CallStatic<string>("getExternalStorageState", new object[0]) != "mounted")
		{
			return null;
		}
		using (AndroidJavaObject androidJavaObject = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]))
		{
			string arg = androidJavaObject.Call<string>("getPath", new object[0]);
			return string.Format("{0}/{1}/{2}", arg, "Android/obb", obb_package);
		}
	}

	public static string GetMainOBBPath(string expansionFilePath)
	{
		populateOBBData();
		if (expansionFilePath == null)
		{
			return null;
		}
		string text = string.Format("{0}/main.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static string GetPatchOBBPath(string expansionFilePath)
	{
		populateOBBData();
		if (expansionFilePath == null)
		{
			return null;
		}
		string text = string.Format("{0}/patch.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static void FetchOBB()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.content.Intent", androidJavaObject, new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity"));
			int num = 65536;
			androidJavaObject2.Call<AndroidJavaObject>("addFlags", new object[1] { num });
			androidJavaObject2.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				"unityplayer.Activity",
				androidJavaObject.Call<AndroidJavaObject>("getClass", new object[0]).Call<string>("getName", new object[0])
			});
			androidJavaObject.Call("startActivity", androidJavaObject2);
			if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
			{
				Debug.LogError("Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?");
				AndroidJNI.ExceptionDescribe();
				AndroidJNI.ExceptionClear();
			}
		}
	}

	private static void populateOBBData()
	{
		if (obb_version != 0)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			obb_package = androidJavaObject.Call<string>("getPackageName", new object[0]);
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("getPackageInfo", new object[2] { obb_package, 0 });
			obb_version = androidJavaObject2.Get<int>("versionCode");
		}
	}
}
