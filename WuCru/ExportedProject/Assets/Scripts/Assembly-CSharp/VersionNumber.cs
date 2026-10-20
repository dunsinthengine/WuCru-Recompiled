public static class VersionNumber
{
	public static readonly int vmajor = 1;

	public static readonly int vminor = 2;

	public static readonly int vrevision;

	public static readonly string dateIOS = "9/29/2016 1:46:40 PM";

	public static readonly string dateAndroid = "9/29/2016 1:49:51 PM";

	public static readonly string dateStandalone = "12/3/2015 7:52:06 PM";

	public static readonly string dateWebplayer = "12/3/2015 7:52:06 PM";

	public static readonly int buildCountIOS;

	public static readonly int buildCountAndroid = 1;

	public static readonly int buildCountStandalone;

	public static readonly int buildCountWebplayer;

	public static int buildCount
	{
		get
		{
			return buildCountAndroid;
		}
	}

	public static string date
	{
		get
		{
			return dateAndroid;
		}
	}

	public static string versionString
	{
		get
		{
			return vmajor + "." + vminor + "." + vrevision + " b" + buildCount;
		}
	}
}
