using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EnableCameraDepthInForward : MonoBehaviour
{
	private void OnDrawGizmo()
	{
		Set();
	}

	private void Start()
	{
		Set();
	}

	private void Set()
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
	}
}
