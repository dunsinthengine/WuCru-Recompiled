using UnityEngine;

public class TraversalEnemy : MonoBehaviour
{
	private BoxCollider boxCollider;

	protected TraversalEnemyState state;

	public virtual void OnStateChange()
	{
	}

	public virtual void Start()
	{
		boxCollider = base.gameObject.GetComponent<BoxCollider>();
	}

	public virtual void Update()
	{
		if (state == TraversalEnemyState.Invisible && IsVisible())
		{
			state = TraversalEnemyState.Visible;
			OnStateChange();
		}
		if (state == TraversalEnemyState.Visible && !IsVisible())
		{
			state = TraversalEnemyState.Invisible;
			OnStateChange();
		}
	}

	protected bool IsVisible()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		if (GeometryUtility.TestPlanesAABB(planes, boxCollider.bounds))
		{
			return true;
		}
		return false;
	}
}
