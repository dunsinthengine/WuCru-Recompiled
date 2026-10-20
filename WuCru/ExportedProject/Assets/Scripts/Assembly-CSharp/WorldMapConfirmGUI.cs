using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapConfirmGUI : MonoBehaviour
{
	public delegate void OnOKDelegate();

	public Image imageArea;

	public Localize localizeTextAreaName;

	public Text textLevelsPlayed;

	public Button buttonOK;

	public Button buttonCancel;

	public OnOKDelegate OnOK;

	private void Awake()
	{
		imageArea.sprite = GameController.Instance.areaMapMetaLayer.previewImage;
		string displayName = GameController.Instance.areaMapMetaLayer.displayName;
		localizeTextAreaName.SetKey(displayName);
		OnLocalize();
		Localization.Instance.LocalizationChanged += OnLocalize;
		buttonCancel.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			Object.Destroy(base.gameObject);
		});
		buttonOK.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			if (OnOK != null)
			{
				OnOK();
			}
			Object.Destroy(base.gameObject);
		});
	}

	private void OnDestroy()
	{
		Localization.Instance.LocalizationChanged -= OnLocalize;
	}

	public void OnLocalize()
	{
		int num = GameController.Instance.areaMapMetaLayer.NumLevelsPlayed();
		int num2 = GameController.Instance.areaMapMetaLayer.NumPlayableLevels();
		string text = Localization.Instance.Get("LABEL_PLAYED");
		string text2 = num + "/" + num2 + " " + text;
		textLevelsPlayed.text = text2;
	}
}
