using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelOverview : MonoBehaviour
{
	public Canvas canvas;

	public int seed = 1;

	public int rows = 22;

	public int cols = 40;

	public int minElevation = 1;

	public int maxElevation = 5;

	public int minElevationHills = 5;

	public int maxElevationHills = 10;

	public int minElevationPlateauRows = 3;

	public int maxElevationPlateauRows = 6;

	public int minElevationPlateauCols = 4;

	public int maxElevationPlateauCols = 8;

	public int minStartPointRow = 5;

	public int maxStartPointRow = 17;

	public int minEndPointRow = 5;

	public int maxEndPointRow = 17;

	public int mainPathMinWidth = 4;

	public int mainPathMaxWidth = 9;

	public float mainPathSegmentLength = 2f;

	public float mainPathVarianceFactor = 0.6f;

	public int numRectangleRooms = 1;

	public int numEllipseRooms = 2;

	public int minRectangleRoomExtentRows = 4;

	public int maxRectangleRoomExtentRows = 8;

	public int minRectangleRoomExtentCols = 5;

	public int maxRectangleRoomExtentCols = 12;

	public int minEllipseRoomExtentRows = 4;

	public int maxEllipseRoomExtentRows = 8;

	public int minEllipseRoomExtentCols = 5;

	public int maxEllipseRoomExtentCols = 12;

	public int rectangleRoomElevationVariance = 2;

	public int ellipseRoomElevationVariance = 2;

	public int numSideQuests = 2;

	public int sideQuestMinWidth = 4;

	public int sideQuestMaxWidth = 8;

	public float sideQuestSegmentLength = 2f;

	public float sideQuestVarianceFactor = 0.6f;

	public InputField inputFieldSeed;

	public Toggle toggleElevation;

	public Toggle toggleRooms;

	public Toggle toggleSideQuests;

	public Toggle toggleFilters;

	private int maxLevels = 9;

	private GameObject[] levelNodes;

	private RawImage[] images;

	private int selectedIndex = -1;

	private int selectedTranslationIndex = -1;

	private void Start()
	{
		if (inputFieldSeed != null)
		{
			inputFieldSeed.text = seed.ToString();
		}
		levelNodes = new GameObject[maxLevels];
		images = new RawImage[maxLevels];
		for (int i = 0; i < maxLevels; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "RawImage " + i;
			gameObject.transform.parent = canvas.transform;
			RawImage rawImage = gameObject.AddComponent<RawImage>();
			rawImage.rectTransform.sizeDelta = new Vector2(270f, 270f);
			rawImage.transform.localPosition = new Vector3(-270 + i % 3 * 270, -230 + i / 3 * 170, 0f);
			images[i] = rawImage;
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.parent = canvas.transform;
			gameObject2.transform.localPosition = new Vector3(-270 + i % 3 * 270, -150 + i / 3 * 150, 0f);
			levelNodes[i] = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/LevelOverviewNode"), new Vector3(i * 100, 0f, 0f), Quaternion.identity);
			Camera componentInChildren = levelNodes[i].GetComponentInChildren<Camera>();
			componentInChildren.targetTexture = new RenderTexture(512, 512, 32);
			rawImage.texture = componentInChildren.targetTexture;
		}
	}

	public LevelInput CreateLevelInput(int numViewport)
	{
		Debug.Log("Creating Level Input");
		LevelInput levelInput = new LevelInput();
		levelInput.seed = Convert.ToInt32(inputFieldSeed.text) + numViewport;
		levelInput.rows = rows;
		levelInput.cols = cols;
		levelInput.minElevation = minElevation;
		levelInput.maxElevation = maxElevation;
		levelInput.minElevationHills = minElevationHills;
		levelInput.maxElevationHills = maxElevationHills;
		levelInput.minElevationPlateauRows = minElevationPlateauRows;
		levelInput.maxElevationPlateauRows = maxElevationPlateauRows;
		levelInput.minElevationPlateuaCols = minElevationPlateauCols;
		levelInput.maxElevationPlateuaCols = maxElevationPlateauCols;
		levelInput.startPoint = new LevelPoint(UnityEngine.Random.Range(minStartPointRow, maxStartPointRow), 0);
		levelInput.endPoint = new LevelPoint(UnityEngine.Random.Range(minEndPointRow, maxEndPointRow), cols - 1);
		Debug.Log("Adding main path to input");
		MainPathInput mainPathInput = new MainPathInput();
		mainPathInput.minWidth = mainPathMinWidth;
		mainPathInput.maxWidth = mainPathMaxWidth;
		mainPathInput.segmentLength = mainPathSegmentLength;
		mainPathInput.varianceFactor = mainPathVarianceFactor;
		levelInput.chunks.Add(mainPathInput);
		int num = numRectangleRooms + numEllipseRooms;
		int num2 = 0;
		int num3 = 0;
		if (toggleRooms.isOn)
		{
			for (int i = 0; i < num; i++)
			{
				LevelChunkType levelChunkType = LevelChunkType.RectangleRoom;
				if (num2 < numRectangleRooms && num3 < numEllipseRooms)
				{
					levelChunkType = ((!(UnityEngine.Random.value < 0.5f)) ? LevelChunkType.RectangleRoom : LevelChunkType.EllipseRoom);
				}
				else if (num2 < numRectangleRooms)
				{
					levelChunkType = LevelChunkType.RectangleRoom;
				}
				else if (num3 < numEllipseRooms)
				{
					levelChunkType = LevelChunkType.EllipseRoom;
				}
				else
				{
					Debug.LogError("Not able to select a room type...something very wrong!");
				}
				switch (levelChunkType)
				{
				case LevelChunkType.EllipseRoom:
				{
					Debug.Log("Adding EllipseRoom to input");
					EllipseRoomInput ellipseRoomInput = new EllipseRoomInput();
					ellipseRoomInput.extent = new LevelPoint(UnityEngine.Random.Range(minEllipseRoomExtentRows, maxEllipseRoomExtentRows), UnityEngine.Random.Range(minEllipseRoomExtentCols, maxEllipseRoomExtentCols));
					ellipseRoomInput.elevationVariance = ellipseRoomElevationVariance;
					levelInput.chunks.Add(ellipseRoomInput);
					num3++;
					break;
				}
				case LevelChunkType.RectangleRoom:
				{
					Debug.Log("Adding RectangleRoom to input");
					RectangleRoomInput rectangleRoomInput = new RectangleRoomInput();
					rectangleRoomInput.extent = new LevelPoint(UnityEngine.Random.Range(minRectangleRoomExtentRows, maxRectangleRoomExtentRows), UnityEngine.Random.Range(minRectangleRoomExtentCols, maxRectangleRoomExtentCols));
					rectangleRoomInput.elevationVariance = rectangleRoomElevationVariance;
					levelInput.chunks.Add(rectangleRoomInput);
					num2++;
					break;
				}
				}
			}
		}
		if (toggleSideQuests.isOn)
		{
			for (int j = 0; j < numSideQuests; j++)
			{
				Debug.Log("Adding SideQuest to input");
				SideQuestInput sideQuestInput = new SideQuestInput();
				sideQuestInput.minWidth = sideQuestMinWidth;
				sideQuestInput.maxWidth = sideQuestMaxWidth;
				sideQuestInput.segmentLength = sideQuestSegmentLength;
				sideQuestInput.varianceFactor = sideQuestVarianceFactor;
				levelInput.chunks.Add(sideQuestInput);
			}
		}
		return levelInput;
	}

	public void Update()
	{
		if (Input.GetMouseButtonDown(0) && selectedIndex == -1)
		{
			for (int i = 0; i < maxLevels; i++)
			{
				Rect rect = images[i].rectTransform.rect;
				if (new Rect(rect.xMin, rect.yMin, rect.width, rect.height).Contains(Input.mousePosition - images[i].transform.position))
				{
					selectedIndex = i;
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			selectedIndex = -1;
		}
		if (Input.GetMouseButtonDown(1) && selectedTranslationIndex == -1)
		{
			for (int j = 0; j < maxLevels; j++)
			{
				Rect rect2 = images[j].rectTransform.rect;
				if (new Rect(rect2.xMin, rect2.yMin, rect2.width, rect2.height).Contains(Input.mousePosition - images[j].transform.position))
				{
					selectedTranslationIndex = j;
				}
			}
		}
		if (Input.GetMouseButtonUp(1))
		{
			selectedTranslationIndex = -1;
		}
		if (selectedIndex != -1)
		{
			LevelVisualizer componentInChildren = levelNodes[selectedIndex].GetComponentInChildren<LevelVisualizer>();
			componentInChildren.transform.RotateAround(new Vector3(selectedIndex * 100, 0f, 0f) + new Vector3(20f, 0f, 11f), Vector3.up, Input.GetAxis("Mouse X"));
			componentInChildren.transform.RotateAround(new Vector3(selectedIndex * 100, 0f, 0f) + new Vector3(20f, 0f, 11f), componentInChildren.transform.right, 0f - Input.GetAxis("Mouse Y"));
		}
		if (selectedTranslationIndex != -1)
		{
			LevelVisualizer componentInChildren2 = levelNodes[selectedTranslationIndex].GetComponentInChildren<LevelVisualizer>();
			componentInChildren2.transform.position -= componentInChildren2.transform.forward * Input.GetAxis("Mouse Y");
			componentInChildren2.transform.position -= componentInChildren2.transform.right * Input.GetAxis("Mouse X");
			Camera componentInChildren3 = levelNodes[selectedTranslationIndex].GetComponentInChildren<Camera>();
			componentInChildren3.transform.position += componentInChildren3.transform.forward * Input.GetAxis("Mouse ScrollWheel") * 10f;
		}
	}

	public void OnSeedChange()
	{
		seed = int.Parse(inputFieldSeed.text);
	}

	public void Generate()
	{
		StartCoroutine(DoGenerate());
	}

	private IEnumerator DoGenerate()
	{
		yield return 0;
	}
}
