public class DownloadRequest_RemoteWCaching : DownloadRequest
{
	public DownloadRequest_RemoteWCaching()
	{
		doRemoteDownload = true;
		doCacheDownload = true;
		doCacheHeaderInfo = true;
		doLocalDownload = false;
		doLocalHeaderInfo = false;
	}
}
