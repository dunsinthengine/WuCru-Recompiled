using UnityEngine;

public static class TeahouseVariables
{
	public static int weaponType;

	public static int stillPoses;

	static TeahouseVariables()
	{
		weaponType = Animator.StringToHash("weapon type");
		stillPoses = Animator.StringToHash("stillposes");
	}
}
