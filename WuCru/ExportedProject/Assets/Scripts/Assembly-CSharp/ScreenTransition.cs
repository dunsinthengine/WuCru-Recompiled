using System;
using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class ScreenTransition
{
	public enum EStage
	{
		NULL = 0,
		OUT = 1,
		IN = 2
	}

	private const string LOG_TAG = "[ScreenTransition] ";

	private ScreenFlow _screenFlow;

	private ScreenBase _screenOut;

	private ScreenBase _screenIn;

	private List<ScreenBase> _keepLayers;

	private Action _onComplete;

	private EStage _currStage;

	private JobCounter _currJobs;

	public EStage _pCurrentStage
	{
		get
		{
			return _currStage;
		}
	}

	public ScreenBase _pCurrentScreen
	{
		get
		{
			return (_currStage == EStage.OUT) ? _screenOut : ((_currStage != EStage.IN) ? null : _screenIn);
		}
	}

	public ScreenBase _pScreenOut
	{
		get
		{
			return _screenOut;
		}
	}

	public ScreenBase _pScreenIn
	{
		get
		{
			return _screenIn;
		}
	}

	public ScreenTransition()
	{
		_keepLayers = new List<ScreenBase>();
	}

	public void Reset()
	{
		_screenFlow = null;
		_screenOut = null;
		_screenIn = null;
		_keepLayers.Clear();
		_onComplete = null;
		_currStage = EStage.NULL;
		_currJobs = null;
	}

	public void Init(ScreenFlow screenFlow, ScreenBase screenIn, Action onComplete)
	{
		Reset();
		_screenFlow = screenFlow;
		_screenIn = screenIn;
		_onComplete = onComplete;
		BeginStageIn();
	}

	public void Init(ScreenBase screenOut, ScreenBase screenIn, Action onComplete)
	{
		Reset();
		if (screenOut == null)
		{
			Debug.LogError("[ScreenTransition] Null screenOut");
			return;
		}
		_screenFlow = screenOut._pScreenFlow;
		_screenOut = screenOut;
		_screenIn = screenIn;
		FindPersistantLayers();
		_onComplete = onComplete;
		BeginStageOut();
	}

	public void Modify(ScreenBase screenIn, Action onComplete)
	{
		if (_currStage != EStage.OUT)
		{
			Debug.LogError("[ScreenTransition] Can only modify transition in OUT stage!");
			return;
		}
		ModifyScreenIn(screenIn);
		ModifyOnComplete(onComplete);
	}

	public void Modify(ScreenBase screenIn)
	{
		if (_currStage != EStage.OUT)
		{
			Debug.LogError("[ScreenTransition] Can only modify transition in OUT stage!");
		}
		else
		{
			ModifyScreenIn(screenIn);
		}
	}

	public void Modify(Action onComplete)
	{
		ModifyOnComplete(onComplete);
	}

	public void Cancel()
	{
		if (_currStage != EStage.IN)
		{
			Debug.LogError("[ScreenTransition] Can only cancel transition in IN stage!");
		}
		else
		{
			CancelStageIn();
		}
	}

	private void BeginStageOut()
	{
		_currStage = EStage.OUT;
		if (_screenOut == null)
		{
			EndStageOut();
			return;
		}
		_currJobs = _screenOut.ForEachLayerAsync(true, (ScreenBase s, Action oc) =>
		{
			s.ExitScreen(oc);
		}, EndStageOut, (ScreenBase s) => _keepLayers == null || !_keepLayers.Contains(s));
	}

	private void EndStageOut()
	{
		_currStage = EStage.NULL;
		_currJobs = null;
		BeginStageIn();
	}

	private void BeginStageIn()
	{
		_currStage = EStage.IN;
		if (_screenIn == null)
		{
			EndStageIn();
			return;
		}
		_currJobs = _screenIn.ForEachLayerAsync(true, (ScreenBase s, Action oc) =>
		{
			s.ShowScreen(_screenFlow, oc);
		}, EndStageIn, (ScreenBase s) => _keepLayers == null || !_keepLayers.Contains(s));
	}

	private void EndStageIn()
	{
		_currStage = EStage.NULL;
		_currJobs = null;
		if (_onComplete != null)
		{
			_onComplete();
		}
	}

	private void CancelStageIn()
	{
		_screenIn.ForEachLayer(true, (ScreenBase s) =>
		{
			s.CancelShowScreen();
		}, (ScreenBase s) => _keepLayers == null || !_keepLayers.Contains(s));
		_onComplete = null;
		_currStage = EStage.NULL;
		_currJobs = null;
	}

	private void ModifyScreenIn(ScreenBase screenIn)
	{
		_screenIn = screenIn;
		List<ScreenBase> list = new List<ScreenBase>(_keepLayers);
		FindPersistantLayers();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && !_keepLayers.Contains(list[i]))
			{
				if (_currJobs == null)
				{
					Debug.LogError("[ScreenTransition] No current jobs to append to!");
					break;
				}
				_currJobs.BeginJob();
				list[i].ExitScreen(() =>
				{
					_currJobs.EndJob();
				});
			}
		}
	}

	private void ModifyOnComplete(Action onComplete)
	{
		_onComplete = onComplete;
	}

	private void FindPersistantLayers()
	{
		_keepLayers.Clear();
		if (!(_screenIn != null))
		{
			return;
		}
		IList<ScreenBase> pLayers = _screenOut._pLayers;
		IList<ScreenBase> pLayers2 = _screenIn._pLayers;
		if (pLayers == null || pLayers2 == null)
		{
			return;
		}
		for (int i = 0; i < pLayers.Count; i++)
		{
			if (pLayers[i] != null && pLayers2.Contains(pLayers[i]))
			{
				_keepLayers.Add(pLayers[i]);
			}
		}
	}
}
