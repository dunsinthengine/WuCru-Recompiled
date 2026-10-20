using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, InitialisationProxy.ITarget
{
	private static bool DO_DEBUG = true;

	private const int DELAY_FRAMES = 2;

	public string levelName;

	public bool assetbundle;

	public bool forceLoadIfAlreadyLoaded;

	public SignalSender onLoadComplete;

	public bool loadAdditively;

	public bool loadAsync = true;

	public bool _isClearResourcesOnLoadComplete;

	private AsyncOperation levelLoader;

	[SerializeField]
	private bool _dontDestroyOnLoad = true;

	private int _delayFrames = 2;

	private InitialisationState _initialisationState;

	private Action<bool> _onLoadCompleteAction;

	private static List<string> _sLoadedSceneNames = new List<string>();

	InitialisationState InitialisationProxy.ITarget._pInitialisationState
	{
		get
		{
			return _initialisationState;
		}
		set
		{
			_initialisationState = value;
		}
	}

	public bool isLoading
	{
		get
		{
			return levelLoader != null && !levelLoader.isDone;
		}
	}

	public float _pLoadingProgress
	{
		get
		{
			return (levelLoader == null) ? 0f : levelLoader.progress;
		}
	}

	private void Awake()
	{
		if (_dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnSignal()
	{
		if (!AttemptLoadLevel())
		{
			Log("Not switching to scene: " + levelName + " (already loaded)");
		}
	}

	public bool AttemptLoadLevel(Action<bool> onLoadComplete = null)
	{
		Log("Attempting to load level [" + levelName + "]");
		_delayFrames = 2;
		SafeCallOnLoadCompleteAction(false);
		_onLoadCompleteAction = onLoadComplete;
		if (IsSceneLoaded(levelName) && !forceLoadIfAlreadyLoaded)
		{
			Log("Level loading not required.");
			SafeCallOnLoadCompleteAction(true);
			return false;
		}
		if (assetbundle)
		{
			if (!File.Exists(streamingFilePath()) && !File.Exists(persistantFilePath()))
			{
				Log("Unable to find scene bundle at path [" + streamingFilePath() + "]");
				Log("Could not load level bundle");
				return false;
			}
			Log("Loading scene from file.");
			StartCoroutine(LoadSceneBundleAsync());
		}
		else
		{
			Log("Loading level.");
			LoadLevel(null);
		}
		return true;
	}

	private void LoadLevel(AssetBundle bundle)
	{
		if (loadAsync)
		{
			StartCoroutine(LoadLevelAsync(null));
		}
		else
		{
			LoadLevelBlocking(bundle);
		}
	}

	private void LoadLevelBlocking(AssetBundle bundle)
	{
		if (loadAdditively)
		{
			Log("Loading scene additively: " + levelName);
			SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		}
		else
		{
			Log("Loading scene now: " + levelName);
			SceneManager.LoadScene(levelName, LoadSceneMode.Single);
		}
		if (bundle != null)
		{
			bundle.Unload(false);
		}
		if (_isClearResourcesOnLoadComplete)
		{
			Log("Unloading unused assets");
			AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
			while (!asyncOperation.isDone)
			{
			}
		}
		OnPostSceneLoad();
	}

	private IEnumerator LoadLevelAsync(AssetBundle bundle)
	{
		while (_delayFrames > 0)
		{
			_delayFrames--;
			yield return new WaitForEndOfFrame();
		}
		if (loadAdditively)
		{
			Log("Loading scene additively: " + levelName);
			levelLoader = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		}
		else
		{
			Log("Loading scene now: " + levelName);
			levelLoader = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
		}
		yield return levelLoader;
		if (bundle != null)
		{
			yield return new WaitForEndOfFrame();
			bundle.Unload(false);
		}
		if (_isClearResourcesOnLoadComplete)
		{
			Log("Unloading unused assets");
			yield return Resources.UnloadUnusedAssets();
			yield return new WaitForSeconds(0.1f);
		}
		OnPostSceneLoad();
		levelLoader = null;
	}

	private void OnPostSceneLoad()
	{
		if (loadAdditively)
		{
			AddLoadedScene(levelName);
		}
		else
		{
			SetLoadedScene(levelName);
		}
		Log("Scene loading complete. Current level name [" + SceneManager.GetActiveScene().name + "]");
		SafeCallOnLoadCompleteAction(true);
		if (onLoadComplete != null)
		{
			onLoadComplete.SendSignals(this);
		}
	}

	private void SafeCallOnLoadCompleteAction(bool success)
	{
		if (_onLoadCompleteAction != null)
		{
			Action<bool> onLoadCompleteAction = _onLoadCompleteAction;
			_onLoadCompleteAction = null;
			onLoadCompleteAction(true);
		}
	}

	private void UpdateLoadingFeedback()
	{
	}

	private IEnumerator LoadSceneBundleAsync()
	{
		string path = streamingFilePath();
		if (!File.Exists(streamingFilePath()))
		{
			path = persistantFilePath();
		}
		path = FileHelper.AddLocalFileUrlScheme(path);
		Log("Retrieving [" + path + "]");
		WWW download = new WWW(path);
		yield return download;
		if (download.isDone && download.assetBundle != null)
		{
			AssetBundle assetBundle = download.assetBundle;
			if (assetBundle != null)
			{
				Log("SceneLoader: Level loaded from file. Loading scene");
				StartCoroutine(LoadLevelAsync(assetBundle));
			}
			else
			{
				Log("Could not load level bundle");
			}
		}
		else
		{
			Log("Unable to download: " + path);
		}
	}

	private string GetPlatformFolder()
	{
		return "Android";
	}

	private string streamingFilePath()
	{
		return Application.streamingAssetsPath + "/" + GetPlatformFolder() + "/" + levelName + ".assetbundle";
	}

	private string persistantFilePath()
	{
		return Application.persistentDataPath + "/" + GetPlatformFolder() + "/" + levelName + ".assetbundle";
	}

	private static void CheckInitLoadedScenes()
	{
		if (_sLoadedSceneNames == null)
		{
			_sLoadedSceneNames = new List<string>(1);
		}
	}

	private static void SetLoadedScene(string sceneName)
	{
		CheckInitLoadedScenes();
		_sLoadedSceneNames.Clear();
		_sLoadedSceneNames.Add(sceneName);
	}

	private static void AddLoadedScene(string sceneName)
	{
		CheckInitLoadedScenes();
		int num = _sLoadedSceneNames.FindIndex((string sn) => sn == sceneName);
		if (num < 0)
		{
			_sLoadedSceneNames.Add(sceneName);
		}
		else if (num < _sLoadedSceneNames.Count - 1)
		{
			_sLoadedSceneNames.RemoveAt(num);
			_sLoadedSceneNames.Add(sceneName);
		}
	}

	public static bool IsSceneLoaded(string sceneName)
	{
		return _sLoadedSceneNames != null && _sLoadedSceneNames.Contains(sceneName);
	}

	public static bool IsSceneLoadedLast(string sceneName)
	{
		return _sLoadedSceneNames != null && _sLoadedSceneNames.Count > 0 && _sLoadedSceneNames[_sLoadedSceneNames.Count - 1] == sceneName;
	}

	public static void Log(string message, UnityEngine.Object o = null)
	{
		if (DO_DEBUG)
		{
		}
	}

	void InitialisationProxy.ITarget.OnStartInitialising()
	{
		_initialisationState = InitialisationState.INITIALISING;
		if (!AttemptLoadLevel((bool success) =>
		{
			_initialisationState = InitialisationState.FINISHED;
		}))
		{
			_initialisationState = InitialisationState.FINISHED;
		}
	}

	InitialisationState InitialisationProxy.ITarget.OnUpdateInitialising()
	{
		return _initialisationState;
	}
}
