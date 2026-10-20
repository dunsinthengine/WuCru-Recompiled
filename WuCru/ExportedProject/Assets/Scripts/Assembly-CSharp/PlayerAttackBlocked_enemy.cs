using Fabric;
using UnityEngine;

public class PlayerAttackBlocked_enemy : StateMachineBehaviour
{
	private const string SwordParryAudioEvent = "player/sword/parry";

	private Character character;

	private SwordFXs swordFX;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = animator.transform.parent.GetComponent<Character>();
		swordFX = character.GetComponentInChildren<SwordFXs>();
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.3f, 2.5f, Color.blue);
		swordFX.parryFX.Play();
		GamePlayer.Instance.TimeBlink(0.1f);
		EventManager.Instance.PostEvent("player/sword/parry", animator.transform.parent.gameObject);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
