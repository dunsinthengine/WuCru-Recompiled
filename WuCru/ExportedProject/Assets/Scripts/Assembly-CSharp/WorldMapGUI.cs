using UnityEngine;
using UnityEngine.UI;

public class WorldMapGUI : MonoBehaviour
{
	public Button buttonStoreLocations;

	public bool isDesigner;

	public WorldMapConfirmGUI confirmGUI;

	private void Awake()
	{
		AnalyticsEvents.LogLoadScene("WorldMap");
		foreach (AreaMapMetaLayer value in GameController.Instance.areaMapDB.areaMaps.values)
		{
			if (GameController.Instance.PlayerProfile.IsAreaMapUnlocked(value) || isDesigner)
			{
				WorldMapButtonAreaMap componentInChildren = ((GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/WorldMapButtonAreaMap"))).GetComponentInChildren<WorldMapButtonAreaMap>();
				componentInChildren.transform.parent = base.gameObject.transform;
				componentInChildren.areaMapMetaLayer = value;
				componentInChildren.SetLocalizationKey(value.displayName);
				componentInChildren.transform.localPosition = value.worldMapButtonPosition;
			}
		}
	}
}
