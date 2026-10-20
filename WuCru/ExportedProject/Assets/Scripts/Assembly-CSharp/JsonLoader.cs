using System;
using System.Collections.Generic;
using System.IO;
using AmuzoEngine;
using LitJson;
using UnityEngine;

public class JsonLoader : InitialisationObject
{
	private enum EStage
	{
		NULL = 0,
		LOAD = 1,
		ASYNC_ADD = 2,
		READY = 3
	}

	public TextAsset[] json;

	private const float MAX_ASYNC_ADD_JSON_ACTION_TIME = 0.01f;

	private EStage _stage;

	private int _loadIndex;

	public bool _isAsyncAddJson;

	private ActionQueue _asyncAddJsonActions;

	private static Dictionary<string, JsonData> _d;

	public bool Ready
	{
		get
		{
			return _stage == EStage.READY;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_asyncAddJsonActions = new ActionQueue(base.gameObject.name + " - AddJson actions");
		_stage = EStage.NULL;
		if (InitialisationFacade.Instance == null)
		{
			Load();
		}
	}

	public override void startInitialising()
	{
		_stage = EStage.LOAD;
		_currentState = InitialisationState.INITIALISING;
		Load();
	}

	public override InitialisationState updateInitialisation()
	{
		if (_stage == EStage.ASYNC_ADD)
		{
			if (_asyncAddJsonActions == null)
			{
				EndInitialization();
				return _currentState;
			}
			_asyncAddJsonActions.DoActions(0.01f, () => Time.realtimeSinceStartup);
			if (_asyncAddJsonActions._pIsEmpty)
			{
				EndInitialization();
			}
		}
		return _currentState;
	}

	private void EndInitialization()
	{
		_stage = EStage.READY;
		_currentState = InitialisationState.FINISHED;
	}

	private void Load()
	{
		if (json == null || json.Length == 0)
		{
			return;
		}
		if (_loadIndex == json.Length)
		{
			LoadComplete();
			return;
		}
		if (_d == null)
		{
			_d = new Dictionary<string, JsonData>();
		}
		for (int i = _loadIndex; i < json.Length; i++)
		{
			TextAsset textAsset = json[i];
			if (!(textAsset == null))
			{
				string text = textAsset.name;
				string text2 = Application.streamingAssetsPath + "/" + text + ".txt";
				string text3 = Application.persistentDataPath + "/" + text + ".txt";
				if (File.Exists(text3))
				{
					Debug.Log("Loading JSON [" + text + "] from File [" + text3 + "]");
					LoadJSON(text, text3);
				}
				else
				{
					Debug.Log("Loading JSON [" + text + "] from File [" + text2 + "]");
					LoadJSON(text, text2);
				}
				return;
			}
		}
		LoadComplete();
	}

	private void LoadComplete()
	{
		if (_isAsyncAddJson)
		{
			_stage = EStage.ASYNC_ADD;
		}
		else
		{
			EndInitialization();
		}
	}

	private void AddJson(string name, string json)
	{
		Action action = () =>
		{
			_d.Add(name, Extensions.LoadJson(json));
		};
		if (_isAsyncAddJson && _asyncAddJsonActions != null)
		{
			_asyncAddJsonActions.AddAction(action);
		}
		else
		{
			action();
		}
	}

	private void LoadJSON(string name, string url)
	{
		_loadIndex++;
		if (Application.isPlaying)
		{
			GetComponent<TextLoaderBehaviour>().LoadText(url, OnLoadComplete, name);
		}
	}

	private void OnLoadComplete(string newjson, params object[] args)
	{
		string text = (string)args[0];
		if (string.IsNullOrEmpty(newjson))
		{
			Debug.Log("Failed to find local copy of Json [" + text + "]");
			for (int i = 0; i < json.Length; i++)
			{
				if (!(json[i] == null) && !(json[i].name != text))
				{
					AddJson(text, json[i].ToString());
				}
			}
		}
		else
		{
			AddJson(text, newjson);
		}
		Load();
	}

	public static JsonData data(string name)
	{
		if (_d == null)
		{
			return null;
		}
		if (!_d.ContainsKey(name))
		{
			Debug.LogError("[JsonLoader] Data '" + name + "' not found");
			return null;
		}
		return _d[name];
	}

	public static void Dispose(string name)
	{
		if (_d != null)
		{
			_d.Remove(name);
		}
	}

	public static void DisposeAll()
	{
		if (_d != null)
		{
			_d.Clear();
			_d = null;
		}
	}
}

// recompile trigger
