using System;
using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmQuitGUI : MonoBehaviour
{
	public bool quitBox;

	public Image imageQuitBG;

	public Image imageMapBG;

	public Image imageLevel;

	public Image imageStar1;

	public Image imageStar2;

	public Image imageStar3;

	public Button buttonOK;

	public Button buttonCancel;

	public Action onOKAction;

	public Action onCancelAction;

	public Text textLevelName;

	private void Awake()
	{
		OnLocalize();
		Localization.Instance.LocalizationChanged += OnLocalize;
		GameControllerStateAreaMap gameControllerStateAreaMap = GameController.Instance.currentState as GameControllerStateAreaMap;
		buttonOK.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			onOKAction();
			UnityEngine.Object.Destroy(base.gameObject);
		});
		buttonCancel.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			onCancelAction();
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	private void OnDestroy()
	{
		Localization.Instance.LocalizationChanged -= OnLocalize;
	}

	public void OnLocalize()
	{
	}
}
