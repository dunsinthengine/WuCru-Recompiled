using UnityEngine;

public class Destructable : MonoBehaviour
{
	public delegate void OnHitDelegate();

	public event OnHitDelegate OnHit;

	public void Hit()
	{
		if (OnHit != null)
		{
			OnHit();
		}
	}
}
