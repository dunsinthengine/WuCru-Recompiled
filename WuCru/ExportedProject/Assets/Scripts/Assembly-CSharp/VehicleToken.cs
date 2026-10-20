using System.Collections;
using UnityEngine;

public class VehicleToken : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(ZoomToPlayer());
	}

	private void Update()
	{
		float zAngle = 360f * Time.deltaTime;
		base.transform.Rotate(0f, 0f, zAngle);
	}

	private IEnumerator ZoomToPlayer()
	{
		TraversalController traversalController = TraversalController.Instance;
		float startTime = Time.timeSinceLevelLoad;
		float delay = 1f;
		Vector3 velocity = traversalController.vehicleBaseDirection * traversalController.vehicleBaseSpeed;
		while (Time.timeSinceLevelLoad - startTime < delay)
		{
			base.transform.position += velocity * Time.deltaTime;
			yield return null;
		}
		Vector3 pos = base.transform.position;
		for (float t = 0f; t < 2f; t += Time.deltaTime)
		{
			base.transform.position = Vector3.Lerp(pos, TraversalController.Instance.car.transform.position, t / 2f);
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
