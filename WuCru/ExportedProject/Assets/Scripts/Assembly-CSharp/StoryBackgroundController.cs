using UnityEngine;

public class StoryBackgroundController : MonoBehaviour
{
	private GameObject bgPrefab;

	public string unknownLocation;

	public string teaFarm;

	public string forest;

	public string desert;

	public string mountain;

	public string dungeon;

	public string samuraiXCave;

	public string swamp;

	public string holoPlayer;

	public string hypnobraitomb;

	public string fangpyretomb;

	public string constrictaitomb;

	private int numBackgrounds = 12;

	private string[] gameobjects;

	private void Awake()
	{
		gameobjects = new string[numBackgrounds];
		gameobjects[0] = unknownLocation;
		gameobjects[1] = teaFarm;
		gameobjects[2] = forest;
		gameobjects[3] = desert;
		gameobjects[4] = mountain;
		gameobjects[5] = dungeon;
		gameobjects[6] = samuraiXCave;
		gameobjects[7] = swamp;
		gameobjects[8] = holoPlayer;
		gameobjects[9] = hypnobraitomb;
		gameobjects[10] = fangpyretomb;
		gameobjects[11] = constrictaitomb;
	}

	public void SetBackground(StoryBackground bg)
	{
		if (bgPrefab != null)
		{
			Object.Destroy(bgPrefab);
		}
		bgPrefab = (GameObject)Object.Instantiate(Resources.Load(gameobjects[(int)bg]));
	}
}
