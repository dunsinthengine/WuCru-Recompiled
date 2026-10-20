using UnityEngine;

public class StoryOverlayController : MonoBehaviour
{
	private GameObject overlayPrefab;

	public string none;

	public string holoPlayer;

	public string corrupted;

	private int numBackgrounds = 3;

	private string[] gameobjects;

	private void Awake()
	{
		gameobjects = new string[numBackgrounds];
		gameobjects[0] = none;
		gameobjects[1] = holoPlayer;
		gameobjects[2] = corrupted;
	}

	public void SetOverlay(StoryOverlay overlay)
	{
		if (overlayPrefab != null)
		{
			Object.Destroy(overlayPrefab);
		}
		overlayPrefab = (GameObject)Object.Instantiate(Resources.Load(gameobjects[(int)overlay]));
	}
}
