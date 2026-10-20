using UnityEngine;

public static class VarHash
{
	public static int isMoving;

	public static int duellingStatus;

	public static int movementSpeed;

	public static int climbingDir;

	public static int climbingLevels;

	public static int weaponType;

	public static int attackType;

	public static void Initialize()
	{
		isMoving = Animator.StringToHash("IsMoving");
		duellingStatus = Animator.StringToHash("Duelling status");
		movementSpeed = Animator.StringToHash("Movement speed");
		climbingDir = Animator.StringToHash("Climbing dir");
		climbingLevels = Animator.StringToHash("Climbing levels");
		weaponType = Animator.StringToHash("Weapon type");
		attackType = Animator.StringToHash("Attack type");
	}
}
