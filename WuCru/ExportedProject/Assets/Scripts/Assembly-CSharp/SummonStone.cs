using UnityEngine;

public class SummonStone : Collectible
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	protected override void OnPickup(Character character)
	{
		Object.Destroy(base.gameObject);
	}
}
