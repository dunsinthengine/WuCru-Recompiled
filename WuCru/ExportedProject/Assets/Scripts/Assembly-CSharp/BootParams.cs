using UnityEngine;

public static class BootParams
{
	private const string LOG_TAG = "[BootParams] ";

	private static string PREPEND_FILE_PATH = "file:///";

	private static string FULL_DATA_PATH = PREPEND_FILE_PATH + Application.dataPath;

	public static string GetBootParam(string value, bool doValidate = true)
	{
		return null;
	}

	private static string ValidateURL(string url)
	{
		url = WWW.UnEscapeURL(url);
		if (!url.StartsWith("http"))
		{
			return string.Format("{0}/{1}", FULL_DATA_PATH.Replace("%20", " "), url);
		}
		return url;
	}
}
