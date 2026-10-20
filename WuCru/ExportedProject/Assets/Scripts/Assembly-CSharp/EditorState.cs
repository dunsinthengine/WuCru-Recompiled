using UnityEngine;

public class EditorState : MonoBehaviour
{
	public virtual void Begin()
	{
	}

	public virtual void End()
	{
	}

	public virtual void Pause()
	{
	}

	public virtual void Resume()
	{
	}

	public virtual void Update()
	{
	}

	private void Awake()
	{
		base.enabled = false;
	}
}
