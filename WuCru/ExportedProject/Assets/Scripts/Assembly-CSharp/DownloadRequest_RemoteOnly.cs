public class DownloadRequest_RemoteOnly : DownloadRequest
{
	public DownloadRequest_RemoteOnly()
	{
		doRemoteDownload = true;
		doCacheDownload = false;
		doCacheHeaderInfo = false;
		doLocalDownload = false;
		doLocalHeaderInfo = false;
	}
}
