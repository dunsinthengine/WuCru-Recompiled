using UnityEngine;

public class SecondaryAttack_enemy : StateMachineBehaviour
{
	private Character character;

	private SwordFXs swordFX;

	private SwordTappingIcon sti;

	private bool ended;

	private float originalGraceWindow = 0.3f;

	private float originalBeginWindow = 0.7f;

	private float originalEndWindow = 1.2f;

	private float originalDuration = 1.8f;

	private float startTime;

	private float emoticonDuration = 0.5f;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		startTime = Time.time;
		character = animator.transform.parent.GetComponent<Character>();
		swordFX = character.GetComponentInChildren<SwordFXs>();
		sti = character.GetSwordTappingIcon();
		HUDItemTracker hUDItemTracker = ScreenHud._pInstance._enemyIconTracker;
		if (GameController.Instance.Seed >= 5000)
		{
			hUDItemTracker = ScreenHud._pInstance._enemyIconTrackerCity;
		}
		hUDItemTracker.DismissIcon(character.gameObject.transform);
		ScreenHud._pInstance._enemyIconSeenTracker.DismissIcon(character.gameObject.transform);
		ScreenHud._pInstance._enemyIconSuspiciousTracker.DismissIcon(character.gameObject.transform);
		originalGraceWindow = sti.graceWindow;
		originalBeginWindow = sti.beginWindow;
		originalEndWindow = sti.endWindow;
		originalDuration = sti.duration;
		sti.graceWindow = 0.3f;
		sti.beginWindow = 0.35f;
		sti.endWindow = 0.95f;
		sti.duration = 1.1f;
		sti.Show(true, (GameController.Instance.PlayerProfile.WeaponLevel >= 2) ? true : false);
		ScreenHud._pInstance._swordTapTelegraph.Show(sti);
		ended = false;
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		DamageType type = DamageType.SecondaryAttack;
		GamePlayer.Instance.CombatManager.SetWeaponTrailColor(character, componentInChildren, type);
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamePlayer.Instance.CombatManager.SecondaryAttacker = null;
		sti.graceWindow = originalGraceWindow;
		sti.beginWindow = originalBeginWindow;
		sti.endWindow = originalEndWindow;
		sti.duration = originalDuration;
		sti.Show(false);
		ScreenHud._pInstance._swordTapTelegraph.Hide();
		MeleeWeaponTrail componentInChildren = character.swordTransform.GetComponentInChildren<MeleeWeaponTrail>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit = false;
		}
		HUDItemTracker hUDItemTracker = ScreenHud._pInstance._enemyIconTracker;
		if (GameController.Instance.Seed >= 5000)
		{
			hUDItemTracker = ScreenHud._pInstance._enemyIconTrackerCity;
		}
		hUDItemTracker.DismissIcon(character.gameObject.transform);
		ScreenHud._pInstance._enemyIconSeenTracker.DismissIcon(character.gameObject.transform);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (ended)
		{
			return;
		}
		if (sti.IsDone)
		{
			AttackSuccess(animator);
		}
		else
		{
			if (!Input.GetMouseButtonDown(0) || !sti.IsPointInsideIcon(Input.mousePosition))
			{
				return;
			}
			if (sti.IsInWindow)
			{
				if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
				{
					GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.breakDuelling);
				}
				GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.breakDuelling);
				GamePlayer.Instance.CombatManager.SecondaryAttacker.Character.Animator.SetTrigger(TriggerHash.enemyAttack);
				GamePlayer.Instance.CombatManager.CurrentAttacker = null;
				GamePlayer.Instance.CombatManager.CurrentAttacker = (character.Controller as AIController).agent;
				GamePlayer.Instance.CombatManager.SecondaryAttacker = null;
				ended = true;
				sti.Show(false);
				ScreenHud._pInstance._swordTapTelegraph.Hide();
				swordFX.parryFX.Play();
			}
			else if (!sti.IsInGrace)
			{
				AttackSuccess(animator);
			}
		}
	}

	private void AttackSuccess(Animator animator)
	{
		animator.SetTrigger(TriggerHash.attackSuccess);
		ended = true;
		Character character = GamePlayer.Instance.Player.Character;
		int enemyDamageAmount = GamePlayer.Instance.CombatManager.GetEnemyDamageAmount(DamageType.SecondaryAttack);
		Vector3 position = this.character.transform.position;
		AIAgentType agentType = (this.character.Controller as AIController).agent.GetAgentType();
		character.TakeDamage(enemyDamageAmount, position, DamageType.SecondaryAttack, agentType);
	}
}
