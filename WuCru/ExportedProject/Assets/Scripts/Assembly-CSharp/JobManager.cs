using System;
using System.Collections.Generic;

public class JobManager
{
	private enum EJobState
	{
		NULL = 0,
		WAITING = 1,
		RUNNING = 2,
		FINISHED = 3
	}

	public interface IOwner
	{
		string _pName { get; }

		bool _pCanStartJobs { get; }
	}

	public interface IJob
	{
		string _pName { get; }

		bool _pIsFinished { get; }

		bool Start();

		void Update();
	}

	private class JobData
	{
		public EJobState _state;

		public List<JobData> _dependencies;

		public bool _pHasUnfinishedDependencies
		{
			get
			{
				return _dependencies != null && _dependencies.Count > 0 && _dependencies.Exists((JobData d) => d._state != EJobState.FINISHED);
			}
		}

		public JobData()
		{
			_state = EJobState.NULL;
			_dependencies = new List<JobData>();
		}
	}

	private const string LOG_TAG = "[JobManager] ";

	private IOwner _owner;

	private List<IJob> _jobs;

	private Dictionary<IJob, JobData> _jobDataDict;

	private Action<IJob> _onJobStarting;

	private Action<IJob> _onJobStarted;

	private Action<IJob> _onJobFinished;

	private Action _onAllJobsFinished;

	private int _numUnfinishedJobs;

	private string _pOwnerName
	{
		get
		{
			return (_owner == null) ? "[no owner]" : _owner._pName;
		}
	}

	private string _pLogTag
	{
		get
		{
			return "[JobManager:" + _pOwnerName + "] ";
		}
	}

	public bool _pHasAllJobsFinished
	{
		get
		{
			return _numUnfinishedJobs == 0;
		}
	}

	public event Action<IJob> _pOnJobStarting
	{
		add
		{
			_onJobStarting = (Action<IJob>)Delegate.Combine(_onJobStarting, value);
		}
		remove
		{
			_onJobStarting = (Action<IJob>)Delegate.Remove(_onJobStarting, value);
		}
	}

	public event Action<IJob> _pOnJobStarted
	{
		add
		{
			_onJobStarted = (Action<IJob>)Delegate.Combine(_onJobStarted, value);
		}
		remove
		{
			_onJobStarted = (Action<IJob>)Delegate.Remove(_onJobStarted, value);
		}
	}

	public event Action<IJob> _pOnJobFinished
	{
		add
		{
			_onJobFinished = (Action<IJob>)Delegate.Combine(_onJobFinished, value);
		}
		remove
		{
			_onJobFinished = (Action<IJob>)Delegate.Remove(_onJobFinished, value);
		}
	}

	public event Action _pOnAllJobsFinished
	{
		add
		{
			_onAllJobsFinished = (Action)Delegate.Combine(_onAllJobsFinished, value);
		}
		remove
		{
			_onAllJobsFinished = (Action)Delegate.Remove(_onAllJobsFinished, value);
		}
	}

	public JobManager(IOwner owner)
	{
		_owner = owner;
		_jobs = new List<IJob>();
		_jobDataDict = new Dictionary<IJob, JobData>();
		_onJobStarting = null;
		_onJobStarted = null;
		_onJobFinished = null;
		_onAllJobsFinished = null;
		_numUnfinishedJobs = 0;
	}

	private JobManager()
	{
	}

	public void RegisterJob(IJob job, bool enable)
	{
		if (_jobs != null && job != null)
		{
			if (!_jobs.Contains(job))
			{
				_jobs.Add(job);
				_jobDataDict[job] = new JobData();
			}
			if (enable)
			{
				EnableJob(job);
			}
		}
	}

	public void UnregisterJob(IJob job)
	{
		if (job != null)
		{
			RemoveDependency(job);
			if (_jobs != null && _jobs.Contains(job))
			{
				_jobs.Remove(job);
			}
			if (_jobDataDict != null && _jobDataDict.ContainsKey(job))
			{
				_jobDataDict.Remove(job);
			}
		}
	}

	public void AddDependency(IJob job, IJob dep)
	{
		if (job == null || dep == null)
		{
			return;
		}
		JobData jobData = FindJobData(job);
		if (jobData != null)
		{
			JobData jobData2 = FindJobData(dep);
			if (jobData2 == null)
			{
				RegisterJob(dep, false);
				jobData2 = FindJobData(dep);
			}
			if (jobData2 != null && !jobData._dependencies.Contains(jobData2))
			{
				jobData._dependencies.Add(jobData2);
			}
		}
	}

	public void RemoveDependency(IJob job, IJob dep)
	{
		if (job == null || dep == null)
		{
			return;
		}
		JobData jobData = FindJobData(job);
		if (jobData != null)
		{
			JobData jobData2 = FindJobData(dep);
			if (jobData2 != null && jobData._dependencies.Contains(jobData2))
			{
				jobData._dependencies.Remove(jobData2);
			}
		}
	}

	public void EnableJob(IJob job)
	{
		if (job != null)
		{
			JobData jobData = FindJobData(job);
			if (jobData != null && jobData._state == EJobState.NULL)
			{
				jobData._state = EJobState.WAITING;
			}
		}
	}

	public void Update()
	{
		UpdateJobs();
	}

	private JobData FindJobData(IJob job)
	{
		return (_jobDataDict == null || !_jobDataDict.ContainsKey(job)) ? null : _jobDataDict[job];
	}

	private void RemoveDependency(IJob dep)
	{
		if (_jobs == null)
		{
			return;
		}
		for (int i = 0; i < _jobs.Count; i++)
		{
			if (_jobs[i] != null && _jobs[i] != dep)
			{
				RemoveDependency(_jobs[i], dep);
			}
		}
	}

	private void CheckStartJob(IJob job, ref JobData jd)
	{
		if (_owner != null && !_owner._pCanStartJobs)
		{
			return;
		}
		if (jd == null)
		{
			jd = FindJobData(job);
			if (jd == null)
			{
				return;
			}
		}
		if (!CanJobStart(job, ref jd))
		{
			return;
		}
		if (_onJobStarting != null)
		{
			_onJobStarting(job);
		}
		if (job.Start())
		{
			jd._state = EJobState.RUNNING;
			_numUnfinishedJobs++;
			if (_onJobStarted != null)
			{
				_onJobStarted(job);
			}
		}
	}

	private bool CanJobStart(IJob job, ref JobData jd)
	{
		if (jd == null)
		{
			jd = FindJobData(job);
			if (jd == null)
			{
				return false;
			}
		}
		if (jd._state != EJobState.WAITING)
		{
			return false;
		}
		return !jd._pHasUnfinishedDependencies;
	}

	private void UpdateJob(IJob job, ref JobData jd)
	{
		if (jd == null)
		{
			jd = FindJobData(job);
			if (jd == null)
			{
				return;
			}
		}
		CheckStartJob(job, ref jd);
		if (jd._state != EJobState.RUNNING)
		{
			return;
		}
		job.Update();
		if (job._pIsFinished)
		{
			jd._state = EJobState.FINISHED;
			if (_onJobFinished != null)
			{
				_onJobFinished(job);
			}
		}
	}

	private void UpdateJobs()
	{
		int numUnfinishedJobs = _numUnfinishedJobs;
		int num = 0;
		for (int i = 0; i < _jobs.Count; i++)
		{
			JobData jd = FindJobData(_jobs[i]);
			if (jd != null && jd._state != EJobState.FINISHED)
			{
				UpdateJob(_jobs[i], ref jd);
				if (jd._state != EJobState.FINISHED)
				{
					num++;
				}
			}
		}
		_numUnfinishedJobs = num;
		if (num == 0 && numUnfinishedJobs != 0 && _onAllJobsFinished != null)
		{
			_onAllJobsFinished();
		}
	}
}
