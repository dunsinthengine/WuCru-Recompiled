using System;

public static class AppVersionFacade
{
	private const string LOG_TAG = "[AppVersionFacade] ";

	public static string _pAppVersionString
	{
		get
		{
			return AnvilBuildInfo._pBundleVersion;
		}
	}

	public static Version _pAppVersion
	{
		get
		{
			return new Version(_pAppVersionString);
		}
	}
}
