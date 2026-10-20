public class DownloadRequest_LocalOnly : DownloadRequest
{
	public DownloadRequest_LocalOnly()
	{
		doRemoteDownload = false;
		doCacheDownload = false;
		doCacheHeaderInfo = false;
		doLocalDownload = true;
		doLocalHeaderInfo = true;
	}
}
