using UnityEngine;

[RequireComponent(typeof(Destructable))]
public class BricksCollapse : MonoBehaviour
{
	[SerializeField]
	private int AmountLife = 1;

	[SerializeField]
	private Transform BricksToSpawn;

	[SerializeField]
	private Transform EffectToSpawn;

	[SerializeField]
	private Transform Effect2ToSpawn;

	[SerializeField]
	private bool spawnReward;

	private void Start()
	{
		Destructable component = GetComponent<Destructable>();
		if ((bool)component)
		{
			component.OnHit += OnHit;
		}
	}

	private void OnHit()
	{
		AmountLife--;
		if (AmountLife <= 0)
		{
			Transform transform = Object.Instantiate(BricksToSpawn);
			if ((bool)EffectToSpawn)
			{
				Transform transform2 = Object.Instantiate(EffectToSpawn);
				transform2.position = base.transform.position;
			}
			if ((bool)Effect2ToSpawn)
			{
				Transform transform3 = Object.Instantiate(Effect2ToSpawn);
				transform3.position = base.transform.position;
			}
			transform.position = base.transform.position;
			Object.Destroy(base.gameObject);
		}
	}
}
