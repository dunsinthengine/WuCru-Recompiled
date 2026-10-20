using UnityEngine;

public class EvilLaugh : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Evil Laugh");
		AudioPlayer.PlaySound("npc/enemy/laugh");
	}
}
