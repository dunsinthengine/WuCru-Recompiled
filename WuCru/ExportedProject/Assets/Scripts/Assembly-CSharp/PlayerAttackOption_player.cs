using Fabric;
using UnityEngine;

public class PlayerAttackOption_player : StateMachineBehaviour
{
	private Character character;

	private SwordTappingIcon sti;

	private bool ended;

	private int unlockDeflectLevel = 3;

	private float animatedHandShowDuration = 0.4f;

	private float animatedHandPauseDuration = 0.8f;

	private float animatedHandGracePeriod = 0.5f;

	private float animatedHandStartTime;

	private float animatedHandStartDelay = 0.3f;

	private bool animatedHandStarted;

	private Vector3 animatedHandOffset = new Vector3(0f, -0f, 0f);

	private const string AttackSuccessAudioEvent = "player/attack/success";

	private const string AttackFailAudioEvent = "player/attack/fail";

	private const string WeaponWhooshAudioEvent = "player/weapon/swoosh";

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		sti = GamePlayer.Instance.Player.Character.GetSwordTappingIcon();
		sti.Show(true, (GameController.Instance.PlayerProfile.WeaponLevel >= 1) ? true : false);
		ScreenHud._pInstance._swordTapTelegraph.Show(sti);
		sti.Apply(character.GetWeapon().GetAttackTappingSettings());
		ended = false;
		if (character.GetWeapon().type == WeaponType.Nunchuck)
		{
			character.GetWeapon().GetComponentInChildren<Animation>().Play("anticipation");
		}
		if (animator.GetFloat(VarHash.attackType) == 0f)
		{
			animator.SetFloat(VarHash.attackType, 1f);
		}
		else
		{
			animator.SetFloat(VarHash.attackType, 0f);
		}
		animatedHandStartDelay += Time.realtimeSinceStartup;
		GamePlayer.Instance.SetTimeScale(0.2f, 1f);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		sti.Show(false);
		ScreenHud._pInstance._swordTapTelegraph.Hide();
		GamePlayer.Instance.SetTimeScale(1f, 0.2f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (ended || animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.PlayerAttack))
		{
			if (Input.GetMouseButtonDown(0) && sti.IsPointInsideIcon(Input.mousePosition))
			{
				if (sti.IsInWindow)
				{
					if (GamePlayer.Instance.CombatManager.consecutiveHits >= 4 && GameController.Instance.PlayerProfile.WeaponLevel >= 4)
					{
						GamePlayer.Instance.Player.Character.StartSpinjitzu(false);
					}
					else if (sti.IsInPerfectWindow)
					{
						sti.Freeze();
						EventManager.Instance.PostEvent("player/weapon/swoosh", character.gameObject);
						EventManager.Instance.PostEvent("player/attack/success", character.gameObject);
						animator.SetTrigger(TriggerHash.attackPerfect);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackPerfect);
						GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.white);
						GamePlayer.Instance.TimeBlink(1f);
					}
					else if (CanDoDeflect() && GamePlayer.Instance.CombatManager.CurrentAttacker.combatDeflectChance > Random.Range(0f, 1f))
					{
						animator.SetTrigger(TriggerHash.attackFailed);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackFailed);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.ShowCombatEventType(CombatSpecialEventType.Deflection);
						ended = true;
						sti.Show(false);
						ScreenHud._pInstance._swordTapTelegraph.Hide();
						GamePlayer.Instance.SetTimeScale(1f, 0.2f);
					}
					else
					{
						EventManager.Instance.PostEvent("player/weapon/swoosh", character.gameObject);
						EventManager.Instance.PostEvent("player/attack/success", character.gameObject);
						animator.SetTrigger(TriggerHash.attackSuccess);
						GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackSuccess);
					}
					ended = true;
					sti.Show(false);
					ScreenHud._pInstance._swordTapTelegraph.Hide();
					if (character.GetWeapon().type == WeaponType.Nunchuck)
					{
						character.GetWeapon().GetComponentInChildren<Animation>().Play("attack");
					}
					GamePlayer.Instance.SetTimeScale(1f, 0.2f);
				}
				else if (!sti.IsInGrace)
				{
					EventManager.Instance.PostEvent("player/attack/fail", character.gameObject);
					animator.SetTrigger(TriggerHash.attackFailed);
					GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackFailed);
					ended = true;
					sti.Show(false);
					ScreenHud._pInstance._swordTapTelegraph.Hide();
					if (character.GetWeapon().type == WeaponType.Nunchuck)
					{
						character.GetWeapon().GetComponentInChildren<Animation>().Play("attack");
					}
					GamePlayer.Instance.SetTimeScale(1f, 0.2f);
				}
			}
			else if (sti.IsDone)
			{
				animator.SetTrigger(TriggerHash.attackFailed);
				GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackFailed);
				ended = true;
				sti.Show(false);
				ScreenHud._pInstance._swordTapTelegraph.Hide();
				GamePlayer.Instance.SetTimeScale(1f, 0.2f);
			}
		}
		else if (!animatedHandStarted && Time.realtimeSinceStartup > animatedHandStartDelay)
		{
			animatedHandStarted = true;
			animatedHandStartTime = Time.time;
			animatedHandGracePeriod += animatedHandStartTime;
		}
		else if (animatedHandStarted && Input.GetMouseButtonDown(0) && sti.IsPointInsideIcon(Input.mousePosition) && Time.realtimeSinceStartup > animatedHandGracePeriod)
		{
			EventManager.Instance.PostEvent("player/weapon/swoosh", character.gameObject);
			EventManager.Instance.PostEvent("player/attack/success", character.gameObject);
			animator.SetTrigger(TriggerHash.attackSuccess);
			GamePlayer.Instance.CombatManager.CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.attackSuccess);
			ended = true;
			sti.Show(false);
			ScreenHud._pInstance._swordTapTelegraph.Hide();
			GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.PlayerAttack);
		}
	}

	private bool CanDoDeflect()
	{
		if (GameController.Instance.PlayerProfile.Level >= unlockDeflectLevel)
		{
			return true;
		}
		return false;
	}
}
