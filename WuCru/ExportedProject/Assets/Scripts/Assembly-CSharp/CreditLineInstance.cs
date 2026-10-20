using UnityEngine;

public abstract class CreditLineInstance : MonoBehaviour
{
	private float speed;

	private float fadeTime;

	private Vector2 pos;

	private RectTransform rect;

	private bool nextLine;

	public abstract void setVisual(string data, string data2);

	public abstract void fadeIn(float time);

	public abstract void fadeOut(float time);

	public void go(string data, string data2, int height, float speed, float fadeTime)
	{
		this.speed = speed;
		this.fadeTime = Mathf.Clamp(fadeTime, 0f, 10f);
		setVisual(data, data2);
		fadeOut(0f);
		rect = GetComponent<RectTransform>();
		pos = rect.anchoredPosition;
		float time = rect.rect.height / speed;
		Invoke("fadeIn", time);
		float num = ((float)height - this.fadeTime * speed) / speed;
		Invoke("fadeOut", num);
		Invoke("destroy", num + this.fadeTime + 0.1f);
	}

	private void Update()
	{
		if (rect != null)
		{
			pos.y += Time.deltaTime * speed;
			rect.anchoredPosition = pos;
			if (!nextLine && pos.y > rect.rect.height)
			{
				nextLine = true;
				Credits.getInstance().spawnNext();
			}
		}
	}

	private void fadeIn()
	{
		fadeIn(fadeTime);
	}

	private void fadeOut()
	{
		fadeOut(fadeTime);
	}

	private void destroy()
	{
		Credits.getInstance().callbackDeleted();
		Object.DestroyImmediate(base.gameObject);
	}
}
