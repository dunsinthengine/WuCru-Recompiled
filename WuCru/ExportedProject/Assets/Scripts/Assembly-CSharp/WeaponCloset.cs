using UnityEngine;

public class WeaponCloset : InteractableObject
{
	private static int enterCounter;

	public override int PickingPriority()
	{
		return 21;
	}

	protected override void OnEnter(Character character)
	{
		if (character == null)
		{
			Debug.Log("Character is null!");
			return;
		}
		if (character.Controller == null)
		{
			Debug.Log("Character controller is null");
			return;
		}
		enterCounter++;
		character.Controller.ToggleHide(true);
		StartCoroutine(CenterCharacter());
		GamePlayer.Instance.SelectedObject = null;
	}

	protected override void OnExit(Character character)
	{
		enterCounter--;
		if (enterCounter == 0)
		{
			character.Controller.ToggleHide(false);
		}
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		character.Controller.WalkTo(levelNode);
	}
}
