public class AmuzoDebugMenu_BuildInfo : AmuzoDebugMenu
{
	public AmuzoDebugMenu_BuildInfo(string menuName)
		: base(menuName)
	{
		AddInfoTextFunction(GetDebugText, false);
	}

	private string GetDebugText()
	{
		return "AppName: " + AnvilBuildInfo._pAppName + AmuzoDebugMenu.NEW_LINE + "BambooVersion: " + AnvilBuildInfo._pBambooVersion + AmuzoDebugMenu.NEW_LINE + "BuildDate: " + AnvilBuildInfo._pBuildDate + AmuzoDebugMenu.NEW_LINE + "BuildId: " + AnvilBuildInfo._pBuildId + AmuzoDebugMenu.NEW_LINE + "BundleId: " + AnvilBuildInfo._pBundleId + AmuzoDebugMenu.NEW_LINE + "BundleVersion: " + AnvilBuildInfo._pBundleVersion + AmuzoDebugMenu.NEW_LINE + "Certificate: " + AnvilBuildInfo._pCertificate + AmuzoDebugMenu.NEW_LINE + "Defines: " + AnvilBuildInfo._pDefines + AmuzoDebugMenu.NEW_LINE + "DeviceCapabilities: " + AnvilBuildInfo._pDeviceCapabilities + AmuzoDebugMenu.NEW_LINE + "OsVersion: " + AnvilBuildInfo._pOsVersion + AmuzoDebugMenu.NEW_LINE + "Platform: " + AnvilBuildInfo._pPlatform + AmuzoDebugMenu.NEW_LINE + "ProjectCode: " + AnvilBuildInfo._pProjectCode + AmuzoDebugMenu.NEW_LINE + "SplitApk: " + AnvilBuildInfo._pSplitApk + AmuzoDebugMenu.NEW_LINE + "Title: " + AnvilBuildInfo._pTitle + AmuzoDebugMenu.NEW_LINE + "Version: " + AnvilBuildInfo._pVersion + AmuzoDebugMenu.NEW_LINE;
	}
}
