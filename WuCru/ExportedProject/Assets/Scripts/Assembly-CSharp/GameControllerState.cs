using UnityEngine;

public abstract class GameControllerState : MonoBehaviour
{
	public virtual void BeginState()
	{
	}

	public virtual void EndState()
	{
	}

	public virtual void UpdateState()
	{
	}

	private void Awake()
	{
		base.enabled = false;
	}
}
