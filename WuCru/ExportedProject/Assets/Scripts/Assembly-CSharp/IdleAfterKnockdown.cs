using UnityEngine;

public class IdleAfterKnockdown : AIBehaviour
{
	private float _timer;

	private float _lastUpdateTime;

	public override void Initialize(AIController controller)
	{
		_lastUpdateTime = Time.time;
		_timer = 5f;
	}

	public override void Update(AIController controller)
	{
		float num = Time.time - _lastUpdateTime;
		_lastUpdateTime = Time.time;
		_timer -= num;
		if (_timer < 0f)
		{
			controller.Character.Animator.SetTrigger(TriggerHash.combatEnd);
			controller.agent.SetState(AIAgentState.Idle);
		}
	}
}
