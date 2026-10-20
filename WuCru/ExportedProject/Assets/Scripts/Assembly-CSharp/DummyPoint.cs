using UnityEngine;

public class DummyPoint : MonoBehaviour
{
	public DummyPointType type;

	private void OnDrawGizmos()
	{
		if (type == DummyPointType.Hand)
		{
			Gizmos.color = Color.green;
		}
		else if (type == DummyPointType.LeftHand)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.yellow;
		}
		Gizmos.DrawSphere(base.gameObject.transform.position, 0.015f);
	}
}
