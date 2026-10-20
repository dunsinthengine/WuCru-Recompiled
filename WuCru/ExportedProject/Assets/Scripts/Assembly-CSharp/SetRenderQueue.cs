using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	public int renderQueue = 3000;

	private Material mMat;

	private void Start()
	{
		Renderer component = GetComponent<Renderer>();
		if (component == null)
		{
			ParticleSystem component2 = GetComponent<ParticleSystem>();
			if (component2 != null)
			{
				component = component2.GetComponent<ParticleSystemRenderer>();
			}
		}
		if (component != null)
		{
			mMat = new Material(component.sharedMaterial);
			mMat.renderQueue = renderQueue;
			component.material = mMat;
		}
	}

	private void OnDestroy()
	{
		if (mMat != null)
		{
			Object.Destroy(mMat);
		}
	}
}
