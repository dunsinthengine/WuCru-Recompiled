using UnityEngine;

public class OcclusionRenderer : MonoBehaviour
{
	public Shader occludedShader;

	public Color occludedColor;

	private void Start()
	{
		GetComponent<Camera>().SetReplacementShader(occludedShader, "Occluded");
		Shader.SetGlobalColor("_OccludedColor", occludedColor);
	}

	public void OnPreRender()
	{
		Shader.SetGlobalColor("_OccludedColor", occludedColor);
	}

	public void SetOccludedColor(Color color)
	{
		occludedColor = color;
	}
}
