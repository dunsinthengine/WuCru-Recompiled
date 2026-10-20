using UnityEngine;

public class Sword : Collectible
{
	private float rotationSpeed = 42f;

	private void Start()
	{
	}

	private void Update()
	{
		base.gameObject.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
	}

	protected override void OnPickup(Character character)
	{
		Object.Destroy(base.gameObject);
	}
}
