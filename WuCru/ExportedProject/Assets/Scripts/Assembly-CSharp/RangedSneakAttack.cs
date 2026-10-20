using UnityEngine;

public class RangedSneakAttack : StateMachineBehaviour
{
	private Character character;

	private Character enemy;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("RangedSneakAttack OnStateEnter");
		character = animator.transform.parent.GetComponent<Character>();
		enemy = character.Controller.CurrentEnemy;
		GameObject gameObject = Object.Instantiate(character.GetWeapon().gameObject);
		gameObject.gameObject.name = "ShurikenProjectile";
		gameObject.transform.position = character.GetWeapon().gameObject.transform.position;
		gameObject.transform.rotation = character.transform.rotation;
		gameObject.transform.localScale = gameObject.transform.localScale * 0.5f;
		ShurikenProjectile shurikenProjectile = gameObject.AddComponent<ShurikenProjectile>();
		shurikenProjectile.character = character;
		shurikenProjectile.projectileGO = gameObject;
		shurikenProjectile.speed = 10f;
		shurikenProjectile.target = enemy.transform.position + new Vector3(0f, 0.75f, 0f);
		character.MissilesLeft--;
		if (character.MissilesLeft <= 0)
		{
			(character.Controller as PlayerController).ChangeWeapon(WeaponType.Katana);
			character.RefreshNinjaPortrait();
			character.EquipWeapon(false);
		}
		else
		{
			character.RefreshNinjaPortrait();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		(character.Controller as PlayerController).HasEnemySneakAttackTarget = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
