using System.Collections.Generic;
using UnityEngine;

public class ScreenLoader : InitialisationObject
{
	public bool initialiseGameSceneAfterScreenLoad = true;

	public bool dontDestroyOnLoad = true;

	public bool emulateLowMemInEditor;

	[HideInInspector]
	public List<ScreenLoaderInfo> screensToLoad;

	[HideInInspector]
	public bool showScreens;

	private int _currentScreenLoad;

	private int _numScreensToLoad;

	private bool _isLoading;

	protected override void Awake()
	{
		if (dontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
		base.Awake();
	}

	public override void startInitialising()
	{
		_currentState = InitialisationState.INITIALISING;
		LoadScreens();
	}

	public void LoadScreens()
	{
		_currentScreenLoad = 0;
		_numScreensToLoad = screensToLoad.Count;
		_isLoading = true;
	}

	private void Update()
	{
		if (!_isLoading)
		{
			return;
		}
		if (_currentScreenLoad < _numScreensToLoad)
		{
			if (screensToLoad[_currentScreenLoad] != null && screensToLoad[_currentScreenLoad].screenBase != null)
			{
				ScreenBase screenBase = Object.Instantiate(screensToLoad[_currentScreenLoad].screenBase);
				screenBase.gameObject.name = screensToLoad[_currentScreenLoad].screenBase.name;
				screenBase.RegisterScreen();
				screenBase.transform.parent = Facades<ScreenFacade>.Instance.transform;
				screenBase.transform.localScale = Vector3.one;
				screenBase.gameObject.SetActive(false);
			}
			_currentScreenLoad++;
			return;
		}
		ScreenBase screenBase2 = null;
		for (int i = 0; i < screensToLoad.Count; i++)
		{
			ScreenBase screenBase3 = Facades<ScreenFacade>.Instance.FindScreen(screensToLoad[i].screenBase.name);
			if (screenBase3 != null)
			{
				screenBase3.OnAllScreensLoaded();
			}
			if (screensToLoad[i].immediatelyShowOnLoad && screenBase2 == null)
			{
				screenBase2 = screenBase3;
			}
		}
		if (screenBase2 != null)
		{
			Facades<ScreenFacade>.Instance.ShowScreen(screenBase2);
		}
		_isLoading = false;
		_currentState = InitialisationState.FINISHED;
	}
}
