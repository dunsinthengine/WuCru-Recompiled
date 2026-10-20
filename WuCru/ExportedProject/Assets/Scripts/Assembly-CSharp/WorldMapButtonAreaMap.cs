using Cape.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMapButtonAreaMap : MonoBehaviour
{
	public Button buttonAreaMap;

	public Localize localizeText;

	public AreaMapMetaLayer areaMapMetaLayer;

	private WorldMapGUI worldMapGUI;

	private Animator animator;

	private void Awake()
	{
	}

	private void Start()
	{
		worldMapGUI = GetComponentInParent<WorldMapGUI>();
		buttonAreaMap.onClick.AddListener(() =>
		{
			if (worldMapGUI.confirmGUI == null)
			{
				AudioPlayer.PlaySound("ui/button/click");
				GameController.Instance.SetAreaMapMetaLayer(areaMapMetaLayer);
				worldMapGUI.confirmGUI = ((GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/WorldMapConfirmGUI"))).GetComponent<WorldMapConfirmGUI>();
				worldMapGUI.confirmGUI.OnOK = GotoAreaMap;
			}
		});
		animator = GetComponentInChildren<Animator>();
		if (GameController.Instance.ObjectiveController.CurrentObjective != null && GameController.Instance.ObjectiveController.CurrentObjective.areaMap == areaMapMetaLayer.name)
		{
			animator.SetTrigger("start");
		}
	}

	public void SetLocalizationKey(string key)
	{
		localizeText.SetKey(key);
	}

	public void GotoAreaMap()
	{
		SceneManager.LoadScene("AreaMap");
	}
}
