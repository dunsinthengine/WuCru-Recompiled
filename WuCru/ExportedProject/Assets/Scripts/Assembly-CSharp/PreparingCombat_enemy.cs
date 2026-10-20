using UnityEngine;

public class PreparingCombat_enemy : StateMachineBehaviour
{
	private Character character;

	private LevelNode combatSlot;

	private LevelNode spectatorSlot;

	private bool canAttack;

	private float destinationDist = 0.1f;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		combatSlot = null;
		spectatorSlot = null;
		canAttack = false;
		combatSlot = GamePlayer.Instance.CombatManager.RequestEnemyCombatSlot(character);
		if (combatSlot != null)
		{
			character.ResetPath();
			character.transform.LookAt(LevelHelper.NodeToVector3(combatSlot));
			character.WalkTo(combatSlot);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (GamePlayer.Instance.State != GamePlayerState.Combat || ((character.Controller as AIController).agent.State != AIAgentState.Combat && (character.Controller as AIController).agent.State != AIAgentState.Alert) || character.CurrentPositionNode.Elevation != GamePlayer.Instance.Player.Character.CurrentPositionNode.Elevation)
		{
			GamePlayer.Instance.CombatManager.ReleaseEnemyCombatSlot(character);
			animator.SetTrigger(TriggerHash.combatEnd);
			(character.Controller as AIController).agent.SetState(AIAgentState.Idle);
		}
		else
		{
			if (canAttack)
			{
				return;
			}
			if (character.IsDead())
			{
				GamePlayer.Instance.CombatManager.ReleaseEnemyCombatSlot(character);
				animator.SetTrigger(TriggerHash.die);
				return;
			}
			if (combatSlot != null || spectatorSlot != null)
			{
				character.MoveStep();
			}
			if (combatSlot != null)
			{
				if (canAttack)
				{
					return;
				}
				float num = Vector3.Distance(character.transform.position, LevelHelper.NodeToVector3(combatSlot));
				if (num < destinationDist || (!animator.GetBool(VarHash.isMoving) && character.CurrentPositionNode == combatSlot))
				{
					character.Animator.SetTrigger(TriggerHash.combatEnter);
					canAttack = true;
					character.EquipWeapon(true);
				}
				else if (!animator.GetBool(VarHash.isMoving))
				{
					if (character.CurrentDestination == null)
					{
						character.ResetPath();
						character.transform.LookAt(LevelHelper.NodeToVector3(combatSlot));
						character.WalkTo(combatSlot);
					}
					else
					{
						Debug.LogWarning("***** Not close enough to CombatSlot, but not moving! *****");
					}
				}
			}
			else
			{
				if (character.IsDead() || combatSlot != null)
				{
					return;
				}
				combatSlot = GamePlayer.Instance.CombatManager.RequestEnemyCombatSlot(character);
				if (combatSlot != null)
				{
					character.ResetPath();
					character.transform.LookAt(LevelHelper.NodeToVector3(combatSlot));
					character.WalkTo(combatSlot);
				}
				else if (spectatorSlot == null)
				{
					spectatorSlot = GamePlayer.Instance.CombatManager.RequestEnemySpectatorSlot(character);
					if (spectatorSlot != null)
					{
						character.ResetPath();
						character.transform.LookAt(LevelHelper.NodeToVector3(spectatorSlot));
						character.WalkTo(spectatorSlot);
					}
					else
					{
						Debug.LogWarning("Could not find combat slot OR spectator slot for enemy...");
						(character.Controller as AIController).agent.SetState(AIAgentState.Idle);
					}
				}
				else if (spectatorSlot != null)
				{
					float num2 = Vector3.Distance(character.transform.position, LevelHelper.NodeToVector3(spectatorSlot));
					if (num2 < destinationDist || (!animator.GetBool(VarHash.isMoving) && character.CurrentPositionNode == spectatorSlot))
					{
						animator.SetTrigger(TriggerHash.combatSpectate);
					}
				}
			}
		}
	}
}
