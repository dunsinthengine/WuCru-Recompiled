using System;
using System.Net;
using UnityEngine;

public class NetworkUtils
{
	private class MyWebClient : WebClient
	{
		protected override WebRequest GetWebRequest(Uri uri)
		{
			WebRequest webRequest = base.GetWebRequest(uri);
			webRequest.Timeout = 5000;
			return webRequest;
		}
	}

	public static bool HasNetworkConnection()
	{
		bool flag = false;
		bool flag2 = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
		bool flag3 = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
		if ((flag2 || flag3) && CanLoadWebPage())
		{
			flag = true;
		}
		if (flag)
		{
			Debug.Log("Internet connection : OK");
		}
		else
		{
			Debug.LogWarning("Internet connection : NONE");
		}
		return flag;
	}

	private static bool CanLoadWebPage()
	{
		bool result = true;
		try
		{
			using (MyWebClient myWebClient = new MyWebClient())
			{
				using (myWebClient.OpenRead("http://www.google.com"))
				{
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
			result = false;
		}
		return result;
	}
}
