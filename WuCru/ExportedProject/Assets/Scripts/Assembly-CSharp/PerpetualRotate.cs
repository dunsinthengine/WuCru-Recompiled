using UnityEngine;

public class PerpetualRotate : MonoBehaviour
{
	public Vector3 _vel;

	private void Update()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles += _vel * Time.deltaTime;
		base.transform.localEulerAngles = localEulerAngles;
	}
}
