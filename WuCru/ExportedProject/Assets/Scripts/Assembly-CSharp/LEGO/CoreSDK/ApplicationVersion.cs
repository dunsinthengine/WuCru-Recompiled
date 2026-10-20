using System;

namespace LEGO.CoreSDK
{
	public static class ApplicationVersion
	{
		public static Version Version()
		{
			try
			{
				return new Version("110.11.348");
			}
			catch (Exception ex)
			{
				LEGOSDK.Log.Error("Failed to convert Version Number: '110.11.348' into a valid version. See https://msdn.microsoft.com/en-us/library/y0hf9t2e(v=vs.110).aspx for how to create a valid version string.");
				throw ex;
			}
		}
	}
}
