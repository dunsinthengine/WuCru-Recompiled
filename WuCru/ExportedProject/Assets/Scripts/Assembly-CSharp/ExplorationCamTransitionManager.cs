using UnityEngine;

public class ExplorationCamTransitionManager
{
	private int currentIndex;

	private int newIndex;

	private float newIndexStartTime;

	private float newIndexDuration = 0.5f;

	private float shiftStartTime;

	private float shiftTimeout = 0.75f;

	private float shiftTransitionTime = 1.1f;

	public int CurrentIndex
	{
		get
		{
			return currentIndex;
		}
	}

	public int NewIndex
	{
		get
		{
			return newIndex;
		}
	}

	public float CalcTimeSinceNewIndex
	{
		get
		{
			return Time.time - newIndexStartTime;
		}
	}

	public float CalcTimeSinceShift
	{
		get
		{
			return Time.time - shiftStartTime;
		}
	}

	public float ShiftLerpValue
	{
		get
		{
			return Mathf.Clamp01((Time.time - shiftStartTime) / shiftTransitionTime);
		}
	}

	public bool EvaluateNewIndex(int index)
	{
		if (newIndex != index && currentIndex != index)
		{
			newIndex = index;
			newIndexStartTime = Time.time;
		}
		else if (newIndex != currentIndex && Time.time - newIndexStartTime > newIndexDuration && Time.time - shiftStartTime > shiftTimeout)
		{
			return true;
		}
		return false;
	}

	public void SetNewIndex()
	{
		currentIndex = newIndex;
		shiftStartTime = Time.time;
	}
}
