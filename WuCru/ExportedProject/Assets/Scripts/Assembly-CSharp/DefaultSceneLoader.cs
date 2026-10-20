using System;
using UnityEngine;

public static class DefaultSceneLoader
{
	private const string LOG_TAG = "[DefaultSceneLoader] ";

	private static SceneLoader _instance;

	public static bool LoadLevel(string sceneName, bool isAsync = true, bool isAdditive = false, bool isReload = false, Action onComplete = null, bool isWaitForSceneInit = false)
	{
		if (string.IsNullOrEmpty(sceneName))
		{
			return false;
		}
		CheckCreateInstance();
		_instance.levelName = sceneName;
		_instance.assetbundle = false;
		_instance.forceLoadIfAlreadyLoaded = isReload;
		_instance.onLoadComplete = null;
		_instance.loadAdditively = isAdditive;
		_instance.loadAsync = isAsync;
		return _instance.AttemptLoadLevel((bool isSuccess) =>
		{
			if (onComplete != null)
			{
				if (isSuccess && isWaitForSceneInit && InitialisationFacade._pExists && !InitialisationFacade.Instance._pHasFinished)
				{
					InitialisationFacade.Instance._pOnFinished += () =>
					{
						onComplete();
					};
				}
				else
				{
					onComplete();
				}
			}
		});
	}

	public static bool ClearScene(bool isAsync = true, Action onComplete = null)
	{
		string sceneName = "Empty";
		bool isAsync2 = isAsync;
		return LoadLevel(sceneName, isAsync2, false, false, onComplete);
	}

	public static bool ReloadLevel(Action onComplete = null, bool isWaitForSceneInit = false)
	{
		if (_instance == null)
		{
			return false;
		}
		return LoadLevel(_instance.levelName, _instance.loadAsync, _instance.loadAdditively, true, onComplete, isWaitForSceneInit);
	}

	private static void CheckCreateInstance()
	{
		if (!(_instance != null))
		{
			GameObject gameObject = new GameObject("DefaultSceneLoader");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			_instance = gameObject.AddComponent<SceneLoader>();
		}
	}
}
