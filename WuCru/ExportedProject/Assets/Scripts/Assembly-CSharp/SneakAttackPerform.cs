using Fabric;
using UnityEngine;

public class SneakAttackPerform : StateMachineBehaviour
{
	private const string PlayerSwordAttackAudioEvent = "player/combat/sword/hit/enemy";

	private Character character;

	private Character enemy;

	private float attackDelay = 0.45f;

	private bool hasFired;

	private float areaDamageRadius = 2.5f;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		enemy = character.Controller.CurrentEnemy;
		AIBeacon.Instance.ReportEvent((enemy.Controller as AIController).agent, character.CurrentPositionNode, enemy.transform.position, 1.5f, 1f);
		hasFired = false;
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		DamageType type = DamageType.SneakAttack;
		GamePlayer.Instance.CombatManager.SetWeaponTrailColor(character, componentInChildren, type);
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = true;
		}
		if (character.GetWeapon().type == WeaponType.Nunchuck)
		{
			character.GetWeapon().GetComponentInChildren<Animation>().Play("attack");
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger(TriggerHash.sneakAttack);
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = false;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!(stateInfo.normalizedTime > attackDelay) || hasFired)
		{
			return;
		}
		hasFired = true;
		if (enemy.IsSnakeBoss())
		{
			enemy.TakeDamage(GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.SneakAttack) / 4, character.transform.position, DamageType.SneakAttack);
		}
		else if (enemy.IsElementalBoss())
		{
			if (GamePlayer.Instance.State == GamePlayerState.Combat)
			{
				(enemy.Controller as AIController).agent.SetState(AIAgentState.Combat);
			}
			else
			{
				if (enemy.elementalPower == GamePlayer.Instance.Player.Character.elementalPower)
				{
					enemy.TakeDamage(GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.SneakAttack), character.transform.position, DamageType.SneakAttack);
				}
				(enemy.Controller as AIController).agent.SetState(AIAgentState.Alert);
			}
		}
		else
		{
			enemy.TakeDamage(GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.SneakAttack), character.transform.position, DamageType.SneakAttack);
		}
		if (GameController.Instance.PlayerProfile.WeaponLevel >= 3)
		{
			AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
			foreach (AIAgent aIAgent in agents)
			{
				if (aIAgent.Character.CurrentPositionNode.Elevation == character.CurrentPositionNode.Elevation && Vector3.Distance(aIAgent.Character.transform.position, character.transform.position) <= areaDamageRadius)
				{
					aIAgent.Character.TakeDamage(GamePlayer.Instance.CombatManager.GetPlayerDamageAmount(DamageType.SneakAttack), character.transform.position, DamageType.SneakAttack);
				}
			}
		}
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.red);
		EventManager.Instance.PostEvent("player/combat/sword/hit/enemy", character.gameObject);
		GamePlayer.Instance.TimeBlink(0.05f);
	}
}
