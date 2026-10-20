using UnityEngine;

public class NinjagoFog : MonoBehaviour
{
	[Tooltip("Fog start Y coordinate (top, no fog)")]
	public float heightStart = 1f;

	[Tooltip("Fog end Y coordinate (bottom, full fog)")]
	public float heightEnd;

	public void OnPreRender()
	{
		Shader.SetGlobalFloat("_fogHeightStart", heightStart);
		Shader.SetGlobalFloat("_fogHeightEnd", heightEnd);
	}
}
