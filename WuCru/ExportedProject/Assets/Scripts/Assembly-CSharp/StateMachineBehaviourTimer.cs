using UnityEngine;

public class StateMachineBehaviourTimer
{
	private float startTime;

	private float duration;

	private bool hasTriggered;

	public StateMachineBehaviourTimer(float duration)
	{
		startTime = Time.time;
		this.duration = duration;
		hasTriggered = false;
	}

	public bool Update()
	{
		if (hasTriggered)
		{
			return false;
		}
		if (Time.time - startTime > duration)
		{
			hasTriggered = true;
			return true;
		}
		return false;
	}
}
