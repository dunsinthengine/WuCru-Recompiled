using UnityEngine;

public class DelayedSelfDestroy : MonoBehaviour
{
	public float delay = 1f;

	private float _t;

	private void OnEnable()
	{
		_t = delay;
	}

	public void RefreshTimer()
	{
		_t = delay;
	}

	private void Update()
	{
		_t -= Time.deltaTime;
		if (_t <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
