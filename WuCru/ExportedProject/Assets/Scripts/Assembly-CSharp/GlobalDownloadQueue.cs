using System;
using AmuzoEngine;
using UnityEngine;

public class GlobalDownloadQueue : AmuzoMonoSingleton<GlobalDownloadQueue>
{
	private const string LOG_TAG = "[GlobalDownloadQueue] ";

	[SerializeField]
	private int _maxConcurrentDownloads = 1;

	private DownloadQueue _downloadQueue;

	public event Action<int> _pOnDownloadStarted;

	public event Action<int> _pOnDownloadEnded;

	public event Action _pOnAllDownloadsEnded;

	private void Update()
	{
		if (_downloadQueue != null)
		{
			_downloadQueue.Update();
		}
	}

	public int Download(DownloadRequest request, object groupId = null)
	{
		int result = -1;
		if (_downloadQueue != null)
		{
			result = _downloadQueue.Download(request, groupId);
		}
		return result;
	}

	public int DownloadNext(DownloadRequest request, object groupId = null)
	{
		int result = -1;
		if (_downloadQueue != null)
		{
			result = _downloadQueue.DownloadNext(request, groupId);
		}
		return result;
	}

	public int DownloadNow(DownloadRequest request, object groupId = null)
	{
		int result = -1;
		if (_downloadQueue != null)
		{
			result = _downloadQueue.DownloadNow(request, groupId);
		}
		return result;
	}

	public void CancelDownload(int downloadId)
	{
		if (_downloadQueue != null)
		{
			_downloadQueue.CancelDownload(downloadId);
		}
	}

	public void CancelDownloads(object groupId = null)
	{
		if (_downloadQueue != null)
		{
			_downloadQueue.CancelDownloads(groupId);
		}
	}

	public bool IsAllDownloadsEnded(object groupId = null)
	{
		return _downloadQueue == null || _downloadQueue.IsAllDownloadsEnded(groupId);
	}

	public void PauseDownloads(bool wantPaused, object requester)
	{
		if (_downloadQueue != null)
		{
			_downloadQueue.PauseDownloads(wantPaused, requester);
		}
	}

	protected override void OnInitialise()
	{
		_downloadQueue = new DownloadQueue("GlobalDownloadQueue");
		_downloadQueue._pOnDownloadStarted += (int downloadId) =>
		{
			if (_pOnDownloadStarted != null)
			{
				_pOnDownloadStarted(downloadId);
			}
		};
		_downloadQueue._pOnDownloadEnded += (int downloadId) =>
		{
			if (_pOnDownloadEnded != null)
			{
				_pOnDownloadEnded(downloadId);
			}
		};
		_downloadQueue._pOnAllDownloadsEnded += () =>
		{
			if (_pOnAllDownloadsEnded != null)
			{
				_pOnAllDownloadsEnded();
			}
		};
		_downloadQueue.SetMaxConcurrentDownloads(_maxConcurrentDownloads);
	}

	protected override bool OnResolveSingletonConflict(GlobalDownloadQueue newInstance)
	{
		_maxConcurrentDownloads = newInstance._maxConcurrentDownloads;
		UnityEngine.Object.Destroy(newInstance.gameObject);
		return true;
	}
}
