using System;

namespace AmuzoEngine
{
	public class JobCounter
	{
		private int _count;

		private Action _onBegin;

		private Action _onEnd;

		private Action _onNextEnd;

		public bool _pHasJobs
		{
			get
			{
				return _count > 0;
			}
		}

		public JobCounter(Action onBegin, Action onEnd)
		{
			_count = 0;
			_onBegin = onBegin;
			_onEnd = onEnd;
			_onNextEnd = null;
		}

		public JobCounter(Action onEnd)
		{
			_count = 0;
			_onBegin = null;
			_onEnd = onEnd;
			_onNextEnd = null;
		}

		public void BeginJob(Action onEnd = null)
		{
			if (_count == 0)
			{
				_onNextEnd = null;
				if (_onBegin != null)
				{
					_onBegin();
				}
			}
			if (onEnd != null)
			{
				_onNextEnd = (Action)Delegate.Combine(_onNextEnd, onEnd);
			}
			_count++;
		}

		public void EndJob()
		{
			if (_count <= 0)
			{
				return;
			}
			_count--;
			if (_count == 0)
			{
				if (_onNextEnd != null)
				{
					Action onNextEnd = _onNextEnd;
					_onNextEnd = null;
					onNextEnd();
				}
				if (_onEnd != null)
				{
					_onEnd();
				}
			}
		}
	}
}
