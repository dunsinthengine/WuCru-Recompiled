using UnityEngine;

public class ForceClearCamera : MonoBehaviour
{
	public bool clearDepth;

	public bool clearColor;

	public Color bgColor = Color.black;

	private void OnPreRender()
	{
		GL.Clear(clearDepth, clearColor, bgColor);
	}
}
