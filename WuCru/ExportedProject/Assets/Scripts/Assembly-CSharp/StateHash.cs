using System.Collections.Generic;
using UnityEngine;

public static class StateHash
{
	public static Dictionary<int, string> hashToName = new Dictionary<int, string>();

	public static int idle;

	public static int moving;

	public static int climbing;

	public static int dismantling;

	public static int trapped;

	public static int performSneakAttack;

	public static int preparingSneakAttack;

	public static int hiding;

	public static int preparingCombat;

	public static int dead;

	public static int enemyAlerted;

	public static int tigersprinting;

	public static int combatReady;

	public static int duelling;

	public static int enemyAttackOption;

	public static int playerAttackOption;

	public static int enemyAttackSuccess;

	public static int enemyAttackBlocked;

	public static int enemyAttackBlockedPerfect;

	public static int playerAttackSuccess;

	public static int playerAttackBlocked;

	public static int playerAttackPerfect;

	public static int missionEnding;

	public static int secondaryAttack;

	public static int secondaryAttackHit;

	public static int secondaryAttackBlocked;

	public static int prepareSpinjitzu;

	public static int spintitzu;

	public static int endSpinjitzu;

	public static int enemyScared;

	public static int smoke;

	public static int sleep;

	public static int wakingUp;

	public static int spectatingCombat;

	public static int preparingSwingHit;

	public static int swingHitSuccess;

	public static int swingHitFailed;

	public static int rangedSneakAttack;

	public static int rangedSneakAttackAnticipation;

	public static int elementalAttackAnticipation;

	public static int elementalAttackPerform;

	public static int knockedDown;

	public static void Initialize()
	{
		hashToName.Clear();
		idle = Animator.StringToHash("Idle");
		hashToName.Add(idle, "Idle");
		moving = Animator.StringToHash("Moving");
		hashToName.Add(moving, "Moving");
		climbing = Animator.StringToHash("Climbing");
		hashToName.Add(climbing, "Climbing");
		dismantling = Animator.StringToHash("Dismantling");
		hashToName.Add(dismantling, "Dismantling");
		trapped = Animator.StringToHash("Trapped");
		hashToName.Add(trapped, "Trapped");
		performSneakAttack = Animator.StringToHash("Performing Sneak Attack");
		hashToName.Add(performSneakAttack, "PerformingSneakAttack");
		preparingSneakAttack = Animator.StringToHash("Preparing Sneak Attack");
		hashToName.Add(preparingSneakAttack, "PreparingSneakAttack");
		hiding = Animator.StringToHash("Hiding");
		hashToName.Add(hiding, "Hiding");
		preparingCombat = Animator.StringToHash("Preparing Combat");
		hashToName.Add(preparingCombat, "PreparingCombat");
		dead = Animator.StringToHash("Dead");
		hashToName.Add(dead, "Dead");
		enemyAlerted = Animator.StringToHash("Enemy Alerted");
		hashToName.Add(enemyAlerted, "EnemyAlerted");
		tigersprinting = Animator.StringToHash("Tigersprinting");
		hashToName.Add(tigersprinting, "Tigersprinting");
		combatReady = Animator.StringToHash("Combat Ready");
		hashToName.Add(combatReady, "Combat Ready");
		duelling = Animator.StringToHash("Duelling");
		hashToName.Add(duelling, "Duelling");
		enemyAttackOption = Animator.StringToHash("Enemy Attack Option");
		hashToName.Add(enemyAttackOption, "EnemyAttackOption");
		playerAttackOption = Animator.StringToHash("Player Attack Option");
		hashToName.Add(playerAttackOption, "PlayerAttackOption");
		enemyAttackSuccess = Animator.StringToHash("Enemy Attack Success");
		hashToName.Add(enemyAttackSuccess, "EnemyAttackSuccess");
		enemyAttackBlocked = Animator.StringToHash("Enemy Attack Blocked");
		hashToName.Add(enemyAttackBlocked, "EnemyAttackBlocked");
		enemyAttackBlockedPerfect = Animator.StringToHash("Enemy Attack Blocked Perfect");
		hashToName.Add(enemyAttackBlockedPerfect, "EnemyAttackBlockedPerfect");
		playerAttackSuccess = Animator.StringToHash("Player Attack Success");
		hashToName.Add(playerAttackSuccess, "PlayerAttackSuccess");
		playerAttackBlocked = Animator.StringToHash("Player Attack Blocked");
		hashToName.Add(playerAttackBlocked, "PlayerAttackBlocked");
		playerAttackPerfect = Animator.StringToHash("Player Attack Perfect");
		hashToName.Add(playerAttackPerfect, "PlayerAttackPerfect");
		missionEnding = Animator.StringToHash("Mission Ending");
		hashToName.Add(missionEnding, "MissionEnding");
		secondaryAttack = Animator.StringToHash("Secondary Attack");
		hashToName.Add(secondaryAttack, "SecondaryAttack");
		secondaryAttackHit = Animator.StringToHash("Secondary Attack Hit");
		hashToName.Add(secondaryAttackHit, "SecondaryAttackHit");
		secondaryAttackBlocked = Animator.StringToHash("Secondary Attack Blocked");
		hashToName.Add(secondaryAttackBlocked, "SecondaryAttackBlocked");
		prepareSpinjitzu = Animator.StringToHash("Prepare Spinjitzu");
		hashToName.Add(prepareSpinjitzu, "PrepareSpinjitzu");
		spintitzu = Animator.StringToHash("Spinjitzu");
		hashToName.Add(spintitzu, "Spinjitzu");
		endSpinjitzu = Animator.StringToHash("End Spinjitzu");
		hashToName.Add(endSpinjitzu, "EndSpinjitzu");
		enemyScared = Animator.StringToHash("Enemy Scared");
		hashToName.Add(enemyScared, "EnemyScared");
		smoke = Animator.StringToHash("Smoke");
		hashToName.Add(smoke, "Smoke");
		sleep = Animator.StringToHash("Sleep");
		hashToName.Add(sleep, "Sleep");
		wakingUp = Animator.StringToHash("Waking up");
		hashToName.Add(wakingUp, "WakingUp");
		spectatingCombat = Animator.StringToHash("Spectating Combat");
		hashToName.Add(spectatingCombat, "SpectatingCombat");
		preparingSwingHit = Animator.StringToHash("Preparing Swing Hit");
		hashToName.Add(preparingSwingHit, "PreparingSwingHit");
		swingHitSuccess = Animator.StringToHash("Swing Hit Success");
		hashToName.Add(swingHitSuccess, "SwingHitSuccess");
		swingHitFailed = Animator.StringToHash("Swing hit Failed");
		hashToName.Add(swingHitFailed, "SwingHitFailed");
		rangedSneakAttack = Animator.StringToHash("Ranged Sneak Attack");
		hashToName.Add(rangedSneakAttack, "RangedSneakAttack");
		rangedSneakAttackAnticipation = Animator.StringToHash("Ranged Sneak Attack Anticipation");
		hashToName.Add(rangedSneakAttackAnticipation, "RangedSneakAttackAnticipation");
		elementalAttackAnticipation = Animator.StringToHash("Elemental Attack Anticipation");
		hashToName.Add(elementalAttackAnticipation, "ElementalAttackAnticipation");
		elementalAttackPerform = Animator.StringToHash("Elemental Attack");
		hashToName.Add(elementalAttackPerform, "ElementalAttack");
		knockedDown = Animator.StringToHash("Knock Down");
		hashToName.Add(knockedDown, "Knock Down");
	}
}
