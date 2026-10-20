using UnityEngine;

public class PlayerAttackPerfect_player : StateMachineBehaviour
{
	private Character character;

	private StateMachineBehaviourTimer timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		DamageType type = DamageType.PerfectAttack;
		GamePlayer.Instance.CombatManager.SetWeaponTrailColor(character, componentInChildren, type);
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = true;
		}
		GamePlayer.Instance.CombatManager.ReportResult(CombatResult.PlayerAttackSuccess);
		timer = new StateMachineBehaviourTimer(0.15f);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamePlayer.Instance.Player.Character.GetSwordTappingIcon().Unfreeze();
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = false;
		}
		GamePlayer.Instance.SetTimeScale(1f, 0.2f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (timer != null && timer.Update())
		{
			GamePlayer.Instance.TimeBlink(0.5f);
		}
	}
}
