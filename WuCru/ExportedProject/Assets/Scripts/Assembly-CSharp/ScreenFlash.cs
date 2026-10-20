using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
	public CanvasGroup myCG;

	public Image myCanvas;

	private bool flash;

	private float flashSpeed = 1f;

	private Color flashColor = Color.white;

	private void Start()
	{
		myCG.alpha = 0f;
	}

	private void Update()
	{
		if (flash)
		{
			myCG.gameObject.SetActive(true);
			myCG.alpha -= Time.deltaTime * flashSpeed;
			if (myCG.alpha <= 0f)
			{
				myCG.alpha = 0f;
				flash = false;
				myCG.gameObject.SetActive(false);
			}
		}
	}

	public void FlashAlpha(float alpha, float speed, Color color)
	{
		flash = true;
		flashSpeed = speed;
		flashColor = color;
		myCanvas.color = color;
		myCG.alpha = alpha;
	}
}
