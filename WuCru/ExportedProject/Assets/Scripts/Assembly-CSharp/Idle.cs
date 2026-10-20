using UnityEngine;

public class Idle : StateMachineBehaviour
{
	private Character character;

	private bool hideInitiated;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		character = (animator.transform.parent != null) ? animator.transform.parent.GetComponent<Character>() : animator.GetComponentInParent<Character>();
		if (character != null)
		{
			character.EquipWeapon(false);
		}
		hideInitiated = false;
		animator.SetFloat(VarHash.movementSpeed, 0f);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (character == null && animator != null)
		{
			character = (animator.transform.parent != null) ? animator.transform.parent.GetComponent<Character>() : animator.GetComponentInParent<Character>();
		}
		if (character == null)
		{
			return;
		}
		if (character.isPlayer && character.IsIdle() && !ScreenResults._pInResults)
		{
			if (character.Controller != null)
			{
				(character.Controller as PlayerController).HandlePicking();
			}
			else if (!character.IsDead())
			{
				Debug.LogWarning("Character.Controller is null: ", character.gameObject);
			}
		}
		if (!character.isPlayer || character.IsSneakAttacking() || character.IsPreparingSneakAttack() || animator.IsInTransition(layerIndex) || !character.CurrentPositionNode.ContainsObjectType(LevelNodeObjectType.Bush) || hideInitiated)
		{
			return;
		}
		foreach (GameObject @object in character.CurrentPositionNode.Objects)
		{
			Bush componentInChildren = @object.GetComponentInChildren<Bush>();
			if (componentInChildren != null)
			{
				hideInitiated = true;
				componentInChildren.OnPicked(character, true);
			}
		}
	}
}
