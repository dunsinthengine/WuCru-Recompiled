using UnityEngine;

public class DAX_Rotator : MonoBehaviour
{
	public float AngularSpeed = 360f;

	private void Awake()
	{
	}

	private void Update()
	{
		base.gameObject.transform.Rotate(0f, 0f, AngularSpeed * Time.deltaTime, Space.Self);
	}
}
