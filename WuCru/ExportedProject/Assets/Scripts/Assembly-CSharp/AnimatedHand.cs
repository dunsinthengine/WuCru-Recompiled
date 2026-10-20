using UnityEngine;

public class AnimatedHand : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] handSprites;

	[SerializeField]
	private float[] intervalModifier;

	public float interval = 0.2f;

	private float startTime;

	private int currentIndex;

	private void Start()
	{
		SpriteRenderer[] array = handSprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.enabled = false;
		}
		handSprites[currentIndex].enabled = true;
	}

	private void Update()
	{
		float num = intervalModifier[currentIndex];
		if (Time.realtimeSinceStartup - startTime >= interval * num)
		{
			startTime = Time.realtimeSinceStartup;
			handSprites[currentIndex].enabled = false;
			currentIndex++;
			if (currentIndex >= handSprites.Length)
			{
				currentIndex = 0;
			}
			handSprites[currentIndex].enabled = true;
		}
	}

	private void LateUpdate()
	{
		SpriteRenderer[] array = handSprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (spriteRenderer.enabled)
			{
				base.transform.LookAt(Camera.main.transform.position, Vector3.up);
				base.transform.Rotate(Vector3.up, 180f);
			}
		}
	}
}
