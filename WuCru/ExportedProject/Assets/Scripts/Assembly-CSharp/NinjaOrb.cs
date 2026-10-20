using UnityEngine;

public class NinjaOrb : Collectible
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	protected override void OnPickup(Character character)
	{
		GetComponent<BoxCollider>().enabled = false;
		Object.Destroy(base.gameObject);
	}
}
