using System;
using System.Linq;
using System.Text;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

public class LocalPushNotificationFacade : AmuzoMonoSingleton<LocalPushNotificationFacade>, InitialisationProxy.ITarget
{
	private const string LOG_PREFIX = "LPNFacade: ";

	private const bool DO_DEBUG = false;

	private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";

	private const string MainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";

	private InitialisationState _currentState;

	public bool _pAreLocalNotificationsEnabled
	{
		get
		{
			return AreNotificationsEnabled();
		}
	}

	public InitialisationState _pInitialisationState
	{
		get
		{
			return _currentState;
		}
		set
		{
			_currentState = value;
		}
	}

	public InitialisationState OnUpdateInitialising()
	{
		return _currentState;
	}

	public void OnStartInitialising()
	{
		AddDebugMenu();
		_currentState = InitialisationState.FINISHED;
	}

	private void OnApplicationPause(bool pause)
	{
	}

	public void EnablePushNotifications()
	{
		if (!_pAreLocalNotificationsEnabled)
		{
			OpenAndroidNotificationSettingsPage();
		}
	}

	public void iOS8RegistrationConfirmation(string message)
	{
	}

	public void DisablePushNotifications()
	{
		OpenAndroidNotificationSettingsPage();
	}

	public void ScheduleBasicNotification(int seconds, string body)
	{
		if (_pAreLocalNotificationsEnabled)
		{
			SendCustomAndroidNotification(new NotificationParams
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = new TimeSpan(0, 0, 0, seconds, 0),
				Title = string.Empty,
				Message = body,
				Ticker = body,
				Sound = true,
				Vibrate = true,
				Light = true,
				SmallIcon = NotificationIcon.Event,
				SmallIconColor = Color.cyan,
				LargeIcon = string.Empty,
				Mode = NotificationExecuteMode.Schedule
			});
		}
	}

	public void ScheduleNotification(int seconds, string title, string body, Color smallIconColor, NotificationIcon smallIcon = NotificationIcon.Event, string largeIcon = "app_icon")
	{
		if (_pAreLocalNotificationsEnabled)
		{
			SendCustomAndroidNotification(new NotificationParams
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = new TimeSpan(0, 0, 0, seconds, 0),
				Title = title,
				Message = body,
				Ticker = body,
				Sound = true,
				Vibrate = true,
				Light = true,
				SmallIcon = smallIcon,
				SmallIconColor = smallIconColor,
				LargeIcon = largeIcon,
				Mode = NotificationExecuteMode.Schedule
			});
		}
	}

	public void CancelAllNotifications()
	{
		if (Application.platform != RuntimePlatform.Android) return;
		new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("CancelAllNotifications");
	}

	private void SendCustomAndroidNotification(NotificationParams notificationParams)
	{
		if (Application.platform != RuntimePlatform.Android) return;
		long num = (long)notificationParams.Delay.TotalMilliseconds;
		string text = string.Join(",", notificationParams.Vibration.Select((int i) => i.ToString()).ToArray());
		new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("SetNotification", notificationParams.Id, num, notificationParams.Title, notificationParams.Message, notificationParams.Ticker, notificationParams.Sound ? 1 : 0, notificationParams.Vibrate ? 1 : 0, text, notificationParams.Light ? 1 : 0, notificationParams.LightOnMs, notificationParams.LightOffMs, ColotToInt(notificationParams.LightColor), notificationParams.LargeIcon, GetSmallIconName(notificationParams.SmallIcon), ColotToInt(notificationParams.SmallIconColor), (int)notificationParams.Mode, "com.unity3d.player.UnityPlayerNativeActivity");
		NotificationIdHandler.AddScheduledNotificaion(notificationParams.Id);
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

	public static void OpenAndroidNotificationSettingsPage()
	{
		if (Application.platform != RuntimePlatform.Android) return;
		new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("OpenNotificationSettings");
	}

	public bool AreNotificationsEnabled()
	{
		return AreAndroidNotificationsEnabled();
	}

	public static bool AreIOSNotificationsEnabled()
	{
		return true;
	}

	public static bool AreAndroidNotificationsEnabled()
	{
		if (Application.platform != RuntimePlatform.Android) return true;
		return new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic<bool>("AreNotificationsEnabled", new object[0]);
	}

	private bool isIOS8()
	{
		return false;
	}

	private void AddDebugMenu()
	{
		AmuzoDebugMenu newMenu = new AmuzoDebugMenu("LOCAL NOTIF. FACADE");
		Func<string> textAreaFunction = () =>
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(AmuzoDebugMenu.NEW_LINE);
			stringBuilder.Append("Local Notification Facade:");
			stringBuilder.Append(AmuzoDebugMenu.NEW_LINE);
			stringBuilder.Append(AmuzoDebugMenu.NEW_LINE);
			stringBuilder.Append("Has notification permission? [");
			stringBuilder.Append(AreAndroidNotificationsEnabled());
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		};
		newMenu.AddInfoTextFunction(textAreaFunction);
		newMenu.AddButton(new AmuzoDebugMenuButton(newMenu, "Enable Notifications", () =>
		{
			EnablePushNotifications();
			newMenu.RefreshText();
		}));
		newMenu.AddButton(new AmuzoDebugMenuButton(newMenu, "Open Notif. Settings", () =>
		{
			OpenAndroidNotificationSettingsPage();
		}));
		newMenu.AddButton(new AmuzoDebugMenuButton(newMenu, "Basic Notification 5sec", () =>
		{
			ScheduleBasicNotification(5, "Basic notification body text");
			newMenu.RefreshText();
		}));
		newMenu.AddButton(new AmuzoDebugMenuButton(newMenu, "Notification 5sec", () =>
		{
			ScheduleNotification(5, "Notification title text", "Notification body text", Color.cyan);
			newMenu.RefreshText();
		}));
		newMenu.AddButton(new AmuzoDebugMenuButton(newMenu, "Notification 1min", () =>
		{
			ScheduleNotification(60, "Notification title text", "Notification body text", Color.cyan);
			newMenu.RefreshText();
		}));
		AmuzoDebugMenuManager.RegisterRootDebugMenu(newMenu);
	}
}
