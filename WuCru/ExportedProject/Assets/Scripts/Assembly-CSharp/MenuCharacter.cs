using UnityEngine;

public class MenuCharacter : MonoBehaviour
{
	public Transform headTransform;

	public Transform backTransform;

	public Transform swordTransform;

	public Transform leftHandTransform;

	public Transform rightHandTransform;

	public Transform shoulderTransform;

	public GameObject[] bodyparts;

	public GameObject hairGameObject;

	public GameObject helmetGameObject;

	public GameObject weaponGameObject;

	public int ninjaID;

	public Weapon GetWeapon()
	{
		if (swordTransform == null)
		{
			return null;
		}
		return swordTransform.GetComponentInChildren<Weapon>();
	}

	private void OnDestroy()
	{
		CharacterBuilder.CleanupMenuCharacterTexture(ninjaID);
	}
}
