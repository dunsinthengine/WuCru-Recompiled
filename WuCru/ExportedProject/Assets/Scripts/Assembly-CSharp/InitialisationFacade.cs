using System;
using System.Collections.Generic;
using UnityEngine;

public class InitialisationFacade : MonoBehaviour, JobManager.IOwner
{
	private static InitialisationFacade _instance;

	[SerializeField]
	private bool _orderByNumberPrefix;

	private const bool DO_DEBUG = false;

	private float _queueStartTime;

	private bool _hasInitialised;

	private JobManager _jobManager;

	private List<InitialisationObject> _objects;

	private Action _onFinished;

	string JobManager.IOwner._pName
	{
		get
		{
			return "InitialisationFacade";
		}
	}

	bool JobManager.IOwner._pCanStartJobs
	{
		get
		{
			return _queueStartTime > 0f;
		}
	}

	public static InitialisationFacade Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			_instance = (InitialisationFacade)UnityEngine.Object.FindObjectOfType(typeof(InitialisationFacade));
			if (_instance != null)
			{
				return _instance;
			}
			Log("New facade created.");
			GameObject gameObject = new GameObject("InitialisationFacade");
			_instance = gameObject.AddComponent<InitialisationFacade>();
			return _instance;
		}
	}

	public static bool _pExists
	{
		get
		{
			return _instance != null;
		}
	}

	public bool _pHasFinished
	{
		get
		{
			return _jobManager == null || _jobManager._pHasAllJobsFinished;
		}
	}

	public event Action _pOnFinished
	{
		add
		{
			_onFinished = (Action)Delegate.Combine(_onFinished, value);
		}
		remove
		{
			_onFinished = (Action)Delegate.Remove(_onFinished, value);
		}
	}

	private void Awake()
	{
		init();
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_queueStartTime = Time.realtimeSinceStartup;
		Log("Checking for un-assigned children.");
		for (int i = 0; i < base.transform.childCount; i++)
		{
			InitialisationObject component = base.transform.GetChild(i).gameObject.GetComponent<InitialisationObject>();
			if (!(component == null))
			{
				addToQueue(component);
			}
		}
	}

	private void Update()
	{
		if (_jobManager != null)
		{
			_jobManager.Update();
		}
	}

	public void addToQueue(InitialisationObject obj)
	{
		init();
		if (obj == null)
		{
			return;
		}
		if (_objects != null && !_objects.Contains(obj))
		{
			_objects.Add(obj);
			if (_orderByNumberPrefix)
			{
				DoOrderByNumberPrefix();
			}
		}
		if (_jobManager == null)
		{
			return;
		}
		Log("Added new init job [" + obj.name + "]");
		_jobManager.RegisterJob(obj, false);
		if (obj._initDepends != null)
		{
			foreach (InitialisationObject initDepend in obj._initDepends)
			{
				if (initDepend != null)
				{
					_jobManager.AddDependency(obj, initDepend);
				}
			}
		}
		_jobManager.EnableJob(obj);
	}

	public void removeFromQueue(InitialisationObject obj)
	{
		if (!(obj == null))
		{
			if (_jobManager != null)
			{
				_jobManager.UnregisterJob(obj);
			}
			if (_objects != null && _objects.Contains(obj))
			{
				_objects.Remove(obj);
			}
		}
	}

	private void init()
	{
		if (_hasInitialised)
		{
			return;
		}
		_hasInitialised = true;
		_instance = this;
		_jobManager = new JobManager(this);
		_jobManager._pOnJobStarting += (JobManager.IJob job) =>
		{
			Log("Starting job '" + job._pName + "'");
		};
		_jobManager._pOnJobStarted += (JobManager.IJob job) =>
		{
			Log("Job '" + job._pName + "' started");
		};
		_jobManager._pOnJobFinished += (JobManager.IJob job) =>
		{
			Log("Job '" + job._pName + "' finished");
		};
		_jobManager._pOnAllJobsFinished += () =>
		{
			Log("All jobs finished. Time taken: [" + (Time.realtimeSinceStartup - _queueStartTime) + "] secs.");
			if (_onFinished != null)
			{
				_onFinished();
				_onFinished = null;
			}
		};
		_objects = new List<InitialisationObject>();
	}

	private void DoOrderByNumberPrefix()
	{
		if (_objects == null)
		{
			return;
		}
		_objects.Sort((InitialisationObject a, InitialisationObject b) =>
		{
			string text2 = a.gameObject.name;
			string text3 = b.gameObject.name;
			int length = text2.Length;
			int length2 = text3.Length;
			bool flag2 = length > 0 && text2[0] >= '0' && text2[0] <= '9';
			bool flag3 = length2 > 0 && text3[0] >= '0' && text3[0] <= '9';
			if (!flag2)
			{
				if (flag3)
				{
					return -1;
				}
				return 0;
			}
			if (!flag3)
			{
				return 1;
			}
			int i = 0;
			int j = 0;
			while (true)
			{
				int num2 = 0;
				for (; i < length && text2[i] >= '0' && text2[i] <= '9'; i++)
				{
					num2 *= 10;
					num2 += text2[i] - 48;
				}
				int num3 = 0;
				for (; j < length2 && text3[j] >= '0' && text3[j] <= '9'; j++)
				{
					num3 *= 10;
					num3 += text3[j] - 48;
				}
				if (num2 != num3)
				{
					return num2.CompareTo(num3);
				}
				if (i < length && text2[i] == '.')
				{
					i++;
					if (j < length2 && text3[j] == '.')
					{
						j++;
					}
				}
				else
				{
					if (j >= length2 || text3[j] != '.')
					{
						break;
					}
					j++;
				}
			}
			return 0;
		});
		bool flag = false;
		for (int num = 0; num < _objects.Count; num++)
		{
			if (flag)
			{
				if (!_objects[num]._initDepends.Contains(_objects[num - 1]))
				{
					_objects[num]._initDepends.Add(_objects[num - 1]);
				}
				continue;
			}
			string text = _objects[num].gameObject.name;
			if (text.Length > 0 && text[0] >= '0' && text[0] <= '9')
			{
				flag = true;
			}
		}
	}

	public static void Log(string message, UnityEngine.Object o = null)
	{
	}
}
