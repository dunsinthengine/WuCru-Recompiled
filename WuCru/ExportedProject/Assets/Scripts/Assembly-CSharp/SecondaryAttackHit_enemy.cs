using Fabric;
using UnityEngine;

public class SecondaryAttackHit_enemy : StateMachineBehaviour
{
	private Character character;

	private CharacterFXs characterFX;

	public bool hasDealtDamage;

	private const string PlayerIsHitAudioEvent = "player/is/hit";

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		characterFX = GamePlayer.Instance.Player.Character.GetComponentInChildren<CharacterFXs>();
		hasDealtDamage = false;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamePlayer.Instance.CombatManager.SecondaryAttacker = null;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime > stateInfo.length / 2f && !hasDealtDamage && !(GamePlayer.Instance.CombatManager.CurrentAttacker == null))
		{
			hasDealtDamage = true;
			Character character = GamePlayer.Instance.Player.Character;
			int enemyDamageAmount = GamePlayer.Instance.CombatManager.GetEnemyDamageAmount(DamageType.SecondaryAttack);
			AIAgentType agentType = GamePlayer.Instance.CombatManager.CurrentAttacker.GetAgentType();
			character.TakeDamage(enemyDamageAmount, this.character.transform.position, DamageType.SecondaryAttack, agentType);
			characterFX.impactFX.Play();
			EventManager.Instance.PostEvent("player/is/hit", animator.transform.parent.gameObject);
			GamePlayer.Instance.TimeBlink(0.05f);
		}
	}
}
