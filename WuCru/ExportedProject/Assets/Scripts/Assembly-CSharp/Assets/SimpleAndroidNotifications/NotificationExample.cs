using System;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	public class NotificationExample : MonoBehaviour
	{
		public void OnGUI()
		{
			if (GUILayout.Button("Simple 5 sec", GUILayout.Height((float)Screen.height * 0.2f), GUILayout.Width(Screen.width)))
			{
				NotificationManager.Send(TimeSpan.FromSeconds(5.0), "Simple notification", "Customize icon and color", new Color(1f, 0.3f, 0.15f));
			}
			if (GUILayout.Button("Normal 5 sec", GUILayout.Height((float)Screen.height * 0.2f), GUILayout.Width(Screen.width)))
			{
				NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5.0), "Notification", "Notification with app icon", new Color(0f, 0.6f, 1f), NotificationIcon.Message);
			}
			if (GUILayout.Button("Custom 5 sec", GUILayout.Height((float)Screen.height * 0.2f), GUILayout.Width(Screen.width)))
			{
				NotificationParams notificationParams = new NotificationParams();
				notificationParams.Id = NotificationIdHandler.GetNotificationId();
				notificationParams.Delay = TimeSpan.FromSeconds(5.0);
				notificationParams.Title = "Custom notification";
				notificationParams.Message = "Message";
				notificationParams.Ticker = "Ticker";
				notificationParams.Sound = true;
				notificationParams.Vibrate = true;
				notificationParams.Vibration = new int[6] { 500, 500, 500, 500, 500, 500 };
				notificationParams.Light = true;
				notificationParams.LightOnMs = 1000;
				notificationParams.LightOffMs = 1000;
				notificationParams.LightColor = Color.red;
				notificationParams.SmallIcon = NotificationIcon.Biohazard;
				notificationParams.SmallIconColor = new Color(0f, 0.5f, 0f);
				notificationParams.LargeIcon = "app_icon";
				notificationParams.Mode = NotificationExecuteMode.Schedule;
				NotificationParams notificationParams2 = notificationParams;
				NotificationManager.SendCustom(notificationParams2);
			}
			if (GUILayout.Button("Cancel all", GUILayout.Height((float)Screen.height * 0.2f), GUILayout.Width(Screen.width)))
			{
				NotificationManager.CancelAll();
			}
		}
	}
}
