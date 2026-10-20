using UnityEngine;

public class SpectatingCombat_enemy : StateMachineBehaviour
{
	private Character character;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		character.transform.LookAt(GamePlayer.Instance.Player.Character.transform);
		character.EquipWeapon(true);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamePlayer.Instance.CombatManager.ReleaseEnemySpectatorSlot(character);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!animator.IsInTransition(layerIndex) && GamePlayer.Instance.CombatManager.NumEnemies < GamePlayer.Instance.CombatManager.EnemyCombatSlots.Length)
		{
			Debug.Log("NumEnemies: " + GamePlayer.Instance.CombatManager.NumEnemies);
			Debug.Log("CombatSlots: " + GamePlayer.Instance.CombatManager.EnemyCombatSlots.Length);
			animator.SetTrigger(TriggerHash.combatPrepare);
		}
	}
}
