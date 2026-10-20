using UnityEngine;

public class PreparingCombat_player : StateMachineBehaviour
{
	private Character character;

	private LevelNode combatSlot;

	private bool canAttack;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		animator.SetBool(VarHash.isMoving, true);
		combatSlot = GamePlayer.Instance.CombatManager.PlayerCombatSlot;
		if (combatSlot != null)
		{
			character.WalkTo(combatSlot);
		}
		else
		{
			Debug.LogWarning("No combat-slot in GamePlayer. Trying to use CurrentPositionNode instead");
			combatSlot = character.CurrentPositionNode;
			if (combatSlot == null)
			{
				Debug.LogWarning("Player could not get combatSlot!");
			}
		}
		canAttack = false;
		if (Time.timeScale < 1f)
		{
			GamePlayer.Instance.SetTimeScale(1f, 0.2f);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (combatSlot == null || animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (!canAttack)
		{
			float num = Vector3.Distance(LevelHelper.NodeToVector3(combatSlot), character.transform.position);
			if (num < 0.1f)
			{
				canAttack = true;
				animator.SetBool(VarHash.isMoving, false);
				character.EquipWeapon(true);
				animator.SetTrigger(TriggerHash.combatEnter);
				return;
			}
			Vector3 vector = LevelHelper.NodeToVector3(combatSlot) - character.transform.position;
			float magnitude = vector.magnitude;
			float num2 = GameController.Instance.PlayerProfile.MovementSpeed * character.LadderSpeedRatio * Time.deltaTime;
			if (num2 > magnitude)
			{
				num2 = magnitude;
			}
			character.transform.position = character.transform.position + vector.normalized * num2;
			Vector3 normalized = vector.normalized;
			normalized.y = 0f;
			character.transform.LookAt(character.transform.position + normalized);
		}
		else if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
		{
			character.transform.LookAt(GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform);
		}
	}
}
