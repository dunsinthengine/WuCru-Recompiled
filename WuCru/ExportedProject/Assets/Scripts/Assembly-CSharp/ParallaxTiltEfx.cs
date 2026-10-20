using UnityEngine;

public class ParallaxTiltEfx : MonoBehaviour
{
	public float ForegroundMaxMovement = -3f;

	public float MiddlegroundMaxMovement = -1.5f;

	public float BackgroundMaxMovement = 4.5f;

	public GameObject[] ForegroundObjects;

	public GameObject[] MiddlegroundObjects;

	public GameObject[] BackgroundObjects;

	private Vector2[] foregroundArrayOfOriginalV2;

	private Vector2[] middlegroundArrayOfOriginalV2;

	private Vector2[] backgroundArrayOfOriginalV2;

	private float RabbitX = 0.1f;

	private float RabbitY = 0.1f;

	private void Start()
	{
		Input.gyro.enabled = true;
		if (ForegroundObjects != null)
		{
			foregroundArrayOfOriginalV2 = new Vector2[ForegroundObjects.Length];
		}
		for (int i = 0; i < ForegroundObjects.Length; i++)
		{
			float x = ForegroundObjects[i].GetComponent<RectTransform>().anchoredPosition.x;
			float y = ForegroundObjects[i].GetComponent<RectTransform>().anchoredPosition.y;
			foregroundArrayOfOriginalV2[i] = new Vector2(x, y);
		}
		if (MiddlegroundObjects != null)
		{
			middlegroundArrayOfOriginalV2 = new Vector2[MiddlegroundObjects.Length];
		}
		for (int j = 0; j < MiddlegroundObjects.Length; j++)
		{
			float x2 = MiddlegroundObjects[j].GetComponent<RectTransform>().anchoredPosition.x;
			float y2 = MiddlegroundObjects[j].GetComponent<RectTransform>().anchoredPosition.y;
			middlegroundArrayOfOriginalV2[j] = new Vector2(x2, y2);
		}
		if (BackgroundObjects != null)
		{
			backgroundArrayOfOriginalV2 = new Vector2[BackgroundObjects.Length];
		}
		for (int k = 0; k < BackgroundObjects.Length; k++)
		{
			float x3 = BackgroundObjects[k].GetComponent<RectTransform>().anchoredPosition.x;
			float y3 = BackgroundObjects[k].GetComponent<RectTransform>().anchoredPosition.y;
			backgroundArrayOfOriginalV2[k] = new Vector2(x3, y3);
		}
	}

	private void LateUpdate()
	{
		RabbitX -= Input.gyro.rotationRateUnbiased.y;
		float num = Mathf.Abs(RabbitX) + 1f;
		RabbitX /= num;
		RabbitX *= 1.34f;
		RabbitY += Input.gyro.rotationRateUnbiased.x;
		float num2 = Mathf.Abs(RabbitY) + 1f;
		RabbitY /= num2;
		RabbitX = Mathf.Clamp(RabbitX, -1f, 1f);
		RabbitY = Mathf.Clamp(RabbitY, -1f, 1f);
		if (ForegroundObjects != null)
		{
			for (int i = 0; i < ForegroundObjects.Length; i++)
			{
				float x = ForegroundObjects[i].GetComponent<RectTransform>().anchoredPosition.x;
				float y = ForegroundObjects[i].GetComponent<RectTransform>().anchoredPosition.y;
				float num3 = ForegroundMaxMovement * RabbitX;
				float num4 = ForegroundMaxMovement * RabbitY;
				float x2 = Mathf.Lerp(foregroundArrayOfOriginalV2[i].x, foregroundArrayOfOriginalV2[i].x + num3, Time.time);
				float y2 = Mathf.Lerp(foregroundArrayOfOriginalV2[i].y, foregroundArrayOfOriginalV2[i].y + num4, Time.time);
				ForegroundObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x2, y2);
			}
		}
		if (MiddlegroundObjects != null)
		{
			for (int j = 0; j < MiddlegroundObjects.Length; j++)
			{
				float x3 = MiddlegroundObjects[j].GetComponent<RectTransform>().anchoredPosition.x;
				float y3 = MiddlegroundObjects[j].GetComponent<RectTransform>().anchoredPosition.y;
				float num5 = MiddlegroundMaxMovement * RabbitX;
				float num6 = MiddlegroundMaxMovement * RabbitY;
				float x4 = Mathf.Lerp(middlegroundArrayOfOriginalV2[j].x, middlegroundArrayOfOriginalV2[j].x + num5, Time.time);
				float y4 = Mathf.Lerp(middlegroundArrayOfOriginalV2[j].y, middlegroundArrayOfOriginalV2[j].y + num6, Time.time);
				MiddlegroundObjects[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(x4, y4);
			}
		}
		if (BackgroundObjects != null)
		{
			for (int k = 0; k < BackgroundObjects.Length; k++)
			{
				float x5 = BackgroundObjects[k].GetComponent<RectTransform>().anchoredPosition.x;
				float y5 = BackgroundObjects[k].GetComponent<RectTransform>().anchoredPosition.y;
				float num7 = BackgroundMaxMovement * RabbitX;
				float num8 = BackgroundMaxMovement * RabbitY;
				float x6 = Mathf.Lerp(backgroundArrayOfOriginalV2[k].x, backgroundArrayOfOriginalV2[k].x + num7, Time.time);
				float y6 = Mathf.Lerp(backgroundArrayOfOriginalV2[k].y, backgroundArrayOfOriginalV2[k].y + num8, Time.time);
				BackgroundObjects[k].GetComponent<RectTransform>().anchoredPosition = new Vector2(x6, y6);
			}
		}
	}
}
