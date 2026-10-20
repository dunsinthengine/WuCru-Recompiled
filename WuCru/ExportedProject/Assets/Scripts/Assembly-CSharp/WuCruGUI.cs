using System;
using DentedPixel.LTEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WuCruGUI : MonoBehaviour
{
	public Button wuCruButton;

	[SerializeField]
	private LeanTweenVisual animIn;

	[SerializeField]
	private GameObject _alertIcon;

	public event EventHandler<DataEventArgs<bool>> ClickBlockerEvent;

	private void Awake()
	{
		animIn.playOnStart = false;
		wuCruButton.gameObject.SetActive(false);
		_alertIcon.gameObject.SetActive(false);
	}

	private void Start()
	{
		Scene sceneByName = SceneManager.GetSceneByName("LegoIdLogin");
		if (string.IsNullOrEmpty(sceneByName.name))
		{
			if (GameController.Instance.MissionController.CurrentMission == null)
			{
				Debug.Log("Set up new Wu Cru mission");
				return;
			}
			Debug.Log("Existing Wu Cru mission already set up");
			if (!GameController.Instance.MissionController.CurrentMissionAcknowledged)
			{
				_alertIcon.gameObject.SetActive(true);
				DoNewMissionTweenAndSoundFx();
			}
			WuCruButtonShowMission();
		}
		else
		{
			SceneManager.UnloadSceneAsync(sceneByName.buildIndex);
		}
	}

	private void OnDestroy()
	{
		if (GameController.Instance.MissionController != null)
		{
			GameController.Instance.MissionController.NewMissionReadyEvent -= NewMissionReadyHandler;
		}
	}

	private void DoNewMissionTweenAndSoundFx()
	{
		AudioPlayer.PlaySound("fx/sub/goal/reached");
		if (animIn != null)
		{
			animIn.start(wuCruButton.gameObject);
		}
	}

	private void WuCruButtonShowMission()
	{
		wuCruButton.onClick.RemoveAllListeners();
		wuCruButton.onClick.AddListener(() =>
		{
			if (GameController.Instance.MissionController.CurrentMission != null)
			{
				GameController.Instance.MissionController.CurrentMissionAcknowledged = true;
				_alertIcon.gameObject.SetActive(false);
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/WuCruMissionUnlockedGUI"));
				MissionPopupGUI component = gameObject.GetComponent<MissionPopupGUI>();
				component.SetImage(false);
			}
		});
		wuCruButton.gameObject.SetActive(true);
	}

	private void NewMissionReadyHandler(object sender, DataEventArgs<Mission> args)
	{
		Debug.LogWarningFormat("New mission ready, ID : {0}", GameController.Instance.MissionController.CurrentMission.MissionID);
		GameController.Instance.MissionController.NewMissionReadyEvent -= NewMissionReadyHandler;
		WuCruButtonShowMission();
		_alertIcon.gameObject.SetActive(true);
		DoNewMissionTweenAndSoundFx();
		if (ClickBlockerEvent != null)
		{
			DataEventArgs<bool> e = new DataEventArgs<bool>();
			e.data = false;
			DataEventArgs<bool> e2 = e;
			ClickBlockerEvent(this, e2);
		}
	}
}
