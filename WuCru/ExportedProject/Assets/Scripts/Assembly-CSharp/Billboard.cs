using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void Update()
	{
		if (InGameCam._pInstance.gameObject.activeInHierarchy)
		{
			base.transform.LookAt(Camera.main.transform.position, Vector3.up);
		}
	}
}
