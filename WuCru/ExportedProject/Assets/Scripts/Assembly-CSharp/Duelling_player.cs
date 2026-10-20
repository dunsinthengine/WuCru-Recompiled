using Fabric;
using UnityEngine;

public class Duelling_player : StateMachineBehaviour
{
	private float ANIMATED_HAND_SHOW_DELAY = 0.2f;

	private float ANIMATED_HAND_GRACE_PERIOD = 0.8f;

	private Character character;

	private SwordFXs swordFX;

	private bool hasSeenBriefing;

	private float animatedHandShowDelay = 0.2f;

	private float animatedHandGracePeriod = 0.8f;

	private Vector3 animatedHandOffset = new Vector3(0f, 0.1f, 0f);

	private const string swordClashAudioEvent = "player/stick/sword/clash";

	private const string swordClashOverlayAudioEvent = "player/sword/clash/overlay";

	private const string playerStrainLoopAudioEvent = "player/combat/strain/loop/";

	private const string playerHeavyStrainAudioEvent = "player/combat/strain/heavy";

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		ScreenHud._pInstance._hudBattleMinigame.BeginMinigame();
		swordFX = character.GetComponentInChildren<SwordFXs>();
		EventManager.Instance.PostEvent("player/combat/strain/loop/", character.gameObject);
		EventManager.Instance.PostEvent("player/combat/strain/loop/", EventAction.SetVolume, 0f);
		if (character.GetWeapon().type == WeaponType.Nunchuck)
		{
			character.GetWeapon().GetComponentInChildren<Animation>().Play("stretched");
		}
		swordFX.grindFX.Play();
		hasSeenBriefing = GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.Duelling);
		animatedHandShowDelay = Time.time + ANIMATED_HAND_SHOW_DELAY;
		animatedHandGracePeriod = Time.time + ANIMATED_HAND_GRACE_PERIOD;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (swordFX != null)
		{
			swordFX.grindFX.Stop();
		}
		ScreenHud._pInstance._hudBattleMinigame.CloseMinigame();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null && GamePlayer.Instance.CombatManager.CurrentAttacker.Character.IsDead())
		{
			animator.SetTrigger(TriggerHash.combatEnd);
		}
		if (animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (hasSeenBriefing)
		{
			if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
			{
				character.transform.LookAt(GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform);
			}
			if (Input.GetMouseButtonDown(0))
			{
				GamePlayer.Instance.CombatManager.DuellingStatus = GamePlayer.Instance.CombatManager.DuellingStatus + GetDuellingStrength() * character.GetWeapon().GetDuellingStrengthFactor();
				if (swordFX == null)
				{
					Debug.LogWarning("SwordFX is null!");
				}
				else
				{
					int consecutiveHits = GamePlayer.Instance.CombatManager.consecutiveHits;
					if (consecutiveHits < 2)
					{
						swordFX.duellingFX.startSize = 0.3f;
					}
					else if (consecutiveHits < 3)
					{
						swordFX.duellingFX.startSize = 0.4f;
					}
					else if (consecutiveHits < 4)
					{
						swordFX.duellingFX.startSize = 0.5f;
					}
					else if (consecutiveHits < 5)
					{
						swordFX.duellingFX.startSize = 0.6f;
					}
					else
					{
						swordFX.duellingFX.startSize = 0.7f;
					}
					swordFX.duellingFX.Play();
				}
				PlaySwordClash();
			}
			animator.SetFloat(VarHash.duellingStatus, GamePlayer.Instance.CombatManager.DuellingStatus);
			if (GamePlayer.Instance.CombatManager.DuellingStatus < 0.1f)
			{
				EventManager.Instance.PostEvent("player/combat/strain/loop/", EventAction.StopSound);
				EventManager.Instance.PostEvent("player/combat/strain/heavy", character.transform.gameObject);
			}
			EventManager.Instance.PostEvent("player/combat/strain/loop/", EventAction.SetVolume, 1f - GamePlayer.Instance.CombatManager.DuellingStatus);
			return;
		}
		if (GamePlayer.Instance.CombatManager.CurrentAttacker != null)
		{
			character.transform.LookAt(GamePlayer.Instance.CombatManager.CurrentAttacker.Character.transform);
		}
		if (GamePlayer.Instance.CombatManager.DuellingStatus < 0.5f)
		{
			GamePlayer.Instance.CombatManager.DuellingStatus = 0.5f;
		}
		animator.SetFloat(VarHash.duellingStatus, GamePlayer.Instance.CombatManager.DuellingStatus);
		if (GamePlayer.Instance.CombatManager.DuellingStatus < 0.1f)
		{
			EventManager.Instance.PostEvent("player/combat/strain/loop/", EventAction.StopSound);
			EventManager.Instance.PostEvent("player/combat/strain/heavy", character.transform.gameObject);
		}
		EventManager.Instance.PostEvent("player/combat/strain/loop/", EventAction.SetVolume, 1f - GamePlayer.Instance.CombatManager.DuellingStatus);
		if (Time.time > animatedHandShowDelay && Input.GetMouseButtonDown(0) && Time.time > animatedHandGracePeriod)
		{
			GameControllerStatePlaying gameControllerStatePlaying = GameController.Instance.currentState as GameControllerStatePlaying;
			if (gameControllerStatePlaying != null && !gameControllerStatePlaying.IsConfirmingQuit)
			{
				Time.timeScale = 1f;
			}
			GamePlayer.Instance.CombatManager.DuellingStatus = GamePlayer.Instance.CombatManager.DuellingStatus + GetDuellingStrength() * character.GetWeapon().GetDuellingStrengthFactor();
			if (swordFX == null)
			{
				Debug.LogWarning("SwordFX is null!");
			}
			else
			{
				swordFX.duellingFX.startSize = 0.3f;
				swordFX.duellingFX.Play();
			}
			PlaySwordClash();
		}
	}

	private void PlaySwordClash()
	{
		int num = (int)(GamePlayer.Instance.CombatManager.DuellingStatus * 10f);
		EventManager.Instance.PostEvent("player/stick/sword/clash", character.gameObject);
		EventManager.Instance.PostEvent("player/sword/clash/overlay", EventAction.SetSwitch, num.ToString(), character.gameObject);
		EventManager.Instance.PostEvent("player/sword/clash/overlay", character.gameObject);
	}

	private float GetDuellingStrength()
	{
		if (GameController.Instance.PlayerProfile.Level >= 4)
		{
			return 0.16f;
		}
		if (GameController.Instance.PlayerProfile.Level >= 3)
		{
			return 0.18f;
		}
		if (GameController.Instance.PlayerProfile.Level >= 2)
		{
			return 0.2f;
		}
		return 0.22f;
	}
}
