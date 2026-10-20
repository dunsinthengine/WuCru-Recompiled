using UnityEngine;
using UnityEngine.UI;

public class LevelProgressionGUI : MonoBehaviour
{
	public Image pointer;

	public Image imageProgressBar;

	public Image imageGate1;

	public Image imageGate2;

	private int currentIndex = -1;

	private float thirdWidth;

	private int maxCols;

	private Canvas guiCanvas;

	private void Start()
	{
		if (GamePlayer.Instance != null && GamePlayer.Instance.Level.LevelThemeCollection.themes.Count == 3)
		{
			maxCols = GamePlayer.Instance.Level.Nodes.GetLength(1);
			thirdWidth = GetWidth() / 3f;
			pointer.transform.position = new Vector3(ProgressBarX(), pointer.transform.position.y, 0f);
			imageGate1.transform.position = new Vector3(ProgressBarX() + thirdWidth, imageGate1.transform.position.y, 0f);
			imageGate2.transform.position = new Vector3(ProgressBarX() + thirdWidth * 2f, imageGate2.transform.position.y, 0f);
			guiCanvas = GetComponentInParent<Canvas>();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetMapIndex(int index)
	{
		currentIndex = index;
	}

	private void Update()
	{
		if (!(GamePlayer.Instance == null))
		{
			if (GamePlayer.Instance.Level.LevelTheme.index > currentIndex)
			{
				SetMapIndex(GamePlayer.Instance.Level.LevelTheme.index);
			}
			int col = GamePlayer.Instance.Player.Character.CurrentPositionNode.Col;
			float num = (float)col / (float)maxCols;
			pointer.transform.position = new Vector3(ProgressBarX() + thirdWidth * (float)currentIndex + thirdWidth * num, pointer.transform.position.y, 0f);
		}
	}

	private float ProgressBarX()
	{
		return imageProgressBar.transform.position.x - GetWidth() / 2f;
	}

	private float GetWidth()
	{
		Vector3[] array = new Vector3[4];
		imageProgressBar.rectTransform.GetWorldCorners(array);
		return (array[2].x - array[0].x) * 0.9f;
	}
}
