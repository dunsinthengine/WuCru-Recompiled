using UnityEngine;

public class CombatReady_enemy : StateMachineBehaviour
{
	private Character character;

	private int unlockAttackInBackLevel = 3;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		animator.SetBool(VarHash.isMoving, false);
		character.transform.LookAt(GamePlayer.Instance.Player.Character.transform);
		character.CenterOnPositionTile();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (character.CurrentStateNameHash() == StateHash.combatReady && CanDoSecondaryAttack() && GamePlayer.Instance.CombatManager.TimeSinceLastSecondaryAttack > SecondaryAttackInterval() && Random.value < 0.1f)
		{
			GamePlayer.Instance.CombatManager.SecondaryAttacker = (character.Controller as AIController).agent;
		}
	}

	private bool CanDoSecondaryAttack()
	{
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null && GamePlayer.Instance.CombatManager.SecondaryAttacker == null && GamePlayer.Instance.Player.Character.CurrentStateNameHash() == StateHash.duelling && GamePlayer.Instance.CombatManager.CurrentAttacker.Character.CurrentStateNameHash() == StateHash.duelling && GameController.Instance.PlayerProfile.Level >= unlockAttackInBackLevel && !character.IsDead())
		{
			AIController aIController = character.Controller as AIController;
			if (aIController.agent is ElementalBossEnemy)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private float SecondaryAttackInterval()
	{
		int num = 10;
		float num2 = 4f;
		float num3 = 8f;
		float num4 = 1f - (float)Mathf.Clamp(GameController.Instance.PlayerProfile.Level - unlockAttackInBackLevel, 0, num) / (float)num;
		return num2 + (num3 - num2) * num4;
	}
}
