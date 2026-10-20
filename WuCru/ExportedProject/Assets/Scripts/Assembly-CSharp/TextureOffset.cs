using UnityEngine;

public class TextureOffset : MonoBehaviour
{
	public float scrollSpeed = 0.5f;

	public Renderer rend;

	private void Start()
	{
		rend = GetComponent<Renderer>();
	}

	private void Update()
	{
		float x = Time.time * scrollSpeed;
		rend.material.SetTextureOffset("_MainTex", new Vector2(x, 0f));
	}
}
