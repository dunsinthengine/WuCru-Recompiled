using System;

namespace AmuzoEngine
{
	public class DownloadQueue
	{
		private const string LOG_TAG = "[DownloadQueue] ";

		private ActionQueue _actionQueue;

		private int _maxConcurrent = 1;

		private Func<bool> _canStartNextDownload;

		private bool _isPaused;

		private BooleanStateRequests _isPausedRequests;

		public event Action<int> _pOnDownloadStarted;

		public event Action<int> _pOnDownloadEnded;

		public event Action _pOnAllDownloadsEnded;

		public DownloadQueue(string name)
		{
			_actionQueue = new ActionQueue(name);
			_actionQueue._pOnActionStarted += (int downloadId) =>
			{
				if (_pOnDownloadStarted != null)
				{
					_pOnDownloadStarted(downloadId);
				}
			};
			_actionQueue._pOnActionEnded += (int downloadId) =>
			{
				if (_pOnDownloadEnded != null)
				{
					_pOnDownloadEnded(downloadId);
				}
			};
			_actionQueue._pOnAllActionsEnded += () =>
			{
				if (_pOnAllDownloadsEnded != null)
				{
					_pOnAllDownloadsEnded();
				}
			};
			_canStartNextDownload = () => _maxConcurrent < 0 || _actionQueue._pActiveCount < _maxConcurrent;
			_isPausedRequests = new BooleanStateRequests((bool newState) =>
			{
				_isPaused = newState;
			}, () => _isPaused, false);
		}

		public void SetMaxConcurrentDownloads(int maxConcurrent)
		{
			_maxConcurrent = maxConcurrent;
		}

		public int Download(DownloadRequest request, object groupId = null)
		{
			return AddDownload(request, ActionQueue.EActionStart.LAST, groupId);
		}

		public int DownloadNext(DownloadRequest request, object groupId = null)
		{
			return AddDownload(request, ActionQueue.EActionStart.NEXT, groupId);
		}

		public int DownloadNow(DownloadRequest request, object groupId = null)
		{
			return AddDownload(request, ActionQueue.EActionStart.NOW, groupId);
		}

		public void CancelDownload(int downloadId)
		{
			_actionQueue.CancelAction(downloadId);
		}

		public void CancelDownloads(object groupId = null)
		{
			if (groupId == null)
			{
				_actionQueue.Reset();
			}
			else
			{
				_actionQueue.CancelActions(groupId);
			}
		}

		public bool IsAllDownloadsEnded(object groupId = null)
		{
			return (groupId != null) ? _actionQueue.IsAllActionsEnded(groupId) : _actionQueue._pIsAllActionsEnded;
		}

		public void Update()
		{
			if (!_isPaused)
			{
				_actionQueue.DoActions(_canStartNextDownload);
			}
		}

		public void PauseDownloads(bool wantPaused, object requester)
		{
			_isPausedRequests.RequestState(wantPaused, requester, 0, true);
		}

		private int AddDownload(DownloadRequest request, ActionQueue.EActionStart start, object groupId)
		{
			return _actionQueue.AddAction(() =>
			{
				request.download();
			}, (int downloadId) =>
			{
				request.onSuccessCallBack += (DownloadRequest rq) =>
				{
					_actionQueue.OnActionComplete(downloadId);
				};
				request.onFailCallback += (DownloadRequest rq) =>
				{
					_actionQueue.OnActionComplete(downloadId);
				};
			}, true, request.requestName, groupId, start);
		}
	}
}
