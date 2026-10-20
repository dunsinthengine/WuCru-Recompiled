using System;
using Cape.LegoSdk.Core;
using LEGO.CoreSDK.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LegoSdkInitializer : MonoBehaviour
{
	[SerializeField]
	private string _firstScene;

	private bool _firstLoad = true;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		Debug.Log("Calling LEGO Core SDK initialise function. ");
		try
		{
			LegoCoreSdk.Instance.Initialize();
			LegoCoreSdk.ConfigurationChangedEvent += CoreSdkConfigurationChangedHandler;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("LegoCoreSdk Initialize exception handled: " + ex.Message);
		}

		ScreenFacade instance = Facades<ScreenFacade>.Instance;
		if (instance != null)
		{
			ScreenTitle screenInstance = instance.GetScreenInstance<ScreenTitle>();
			if (screenInstance != null)
			{
				Debug.Log("Showing ScreenTitle via GetScreenInstance");
				instance.ShowScreen(screenInstance);
			}
			else
			{
				ScreenBase screenByName = instance.FindScreen("ScreenTitle");
				if (screenByName != null)
				{
					Debug.Log("Showing ScreenTitle via FindScreen");
					instance.ShowScreen(screenByName);
				}
				else
				{
					Debug.LogError("ScreenTitle not found in ScreenFacade!");
				}
			}
		}
		_firstLoad = false;
		Debug.Log("Initialised Lego Core SDK");
		try
		{
			ManagerCrossPromo._pInstance.SetUp();
		}
		catch (Exception ex2)
		{
			Debug.LogWarning("ManagerCrossPromo SetUp ignored on PC: " + ex2.Message);
		}
	}

	private void Destroy()
	{
		LegoCoreSdk.ConfigurationChangedEvent -= CoreSdkConfigurationChangedHandler;
	}

	private void CoreSdkConfigurationChangedHandler(object sender, EventArgs args)
	{
		try
		{
			ManagerCrossPromo._pInstance.SetUp();
		}
		catch (Exception)
		{
		}
		if (LegoCoreSdk.Instance.State == CoreSdkState.KillSwitch || LegoCoreSdk.Instance.State == CoreSdkState.Version)
		{
			PruneSceneForAlert();
		}
	}

	private void PruneSceneForAlert()
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			AlertBehaviour component = gameObject.GetComponent<AlertBehaviour>();
			EventSystem component2 = gameObject.GetComponent<EventSystem>();
			if (!(gameObject == base.gameObject) && !(component != null) && !(component2 != null))
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}
}
