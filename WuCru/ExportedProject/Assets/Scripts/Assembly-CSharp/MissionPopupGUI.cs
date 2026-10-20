using Cape.Localization;
using DentedPixel.LTEditor;
using UnityEngine;
using UnityEngine.UI;

public class MissionPopupGUI : MonoBehaviour
{
	public delegate void OnCloseDelegate();

	[SerializeField]
	private LeanTweenVisual _tween;

	[SerializeField]
	private GameObject _tweenTarget;

	public Image imageMission;

	public Button ButtonOK;

	public Localize localize;

	public Text textAccomplished;

	public OnCloseDelegate OnClose;

	private void Start()
	{
		ButtonOK.onClick.AddListener(() =>
		{
			Object.Destroy(base.gameObject);
			if (OnClose != null)
			{
				OnClose();
			}
		});
	}

	public void SetImage(bool completed)
	{
		if (completed)
		{
			imageMission.sprite = Resources.Load<Sprite>(GameController.Instance.MissionController.CurrentMission.MissionCompletedImagePath);
			textAccomplished.gameObject.SetActive(true);
			localize.SetKey("HEADER_MISSION_ACCOMPLISHED");
			_tween.start(_tweenTarget);
			AudioPlayer.PlaySound("fx/sub/goal/reached");
		}
		else
		{
			imageMission.sprite = Resources.Load<Sprite>(GameController.Instance.MissionController.CurrentMission.MissionImagePath);
			textAccomplished.gameObject.SetActive(false);
		}
	}
}
