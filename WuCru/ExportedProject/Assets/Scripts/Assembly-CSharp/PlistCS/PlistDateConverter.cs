using System;

namespace PlistCS
{
	public static class PlistDateConverter
	{
		public static long timeDifference = 978307200L;

		public static long GetAppleTime(long unixTime)
		{
			return unixTime - timeDifference;
		}

		public static long GetUnixTime(long appleTime)
		{
			return appleTime + timeDifference;
		}

		public static DateTime ConvertFromAppleTimeStamp(double timestamp)
		{
			return new DateTime(2001, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
		}

		public static double ConvertToAppleTimeStamp(DateTime date)
		{
			DateTime dateTime = new DateTime(2001, 1, 1, 0, 0, 0, 0);
			return Math.Floor((date - dateTime).TotalSeconds);
		}
	}
}
