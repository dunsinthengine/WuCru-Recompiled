using UnityEngine;

public class SwapNinjaFX : MonoBehaviour
{
	private ParticleSystem[] particleSystems;

	private bool isDone;

	private float timeout = 2.5f;

	private float startTime;

	public bool IsDone
	{
		get
		{
			return isDone;
		}
	}

	private void Awake()
	{
		particleSystems = GetComponentsInChildren<ParticleSystem>();
		startTime = Time.time;
	}

	private void Update()
	{
		if (isDone)
		{
			return;
		}
		bool flag = true;
		if (Time.time - startTime < timeout)
		{
			ParticleSystem[] array = particleSystems;
			foreach (ParticleSystem particleSystem in array)
			{
				if (!particleSystem.isStopped)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			isDone = true;
			Object.Destroy(base.gameObject);
		}
	}
}
