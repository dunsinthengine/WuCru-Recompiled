using UnityEngine;

public class EnemyAttackSuccess_enemy : StateMachineBehaviour
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
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
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
