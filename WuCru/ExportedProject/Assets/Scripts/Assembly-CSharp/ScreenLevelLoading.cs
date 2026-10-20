using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenLevelLoading : ScreenBase
{
	private bool _loaded;

	private float _loadTimer;

	protected override void OnShowScreen()
	{
		Debug.Log("OnShowScreen()");
		_loaded = false;
		base.OnShowScreen();
	}

	protected override void OnScreenShowComplete()
	{
		base.OnScreenShowComplete();
		SceneManager.LoadScene("LevelPlayerNew");
	}

	protected override void Update()
	{
		base.Update();
		if (_loadTimer > 0f)
		{
			_loadTimer -= Time.deltaTime;
			if (_loadTimer <= 0f)
			{
				Debug.Log("Loaded");
				ScreenFacade instance = Facades<ScreenFacade>.Instance;
				ScreenHud screenInstance = instance.GetScreenInstance<ScreenHud>();
				Navigate(screenInstance);
			}
		}
	}

	public void OnLevelReady()
	{
		Debug.Log("OnLevelReady()");
		if (!_loaded)
		{
			_loaded = true;
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			ScreenHud screenInstance = instance.GetScreenInstance<ScreenHud>();
			screenInstance.RefreshPlayerTexture();
			_loadTimer = 0.5f;
		}
	}
}
