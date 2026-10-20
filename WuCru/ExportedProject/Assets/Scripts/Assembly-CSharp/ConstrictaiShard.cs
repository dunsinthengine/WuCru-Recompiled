using System.Collections;
using UnityEngine;

public class ConstrictaiShard : Collectible
{
	private void Start()
	{
		StartCoroutine(DelayedAttract());
	}

	private void Update()
	{
	}

	protected override void OnPickup(Character character)
	{
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DelayedAttract()
	{
		yield return new WaitForSeconds(1.2f);
		StartAttract();
	}
}
