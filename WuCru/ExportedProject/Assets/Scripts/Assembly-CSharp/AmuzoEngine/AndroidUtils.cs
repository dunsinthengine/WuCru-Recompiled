using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace AmuzoEngine
{
	public static class AndroidUtils
	{
		private const string LOG_TAG = "[AndroidUtils] ";

		public static bool IsPermissionGranted(string perm)
		{
			if (Application.platform != RuntimePlatform.Android) return true;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.support.v4.content.ContextCompat");
			int num = androidJavaClass2.CallStatic<int>("checkSelfPermission", new object[2] { androidJavaObject, perm });
			return num == 0;
		}

		public static void RequestPermissions(IList<string> perms, int requestId)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			string[] array = new string[perms.Count];
			perms.CopyTo(array, 0);
			Debug.Log("[AndroidUtils] " + string.Format("Calling requestPermissions with {0} perms ({1}, ...), and request ID {2}", array.Length, array[0], requestId));
			androidJavaObject.Call("requestPermissions", array, requestId);
		}

		public static bool AreNotificationsEnabled()
		{
			if (Application.platform != RuntimePlatform.Android) return true;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.support.v4.app.NotificationManagerCompat");
			AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("from", new object[1] { androidJavaObject });
			return androidJavaObject2.Call<bool>("areNotificationsEnabled", new object[0]);
		}

		public static string GetDeviceUniqueIdentifier_ReadPhoneState(bool oldBehaviour = true)
		{
			if (Application.platform != RuntimePlatform.Android) return SystemInfo.deviceUniqueIdentifier;
			string text = string.Empty;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.content.Context");
			string text2 = androidJavaClass2.GetStatic<string>("TELEPHONY_SERVICE");
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getSystemService", new object[1] { text2 });
			bool flag = false;
			try
			{
				text = androidJavaObject2.Call<string>("getDeviceId", new object[0]);
			}
			catch (Exception)
			{
				flag = true;
			}
			if (text == null)
			{
				text = string.Empty;
			}
			if ((flag && !oldBehaviour) || (!flag && text == string.Empty && oldBehaviour))
			{
				AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("android.provider.Settings$Secure");
				string text3 = androidJavaClass3.GetStatic<string>("ANDROID_ID");
				AndroidJavaObject androidJavaObject3 = androidJavaObject.Call<AndroidJavaObject>("getContentResolver", new object[0]);
				text = androidJavaClass3.CallStatic<string>("getString", new object[2] { androidJavaObject3, text3 });
				if (text == null)
				{
					text = string.Empty;
				}
			}
			if (text == string.Empty)
			{
				string text4 = "00000000000000000000000000000000";
				try
				{
					StreamReader streamReader = new StreamReader("/sys/class/net/wlan0/address");
					text4 = streamReader.ReadLine();
					streamReader.Close();
				}
				catch (Exception)
				{
				}
				text = text4.Replace(":", string.Empty);
			}
			return GetMd5Hash(text);
		}

		private static string GetMd5Hash(string input)
		{
			if (input == string.Empty)
			{
				return string.Empty;
			}
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
