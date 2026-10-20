using UnityEngine;

public static class TeahouseTriggers
{
	public static int idle;

	public static int levelUp;

	public static int power;

	public static int attack;

	public static int stillPose;

	public static int reset;

	static TeahouseTriggers()
	{
		idle = Animator.StringToHash("idle");
		levelUp = Animator.StringToHash("level up");
		power = Animator.StringToHash("power");
		attack = Animator.StringToHash("attack");
		stillPose = Animator.StringToHash("stillpose");
		reset = Animator.StringToHash("reset");
	}
}
