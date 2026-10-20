using UnityEngine;

public class PressAndHoldBar : MonoBehaviour
{
	public SpriteRenderer bgSprite;

	public SpriteRenderer frameSprite;

	public SpriteRenderer pointerSprite;

	public SpriteRenderer markerStart;

	public SpriteRenderer markerEnd;

	public float graceWindow = 0.3f;

	public float beginWindow = 0.7f;

	public float endWindow = 1.2f;

	public float duration = 1.8f;

	public Color colorInWindow = Color.green;

	public Color colorOutsideWindow = Color.red;

	private bool isInWindow;

	private float startTime;

	public bool IsInWindow
	{
		get
		{
			return bgSprite.enabled && isInWindow;
		}
	}

	public bool IsDone
	{
		get
		{
			return bgSprite.enabled && startTime + duration < Time.time;
		}
	}

	public bool IsInGrace
	{
		get
		{
			return bgSprite.enabled && startTime + graceWindow >= Time.time;
		}
	}

	public bool Visible
	{
		get
		{
			return bgSprite.enabled;
		}
	}

	private void Awake()
	{
		Show(false);
	}

	public void Show(bool visible)
	{
		if (visible && !bgSprite.enabled)
		{
			bgSprite.enabled = true;
			if (frameSprite != null)
			{
				frameSprite.enabled = true;
			}
			pointerSprite.enabled = true;
			markerStart.enabled = true;
			markerEnd.enabled = true;
			markerStart.transform.localPosition = bgSprite.transform.localPosition + new Vector3(beginWindow / duration - 0.5f, 0f, 0f);
			markerEnd.transform.localPosition = bgSprite.transform.localPosition + new Vector3(endWindow / duration - 0.5f, 0f, 0f);
			startTime = Time.time;
			bgSprite.material.color = colorOutsideWindow;
			isInWindow = false;
		}
		else if (!visible && bgSprite.enabled)
		{
			bgSprite.enabled = false;
			if (frameSprite != null)
			{
				frameSprite.enabled = false;
			}
			pointerSprite.enabled = false;
			markerStart.enabled = false;
			markerEnd.enabled = false;
		}
	}

	private void Update()
	{
		if (bgSprite.enabled)
		{
			float num = Time.time - startTime;
			if (num > beginWindow && num < endWindow && !isInWindow)
			{
				isInWindow = true;
				bgSprite.material.color = colorInWindow;
			}
			else if (num > endWindow && isInWindow)
			{
				isInWindow = false;
				bgSprite.material.color = colorOutsideWindow;
			}
			float num2 = Mathf.Clamp01(num / duration);
			pointerSprite.transform.localPosition = bgSprite.transform.localPosition + new Vector3(num2 - 0.5f, 0f, 0f);
		}
	}

	private void LateUpdate()
	{
		if (bgSprite.enabled)
		{
			base.transform.LookAt(Camera.main.transform.position, Vector3.up);
			base.transform.Rotate(Vector3.up, 180f);
		}
	}
}
