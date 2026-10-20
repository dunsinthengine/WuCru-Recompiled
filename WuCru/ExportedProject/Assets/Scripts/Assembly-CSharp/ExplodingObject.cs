using UnityEngine;

public class ExplodingObject : MonoBehaviour
{
	public GameObject explodingObjectRoot;

	private void Awake()
	{
		base.enabled = false;
	}
}
