using UnityEngine;

public class PlayerAttackSuccess_player : StateMachineBehaviour
{
	private Character character;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		DamageType type = DamageType.PrimaryAttack;
		GamePlayer.Instance.CombatManager.SetWeaponTrailColor(character, componentInChildren, type);
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = true;
		}
		animator.speed = GamePlayer.Instance.CombatManager.DamageMultiplier;
		GamePlayer.Instance.CombatManager.ReportResult(CombatResult.PlayerAttackSuccess);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.speed = 1f;
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = false;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
