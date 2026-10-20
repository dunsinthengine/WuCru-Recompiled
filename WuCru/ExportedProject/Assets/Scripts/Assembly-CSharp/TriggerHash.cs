using System.Collections.Generic;
using UnityEngine;

public static class TriggerHash
{
	private static List<int> triggerList = new List<int>();

	public static int tigersprint;

	public static int enemyAlert;

	public static int die;

	public static int sneakAttack;

	public static int combatEnter;

	public static int duellingWon;

	public static int duellingLost;

	public static int combatEnd;

	public static int attackSuccess;

	public static int attackFailed;

	public static int attackPerfect;

	public static int attackBlockedPerfect;

	public static int enemyAttack;

	public static int sneakAttackPerform;

	public static int trapped;

	public static int untrapped;

	public static int dismantle;

	public static int dismantled;

	public static int combatPrepare;

	public static int knockDown;

	public static int reset;

	public static int sleepToIdle;

	public static int missionAccomplished;

	public static int breakDuelling;

	public static int initiateSecondaryAttack;

	public static int climb;

	public static int stopClimbing;

	public static int hide;

	public static int move;

	public static int startSpinjitzu;

	public static int stopSpinjitzu;

	public static int enemyScare;

	public static int switchNinja;

	public static int wakeUp;

	public static int combatSpectate;

	public static int swingHitPrepare;

	public static int swingHitSucceed;

	public static int swingHitFail;

	public static int rangedSneakAttack;

	public static int elementalAttackAnticipation;

	public static int elementalAttackPerform;

	public static int elementalAttackCancel;

	public static int evilLaughStart;

	public static void Initialize()
	{
		triggerList.Clear();
		combatEnter = Animator.StringToHash("Enter combat");
		triggerList.Add(combatEnter);
		tigersprint = Animator.StringToHash("Tigersprint");
		triggerList.Add(tigersprint);
		enemyAlert = Animator.StringToHash("Enemy alert");
		triggerList.Add(enemyAlert);
		die = Animator.StringToHash("Die");
		triggerList.Add(die);
		sneakAttack = Animator.StringToHash("Sneak attack");
		triggerList.Add(sneakAttack);
		duellingWon = Animator.StringToHash("Duelling won");
		triggerList.Add(duellingWon);
		duellingLost = Animator.StringToHash("Duelling lost");
		triggerList.Add(duellingLost);
		combatEnd = Animator.StringToHash("Combat end");
		triggerList.Add(combatEnd);
		attackSuccess = Animator.StringToHash("Attack success");
		triggerList.Add(attackSuccess);
		attackFailed = Animator.StringToHash("Attack failed");
		triggerList.Add(attackFailed);
		attackPerfect = Animator.StringToHash("Attack perfect");
		triggerList.Add(attackPerfect);
		attackBlockedPerfect = Animator.StringToHash("Attack blocked perfect");
		triggerList.Add(attackBlockedPerfect);
		enemyAttack = Animator.StringToHash("Enemy attack");
		triggerList.Add(enemyAttack);
		sneakAttackPerform = Animator.StringToHash("Sneak attack perform");
		triggerList.Add(sneakAttackPerform);
		trapped = Animator.StringToHash("Trapped");
		triggerList.Add(trapped);
		untrapped = Animator.StringToHash("Untrapped");
		triggerList.Add(untrapped);
		dismantle = Animator.StringToHash("Dismantle");
		triggerList.Add(dismantle);
		dismantled = Animator.StringToHash("Dismantled");
		triggerList.Add(dismantled);
		combatPrepare = Animator.StringToHash("Prepare combat");
		triggerList.Add(combatPrepare);
		knockDown = Animator.StringToHash("Knock Down");
		triggerList.Add(knockDown);
		reset = Animator.StringToHash("Reset");
		triggerList.Add(reset);
		sleepToIdle = Animator.StringToHash("Sleep To Idle");
		triggerList.Add(sleepToIdle);
		missionAccomplished = Animator.StringToHash("Mission accomplished");
		triggerList.Add(missionAccomplished);
		breakDuelling = Animator.StringToHash("Break duelling");
		triggerList.Add(breakDuelling);
		initiateSecondaryAttack = Animator.StringToHash("Initiate secondary attack");
		triggerList.Add(initiateSecondaryAttack);
		climb = Animator.StringToHash("Climb");
		triggerList.Add(climb);
		stopClimbing = Animator.StringToHash("Stop climbing");
		triggerList.Add(stopClimbing);
		hide = Animator.StringToHash("Hide");
		triggerList.Add(hide);
		move = Animator.StringToHash("Move");
		triggerList.Add(move);
		startSpinjitzu = Animator.StringToHash("Start spinjitzu");
		triggerList.Add(startSpinjitzu);
		stopSpinjitzu = Animator.StringToHash("Stop spinjitzu");
		triggerList.Add(stopSpinjitzu);
		enemyScare = Animator.StringToHash("Enemy scare");
		triggerList.Add(enemyScare);
		switchNinja = Animator.StringToHash("Switch ninja");
		triggerList.Add(switchNinja);
		wakeUp = Animator.StringToHash("Wake up");
		triggerList.Add(wakeUp);
		combatSpectate = Animator.StringToHash("Combat spectate");
		triggerList.Add(combatSpectate);
		swingHitPrepare = Animator.StringToHash("Prepare swing hit");
		triggerList.Add(swingHitPrepare);
		swingHitSucceed = Animator.StringToHash("Swing hit succeed");
		triggerList.Add(swingHitSucceed);
		swingHitFail = Animator.StringToHash("Swing hit fail");
		triggerList.Add(swingHitFail);
		rangedSneakAttack = Animator.StringToHash("Ranged sneak attack");
		triggerList.Add(rangedSneakAttack);
		elementalAttackAnticipation = Animator.StringToHash("Elemental Attack Anticipation");
		triggerList.Add(elementalAttackAnticipation);
		elementalAttackPerform = Animator.StringToHash("Elemental Attack Perform");
		triggerList.Add(elementalAttackPerform);
		elementalAttackCancel = Animator.StringToHash("Elemental Attack Cancel");
		triggerList.Add(elementalAttackCancel);
		evilLaughStart = Animator.StringToHash("EvilLaughStart");
		triggerList.Add(evilLaughStart);
	}

	public static void ResetTriggers(Animator animator)
	{
		foreach (int trigger in triggerList)
		{
			AnimatorControllerParameter[] parameters = animator.parameters;
			foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
			{
				if (animatorControllerParameter.nameHash == trigger)
				{
					animator.ResetTrigger(trigger);
				}
			}
		}
	}
}
