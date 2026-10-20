using System.Collections;
using UnityEngine;

public class TraversalMissile : MonoBehaviour
{
	private TraversalController traversalController;

	public Vehicle owner;

	public Vector3 baseVelocity;

	public float speed = 10f;

	public int damage = 10;

	public float acceleration;

	public float delay;

	private float startTime;

	public Renderer meshRenderer;

	public ParticleSystem particles;

	private void Start()
	{
		traversalController = Object.FindObjectOfType<TraversalController>();
		startTime = Time.timeSinceLevelLoad;
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad > startTime + delay)
		{
			speed += acceleration * Time.deltaTime;
		}
		Vector3 vector = baseVelocity + base.transform.forward * speed;
		base.transform.position = base.transform.position + vector * Time.deltaTime;
		if (Time.timeSinceLevelLoad > startTime + 5f)
		{
			traversalController.OnMissileTimeout(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		traversalController.OnMissileHit(this, other);
	}

	public void Explode()
	{
		meshRenderer.enabled = false;
		ParticleSystem.EmissionModule emission = particles.emission;
		emission.enabled = false;
		base.enabled = false;
		Collider componentInChildren = GetComponentInChildren<Collider>();
		if ((bool)componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		StartCoroutine(DestroyOnParticlesEnd());
	}

	private IEnumerator DestroyOnParticlesEnd()
	{
		while (particles.IsAlive())
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	public bool IsVisible()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		if (GeometryUtility.TestPlanesAABB(planes, GetComponent<BoxCollider>().bounds))
		{
			return true;
		}
		return false;
	}
}
