using System;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

public class WuCruNotifications
{
	public static bool _wantDebugDuration;

	public static long _lastIssueTime;

	private int _targetTime;

	private float _issueDelay;

	public WuCruNotifications()
	{
		if (AmuzoMonoSingleton<AmuzoDebugMenuManager>._pExists)
		{
			AmuzoDebugMenu amuzoDebugMenu = new AmuzoDebugMenu("CITY LOCAL NOTIFICATIONS");
			amuzoDebugMenu.AddInfoTextFunction(() => string.Empty);
			amuzoDebugMenu.AddButton(new AmuzoDebugMenuButton(amuzoDebugMenu, "SET SCHEDULE TIME TO 0", () =>
			{
				_wantDebugDuration = true;
			}));
			amuzoDebugMenu.AddButton(new AmuzoDebugMenuButton(amuzoDebugMenu, "SET SCHEDULE TIME TO NORMAL", () =>
			{
				_wantDebugDuration = false;
			}));
			AmuzoDebugMenuManager.RegisterRootDebugMenu(amuzoDebugMenu);
		}
	}

	public static void LoadLastIssueTime()
	{
		if (PlayerPrefs.HasKey("NotificationIssueTime"))
		{
			string text = PlayerPrefs.GetString("NotificationIssueTime");
			Debug.Log("Last Notification Issue Time Was: " + text);
			_lastIssueTime = DateTime.Parse(text).Ticks;
		}
	}

	public void GenerateNotifications()
	{
		if (!AmuzoMonoSingleton<LocalPushNotificationFacade>._pExists)
		{
			Debug.LogWarning("LocalPushNotificationFacade does not exist!");
		}
		LocalPushNotificationFacade pInstance = AmuzoMonoSingleton<LocalPushNotificationFacade>._pInstance;
		if (pInstance == null)
		{
			Debug.LogWarning("No LocalPushNotificationFacade");
			return;
		}
		DateTime now = DateTime.Now;
		if (_lastIssueTime == 0 || now.Ticks <= _lastIssueTime)
		{
			DayOfWeek dayOfWeek = now.DayOfWeek;
			int num = 86400;
			int num2 = 0;
			switch (dayOfWeek)
			{
			case DayOfWeek.Monday:
				num2 = num * 5;
				break;
			case DayOfWeek.Tuesday:
				num2 = num * 4;
				break;
			case DayOfWeek.Wednesday:
				num2 = num * 3;
				break;
			case DayOfWeek.Thursday:
				num2 = num * 2;
				break;
			case DayOfWeek.Friday:
				num2 = num;
				break;
			case DayOfWeek.Saturday:
				num2 = num * 7;
				break;
			case DayOfWeek.Sunday:
				num2 = num * 6;
				break;
			}
			int num3 = 12 - now.Hour;
			num2 += num3 * 60 * 60;
			int minute = now.Minute;
			num2 -= minute * 60;
			int second = now.Second;
			num2 -= now.Second;
			DateTime dateTime = now;
			if (!_wantDebugDuration)
			{
				dateTime = dateTime.AddSeconds(num2);
			}
			else
			{
				dateTime = now.AddSeconds(5.0);
				num2 = 5;
			}
			if (pInstance._pAreLocalNotificationsEnabled)
			{
				Debug.Log("Issuing The Notification");
				_lastIssueTime = dateTime.Ticks;
				PlayerPrefs.SetString("NotificationIssueTime", dateTime.ToString());
				PlayerPrefs.Save();
				pInstance.CancelAllNotifications();
				_targetTime = num2;
				_issueDelay = 1f;
			}
		}
	}

	public void TryIssueMessage()
	{
		if (_targetTime != 0)
		{
			if (!AmuzoMonoSingleton<LocalPushNotificationFacade>._pExists)
			{
				Debug.LogWarning("LocalPushNotificationFacade does not exist!");
			}
			LocalPushNotificationFacade pInstance = AmuzoMonoSingleton<LocalPushNotificationFacade>._pInstance;
			if (pInstance == null)
			{
				Debug.LogWarning("No LocalPushNotificationFacade");
				return;
			}
			if (_issueDelay > 0f)
			{
				_issueDelay -= RealTime.deltaTime;
				return;
			}
			pInstance.ScheduleNotification(Mathf.Max(5, _targetTime - 1), Localise("LocalNotification.Title"), Localise("LocalNotification.Body"), new Color32(31, 121, 220, byte.MaxValue), NotificationIcon.Brick);
			_targetTime = 0;
		}
	}

	public void InvalidateLastIssueTime()
	{
		_lastIssueTime = 0L;
		PlayerPrefs.DeleteKey("NotificationIssueTime");
		PlayerPrefs.Save();
	}

	protected string Localise(string key)
	{
		string text = LocalisationFacade.Instance.GetString(key);
		if (text == string.Empty || text == null)
		{
			return key;
		}
		return text;
	}
}
