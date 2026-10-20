using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	public Transform target;

	public Quaternion rotation;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.rotation = rotation;
	}
}
