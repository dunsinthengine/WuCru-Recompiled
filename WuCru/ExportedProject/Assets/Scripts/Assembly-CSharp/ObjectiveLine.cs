using System;
using UnityEngine;

[Serializable]
public class ObjectiveLine
{
	[SerializeField]
	public int season;

	[SerializeField]
	public int chapter;

	[SerializeField]
	public string areaMap = "S1A1";

	[SerializeField]
	public ObjectiveType objectiveType;

	[SerializeField]
	public int amountToComplete;

	[SerializeField]
	public ObjectiveObjectType objectType;

	[SerializeField]
	public ObjectiveObjectSpawnType objectSpawnType;

	[SerializeField]
	public float spawnChance = 1f;

	[SerializeField]
	public int seed = -1;

	[SerializeField]
	public int levelSequence = -1;

	[SerializeField]
	public ObjectivePatternID patternID;

	[SerializeField]
	public string descriptionStringID = "o_1";

	[SerializeField]
	private bool completed;

	[SerializeField]
	public ObjectiveEvent objectiveEvent;

	[SerializeField]
	public int objectCounter;

	public bool Completed
	{
		get
		{
			return completed;
		}
		set
		{
			completed = value;
			if (value)
			{
				GameController.Instance.ObjectiveController.OnObjectiveCompleted(this);
			}
		}
	}
}
