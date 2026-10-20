using Fabric;
using UnityEngine;

public class EnemyAttackSuccess_player : StateMachineBehaviour
{
	private const string PlayerIsHitAudioEvent = "player/is/hit";

	private Character character;

	private CharacterFXs characterFX;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		characterFX = character.GetComponentInChildren<CharacterFXs>();
		int enemyDamageAmount = GamePlayer.Instance.CombatManager.GetEnemyDamageAmount(DamageType.PrimaryAttack);
		Vector3 position = GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform.position;
		AIAgentType agentType = GamePlayer.Instance.CombatManager.CurrentAttacker.GetAgentType();
		character.TakeDamage(enemyDamageAmount, position, DamageType.PrimaryAttack, agentType);
		characterFX.impactFX.Play();
		EventManager.Instance.PostEvent("player/is/hit", animator.transform.parent.gameObject);
		GamePlayer.Instance.TimeBlink(0.05f);
		GamePlayer.Instance.CombatManager.ReportResult(CombatResult.EnemyAttackSuccess);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
