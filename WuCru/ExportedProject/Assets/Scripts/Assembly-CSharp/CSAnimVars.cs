using System.Collections.Generic;
using UnityEngine;

public static class CSAnimVars
{
	private static List<int> triggerList = new List<int>();

	public static int movementSpeed;

	public static int deathAngle;

	public static int dying;

	public static void Initialize()
	{
		movementSpeed = Animator.StringToHash("MovementSpeed");
		deathAngle = Animator.StringToHash("DeathAngle");
		triggerList.Clear();
		dying = Animator.StringToHash("Dying");
		triggerList.Add(dying);
	}

	public static void ResetTriggers(Animator animator)
	{
		foreach (int trigger in triggerList)
		{
			animator.ResetTrigger(trigger);
		}
	}
}
