using UnityEngine;

public class LightSource : MonoBehaviour
{
	public SpriteRenderer lightSprite;

	public float range = 7f;

	public float intensity = 1f;

	private void LateUpdate()
	{
		if (lightSprite != null)
		{
			base.transform.LookAt(Camera.main.transform.position, Vector3.up);
			base.transform.Rotate(Vector3.up, 180f);
		}
	}
}
