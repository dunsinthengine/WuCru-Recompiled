using System.Collections.Generic;
using UnityEngine;

public class InitialisationObject : MonoBehaviour, JobManager.IJob
{
	public List<InitialisationObject> _initDepends;

	protected InitialisationState _currentState;

	string JobManager.IJob._pName
	{
		get
		{
			return base.name;
		}
	}

	bool JobManager.IJob._pIsFinished
	{
		get
		{
			return _currentState == InitialisationState.FINISHED;
		}
	}

	public InitialisationState _pInitialisationState
	{
		get
		{
			return _currentState;
		}
	}

	protected virtual void Awake()
	{
		InitialisationFacade.Instance.addToQueue(this);
		_currentState = InitialisationState.WAITING_TO_START;
	}

	protected virtual void OnDestroy()
	{
		_currentState = InitialisationState.FINISHED;
		if (InitialisationFacade._pExists)
		{
			InitialisationFacade.Instance.removeFromQueue(this);
		}
	}

	public virtual void startInitialising()
	{
	}

	public virtual InitialisationState updateInitialisation()
	{
		return _currentState;
	}

	bool JobManager.IJob.Start()
	{
		startInitialising();
		return _currentState != InitialisationState.WAITING_TO_START;
	}

	void JobManager.IJob.Update()
	{
		_currentState = updateInitialisation();
	}
}
