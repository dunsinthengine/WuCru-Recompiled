using UnityEngine;

public class ElementalAttackAnticipation : StateMachineBehaviour
{
	private Character character;

	private PlayerController playerController;

	private Character enemy;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger(TriggerHash.elementalAttackAnticipation);
		animator.ResetTrigger(TriggerHash.elementalAttackPerform);
		animator.ResetTrigger(TriggerHash.elementalAttackCancel);
		Debug.Log("ElementalAttackAnticipation :: OnStateEnter");
		character = animator.transform.parent.GetComponent<Character>();
		enemy = character.Controller.CurrentEnemy;
		playerController = character.Controller as PlayerController;
		string text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Earth";
		switch (character.elementalPower)
		{
		default:
			text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Earth";
			break;
		case ElementalPower.Electricity:
			text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Electricity";
			break;
		case ElementalPower.Energy:
			text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Energy";
			break;
		case ElementalPower.Fire:
			text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Fire";
			break;
		case ElementalPower.Ice:
			text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Ice";
			break;
		case ElementalPower.Water:
			text = "Prefabs/Characters/ElementalAttacks/FX_Elemental_Water";
			break;
		}
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>(text));
		gameObject.gameObject.name = string.Concat(character.elementalPower, "ElementalFx_", Random.Range(0, 1000));
		ElementalAttack component = gameObject.GetComponent<ElementalAttack>();
		component.Initialize(character, playerController.ElementalCharger);
		component.SetState(ElementalAttack.ElementalAttackState.CHARGING);
		character.EquipWeapon(false);
		playerController.ElementalAttackFx = component;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!character.IsElemetalAttacking())
		{
			playerController.ElementalAttackFx.FadeAway();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (character.isPlayer)
		{
			GamePlayer.Instance.MovementPointer.Hide();
			if (Input.GetMouseButtonDown(0))
			{
				playerController.HandlePicking();
			}
		}
	}
}
