using System.Collections;
using UnityEngine;

public class CollapseEFX : MonoBehaviour
{
	[SerializeField]
	private float velocity = 10f;

	[SerializeField]
	private float duration = 3f;

	[SerializeField]
	private float angularVelocity = 10f;

	private float cleanupInterval = 0.2f;

	private Rigidbody[] rigidbodies;

	private IEnumerator Start()
	{
		rigidbodies = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			Rigidbody rigidbody = rigidbodies[i];
			Transform transform = rigidbody.transform;
			Vector3 vector = Vector3.Cross(transform.localPosition, Vector3.up).normalized;
			if ((double)transform.localPosition.sqrMagnitude < 0.01)
			{
				vector = Vector3.right;
			}
			rigidbody.AddTorque(-vector * angularVelocity, ForceMode.VelocityChange);
			Vector3 onUnitSphere = Random.onUnitSphere;
			if (onUnitSphere.y < 0f)
			{
				onUnitSphere.y = 0f - onUnitSphere.y;
			}
			rigidbody.AddForce((onUnitSphere + Vector3.up * 1.5f).normalized * velocity, ForceMode.VelocityChange);
		}
		yield return new WaitForSeconds(duration);
		for (int j = 0; j < rigidbodies.Length; j++)
		{
			rigidbodies[j].gameObject.SetActive(false);
			yield return new WaitForSeconds(cleanupInterval);
		}
		Object.Destroy(base.gameObject);
	}
}
