using UnityEngine;

public class TraversalCamera : MonoBehaviour
{
	public GameObject target;

	public Vector3 offset;

	public Transform[] planeSpawnPoints;

	private Vector3 baseOffset;

	public void SetTarget(GameObject go)
	{
		target = go;
		baseOffset = base.transform.position - target.transform.position;
	}

	private void Awake()
	{
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			base.transform.localPosition = baseOffset + target.transform.localPosition + offset;
		}
	}
}
