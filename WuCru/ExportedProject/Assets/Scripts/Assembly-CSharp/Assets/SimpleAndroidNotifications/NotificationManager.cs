using System;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	public static class NotificationManager
	{
		private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";

		private const string MainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";

		public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = NotificationIcon.Bell)
		{
			NotificationParams notificationParams = new NotificationParams();
			notificationParams.Id = NotificationIdHandler.GetNotificationId();
			notificationParams.Delay = delay;
			notificationParams.Title = title;
			notificationParams.Message = message;
			notificationParams.Ticker = message;
			notificationParams.Sound = true;
			notificationParams.Vibrate = true;
			notificationParams.Light = true;
			notificationParams.SmallIcon = smallIcon;
			notificationParams.SmallIconColor = smallIconColor;
			notificationParams.LargeIcon = string.Empty;
			notificationParams.Mode = NotificationExecuteMode.Schedule;
			return SendCustom(notificationParams);
		}

		public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = NotificationIcon.Bell)
		{
			NotificationParams notificationParams = new NotificationParams();
			notificationParams.Id = NotificationIdHandler.GetNotificationId();
			notificationParams.Delay = delay;
			notificationParams.Title = title;
			notificationParams.Message = message;
			notificationParams.Ticker = message;
			notificationParams.Sound = true;
			notificationParams.Vibrate = true;
			notificationParams.Light = true;
			notificationParams.SmallIcon = smallIcon;
			notificationParams.SmallIconColor = smallIconColor;
			notificationParams.LargeIcon = "app_icon";
			notificationParams.Mode = NotificationExecuteMode.Schedule;
			return SendCustom(notificationParams);
		}

		public static int SendCustom(NotificationParams notificationParams)
		{
			if (Application.platform != RuntimePlatform.Android) return notificationParams.Id;
			Debug.Log("Simple Android Notifications: SendCustom");
			long num = (long)notificationParams.Delay.TotalMilliseconds;
			string text = string.Join(",", notificationParams.Vibration.Select((int i) => i.ToString()).ToArray());
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("SetNotification", notificationParams.Id, num, notificationParams.Title, notificationParams.Message, notificationParams.Ticker, notificationParams.Sound ? 1 : 0, notificationParams.Vibrate ? 1 : 0, text, notificationParams.Light ? 1 : 0, notificationParams.LightOnMs, notificationParams.LightOffMs, ColotToInt(notificationParams.LightColor), notificationParams.LargeIcon, GetSmallIconName(notificationParams.SmallIcon), ColotToInt(notificationParams.SmallIconColor), (int)notificationParams.Mode, "com.unity3d.player.UnityPlayerNativeActivity");
			NotificationIdHandler.AddScheduledNotificaion(notificationParams.Id);
			return notificationParams.Id;
		}

		public static void Cancel(int id)
		{
			if (Application.platform != RuntimePlatform.Android) return;
			Debug.Log("Simple Android Notifications: Cancel");
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("CancelNotification", id);
			NotificationIdHandler.RemoveScheduledNotificaion(id);
		}

		public static void CancelAll()
		{
			if (Application.platform != RuntimePlatform.Android) return;
			Debug.Log("Simple Android Notifications: CancelAll");
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("CancelAllNotifications");
		}

		private static int ColotToInt(Color color)
		{
			Color32 color2 = color;
			return color2.r * 65536 + color2.g * 256 + color2.b;
		}

		private static string GetSmallIconName(NotificationIcon icon)
		{
			return "anp_" + icon.ToString().ToLower();
		}
	}
}
