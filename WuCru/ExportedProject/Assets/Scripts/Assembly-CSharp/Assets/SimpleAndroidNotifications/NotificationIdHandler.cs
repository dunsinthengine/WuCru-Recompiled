using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	public static class NotificationIdHandler
	{
		private const string PlayerPrefsKey = "NotificationHelper.Scheduled";

		public static List<int> GetScheduledNotificaions()
		{
			return (!PlayerPrefs.HasKey("NotificationHelper.Scheduled")) ? new List<int>() : (from i in PlayerPrefs.GetString("NotificationHelper.Scheduled").Split('|')
				where i != string.Empty
				select i).Select(int.Parse).ToList();
		}

		public static void SetScheduledNotificaions(List<int> scheduledNotificaions)
		{
			PlayerPrefs.SetString("NotificationHelper.Scheduled", string.Join("|", scheduledNotificaions.Select((int i) => i.ToString()).ToArray()));
		}

		public static void AddScheduledNotificaion(int notificationId)
		{
			List<int> scheduledNotificaions = GetScheduledNotificaions();
			scheduledNotificaions.Add(notificationId);
			SetScheduledNotificaions(scheduledNotificaions);
		}

		public static void RemoveScheduledNotificaion(int id)
		{
			List<int> scheduledNotificaions = GetScheduledNotificaions();
			scheduledNotificaions.RemoveAll((int i) => i == id);
			SetScheduledNotificaions(scheduledNotificaions);
		}

		public static int GetNotificationId()
		{
			List<int> scheduledNotificaions = GetScheduledNotificaions();
			int num;
			do
			{
				num = Random.Range(0, int.MaxValue);
			}
			while (scheduledNotificaions.Contains(num));
			return num;
		}
	}
}
